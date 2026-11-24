using System;
using System.IO;
using System.Reflection;
using System.Threading;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace ROTWeatherBoundsFix
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            try
            {
                var harmony = new Harmony("rot.weather.bounds.fix");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Logger.Write(
                    "Weather Bounds Fix V2 installed - NaN protection enabled."
                );
            }
            catch (Exception ex)
            {
                Logger.Write("PATCH INSTALLATION FAILED: " + ex);
            }
        }
    }

    public static class Logger
    {
        public static void Write(string message)
        {
            try
            {
                string path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ROTWeatherBoundsFix.log"
                );

                File.AppendAllText(
                    path,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    " " + message + Environment.NewLine
                );
            }
            catch
            {
            }
        }
    }

    [HarmonyPatch(
        typeof(DefaultMapWeatherModel),
        "GetWeatherEventInPosition"
    )]
    public static class WeatherBoundsPatch
    {
        private const float Epsilon = 0.01f;

        private static int _loggedCorrections = 0;

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) &&
                   !float.IsInfinity(value);
        }

        [HarmonyPrefix]
        public static void Prefix(ref Vec2 pos)
        {
            var campaign = Campaign.Current;

            if (campaign == null ||
                campaign.MapSceneWrapper == null)
                return;

            Vec2 terrain = campaign.MapSceneWrapper.GetTerrainSize();

            // Validate terrain dimensions before using them.

            if (!IsFinite(terrain.X) ||
                !IsFinite(terrain.Y) ||
                terrain.X <= Epsilon ||
                terrain.Y <= Epsilon)
            {
                if (Interlocked.Increment(
                    ref _loggedCorrections) <= 20)
                {
                    Logger.Write(
                        "INVALID TERRAIN SIZE: " +
                        terrain.X + "," + terrain.Y
                    );
                }

                return;
            }

            float oldX = pos.X;
            float oldY = pos.Y;

            // NaN or Infinity: use the terrain center.
            // Otherwise clamp ordinary out-of-bounds coordinates.

            float x = IsFinite(oldX)
                ? Math.Max(0f, Math.Min(oldX, terrain.X - Epsilon))
                : terrain.X * 0.5f;

            float y = IsFinite(oldY)
                ? Math.Max(0f, Math.Min(oldY, terrain.Y - Epsilon))
                : terrain.Y * 0.5f;

            if (x != oldX || y != oldY)
            {
                if (Interlocked.Increment(
                    ref _loggedCorrections) <= 20)
                {
                    Logger.Write(
                        "Weather lookup corrected: " +
                        oldX + "," + oldY +
                        " -> " + x + "," + y +
                        " | Terrain: " +
                        terrain.X + "," + terrain.Y
                    );
                }

                pos = new Vec2(x, y);
            }
        }
    }
}
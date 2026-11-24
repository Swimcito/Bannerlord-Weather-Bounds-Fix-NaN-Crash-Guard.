# Bannerlord Weather Bounds Fix — NaN Crash Guard

A lightweight Harmony compatibility patch for Mount & Blade II: Bannerlord.

## Problem

Some modded campaigns crash during campaign-map simulation with:

System.IndexOutOfRangeException
DefaultMapWeatherModel.GetWeatherEventInPosition()

The crash can occur during mobile-party speed calculations,
including the War Sails naval weather calculation path.

## Fix

The mod intercepts weather lookups and:

- Clamps finite coordinates to valid terrain bounds.
- Detects NaN and Infinity coordinates explicitly.
- Substitutes the terrain center for invalid coordinate components.
- Allows the original weather calculation to continue.
- Logs the first 20 corrections per game session.

The patch changes weather-lookup arguments only.
It does not deliberately move parties or modify saved positions.

## Installation

1. Install and enable Bannerlord Harmony.
2. Extract the release ZIP into the game's Modules directory.
3. Enable ROT Weather Bounds Fix in the launcher.
4. Load your campaign.

Keep the mod enabled if your campaign depends on the workaround.

## Compatibility

Initial successful test:

- Mount & Blade II: Bannerlord 1.4.8
- Harmony 2.4.2.248
- Realm of Thrones 8.1.8
- War Sails

Other game versions and total conversions have not yet been verified.

The module ID remains ROTWeatherBoundsFix for compatibility,
but the patch does not directly depend on Realm of Thrones.

## Diagnostics

Log file:

bin/Win64_Shipping_Client/ROTWeatherBoundsFix.log

Look for the installation message and corrected weather coordinates.

## Limitations

This is a defensive compatibility workaround.

It does not correct the underlying source of invalid map coordinates.
Weather at invalid positions is approximated using a fallback position.

It patches GetWeatherEventInPosition only, not all weather methods.

It is not guaranteed to fix every weather-related crash.

## Credits

Inspired by the Weather Bounds Guard documented by the
Tales from the Age of Men (TAOM) project:

https://github.com/haterade22/TAOM/blob/bannerlord-1.4.5/docs/features/weather-bounds-guard.md

The NaN-specific workaround was developed and tested independently
during troubleshooting of a reproducible Realm of Thrones crash.

## AI disclosure

The C# implementation and project documentation were generated
with assistance from OpenAI ChatGPT. The fix was compiled and
gameplay-tested against a reproducible crash by a human user.
'@

    Set-Content `
        -Path (Join-Path $repo "README.md") `
        -Value $readme -Encoding UTF8

    # Changelog
    $changelog = @"
# Changelog

## $version — Initial public release candidate

- Harmony weather-position interception.
- Out-of-bounds coordinate clamping.
- Explicit NaN and Infinity handling.
- Terrain-center fallback for invalid coordinates.
- Diagnostic logging, limited to 20 corrections per session.
- Matching launcher and DLL assembly versions.
- Standalone module packaging.

Known limitation: only GetWeatherEventInPosition is patched.
"@

    Set-Content `
        -Path (Join-Path $repo "CHANGELOG.md") `
        -Value $changelog -Encoding UTF8

    # Credits
    $credits = @'
# Credits and acknowledgements

Weather Bounds Guard concept:
TAOM contributors — Tales from the Age of Men

https://github.com/haterade22/TAOM

Reference documentation:
https://github.com/haterade22/TAOM/blob/bannerlord-1.4.5/docs/features/weather-bounds-guard.md

TAOM is published under the MIT License.

This standalone implementation adds explicit handling for
NaN and Infinity coordinate inputs.

This is published under the MIT License.

No TaleWorlds, Harmony, or TAOM binary files are redistributed.

# FAQ

## Does the program work only with one specific GTA build?
No. Profile names are fully customizable. You can use Vanilla/Modded, SA-MP/Singleplayer, Original/HD, or any other pair.

## Does it modify gta3.img?
No. It only moves the complete file between folders.

## Why is switching so fast?
The profile folders are created next to the game's `models` folder. On the same drive, moving a file usually changes directory metadata instead of copying all data.

## Can I use more than two profiles?
Not in v1.0. The first public version intentionally uses two profiles to keep file-state detection simple and safe.

## Can I rename the profiles?
Yes. Use **Profile names**.
The corresponding profile folders are renamed too, using a Windows-safe folder name.

## Can I change the interface language?
Yes. Use the RU/EN button in the top-right corner.

## Where are settings stored?
`%AppData%\GTA3 IMG Switcher`

## Where are logs stored?
`%AppData%\GTA3 IMG Switcher\logs\latest.log`

## Are logs sent automatically?
No. Nothing is uploaded automatically.

## What should I attach to a bug report?
Use **Report a problem** and attach `diagnostics.txt` and `latest.log`.

## Does it work with mod loaders?
The switcher only manages `gta3.img`. Other mods and loaders are outside its scope.

## Should GTA be closed?
Yes. The app blocks switching while it detects GTA San Andreas running.

## Why not hash the whole archive?
Large `gta3.img` files made earlier prototypes pause while calculating hashes. The public version uses folder state instead, keeping switching fast.

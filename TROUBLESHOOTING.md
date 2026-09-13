# Troubleshooting

## 1. "Choose the models folder first"
Select the actual `GTA San Andreas\models` folder, not the game root.

## 2. "gta3.img was not found in models"
Check that `models\gta3.img` exists. Do not continue switching until the active game archive is restored.

## 3. "Both profile folders contain gta3.img"
The app cannot know which archive is currently active. Move the duplicate/archive you know is inactive out of one profile folder, then refresh status.

## 4. "No parked file"
One inactive profile must contain `gta3.img`. Put the alternate archive into `gta3_MODDED` or `gta3_VANILLA`.

## 5. GTA is shown as running even after closing it
Open Task Manager and check whether `gta_sa.exe` is still running. End the process only if the game is actually closed.

## 6. Access denied / file is in use
Close GTA, mod tools, IMG editors, antivirus scan windows, launchers, or any program that may hold `gta3.img` open.

## 7. Switching is slow
The profile folders should be next to `models` on the same drive. If you manually moved them to another drive, Windows must copy the whole file.

## 8. The program says "Not responding"
The current public version avoids full-file hashing. If this still occurs, create a diagnostic report and attach the log.

## 9. build.bat says C# compiler not found
Enable/install .NET Framework 4.x or build the source with a compatible C# compiler.

## 10. build.bat shows compiler errors
Make sure you are building the current repository version and did not edit `src\Program.cs` accidentally.

## 11. The wrong profile name is shown
Check the folder state: the active profile's own storage folder should be empty while the other profile contains the parked archive.

## 12. I moved files manually and detection is confused
Restore this invariant:
- `models\gta3.img` exists;
- exactly one profile folder contains `gta3.img`;
- the other profile folder is empty.

Then press **Refresh status**.

## 13. Antivirus warns about the executable
The project is open source and built locally by `build.bat`. Review `src\Program.cs` and build the executable yourself. Do not download binaries from untrusted mirrors.

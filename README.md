<div align="center">

# 🎮 GTA3 IMG Switcher

### Fast, safe and customizable `gta3.img` switching for GTA San Andreas

**Vanilla ↔ Modded · Singleplayer ↔ SA-MP · Original ↔ HD · or any two custom profiles**

[Русский](README_RU.md) · [FAQ](FAQ.md) · [Troubleshooting](TROUBLESHOOTING.md) · [Report a bug](../../issues/new)

</div>

---

## ✨ About

**GTA3 IMG Switcher** is a small Windows utility for people who keep multiple `gta3.img` archives for GTA San Andreas.

Instead of copying a multi-gigabyte archive every time, the program keeps one inactive profile next to the game's `models` folder and swaps files with a fast same-drive move.

### Highlights

- ⚡ Near-instant switching when profiles are on the same drive
- 🎨 Dark UI with smooth animated buttons
- 🇬🇧 English and 🇷🇺 Russian interface
- 🏷️ Custom names for both profiles
- 🔍 Automatic active-profile detection
- 🛡️ Safety checks before every switch
- 🎮 Blocks switching while GTA San Andreas is running
- ↩️ Automatic rollback if the second file move fails
- 🧾 Local technical logs
- 🩺 One-click diagnostic report
- 📂 Quick access to profile folders
- 💾 Settings stored in `%AppData%\GTA3 IMG Switcher`
- 🔒 Nothing is uploaded automatically

## 📁 How it works

If Profile A is active:

```text
GTA San Andreas/
├── models/
│   └── gta3.img
├── gta3_MODDED/
│   └── (empty)
└── gta3_VANILLA/
    └── gta3.img
```

After switching to Profile B:

```text
GTA San Andreas/
├── models/
│   └── gta3.img
├── gta3_MODDED/
│   └── gta3.img
└── gta3_VANILLA/
    └── (empty)
```

The app never edits the contents of `gta3.img`. It only moves the file.

## 🚀 Quick start

1. Build or download `GTA3 IMG Switcher.exe`.
2. Launch it.
3. Select `GTA San Andreas\models`.
4. The app creates `gta3_MODDED` and `gta3_VANILLA`.
5. Put the inactive `gta3.img` into the matching profile folder.
6. Rename the profiles if you want.
7. Switch with one click.

> [!IMPORTANT]
> Exactly one profile folder should contain a parked `gta3.img`. The archive inside `models` is the active one.

## 🧾 Logs and bug reports

The program writes technical logs locally to:

```text
%AppData%\GTA3 IMG Switcher\logs\latest.log
```

Use **Report a problem** to create a diagnostic folder containing:

- `diagnostics.txt`
- `latest.log`

The report folder opens automatically. Diagnostic text is also copied to the clipboard.

**Nothing is sent automatically.** You decide whether to attach the files to a GitHub Issue.


## 📚 Help

- [FAQ](FAQ.md)
- [Troubleshooting](TROUBLESHOOTING.md)
- [Russian FAQ](FAQ_RU.md)
- [Russian Troubleshooting](TROUBLESHOOTING_RU.md)

## 🔨 Build

```text
1. Clone or download the repository
2. Run build.bat
3. Wait for DONE
4. Launch GTA3 IMG Switcher.exe
```

The project intentionally targets the classic .NET Framework compiler included with many Windows systems.

## 💻 Requirements

- Windows 10 or 11
- GTA San Andreas with a standard `models` folder
- .NET Framework 4.x when building from source

## 🔐 Privacy

The app does not use analytics, telemetry, network tracking, or automatic uploads.

Diagnostic reports may contain:

- program version;
- Windows version;
- file-state information;
- profile names;
- selected game path, with the Windows user-profile prefix redacted as `%USERPROFILE%`;
- technical error messages.

Review report files before posting them publicly.

## ⚠️ Disclaimer

This project is independent and is not affiliated with or endorsed by Rockstar Games or Take-Two Interactive.

Always keep backups of important modded files.

## 📄 License

MIT License. See [LICENSE](LICENSE).

---

<div align="center">

Made for the GTA San Andreas modding community ❤️

</div>
<h2 align="center">🌐 Community & Links</h2>

<p align="center">
  <a href="https://youtube.com/@mrremkagit">
    <img src="https://img.shields.io/badge/YouTube-MrRemka-FF0000?style=for-the-badge&logo=youtube&logoColor=white">
  </a>
  <a href="https://t.me/mrremka">
    <img src="https://img.shields.io/badge/Telegram-MrRemka-26A5E4?style=for-the-badge&logo=telegram&logoColor=white">
  </a>
  <a href="https://discord.gg/ejhdRTGjkJ">
    <img src="https://img.shields.io/badge/Discord-Community-5865F2?style=for-the-badge&logo=discord&logoColor=white">
  </a>
</p>

<p align="center">
  Follow the project, report bugs and join the community 💙
</p>

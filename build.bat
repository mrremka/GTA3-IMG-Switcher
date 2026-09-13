@echo off
setlocal

set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe

if not exist "%CSC%" (
  echo C# compiler not found:
  echo %CSC%
  echo.
  echo Install/enable .NET Framework 4.x and try again.
  pause
  exit /b 1
)

echo Building GTA3 IMG Switcher.exe...
"%CSC%" /nologo /target:winexe /optimize+ /win32icon:"assets\app.ico" /out:"GTA3 IMG Switcher.exe" /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll src\Program.cs

if errorlevel 1 (
  echo.
  echo ERROR: Build failed.
  pause
  exit /b 1
)

copy /Y "assets\app.ico" "app.ico" >nul

echo.
echo DONE: GTA3 IMG Switcher.exe
pause
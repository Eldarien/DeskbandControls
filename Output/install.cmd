@echo off
color 0a
cls
title Deskband Controls Setup
echo[
echo    DESKBAND CONTROLS
echo =======================
echo[
echo Welcome to the Deskband Controls installer.

rem Check permissions
fsutil dirty query %systemdrive% >nul
if %errorLevel% == 0 (
    echo Administrative permissions confirmed.
) else (
    echo Please run this script with administrator permissions.
    goto EXIT
)

rem Search for existing installation
for /f "tokens=2*" %%a in ('reg query "HKEY_CLASSES_ROOT\CLSID\{9690ED28-CD24-4534-B380-77103A4E7774}\InprocServer32" /v CodeBase 2^>^&1^|find "REG_"') do @set fn=%%b
if not defined fn (
  goto INSTALL
)

:UNINSTALL
set fn=%fn:~8%
echo Existing installation found. Uninstalling...
if defined ProgramFiles(x86) (
  %SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe /nologo /unregister "%fn%"
) else (
  %SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm.exe /nologo /unregister "%fn%"
)
if ERRORLEVEL 1 (
  echo Unable to unregister deskband DLL. Please make sure that .NET framework 4 is installed.
  goto EXIT
)
%SystemRoot%\System32\taskkill.exe /F /IM explorer.exe >nul
timeout 5 /NOBREAK >nul
echo Uninstall completed.
echo[
echo[

:INSTALL
set dc=%ProgramFiles%\DeskbandControls
echo Installing to "%dc%"...
if exist "%dc%" (
  rd /S /Q "%dc%"
)
mkdir "%dc%"
xcopy /Q "%~dp0Release" "%dc%"
xcopy /Q "%~dp0uninstall.cmd" "%dc%"

if defined ProgramFiles(x86) (
   %SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe /nologo /codebase "%dc%\Deskband.dll"
) else (
   %SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm.exe /nologo /codebase "%dc%\Deskband.dll"
)
if ERRORLEVEL 1 (
  echo Unable to register deskband DLL. Please make sure that .NET framework 4 is installed.
  goto EXIT
)

echo Installation completed.
echo To enable deskband right-click on taskbar and select "Deskband Controls" from Toolbars submenu and play some music.
echo[
echo Greetings from Ukraine!
echo Enjoy!

:EXIT
echo[
echo[
pause

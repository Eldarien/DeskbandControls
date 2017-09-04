@echo off
color 0a
cls
title Deskband Controls Uninstall
echo[
echo    DESKBAND CONTROLS
echo =======================
echo[
echo Welcome to the Deskband Controls uninstaller.
echo Administrative permissions required. Detecting permissions...

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
  goto CLEANUP
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

:CLEANUP
set dc=%ProgramFiles%\DeskbandControls
if exist "%dc%" (
  rd /S /Q "%dc%"
)
echo Uninstall completed.
echo Explorer.exe process was terminated.
echo Press Ctrl+Shift+ESC to launch task manager and run explorer.exe from "File - New Task (Run)..." menu.

:EXIT
echo[
echo[
pause

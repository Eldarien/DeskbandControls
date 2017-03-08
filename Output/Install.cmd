@echo off
echo Welcome to the Deskband Controls installer.
echo This will try to uninstall previous version and install a new one.
echo Windows shell (explorer.exe) may be restarted.
pause

for /f "tokens=2*" %%a in ('reg query "HKEY_CLASSES_ROOT\CLSID\{9690ED28-CD24-4534-B380-77103A4E7774}\InprocServer32" /v CodeBase 2^>^&1^|find "REG_"') do @set fn=%%b
if not defined fn (
  goto INSTALL
)

:UNINSTALL
echo Uninstalling...
set fn=%fn:~8%
echo Found previous version at %fn%
if defined ProgramFiles(x86) (
  %SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe /unregister "%fn%"
) else (
  %SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm.exe /unregister "%fn%"
)
%SystemRoot%\System32\taskkill.exe /F /IM explorer.exe
timeout 5 /NOBREAK

:INSTALL
echo Installing...
set dc=%SystemDrive%\DeskbandControls
if exist "%dc%" (
  rd /S /Q %dc%
)
mkdir "%dc%"
xcopy "%~dp0Release" "%dc%"
if defined ProgramFiles(x86) (
   %SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe /codebase "%dc%\Deskband.dll"
) else (
   %SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm.exe /codebase "%dc%\Deskband.dll"
)

echo Installation finished.
pause

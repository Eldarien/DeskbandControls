@echo off
if defined ProgramFiles(x86) (
    %SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe /unregister "%~dp0Release\Deskband.dll"
) else (
    %SystemRoot%\Microsoft.NET\Framework\v4.0.30319\regasm.exe /unregister "%~dp0Release\Deskband.dll"
)    
pause

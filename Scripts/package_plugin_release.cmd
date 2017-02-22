@echo off
set root=%~dp0
set release=%root%..\DeskbandPlugin\Release

copy /Y "%release%\DeskbandPlugin.dll" "%root%foo_deskband_controls.dll"
"%root%..\Tools\7za.exe" a -mx9 -tzip "%root%..\Output\Deskband Controls.fb2k-component" "%root%foo_deskband_controls.dll"
del "%root%foo_deskband_controls.dll"
exit

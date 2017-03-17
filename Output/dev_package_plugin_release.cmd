@echo off
set root=%~dp0
set release=%root%..\DeskbandPlugin\Release
set arc=%root%..\Output\foo_deskband_controls.fb2k-component

del "%arc%
copy /Y "%release%\DeskbandPlugin.dll" "%root%foo_deskband_controls.dll"
"%root%..\Tools\7za.exe" a -mx9 -tzip "%arc%" "%root%foo_deskband_controls.dll" "%root%..\Output\Release" "%root%..\Output\install.cmd" "%root%..\Output\uninstall.cmd" "%root%..\Output\!readme.txt"
del "%root%foo_deskband_controls.dll"
exit

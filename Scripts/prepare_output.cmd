set root=%~dp0

copy /Y "%root%..\DeskbandPlugin\Release\DeskbandPlugin.dll" "%root%foo_deskband_controls.dll"
"%root%..\Tools\7za.exe" a -mx9 -tzip "%root%..\Output\Deskband Controls.fb2k-component" "%root%foo_deskband_controls.dll"
del "%root%foo_deskband_controls.dll"

del /Q "%root%..\Output\Bin\*.*"
copy "%root%..\Deskband\bin\Release\*.dll" "%root%..\Output\Bin"
pause
exit

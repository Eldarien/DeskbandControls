@echo off
set tasklist=%windir%\System32\tasklist.exe
set taskkill=%windir%\System32\taskkill.exe
set root=%~dp0
set release=%root%..\DeskbandPlugin\Release
goto :MAIN

-------------------------------------------------------
:STOPPROC
    set wasStopped=0
    set procFound=0
    set notFound_result=ERROR:
    set procName=%1
    for /f "usebackq" %%A in (`%taskkill% /IM %procName%`) do (
      if NOT %%A==%notFound_result% (set procFound=1)
    )
    if %procFound%==0 (
      echo The process was not running.
      goto :EOF
    )
    set wasStopped=1
    set ignore_result=INFO:
:CHECKDEAD
    "%windir%\system32\timeout.exe" 3 /NOBREAK
    for /f "usebackq" %%A in (`%tasklist% /nh /fi "imagename eq %procName%"`) do (
      if not %%A==%ignore_result% (goto :CHECKDEAD)
    )
    goto :EOF
-------------------------------------------------------

:MAIN 
call :STOPPROC foobar2000.exe

if exist "%AppData%\foobar2000\user-components\foo_deskband_controls\nul" ( echo "" ) else ( mkdir "%AppData%\foobar2000\user-components\foo_deskband_controls" )
copy /Y "%release%\DeskbandPlugin.dll" "%AppData%\foobar2000\user-components\foo_deskband_controls\foo_deskband_controls.dll"
exit

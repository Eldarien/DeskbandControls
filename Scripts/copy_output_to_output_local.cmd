set tasklist=%windir%\System32\tasklist.exe
set taskkill=%windir%\System32\taskkill.exe
set root=%~dp0

goto :MAIN

-------------------------------------------------------
:STOPPROC
    set wasStopped=0
    set procFound=0
    set notFound_result=ERROR:
    set procName=%1
    for /f "usebackq" %%A in (`%taskkill% /IM %procName% /F`) do (
      if NOT %%A==%notFound_result% (set procFound=1)
    )
    if %procFound%==0 (
      goto :NOSTOP
    )
    set wasStopped=1
    set ignore_result=INFO:
:CHECKDEAD
    "%windir%\system32\timeout.exe" 3 /NOBREAK
    for /f "usebackq" %%A in (`%tasklist% /nh /fi "imagename eq %procName%"`) do (
      if not %%A==%ignore_result% (goto :CHECKDEAD)
    )
    goto :NOSTOP
    
-------------------------------------------------------
:MAIN
echo Stopping explorer.exe process...
call :STOPPROC explorer.exe

:NOSTOP
del /Q "%root%..\OutputLocal\Bin\"
xcopy /E "%root%..\Output\Bin" "%root%..\OutputLocal\Bin\"
start explorer.exe
pause
exit

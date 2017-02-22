Foobar 2000 Deskband Controls 3.0.0
-----------------------------------

HydrogenAudio forum topic: https://hydrogenaud.io/index.php/topic,78234.0.html

Important notes:

! - Please uninstall previous version before installing this one (see below).

! - Windows sometimes marks files downloaded from Internet as unsafe and this causes various glitches in downloaded software.
!   So, don't forget to unblock downloaded zip - right click it, select "Properties" and then select "Unblock" in properties window.

!- If you are using Windows 10 insider builds deskband may not work. This can be fixed by editing registry:
!- delete HKEY_CLASSES_ROOT\CLSID\{668863E6-D688-4115-8F23-BB7F37621A74} key in the registry.
!- You will have to take ownership and give yourself full access.
!- For those wondering what the hell it is - looks like unfinished people band...

How to install:
  - Install Microsoft .NET Framework 4.
      https://www.microsoft.com/en-us/download/details.aspx?id=17718
    If you are using Windows 7 with Windows Updates turned on you may already have it.
    Windows 8/8.1/10 should have it installed by default.
  - Right-click "Register Deskband.cmd" file and select "Run as administrator".
    A console window should appear with the following message:
      "Types registered successfully".
  - Right-click on taskbar and select "Deskband Controls" from Toolbars submenu.
  - Open foobar2000, go to Preferences - Components page and click "Intstall..." button.
    Select "Deskband Controls.fb2k-component" and restart foobar2000.
  - Right-click deskband and select "Settings" to configure plugin.

How to uninstall:
  - Deselect "Deskband Controls" from Toolbars submenu in taskbar.
    Right-click "Unregister Deskband.cmd" file and select "Run as administrator".
    A console window should appear with the following message:
      "Types un-registered successfully".
  - If you want to replace Deskband.dll you need to restart explorer.exe process. 
    It can be killed in task manager and started from there again. (File - New Task - explorer.exe)
  - Open foobar2000, go to Preferences - Components page, right click "Deskband Controls" entry and select "Remove".

--
Greetings from Ukraine!
Eldarien (eldarien@live.com)

--
** Changelog:

February 22 2017
  * Version 3.0.0
  - Windows XP is no longer supported.
  + New configuration system and UI.
  + Debug console.
  + Text smooth scroll
  + Mouse wheel volume control (when hovering on the toolbar, mouse scroll changes volume).
  + Do not blink when changing tracks and "hide is not playing" is enabled.
  + Stop after current checkmark in menu.
  + Live reload when config file changes.
  + Dragging the scrollbar does not change the playback until you release the mouse button, (as in fb2k main window).
  + Paused format option.
  + Reset the scroll position when change to next track.
  + Deskband now returns the focus to the last active window after receiving some input.
  + A tooltip window with configurable texts and album art when you mouse over the deskband.

October 8 2014
  * Version 1.1.2
  - Fix for taskbar not appearing in auto-hide mode when cursor is placed over deskband controls

October 2 2014
  * Version 1.1.1
  - Fix crush for some album art data

April 10 2014
  * Version 1.1
  - Default button colorization color set to transparent (a bug when at first start buttons were black)
  - Fix a crush when using alpha channel in floating window background color. Use Opacity setting instead.
  - Fix text not drawing in WindowsXP
  + Album Art Preserve aspect ratio option
  + Added link to hydrogenaudio forum topic in foobar plugin about window
  + Hide trackbar borders option
  + Trackbars background color support

February 13 2014
  * Version 1.0.2
  - Minor fixes in UI and startup when autohide is activated
  - Fonts can be set as small as size "1"

December 6 2013
  * Version 1.0.1
  - Fix for deskband always shows up regardless of "Hide if foobar2000 is not running" option
  + Support for alpha channel on trackbars and text blocks
  + "Do not show stub image" option

November 27 2013:
  * Version 1.0.0
  + Hide if foobar2000 is not running option
  + Show album art stub image on stop
  + mouse drag support in trackbars
  + ability to colorize button icons

November 7 2013:
  * Version 1.0.0 beta
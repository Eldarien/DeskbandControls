Foobar 2000 Deskband Controls 1.0.1
-----------------------------------

Hydrogen audio forum topic: http://www.hydrogenaudio.org/forums/index.php?showtopic=78234

How to install:
  - Please uninstall previous version before installing this one (see below).
  - Install Microsoft .NET Framework 4 Client Profile.
      http://www.microsoft.com/en-us/download/details.aspx?id=24872
    If you are using Windows 7 with Windows Updates turned on you may already have it.
    Windows 8 and 8.1 have it installed by default.
  - Windows sometimes marks files downloaded from Internet as unsafe and this causes various glitches in downloaded software.
    So, don't forget to unblock downloaded zip - right click it, select "Properties" and then select "Unblock" in properties window.
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
Eldarien (eldarien@live.com)


--
** Changelog:

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
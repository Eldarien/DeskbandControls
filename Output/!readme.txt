 Deskband Controls
-------------------

HydrogenAudio forum topic: https://hydrogenaud.io/index.php/topic,78234.0.html

Important notes:

!- If you are using Windows 10 insider builds deskband may not work. This can be fixed by editing registry:
!- delete HKEY_CLASSES_ROOT\CLSID\{668863E6-D688-4115-8F23-BB7F37621A74} key in the registry.
!- You will have to take ownership and give yourself full access.
!- For those wondering what the hell it is - looks like unfinished people band...

How to install:
  - Install Microsoft .NET Framework 4: https://www.microsoft.com/en-us/download/details.aspx?id=17718
    If you are using Windows 7 with Windows Updates turned on you may already have it.
    Windows 8/8.1/10 should have it installed by default.
  - Double click on foo_deskband_controls.fb2k-component, foobar2000 will lauch and ask if you want to install. Click 'Yes'.
  - Installer window will launch. If everything works as intened a success message will appear. Press enter to return to foobar2000.
  - Right-click on taskbar and select "Deskband Controls" from Toolbars submenu.

How to uninstall:
  - Open foobar2000, go to Preferences - Components page, right click "Deskband Controls" entry and select "Remove".
  - Deselect "Deskband Controls" from Toolbars submenu in taskbar.
  - Navigate to %ProgramFiles%/DeskbandControls, right-click on "uninstall.cmd" file and select "Run as administrator".
    Uninstaller window will launch. If everything works as intened a success message will appear.
    Explorer.exe process will be terminated.
    Press Ctrl+Shift+ESC to launch task manager and run explorer.exe from "File - New Task (Run)..." menu.

--
Greetings from Ukraine!
Eldarien (eldarien@live.com)

--
** Changelog:

May 09 2017
  * Version 3.3.0
  + Added playlist view in context menu.
  - Minor bugfixes.

March 26 2017
  * Version 3.2.0
  + Added tooltip delay option.
  + Added keep tooltip open on hover option.
  - Fixed tooltip crush when background color contains alpha channel.
  - Fixed issue with trackbar background color transparency.
  - Fixed issue where trackbar position could get less than 0.

March 21 2017
  * Version 3.1.0
  + New installer/uninstaller system. Installer is integrated into foobar2000 plugin.
  + Deskband Controls is now distributed from official plugins repository.
  + Context menu settings - ability to select context menu items that shold be visible.
  - Fixed issues with tooltip window positioning and tooltip crush on Win10.

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
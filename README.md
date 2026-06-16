
# DeskbandControls

A Windows taskbar **Deskband plugin framework for foobar2000 controls and media widgets**, providing playback control, album art, playlist preview, and optional floating window mode.

Originally developed for classic Windows taskbar toolbars (Deskband API), DeskbandControls integrates tightly with foobar2000 and exposes a modular UI system for building taskbar-resident media widgets.

See this forum thread for more details: https://hydrogenaudio.org/index.php/topic,78234.0.html

---

## Overview

DeskbandControls is a .NET-based Windows deskband application that hosts modular UI components for controlling and visualizing foobar2000 playback.

It provides:

- Taskbar-embedded media controls (play/pause/next/prev)
- Album art display
- Playlist context preview
- Optional peak meter visualization
- Floating window mode (alternative to taskbar docking)
- Configurable layout and module system

---

## Features

### Core functionality
- foobar2000 integration via plugin bridge (`dcmFoobar2000`)
- Real-time playback control (play, pause, stop, next, previous)
- Track metadata display (artist, title, etc.)
- Album art rendering (front cover or stub fallback)

### UI modules
- Album Art Module
- Playlist Preview Module
- Control Buttons Module
- Optional Peak Meter Module (depending on configuration)

### Display modes
- Windows taskbar Deskband mode
- Floating window mode
- Horizontal/compact layouts

### Configuration-driven
All layout and behavior is controlled via:

```
%APPDATA%\\DeskbandControls\\DeskbandControls.json
```

---

## Architecture

DeskbandControls is built using:

- **C# (.NET Framework 4.x WinForms)**
- Windows Shell Deskband API (COM-based integration)
- Dependency Injection via **Ninject**
- JSON configuration via **Newtonsoft.Json**
- foobar2000 plugin bridge (`dcmFoobar2000`)

### High-level design

```
Windows Explorer (Taskbar Host)
        ↓
Deskband COM Host (Explorer toolbar process)
        ↓
DeskbandControls .NET assembly
        ↓
Module system (UI widgets)
        ↓
dcmFoobar2000 bridge
        ↓
foobar2000 core
```

Modules are dynamically composed and rendered inside a single deskband container.

---

## Installation

1. Install foobar2000
2. Install DeskbandControls package
3. Register/select DeskbandControls in Windows taskbar toolbars:
   - Right click taskbar → Toolbars → DeskbandControls

---

## Configuration

Main configuration file:

```
%APPDATA%\\DeskbandControls\\DeskbandControls.json
```

---

## Windows 10 / Windows 11 Compatibility

### Windows 10
DeskbandControls generally works, but behavior depends on Explorer shell state:

- Taskbar toolbar support is partially restricted in newer builds
- Explorer restarts may be required after installation or configuration changes

### Windows 11
Windows 11 introduces significant limitations:

- Classic taskbar toolbar/deskband support is **removed**
- Deskband-based applications may not appear in the taskbar at all
- Floating window mode is often the only usable alternative

### Practical implications
- Deskband mode is legacy and not present on Windows 11
- Floating window mode is possible but requires a project redesign, specifically it can not be hosted as a COM deskband object any more.

---

## Known Issues

### Album art not showing
- Often caused by missing embedded front cover metadata
- Some files only contain generic or icon-based art
- Stub image is displayed as fallback

### UI not updating after button clicks
- Make sure foobar2000 is not running as administrator (can not control elevated process)

### Configuration corruption
- Deleting or resetting `DeskbandControls.json` resets UI state

---

## Troubleshooting

### Deskband not visible in taskbar
- Restart Explorer
- Ensure toolbar is enabled in taskbar context menu
- Re-register or reinstall component

### Controls work but UI does not refresh
- Restart Explorer process
- Check configuration JSON validity

### Album art stuck on stub image
- Verify foobar2000 metadata includes embedded artwork
- Check AlbumArt.Visible setting

### Settings not applying
- Ensure JSON file is writable
- Restart application / Explorer after changes

---

## FAQ

### Why is space in the taskbar wasted when peak meter is enabled?
The layout is fixed-width in deskband mode. Enabling/disabling modules does not always trigger automatic resizing.

### Can DeskbandControls be used on Windows 11?
Not reliably. Windows 11 removed or restricted classic deskband support. Floating window mode is the primary alternative.

### Does it work without foobar2000?
No. It is tightly coupled to foobar2000 via the dcmFoobar2000 bridge.


---

## Development

### Tech stack
- C# (.NET Framework 4.x)
- Windows Forms
- COM Deskband API
- Ninject (DI container)
- Newtonsoft.Json
- foobar2000 SDK bridge

### Solution structure (conceptual)

- Deskband (UI host)
- Deskband.Core (module system)
- dcmFoobar2000 (audio bridge)
- Modules (album art, playlist, controls)

---

## Build notes

- Requires .NET Framework 4.x
- Must be compiled for x86/x64 depending on Explorer host
- Registration required for COM deskband component
- foobar2000 plugin must match architecture

See Output directory for details.

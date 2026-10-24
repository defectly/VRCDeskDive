# VRCDeskDive

**English** | [日本語](README.ja.md)

Switch between VR controls and desktop controls without closing VRChat.

VRChat stays in VR mode, but you can use your keyboard and mouse after taking off your headset. Movement, jumping, and other actions use VRChat's OSC input; looking around uses a SteamVR driver that overrides the headset's pose.

This is the English/Japanese localization [fork](https://github.com/defectly/VRCDeskDive) of [gurenchang/VRCDeskDive](https://github.com/gurenchang/VRCDeskDive). The original project and its MIT copyright notice are retained.

## Requirements

- Windows 10 / 11. The original project reports testing with Windows 10 + Quest 3 + Steam Link.
- [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) to run the application.
- SteamVR.
- OSC enabled in VRChat: Action Menu → Options → OSC.

If your headset goes to sleep when you take it off, the original project's workaround is to cover its proximity sensor with tape or a similar material.

## Build and run

To build from source, install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), not just the runtime. The native SteamVR driver also requires Visual Studio / Visual Studio Build Tools with the C++ x64 tools, or [zig](https://ziglang.org/). The build script tries Visual Studio first, then `zig` on `PATH`, then `tools\zig\zig.exe`.

Build the SteamVR driver (C) first, then the application. Run these commands in PowerShell from the repository root:

```powershell
powershell -ExecutionPolicy Bypass -File driver\build.ps1
dotnet build -c Release
.\bin\Release\net10.0-windows\VRCDeskDive.exe
```

On the first run, use the button to register the driver with SteamVR, then restart SteamVR.

- The registered folder is the copy in the application output directory: `bin\Release\net10.0-windows\driver\vrcdeskdive`. Register it again if you move that folder.
- Keep the application and its `driver` subfolder together; copying only the `.exe` does not include the driver.
- Close SteamVR before rebuilding the driver, because it keeps the driver's DLL in use.

## Language

The application supports **English** and **日本語**. Choose either language in the settings language selector. Your choice is saved immediately and applies without restarting, including the main window, status messages, look pad, and an already-open chat window.

The saved setting is `Language` in `%APPDATA%\VRCDeskDive\settings.json`, with the value `"en"` or `"ja"`. If it is unset or unsupported, a Japanese system language selects Japanese; all other system languages select English. Existing settings files continue to work.

The English and Japanese READMEs describe the same setup and controls. This fork adds persistent language selection and complete English interface text while retaining Japanese; build-script messages and diagnostic logs are in English.

## Usage

Use the main window's button to switch between VR controls and desktop controls. Switch to desktop controls while still wearing the headset, then take it off to continue from the same head position and heading.

- Title-bar buttons toggle always-on-top and compact mode; these choices are remembered for the next launch.
- Closing the window exits the application.

### Desktop controls

Keyboard controls and mouse-look activation work only while VRChat or this tool's window is in the foreground. The tool's look pad can also control the view directly.

| Control | Action |
| --- | --- |
| W A S D | Move |
| Left-click inside the VRChat window | Start mouse look: mouse movement directly controls the view |
| Alt (can be changed to Tab / Ctrl in settings) | Release mouse look; switching away from VRChat also releases it |
| Right-click and drag on the tool's look pad | Turn the view like an analog stick |
| Middle mouse button | Level the view |
| Shift | Run |
| Space | Jump |
| V | Microphone |
| Enter | Open chat-box input; Enter sends and Esc cancels |

Outside chat-box input, Esc passes through to VRChat, so it can open VRChat's menu. Adjust mouse-look speed with the mouse sensitivity setting.

## How it works and limitations

```text
VRCDeskDive ── OSC (127.0.0.1:9000) ──► VRChat: movement, jump, run, microphone, chat
     │
     └── shared memory ──► SteamVR driver (inside vrserver): override the HMD pose
```

- This is not a separate VRChat desktop session: VRChat remains in VR mode and SteamVR is required for the view override.
- The driver changes only the headset pose. It does not modify the playspace.
- In desktop mode, the view is leveled while keeping the head position and heading captured when switching modes. Putting the headset on your desk does not change your view height.
- Switching back to VR stops the override and restores the real headset pose.
- If the application crashes, the driver stops overriding the pose after 1.5 seconds.
- The application does not modify VRChat itself.

## Troubleshooting

- **The application does not start:** install the .NET 10 **Desktop** Runtime. Building additionally requires the .NET 10 SDK.
- **No C compiler is found:** install Visual Studio Build Tools with the C++ x64 tools, or make `zig.exe` available on `PATH` / at `tools\zig\zig.exe`. Build the driver before building the application.
- **The driver is missing or not connected:** check that the built application has `driver\vrcdeskdive\bin\win64\driver_vrcdeskdive.dll`, register the driver from the application, and restart SteamVR. Register again after moving the application folder. Keep the headset awake and connected.
- **The driver DLL cannot be overwritten:** close SteamVR before rebuilding it, then rebuild the application to copy the updated driver into its output directory.
- **Movement or chat does not respond:** enable OSC in VRChat and bring VRChat or the tool to the foreground. The default OSC destination is `127.0.0.1:9000`; `OscHost` and `OscPort` are stored in the settings file.
- **Mouse look does not start or release as expected:** check the click-to-mouse-look setting and selected release key. Mouse look starts by left-clicking inside VRChat and releases when switching away from it.
- **Further diagnosis:** inspect `%APPDATA%\VRCDeskDive\debug.log`. Copy it before restarting the application, because each launch replaces it.

## File locations

| File | Contents |
| --- | --- |
| `%APPDATA%\VRCDeskDive\settings.json` | Application settings, including the language preference |
| `%APPDATA%\VRCDeskDive\debug.log` | Diagnostic log; recreated on each launch |

## Unregister the driver

Close SteamVR, then run the following in PowerShell. Adjust the SteamVR installation path and replace `<build-output>` with the application output directory:

```powershell
& "C:\Program Files (x86)\Steam\steamapps\common\SteamVR\bin\win64\vrpathreg.exe" removedriver "<build-output>\driver\vrcdeskdive"
```

## License

[MIT License](LICENSE). Original copyright: © 2026 gurenchang.

## Third-party components

- [OpenVR SDK](https://github.com/ValveSoftware/openvr) (BSD-3-Clause): `ThirdParty/OpenVR/`
- [Rajdhani](https://fonts.google.com/specimen/Rajdhani) (SIL Open Font License 1.1): `Fonts/`

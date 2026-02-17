# CameraInspector - Developer Documentation

## Overview

CameraInspector is a Windows Forms application (targeting .NET 8.0) designed to discover and inspect Zivid and Intel RealSense cameras. It provides a unified interface to view internal parameters, intrinsics, and stream profiles.

## Architecture

The solution follows a clean architecture with a Core library defining abstractions and separate Provider libraries for each camera vendor.

### Solution Structure

- **CameraInspector.Core**: Defines `ICameraProvider`, `CameraDevice`, `CameraDetails` models and interfaces.
- **CameraInspector.Providers.Zivid**: Implements Zivid support using `Zivid.NET`.
- **CameraInspector.Providers.RealSense**: Implements RealSense support using `Intel.RealSense`.
- **CameraInspector.Providers.Generic**: Fallback provider for generic webcams.
- **CameraInspector.UI**: Windows Forms application.
- **CameraInspector.TestRunner**: Console application for verifying logic without UI.

### Discovery Mechanism

Each provider implements `ICameraProvider.EnumerateAsync()`:

1.  **Zivid**: Uses `Zivid.NET.Application.Cameras` to list connected devices.
2.  **RealSense**: Uses `Intel.RealSense.Context.QueryDevices()` to enumerate USB devices via librealsense.
3.  **Generic**: (Simulated) Returns a default webcam entry.

## Parameters Collected

The application normalizes data into `CameraDetails` structure:

- **Basic Info**: Vendor, Model, Serial, Firmware, SDK Version.
- **Intrinsics**: Focal length, Principal point, Distortion models (per stream/sensor).
- **Stream Profiles**: Resolutions, Formats, FPS.
- **Settings**: Acquisition settings (Aperture, Exposure) for Zivid; Sensor options for RealSense.
- **Diagnostics**: Temperature, Status.

## Build Instructions

### Prerequisites

- .NET 8.0 SDK
- Windows OS (for running the UI) or Linux (for building/testing console runner)

### Building

```bash
dotnet restore
dotnet build
```

### Running

**UI Application:**
```bash
dotnet run --project CameraInspector.UI/CameraInspector.UI.csproj
```

**Test Runner (Console):**
```bash
dotnet run --project CameraInspector.TestRunner/CameraInspector.TestRunner.csproj
```

## SDK Requirements & Stubs

**Important:** This project currently uses **Stub/Mock** implementations for `Zivid.NET` and `Intel.RealSense` to allow compilation and testing in environments without the proprietary SDKs installed.

### Deploying with Real SDKs

To use with real hardware:

1.  **Zivid**:
    - Install Zivid SDK from [zivid.com](https://www.zivid.com/downloads).
    - Remove `CameraInspector.Providers.Zivid/Stubs/ZividStub.cs`.
    - Add reference to the official `Zivid.NET.dll`.

2.  **RealSense**:
    - Install Intel RealSense SDK 2.0.
    - Remove `CameraInspector.Providers.RealSense/Stubs/RealSenseStub.cs`.
    - Ensure `Intel.RealSense.dll` and `realsense2.dll` are in the output directory.

## Known Limitations

- The current Generic provider is a simulation. For real webcam support, integrate `DirectShowLib` or `MediaFoundation`.
- Real-time streaming is not implemented (only metadata inspection).

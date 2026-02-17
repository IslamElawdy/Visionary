using System;
using System.Collections.Generic;

namespace Zivid.NET;

// Stub for Zivid.NET.Application
public class Application : IDisposable
{
    public List<Camera> Cameras { get; } = new List<Camera>
    {
        new Camera { SerialNumber = "2024ZV001", Model = "Zivid 2 Plus", FirmwareVersion = "2.10.1" },
        new Camera { SerialNumber = "2024ZV002", Model = "Zivid One+", FirmwareVersion = "2.9.0" }
    };

    public void Dispose() { }
}

// Stub for Zivid.NET.Camera
public class Camera
{
    public string SerialNumber { get; set; } = "";
    public string Model { get; set; } = "";
    public string FirmwareVersion { get; set; } = "";

    public Settings Settings { get; set; } = new Settings();

    public void Connect() { }
    public void Disconnect() { }
}

// Stub for Zivid.NET.Settings
public class Settings
{
    public Acquisition Acquisition { get; set; } = new();
    public Processing Processing { get; set; } = new();
}

public class Acquisition
{
    public double Aperture { get; set; } = 5.6;
    public double ExposureTime { get; set; } = 10000; // microseconds
    public double Brightness { get; set; } = 1.0;
}

public class Processing
{
    public Filters Filters { get; set; } = new();
}

public class Filters
{
    public double ContrastDistortion { get; set; } = 0.5;
    public bool OutlierRemoval { get; set; } = true;
    public Experimental Experimental { get; set; } = new();
}

public class Experimental
{
    public bool GpuAccelerated { get; set; } = true;
}

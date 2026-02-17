using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Intel.RealSense;

public class Context : IDisposable
{
    public DeviceList QueryDevices()
    {
        return new DeviceList();
    }
    public void Dispose() { }
}

public class DeviceList : IEnumerable<Device>, IDisposable
{
    private List<Device> _devices = new List<Device>
    {
        new Device("Intel RealSense D435", "814412071069", "5.13.0.50"),
        new Device("Intel RealSense D415", "943222072188", "5.12.7.100")
    };

    public IEnumerator<Device> GetEnumerator() => _devices.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _devices.GetEnumerator();
    public int Count => _devices.Count;
    public Device this[int index] => _devices[index];
    public void Dispose() { }
}

public class Device : IDisposable
{
    public InfoCollection Info { get; }

    public Device(string name, string serial, string firmware)
    {
        var data = new Dictionary<CameraInfo, string>
        {
            { CameraInfo.Name, name },
            { CameraInfo.SerialNumber, serial },
            { CameraInfo.FirmwareVersion, firmware },
            { CameraInfo.RecommendedFirmwareVersion, firmware },
            { CameraInfo.PhysicalPort, "USB-C" },
            { CameraInfo.DebugOpCode, "Yes" },
            { CameraInfo.AdvancedMode, "Yes" },
            { CameraInfo.ProductId, "0x0B07" },
            { CameraInfo.CameraLocked, "No" },
            { CameraInfo.UsbTypeDescriptor, "3.2" },
            { CameraInfo.ProductLine, "D400" },
            { CameraInfo.AsicSerialNumber, "123456789" },
            { CameraInfo.FirmwareUpdateId, "1" }
        };
        Info = new InfoCollection(data);
    }

    public List<Sensor> Sensors { get; } = new List<Sensor>
    {
        new Sensor("Stereo Module") {
            StreamProfiles = new List<StreamProfile> {
                new VideoStreamProfile(1280, 720, 30, Stream.Depth, Format.Z16),
                new VideoStreamProfile(848, 480, 60, Stream.Depth, Format.Z16),
                new VideoStreamProfile(1280, 720, 30, Stream.Infrared, Format.Y8),
            }
        },
        new Sensor("RGB Camera") {
            StreamProfiles = new List<StreamProfile> {
                new VideoStreamProfile(1920, 1080, 30, Stream.Color, Format.Rgb8),
                new VideoStreamProfile(1280, 720, 60, Stream.Color, Format.Rgb8)
            }
        }
    };

    public void Dispose() { }
}

public class InfoCollection
{
    private Dictionary<CameraInfo, string> _data;
    public InfoCollection(Dictionary<CameraInfo, string> data) { _data = data; }
    public string this[CameraInfo key] => _data.TryGetValue(key, out var v) ? v : "Unknown";
    public bool Supports(CameraInfo key) => _data.ContainsKey(key);
}

public enum CameraInfo
{
    Name,
    SerialNumber,
    FirmwareVersion,
    RecommendedFirmwareVersion,
    PhysicalPort,
    DebugOpCode,
    AdvancedMode,
    ProductId,
    CameraLocked,
    UsbTypeDescriptor,
    ProductLine,
    AsicSerialNumber,
    FirmwareUpdateId
}

public class Sensor
{
    public string Name { get; set; }
    public List<StreamProfile> StreamProfiles { get; set; } = new();

    public Sensor(string name) { Name = name; }
    public bool Is(Extension extension) => true;
}

public enum Extension { DepthSensor, ColorSensor, MotionSensor, FisheyeSensor, AdvancedMode, Record, Playback, VideoProfile, Roi, DepthStereoSensor, DisparityFilter, DecimationFilter, SpatialFilter, TemporalFilter, HoleFillingFilter, ZeroOrderFilter, HdrMerge, SequenceIdFilter, ThresholdFilter, UnitsTransform, DenoiseFilter }

public class StreamProfile
{
    public Stream Stream { get; set; }
    public Format Format { get; set; }
    public int Framerate { get; set; }
    public int Index { get; set; }
    public int UniqueId { get; set; }

    public StreamProfile(int fps, Stream s, Format f)
    {
        Framerate = fps; Stream = s; Format = f;
    }

    public Extrinsics GetExtrinsicsTo(StreamProfile other)
    {
        // Simple mock: identity if same, fixed offset if different
        if (this == other) return new Extrinsics { rotation = new float[]{1,0,0,0,1,0,0,0,1}, translation = new float[]{0,0,0} };
        return new Extrinsics { rotation = new float[]{1,0,0,0,1,0,0,0,1}, translation = new float[]{0.015f, 0, 0} };
    }
}

public class VideoStreamProfile : StreamProfile
{
    public int Width { get; set; }
    public int Height { get; set; }

    public VideoStreamProfile(int w, int h, int fps, Stream s, Format f) : base(fps, s, f)
    {
        Width = w; Height = h;
    }

    public Intrinsics GetIntrinsics() => new Intrinsics
    {
        width = Width, height = Height,
        ppx = Width/2.0f, ppy = Height/2.0f,
        fx = Width/2.0f, fy = Width/2.0f
    };
}

public enum Stream { Depth, Color, Infrared, Fisheye, Gyro, Accel, Gpio, Pose, Confidence }
public enum Format { Z16, Disparity16, Xyz32f, Yuyv, Rgb8, Bgr8, Rgba8, Bgra8, Y8, Y16, Raw10, Raw16, Raw8, Uyvy, MotionXyz32f, GpioRaw, Mjpeg }

public class Intrinsics
{
    public int width { get; set; }
    public int height { get; set; }
    public float ppx { get; set; }
    public float ppy { get; set; }
    public float fx { get; set; }
    public float fy { get; set; }
    public Distortion model { get; set; } = Distortion.BrownConrady;
    public float[] coeffs { get; set; } = new float[5];

    public override string ToString() => $"Size: {width}x{height}, Focal: ({fx},{fy}), Principal: ({ppx},{ppy})";
}

public struct Extrinsics
{
    public float[] rotation;
    public float[] translation;

    public override string ToString() => $"T: [{string.Join(",", translation)}], R: ...";
}

public enum Distortion { None, ModifiedBrownConrady, InverseBrownConrady, Ftheta, BrownConrady, KannalaBrandt4 }

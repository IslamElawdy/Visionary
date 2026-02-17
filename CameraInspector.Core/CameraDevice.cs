namespace CameraInspector.Core;

public record CameraDevice
{
    public required string Vendor { get; init; }
    public required string Model { get; init; }
    public required string SerialNumber { get; init; }
    public required string ConnectionType { get; init; } // e.g., "USB", "Network"
    public bool IsReady { get; init; } = true;
    public CameraType Type { get; init; }

    public string DisplayName => $"{Vendor} {Model} ({SerialNumber})";
}

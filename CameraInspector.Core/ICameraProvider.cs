namespace CameraInspector.Core;

public interface ICameraProvider
{
    string Name { get; }
    CameraType SupportedType { get; }

    bool CanHandle(CameraDevice device)
    {
        return device.Type == SupportedType;
    }

    Task<IEnumerable<CameraDevice>> EnumerateAsync();
    Task<CameraDetails> QueryDetailsAsync(CameraDevice device);
}

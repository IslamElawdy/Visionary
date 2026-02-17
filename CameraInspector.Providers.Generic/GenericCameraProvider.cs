using CameraInspector.Core;

namespace CameraInspector.Providers.Generic;

public class GenericCameraProvider : ICameraProvider
{
    public string Name => "Generic";
    public CameraType SupportedType => CameraType.Generic;

    public Task<IEnumerable<CameraDevice>> EnumerateAsync()
    {
        IEnumerable<CameraDevice> devices =
        [
            new CameraDevice
            {
                Vendor = "Generic",
                Model = "Integrated Webcam",
                SerialNumber = "0000-0000",
                ConnectionType = "Internal",
                Type = CameraType.Generic,
                IsReady = true
            }
        ];

        return Task.FromResult(devices);
    }

    public Task<CameraDetails> QueryDetailsAsync(CameraDevice device)
    {
        var details = new CameraDetails
        {
            RawJson = "{ \"Type\": \"Generic\" }"
        };

        details.BasicInfo["Vendor"] = device.Vendor;
        details.BasicInfo["Model"] = device.Model;
        details.BasicInfo["Serial"] = device.SerialNumber;
        details.BasicInfo["Driver"] = "UVC Video Driver";

        details.StreamProfiles["Default"] = "640x480 @ 30 FPS";

        return Task.FromResult(details);
    }
}

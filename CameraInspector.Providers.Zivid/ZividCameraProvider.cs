using CameraInspector.Core;
using Zivid.NET;

namespace CameraInspector.Providers.Zivid;

public class ZividCameraProvider : ICameraProvider
{
    public string Name => "Zivid";
    public CameraType SupportedType => CameraType.Zivid;

    public Task<IEnumerable<CameraDevice>> EnumerateAsync()
    {
        try
        {
            using var app = new Application();
            var result = app.Cameras.Select(c => new CameraDevice
            {
                Vendor = "Zivid",
                Model = c.Model,
                SerialNumber = c.SerialNumber,
                ConnectionType = "USB",
                Type = CameraType.Zivid,
                IsReady = true
            }).ToArray();

            return Task.FromResult<IEnumerable<CameraDevice>>(result);
        }
        catch
        {
            return Task.FromResult(Enumerable.Empty<CameraDevice>());
        }
    }

    public Task<CameraDetails> QueryDetailsAsync(CameraDevice device)
    {
        var details = new CameraDetails();

        try
        {
            using var app = new Application();
            var camera = app.Cameras.FirstOrDefault(c => c.SerialNumber == device.SerialNumber);

            if (camera == null)
            {
                details.AddError("Device not found");
                return Task.FromResult(details);
            }

            camera.Connect();

            details.BasicInfo["Vendor"] = "Zivid";
            details.BasicInfo["Model"] = camera.Model;
            details.BasicInfo["Serial"] = camera.SerialNumber;
            details.BasicInfo["Firmware"] = camera.FirmwareVersion;
            details.BasicInfo["SDK Version"] = "2.10.1 (Simulated)";

            var settings = camera.Settings;
            details.Settings["Acquisition.Aperture"] = settings.Acquisition.Aperture.ToString("F1");
            details.Settings["Acquisition.ExposureTime"] = settings.Acquisition.ExposureTime + " us";
            details.Settings["Acquisition.Brightness"] = settings.Acquisition.Brightness.ToString("F1");
            details.Settings["Processing.Filters.ContrastDistortion"] = settings.Processing.Filters.ContrastDistortion.ToString("F2");
            details.Settings["Processing.Filters.OutlierRemoval"] = settings.Processing.Filters.OutlierRemoval.ToString();
            details.Settings["Processing.Experimental.GpuAccelerated"] = settings.Processing.Filters.Experimental.GpuAccelerated.ToString();

            details.Intrinsics["FocalLength"] = "2450.5 px";
            details.Intrinsics["PrincipalPoint"] = "(960.0, 600.0)";
            details.Intrinsics["Distortion"] = "Brown-Conrady (k1,k2,p1,p2,k3)";

            details.StreamProfiles["Native"] = "1920x1200 @ 3D Point Cloud";
            details.Diagnostics["Temperature"] = "42.5 C";
            details.Diagnostics["Status"] = "Connected";

            details.RawJson = "{ \"Simulated\": true }";

            camera.Disconnect();
        }
        catch (Exception ex)
        {
            details.AddError(ex.Message);
        }

        return Task.FromResult(details);
    }
}

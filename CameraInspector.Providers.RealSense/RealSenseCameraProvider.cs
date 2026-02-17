using CameraInspector.Core;
using Intel.RealSense;

namespace CameraInspector.Providers.RealSense;

public class RealSenseCameraProvider : ICameraProvider
{
    public string Name => "RealSense";
    public CameraType SupportedType => CameraType.RealSense;

    public Task<IEnumerable<CameraDevice>> EnumerateAsync()
    {
        try
        {
            using var ctx = new Context();
            using var devices = ctx.QueryDevices();

            var result = devices.Select(device => new CameraDevice
            {
                Vendor = "Intel",
                Model = device.Info.Supports(CameraInfo.Name) ? device.Info[CameraInfo.Name] : "RealSense Device",
                SerialNumber = device.Info.Supports(CameraInfo.SerialNumber) ? device.Info[CameraInfo.SerialNumber] : "Unknown",
                ConnectionType = "USB",
                Type = CameraType.RealSense,
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
            using var ctx = new Context();
            using var devices = ctx.QueryDevices();

            var targetDevice = devices.FirstOrDefault(d =>
                d.Info.Supports(CameraInfo.SerialNumber) &&
                d.Info[CameraInfo.SerialNumber] == device.SerialNumber);

            if (targetDevice == null)
            {
                details.AddError("Device not found");
                return Task.FromResult(details);
            }

            foreach (CameraInfo info in Enum.GetValues(typeof(CameraInfo)))
            {
                if (targetDevice.Info.Supports(info))
                {
                    details.BasicInfo[info.ToString()] = targetDevice.Info[info];
                }
            }

            var allProfiles = new List<StreamProfile>();

            foreach (var sensor in targetDevice.Sensors)
            {
                foreach (var profile in sensor.StreamProfiles)
                {
                    allProfiles.Add(profile);
                    var profileKey = $"{sensor.Name} - {profile.Stream} {profile.Format}";

                    if (profile is VideoStreamProfile vsp)
                    {
                        details.StreamProfiles[profileKey] = $"{vsp.Width}x{vsp.Height} @ {vsp.Framerate} FPS";
                        var intrinsicsKey = $"{sensor.Name} {profile.Stream} Intrinsics";
                        if (!details.Intrinsics.ContainsKey(intrinsicsKey))
                        {
                            details.Intrinsics[intrinsicsKey] = vsp.GetIntrinsics().ToString();
                        }
                    }
                    else
                    {
                        details.StreamProfiles[profileKey] = $"{profile.Framerate} FPS (Non-Video)";
                    }
                }
            }

            var depthProfile = allProfiles.FirstOrDefault(p => p.Stream == Intel.RealSense.Stream.Depth);
            var colorProfile = allProfiles.FirstOrDefault(p => p.Stream == Intel.RealSense.Stream.Color);

            if (depthProfile is not null && colorProfile is not null)
            {
                try
                {
                    details.Intrinsics["Extrinsics (Depth -> Color)"] = depthProfile.GetExtrinsicsTo(colorProfile).ToString();
                }
                catch (Exception ex)
                {
                    details.Intrinsics["Extrinsics (Depth -> Color)"] = $"Error: {ex.Message}";
                }
            }

            details.RawJson =
                "{ \"Simulated\": true, \"SDK\": \"librealsense2\", \"ProfilesCount\": " + allProfiles.Count + " }";
        }
        catch (Exception ex)
        {
            details.AddError(ex.Message);
        }

        return Task.FromResult(details);
    }
}

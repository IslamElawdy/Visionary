using CameraInspector.Core;
using CameraInspector.Providers.Generic;
using CameraInspector.Providers.RealSense;
using CameraInspector.Providers.Zivid;

namespace CameraInspector.TestRunner;

internal static class Program
{
    private static async Task<int> Main()
    {
        Console.WriteLine("Camera Inspector Verification Runner");
        Console.WriteLine("===================================");

        var providers = new ICameraProvider[]
        {
            new ZividCameraProvider(),
            new RealSenseCameraProvider(),
            new GenericCameraProvider()
        };

        var failures = new List<string>();
        var totalDevices = 0;

        foreach (var provider in providers)
        {
            await RunProviderChecks(provider, failures, count => totalDevices += count);
        }

        Console.WriteLine();
        Console.WriteLine($"Total devices inspected: {totalDevices}");

        if (failures.Count > 0)
        {
            Console.WriteLine("\nFAILED CHECKS:");
            foreach (var failure in failures)
            {
                Console.WriteLine($" - {failure}");
            }

            return 1;
        }

        Console.WriteLine("\nAll checks passed.");
        return 0;
    }

    private static async Task RunProviderChecks(ICameraProvider provider, List<string> failures, Action<int> countCallback)
    {
        Console.WriteLine($"\n[{provider.Name}] Enumerating devices...");

        IEnumerable<CameraDevice> devices;
        try
        {
            devices = await provider.EnumerateAsync();
        }
        catch (Exception ex)
        {
            failures.Add($"{provider.Name}: EnumerateAsync threw '{ex.Message}'");
            return;
        }

        var deviceArray = devices.ToArray();
        countCallback(deviceArray.Length);

        Console.WriteLine($"[{provider.Name}] Found {deviceArray.Length} device(s)");

        foreach (var device in deviceArray)
        {
            Console.WriteLine($"  - {device.DisplayName} ({device.ConnectionType})");

            if (!provider.CanHandle(device))
            {
                failures.Add($"{provider.Name}: CanHandle returned false for {device.DisplayName}");
            }

            CameraDetails details;
            try
            {
                details = await provider.QueryDetailsAsync(device);
            }
            catch (Exception ex)
            {
                failures.Add($"{provider.Name}: QueryDetailsAsync threw for {device.DisplayName}: '{ex.Message}'");
                continue;
            }

            ValidateDetails(provider, device, details, failures);
        }
    }

    private static void ValidateDetails(ICameraProvider provider, CameraDevice device, CameraDetails details, List<string> failures)
    {
        if (details is null)
        {
            failures.Add($"{provider.Name}: null details for {device.DisplayName}");
            return;
        }

        if (string.IsNullOrWhiteSpace(details.RawJson))
        {
            failures.Add($"{provider.Name}: RawJson missing for {device.DisplayName}");
        }

        if (details.HasError)
        {
            failures.Add($"{provider.Name}: Details returned an error for {device.DisplayName}: {details.BasicInfo["Error"]}");
        }

        if (details.BasicInfo.Count == 0)
        {
            failures.Add($"{provider.Name}: BasicInfo is empty for {device.DisplayName}");
        }
    }
}

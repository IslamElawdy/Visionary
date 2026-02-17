using System.Text.Json;
using CameraInspector.Core;
using CameraInspector.Providers.Generic;
using CameraInspector.Providers.RealSense;
using CameraInspector.Providers.Zivid;

namespace CameraInspector.UI;

public partial class Form1 : Form
{
    private readonly List<ICameraProvider> _providers;
    private readonly Dictionary<CameraType, ICameraProvider> _providerByType;
    private CameraDevice? _selectedDevice;

    public Form1()
    {
        InitializeComponent();

        _providers =
        [
            new ZividCameraProvider(),
            new RealSenseCameraProvider(),
            new GenericCameraProvider()
        ];
        _providerByType = _providers.ToDictionary(x => x.SupportedType, x => x);

        SetupGrids();

        Load += async (_, _) => await RefreshDevicesAsync();
        btnRefresh.Click += async (_, _) => await RefreshDevicesAsync();
        dgvDevices.SelectionChanged += async (_, _) => await OnDeviceSelectedAsync();
        btnExport.Click += (_, _) => ExportJson();
    }

    private void SetupGrids()
    {
        dgvDevices.Columns.Add("Vendor", "Vendor");
        dgvDevices.Columns.Add("Model", "Model");
        dgvDevices.Columns.Add("Serial", "Serial Number");
        dgvDevices.Columns.Add("Type", "Type");
        dgvDevices.Columns.Add("Status", "Status");

        SetupKeyValueGrid(dgvInfo);
        SetupKeyValueGrid(dgvIntrinsics);
        SetupKeyValueGrid(dgvStreams);
        SetupKeyValueGrid(dgvSettings);
        SetupKeyValueGrid(dgvDiagnostics);
    }

    private static void SetupKeyValueGrid(DataGridView dgv)
    {
        dgv.Columns.Clear();
        dgv.Columns.Add("Key", "Property");
        dgv.Columns.Add("Value", "Value");
    }

    private async Task RefreshDevicesAsync()
    {
        btnRefresh.Enabled = false;
        dgvDevices.Rows.Clear();
        _selectedDevice = null;
        ClearDetails();

        try
        {
            var results = await Task.WhenAll(_providers.Select(async p => new
            {
                Provider = p,
                Devices = await p.EnumerateAsync()
            }));

            foreach (var providerResult in results)
            {
                foreach (var device in providerResult.Devices.OrderBy(d => d.DisplayName))
                {
                    var index = dgvDevices.Rows.Add(
                        device.Vendor,
                        device.Model,
                        device.SerialNumber,
                        device.Type,
                        device.IsReady ? "Ready" : "Busy/Error");

                    dgvDevices.Rows[index].Tag = new DeviceRowContext(device, providerResult.Provider);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error refreshing devices: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnRefresh.Enabled = true;
        }
    }

    private async Task OnDeviceSelectedAsync()
    {
        if (dgvDevices.SelectedRows.Count == 0)
        {
            return;
        }

        if (dgvDevices.SelectedRows[0].Tag is not DeviceRowContext rowContext)
        {
            return;
        }

        if (rowContext.Device == _selectedDevice)
        {
            return;
        }

        _selectedDevice = rowContext.Device;
        ClearDetails();

        try
        {
            var provider = rowContext.Provider;
            if (!provider.CanHandle(rowContext.Device) && _providerByType.TryGetValue(rowContext.Device.Type, out var typedProvider))
            {
                provider = typedProvider;
            }

            var details = await provider.QueryDetailsAsync(rowContext.Device);
            PopulateDetails(details);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error fetching details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ClearDetails()
    {
        dgvInfo.Rows.Clear();
        dgvIntrinsics.Rows.Clear();
        dgvStreams.Rows.Clear();
        dgvSettings.Rows.Clear();
        dgvDiagnostics.Rows.Clear();
        txtJson.Text = string.Empty;
    }

    private void PopulateDetails(CameraDetails details)
    {
        PopulateGrid(dgvInfo, details.BasicInfo);
        PopulateGrid(dgvIntrinsics, details.Intrinsics);
        PopulateGrid(dgvStreams, details.StreamProfiles);
        PopulateGrid(dgvSettings, details.Settings);
        PopulateGrid(dgvDiagnostics, details.Diagnostics);

        try
        {
            if (!string.IsNullOrWhiteSpace(details.RawJson))
            {
                var obj = JsonSerializer.Deserialize<object>(details.RawJson);
                txtJson.Text = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            }
        }
        catch
        {
            txtJson.Text = details.RawJson;
        }
    }

    private static void PopulateGrid(DataGridView dgv, Dictionary<string, string> data)
    {
        dgv.Rows.Clear();
        foreach (var kvp in data.OrderBy(x => x.Key))
        {
            dgv.Rows.Add(kvp.Key, kvp.Value);
        }
    }

    private void ExportJson()
    {
        if (string.IsNullOrWhiteSpace(txtJson.Text))
        {
            MessageBox.Show("No data to export.", "Info");
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Filter = "JSON Files|*.json",
            FileName = $"camera_params_{DateTime.Now:yyyyMMdd_HHmmss}.json"
        };

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            File.WriteAllText(sfd.FileName, txtJson.Text);
        }
    }

    private sealed record DeviceRowContext(CameraDevice Device, ICameraProvider Provider);
}

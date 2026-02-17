namespace CameraInspector.Core;

public class CameraDetails
{
    public Dictionary<string, string> BasicInfo { get; set; } = new();
    public Dictionary<string, string> Intrinsics { get; set; } = new();
    public Dictionary<string, string> StreamProfiles { get; set; } = new();
    public Dictionary<string, string> Settings { get; set; } = new();
    public Dictionary<string, string> Diagnostics { get; set; } = new();
    public string RawJson { get; set; } = "";

    public void AddError(string message)
    {
        BasicInfo["Error"] = message;
    }

    public bool HasError => BasicInfo.ContainsKey("Error");
}

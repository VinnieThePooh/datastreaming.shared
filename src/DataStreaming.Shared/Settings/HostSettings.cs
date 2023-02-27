namespace DataStreaming.Settings;

public class HostSettings
{
    public const string SectionName = "HostSettings";

    public string Host { get; set; }

    public int Port { get; set; }
}
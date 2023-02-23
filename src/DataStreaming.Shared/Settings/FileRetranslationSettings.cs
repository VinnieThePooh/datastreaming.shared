namespace DataStreaming.Settings;

public class FileRetranslationSettings : HostSettings
{
    public const string SectionName = "FileRetranslation";

    public uint BufferSize { get; set; } = 1024 * 8;

    public static uint FallbackBufferSize => 1024 * 8;

    public override string ToString()
    {
        return $"{Host}:{Port}";
    }
}
using System.Collections.ObjectModel;

namespace DataStreaming.Models.Negotiation;

public struct NegotiationOption
{
    public NegotiationOption(string protocolName, ProtocolVersion version)
    {
        ProtocolName = protocolName ?? throw new ArgumentNullException(nameof(protocolName));
        Versions.Add(version);
    }

    public string? ProtocolName { get; }

    public ICollection<ProtocolVersion> Versions { get; } = new Collection<ProtocolVersion>();

    public static NegotiationOption DefaultWithName(string name) => new(name, ProtocolVersion.Default);
}
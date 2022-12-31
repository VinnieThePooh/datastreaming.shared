using DataStreaming.Models.Negotiation;

namespace ImageRetranslationShared.Protocols.Interfaces;

public interface IHasProtocolVersion
{
    ProtocolVersion Version { get; }
}
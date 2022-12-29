using ImageRetranslationShared.Models.Negotiation;

namespace DataStreaming.Common.Protocols;

public interface IHasProtocolVersion
{
    ProtocolVersion Version { get; }
}
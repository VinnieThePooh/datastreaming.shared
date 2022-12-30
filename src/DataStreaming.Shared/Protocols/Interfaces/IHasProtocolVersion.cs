using ImageRetranslationShared.Models.Negotiation;

namespace ImageRetranslationShared.Protocols.Interfaces;

public interface IHasProtocolVersion
{
    ProtocolVersion Version { get; }
}
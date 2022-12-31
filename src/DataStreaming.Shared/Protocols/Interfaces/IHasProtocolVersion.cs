using DataStreaming.Models.Negotiation;

namespace DataStreaming.Protocols.Interfaces;

public interface IHasProtocolVersion
{
    ProtocolVersion Version { get; }
}
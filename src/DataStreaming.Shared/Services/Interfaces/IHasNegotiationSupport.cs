using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Services.Interfaces;

public interface IHasNegotiationSupport
{
    IProtocolNegotiator Negotiator { get; }

    //who dictates options
    public bool IsClientInitiator { get; set; }
}
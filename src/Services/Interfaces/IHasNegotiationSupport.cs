using DataStreaming.Services;

namespace DataStreaming.Common.Protocols;

public interface IHasNegotiationSupport
{
    IProtocolNegotiator Negotiator { get; }

    //who dictates options
    public bool IsClientInitiator { get; set; }
}
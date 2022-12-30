using ImageRetranslationShared.Protocols.Interfaces;

namespace ImageRetranslationShared.Services.Interfaces;

public interface IHasNegotiationSupport
{
    IProtocolNegotiator Negotiator { get; }

    //who dictates options
    public bool IsClientInitiator { get; set; }
}
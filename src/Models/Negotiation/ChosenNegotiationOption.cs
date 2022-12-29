namespace ImageRetranslationShared.Models.Negotiation;

public struct ChosenNegotiationOption
{
    public string ProtocolName { get; set; }
    public ProtocolVersion Version { get; set; }

    public static ChosenNegotiationOption FromOption(NegotiationOption option, ProtocolVersion version)
    {
        if (string.IsNullOrEmpty(option.ProtocolName))
            throw new ArgumentException($"{nameof(option.ProtocolName)} property is not set");

        return new ChosenNegotiationOption { ProtocolName = option.ProtocolName, Version = version };
    }
}
namespace ImageRetranslationShared.Models.Negotiation;

public class NegotiationResult
{
    public bool Negotiated { get; set; }
    public string ProtocolName { get; set; }
    public ProtocolVersion ProtocolVersion { get; set; }

    public string NonNegotiationReason { get; set; }

    public static NegotiationResult FromChosenOption(ChosenNegotiationOption chosen) => new() { ProtocolName = chosen.ProtocolName, ProtocolVersion = chosen.Version, Negotiated = true };

    public static NegotiationResult Failed => new() { Negotiated = false };

    public static NegotiationResult FailedWithReason(string reason)
    {
        if (string.IsNullOrEmpty(reason))
            throw new ArgumentException($"Failing {nameof(reason)} parameter is not set");

        return new NegotiationResult { Negotiated = false, NonNegotiationReason = reason };
    }
}
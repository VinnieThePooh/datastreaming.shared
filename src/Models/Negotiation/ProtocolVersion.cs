namespace ImageRetranslationShared.Models.Negotiation;

public struct ProtocolVersion : IEquatable<ProtocolVersion>
{
    public ProtocolVersion(byte major, byte minor)
    {
        Major = major;
        Minor = minor;
    }
    public byte Major { get; }

    public byte Minor { get; }

    public override string ToString() => $"{Major}.{Minor}";
    public static ProtocolVersion Default => new(1, 0);
    public bool Equals(ProtocolVersion other) => Major == other.Major && Minor == other.Minor;

    public override bool Equals(object? obj) => obj is ProtocolVersion other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Major, Minor);
}
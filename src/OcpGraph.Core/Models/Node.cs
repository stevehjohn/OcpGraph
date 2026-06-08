namespace OcpGraph.Core.Models;

public record Node(long Id) : Object(Id)
{
    private const double CoordinateScalingFactor = 10_000_000;

    public double Latitude { get; }

    public double Longitude { get; }

    public Node(BinaryReader reader) : this(reader.Read7BitEncodedInt64())
    {
        Latitude = reader.Read7BitEncodedInt() / CoordinateScalingFactor;
        
        Longitude = reader.Read7BitEncodedInt() / CoordinateScalingFactor;
    }
}
namespace OcpGraph.Console.Models;

public sealed record MapNode(long Id) : MapObject(Id)
{
    public double Latitude { get; }
    
    public double Longitude { get; }

    public MapNode(OsmSharp.Node node) : this(GetId(node.Id))
    {
        Latitude = node.Latitude ?? throw new ArgumentException("Latitude must have a value.", nameof(node));

        Longitude = node.Longitude ?? throw new ArgumentException("Longitude must have a value.", nameof(node));
    }
}
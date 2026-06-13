using OcpGraph.Core.Models;

namespace OcpGraph.Console.Models;

public sealed record MapNode(long Id) : MapObject(Id)
{
    public Amenity Amenity { get; } = Amenity.None;
    
    public string Name { get; }
    
    public double Latitude { get; }
    
    public double Longitude { get; }

    public MapNode(OsmSharp.Node node) : this(GetId(node.Id))
    {
        Latitude = node.Latitude ?? throw new ArgumentException("Latitude must have a value.", nameof(node));

        Longitude = node.Longitude ?? throw new ArgumentException("Longitude must have a value.", nameof(node));

        if (node.Tags.TryGetValue("amenity", out var value))
        {
            if (value.ToLowerInvariant() is "pub" or "bar" or "biergarten")
            {
                Amenity = Amenity.Pub;

                node.Tags.TryGetValue("name", out var name);

                Name = name;
            }
        }
    }
}
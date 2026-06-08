using OsmSharp;

namespace OcpGraph.Core.Models;

public sealed record MapWay(long Id, long[] Nodes) : MapObject(Id)
{
    public string Name { get; init; }
    
    public string Designation { get; init; }
    
    public string Type { get; init; }
    
    public MapWay(Way way) : this(GetId(way.Id), way.Nodes)
    {
        if (way.Tags.TryGetValue("name", out var name))
        {
            Name = name;
        }
        
        if (way.Tags.TryGetValue("ref", out var designation))
        {
            Designation = designation;
        }
        
        if (way.Tags.TryGetValue("highway", out var type))
        {
            Type = type;
        }
    }
}
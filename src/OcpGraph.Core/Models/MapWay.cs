using OsmSharp;

namespace OcpGraph.Core.Models;

public sealed record MapWay(long Id, long[] Nodes) : MapObject(Id)
{
    public string Name { get; }
    
    public string Designation { get; }
    
    public string Type { get; }
    
    public int? MaxSpeed { get; }
    
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

        if (way.Tags.TryGetValue("maxspeed", out var maxSpeed))
        {
            var parts = maxSpeed.Split(' ');

            if (parts.Length > 0)
            {
                if (int.TryParse(parts[0], out var speed))
                {
                    MaxSpeed = speed;
                }
            }
        }
    }
}
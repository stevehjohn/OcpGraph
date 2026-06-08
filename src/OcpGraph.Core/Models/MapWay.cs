using OsmSharp;

namespace OcpGraph.Core.Models;

public sealed record MapWay(long Id, long[] Nodes) : MapObject(Id)
{
    public string Name { get; }

    public string Designation { get; }

    public WayType Type { get; }

    public byte? MaxSpeed { get; }

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
            type = type.Split('_')[0].ToLower();

            Type = type switch
            {
                "motorway" => WayType.Motorway,
                "primary" => WayType.Primary,
                "secondary" => WayType.Secondary,
                "tertiary" => WayType.Tertiary,
                "trunk" => WayType.Trunk,
                _ => WayType.Other
            };
        }

        if (way.Tags.TryGetValue("maxspeed", out var maxSpeed))
        {
            var parts = maxSpeed.Split(' ');

            if (parts.Length > 0)
            {
                if (byte.TryParse(parts[0], out var speed))
                {
                    MaxSpeed = speed;
                }
            }
        }
    }
}
using OcpGraph.Core.Models;

namespace OcpGraph.Console.Models;

public sealed record MapWay(long Id, long[] Nodes) : MapObject(Id)
{
    public string Name { get; }

    public string Designation { get; }

    public WayType Type { get; private init; } = WayType.NoVehicles;

    public Direction Direction { get; } = Direction.Bidirectional;

    public byte? MaxSpeed { get; private init; }

    public MapWay(OsmSharp.Way way) : this(GetId(way.Id), way.Nodes)
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
                "residential" or "service" or "unclassified" or "living_street" or "road" or "byway" => WayType.Other,
                _ => WayType.NoVehicles
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

        if (way.Tags.TryGetValue("oneway", out var oneway))
        {
            var lower = oneway.ToLower();

            Direction = lower switch
            {
                "true" or "yes" or "1" => Direction.OneWay,
                "-1" => Direction.OmwWayReverse,
                _ => Direction
            };
        }
    }
}
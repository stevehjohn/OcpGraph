using OsmSharp;

namespace OcpGraph.Core.Models;

public sealed record MapWay(long? Id, long[] Nodes) : MapObject(Id)
{
    public MapWay(Way way) : this(way.Id, way.Nodes) { }
}
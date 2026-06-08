namespace OcpGraph.Core.Models.Osm;

public sealed record MapNode(long Id) : MapObject(Id)
{
    public MapNode(OsmSharp.Node node) : this(GetId(node.Id)) { }
}
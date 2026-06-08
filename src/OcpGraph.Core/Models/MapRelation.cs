using OsmSharp;

namespace OcpGraph.Core.Models;

public sealed record MapRelation(long? Id) : MapObject(Id)
{
    public MapRelation(Relation relation) : this(relation.Id) { }
}
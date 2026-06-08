namespace OcpGraph.Core.Models.Osm;

public abstract record MapObject(long Id)
{
    protected static long GetId(long? id)
    {
        return id ?? throw new ArgumentException("OSM object has no ID.", nameof(id));
    }
}
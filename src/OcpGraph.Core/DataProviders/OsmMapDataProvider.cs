using OcpGraph.Core.Models;
using OsmSharp.Streams;

namespace OcpGraph.Core.DataProviders;

public class OsmMapDataProvider : IMapDataProvider
{
    public IEnumerable<MapObject> Read()
    {
        using var fileStream = new FileInfo("great-britain.osm.pbf").OpenRead();
        
        var source = new PBFOsmStreamSource(fileStream);
        
        foreach (var element in source)
        {
            if (element.Id != null)
            {
                yield return new MapNode(element.Id.Value);
            }
        }
    }
}
using OcpGraph.Console.Models;
using OsmSharp;
using OsmSharp.Streams;

namespace OcpGraph.Console.DataProviders;

public class OsmMapDataProvider : IMapDataProvider
{
    public double Progress { get; private set; }
    
    public IEnumerable<MapObject> Read(OsmGeoType? type = null)
    {
        using var fileStream = new FileInfo(Path.Combine(AppContext.BaseDirectory, "data", "gb.osm.pbf")).OpenRead();
        
        var stream = new PBFOsmStreamSource(fileStream);
        
        Progress = 0;
        
        foreach (var element in stream)
        {
            if (type == null || element.Type == type)
            {
                var item = Translate(element);

                if (item != null)
                {
                    yield return item;
                }
            }

            Progress = fileStream.Position * 100.0 / fileStream.Length;
        }
    }

    private static MapObject Translate(OsmGeo item)
    {
        return item.Type switch
        {
            OsmGeoType.Node => new MapNode(item as Node),
            OsmGeoType.Way => new MapWay(item as Way),
            _ => null
        };
    }
}
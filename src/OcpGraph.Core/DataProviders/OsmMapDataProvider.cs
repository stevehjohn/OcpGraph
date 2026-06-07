using OcpGraph.Core.Models;
using OsmSharp;
using OsmSharp.Streams;

namespace OcpGraph.Core.DataProviders;

public class OsmMapDataProvider : IMapDataProvider
{
    public double Progress { get; private set; }
    
    public IEnumerable<MapObject> Read()
    {
        using var fileStream = new FileInfo(Path.Combine(AppContext.BaseDirectory, "data", "gb.osm.pbf")).OpenRead();
        
        var stream = new PBFOsmStreamSource(fileStream);
        
        Progress = 0;
        
        foreach (var element in stream)
        {
            if (element.Id != null)
            {
                if (element.Type == OsmGeoType.Way)
                {
                    if (element.Tags.ContainsKey("ref"))
                    {
                        Console.WriteLine(element.Tags["ref"]);
                    }

                    if (element.Tags.ContainsKey("name"))
                    {
                        Console.WriteLine(element.Tags["name"]);
                    }
                }

                yield return new MapNode(element.Id.Value);
            }

            Progress = fileStream.Position * 100.0 / fileStream.Length;
        }
    }
}
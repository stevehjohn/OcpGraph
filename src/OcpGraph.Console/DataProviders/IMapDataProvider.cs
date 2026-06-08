using OcpGraph.Console.Models;
using OsmSharp;

namespace OcpGraph.Console.DataProviders;

public interface IMapDataProvider
{
    double Progress { get; }
    
    IEnumerable<MapObject> Read(OsmGeoType? type = null);
}
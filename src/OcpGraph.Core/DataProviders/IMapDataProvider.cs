using OcpGraph.Core.Models;
using OsmSharp;

namespace OcpGraph.Core.DataProviders;

public interface IMapDataProvider
{
    double Progress { get; }
    
    IEnumerable<MapObject> Read(OsmGeoType? type = null);
}
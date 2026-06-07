using OcpGraph.Core.Models;

namespace OcpGraph.Core.DataProviders;

public interface IMapDataProvider
{
    double Progress { get; }
    
    IEnumerable<MapObject> Read();
}
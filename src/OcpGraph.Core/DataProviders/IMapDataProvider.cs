using OcpGraph.Core.Models;

namespace OcpGraph.Core.DataProviders;

public interface IMapDataProvider
{
    IEnumerable<MapObject> Read();
}
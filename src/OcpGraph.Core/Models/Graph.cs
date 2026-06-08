namespace OcpGraph.Core.Models;

public class Graph
{
    private readonly Dictionary<int, string> _names = [];

    private readonly Dictionary<long, Node> _nodes = [];

    private readonly Dictionary<long, Way> _ways = [];

    public float Progress { get; private set; }

    public void LoadData()
    {
        using var wayReader = new BinaryReader(File.Open("./data/ways.ogc", FileMode.Open));

        using var nodeReader = new BinaryReader(File.Open("./data/nodes.ogc", FileMode.Open));

        using var nameReader = new BinaryReader(File.Open("./data/names.ogc", FileMode.Open));

        var totalSize = wayReader.BaseStream.Length + nodeReader.BaseStream.Length + nameReader.BaseStream.Length;

        while (wayReader.BaseStream.Position != wayReader.BaseStream.Length)
        {
            var way = new Way(wayReader);

            _ways.Add(way.Id, way);

            Progress = wayReader.BaseStream.Position * 100f / totalSize;
        }

        var read = wayReader.BaseStream.Position;

        while (nodeReader.BaseStream.Position != nodeReader.BaseStream.Length)
        {
            var node = new Node(nodeReader);

            _nodes.Add(node.Id, node);

            Progress = (read + nodeReader.BaseStream.Position) * 100f / totalSize;
        }

        read += nodeReader.BaseStream.Position;

        while (nameReader.BaseStream.Position != nameReader.BaseStream.Length)
        {
            var id = nameReader.Read7BitEncodedInt();

            var name = nameReader.ReadString();

            _names.Add(id, name);

            Progress = (read + nameReader.BaseStream.Position) * 100f / totalSize;
        }
    }

    public Way FindNearestWay(double latitude, double longitude)
    {
        var nearestNode = _nodes.Values
            .MinBy(node => DistanceSquared(
                latitude,
                longitude,
                node.Latitude,
                node.Longitude));

        if (nearestNode is null)
        {
            throw new InvalidOperationException("The graph contains no nodes.");
        }

        var nearestWay = _ways.Values.FirstOrDefault(way => ContainsNode(way, nearestNode.Id));

        return nearestWay ?? throw new InvalidOperationException($"No way references node {nearestNode.Id}.");
    }

    public string GetName(Way way)
    {
        if (way.NameId > 0)
        {
            return _names[way.NameId];
        }

        if (way.DesignationId > 0)
        {
            return _names[way.DesignationId];
        }
        
        return "Unknown";
    }

    private static bool ContainsNode(Way way, long nodeId)
    {
        for (var i = 0; i < way.NodeCount; i++)
        {
            if (way[i] == nodeId)
            {
                return true;
            }
        }

        return false;
    }

    private static double DistanceSquared(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        var latitudeDifference = latitude1 - latitude2;

        var longitudeDifference = longitude1 - longitude2;

        return latitudeDifference * latitudeDifference + longitudeDifference * longitudeDifference;
    }
}
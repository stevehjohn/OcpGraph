namespace OcpGraph.Core.Models;

public class Graph
{
    private readonly Dictionary<int, string> _names = [];

    private readonly Dictionary<long, Node> _nodes = [];

    private readonly Dictionary<long, Way> _ways = [];

    public float LoadProgress { get; private set; }

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

            LoadProgress = wayReader.BaseStream.Position * 100f / totalSize;
        }

        var read = wayReader.BaseStream.Position;

        while (nodeReader.BaseStream.Position != nodeReader.BaseStream.Length)
        {
            var node = new Node(nodeReader);

            _nodes.Add(node.Id, node);

            LoadProgress = (read + nodeReader.BaseStream.Position) * 100f / totalSize;
        }

        read += nodeReader.BaseStream.Position;

        while (nameReader.BaseStream.Position != nameReader.BaseStream.Length)
        {
            var id = nameReader.Read7BitEncodedInt();

            var name = nameReader.ReadString();

            _names.Add(id, name);

            LoadProgress = (read + nameReader.BaseStream.Position) * 100f / totalSize;
        }
    }

    public Way FindNearestWay(double latitude, double longitude)
    {
        var namedWays = _ways.Values.Where(way => way.NameId > 0 || way.DesignationId > 0);

        Way nearestWay = null;

        var nearestDistance = double.MaxValue;

        foreach (var way in namedWays)
        {
            for (var i = 0; i < way.NodeCount; i++)
            {
                if (! _nodes.TryGetValue(way[i], out var node))
                {
                    continue;
                }

                var distance = DistanceSquared(latitude, longitude, node.Latitude, node.Longitude);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;

                    nearestWay = way;
                }
            }
        }

        return nearestWay ?? throw new InvalidOperationException("The graph contains no named ways.");
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

    public IReadOnlyList<Way> FindWaysInWindow(double centreLatitude, double centreLongitude, double widthMetres, double heightMetres)
    {
        const double metresPerDegreeLatitude = 111_320d;

        var halfLatitudeDelta = heightMetres / 2d / metresPerDegreeLatitude;

        var metresPerDegreeLongitude = metresPerDegreeLatitude * Math.Cos(centreLatitude * Math.PI / 180d);

        var halfLongitudeDelta = widthMetres / 2d / metresPerDegreeLongitude;

        return FindWaysInBounds(centreLatitude - halfLatitudeDelta, centreLongitude - halfLongitudeDelta, centreLatitude + halfLatitudeDelta, centreLongitude + halfLongitudeDelta);
    }
    
    private List<Way> FindWaysInBounds(double minLatitude, double minLongitude, double maxLatitude, double maxLongitude)
    {
        var results = new List<Way>();

        foreach (var way in _ways.Values)
        {
            for (var i = 0; i < way.NodeCount; i++)
            {
                if (!_nodes.TryGetValue(way[i], out var node))
                {
                    continue;
                }

                if (node.Latitude < minLatitude || node.Latitude > maxLatitude || node.Longitude < minLongitude || node.Longitude > maxLongitude)
                {
                    continue;
                }

                results.Add(way);
                break;
            }
        }

        return results;
    }
    
    private static double DistanceSquared(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        var latitudeDifference = latitude1 - latitude2;

        var longitudeDifference = longitude1 - longitude2;

        return latitudeDifference * latitudeDifference + longitudeDifference * longitudeDifference;
    }
}
namespace OcpGraph.Core.Models;

public class Graph
{
    private readonly Dictionary<int, string> _names = [];
    
    private readonly Dictionary<long, Node> _nodes = [];
    
    private readonly Dictionary<long, Way> _ways = [];

    public Graph()
    {
        using var wayReader = new BinaryReader(File.Open("./data/ways.ogc", FileMode.Open));

        while (wayReader.BaseStream.Position != wayReader.BaseStream.Length)
        {
            var way = new Way(wayReader);
            
            _ways.Add(way.Id, way);
        }
        
        using var nodeReader = new BinaryReader(File.Open("./data/nodes.ogc", FileMode.Open));

        while (nodeReader.BaseStream.Position != nodeReader.BaseStream.Length)
        {
            var node = new Node(nodeReader);
            
            _nodes.Add(node.Id, node);
        }
        
        using var nameReader = new BinaryReader(File.Open("./data/names.ogc", FileMode.Open));

        while (nameReader.BaseStream.Position != nameReader.BaseStream.Length)
        {
            var id = nameReader.Read7BitEncodedInt();
            
            var name = nameReader.ReadString();
            
            _names.Add(id, name);
        }
    }
}
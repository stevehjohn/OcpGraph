namespace OcpGraph.Core.Models;

public sealed record Way(long Id) : Object(Id)
{
    private readonly long[] _nodes;
    
    public WayType Type { get; private init; }
    
    public int NameId { get; private init; }
    
    public int DesignationId { get; private init; }
    
    public int MaxSpeed { get; private init; }
    
    public Direction Direction { get; private init; }
    
    public int NodeCount { get; private init; }
    
    public long this[int index] => _nodes[index];
    
    public Way(BinaryReader reader) : this(reader.Read7BitEncodedInt64())
    {
        Type = (WayType) reader.ReadByte();

        NameId = reader.Read7BitEncodedInt();

        DesignationId = reader.Read7BitEncodedInt();

        MaxSpeed = reader.ReadByte();

        Direction = (Direction) reader.ReadByte();

        NodeCount = reader.Read7BitEncodedInt();
        
        _nodes = new long[NodeCount];

        for (var i = 0; i < NodeCount; i++)
        {
            _nodes[i] = reader.Read7BitEncodedInt64();
        }
    }
}
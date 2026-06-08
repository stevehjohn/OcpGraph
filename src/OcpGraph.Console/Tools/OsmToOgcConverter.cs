using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OcpGraph.Console.DataProviders;
using OcpGraph.Console.Models;
using static System.Console;

namespace OcpGraph.Console.Tools;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public static class OsmToOgcConverter
{
    private const int CoordinateScalingFactor = 10_000_000;
    
    public static void ConvertData()
    {
        IMapDataProvider provider = new OsmMapDataProvider();
        
        var count = 0;
        
        var stopwatch = Stopwatch.StartNew();
        
        var lastUpdateMilliseconds = stopwatch.ElapsedMilliseconds;

        using var wayWriter = new BinaryWriter(File.Create("./data/ways.ogc"));

        using var nameWriter = new BinaryWriter(File.Create("./data/names.ogc"));

        using var nodeWriter = new BinaryWriter(File.Create("./data/nodes.ogc"));

        var names = new Dictionary<string, int>();

        var id = 1;

        foreach (var mapObject in provider.Read())
        {
            count++;

            if (stopwatch.ElapsedMilliseconds - lastUpdateMilliseconds > 500)
            {
                lastUpdateMilliseconds = stopwatch.ElapsedMilliseconds;
                
                WriteLine($"{count:N0} nodes in {stopwatch.Elapsed.TotalSeconds:N2}s, ({provider.Progress:N2}%).");
            }

            if (mapObject is MapWay way)
            {
                wayWriter.Write7BitEncodedInt64(way.Id);
                
                wayWriter.Write((byte) way.Type);

                if (! string.IsNullOrEmpty(way.Name))
                {
                    if (names.TryAdd(way.Name, id))
                    {
                        nameWriter.Write7BitEncodedInt(id++);
            
                        nameWriter.Write(way.Name);
                    }

                    wayWriter.Write7BitEncodedInt(names[way.Name]);
                }
                else
                {
                    wayWriter.Write7BitEncodedInt(0);
                }

                if (! string.IsNullOrEmpty(way.Designation))
                {
                    if (names.TryAdd(way.Designation, id))
                    {
                        nameWriter.Write7BitEncodedInt(id++);
            
                        nameWriter.Write(way.Designation);
                    }
                
                    wayWriter.Write7BitEncodedInt(names[way.Designation]);
                }
                else
                {
                    wayWriter.Write7BitEncodedInt(0);
                }
                
                wayWriter.Write(way.MaxSpeed ?? 0);

                wayWriter.Write((byte) way.Direction);

                wayWriter.Write7BitEncodedInt(way.Nodes.Length);

                foreach (var item in way.Nodes)
                {
                    wayWriter.Write7BitEncodedInt64(item);
                }
                
                continue;
            }
            
            if (mapObject is MapNode node)
            {
                nodeWriter.Write7BitEncodedInt64(node.Id);
                
                nodeWriter.Write7BitEncodedInt((int) (node.Latitude * CoordinateScalingFactor));
                
                nodeWriter.Write7BitEncodedInt((int) (node.Longitude * CoordinateScalingFactor));
            }
        }

        stopwatch.Stop();
        
        WriteLine($"{count:N0} map objects indexed in {stopwatch.Elapsed.TotalSeconds}s.");
    }
}
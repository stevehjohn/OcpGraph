using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OcpGraph.Core.DataProviders;
using OcpGraph.Core.Models;
using static System.Console;

namespace OcpGraph.Console;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public static class EntryPoint
{
    public static void Main()
    {
        IMapDataProvider provider = new OsmMapDataProvider();
        
        var count = 0;
        
        var stopwatch = Stopwatch.StartNew();
        
        var lastUpdateMilliseconds = stopwatch.ElapsedMilliseconds;

        using var wayWriter = new BinaryWriter(File.Create("./data/ways.ogc"));

        var names = new HashSet<string>();
        
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

                wayWriter.Write7BitEncodedInt64(way.Nodes.Length);

                foreach (var node in way.Nodes)
                {
                    wayWriter.Write7BitEncodedInt64(node);
                }

                names.Add(way.Name);

                names.Add(way.Designation);
            }
        }

        using var nameWriter = new BinaryWriter(File.Create("./data/names.ogc"));

        var id = 0;
        
        foreach (var name in names)
        {
            nameWriter.Write7BitEncodedInt(++id);
            
            nameWriter.Write(name);
        }

        stopwatch.Stop();
        
        WriteLine($"{count:N0} map objects indexed in {stopwatch.Elapsed.TotalSeconds}s.");
    }
}
using System.Diagnostics;
using OcpGraph.Core.DataProviders;
using OcpGraph.Core.Models;
using static System.Console;

namespace OcpGraph.Console;

public static class EntryPoint
{
    public static void Main()
    {
        IMapDataProvider provider = new OsmMapDataProvider();
        
        var count = 0;
        
        var stopwatch = Stopwatch.StartNew();
        
        var lastUpdateMilliseconds = stopwatch.ElapsedMilliseconds;

        using var wayWriter = new BinaryWriter(File.Create("./data/ways.ogc"));
        
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
                wayWriter.Write7BitEncodedInt64(way.Id.Value);

                wayWriter.Write7BitEncodedInt64(way.Nodes.Length);

                foreach (var node in way.Nodes)
                {
                    wayWriter.Write7BitEncodedInt64(node);
                }

                wayWriter.Write7BitEncodedInt64(0);
            }
        }
        
        stopwatch.Stop();
        
        WriteLine($"{count:N0} map objects indexed in {stopwatch.Elapsed.TotalSeconds}s.");
    }
}
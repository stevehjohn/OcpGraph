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
        
        foreach (var mapObject in provider.Read())
        {
            count++;

            if (stopwatch.ElapsedMilliseconds - lastUpdateMilliseconds > 500)
            {
                lastUpdateMilliseconds = stopwatch.ElapsedMilliseconds;
                
                WriteLine($"{count:N0} nodes in {stopwatch.Elapsed.TotalSeconds:N2}s, ({provider.Progress:N2}%).");
            }

            if (mapObject is MapWay)
            {
                var way = mapObject as MapWay;
                
                WriteLine(string.Join(',', way.Nodes));
            }

            if (mapObject.Id == null || mapObject.Id == 0)
            {
                WriteLine("!");
            }

            //
            // var way = mapObject as MapWay;
            //
            // if (way == null)
            // {
            //     continue;
            // }
            //
            // if (way.Nodes.Contains(0))
            // {
            //     WriteLine("!");
            // }
        }
        
        stopwatch.Stop();
        
        WriteLine($"{count:N0} map objects indexed in {stopwatch.Elapsed.TotalSeconds}s.");
    }
}
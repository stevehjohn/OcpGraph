using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OcpGraph.Console.Tools;
using OcpGraph.Core.Models;
using static System.Console;

namespace OcpGraph.Console;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public static class EntryPoint
{
    public static void Main(string[] arguments)
    {
        if (arguments.Length > 0 && arguments[0] == "convert")
        {
            OsmToOgcConverter.ConvertData();
            
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        Graph graph = null;
        
        var loadTask = Task.Run(() =>
        {
            graph = new Graph();
        });

        while (! loadTask.IsCompleted)
        {
            Thread.Sleep(100);
            
            Write($"{graph.Progress}% loaded.");
        }

        stopwatch.Stop();
        
        Write($"Graph loaded in {stopwatch.Elapsed.TotalMilliseconds}ms");
    }
}
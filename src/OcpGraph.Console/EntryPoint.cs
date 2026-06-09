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

        var graph = new Graph();

        var loadTask = Task.Run(graph.LoadData);

        WriteLine();

        while (! loadTask.IsCompleted)
        {
            Thread.Sleep(100);

            Write($"{graph.LoadProgress:N2}% loaded.");

            CursorLeft = 0;
        }

        stopwatch.Stop();

        Write($"\n\nGraph loaded in {stopwatch.Elapsed.Seconds}s\n\n");

        var way = graph.FindNearestWay(51.5037567,-3.5642593);

        WriteLine($"Nearest way: {graph.GetName(way)}\n");

        var ways = graph.FindWaysInWindow(51.5037567,-3.5642593, 1000, 1000);

        foreach (var item in ways)
        {
            WriteLine(graph.GetName(item));
        }
        
        WriteLine();
    }
}
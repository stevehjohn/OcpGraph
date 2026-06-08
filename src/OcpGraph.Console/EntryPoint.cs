using System.Diagnostics.CodeAnalysis;
using OcpGraph.Console.Tools;

namespace OcpGraph.Console;

[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public static class EntryPoint
{
    public static void Main()
    {
        OsmToOgcConverter.ConvertData();
    }
}
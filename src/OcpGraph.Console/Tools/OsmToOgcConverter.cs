using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OcpGraph.Console.DataProviders;
using OcpGraph.Console.Models;
using OcpGraph.Core.Models;
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

        var names = new Dictionary<string, int>();

        var id = 1;

        var nodeIds = new HashSet<long>();

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
                if (way.Type == WayType.NoVehicles)
                {
                    continue;
                }

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

                    if (way.Type != WayType.NoVehicles)
                    {
                        nodeIds.Add(item);
                    }
                }
            }
        }

        using var nodeWriter = new BinaryWriter(File.Create("./data/nodes.ogc"));

        foreach (var mapObject in provider.Read())
        {
            count++;

            if (stopwatch.ElapsedMilliseconds - lastUpdateMilliseconds > 500)
            {
                lastUpdateMilliseconds = stopwatch.ElapsedMilliseconds;

                WriteLine($"{count:N0} nodes in {stopwatch.Elapsed.TotalSeconds:N2}s, ({provider.Progress:N2}%).");
            }

            if (mapObject is MapNode node && (nodeIds.Contains(node.Id) || node.Amenity != Amenity.None))
            {
                nodeWriter.Write7BitEncodedInt64(node.Id);

                nodeWriter.Write7BitEncodedInt((int) (node.Latitude * CoordinateScalingFactor));

                nodeWriter.Write7BitEncodedInt((int) (node.Longitude * CoordinateScalingFactor));

                nodeWriter.Write7BitEncodedInt((int) node.Amenity);

                if (! string.IsNullOrWhiteSpace(node.Name))
                {
                    if (names.TryAdd(node.Name, id))
                    {
                        nameWriter.Write7BitEncodedInt(id++);

                        WriteLine(node.Name);
                        
                        nameWriter.Write(node.Name);
                    }

                    nodeWriter.Write7BitEncodedInt(names[node.Name]);
                }
                else
                {
                    nodeWriter.Write7BitEncodedInt(0);
                }
            }
        }

        stopwatch.Stop();

        WriteLine($"{count:N0} map objects indexed in {stopwatch.Elapsed.TotalSeconds}s.");
    }
}
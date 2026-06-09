using OcpGraph.Viewer.Display;

namespace OcpGraph.Viewer;

public static class EntryPoint
{
    public static void Main()
    {
        var renderer = new Renderer();
        
        renderer.Run();
    }
}
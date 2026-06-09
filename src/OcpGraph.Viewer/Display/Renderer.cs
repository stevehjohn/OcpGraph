using Microsoft.Xna.Framework;

namespace OcpGraph.Viewer.Display;

public sealed class Renderer : Game
{
    private const int WindowWidth = 800;

    private const int WindowHeight = 600;

    private readonly GraphicsDeviceManager _graphics;
    
    public Renderer()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = WindowWidth,
            PreferredBackBufferHeight = WindowHeight
        };

        IsMouseVisible = true;
    }
    
    protected override void Initialize()
    {
        Window.Title = "OcpGraph Viewer";
        
        base.Initialize();
    }
}
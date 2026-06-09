using Microsoft.Xna.Framework;

namespace OcpGraph.Viewer.Display;

public sealed class Renderer : Game
{
    private const int WindowWidth = 800;

    private const int WindowHeight = 600;

    // ReSharper disable once NotAccessedField.Local
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

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}
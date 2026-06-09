using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OcpGraph.Viewer.Infrastructure;

namespace OcpGraph.Viewer.Display;

public sealed class Renderer : Game
{
    private const int WindowWidth = 800;

    private const int WindowHeight = 600;

    // ReSharper disable once NotAccessedField.Local
    private readonly GraphicsDeviceManager _graphics;

    private TextManager _textManager;
    
    private SpriteBatch _spriteBatch;
    
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

    protected override void LoadContent()
    {
        Content.RootDirectory = "_Content";
        
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _textManager = new TextManager(_spriteBatch, Content.Load<SpriteFont>("Font"));

        base.LoadContent();
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OcpGraph.Core.Models;
using OcpGraph.Viewer.Infrastructure;

namespace OcpGraph.Viewer.Display;

public sealed class Renderer : Game
{
    private const int WindowWidth = 800;

    private const int WindowHeight = 600;

    // ReSharper disable once NotAccessedField.Local
    private readonly GraphicsDeviceManager _graphics;
    
    private readonly Graph _graph = new();

    private TextManager _textManager;
    
    private SpriteBatch _spriteBatch;

    private bool _isLoading;
    
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

        Task.Run(() =>
        {
            _isLoading = true;

            _graph.LoadData();
        }).ContinueWith(task =>
        {
            _isLoading = false;

            if (task.IsFaulted)
            {
                throw task.Exception;
            }
        });
        
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
        DrawText();
        
        base.Draw(gameTime);
    }

    private void DrawText()
    {
        if (_isLoading)
        {
            _textManager.DrawMessage($"Loading ({_graph.LoadProgress})%...", WindowWidth / 2, WindowHeight / 2, Color.White, true);
        }
    }
}
using System.Collections.Generic;
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

    private static Vector2 Project(double latitude, double longitude, double minLatitude, double maxLatitude, double minLongitude, double maxLongitude, int width, int height)
    {
        var x = (float) ((longitude - minLongitude) / (maxLongitude - minLongitude) * width);

        var y = (float) ((maxLatitude - latitude) / (maxLatitude - minLatitude) * height);

        return new Vector2(x, y);
    }

    // ReSharper disable once NotAccessedField.Local
    private readonly GraphicsDeviceManager _graphics;

    private readonly Graph _graph = new();

    private List<VertexPositionColor> _vertices = [];

    private TextManager _textManager;

    private SpriteBatch _spriteBatch;

    private BasicEffect _effect;

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

        _isLoading = true;

        Task.Run(() =>
        {
            _graph.LoadData();
        }).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                throw task.Exception;
            }

            LoadComplete();
        });

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Content.RootDirectory = "_Content";

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _textManager = new TextManager(_spriteBatch, Content.Load<SpriteFont>("Font"));

        _effect = new BasicEffect(GraphicsDevice)
        {
            VertexColorEnabled = true,
            LightingEnabled = false
        };

        base.LoadContent();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        DrawRoads(GraphicsDevice);

        _spriteBatch.Begin();

        DrawText();

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void LoadComplete()
    {
        _isLoading = false;

        Task.Run(() =>
        {
            var ways = _graph.FindWaysInWindow(51.5037567, -3.5642593, 1_000, 1_000);

            var vertices = BuildRoadVertices(ways, _graph, _graph.Bounds, WindowWidth, WindowHeight);

            _vertices = vertices;
        });
    }

    private void DrawText()
    {
        if (_isLoading)
        {
            _textManager.DrawMessage($"Loading {_graph.LoadProgress:N0}%...", WindowWidth / 2, WindowHeight / 2, Color.White, true);
        }
    }

    private List<VertexPositionColor> BuildRoadVertices(IEnumerable<Way> ways, Graph graph, MapBounds bounds, int width, int height)
    {
        var vertices = new List<VertexPositionColor>();

        foreach (var way in ways)
        {
            var colour = GetRoadColour(way.Type);

            for (var i = 0; i < way.NodeCount - 1; i++)
            {
                if (! graph.TryGetNode(way[i], out var first) || ! graph.TryGetNode(way[i + 1], out var second))
                {
                    continue;
                }

                var a = Project(first.Latitude, first.Longitude, bounds.MinLatitude, bounds.MaxLatitude, bounds.MinLongitude, bounds.MaxLongitude, width, height);

                var b = Project(second.Latitude, second.Longitude, bounds.MinLatitude, bounds.MaxLatitude, bounds.MinLongitude, bounds.MaxLongitude, width, height);

                _vertices.Add(new VertexPositionColor(new Vector3(a, 0), colour));

                _vertices.Add(new VertexPositionColor(new Vector3(b, 0), colour));
            }
        }
        
        return vertices;
    }

    private void DrawRoads(GraphicsDevice graphicsDevice)
    {
        if (_vertices.Count == 0)
        {
            return;
        }

        _effect.World = Matrix.Identity;

        _effect.View = Matrix.Identity;

        _effect.Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);

        _effect.VertexColorEnabled = true;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _vertices.ToArray(), 0, _vertices.Count / 2);
        }
    }

    private static Color GetRoadColour(WayType type)
    {
        return Color.Aqua;
    }
}
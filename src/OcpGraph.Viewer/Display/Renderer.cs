using System;
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

    private static readonly RasterizerState AntiAliasedRasterizerState = new()
    {
        CullMode = CullMode.None,
        MultiSampleAntiAlias = true
    };

    // ReSharper disable once NotAccessedField.Local
    private readonly GraphicsDeviceManager _graphics;

    private readonly Graph _graph = new();

    private VertexPositionColor[] _vertices = [];

    private TextManager _textManager;

    private SpriteBatch _spriteBatch;

    private BasicEffect _effect;

    private bool _isLoading;

    public Renderer()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = WindowWidth,
            PreferredBackBufferHeight = WindowHeight,
            PreferMultiSampling = true
        };

        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Window.Title = "OcpGraph Viewer";

        _isLoading = true;

        Task.Run(() => { _graph.LoadData(); }).ContinueWith(task =>
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

        DrawRoads();

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

            var bounds = MapBounds.FromCentre(51.5037567, -3.5642593, 1_000, 1_000);

            var vertices = BuildRoadVertices(ways, _graph, bounds, WindowWidth, WindowHeight);

            _vertices = vertices;

            Console.WriteLine("Done");
        });
    }

    private void DrawText()
    {
        if (_isLoading)
        {
            _textManager.DrawMessage($"Loading {_graph.LoadProgress:N0}%...", WindowWidth / 2, WindowHeight / 2, Color.White, true);
        }
    }

    private static Vector2 Project(double latitude, double longitude, double minLatitude, double maxLatitude, double minLongitude, double maxLongitude, int width, int height)
    {
        var x = (float) ((longitude - minLongitude) / (maxLongitude - minLongitude) * width);

        var y = (float) ((maxLatitude - latitude) / (maxLatitude - minLatitude) * height);

        return new Vector2(x, y);
    }

    private static VertexPositionColor[] BuildRoadVertices(IEnumerable<Way> ways, Graph graph, MapBounds bounds, int width, int height)
    {
        var vertices = new List<VertexPositionColor>();

        foreach (var way in ways)
        {
            var colour = GetRoadColour(way.Type);

            var thickness = GetRoadThickness(way.Type);

            for (var i = 0; i < way.NodeCount - 1; i++)
            {
                if (! graph.TryGetNode(way[i], out var first) || ! graph.TryGetNode(way[i + 1], out var second))
                {
                    continue;
                }

                var start = Project(first.Latitude, first.Longitude, bounds.MinLatitude, bounds.MaxLatitude, bounds.MinLongitude, bounds.MaxLongitude, width, height);

                var end = Project(second.Latitude, second.Longitude, bounds.MinLatitude, bounds.MaxLatitude, bounds.MinLongitude, bounds.MaxLongitude, width, height);

                AddThickLine(vertices, start, end, thickness, colour);
            }
        }

        return vertices.ToArray();
    }

    private void DrawRoads()
    {
        var vertices = _vertices;

        if (vertices.Length == 0)
        {
            return;
        }

        _effect.World = Matrix.Identity;

        _effect.View = Matrix.Identity;

        _effect.Projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);

        GraphicsDevice.RasterizerState = AntiAliasedRasterizerState;
        
        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
        }
    }

    private static void AddThickLine(List<VertexPositionColor> vertices, Vector2 start, Vector2 end, float thickness, Color colour)
    {
        var difference = end - start;

        if (difference.LengthSquared() <= float.Epsilon)
        {
            return;
        }

        var direction = Vector2.Normalize(difference);

        var normal = new Vector2(-direction.Y, direction.X);

        var offset = normal * (thickness / 2f);

        var topLeft = start + offset;

        var bottomLeft = start - offset;

        var topRight = end + offset;

        var bottomRight = end - offset;

        vertices.Add(new VertexPositionColor(new Vector3(topLeft, 0), colour));

        vertices.Add(new VertexPositionColor(new Vector3(bottomLeft, 0), colour));

        vertices.Add(new VertexPositionColor(new Vector3(topRight, 0), colour));

        vertices.Add(new VertexPositionColor(new Vector3(topRight, 0), colour));

        vertices.Add(new VertexPositionColor(new Vector3(bottomLeft, 0), colour));

        vertices.Add(new VertexPositionColor(new Vector3(bottomRight, 0), colour));
    }

    private static Color GetRoadColour(WayType type)
    {
        return type switch
        {
            WayType.Motorway => new Color(90, 150, 255),
            WayType.Trunk => new Color(255, 170, 80),
            WayType.Primary => new Color(255, 215, 90),
            WayType.Secondary => new Color(245, 245, 190),
            WayType.Tertiary => new Color(220, 220, 220),
            WayType.Other => new Color(150, 150, 150),
            _ => new Color(90, 90, 90)
        };
    }

    private static float GetRoadThickness(WayType type)
    {
        return type switch
        {
            WayType.Motorway => 9f,
            WayType.Trunk => 8f,
            WayType.Primary => 7f,
            WayType.Secondary => 6f,
            WayType.Tertiary => 5f,
            WayType.Other => 3f,
            _ => 2f
        };
    }
}
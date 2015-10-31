using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public class TileGrid : IDrawElement
    {
        public TiledTexture TiledTexture { private set; get; }

        public int X { set; get; }
        public int Y { set; get; }
        public int Width { private set; get; }
        public int Height { private set; get; }
        public Vector2 Scale { private set; get; }
        private int drawCount;

        private bool tileUpdated;
        private int tileBO;
        private ushort[] tiles;
        private Vector2[] tileVertices;

        private Program program;


        public TileGrid(TiledTexture tiledTexture, int width, int height) : this(tiledTexture, width, height, 1f, 1f)
        { }

        public TileGrid(TiledTexture tiledTexture, int width, int height, float scaleX, float scaleY)
        {
            Width = width;
            Height = height;
            Scale = new Vector2(scaleX, scaleY);
            drawCount = width * height;
            TiledTexture = tiledTexture;

            //Shader
            program = new Program();
            string vertexShader =
                @"
                in vec2 vertex_position;
                in vec2 tex_position;
                varying vec2 tex_coord;

                uniform mat4 projection;
                uniform vec2 offset;
                uniform float grid_width;
                uniform float tile_width;
                uniform float tile_height;
                uniform mat2 uvTile;

                void main()
                {
                    float a = floor(gl_InstanceID / grid_width);

                    gl_Position = projection * 

                    mat4(vec4(tile_width, 0.0, 0.0, 0.0),
                         vec4(0.0, tile_height, 0.0, 0.0),
                         vec4(0.0, 0.0, 1.0, 0.0),
                         vec4((gl_InstanceID - (grid_width * a)) * tile_width, a * tile_height, 0.0, 1.0)) *

                    vec4(vertex_position + offset, 0.0, 1.0);

                    tex_coord = tex_position + (vertex_position * uvTile);
                }
                ";

            string fragShader =
                @"
                varying vec2 tex_coord;
                uniform sampler2D tex_sampler;

                void main()
                {
                    gl_FragColor = texture2D(tex_sampler, tex_coord).bgra;
                }
                ";

            program.AddShader(ShaderType.VertexShader, vertexShader);
            program.AddShader(ShaderType.FragmentShader, fragShader);

            program.SetVariable("grid_width", (float)width);
            program.SetVariable("offset", new Vector2(X, Y));
            program.SetVariable("tile_width", (float)tiledTexture.TileWidth * Scale.X);
            program.SetVariable("tile_height", (float)tiledTexture.TileWidth * Scale.Y);
            program.SetVariable("uvTile", new Matrix2(tiledTexture.uTile, 0, 0, tiledTexture.vTile));
            program.SetVariable("projection", Projection.Matrix);
            program.SetVariable("tex_sampler", 0);

            Projection.ProjectionChangeEvent += Projection_ProjectionChangeEvent;

            //Tile
            tiles = new ushort[drawCount];
            tileVertices = new Vector2[drawCount];

            tileBO = GL.GenBuffer();
            GLStates.BindBuffer(BufferTarget.ArrayBuffer, tileBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tileVertices.Length * sizeof(float) * 2), IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        private void Projection_ProjectionChangeEvent(object sender, EventArgs e)
        {
            program.SetVariable("projection", Projection.Matrix);
        }

        public void Draw()
        {
            if (tileUpdated)
            {
                UpdateTileBuffer();
                tileUpdated = false;
            }

            GLStates.ActiveTexture(TextureUnit.Texture0);
            GLStates.BindTexture(TextureTarget.Texture2D, TiledTexture.TextureId);

            GLStates.BindBuffer(BufferTarget.ElementArrayBuffer, Quad.IBO);

            GLStates.UseProgram(program.ProgramId);

            GLStates.BindBuffer(BufferTarget.ArrayBuffer, Quad.VBO);
            GLStates.EnableVertexAttribArray(program.GetAttribLocation("vertex_position"));
            GL.VertexAttribPointer(program.GetAttribLocation("vertex_position"), 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            GLStates.BindBuffer(BufferTarget.ArrayBuffer, tileBO);
            GLStates.EnableVertexAttribArray(program.GetAttribLocation("tex_position"));
            GL.VertexAttribPointer(program.GetAttribLocation("tex_position"), 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.VertexAttribDivisor(program.GetAttribLocation("tex_position"), 1);

            GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedByte, IntPtr.Zero, drawCount);

        }

        private void UpdateTileBuffer()
        {
            for(int i = 0; i < tiles.Length; i++)
            {
                tileVertices[i] = TiledTexture.GetUV(tiles[i]);
            }
            GLStates.BindBuffer(BufferTarget.ArrayBuffer, tileBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tileVertices.Length * sizeof(float) * 2), tileVertices, BufferUsageHint.DynamicDraw);
        }


        public void ClearTiles(ushort tile)
        {
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = tile;
            tileUpdated = true;
        }

        public void SetTile(int x, int y, ushort tile)
        {
            if (x < Width && x >= 0 && y < Height && y >= 0)
            {
                int index = x + y * Width;
                if (tiles[index] != tile)
                {
                    tiles[index] = tile;
                    tileUpdated = true;
                }
            }
        }
    }
}

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

        public int X { private set; get; }
        public int Y { private set; get; }
        public int Z { private set; get; }
        public int Width { private set; get; }
        public int Height { private set; get; }
        public Vector2 Scale { private set; get; }
        public bool BackColor { private set; get; }
        private int drawCount;

        private bool updated;
        private int tileBO;
        private ushort[] tiles;
        private Vector4[] colors;
        private Vector4[] backColors;
        private float[] tileVertices;

        private Program program;


        public TileGrid(TiledTexture tiledTexture, int x, int y, int z, int width, int height, float scaleX, float scaleY, bool backColor)
        {
            Width = width;
            Height = height;
            Scale = new Vector2(scaleX, scaleY);
            drawCount = width * height;
            TiledTexture = tiledTexture;
            BackColor = backColor;
            X = x;
            Y = y;
            Z = z;

            //Shader
            program = new Program();
            string vertexShader =
                @"
                in vec2 vertex_position;
                in vec2 tex_position;
                in vec4 vx_color;
                in vec4 vx_back_color;
                varying vec4 fg_color;
                varying vec2 tex_coord;
                uniform mat4 projection;
                uniform vec2 offset;
                uniform float grid_width;
                uniform vec2 tile_size;
                uniform mat2 uvTile;
                {0}

                void main()
                {
                    fg_color = vx_color;
                    {1}

                    float a = floor(gl_InstanceID / grid_width);

                    gl_Position = projection * 

                    mat4(vec4(tile_size.x, 0.0, 0.0, 0.0),
                         vec4(0.0, tile_size.y, 0.0, 0.0),
                         vec4(0.0, 0.0, 1.0, 0.0),
                         vec4((gl_InstanceID - (grid_width * a)) * tile_size.x, a * tile_size.y, 0.0, 1.0)) *

                    vec4(vertex_position + offset, 0.0, 1.0);

                    tex_coord = tex_position + (vertex_position * uvTile);
                }
                ";

            string fragShader =
                @"
                varying vec2 tex_coord;
                varying vec4 fg_color;
                uniform sampler2D tex_sampler;
                {0}

                void main()
                {
                    gl_FragColor = texture2D(tex_sampler, tex_coord).bgra;
                    {1}
                }
                ";

            if (BackColor)
            {
                vertexShader = vertexShader.Replace("{0}", "varying vec4 fg_back_color;");
                vertexShader = vertexShader.Replace("{1}", "fg_back_color = vx_back_color;");
                fragShader = fragShader.Replace("{0}", "varying vec4 fg_back_color; uniform vec4 background;");
                fragShader = fragShader.Replace("{1}", "if (gl_FragColor.rgb == background.rgb) gl_FragColor = fg_back_color; else gl_FragColor *= fg_color;");
            }
            else
            {
                vertexShader = vertexShader.Replace("{0}", string.Empty);
                vertexShader = vertexShader.Replace("{1}", string.Empty);
                fragShader = fragShader.Replace("{0}", string.Empty);
                fragShader = fragShader.Replace("{1}", "gl_FragColor *= fg_color;");
            }

            program.AddShader(ShaderType.VertexShader, vertexShader);
            program.AddShader(ShaderType.FragmentShader, fragShader);

            program.SetVariable("grid_width", (float)width);
            program.SetVariable("offset", new Vector2(X, Y));
            program.SetVariable("tile_size", tiledTexture.TileSize * Scale);
            program.SetVariable("uvTile", new Matrix2(tiledTexture.uTile, 0, 0, tiledTexture.vTile));
            program.SetVariable("projection", Projection.Matrix);
            program.SetVariable("tex_sampler", 0);

            if (BackColor) program.SetVariable("background", tiledTexture.Background);

            Projection.ProjectionChangeEvent += Projection_ProjectionChangeEvent;

            //Tile and Colors
            tiles = new ushort[drawCount];
            colors = new Vector4[drawCount];
            backColors = new Vector4[drawCount];

            if (BackColor) tileVertices = new float[drawCount * 10];
            else tileVertices = new float[drawCount * 6];

            tileBO = GL.GenBuffer();
            GLStates.BindBuffer(BufferTarget.ArrayBuffer, tileBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tileVertices.Length * sizeof(float)), IntPtr.Zero, BufferUsageHint.DynamicDraw);
        }

        private void Projection_ProjectionChangeEvent(object sender, EventArgs e)
        {
            program.SetVariable("projection", Projection.Matrix);
        }

        public void Draw()
        {
            if (updated)
            {
                UpdateBuffer();
                updated = false;
            }

            GLStates.ActiveTexture(TextureUnit.Texture0);
            GLStates.BindTexture(TextureTarget.Texture2D, TiledTexture.TextureId);

            GLStates.BindBuffer(BufferTarget.ElementArrayBuffer, Quad.IBO);

            GLStates.UseProgram(program.ProgramId);

            GLStates.BindBuffer(BufferTarget.ArrayBuffer, Quad.VBO);
            GLStates.EnableVertexAttribArray(program.GetAttribLocation("vertex_position"));
            GL.VertexAttribPointer(program.GetAttribLocation("vertex_position"), 2, VertexAttribPointerType.Float, false, 0, 0);

            if (BackColor)
            {
                GLStates.BindBuffer(BufferTarget.ArrayBuffer, tileBO);
                GLStates.EnableVertexAttribArray(program.GetAttribLocation("tex_position"));
                GL.VertexAttribPointer(program.GetAttribLocation("tex_position"), 2, VertexAttribPointerType.Float, false, 10 * sizeof(float), 0);
                GL.VertexAttribDivisor(program.GetAttribLocation("tex_position"), 1);

                GLStates.EnableVertexAttribArray(program.GetAttribLocation("vx_color"));
                GL.VertexAttribPointer(program.GetAttribLocation("vx_color"), 4, VertexAttribPointerType.Float, false, 10 * sizeof(float), 2 * sizeof(float));
                GL.VertexAttribDivisor(program.GetAttribLocation("vx_color"), 1);

                GLStates.EnableVertexAttribArray(program.GetAttribLocation("vx_back_color"));
                GL.VertexAttribPointer(program.GetAttribLocation("vx_back_color"), 4, VertexAttribPointerType.Float, false, 10 * sizeof(float), 6 * sizeof(float));
                GL.VertexAttribDivisor(program.GetAttribLocation("vx_back_color"), 1);
            }
            else
            {
                GLStates.BindBuffer(BufferTarget.ArrayBuffer, tileBO);
                GLStates.EnableVertexAttribArray(program.GetAttribLocation("tex_position"));
                GL.VertexAttribPointer(program.GetAttribLocation("tex_position"), 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.VertexAttribDivisor(program.GetAttribLocation("tex_position"), 1);

                GLStates.EnableVertexAttribArray(program.GetAttribLocation("vx_color"));
                GL.VertexAttribPointer(program.GetAttribLocation("vx_color"), 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), 2 * sizeof(float));
                GL.VertexAttribDivisor(program.GetAttribLocation("vx_color"), 1);
            }

            GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedByte, IntPtr.Zero, drawCount);

        }

        private void UpdateBuffer()
        {
            if (BackColor)
            {
                for (int i = 0; i < drawCount; i++)
                {
                    Vector2 uv = TiledTexture.GetUV(tiles[i]);
                    int i10 = i * 10;
                    tileVertices[i10] = uv[0];
                    tileVertices[i10 + 1] = uv[1];
                    tileVertices[i10 + 2] = colors[i][0];
                    tileVertices[i10 + 3] = colors[i][1];
                    tileVertices[i10 + 4] = colors[i][2];
                    tileVertices[i10 + 5] = colors[i][3];
                    tileVertices[i10 + 6] = backColors[i][0];
                    tileVertices[i10 + 7] = backColors[i][1];
                    tileVertices[i10 + 8] = backColors[i][2];
                    tileVertices[i10 + 9] = backColors[i][3];
                }
            }
            else
            {
                for (int i = 0; i < drawCount; i++)
                {
                    Vector2 uv = TiledTexture.GetUV(tiles[i]);
                    int i10 = i * 6;
                    tileVertices[i10] = uv[0];
                    tileVertices[i10 + 1] = uv[1];
                    tileVertices[i10 + 2] = colors[i][0];
                    tileVertices[i10 + 3] = colors[i][1];
                    tileVertices[i10 + 4] = colors[i][2];
                    tileVertices[i10 + 5] = colors[i][3];
                }
            }
            GLStates.BindBuffer(BufferTarget.ArrayBuffer, tileBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tileVertices.Length * sizeof(float)), tileVertices, BufferUsageHint.DynamicDraw);
        }

        public void SetXY(int x, int y)
        {
            if (X != x || Y != y)
            {
                X = x;
                Y = y;
                program.SetVariable("offset", new Vector2(X, Y));
            }
        }

        public void ClearTiles(ushort tile, Vector4 color, Vector4 backColor)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = tile;
                colors[i] = color;
                backColors[i] = backColor;
            }
            updated = true;
        }

        public void ClearTiles(ushort tile, Vector4 color)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i] = tile;
                colors[i] = color;
            }
            updated = true;
        }

        public void SetColor(int x, int y, Vector4 color)
        {
            if (x < Width && x >= 0 && y < Height && y >= 0)
            {
                int index = x + y * Width;
                if (colors[index] != color)
                {
                    colors[index] = color;
                    updated = true;
                }
            }
        }

        public void SetBackColor(int x, int y, Vector4 backColor)
        {
            if (BackColor && x < Width && x >= 0 && y < Height && y >= 0)
            {
                int index = x + y * Width;
                if (backColors[index] != backColor)
                {
                    backColors[index] = backColor;
                    updated = true;
                }
            }
        }

        public void SetTile(int x, int y, ushort tile)
        {
            if (x < Width && x >= 0 && y < Height && y >= 0)
            {
                int index = x + y * Width;
                if (tiles[index] != tile)
                {
                    tiles[index] = tile;
                    updated = true;
                }
            }
        }

        public void Set(int x, int y, ushort tile, Vector4 color, Vector4 backColor)
        {
            if (x < Width && x >= 0 && y < Height && y >= 0)
            {
                int index = x + y * Width;
                if (tiles[index] != tile)
                {
                    tiles[index] = tile;
                    updated = true;
                }
                if (colors[index] != color)
                {
                    colors[index] = color;
                    updated = true;
                }
                if (BackColor && backColors[index] != backColor)
                {
                    backColors[index] = backColor;
                    updated = true;
                }
            }
        }

        public void Set(int x, int y, ushort tile, Vector4 color)
        {
            if (x < Width && x >= 0 && y < Height && y >= 0)
            {
                int index = x + y * Width;
                if (tiles[index] != tile)
                {
                    tiles[index] = tile;
                    updated = true;
                }
                if (colors[index] != color)
                {
                    colors[index] = color;
                    updated = true;
                }
            }
        }
    }
}

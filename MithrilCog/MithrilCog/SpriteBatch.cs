using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public class SpriteBatch : IDrawElement
    {
        private const int spriteSize = 13 * sizeof(float);

        public List<Sprite> Sprites { private set; get; }
        private Program program;
        public TiledTexture TiledTexture { private set; get; }
        public Vector2 Scale { private set; get; }
        private BufferObject bufferObject;

        public int X { private set; get; }
        public int Y { private set; get; }
        public int Z { private set; get; }

        public SpriteBatch(TiledTexture tiledTexture, int x, int y, int z, float scaleX, float scaleY)
        {
            bufferObject = new BufferObject();
            Scale = new Vector2(scaleX, scaleY);
            Sprites = new List<Sprite>();
            TiledTexture = tiledTexture;
            X = x;
            Y = y;
            Z = z;

            //Shader
            program = new Program();
            string vertexShader =
            @"
                in vec2 vertex_position;
                in vec2 sprite_position;
                in vec2 tex_position;
                in vec4 vx_color;
                in float rotation;
                in vec2 scale;
                in vec2 size;
                varying vec4 color;
                varying vec2 tex_coord;
                uniform mat4 projection;
                uniform vec2 offset;
                uniform vec2 tile_size;
                uniform mat2 uvTile;

                void main()
                {
                    color = vx_color;

                    float calc_cos = cos(rotation);
                    float calc_sin = sin(rotation);
                    vec2 size_total = size * tile_size * scale;

                    gl_Position =  projection *

                    //Object Translate
                    mat4(
                        vec4(1.0, 0.0, 0.0, 0.0), 
                        vec4(0.0, 1.0, 0.0, 0.0), 
                        vec4(0.0, 0.0, 1.0, 0.0), 
                        vec4(offset + (sprite_position * tile_size) + (size_total * 0.5), 0.0, 1.0)) *

                    //Rotation
                    mat4(
                        vec4(calc_cos, calc_sin, 0.0, 0.0), 
                        vec4(-calc_sin, calc_cos, 0.0, 0.0), 
                        vec4(0.0, 0.0, 1.0, 0.0), 
                        vec4(0.0, 0.0, 0.0, 1.0)) *

                    //Rotation Origin Translate
                    mat4(
                        vec4(1.0, 0.0, 0.0, 0.0), 
                        vec4(0.0, 1.0, 0.0, 0.0), 
                        vec4(0.0, 0.0, 1.0, 0.0), 
                        vec4(size_total * -0.5, 0.0, 1.0)) *

                    //Scale
                    mat4(
                        vec4(size_total.x, 0.0, 0.0, 0.0), 
                        vec4(0.0, size_total.y, 0.0, 0.0), 
                        vec4(0.0, 0.0, 1.0, 0.0), 
                        vec4(0.0, 0.0, 0.0, 1.0)) *

                    vec4(vertex_position, 0.0, 1.0);

                    tex_coord = tex_position + (vertex_position * uvTile * size);
                }
            ";

            string fragShader =
                @"
                varying vec2 tex_coord;
                varying vec4 color;
                uniform sampler2D tex_sampler;

                void main()
                {
                    gl_FragColor = texture2D(tex_sampler, tex_coord).bgra * color;
                }
                ";

            program.AddShader(ShaderType.VertexShader, vertexShader);
            program.AddShader(ShaderType.FragmentShader, fragShader);

            program.SetVariable("offset", new Vector2(X, Y));
            program.SetVariable("tile_size", new Vector2((float)tiledTexture.TileWidth, (float)tiledTexture.TileWidth) * Scale);
            program.SetVariable("uvTile", new Matrix2(tiledTexture.uTile, 0, 0, tiledTexture.vTile));
            program.SetVariable("projection", Projection.Matrix);
            program.SetVariable("tex_sampler", 0);

            Projection.ProjectionChangeEvent += Projection_ProjectionChangeEvent;
        }

        private void Projection_ProjectionChangeEvent(object sender, EventArgs e)
        {
            program.SetVariable("projection", Projection.Matrix);
        }

        public void Draw()
        {
            UpdateBuffer();

            GLStates.Enable(EnableCap.Blend);
            GLStates.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GLStates.ActiveTexture(TextureUnit.Texture0);
            GLStates.BindTexture(TextureTarget.Texture2D, TiledTexture.TextureId);

            GLStates.BindBuffer(BufferTarget.ElementArrayBuffer, Quad.IBO);

            GLStates.UseProgram(program.ProgramId);

            GLStates.BindBuffer(BufferTarget.ArrayBuffer, Quad.VBO);
            GLStates.EnableVertexAttribArray(program.GetAttribLocation("vertex_position"));
            GL.VertexAttribPointer(program.GetAttribLocation("vertex_position"), 2, VertexAttribPointerType.Float, false, 0, 0);

            GLStates.BindBuffer(BufferTarget.ArrayBuffer, bufferObject.Id);
            GLStates.EnableVertexAttribArray(program.GetAttribLocation("sprite_position"));
            GL.VertexAttribPointer(program.GetAttribLocation("sprite_position"), 2, VertexAttribPointerType.Float, false, 13 * sizeof(float), 0);
            GL.VertexAttribDivisor(program.GetAttribLocation("sprite_position"), 1);

            GLStates.EnableVertexAttribArray(program.GetAttribLocation("size"));
            GL.VertexAttribPointer(program.GetAttribLocation("size"), 2, VertexAttribPointerType.Float, false, 13 * sizeof(float), 2 * sizeof(float));
            GL.VertexAttribDivisor(program.GetAttribLocation("size"), 1);

            GLStates.EnableVertexAttribArray(program.GetAttribLocation("tex_position"));
            GL.VertexAttribPointer(program.GetAttribLocation("tex_position"), 2, VertexAttribPointerType.Float, false, 13 * sizeof(float), 4 * sizeof(float));
            GL.VertexAttribDivisor(program.GetAttribLocation("tex_position"), 1);

            GLStates.EnableVertexAttribArray(program.GetAttribLocation("scale"));
            GL.VertexAttribPointer(program.GetAttribLocation("scale"), 2, VertexAttribPointerType.Float, false, 13 * sizeof(float), 6 * sizeof(float));
            GL.VertexAttribDivisor(program.GetAttribLocation("scale"), 1);

            GLStates.EnableVertexAttribArray(program.GetAttribLocation("rotation"));
            GL.VertexAttribPointer(program.GetAttribLocation("rotation"), 1, VertexAttribPointerType.Float, false, 13 * sizeof(float), 8 * sizeof(float));
            GL.VertexAttribDivisor(program.GetAttribLocation("rotation"), 1);

            GLStates.EnableVertexAttribArray(program.GetAttribLocation("vx_color"));
            GL.VertexAttribPointer(program.GetAttribLocation("vx_color"), 4, VertexAttribPointerType.Float, false, 13 * sizeof(float), 9 * sizeof(float));
            GL.VertexAttribDivisor(program.GetAttribLocation("vx_color"), 1);

            GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedByte, IntPtr.Zero, Sprites.Count);
        }


        private void UpdateBuffer()
        {
            for (int i = 0; i < Sprites.Count; i++)
            {
                int i2 = i * 13;
                Vector2 uv = TiledTexture.GetUV(Sprites[i].tile);

                //sprite_position
                bufferObject.Data[i2] = Sprites[i].position.X;
                bufferObject.Data[i2 + 1] = Sprites[i].position.Y;

                //Size
                bufferObject.Data[i2 + 2] = Sprites[i].size.X;
                bufferObject.Data[i2 + 3] = Sprites[i].size.Y;

                //Texture
                bufferObject.Data[i2 + 4] = uv.X;
                bufferObject.Data[i2 + 5] = uv.Y;

                //Scale
                bufferObject.Data[i2 + 6] = Sprites[i].scale.X;
                bufferObject.Data[i2 + 7] = Sprites[i].scale.Y;

                //Rotation
                bufferObject.Data[i2 + 8] = Sprites[i].rotation;

                //Color
                bufferObject.Data[i2 + 9] = Sprites[i].color[0];
                bufferObject.Data[i2 + 10] = Sprites[i].color[1];
                bufferObject.Data[i2 + 11] = Sprites[i].color[2];
                bufferObject.Data[i2 + 12] = Sprites[i].color[3];
            }

            GLStates.BindBuffer(BufferTarget.ArrayBuffer, bufferObject.Id);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (IntPtr)(Sprites.Count * 13 * sizeof(float)), bufferObject.Data);
        }
    }
}

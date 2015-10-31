using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public static class Quad
    {
        public static int VBO { private set; get; }
        public static int IBO { private set; get; }

        static Quad()
        {
            VBO = GL.GenBuffer();
            float[] vertices = 
                { 1.0f, 0.0f,   //Pos 0
                  0.0f, 0.0f,   //Pos 1
                  0.0f, 1.0f,   //Pos 2
                  1.0f, 1.0f }; //Pos 3

            GLStates.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            IBO = GL.GenBuffer();
            byte[] indices = { 0, 1, 2, 2, 3, 0 };
            GLStates.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(byte)), indices, BufferUsageHint.StaticDraw);
        }
    }
}

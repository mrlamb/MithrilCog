using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public static class GLStates
    {
        private static HashSet<EnableCap> enableCapSet = new HashSet<EnableCap>();

        public static void Enable(EnableCap enableCap)
        {
            if (!enableCapSet.Contains(enableCap))
            {
                enableCapSet.Add(enableCap);
                GL.Enable(enableCap);
            }
        }
        public static void Disable(EnableCap enableCap)
        {
            if (enableCapSet.Contains(enableCap))
            {
                enableCapSet.Remove(enableCap);
                GL.Disable(enableCap);
            }
        }

        private static HashSet<ArrayCap> arrayCapSet = new HashSet<ArrayCap>();

        public static void Enable(ArrayCap arrayCap)
        {
            if (!arrayCapSet.Contains(arrayCap))
            {
                arrayCapSet.Add(arrayCap);
                GL.EnableClientState(arrayCap);
            }
        }
        public static void Disable(ArrayCap arrayCap)
        {
            if (arrayCapSet.Contains(arrayCap))
            {
                arrayCapSet.Remove(arrayCap);
                GL.DisableClientState(arrayCap);
            }
        }

        //private static MatrixMode matrixMode;
        //public static void MatrixMode(MatrixMode matrixMode)
        //{
        //    if (this.matrixMode != matrixMode)
        //    {
        //        this.matrixMode = matrixMode;
        //        GL.MatrixMode(matrixMode);
        //    }
        //}

        private static Dictionary<TextureTarget, int> boundTextures = new Dictionary<TextureTarget, int>();

        public static void BindTexture(TextureTarget textureTarget, int textureId)
        {
            if (boundTextures.ContainsKey(textureTarget) == false)
            {
                boundTextures.Add(textureTarget, textureId);
                GL.BindTexture(textureTarget, textureId);
            }
            else if (boundTextures[textureTarget] != textureId)
            {
                boundTextures[textureTarget] = textureId;
                GL.BindTexture(textureTarget, textureId);
            }
        }

        private static Dictionary<BufferTarget, int> boundBuffers = new Dictionary<BufferTarget, int>();

        public static void BindBuffer(BufferTarget bufferTarget, int bufferId)
        {
            if (boundBuffers.ContainsKey(bufferTarget) == false)
            {
                boundBuffers.Add(bufferTarget, bufferId);
                GL.BindBuffer(bufferTarget, bufferId);
            }
            else if (boundBuffers[bufferTarget] != bufferId)
            {
                boundBuffers[bufferTarget] = bufferId;
                GL.BindBuffer(bufferTarget, bufferId);
            }
        }

        private static int program = 0;

        public static void UseProgram(Program program)
        {
            UseProgram(program.ProgramId);
        }
 
        public static void UseProgram(int programId)
        {
            if (program != programId)
            {
                program = programId;
                GL.UseProgram(programId);
            }
        }

        private static HashSet<int> enabledVertexAttribArray = new HashSet<int>();

        public static void EnableVertexAttribArray(int index)
        {
            if (enabledVertexAttribArray.Contains(index) == false)
            {
                enabledVertexAttribArray.Add(index);
                GL.EnableVertexAttribArray(index);
            }
        }
        public static void DisableVertexAttribArray(int index)
        {
            if (enabledVertexAttribArray.Contains(index))
            {
                enabledVertexAttribArray.Remove(index);
                GL.DisableVertexAttribArray(index);
            }
        }

        private static TextureUnit textureUni;
        public static void ActiveTexture(TextureUnit textureUnit)
        {
            if (textureUni != textureUnit)
            {
                textureUni = textureUnit;
                GL.ActiveTexture(textureUnit);
            }
        }


    }
}

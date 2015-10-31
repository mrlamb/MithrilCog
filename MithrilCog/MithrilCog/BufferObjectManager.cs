using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public static class BufferObjectManager
    {
        public const int BufferSize = 1048576;

        private static Queue<int> boQueue = new Queue<int>();

        static BufferObjectManager()
        {

        }

        public static int GetBufferObject()
        {
            if (boQueue.Count > 0)
                return boQueue.Dequeue();
            else
            {
                int boId = GL.GenBuffer();
                GLStates.BindBuffer(BufferTarget.ArrayBuffer, boId);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)BufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                return boId;
            }
        }

        public static void RecycleBufferObject(int bufferObjectId)
        {
            boQueue.Enqueue(bufferObjectId);
        }
    }
}

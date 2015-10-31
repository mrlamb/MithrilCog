using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public static class Projection
    {
        public static Matrix4 Matrix { private set; get; }

        public static event EventHandler<EventArgs> ProjectionChangeEvent;

        internal static void Set(int width, int height)
        {
            Matrix = Ortho(0, width, height, 0, -1, 1);
            ProjectionChangeEvent(null, new EventArgs());
        }

        private static Matrix4 Ortho(float left, float right, float bottom, float top, float near, float far)
        {
            return new Matrix4(
                2f / (right - left), 0f, 0f, 0f,
                0f, 2f / (top - bottom), 0f, 0f,
                0f, 0f, -2f / (far - near), 0f,
                -(right + left) / (right - left), -(top + bottom) / (top - bottom), -(far + near) / (far - near), 1f);
        }
    }
}

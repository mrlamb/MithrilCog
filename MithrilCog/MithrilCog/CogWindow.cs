using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public class CogWindow : GameWindow
    {
        private List<IDrawElement> drawElements = new List<IDrawElement>();
        public CogKeyboard CogKeyboard { private set; get; }

        public CogWindow() : base()
        { }

        public CogWindow(int width, int height) : 
            base(width, height)
        { }

        public CogWindow(int width, int height, GraphicsMode mode) : 
            base(width, height, mode)
        { }

        public CogWindow(int width, int height, GraphicsMode mode, string title) : 
            base(width, height, mode, title)
        { }

        public CogWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options) : 
            base(width, height, mode, title, options)
        { }

        public CogWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options, DisplayDevice device) : 
            base(width, height, mode, title, options, device)
        { }

        public CogWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options, DisplayDevice device, int major, int minor, GraphicsContextFlags flags) :
            base(width, height, mode, title, options, device, major, minor, flags)
        { }

        public CogWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options, DisplayDevice device, int major, int minor, GraphicsContextFlags flags, IGraphicsContext sharedContext) :
            base(width, height, mode, title, options, device, major, minor, flags, sharedContext)
        { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CogKeyboard = new CogKeyboard(this);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Draw();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            Projection.Set(Width, Height);
        }

        public IList<IDrawElement> DrawElements
        {
            get
            {
                return drawElements;
            }
        }

        private void Draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.CornflowerBlue);

            foreach (IDrawElement drawChunk in drawElements)
                drawChunk.Draw();

            SwapBuffers();
        }
    }
}

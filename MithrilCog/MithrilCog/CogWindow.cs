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
        private SortedSet<IDrawElement> drawElements;

        public CogKeyboard CogKeyboard { private set; get; }

        public CogWindow() : base()
        { Initialize(); }

        public CogWindow(int width, int height) : 
            base(width, height)
        { Initialize(); }

        public CogWindow(int width, int height, GraphicsMode mode) : 
            base(width, height, mode)
        { Initialize(); }

        public CogWindow(int width, int height, GraphicsMode mode, string title) : 
            base(width, height, mode, title)
        { Initialize(); }

        public CogWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options) : 
            base(width, height, mode, title, options)
        { Initialize(); }

        public CogWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options, DisplayDevice device) : 
            base(width, height, mode, title, options, device)
        { Initialize(); }

        public CogWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options, DisplayDevice device, int major, int minor, GraphicsContextFlags flags) :
            base(width, height, mode, title, options, device, major, minor, flags)
        { Initialize(); }

        public CogWindow(int width, int height, GraphicsMode mode, string title, GameWindowFlags options, DisplayDevice device, int major, int minor, GraphicsContextFlags flags, IGraphicsContext sharedContext) :
            base(width, height, mode, title, options, device, major, minor, flags, sharedContext)
        { Initialize(); }

        protected void Initialize()
        {
            CogKeyboard = new CogKeyboard(this);
            drawElements = new SortedSet<IDrawElement>(new DrawElementComparer());
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
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

        public ICollection<IDrawElement> DrawElements
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

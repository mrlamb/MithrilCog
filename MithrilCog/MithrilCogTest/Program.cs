using MithrilCog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCogTest
{
    public class Program
    {
        private static CogWindow window;
        private static TileGrid grid;
        private static SpriteBatch spriteBatch;
        private static Sprite sprite;
        private static int x;
        private static int y;

        public static void Main(string[] args)
        {
            window = new CogWindow(800,600);

            window.UpdateFrame += Window_UpdateFrame;

            grid = new TileGrid(new TiledTexture("ascii_8x8.png", 8, 8), 0, 0, 0, 100, 75, 1f, 1f, false);
            spriteBatch = new SpriteBatch(new TiledTexture("ascii_8x8.png", 8, 8), 0, 0, 1, 1f, 1f);
            sprite = new Sprite();
            sprite.tile = 2;
            sprite.position = new OpenTK.Vector2(32f, 32f);
            sprite.rotation = 1f;
            sprite.color = new OpenTK.Vector4(1f, 1f, 1f, .5f);
            sprite.scale = new OpenTK.Vector2(1f, 1f);
            sprite.size = new OpenTK.Vector2(1f, 1f);

            spriteBatch.Sprites.Add(sprite);

            window.DrawElements.Add(grid);
            window.DrawElements.Add(spriteBatch);

            grid.ClearTiles(0, new OpenTK.Vector4(1f, 1f, 1f, 1f));

            window.Run(30d, 30d);
        }

        private static void Window_UpdateFrame(object sender, OpenTK.FrameEventArgs e)
        {
            KeyPress keyPress = window.CogKeyboard.GetKeyPress();
            if (keyPress != null)
            {
                if (keyPress.Key == Key.Up)
                    y--;
                else if (keyPress.Key == Key.Down)
                    y++;
                else if (keyPress.Key == Key.Left)
                    x--;
                else if (keyPress.Key == Key.Right)
                    x++;
                if (keyPress.Key == Key.Escape)
                    window.Close();
            }

            grid.ClearTiles(0, new OpenTK.Vector4(1f, 1f, 1f, 1f));
            sprite.position.X = x;
            sprite.position.Y = y;
        }
    }
}

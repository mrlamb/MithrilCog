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
        private static int x;
        private static int y;

        public static void Main(string[] args)
        {
            window = new CogWindow(800,600);

            window.UpdateFrame += Window_UpdateFrame;

            grid = new TileGrid(new TiledTexture("ascii_8x8.png", 8, 8), 100, 75, 1f, 1f);
            window.DrawElements.Add(grid);

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

            grid.ClearTiles(0);
            grid.SetTile(x, y, 2);

        }
    }
}

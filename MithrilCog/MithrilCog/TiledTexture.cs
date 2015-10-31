using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MithrilCog
{
    public class TiledTexture
    {
        public string Filename { private set; get; }
        public int TextureId { private set; get; }
        public float uRatio { private set; get; }
        public float vRatio { private set; get; }
        public int TileWidth { private set; get; }
        public int TileHeight { private set; get; }
        public float uTile { private set; get; }
        public float vTile { private set; get; }
        public int Stride { private set; get; }

        public TiledTexture(string filename, int tileWidth, int tileHeight)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Filename = filename;

            TextureId = GL.GenTexture();

            GLStates.BindTexture(TextureTarget.Texture2D, TextureId);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmpData = bmp.LockBits(
                new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Stride = bmp.Width / tileWidth;
            uRatio = 1f / (float)bmp.Width;
            vRatio = 1f / (float)bmp.Height;
            uTile = uRatio * (float)TileWidth;
            vTile = vRatio * (float)TileHeight;

            //Set Alpha
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            int r = rgbValues[0];
            int b = rgbValues[1];
            int g = rgbValues[2];
            for (int i = 0; i < rgbValues.Length; i += 4)
            {
                if (rgbValues[i] == r && rgbValues[i + 1] == b && rgbValues[i + 2] == g)
                    rgbValues[i + 3] = 0;
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            //Create Texture
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            bmp.UnlockBits(bmpData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        public Vector2 GetUV(int index)
        {
            int a = index / Stride;
            return new Vector2(((float)index - (float)(Stride * a)) * uTile, (float)a * vRatio);
        }

    }
}

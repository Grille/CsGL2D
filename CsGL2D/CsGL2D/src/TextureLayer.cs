using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SDI = System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace CsGL2D
{
    class TextureLayer
    {
        int idz;
        int tileSize = 64;
        int[] tileID;
        byte[] tileRefX, tileRefY;
        bool[] spaceFree;
        int size = 0;
        public TextureLayer(int id,int iLayersize,int iTilesize)
        {
            idz = id;
            tileSize = iTilesize;
            size = iLayersize / tileSize;

            tileID = new int[size * size];
            tileRefX = new byte[size * size];
            tileRefY = new byte[size * size];

            spaceFree = new bool[size * size];
        }
        public void _DEBUG()
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            for (int iy = 0; iy < size; iy++)
            {
                for (int ix = 0; ix < size; ix++)
                {
                    int i = ix + iy * size;
                    if (tileID[i] == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Write("  ");
                    }
                    else
                    {
                        int color = tileID[i] % 12;
                        if (color<6)Console.BackgroundColor = (ConsoleColor)color+1;
                        else Console.BackgroundColor = (ConsoleColor)color + 3;
                        if (tileRefX[i]==0&&tileRefY[i]==0)
                            Console.Write("X.");
                        else
                            Console.Write(" .");
                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(" .\n");
            }
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public bool Alloc(int w, int h,out int x,out int y)
        {
            for (x = 0; x <= size - w; x++)
                for (y = 0; y <= size - h; y++)
                {
                    bool result = true;
                    for (int iw = 0; iw < w; iw++)
                        for (int ih = 0; ih < h; ih++)
                            if (tileID[x + iw + y * size + ih * size] > 0)
                            {
                                result = false;
                                goto end;
                            }
                    end:
                    if (result)
                        return true;
                }
            x = 0;y = 0;
            return false;
        }
        public bool Alloc(Image img, out int x, out int y)
        {
            int w = (int)Math.Ceiling(img.Width / (float)tileSize);
            int h = (int)Math.Ceiling(img.Height / (float)tileSize);
            return Alloc(w, h, out x, out y);
        }

        public void Free(int id)
        {
            for (int i = 0;i< tileID.Length; i++)
            {
                if (tileID[i] == id)
                {
                    tileID[i] = 0;
                    tileRefX[i] = 0;
                    tileRefY[i] = 0;
                }
            }
        }
        public void SetData(Image img,int id,int x,int y)
        {
            int w = (int)Math.Ceiling(img.Width / (float)tileSize);
            int h = (int)Math.Ceiling(img.Height / (float)tileSize);

            for (int iw = 0; iw < w; iw++)
            {
                for (int ih = 0; ih < h; ih++)
                {
                    int index = x + iw + y * size + ih * size;
                    tileID[index] = id;
                    tileRefX[index] = (byte)iw;
                    tileRefY[index] = (byte)ih;
                }
            }

            Bitmap bitmap = (Bitmap)img;
            SDI.BitmapData ptr = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), SDI.ImageLockMode.ReadOnly, SDI.PixelFormat.Format32bppArgb);
            byte[] data = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(ptr.Scan0, data, 0, data.Length);
            bitmap.UnlockBits(ptr);

            GL.BindTexture(TextureTarget.Texture2DArray, TextureAtlas.TextureArray);
            //GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Rgba8, w, h, 1, 0, PixelFormat.Rgba, PixelType.Byte, data);
            GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, x*tileSize, y * tileSize, idz, img.Width, img.Height, 1, PixelFormat.Bgra, PixelType.UnsignedByte, data);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            /*
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            */
        }
        public void Defrag()
        {

        }
        public Bitmap GenerateLayer()
        {
            return null;
        }
    }
}

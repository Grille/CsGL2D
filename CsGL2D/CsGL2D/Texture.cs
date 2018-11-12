using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace CsGL2D
{
    public class Texture : IDisposable
    {
        internal int id,x, y, z,px,py;
        
        public int Width { get; private set; }
        public int Height { get; private set; }

        ~Texture() {
            TextureAtlas.FreeTexture(id, z);
        }
        public Texture()
        {
        }
        public Texture(string path)
        {
            addToAtlas(new Bitmap(path));
        }
        public Texture(int width, int height)
        {
            addToAtlas(new Bitmap(width, height));
        }
        public Texture(Image img)
        {
            addToAtlas(img);
        }

        private void addToAtlas(Image img)
        {
            Width = img.Width;Height = img.Height;
            TextureAtlas.GenerateTexture(img,out id,out x, out y, out z);
            px = x * TextureAtlas.tileSize; py = y * TextureAtlas.tileSize;
        }

        public void Dispose()
        {
            TextureAtlas.FreeTexture(id, z);
        }
    }
}

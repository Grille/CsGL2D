using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
namespace CsGL2D
{
    public static class TextureAtlas
    {
        static internal int layerSize = 2048, tileSize = 64;
        static internal int TextureArray;
        static int nextID = 1;
        static private TextureLayer[] textureLayers;
        static TextureAtlas()
        {
            Console.WriteLine(GL.GetInteger(GetPName.MaxTextureSize));
            Console.WriteLine(GL.GetInteger(GetPName.MaxArrayTextureLayers));
            //OpenTK.GLControl gLControl = new OpenTK.GLControl();
            //GLControl glControl = new GLControl();
            //glControl.CreateControl();
            //gameWindow;
            textureLayers = new TextureLayer[0];
            TextureArray = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, TextureArray);
            GL.TextureStorage3D(TextureArray, 1, SizedInternalFormat.Rgba8, 2048, 2048, 8);
            GL2D.LogError("_InitAtlas");
        }
        static public void _DEBUG()
        {
            Console.WriteLine("render textureLayers");
            for (int i= 0; i < textureLayers.Length; i++)
            {
                Console.WriteLine("\nLayer "+i+"...");
                textureLayers[i]._DEBUG();
            }
        }
        static public void GenerateTexture(Image img,out int id, out int x, out int y, out int z)
        {

            id = nextID++;
            x = 0;y = 0;

            bool isAllocated = false;
            for (z = 0; z < textureLayers.Length; z++)
                if (isAllocated = textureLayers[z].Alloc(img, out x, out y))
                    break;
            if (!isAllocated)
            {
                z = textureLayers.Length;
                addLayer();
                textureLayers[z].Alloc(img, out x, out y);
            }
            textureLayers[z].SetData(img, id, x, y);
        }
        static public void FreeTexture(int id,int z)
        {
            textureLayers[z].Free(id);
        }
        static private void addLayer()
        {
            Array.Resize(ref textureLayers, textureLayers.Length + 1);
            textureLayers[textureLayers.Length-1] = new TextureLayer(textureLayers.Length - 1, layerSize, tileSize);
        }
        static public void AddTexture(Image img,out int index)
        {

            index = 0;
        }
    }
}

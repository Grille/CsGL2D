using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsGL2D
{
    class Shader
    {
        internal int id;
        static Shader()
        {
            if (GL2D.IsRendererReady() != 0)
                GL2D.CreateThempContext();
        }
        public Shader()
        {

        }
        public Shader(string v)
        {

        }
    }
}

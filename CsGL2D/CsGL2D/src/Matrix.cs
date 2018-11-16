using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsGL2D
{
    public class Matrix
    {
        float scaleX, scaleY, transX, transY;
        public void Reset()
        {
            scaleX = scaleY = 1;
            transX = transY = 0;
        }
        public void Translate(float x,float y)
        {
            transX = x;transY = y;
        }
        public void Scale(float x, float y)
        {
            scaleX = x;scaleY = y;
        }
    }
}

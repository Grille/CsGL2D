using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;
//using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;



namespace CsGL2D
{
    public class Context
    {
        Control parant;
        Shader curShader;
        DrawBuffer curBuffer;
        GLControl glControl;
        public Context(Control control)
        {
            parant = control;

            glControl = new GLControl(
            GraphicsMode.Default, 3, 0,
            GraphicsContextFlags.Default);
            glControl.Dock = DockStyle.Fill;
            glControl.BackColor = Color.Black;
            glControl.BorderStyle = BorderStyle.FixedSingle;
            glControl.Enabled = false;
            glControl.CreateControl();
            GL2D.MakeCurrent(glControl);
            control.Controls.Add(glControl);
            
        }
        public void Clear()
        {
            Clear(Color.Black);
        }
        public void Clear(Color color)
        {
            GL2D.MakeCurrent(glControl);
            GL.ClearColor(color);
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        public void Refresh()
        {
            GL2D.MakeCurrent(glControl);
            glControl.SwapBuffers();
            glControl.Refresh();
        }

        public void Render(DrawBuffer drawBuffer,Shader shader)
        {
            Render(drawBuffer, shader, 0, drawBuffer.getLenght());
        }
        public void Render(DrawBuffer drawBuffer, Shader shader,int offset,int count)
        {
            GL2D.MakeCurrent(glControl);
            drawBuffer.update();
            if (curBuffer != drawBuffer)
                GL2D.UseBuffer(curBuffer = drawBuffer);
            if (curShader != shader)
                GL2D.UseShader(curShader = shader);

            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            GL.Uniform2(GL2D.resolutionUniform, new Vector2(glControl.Width, glControl.Height));

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, TextureAtlas.TextureArray);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, drawBuffer.indexBuffer);
            GL.DrawElements(BeginMode.Triangles, count*6, DrawElementsType.UnsignedInt, offset*6*4);
        }
    }
}

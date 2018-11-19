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
    public static class GL2D
    {
        static GLControl glControl;
        internal static void CreateThempContext()
        {
            glControl = new GLControl(GraphicsMode.Default, 3, 0,GraphicsContextFlags.Default);
            glControl.CreateControl();
        }
        public static void GetRenderContext(Control control)
        {

        }
        public static void SetRenderControl(System.Windows.Forms.Control control)
        {
            //Console.WriteLine(Assembly.GetExecutingAssembly().GetManifestResourceNames()[0]);
            
            glControl = new GLControl(
            GraphicsMode.Default, 3, 0,
            GraphicsContextFlags.Default);
            glControl.Dock = DockStyle.Fill;
            glControl.BackColor = Color.Black;
            glControl.BorderStyle = BorderStyle.FixedSingle;
            glControl.Enabled = false;
            glControl.CreateControl();
            glControl.MakeCurrent();
            control.Controls.Add(glControl);
        }
        public static void SwapBuffers()
        {
            glControl.SwapBuffers();
            glControl.Refresh();
        }
        public static byte IsRendererReady()
        {
            if (glControl == null) return 2;
            if (!glControl.IsHandleCreated) return 3;
            if (GL.GetInteger(GetPName.MajorVersion) < 2) return 1;
            return 0;
        }
        public static void Clear()
        {
            Clear(Color.Black);
        }
        public static void Clear(Color color)
        {
            GL.ClearColor(color);
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        public static string GetError()
        {
            return GL.GetError().ToString();
        }

        private static Shader shader;
        private static DrawBuffer drawBuffer;

        internal static int positionAttrib, texturePosAttrib, colorAttrib;


        internal static int resolutionUniform, translateUniform, scaleUniform;
        internal static int samplerUniform1;

        public static void UseBuffer(DrawBuffer buffer)
        {
            drawBuffer = buffer; ;

            enable();
        }
        public static void UseShader(Shader shader)
        {
            GL2D.shader = shader;
            GL.Enable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.UseProgram(shader.id);

            enable();
        }
        private static void enable()
        {
            if (shader != null && drawBuffer != null)
                drawBuffer.setAttrPtr(); 
        }
        
        public static void LogError(string text)
        {
            Console.WriteLine(text +": "+GL.GetError());
        }
        public static void Render()
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            GL.Uniform2(resolutionUniform, new Vector2(glControl.Width, glControl.Height));
            //GL.DrawArrays(PrimitiveType.Triangles, 0, vertexOffset);
            //GL.DrawElements(BeginMode.Triangles,vertexOffset,DrawElementsType.)

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, TextureAtlas.TextureArray);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, drawBuffer.indexBuffer);
            GL.DrawElements(BeginMode.Triangles, drawBuffer.indexOffset, DrawElementsType.UnsignedInt, 0);
        }

    }
}

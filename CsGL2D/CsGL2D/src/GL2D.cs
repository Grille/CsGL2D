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
        public static void ClearBuffer()
        {
            ClearBuffer(Color.Black);
        }
        public static void ClearBuffer(Color color)
        {
            GL.ClearColor(color);
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        public static string GetError()
        {
            return GL.GetError().ToString();
        }

        private static int shaderProgram;

        private static int vertexOffset, indexOffset;

        private static int positionAttrib, texturePosAttrib, colorAttrib;

        private static int positionBuffer, texturePosBuffer, colorBuffer, indexBuffer;

        private static int[] indexData;
        private static Vector2[] positionData;
        private static Vector3[] texturePosData;
        private static Vector4[] colorData;

        private static int resolutionUniform;
        private static int samplerUniform1;

        public static void CreateBuffer(int bufferSize)
        {
            //GL.BindVertexArray(GL.GenVertexArray());
            indexData = new int[bufferSize * 6];
            positionData = new Vector2[bufferSize * 4];
            texturePosData = new Vector3[bufferSize * 4];
            colorData = new Vector4[bufferSize * 4];

            for (int i = 0; i < bufferSize; i++)
            {
                positionData[i] = new Vector2(0, 0);
                texturePosData[i] = new Vector3(0, 0,0);
                colorData[i] = new Vector4(255, 255, 255, 255);
            }

            GL.GenBuffers(1, out indexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer, new IntPtr(indexData.Length * 4), indexData, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out positionBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, new IntPtr(positionData.Length * 8), positionData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(positionAttrib, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.GenBuffers(1, out texturePosBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texturePosBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, new IntPtr(texturePosData.Length * 12), texturePosData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(texturePosAttrib, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.GenBuffers(1, out colorBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, new IntPtr(colorData.Length * 16), colorData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(colorAttrib, 4, VertexAttribPointerType.Float, false, 0, 0);
        }

        public static void UpdateBuffer()
        {

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferSubData<int>(BufferTarget.ElementArrayBuffer, (IntPtr)(0 * 4), indexOffset * 4, indexData);

            //basic buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
            GL.BufferSubData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(0 * 8), vertexOffset * 8, positionData);

            
            GL.BindBuffer(BufferTarget.ArrayBuffer, texturePosBuffer);
            GL.BufferSubData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(0 * 12), vertexOffset * 12, texturePosData);

            //color Buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
            GL.BufferSubData<Vector4>(BufferTarget.ArrayBuffer, (IntPtr)(0 * 16), (vertexOffset * 16), colorData);
            
        }

        public static void DrawImage(Texture texture, RectangleF dst, Color color)
        {
            DrawImage(texture, new Rectangle(0, 0, texture.Width, texture.Height), dst, color);
        }
        public static void DrawImage(Texture texture, RectangleF src, RectangleF dst, Color color)
        {
            
            positionData[vertexOffset + 0] = new Vector2(dst.X, dst.Y); //ol
            positionData[vertexOffset + 1] = new Vector2(dst.X+dst.Width, dst.Y); //or
            positionData[vertexOffset + 2] = new Vector2(dst.X + dst.Width, dst.Y + dst.Height); //ur
            positionData[vertexOffset + 3] = new Vector2(dst.X, dst.Y + dst.Height); //ul

            /*
            texturePosData[vertexOffset + 0] = new Vector3(0, 0, 0); //ol
            texturePosData[vertexOffset + 1] = new Vector3(1, 0, 0); //or
            texturePosData[vertexOffset + 2] = new Vector3(1, 1, 0); //ur
            texturePosData[vertexOffset + 3] = new Vector3(0, 1, 0); //ul
            */

            float texX = texture.px + src.X, texY = texture.py + src.Y;
            texturePosData[vertexOffset + 0] = new Vector3(texX, texY, texture.z); //ol
            texturePosData[vertexOffset + 1] = new Vector3(texX + src.Width, texY, texture.z); //or
            texturePosData[vertexOffset + 2] = new Vector3(texX + src.Width, texY + src.Height, texture.z); //ur
            texturePosData[vertexOffset + 3] = new Vector3(texX, texY + src.Height, texture.z); //ul
            
            colorData[vertexOffset + 0] =
            colorData[vertexOffset + 1] =
            colorData[vertexOffset + 2] =
            colorData[vertexOffset + 3] = new Vector4(color.R, color.G, color.B, color.A);

            indexData[indexOffset + 0] = vertexOffset + 0;
            indexData[indexOffset + 1] = vertexOffset + 1;
            indexData[indexOffset + 2] = vertexOffset + 2;
            indexData[indexOffset + 3] = vertexOffset + 2;
            indexData[indexOffset + 4] = vertexOffset + 3;
            indexData[indexOffset + 5] = vertexOffset + 0;

            vertexOffset += 4;
            indexOffset += 6;
        }

        public static int CreateShader()
        {
            return CreateShader(
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CsGL2D.src.shader.dvertex.glsl")).ReadToEnd()
                ,
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CsGL2D.src.shader.dfragment.glsl")).ReadToEnd()
            );
        }
        public static int CreateShader(string fragmentShaderCode)
        {
            return CreateShader(
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CsGL2D.src.shader.dvertex.glsl")).ReadToEnd()
                ,
                fragmentShaderCode
            );
        }
        public static int CreateShader(string vertexShaderCode, string fragmentShaderCode)
        {
            try
            {

                LogError("0");
                int vertexShader = compileShader(vertexShaderCode, ShaderType.VertexShader);
                int fragmentShader = compileShader(fragmentShaderCode, ShaderType.FragmentShader);

                if (vertexShader == -1 || fragmentShader == -1) return -1;

                // Create program
                shaderProgram = GL.CreateProgram();

                GL.AttachShader(shaderProgram, vertexShader);
                GL.AttachShader(shaderProgram, fragmentShader);

                GL.LinkProgram(shaderProgram);


                positionAttrib = GL.GetAttribLocation(shaderProgram, "aPosition");
                texturePosAttrib = GL.GetAttribLocation(shaderProgram, "aTexturePos");
                LogError("0");
                colorAttrib = GL.GetAttribLocation(shaderProgram, "aColor");



                GL.EnableVertexAttribArray(positionAttrib);
                GL.EnableVertexAttribArray(texturePosAttrib);
                GL.EnableVertexAttribArray(colorAttrib);

                //uniforms
                samplerUniform1 = GL.GetUniformLocation(shaderProgram, "uSampler");

                //timeUniform = 
                    GL.GetUniformLocation(shaderProgram, "uTime");

                resolutionUniform = GL.GetUniformLocation(shaderProgram, "uResolution");

                //colorUniform = 
                    GL.GetUniformLocation(shaderProgram, "uColor");

                return shaderProgram;
            }
            catch (AccessViolationException e)
            {
                return -2;
            }

        }
        private static int compileShader(string code, ShaderType type)
        {
            string errorLog;
            int shader = GL.CreateShader(type);
            int status_code;
            GL.ShaderSource(shader, code);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status_code);
            if (status_code != 1)
            {
                Console.WriteLine(errorLog = type + ":\n" + GL.GetShaderInfoLog(shader));
                return -1;
            }
            else return shader;

        }

        public static void UseShader(int shaderProgram)
        {

            GL.Enable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.UseProgram(shaderProgram);


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

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(BeginMode.Triangles, indexOffset, DrawElementsType.UnsignedInt, 0);
            vertexOffset = 0;
            indexOffset = 0;
        }

    }
}

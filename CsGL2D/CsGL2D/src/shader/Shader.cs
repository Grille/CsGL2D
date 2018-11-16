using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace CsGL2D
{
    public class Shader
    {
        public Matrix Matrix;
        internal int id;
        static Shader()
        {
            if (GL2D.IsRendererReady() != 0)
                GL2D.CreateThempContext();
        }
        public Shader()
        {
            id = CreateShader(
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CsGL2D.src.shader.dvertex.glsl")).ReadToEnd()
                ,
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CsGL2D.src.shader.dfragment.glsl")).ReadToEnd()
            );
        }
        public Shader(string fragmentShader)
        {
            id = CreateShader(
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CsGL2D.src.shader.dvertex.glsl")).ReadToEnd()
                ,
                fragmentShader
            );
        }
        public Shader(string vertexShader,string fragmentShader)
        {
            id = CreateShader(
                vertexShader
                ,
                fragmentShader
            );
        }

        private int CreateShader(string vertexShaderCode, string fragmentShaderCode)
        {
            try
            {

                int vertexShader = compileShader(vertexShaderCode, ShaderType.VertexShader);
                int fragmentShader = compileShader(fragmentShaderCode, ShaderType.FragmentShader);

                if (vertexShader == -1 || fragmentShader == -1) return -1;

                // Create program
                int shaderProgram = GL.CreateProgram();

                GL.AttachShader(shaderProgram, vertexShader);
                GL.AttachShader(shaderProgram, fragmentShader);

                GL.LinkProgram(shaderProgram);


                GL2D.positionAttrib = GL.GetAttribLocation(shaderProgram, "aPosition");
                GL2D.texturePosAttrib = GL.GetAttribLocation(shaderProgram, "aTexturePos");
                GL2D.colorAttrib = GL.GetAttribLocation(shaderProgram, "aColor");



                GL.EnableVertexAttribArray(GL2D.positionAttrib);
                GL.EnableVertexAttribArray(GL2D.texturePosAttrib);
                GL.EnableVertexAttribArray(GL2D.colorAttrib);

                //uniforms
                GL2D.samplerUniform1 = GL.GetUniformLocation(shaderProgram, "uSampler");

                //timeUniform = 
                GL.GetUniformLocation(shaderProgram, "uTime");

                GL2D.resolutionUniform = GL.GetUniformLocation(shaderProgram, "uResolution");

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

    }
}

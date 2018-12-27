using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CsGL2D
{
    public class DrawBuffer : IDisposable
    {
        public int Index {
            get => indexOffset/6;
            set
            {
                if (Index > maxIndex)
                    maxIndex = Index;
                indexOffset = value * 6;
                vertexOffset = value * 4;
            }
        }

        bool disposed = false;
        private bool vertexChanged = false, sourceChanged = false,colorChanged = false;

        internal int maxIndex = 0;
        internal int vertexOffset, indexOffset;
        internal int length;
        internal int positionBuffer, texturePosBuffer, colorBuffer, indexBuffer;

        private int[] indexData;
        private Vector2[] positionData;
        private Vector3[] texturePosData;
        private Vector4[] colorData;

        static DrawBuffer()
        {
            if (GL2D.IsRendererReady() != 0)
                GL2D.CreateThempContext();
        }
        ~DrawBuffer()
        {
            dispose();
        }
        public void Dispose()
        {
            dispose();
            GC.SuppressFinalize(this);
        }
        protected virtual void dispose()
        {
            if (disposed)
                return;

            if (GL2D.IsRendererReady() == 0)
            {
                GL.DeleteBuffer(indexBuffer);
                GL.DeleteBuffer(positionBuffer);
                GL.DeleteBuffer(texturePosBuffer);
                GL.DeleteBuffer(colorBuffer);
            }
            disposed = true;
        }

        public DrawBuffer(int bufferSize)
        {
            //GL.BindVertexArray(GL.GenVertexArray());
            indexData = new int[bufferSize * 6];
            positionData = new Vector2[bufferSize * 4];
            texturePosData = new Vector3[bufferSize * 4];
            colorData = new Vector4[bufferSize * 4];

            for (int i = 0; i < bufferSize; i++)
            {
                positionData[i] = new Vector2(0, 0);
                texturePosData[i] = new Vector3(0, 0, 0);
                colorData[i] = new Vector4(255, 255, 255, 255);
            }

            Index = 0;
            for (int i = 0; i < bufferSize; i++)
            {
                indexData[indexOffset + 0] = vertexOffset + 0;
                indexData[indexOffset + 1] = vertexOffset + 1;
                indexData[indexOffset + 2] = vertexOffset + 2;
                indexData[indexOffset + 3] = vertexOffset + 2;
                indexData[indexOffset + 4] = vertexOffset + 3;
                indexData[indexOffset + 5] = vertexOffset + 0;
                Index++;
            }
            Reset();

            GL.GenBuffers(1, out indexBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer, new IntPtr(indexData.Length * 4), indexData, BufferUsageHint.DynamicRead);

            GL.GenBuffers(1, out positionBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, new IntPtr(positionData.Length * 8), positionData, BufferUsageHint.DynamicCopy);

            GL.GenBuffers(1, out texturePosBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texturePosBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, new IntPtr(texturePosData.Length * 12), texturePosData, BufferUsageHint.DynamicCopy);

            GL.GenBuffers(1, out colorBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, new IntPtr(colorData.Length * 16), colorData, BufferUsageHint.DynamicCopy);
        }
        internal void setAttrPtr()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
            GL.VertexAttribPointer(GL2D.positionAttrib, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texturePosBuffer);
            GL.VertexAttribPointer(GL2D.texturePosAttrib, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
            GL.VertexAttribPointer(GL2D.colorAttrib, 4, VertexAttribPointerType.Float, false, 0, 0);
        }
        public int getLenght()
        {
            return Math.Max(Index, maxIndex);
        }
        internal void update()
        {
            int drawVertexOffset = Math.Max(vertexOffset, maxIndex * 4);

            if (vertexChanged)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
                GL.BufferSubData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(0 * 8), drawVertexOffset * 8, positionData);
            }
            if (sourceChanged)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, texturePosBuffer);
                GL.BufferSubData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(0 * 12), drawVertexOffset * 12, texturePosData);
            }
            if (colorChanged)
            {
                //color Buffer
                GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
                GL.BufferSubData<Vector4>(BufferTarget.ArrayBuffer, (IntPtr)(0 * 16), drawVertexOffset * 16, colorData);
            }
            vertexChanged = sourceChanged = colorChanged = false;
        }
        public void Reset()
        {
            indexOffset = 0;
            vertexOffset = 0;
            maxIndex = 0;
        }
        public void Clear()
        {
            Reset();
            for (int i = 0; i < positionData.Length; i++)
            {
                positionData[i] = new Vector2(0, 0);
                texturePosData[i] = new Vector3(0, 0, 0);
                colorData[i] = new Vector4(255, 255, 255, 255);
            }
            vertexChanged = sourceChanged = colorChanged = true;
        }
        public void DrawImage(Texture texture, RectangleF dst, Color color)
        {
            DrawImage(texture, new Rectangle(0, 0, texture.Width, texture.Height), dst, color);
        }
        public void DrawImage(Texture texture, RectangleF src, RectangleF dst, Color color)
        {

            positionData[vertexOffset + 0] = new Vector2(dst.X, dst.Y); //ol
            positionData[vertexOffset + 1] = new Vector2(dst.X + dst.Width, dst.Y); //or
            positionData[vertexOffset + 2] = new Vector2(dst.X + dst.Width, dst.Y + dst.Height); //ur
            positionData[vertexOffset + 3] = new Vector2(dst.X, dst.Y + dst.Height); //ul

            float texX = texture.px + src.X+0.5f, texY = texture.py + src.Y+0.5f;
            texturePosData[vertexOffset + 0] = new Vector3(texX, texY, texture.z); //ol
            texturePosData[vertexOffset + 1] = new Vector3(texX + src.Width-1, texY, texture.z); //or
            texturePosData[vertexOffset + 2] = new Vector3(texX + src.Width-1, texY + src.Height-1, texture.z); //ur
            texturePosData[vertexOffset + 3] = new Vector3(texX, texY + src.Height-1, texture.z); //ul

            colorData[vertexOffset + 0] =
            colorData[vertexOffset + 1] =
            colorData[vertexOffset + 2] =
            colorData[vertexOffset + 3] = new Vector4(color.R, color.G, color.B, color.A);

            vertexOffset += 4;
            indexOffset += 6;
            vertexChanged = sourceChanged = colorChanged = true;
        }
        public void UpdateDst(RectangleF dst)
        {
            positionData[vertexOffset + 0] = new Vector2(dst.X, dst.Y); //ol
            positionData[vertexOffset + 1] = new Vector2(dst.X + dst.Width, dst.Y); //or
            positionData[vertexOffset + 2] = new Vector2(dst.X + dst.Width, dst.Y + dst.Height); //ur
            positionData[vertexOffset + 3] = new Vector2(dst.X, dst.Y + dst.Height); //ul

            vertexChanged = true;
        }
        public void UpdateSrc(Texture texture, RectangleF src)
        {
            float texX = texture.px + src.X + 0.5f, texY = texture.py + src.Y + 0.5f;
            texturePosData[vertexOffset + 0] = new Vector3(texX, texY, texture.z); //ol
            texturePosData[vertexOffset + 1] = new Vector3(texX + src.Width - 1, texY, texture.z); //or
            texturePosData[vertexOffset + 2] = new Vector3(texX + src.Width - 1, texY + src.Height - 1, texture.z); //ur
            texturePosData[vertexOffset + 3] = new Vector3(texX, texY + src.Height - 1, texture.z); //ul

            sourceChanged = true;
        }
        public void UpdateColor(Color color)
        {
            colorData[vertexOffset + 0] =
            colorData[vertexOffset + 1] =
            colorData[vertexOffset + 2] =
            colorData[vertexOffset + 3] = new Vector4(color.R, color.G, color.B, color.A);

            colorChanged = true;
        }
    }
}

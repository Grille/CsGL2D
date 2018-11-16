using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CsGL2D;

namespace CsGL2D_FormTest
{
    public partial class Form1 : Form
    {
        DrawBuffer drawBuffer,drawBuffer2;
        Texture texture;
        Shader shader;
        Context ctx;
        public Form1()
        {
            Console.WriteLine("start");
            InitializeComponent();

            texture = new Texture("test.png");

            ctx = new Context(this);
            //GL2D.SetRenderControl(this);

            shader = new Shader();

            drawBuffer = new DrawBuffer(264000);
            drawBuffer2 = new DrawBuffer(4);

            //this.CreateGraphics().Transform.

            int size = 20;
            int scale = 512;
            for (int ix = 0; ix < scale; ix++)
                for (int iy = 0; iy < scale; iy++)
                {
                    drawBuffer.DrawImage(texture, new Rectangle(ix * size, iy * size, size, size), Color.DarkGray);
                }
            drawBuffer.Update();

        }

        private void render()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine();

            ctx.Clear(Color.Black);


            ctx.Render(drawBuffer, shader);
            drawBuffer2.Clear();
            drawBuffer2.DrawImage(texture, new Rectangle(64, 64, 64, 64), Color.Lime);
            drawBuffer2.DrawImage(texture, new Rectangle(512, 512, 64, 64), Color.White);
            drawBuffer2.DrawImage(texture, new Rectangle(this.ClientSize.Width - 64, this.ClientSize.Height - 64, 64, 64), Color.Blue);
            drawBuffer2.Update();
            ctx.Render(drawBuffer2, shader);
            ctx.Refresh();
            //GL2D.SwapBuffers();

            stopwatch.Stop();
            label1.Text = ""+stopwatch.ElapsedMilliseconds;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            render();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            render();
        }
    }
}

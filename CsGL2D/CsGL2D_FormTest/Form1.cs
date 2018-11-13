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
        Texture texture,texture2;
        public Form1()
        {
            Console.WriteLine("start");
            InitializeComponent();



            GL2D.SetRenderControl(this);
            GL2D.LogError("Init control");
            GL2D.UseShader(GL2D.CreateShader());
            GL2D.LogError("Init shader");
            GL2D.CreateBuffer(264000+4);
            GL2D.LogError("Init buffer");
            GL2D.LogError("Init cleanup");

            Console.WriteLine("texture");
            texture = new Texture("test.png");
            texture2 = new Texture("test2.png");
            Console.WriteLine("texture");

            TextureAtlas._DEBUG();
            GL2D.LogError("Texture");
        }

        private void render()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine();
            GL2D.ClearBuffer(Color.Black);
            Random rnd = new Random();
            int size = 20;
            int scale = 512;
            for (int ix = 0; ix < 256; ix++)
                for (int iy = 0; iy < 256; iy++)
                {
                    GL2D.DrawImage(texture2, new Rectangle(ix * size, iy * size, size, size), Color.DarkGray);
                    GL2D.DrawImage(texture, new Rectangle(ix * size, iy * size, size, size), Color.DarkGray);
                }
            GL2D.DrawImage(texture, new Rectangle(64,64,64,64), Color.Lime);
            GL2D.DrawImage(texture2, new Rectangle(0, 128, 128, 128), Color.White);
            GL2D.DrawImage(texture, new Rectangle(512, 512, 64, 64), Color.White);
            GL2D.DrawImage(texture, new Rectangle(this.ClientSize.Width-64, this.ClientSize.Height-64, 64, 64), Color.Blue);
            GL2D.UpdateBuffer();
            GL2D.LogError("Update");
            GL2D.Render();
            GL2D.LogError("Render");
            GL2D.SwapBuffers();
            GL2D.LogError("Swap");

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

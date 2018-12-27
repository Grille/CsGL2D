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
        Texture testTexture,groundTexture,houseTexture;
        Shader shader;
        Context ctx;
        float size = 1f;
        public Form1()
        {
            Console.WriteLine("start");
            InitializeComponent();


            groundTexture = new Texture("../assets/ground.png");
            houseTexture = new Texture("../assets/house.png");
            testTexture = new Texture("../assets/test.png");

            ctx = new Context(this);
            //GL2D.SetRenderControl(this);

            shader = new Shader();

            drawBuffer = new DrawBuffer(264000);
            drawBuffer2 = new DrawBuffer(400);

            //this.CreateGraphics().Transform.

            int size = 64;
            int scale = 8;
            for (int ix = 0; ix < scale; ix++)
                for (int iy = 0; iy < scale; iy++)
                {
                    drawBuffer.DrawImage(groundTexture,new Rectangle(64,0,64,64), new Rectangle(ix * size, iy * size, size, size), Color.Green);
                }
        }

        int index = 0;


        float count = 16;
        private void render()
        {
      
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ctx.Clear(Color.Black);



            //drawBuffer.Index = index++;
            //drawBuffer.UpdateColor(Color.Red);

            ctx.Render(drawBuffer, shader);
            //for (int i=0;i< 4; i++) ctx.Render(drawBuffer, shader, i* 512, 4);



            drawBuffer2.Reset();

            this.size *= 1.01f;
            if (size > 1) size = 1f;
            int tileSize = 16;
            float drawSize = (tileSize*size);
            count -= 1f;
            for (int ix = 0; ix < 16; ix++)
            {
                for (int iy = 0; iy < (int)(count); iy++)
                {
                    drawBuffer2.DrawImage(testTexture, new Rectangle((int)(ix/8)*16, (int)(iy / 8) * 16, 16,16),new RectangleF(drawSize * ix, drawSize * iy, drawSize, drawSize), Color.White);
                }
            }

            Console.WriteLine(drawBuffer2.Index);
            Console.WriteLine(drawBuffer2.getLenght()/6);
            /*
            drawBuffer2.DrawImage(houseTexture, new Rectangle(128, 64, 96, 96), Color.White);
            drawBuffer2.DrawImage(houseTexture, new Rectangle(64, 64, 96, 96), Color.White);
            drawBuffer2.DrawImage(houseTexture, new Rectangle(512, 512, 96, 96), Color.White);
            drawBuffer2.DrawImage(houseTexture, new Rectangle(this.ClientSize.Width - 64, this.ClientSize.Height - 64, 96, 96), Color.Blue);


            drawBuffer2.DrawImage(groundTexture, new Rectangle(0 - 2, 0 + 2, 4, 4), Color.Blue);
            drawBuffer2.DrawImage(groundTexture, new Rectangle(200 - 2, 0 + 2, 4, 4), Color.Blue);
            */

            float rx = 0,ry = 0;
            for (float i = 0; i <= 10; i++)
            {
                rx += 0.1f;
                ry -= 0.1f;
                //drawBuffer2.DrawImage(groundTexture, new Rectangle(rx -2, + 2, 4, 4), Color.Blue);
            }

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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
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

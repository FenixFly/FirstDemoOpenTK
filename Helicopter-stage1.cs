using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Helicopter
{
    public partial class Form1 : Form
    {
        bool loaded = false;

        float eyeX = 3.0f;
        float eyeY = 4.0f;
        float eyeZ = 5.0f;

        float directionX = 0.0f;
        float directionY = 0.0f;
        float directionZ = 0.0f;

        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;
            SetupViewport();
        }

        private void SetupViewport()
        {
            GL.ClearColor(Color.DarkGray);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.DepthTest);

            int viewportWidth = glControl1.Width;
            int viewportHeight = glControl1.Height;
            
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(
                                            MathHelper.PiOver4, 
                                            viewportWidth / (float)viewportHeight, 
                                            1, 
                                            64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            SetupViewport();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            if (!loaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 projectionMat = Matrix4.LookAt(eyeX, eyeY, eyeZ, 
                                                    directionX, directionY, directionZ, 
                                                    0, 0, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref projectionMat);
            drawFigure();
            glControl1.SwapBuffers();
        }

        private void drawFigure()
        {
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(Color.Black);
            GL.Vertex3(-1.0f, -0.1f, 1.0f);
            GL.Vertex3(-1.0f,  0.1f, 1.0f);
            GL.Vertex3( 1.0f,  0.1f, 1.0f);
            GL.Vertex3( 1.0f, -0.1f, 1.0f);

            GL.Color3(Color.Black);
            GL.Vertex3(-0.1f, -1.0f, 1.0f);
            GL.Vertex3( 0.1f, -1.0f, 1.0f);
            GL.Vertex3( 0.1f,  1.0f, 1.0f);
            GL.Vertex3(-0.1f,  1.0f, 1.0f);

            GL.End();
            GL.Color3(Color.Blue);
            sphere(1, 20, 20);
        }

        void sphere(double r, int nx, int ny)
        {
            int i, ix, iy;
            double x, y, z;

            for (iy = 0; iy < ny; ++iy)
            {
                GL.Begin(BeginMode.QuadStrip);
                for (ix = 0; ix <= nx; ++ix)
                {
                    x = r * Math.Sin(iy * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx);
                    y = r * Math.Sin(iy * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx);
                    z = r * Math.Cos(iy * Math.PI / ny);
                    GL.Normal3(x, y, z);
                    GL.TexCoord2((double)ix / (double)nx, (double)iy / (double)ny);
                    GL.Vertex3(x, y, z);

                    x = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Cos(2 * ix * Math.PI / nx);
                    y = r * Math.Sin((iy + 1) * Math.PI / ny) * Math.Sin(2 * ix * Math.PI / nx);
                    z = r * Math.Cos((iy + 1) * Math.PI / ny);
                    GL.Normal3(x, y, z);
                    GL.TexCoord2((double)ix / (double)nx, (double)(iy + 1) / (double)ny);
                    GL.Vertex3(x, y, z);
                }
                GL.End();
            }
        }
    }
}

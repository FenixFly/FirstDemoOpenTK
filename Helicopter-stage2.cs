using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        float rotation = 0;

        int TextureID = -1;

        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;
            SetupViewport();
            Application.Idle += Application_Idle;
            TextureID = loadTexture("texture_UNN.bmp");
        }
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                rotation = (rotation + 0.11f) % 360;
                Render();
            }
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
            DrawGrid(Color.Green, 0, 0, -1, 1, 256);
            glControl1.SwapBuffers();
        }

        private void drawFigure()
        {
            GL.Begin(PrimitiveType.Triangles);

            GL.Color3(Color.Black);
            GL.Vertex3(1.5f * Math.Sin(rotation + 0.2f), 1.5f * Math.Cos(rotation + 0.2f), 1.0f);
            GL.Vertex3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(1.5f * Math.Sin(rotation - 0.2f), 1.5f * Math.Cos(rotation - 0.2f), 1.0f);

            GL.Vertex3(1.5f * Math.Sin(rotation + 0.2f + 3.14f / 2.0f), 1.5f * Math.Cos(rotation + 0.2f + 3.14f / 2.0f), 1.0f);
            GL.Vertex3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(1.5f * Math.Sin(rotation - 0.2f + 3.14f / 2.0f), 1.5f * Math.Cos(rotation - 0.2f + 3.14f / 2.0f), 1.0f);

            GL.Vertex3(1.5f * Math.Sin(rotation + 0.2f + 2 * 3.14f / 2.0f), 1.5f * Math.Cos(rotation + 0.2f + 2 * 3.14f / 2.0f), 1.0f);
            GL.Vertex3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(1.5f * Math.Sin(rotation - 0.2f + 2 * 3.14f / 2.0f), 1.5f * Math.Cos(rotation - 0.2f + 2 * 3.14f / 2.0f), 1.0f);

            GL.Vertex3(1.5f * Math.Sin(rotation + 0.2f + 3 * 3.14f / 2.0f), 1.5f * Math.Cos(rotation + 0.2f + 3 * 3.14f / 2.0f), 1.0f);
            GL.Vertex3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(1.5f * Math.Sin(rotation - 0.2f + 3 * 3.14f / 2.0f), 1.5f * Math.Cos(rotation - 0.2f + 3 * 3.14f / 2.0f), 1.0f);


            GL.End();
            GL.Color3(Color.LightGray);
            GL.Enable(EnableCap.Texture2D);
            sphere(1, 20, 20);
            GL.Disable(EnableCap.Texture2D);
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

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!loaded)
                return;
            float dX = eyeX - directionX;
            float dY = eyeY - directionY;
            float length = (float)Math.Sqrt(dX * dX + dY * dY);
            switch (e.KeyCode)
            {
                case Keys.W:
                    break;
                case Keys.S:
                    break;
                case Keys.A:
                    break;
                case Keys.D:
                    break;
            }
            glControl1.Invalidate();
        }

        int loadTexture(string fileName)
        {
            try
            {
                Bitmap image = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\texture_UNN.bmp");

                int texID = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2D, texID);
                BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                image.UnlockBits(data);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                return texID;
            }
            catch (FileNotFoundException е)
            {
                return -1;
            }
        }
        
        public void DrawGrid(System.Drawing.Color color, float X, float Y, float Z, int cell_size = 1, int grid_size = 256)
        {
            int dX = (int)Math.Round(X / cell_size) * cell_size;
            int dY = (int)Math.Round(Y / cell_size) * cell_size;

            int ratio = grid_size / cell_size;

            GL.PushMatrix();

            GL.Translate(dX - grid_size / 2, dY - grid_size / 2, 0);

            int i;

            GL.Color3(color);
            GL.Begin(BeginMode.Lines);

            for (i = 0; i < ratio + 1; i++)
            {
                int current = i * cell_size;

                GL.Vertex3(current, 0, Z);
                GL.Vertex3(current, grid_size, Z);

                GL.Vertex3(0, current, Z);
                GL.Vertex3(grid_size, current, Z);
            }

            GL.End();

            GL.PopMatrix();
        }
    }
}
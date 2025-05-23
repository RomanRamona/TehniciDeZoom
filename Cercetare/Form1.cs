using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cercetare
{
    public partial class Form1 : Form
    {
        
        Bitmap currentImage;
        Bitmap originalImage;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            originalImage = new Bitmap(@"..\..\pisica.jpeg");
            currentImage = new Bitmap(originalImage); 
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = originalImage;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentImage = ZoomStelat(currentImage);
            pictureBox2.Image = currentImage;
            

        }

        private Bitmap ZoomStelat(Bitmap s)
        {
            int newWidth = s.Width * 2 - 1;
            int newHeight = s.Height * 2 - 1;
            Bitmap d = new Bitmap(newWidth, newHeight);

            // Copiem pixelii la poziții pare
            for (int i = 0; i < s.Width; i++)
                for (int j = 0; j < s.Height; j++)
                    d.SetPixel(i * 2, j * 2, s.GetPixel(i, j));

            // Interpolare orizontală (coloane impare, rânduri pare)
            for (int i = 0; i < newWidth; i += 2)
                for (int j = 1; j < newHeight - 1; j += 2)
                {
                    Color c1 = d.GetPixel(i, j - 1);
                    Color c2 = d.GetPixel(i, j + 1);
                    d.SetPixel(i, j, Color.FromArgb((c1.R + c2.R) / 2, (c1.G + c2.G) / 2, (c1.B + c2.B) / 2));
                }

            // Interpolare verticală (rânduri impare, coloane pare)
            for (int i = 1; i < newWidth - 1; i += 2)
                for (int j = 0; j < newHeight; j++)
                {
                    Color c1 = d.GetPixel(i - 1, j);
                    Color c2 = d.GetPixel(i + 1, j);
                    d.SetPixel(i, j, Color.FromArgb((c1.R + c2.R) / 2, (c1.G + c2.G) / 2, (c1.B + c2.B) / 2));
                }

            // Interpolare stelată (diagonale)
            for (int i = 0; i < s.Width - 1; i++)
                for (int j = 0; j < s.Height - 1; j++)
                {
                    int x = 2 * i + 1;
                    int y = 2 * j + 1;

                    if (x - 1 >= 0 && x + 1 < newWidth && y - 1 >= 0 && y + 1 < newHeight)
                    {
                        Color c1 = d.GetPixel(x - 1, y - 1);
                        Color c2 = d.GetPixel(x + 1, y - 1);
                        Color c3 = d.GetPixel(x - 1, y + 1);
                        Color c4 = d.GetPixel(x + 1, y + 1);
                        d.SetPixel(x, y, Color.FromArgb(
                            (c1.R + c2.R + c3.R + c4.R) / 4,
                            (c1.G + c2.G + c3.G + c4.G) / 4,
                            (c1.B + c2.B + c3.B + c4.B) / 4));
                    }
                }
            return d;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentImage = new Bitmap(originalImage);
            pictureBox2.Image=currentImage;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            currentImage = ZoomCruce(currentImage); 
            pictureBox2.Image = currentImage;
        }
        private Bitmap ZoomCruce(Bitmap s)
        {
            int newWidth = s.Width * 2 - 1;
            int newHeight = s.Height * 2 - 1;
            Bitmap d = new Bitmap(newWidth, newHeight);

            // Copiem pixelii la poziții pare
            for (int i = 0; i < s.Width; i++)
                for (int j = 0; j < s.Height; j++)
                    d.SetPixel(i * 2, j * 2, s.GetPixel(i, j));

            // Interpolare orizontală (coloane impare, rânduri pare)
            for (int i = 0; i < newWidth; i += 2)
                for (int j = 1; j < newHeight - 1; j += 2)
                {
                    Color c1 = d.GetPixel(i, j - 1);
                    Color c2 = d.GetPixel(i, j + 1);
                    d.SetPixel(i, j, Color.FromArgb(
                        (c1.R + c2.R) / 2,
                        (c1.G + c2.G) / 2,
                        (c1.B + c2.B) / 2));
                }

            // Interpolare verticală (rânduri impare, coloane pare)
            for (int i = 1; i < newWidth - 1; i += 2)
                for (int j = 0; j < newHeight; j += 2)
                {
                    Color c1 = d.GetPixel(i - 1, j);
                    Color c2 = d.GetPixel(i + 1, j);
                    d.SetPixel(i, j, Color.FromArgb(
                        (c1.R + c2.R) / 2,
                        (c1.G + c2.G) / 2,
                        (c1.B + c2.B) / 2));
                }

            // Interpolare în centru (cruce)
            for (int i = 1; i < newWidth - 1; i += 2)
                for (int j = 1; j < newHeight - 1; j += 2)
                {
                    Color c1 = d.GetPixel(i - 1, j); // stânga
                    Color c2 = d.GetPixel(i + 1, j); // dreapta
                    Color c3 = d.GetPixel(i, j - 1); // sus
                    Color c4 = d.GetPixel(i, j + 1); // jos

                    d.SetPixel(i, j, Color.FromArgb(
                        (c1.R + c2.R + c3.R + c4.R) / 4,
                        (c1.G + c2.G + c3.G + c4.G) / 4,
                        (c1.B + c2.B + c3.B + c4.B) / 4));
                }

            return d;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            currentImage = ZoomDiagonala(currentImage);
            pictureBox2.Image = currentImage;
        }
        private Bitmap ZoomDiagonala(Bitmap s)
        {
            int w = s.Width;
            int h = s.Height;
            Bitmap d = new Bitmap(w, h);

            for (int x = 1; x < w - 1; x++)
            {
                for (int y = 1; y < h - 1; y++)
                {
                    // Pixeli pe diagonale
                    Color c1 = s.GetPixel(x - 1, y - 1); // ↖
                    Color c2 = s.GetPixel(x + 1, y - 1); // ↗
                    Color c3 = s.GetPixel(x - 1, y + 1); // ↙
                    Color c4 = s.GetPixel(x + 1, y + 1); // ↘

                    // Media celor 4 diagonale
                    int r = (c1.R + c2.R + c3.R + c4.R) / 4;
                    int g = (c1.G + c2.G + c3.G + c4.G) / 4;
                    int b = (c1.B + c2.B + c3.B + c4.B) / 4;

                    d.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return d;
        }

    }
}

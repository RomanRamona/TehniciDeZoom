using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Optimizat
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
            originalImage = new Bitmap(@"..\..\poza5.jpg");
            currentImage = new Bitmap(originalImage);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = originalImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //currentImage = ZoomStelat(currentImage);
            //pictureBox2.Image = currentImage;
            float zoomFactor = 400.0f;
            currentImage = ZoomStelatStatic(currentImage, zoomFactor);
            pictureBox2.Image = currentImage;
        }
        private Bitmap ZoomStelatStatic(Bitmap source, float zoomFactor)
        {
            if (zoomFactor <= 1.0f)
                return new Bitmap(source);

            int srcWidth = source.Width;
            int srcHeight = source.Height;

            // Calculăm zona de crop din centru
            int cropWidth = (int)(srcWidth / zoomFactor);
            int cropHeight = (int)(srcHeight / zoomFactor);
            int cropX = (srcWidth - cropWidth) / 2;
            int cropY = (srcHeight - cropHeight) / 2;

            Rectangle cropRect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropped = source.Clone(cropRect, source.PixelFormat);

            // Zoom algoritmic (stelat)
            Bitmap zoomed = ZoomStelat(cropped);

            // Redimensionare la dimensiunea originală
            Bitmap final = new Bitmap(srcWidth, srcHeight);
            using (Graphics g = Graphics.FromImage(final))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(zoomed, new Rectangle(0, 0, srcWidth, srcHeight));
            }

            cropped.Dispose();
            zoomed.Dispose();
            return final;
        }


        private Bitmap ZoomStelat(Bitmap source)
        {
            int srcWidth = source.Width;
            int srcHeight = source.Height;
            int newWidth = srcWidth * 2 - 1;
            int newHeight = srcHeight * 2 - 1;

            Bitmap result = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            BitmapData srcData = source.LockBits(new Rectangle(0, 0, srcWidth, srcHeight),
                                                 ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dstData = result.LockBits(new Rectangle(0, 0, newWidth, newHeight),
                                                 ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int srcStride = srcData.Stride;
            int dstStride = dstData.Stride;

            byte[] srcBytes = new byte[srcStride * srcHeight];
            byte[] dstBytes = new byte[dstStride * newHeight];

            System.Runtime.InteropServices.Marshal.Copy(srcData.Scan0, srcBytes, 0, srcBytes.Length);

            // Pas 1: Copierea pixelilor originali în poziții pare
            for (int y = 0; y < srcHeight; y++)
            {
                for (int x = 0; x < srcWidth; x++)
                {
                    int srcIndex = y * srcStride + x * 3;
                    int dx = x * 2;
                    int dy = y * 2;
                    int dstIndex = dy * dstStride + dx * 3;

                    dstBytes[dstIndex] = srcBytes[srcIndex];       // Blue
                    dstBytes[dstIndex + 1] = srcBytes[srcIndex + 1]; // Green
                    dstBytes[dstIndex + 2] = srcBytes[srcIndex + 2]; // Red
                }
            }

            // Pas 2: Interpolare orizontală
            for (int y = 0; y < newHeight; y += 2)
            {
                for (int x = 1; x < newWidth - 1; x += 2)
                {
                    int leftIndex = y * dstStride + (x - 1) * 3;
                    int rightIndex = y * dstStride + (x + 1) * 3;
                    int dstIndex = y * dstStride + x * 3;

                    for (int c = 0; c < 3; c++)
                        dstBytes[dstIndex + c] = (byte)((dstBytes[leftIndex + c] + dstBytes[rightIndex + c]) / 2);
                }
            }

            // Pas 3: Interpolare verticală
            for (int y = 1; y < newHeight - 1; y += 2)
            {
                for (int x = 0; x < newWidth; x += 2)
                {
                    int topIndex = (y - 1) * dstStride + x * 3;
                    int bottomIndex = (y + 1) * dstStride + x * 3;
                    int dstIndex = y * dstStride + x * 3;

                    for (int c = 0; c < 3; c++)
                        dstBytes[dstIndex + c] = (byte)((dstBytes[topIndex + c] + dstBytes[bottomIndex + c]) / 2);
                }
            }

            // Pas 4: Interpolare stelată (diagonale)
            for (int y = 1; y < newHeight - 1; y += 2)
            {
                for (int x = 1; x < newWidth - 1; x += 2)
                {
                    int p1 = (y - 1) * dstStride + (x - 1) * 3;
                    int p2 = (y - 1) * dstStride + (x + 1) * 3;
                    int p3 = (y + 1) * dstStride + (x - 1) * 3;
                    int p4 = (y + 1) * dstStride + (x + 1) * 3;
                    int dstIndex = y * dstStride + x * 3;

                    for (int c = 0; c < 3; c++)
                        dstBytes[dstIndex + c] = (byte)((dstBytes[p1 + c] + dstBytes[p2 + c] + dstBytes[p3 + c] + dstBytes[p4 + c]) / 4);
                }
            }

            // Copiem datele înapoi în bitmap
            System.Runtime.InteropServices.Marshal.Copy(dstBytes, 0, dstData.Scan0, dstBytes.Length);
            source.UnlockBits(srcData);
            result.UnlockBits(dstData);

            return result;
        }



        private void button2_Click(object sender, EventArgs e)
        {
            currentImage = new Bitmap(originalImage);
            pictureBox2.Image = currentImage;
        }
    }
}

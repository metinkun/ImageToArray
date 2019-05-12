using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ARGA;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace imageToArray
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        bool closing = false;
        Bitmap tempBitmap;
        String bitmapPath = "";
        String rawPath = "";
        String headerFile = "";
        String SourceFile = "";
        String rawFile = "";
        byte[] rawByteArray;
        

        Bitmap bmp;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bitmapPath.Length > 1)
                openFileDialog1.InitialDirectory = bitmapPath;
            openFileDialog1.Filter = "Image Files (JPG,BMP)|*.JPG;*.BMP;";
            
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {

                bitmapPath = openFileDialog1.FileName.Substring(0, openFileDialog1.FileName.LastIndexOf("\\"));
                pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
                
                string extens = openFileDialog1.FileName;
                extens = extens.Substring(extens.LastIndexOf(".") + 1);

                
                if (extens == "jpg" || extens == "JPG")
                {
                    tempBitmap = new Bitmap(ConvertToBitmap(openFileDialog1.OpenFile()));
                }
                else if (extens == "bmp" || extens == "BMP")
                {
                    tempBitmap = new Bitmap(openFileDialog1.OpenFile());
                }
                else
                {
                    return;
                }
                pictureBox1.BackgroundImage = tempBitmap;
                label3.Text = openFileDialog1.FileName;


            }
            
            //ImageToByte();
        }

        public UInt16 Color565(byte r, byte g, byte b)
        {
            return (UInt16)((UInt16)(((r & 0xF8) << 8)) | (UInt16)((g & 0xFC) << 3) | (UInt16)(b >> 3));
        }
        public Bitmap ConvertToBitmap(Stream stream)
        {
            Bitmap bitmap;
            Image temp = Image.FromStream(stream);
            bitmap = new Bitmap(temp);
            return bitmap;
        }
        Color color565toColor(UInt16 Color565)
        {
            byte r, g, b;
            r = (byte)((byte)(Color565 >> 11) << 3);
            g = (byte)((byte)(Color565 >> 5) << 2);
            b = (byte)(Color565 << 3);
            return Color.FromArgb( 255 ,r, g, b);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
            Size size = tempBitmap.Size;

            UInt16[] pixels = new UInt16[size.Height * size.Width];
            rawByteArray = new byte[(size.Height * size.Width * 2) + 2 + 2 + 2];
            rawByteArray[0] = (byte)'A';
            rawByteArray[1] = (byte)'R';
            rawByteArray[2] = (byte)size.Height;
            rawByteArray[3] = (byte)size.Width;
            rawByteArray[4 + (size.Height * size.Width * 2)] = (byte)'G';
            rawByteArray[5 + (size.Height * size.Width * 2)] = (byte)'A';
            for (byte a = 0; a < size.Width; a++)
            {
                for (byte b = 0; b < size.Height; b++)
                {
                    //byte g = tempBitmap.GetPixel(a, b).A;
                    //if (g != 255)
                    //    g = 0;
                    pixels[size.Width * b + a] = Color565(tempBitmap.GetPixel(a, b).R, tempBitmap.GetPixel(a, b).G, tempBitmap.GetPixel(a, b).B);
                    rawByteArray[4 + ((size.Width * b + a) * 2)] = (byte)(pixels[size.Width * b + a] >> 8);
                    rawByteArray[4 + ((size.Width * b + a) * 2) + 1] = (byte)(pixels[size.Width * b + a]);
                }
            }

            //bmp = new Bitmap(rawByteArray[3], rawByteArray[2], PixelFormat.Format8bppIndexed);

            //ColorPalette ncp = bmp.Palette;
            //for (int i = 0; i < 256; i++)
            //    ncp.Entries[i] = Color.FromArgb(255, i, i, i);
            //bmp.Palette = ncp;

            //var BoundsRect = new Rectangle(0, 0, rawByteArray[3], rawByteArray[2]);
            //BitmapData bmpData = bmp.LockBits(BoundsRect,
            //                                ImageLockMode.WriteOnly,
            //                                bmp.PixelFormat);

            //IntPtr ptr = bmpData.Scan0;

            //int bytes = bmpData.Stride * bmp.Height;
            //var rgbValues = new byte[bytes];
            //bmp = new Bitmap(rawByteArray[3], rawByteArray[2] ,PixelFormat.Format24bppRgb);
            //byte[] byteArray = new byte[((rawByteArray.Length - 6) / 2) * 3];
            //for(int v = 0; v < ((rawByteArray.Length - 6) / 2);v++)
            //{
            //    byteArray[v * 3]=color565toColor((UInt16)(rawByteArray[4 + (v * 2)] << 8 | rawByteArray[4 + (v * 2) + 1])).R;
            //    byteArray[v * 3 + 1] = color565toColor((UInt16)(rawByteArray[4 + (v * 2)] << 8 | rawByteArray[4 + (v * 2) + 1])).G;
            //    byteArray[v * 3 + 2] = color565toColor((UInt16)(rawByteArray[4 + (v * 2)] << 8 | rawByteArray[4 + (v * 2) + 1])).B;
            //}

            //using (MemoryStream mStream = new MemoryStream())
            //{
            //    mStream.Write(byteArray, 0, byteArray.Length);
            //    mStream.Seek(0, SeekOrigin.Begin);

            //    bmp = new Bitmap(mStream);
            //}

            //MemoryStream bitmapDataStream = new MemoryStream(byteArray);
            //bmp = new Bitmap(bitmapDataStream);


            //bmp = tempBitmap;
            //for (int i = 0; i < rawByteArray[3]; i++)
            //    for (int j = 0; j < rawByteArray[2]; j++)
            //        //bmp.SetPixel(i, j,color565toColor(Color565(Color.White.R, Color.White.G, Color.White.B)));
            //bmp.SetPixel(i, j,  color565toColor((UInt16)(rawByteArray[4 + (i*j * 2)] << 8 | rawByteArray[4 + (i * j * 2) + 1])));
            //for (int f = 0; f < rawByteArray[3] * rawByteArray[2];f++)
            //{
            //    if (f % 3 == 0)
            //        rgbValues[f] = color565toColor((UInt16)(rawByteArray[4 + (f * 2)]<<8 | rawByteArray[4 + (f * 2)+ 1])).R;
            //    else if (f % 3 == 1)
            //        rgbValues[f] = color565toColor((UInt16)(rawByteArray[4 + (f * 2)] << 8 | rawByteArray[4 + (f * 2) + 1])).G;
            //    else
            //        rgbValues[f] = color565toColor((UInt16)(rawByteArray[4 + (f * 2)] << 8 | rawByteArray[4 + (f * 2) + 1])).B;
            //}

            // fill in rgbValues, e.g. with a for loop over an input array

            //Marshal.Copy(rgbValues, 0, ptr, bytes);
            //bmp.UnlockBits(bmpData);

            string temp = "";
            UInt16 c = 0;
            richTextBox1.AppendText(textBox1.Text +"\r\n"+ "{" +"\r\n");
            for (; c < (size.Height * size.Width)/32; c++)
            {
                temp = pixels.Uint16ArrayToString(" , ", c*32, 32, Converters.FORMAT.HEX)+ " , ";
                richTextBox1.AppendText(temp+"\r\n");
            }
            temp = pixels.Uint16ArrayToString(" , ", c * 32, (size.Height * size.Width) % 32, Converters.FORMAT.HEX);

            richTextBox1.AppendText(temp+ "\r\n" + "};");
            temp = rawByteArray.ByteArrayToString("-", Converters.FORMAT.HEX);
            richTextBox2.AppendText(temp + "\r\n");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "header files (H,HPP)|*.H;*.HPP;";
            if (DialogResult.OK == saveFileDialog1.ShowDialog())
            {
                headerFile = saveFileDialog1.FileName;
                using (StreamWriter writer = new StreamWriter(headerFile, true))
                {
                    writer.Write("");
                    writer.Close();
                }
                String tempStr = headerFile.Substring(0, headerFile.LastIndexOf("."));
                
                if (File.Exists(tempStr + ".cpp"))
                {
                    SourceFile = tempStr + ".cpp";
                }
                else
                    SourceFile = tempStr + ".c";
                textBox2.Text = headerFile;
                textBox3.Text = SourceFile;
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "source files (C,CPP)|*.C;*.CPP;";
            if (DialogResult.OK == saveFileDialog1.ShowDialog())
            {
                SourceFile = saveFileDialog1.FileName;
                using (StreamWriter writer = new StreamWriter(SourceFile, true))
                {
                    writer.Write("");
                    writer.Close();
                }
                String tempStr = SourceFile.Substring(0, SourceFile.LastIndexOf("."));

                if (File.Exists(tempStr + ".hpp"))
                {
                    headerFile = tempStr + ".hpp";
                }
                else
                    headerFile = tempStr + ".h";
                textBox2.Text = headerFile;
                textBox3.Text = SourceFile;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(SourceFile, true))
                {
                    writer.Write(richTextBox1.Text);
                    writer.Close();
                }
                using (StreamWriter writer = new StreamWriter(headerFile, true))
                {
                    writer.Write("extern " +textBox1.Text.Substring(0 ,textBox1.Text.LastIndexOf("]")+1) + ";");
                    writer.Close();
                }
            }
            catch { }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length < 1)
                button5.Enabled = false;
            else
                button5.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (rawPath.Length > 1)
                saveFileDialog1.InitialDirectory = rawPath;
            saveFileDialog1.Filter = "Arga Raw Image (argri)|*.argri;";
            if (DialogResult.OK == saveFileDialog1.ShowDialog())
            {
                rawPath = saveFileDialog1.FileName.Substring(0, saveFileDialog1.FileName.LastIndexOf("\\"));
                rawFile = saveFileDialog1.FileName;
                if (checkBox1.Checked)
                {
                    if (File.Exists(rawFile)) // if file exists , clear it
                        File.WriteAllText(rawFile, String.Empty);
                }
                using (StreamWriter writer = new StreamWriter(rawFile, true))
                {
                    //writer.Write("");
                    writer.Close();
                }
                textBox4.Text = rawFile;
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox2.Text.Length < 1)
                button6.Enabled = false;
            else
                button6.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(rawFile, FileMode.Append)))
                {
                    writer.Write(rawByteArray);
                    writer.Close();
                }
            }
            catch { }
        }
    }
}

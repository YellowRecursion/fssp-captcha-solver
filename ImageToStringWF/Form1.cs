using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace ImageToStringWF
{
    public partial class Form1 : Form
    {
        Thread t;
        string path;

        public Form1()
        {
            InitializeComponent();
            path = "";
            t = new Thread(MainUpdate);
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        static string input = "";
        static string debugPath = @"C:\Users\Vadim\Desktop\ssss";
        static string output = "";
        static int n = 0;
        static Color color;
        static Bitmap image;
        static Bitmap refImg;

        static ImageCodecInfo jpgEncoder;
        static EncoderParameters myEncoderParameters;

        //static char alf = new {'й', 'ц', 'у', 'к', 'е', 'н', 'г', 'ш', 'щ', 'з', 'х', 'ъ', 'ф', 'ы', 'в', 'а', 'п', 'р', 'о', 'л', 'д', 'ж', 'э', 'я', 'ч', 'с', 'м', 'и', 'т', 'ь', 'б', 'ю'};

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            path = folderBrowserDialog1.SelectedPath;
            label1.Text = folderBrowserDialog1.SelectedPath;
        }

        void MainUpdate()
        {
            while (true)
            {
                try
                {
                    if (path == "") printState("Ожидание указания директории");
                    else
                    {
                        if (Directory.Exists(path))
                        {
                            if (File.Exists(path + @"\input.txt"))
                            {
                                printState("Файл найден, анализирую");
                                StreamReader sr = new StreamReader(path + @"\input.txt");
                                var str = ImageToString(sr.ReadToEnd());
                                if (str.Length != 5) str = "11111";
                                if (str.Contains(" ") && str.Contains("!") && str.Contains("?")) str = "11111";
                                printState("Результат: " + str);

                                StreamWriter sw = new StreamWriter(path + @"\output.txt", false, Encoding.GetEncoding(1251));
                                sw.Write(str);
                                sr.Close();
                                sw.Close();
                                File.Delete(path + @"\input.txt");
                            }
                            else
                            {
                                //printState(ImageToString(""));
                                printState("Ожидание файла input");
                            }
                        }
                        else
                        {

                            printState("Директория неверно указана");
                        }
                    }
                    Thread.Sleep(2000);
                }
                catch
                {
                    continue;
                }
                
            }
        }

        public void printState(object obj)
        {
            label2.Invoke(new Action(() => label2.Text = "Статус: " + obj.ToString()));
        }

        public static string ImageToString(string Input)
        {
            output = "";
            Console.WriteLine("GO");
            //Console.WriteLine(Input);
            jpgEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Bmp);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;


            input = Input;
            // Загрузка кода
            //Л7РМВ
            if (input == "")
                 input = "data:image/jpeg;base64,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";
                // ж98гм
                //input = "data:image/jpeg;base64,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";
                //input = "data:image/jpeg;base64,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";

            input = input.Remove(0, 23);
            // Создание Bitmap
            byte[] bitmapData = Convert.FromBase64String(input);
            MemoryStream streamBitmap = new MemoryStream(bitmapData);
            Bitmap img = (Bitmap)Image.FromStream(streamBitmap);

            //img.Save(debugPath + @"\inputFile.bmp", jpgEncoder, myEncoderParameters);
            refImg = (Bitmap)img.Clone();

            Console.WriteLine("GO2");


            // Делаем ИЗО Ч/Б
            bool one = false;
            bool lastOne = false;
            bool start = false;
            int startWidth = 0;
            int endWidth = 0;
            int h = 0, lastH = 0;

            for (int x = 0; x < img.Width; x++)
            {

                for (int y = 0; y < img.Height; y++)
                {
                    Color c = img.GetPixel(x, y);
                    int b = c.R;
                    if (b <= 80) b = 0;
                    else b = 255;

                    c = Color.FromArgb(255, b, b, b);
                    img.SetPixel(x, y, c);

                }
            }

            //img.Save(debugPath + @"\part1.bmp", jpgEncoder, myEncoderParameters);


            // Раскрашиваем картинку 
            for (int x = 1; x < img.Width - 1; x++)
            {
                for (int y = 1; y < img.Height - 1; y++)
                {
                    Color c = img.GetPixel(x, y);
                    if (!ColorEqual(c, Color.White))
                    {
                        if (ColorEqual(c, Color.Black))
                        {
                            color = GetColor();
                            image = img;
                            Fill(x, y);
                            img = image;
                        }

                    }
                }
            }


            //img.Save(debugPath + @"\part2.bmp", jpgEncoder, myEncoderParameters);


            // Делим картинку 
            color = Color.Black;
            for (int x = 1; x < img.Width - 1; x++)
            {
                one = false;

                for (int y = 1; y < img.Height - 1; y++)
                {
                    Color c = img.GetPixel(x, y);

                    if (!ColorEqual(c, Color.White))
                    {
                        if (ColorEqual(c, color))
                        {
                            one = true;
                        }
                        else
                        {
                            color = c;
                            one = false;
                            break;
                        }

                    }
                }

                if (lastOne == false && one == true)
                {
                    start = true;
                    startWidth = x;
                }
                else if (lastOne == true && one == false)
                {
                    start = false;
                    endWidth = x;

                    if (endWidth - startWidth >= 9)
                    {
                        RectangleF cloneRect = new RectangleF(startWidth, 0, endWidth - startWidth, img.Height);
                        var litter = img.Clone(cloneRect, img.PixelFormat);
                        //litter.Save(debugPath + @"\part3_litter" + startWidth + ".bmp", jpgEncoder, myEncoderParameters);

                        output += OCR(litter);
                    }
                }

                lastOne = one;
            }
            return output;
        }

        static bool ColorEqual(Color a, Color b)
        {
            if (a.R != b.R) return false;
            if (a.G != b.G) return false;
            if (a.B != b.B) return false;

            return true;
        }

        static Color GetColor()
        {
            n++;
            if (n == 2) return Color.Blue;
            if (n == 3) return Color.Green;
            if (n == 4) return Color.Orange;
            if (n == 5) return Color.Purple;
            if (n > 5) n = 0;

            return Color.Red;
        }

        static void Fill(int x, int y)
        {
            image.SetPixel(x, y, color);
            if (x + 1 < image.Width && ColorEqual(image.GetPixel(x + 1, y), Color.Black))
            {
                Fill(x + 1, y);
            }
            if (x - 1 >= 0 && ColorEqual(image.GetPixel(x - 1, y), Color.Black))
            {
                Fill(x - 1, y);
            }
            if (y + 1 < image.Height && ColorEqual(image.GetPixel(x, y + 1), Color.Black))
            {
                Fill(x, y + 1);
            }
            if (y - 1 >= 0 && ColorEqual(image.GetPixel(x, y - 1), Color.Black))
            {
                Fill(x, y - 1);
            }
        }

        static ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        static int ocrNum = 0;
        static string OCR(Bitmap bitmap)
        {
            ocrNum++;
            var result = "";

            var bm = new Bitmap(bitmap.Width * 4 + 30, bitmap.Height);

            ocrNum++;

            for (int x = 0; x < bm.Width; x++)
            {
                for (int y = 0; y < bm.Height; y++)
                {
                    bm.SetPixel(x, y, Color.White);
                }
            }
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    if (!ColorEqual(bitmap.GetPixel(x, y), Color.White))
                        bm.SetPixel(x, y, Color.Black);
                }
            }
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    if (!ColorEqual(bitmap.GetPixel(x, y), Color.White))
                        bm.SetPixel(x + bitmap.Width + 20, y, Color.Black);
                }
            }


            //bm.Save(debugPath + @"\part4_litterBorders" + ocrNum + ".bmp", jpgEncoder, myEncoderParameters);


            var ocr = new TesseractEngine(@".\tessdata", "rus");
            var page = ocr.Process(bm);
            result = page.GetText();

            if (result.Length > 0)
            {
                return result[0].ToString();
            }
            return "?";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            t.Abort();
            t.Interrupt();
        }
    }
}

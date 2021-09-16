using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace SerialImageScaler
{
    class Program
    {
        private string PathToImages = "in";
        private double ImagePrescaler = 1;
        private long ImageQuality = 95;

        ImageCodecInfo jgpEncoder;
        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
        EncoderParameters myEncoderParameters = new EncoderParameters(1);
        Size NewSize = new Size();

        static void Main(string[] args)
        {
            Program Pr = new Program();            
        }

        public Program()
        {
            jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            Console.WriteLine("Set scale:");
            string ScaleStr = Console.ReadLine();
            try
            {
                this.ImagePrescaler = double.Parse(ScaleStr.Replace(".", ","));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Incorect image prescaler value");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Set quality (1-100):");
            string QualityStr = Console.ReadLine();
            try
            {
                this.ImageQuality = Int64.Parse(QualityStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Incorect quality value");
                Console.ReadKey();
                return;
            }

            if (this.ImageQuality < 1 || this.ImageQuality > 100)
            {
                Console.WriteLine("Quality value out of range(1-100). Set value: " + this.ImagePrescaler.ToString());
                Console.ReadKey();
                return;
            }

            this.ScaleFiles();
        }

        private void ScaleFiles()
        {
            string[] FullImagePath = { "" };
            try
            {
                FullImagePath = Directory.GetFiles(PathToImages, "*.JPG");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wrong input destination directory. There is no 'in' folder in current directory");
                Console.ReadKey();
                return;
            }

            try
            {
                int iterator = 1;
                foreach (string SingleImage in FullImagePath)
                {
                    using (Bitmap Image = new Bitmap(SingleImage))
                    {
                        // From NET
                        
                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, this.ImageQuality);
                        myEncoderParameters.Param[0] = myEncoderParameter;

                        
                        NewSize.Height = Convert.ToInt32(Image.Height * this.ImagePrescaler);
                        NewSize.Width = Convert.ToInt32(Image.Width * this.ImagePrescaler);
                        //NewSize.Height =Image.Height;
                        //NewSize.Width = Image.Width;
                        this.ResizeImage(Image, NewSize).Save(SingleImage.Replace("in", "out"), jgpEncoder, myEncoderParameters);;
                        /*Image.*/
                        Console.WriteLine("File: " + iterator.ToString() + @" \ " + FullImagePath.Length.ToString() + " Prescaler: " + this.ImagePrescaler + " Quality = " + this.ImageQuality);
                        iterator++;
                        if (iterator % 10 == 0)
                        {
                            Console.WriteLine("GC Run");
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            Console.WriteLine("GC Finish");
                        }
                    }
                }

                Console.WriteLine("\nDone\n\nPress any key to close");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wrong output destination directory. There is no 'out' folder in current directory\n\n" + ex.ToString());
                Console.ReadKey();
            }
        }


        // From NET
        public Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {
            try
            {
                if(size.Width % 8 != 0 || size.Height % 8 != 0)
                {
                    Console.WriteLine("\n\nSize is not mul by 8!!!\n\n");
                }
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    //Graphics g = Graphics.FromImage((Image)b);
                    //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }

                return b;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Resize error: \n\n" + ex.ToString());
                throw;
            }
        }

        // From NET
        private ImageCodecInfo GetEncoder(ImageFormat format)
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
    }
}

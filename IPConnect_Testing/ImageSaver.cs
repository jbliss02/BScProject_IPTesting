using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace IPConnect_Testing
{
    public class ImageSaver
    {
        List<byte[]> images; //the final image files
        static int fileNumber = 0;

        public ImageSaver()
        {
            images = new List<byte[]>();
        }

        public void ListenForImages(ImageExtractor imgClass)
        {
            imgClass.imageCreated += new ImageExtractor.ImageCreateEvent(FileCreated);
        }

        public void FileCreated(byte[] img, EventArgs e)
        {
            WriteBytesToFile( (byte[]) img);
        }

        private void WriteBytesToFile(byte[] img)
        {
            using (FileStream fs = new FileStream(GenerateFileName(), FileMode.Create))
            {
                fs.Write(img, 0, img.Length);
            }

            WriteScreen("Image creeted");
        }

        private String GenerateFileName()
        {
            return @"c:\temp\test_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_"
            + DateTime.Now.Millisecond + "_" + fileNumber + ".jpg";
        }

        private void WriteScreen(string st)
        {
            Console.WriteLine(DateTime.Now + " - " + st);
        }
    }
}

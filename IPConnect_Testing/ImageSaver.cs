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
            imgClass.imageCreated += new ImageExtractor.ImageCreatedEvent(FileCreated);
        }

        public void FileCreated(byte[] img, EventArgs e)
        {
            WriteBytesToFile( (byte[]) img);
        }

        public async Task FileCreatedAsync(byte[] img, EventArgs e)
        {
            await Task.Run(() => { WriteBytesToFile(img); } );
        }

        public async Task SaveFiles(List<byte[]> imgs)
        {
            await Task.Run(() => {
                for(int i = 0; i < imgs.Count; i++)
                {
                    WriteBytesToFile(imgs[i]);
                }

            });
        }

        private void WriteBytesToFile(byte[] img)
        {
            try
            {
                using (FileStream fs = new FileStream(GenerateFileName(), FileMode.Create))
                {
                    fs.Write(img, 0, img.Length);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(DateTime.Now + " - " + ex.Message);
            }

           // Console.WriteLine("Image Saved");
        }

        private String GenerateFileName()
        {
            //  return @"f:\temp\test_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_"
            //  + DateTime.Now.Millisecond + "_" + fileNumber + ".jpg";
            ulong hash = (UInt64)(int)DateTime.Now.Kind;
            var x = (hash << 62) | (UInt64)DateTime.Now.Ticks;

            return @"f:\temp\test_" + x.ToString() + ".jpg";

        }

        private void WriteScreen(string st)
        {
            Console.WriteLine(DateTime.Now + " - " + st);
        }
    }
}

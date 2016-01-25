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
    /// <summary>
    /// Saves bytes as images
    /// </summary>
    public class ImageSaver
    {
        List<byte[]> images; //the final image files
        static UInt64 fileNumber = 0;
        static bool sequenceFileNumbers;
        string savePath;

        /// <summary>
        /// The start string for each file that is saved
        /// </summary>
        public string fileStartName { get; set; }
            
        public ImageSaver(string savePath)
        {
            images = new List<byte[]>();
            this.savePath = savePath;
            SetDefaults();
        }

        /// <summary>
        /// When overloaded with a sequence integer the first file is saved with this value as the prefix
        /// subsequent files are increased by 1
        /// </summary>
        /// <param name="sequenceStart"></param>
        public ImageSaver(int sequenceStart, string savePath)
        {
            fileNumber =(UInt64)sequenceStart;
            sequenceFileNumbers = true;
            this.savePath = savePath;
            SetDefaults();
        }

        private void SetDefaults()
        {
            fileStartName = "test";
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
            UInt64 x = 0;

            if(sequenceFileNumbers)
            {
                x = fileNumber;
                fileNumber = fileNumber + 1;
            }
            else
            {
                UInt64 hash = (UInt64)(int)DateTime.Now.Kind;
                x = (hash << 62) | (UInt64)DateTime.Now.Ticks;
            }


            return savePath + @"\" + fileStartName + "_" + x.ToString() + ".jpg";

        }

        private void WriteScreen(string st)
        {
            Console.WriteLine(DateTime.Now + " - " + st);
        }
    }
}

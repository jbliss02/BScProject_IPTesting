using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace IPConnect_Testing.Images
{
    public class ImageConvert
    {
        public byte[] JPEG_header = new byte[5] { 255, 216, 255, 254, 0 };

        public Bitmap ReturnBitmap(byte[] img)
        {
            MemoryStream stream = new MemoryStream(img);
            BinaryReader reader = new BinaryReader(stream);
            byte[] buffer = reader.ReadBytes(5); //get's rid of the JPEG header
            return new Bitmap(stream);
        }

        //public byte[] ReturnJpegStream(Bitmap bitmap)
        //{
        //    List<byte> returnBytes = new List<byte>();
        //    returnBytes.AddRange(JPEG_header.ToList());

        //    bitmap
        //}


    }
}

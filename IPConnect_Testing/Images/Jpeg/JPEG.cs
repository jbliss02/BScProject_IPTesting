using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using IPConnect_Testing.Images.Bitmaps;

namespace IPConnect_Testing.Images
{
    public class JPEG
    {
        string filePath;
        byte[] bytes;

        public JPEG(string filePath)
        {
            this.filePath = filePath;
        }

        public byte[] Bytes()
        {
            bytes = File.ReadAllBytes(filePath);
            return bytes;
        }

        public Bitmap ReturnBitmap()
        {
            if(bytes == null) { Bytes(); }

            MemoryStream stream = new MemoryStream(bytes);
            BinaryReader reader = new BinaryReader(stream);
            byte[] buffer = reader.ReadBytes(5); //get's rid of the JPEG header
            return new Bitmap(stream);
        }

        public BitmapWrapper ReturnBitmapWrapper()
        {
            return new BitmapWrapper(ReturnBitmap());
        }

    }
}

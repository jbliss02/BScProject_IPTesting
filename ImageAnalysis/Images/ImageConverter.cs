using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.Images
{
    public static class ImageConvert
    {
        public static byte[] JPEG_header = new byte[5] { 255, 216, 255, 254, 0 };

        public static Bitmap ReturnBitmap(byte[] img)
        {
            MemoryStream stream = new MemoryStream(img);
            BinaryReader reader = new BinaryReader(stream);
            byte[] buffer = reader.ReadBytes(5); //get's rid of the JPEG header
            return new Bitmap(stream);
        }

        /// <summary>
        /// Returns a byte wrappper from a JPEG file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ByteWrapper ReturnByteWrapper(string filepath)
        {
            ByteWrapper result = new ByteWrapper(File.ReadAllBytes(filepath));
            return result;
        }


        //public byte[] ReturnJpegStream(Bitmap bitmap)
        //{
        //    List<byte> returnBytes = new List<byte>();
        //    returnBytes.AddRange(JPEG_header.ToList());

        //    bitmap
        //}


    }
}

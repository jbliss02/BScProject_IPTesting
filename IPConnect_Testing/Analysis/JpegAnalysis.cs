using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using IPConnect_Testing.Images;
using IPConnect_Testing.Images.Bitmaps;

namespace IPConnect_Testing.Analysis
{
    /// <summary>
    /// Compares two jpeg images
    /// </summary>
    public class JpegAnalysis
    {

        /// <summary>
        /// Returns the millseconds to turn a in memory JPEG into a Bitmap
        /// </summary>
        /// <returns></returns>
        public double MsToBitmap(JPEG jpeg)
        {
            jpeg.Bytes(); //gets the bytes into memory
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BitmapWrapper wrapper = new BitmapWrapper(jpeg.ReturnBitmap());
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// Returns the milliseconds to turn a in memory JPEG into a Bitmap,
        /// and total all the pixels within the bitmap
        /// </summary>
        /// <returns></returns>
        public double MsToBitmapAndTotalPixels(JPEG jpeg)
        {
            jpeg.Bytes(); //gets the bytes into memory
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BitmapWrapper wrapper = new BitmapWrapper(jpeg.ReturnBitmap());
            wrapper.pixelAnalysis.TotalPixels();
            sw.Stop();
            return sw.Elapsed.TotalMilliseconds;
        }

    }
}

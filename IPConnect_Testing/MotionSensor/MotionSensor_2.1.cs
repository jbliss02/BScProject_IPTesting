using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using IPConnect_Testing.Images;
using IPConnect_Testing.Analysis;
using IPConnect_Testing.Images.Bitmaps;
using IPConnect_Testing.Images.Jpeg;

namespace IPConnect_Testing.MotionSensor
{
    /// <summary>
    /// 2a is an approach which takes a stream of images and compares the sum of pixel colours between adjacent images
    /// A control sample is taken initially which determines a threshold, beyond which the move in pixel colour totals
    /// is considered abnormal, and therefore classed as movement. 
    /// 
    /// Works on percentage change
    /// 
    /// TO DO - CHANGE THE THRESHOLD EVERY XX MINUTES, AS LONG AS NO MOVEMENT HAS BEEN DETECTED
    /// 
    /// </summary>

    public class MotionSensor_2a
    {
        public int controlImageNumber { get; set; } = 10; //number of changes to use as the control (half the images as done in pairs)
        public Queue<ByteWrapper> images { get; set; } //images waiting to be processed
        public String logfile { get; set; }

        private int imagesReceived; //used to flush the queue of images
        private List<double> pixelChange; //holds a list of the difference between pixels of 2 images (used for setting threshold)
        private double pixelChangeThreshold;
        private int imagesChecked = 0;
        private int numberMotionFiles = 0;

        private double sensitivity = 1;

        public MotionSensor_2a()
        {
            images = new Queue<ByteWrapper>();
            imagesReceived = 0;
            pixelChange = new List<double>();
            pixelChangeThreshold = -1;
        }
   
        public async void ImageCreated(ByteWrapper img, EventArgs e)
        {
            await Task.Run(() =>
            {
                imagesReceived++;
                images.Enqueue(img);

                Compare();
            });
        }

        private async void Compare()
        {
            if (images.Count > 1)
            {
                imagesChecked = imagesChecked + 2;

                ByteWrapper img1 = images.Dequeue();
                ByteWrapper img2 = null;
                if (images.Count > 0)
                {
                    img2 = images.Dequeue();
                }

                if(img1 != null && img2 != null)
                {
                    var bm1 = new BitmapWrapper(new ImageConvert().ReturnBitmap(img1.bytes));
                    var bm2 = new BitmapWrapper(new ImageConvert().ReturnBitmap(img2.bytes));

                    PixelMatrix matrix = new PixelMatrix(bm1, bm2);
                    double sumChangedPixels = matrix.SumChangedPixels;

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(logfile, true))
                    {
                        file.WriteLine(img1.sequenceNumber + " - " + img2.sequenceNumber + " - " + sumChangedPixels);
                    }

                    if (pixelChange != null && pixelChange.Count < controlImageNumber)
                    {
                        pixelChange.Add(sumChangedPixels);
                    }
                    else if (pixelChangeThreshold == -1)
                    {
                        //enough images received, but thresholds not set

                        double range = ((pixelChange.Max() - pixelChange.Min()) / pixelChange.Min()) * 100;
                        double buffer = range * 2 * sensitivity;
                        pixelChangeThreshold = pixelChange.Max() + buffer;

                       // pixelChangeThreshold = pixelChange.Max() * 1.05; //start with 0.2
                        Console.WriteLine("Threshold found");
                    }
                    else
                    {
                        //now scanning, compare the two images and see what the difference is
                        if (matrix.SumChangedPixels > pixelChangeThreshold)
                        {
                            Console.WriteLine("Movement");
                            await SaveImageAsync(bm2, img2.sequenceNumber);
                            
                            numberMotionFiles++;
                        }

                    }//if there are images
                }
            }//if images > 1

        }//Compare

        public async Task SaveImageAsync(BitmapWrapper bitmapWrapper, int sequenceNumber)
        {
            await Task.Run(() => 
            {
                bitmapWrapper.bitmap.Save(@"f:\temp\MotionSensor\2.1\movement\" + sequenceNumber + ".bmp");
            });
        }

        /// <summary>
        /// move to 
        /// </summary>
        /// <returns></returns>
        private double PercentDifference(double num1, double num2)
        {
            double d = num2 - num1;

            if(d < 0) { d = -d; }

            return d / num1;

        }

        public void Write(string st)
        {
            Console.WriteLine(DateTime.Now + " - " + st);
        }


    }
}

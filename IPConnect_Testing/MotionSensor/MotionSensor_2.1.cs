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
        public int controlImageNumber = 50; //number of changes to use as the control (half the images as done in pairs)

        public Queue<byte[]> images; //images waiting to be processed
        private int imagesReceived; //used to flush the queue of images

        private List<double> pixelChange; //holds a list of the difference between pixels of 2 images (used for setting threshold)
        private double pixelChangeThreshold;

        private int imagesChecked = 0;

        private int numberMotionFiles = 0;

        public MotionSensor_2a()
        {
            images = new Queue<byte[]>();
            imagesReceived = 0;
            pixelChange = new List<double>();
            pixelChangeThreshold = -1;
        }

        public async void ImageCreated(byte[] img, EventArgs e)
        {

            await Task.Run(() =>
            {
                imagesReceived++;

                if (imagesReceived % 100 == 0) { Console.WriteLine("REC - " + DateTime.Now + " - " + imagesReceived); }

                images.Enqueue(img);

                Task.Run(() => this.Compare());
            });




        }
        private async Task Compare()
        {
            await Task.Run(() =>
            {

                if (images.Count > 1)
                {
                    imagesChecked = imagesChecked + 2;
              
                    //if(imagesChecked % 10 == 0) {
                    //    Task.Delay(4000);
                    //}
                    Console.WriteLine("CHK - " + imagesChecked);

                    byte[] img1 = null;
                    byte[] img2 = null;
                    if(images.Count > 0)
                    {
                        img1 = images.Dequeue();
                    }
                    if (images.Count > 0)
                    {
                        img2 = images.Dequeue();
                    }

                    if(img1 != null && img2 != null)
                    {
                        var bm1 = new BitmapWrapper(new ImageConvert().ReturnBitmap(img1));
                        var bm2 = new BitmapWrapper(new ImageConvert().ReturnBitmap(img2));

                       // Console.WriteLine(imagesReceived);

                        if (pixelChange != null && pixelChange.Count < controlImageNumber)
                        {
                            PixelMatrix matrix = new PixelMatrix(bm1, bm2);
                            pixelChange.Add(matrix.SumChangedPixels);

                        }
                        else if (pixelChangeThreshold == -1)
                        {
                            //enough images received, but thresholds not set
                            pixelChangeThreshold = pixelChange.Max() * 1.1; //start with 0.2
                            //pixelChange = null; //not needed anymore
                            Console.WriteLine("Threshold found");
                        }
                        else
                        {
                           // Console.WriteLine("Start scan");
                            //now scanning, compare the two images and see what the difference is
                            PixelMatrix matrix = new PixelMatrix(bm1, bm2);
                            if(matrix.SumChangedPixels > pixelChangeThreshold)
                            {
                                Console.WriteLine("Movement");
                                bm1.bitmap.Save(@"f:\temp\MotionSensor\2.1\movement\" + numberMotionFiles +"_movement.bmp");
                                numberMotionFiles++;
                            }
                           // Console.WriteLine("End scan");
                        }
                    }
                }
            });

        }//Compare

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


    }
}

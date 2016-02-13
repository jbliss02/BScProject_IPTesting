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


    public class MotionSensor_2a : MotionSensor_2
    { 
        public async override void Compare()
        {
            if (images.Count > 1)
            {            
                //take images out of the queue, as this is async other methods may dequeue between the calls so be defensive
                ByteWrapper img1 = null;
                if (images.Count > 0) { img1 = images.Dequeue(); }
                ByteWrapper img2 = null;
                if (images.Count > 0) { img2 = images.Dequeue();}

                if(img1 != null && img2 != null)
                {
                    imagesChecked = imagesChecked + 2;
                    var bm1 = new BitmapWrapper(new ImageConvert().ReturnBitmap(img1.bytes));
                    var bm2 = new BitmapWrapper(new ImageConvert().ReturnBitmap(img2.bytes));

                    PixelMatrix matrix = new PixelMatrix(bm1, bm2);
                    double sumChangedPixels = matrix.SumChangedPixels;

                    //keep adding for threshold calculation, set the threshold, or monitor
                    if(ThresholdSet)
                    {
                        //now scanning, compare the two images and see what the difference is
                        if (matrix.SumChangedPixels > pixelChangeThreshold)
                        {
                            OnMotion(img1, img2);
                            Console.WriteLine("Movement");
                            await SaveImageAsync(bm2, img2.sequenceNumber);
                            numberMotionFiles++;
                        }
                    }
                    else if (!ThresholdSet && pixelChange.Count < ControlImageNumber)
                    {
                        pixelChange.Add(sumChangedPixels);
                    }
                    else  
                    {
                        SetThreshold(); //enough images received to set the threshold and start monitoring
                    }


                }//if images are not null
            }//if images > 1

        }//Compare

    }
}

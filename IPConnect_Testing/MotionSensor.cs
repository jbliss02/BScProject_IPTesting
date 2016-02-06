using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using IPConnect_Testing.Images.Bitmaps;

namespace IPConnect_Testing
{
    /// <summary>
    /// Takes a list of Bitmaps and sees whether there is any motion
    /// </summary>
    public class MotionSensor
    {
        private BitmapWrapperList bitmaps;   

        private double highestTolerance = 2.6;
        private double lowestTolerance = 0.3;
        private bool setTolerance;
        private bool toleranceSet;
        private double toleranceMultiplier = 1.62;

        private List<double> percentDifferences; //used when setting the tolerance
        private int iterationsForTolerance = 20;

        private double highest;
        private double lowest;
        private double percentDifference;

        /// <summary>
        /// Choosing to set a tolerance will make the motion sensor detect xxx frame
        /// to identify a tolerance. No movement must happen during the tolerance period
        /// </summary>
        /// <param name="setTolerance"></param>
        /// 
        public MotionSensor(bool setTolerance)
        {
            this.setTolerance = setTolerance;
            if (setTolerance) { percentDifferences = new List<double>(); }
        }

        public MotionSensor(double tolerancePercent)
        {
            this.highestTolerance = tolerancePercent;
            this.toleranceSet = true;

        }
            

        public void CheckForMotion(List<Bitmap> bitmaps)
        {
            this.bitmaps = new BitmapWrapperList(bitmaps);

            if (!toleranceSet)
            {
                SetTolerance();
            }
            else if(MotionDetected())
            {
                new System.Media.SoundPlayer(@"f:\temp\emergency.wav").Play();
                Console.WriteLine(DateTime.Now + " - Motion detected on difference of " + percentDifference);
                SaveAll();
               
            }
           
        }//MotionSensor

        private void SetTolerance()
        {
            SetRanges();
            percentDifferences.Add(percentDifference);
            if(percentDifferences.Count == iterationsForTolerance)
            {
                percentDifferences.Sort();
                lowestTolerance = percentDifferences[0] * (1 / toleranceMultiplier);
                highestTolerance = percentDifferences[percentDifferences.Count - 1] * toleranceMultiplier;
                Console.WriteLine(DateTime.Now + " - High Tolerance is " + highestTolerance);
                Console.WriteLine(DateTime.Now + " - Low Tolerance is " + lowestTolerance);

                toleranceSet = true;
            }
        }

        private bool MotionDetected()
        {
            SetRanges();
            return (percentDifference < lowestTolerance || percentDifference > highestTolerance);
        }

        private void SetRanges()
        {
            highest = this.bitmaps.highestPixelTotal;
            lowest = this.bitmaps.lowestPixelTotal;
            double difference = highest - lowest;
            percentDifference = (difference / lowest) * 100;
        }

        private void SaveAll()
        {
            for(int i = 0; i < bitmaps.list.Count; i++)
            {
                bitmaps.list[i].bitmap.Save(@"f:\temp\alarm\"+ bitmaps.list[i].bitmap.GetHashCode().ToString() + ".bmp");
            }
        }

  
    }

}


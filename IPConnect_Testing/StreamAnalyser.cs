using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace IPConnect_Testing
{
    /// <summary>
    /// Determines and logs meta-data about a stream of images
    /// </summary>
    public class StreamAnalyser
    {
        public string logFile { get; set; }

        private Stopwatch mainWatch;
        private Stopwatch stopwatch;
        private List<double> framesPerSecond;
        
        public int nImages; //the number of images analysed
        public int nSections; //the number of sections recorded
        public int imagesPerSection = 100;

        public bool isTrackingFrameRate;

        public StreamAnalyser(string logFile, bool frameRate)
        {
            this.logFile = logFile;
            mainWatch = new Stopwatch();
            stopwatch = new Stopwatch();
            framesPerSecond = new List<double>();
        }

        public async Task ImageCreatedAsync(byte[] img, EventArgs e)
        {
            await Task.Run(() => { LogCreationData(); } );
        }

        private async void LogCreationData()
        {

            if (!isTrackingFrameRate)
            {
                stopwatch.Start();
                mainWatch.Start();
                nImages = 1;
                isTrackingFrameRate = true;
            }
            else
            {
                ++nImages;
            }

            if (nImages == imagesPerSection)
            {
                stopwatch.Stop();
                ++nSections;

                //timeSpans.Add(stopwatch.Elapsed);
                await CalculateFrameRate(stopwatch.Elapsed);

                stopwatch.Reset();
                stopwatch.Start();
                nImages = 0;
            }

        }//LogCreationData

        private async Task CalculateFrameRate(TimeSpan timespan)
        {
            await Task.Run(() =>
            {
                double frameRate = timespan.TotalMilliseconds / imagesPerSection;
                framesPerSecond.Add(frameRate);
                Console.WriteLine(frameRate);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(logFile, true))
                {
                    file.WriteLine(frameRate);
                }

            });

        } //CalculateFrameRate

    }

}

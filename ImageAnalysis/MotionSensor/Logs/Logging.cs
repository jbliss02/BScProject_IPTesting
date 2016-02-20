using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Analysis;

namespace ImageAnalysis.MotionSensor.Logs
{
    public class Logging
    {
        public bool LoggingOn { get; set; }

        public int imagesReceived;
        public int imagesChecked;
        public int numberMotionFiles;

        public double threshold;
        public List<PixelMatrix> matrices;

        public Logging() { matrices = new List<PixelMatrix>(); }

        public void WriteToLog(string logfile)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(logfile, true))
            {
                file.WriteLine(threshold);
                for (int i = 0; i < matrices.Count; i++)
                {
                    string line = matrices[i].image1.sequenceNumber + " ~ " + matrices[i].image2.sequenceNumber + " ~ " + matrices[i].MinChanged +
                        " ~ " + matrices[i].MaxChanged + "~" + matrices[i].SumChangedPixelsPositive;
                    file.WriteLine(line);
                }
            }
        }//WriteToLog
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysis 
{
    /// <summary>
    /// used to hold testing data about a single Capture object
    /// </summary>
    public class CaptureTesting
    {
        public CaptureTesting() { }
        public CaptureTesting(string captureId) { this.captureId = captureId; }
        public string captureId { get; set; } //held here as this class is serialised seperatley

        /// <summary>
        /// What frame numbers have been identified as having movement
        /// </summary>
        public List<Int32> detectedMovmentFrames { get; set; }
    }
}

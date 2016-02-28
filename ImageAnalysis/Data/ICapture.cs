using System;
using System.Collections.Generic;

namespace ImageAnalysis.Data
{
    public interface ICapture
    {
        DateTime? capturedOn { get; set; }
        string captureId { get; set; }
        List<Movement> movement { get; set; }
    }
}
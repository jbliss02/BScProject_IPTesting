using System;
using System.Collections.Generic;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.MotionSensor
{
    public interface IMotionSensor
    {
        int ControlImageNumber { get; set; }
        MotionSensorSettings settings { get; set; }
        bool ThresholdSet { get; set; }
        Queue<ByteWrapper> WorkQueue { get; set; }

        event MotionSensor_2.MotionDetected motionDetected;

        void Compare(ByteWrapper img1, ByteWrapper img2);
        void ImageCreated(ByteWrapper img, EventArgs e);
        void ImageCreatedAsync(ByteWrapper img, EventArgs e);
    }
}
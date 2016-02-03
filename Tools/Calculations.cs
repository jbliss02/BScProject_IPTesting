using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class Calculations
    {

        public static double Framerate(int nFrames, double milliseconds)
        {
            double msPerFrame = milliseconds / nFrames;
            return 1000 / msPerFrame;
        }

        public static double Framerate(int nFrames, int milliseconds)
        {
            double msPerFrame = milliseconds / nFrames;
            return 1000 / msPerFrame;
        }

    }
}

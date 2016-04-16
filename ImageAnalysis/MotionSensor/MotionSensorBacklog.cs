using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Text.RegularExpressions;
using Tools;

namespace ImageAnalysis.MotionSensor
{
    internal class MotionSensorBacklog
    {
        internal int lastImageReceived;
        internal List<int> backlog; //images received v images processed
        internal object backlogLock = new object(); //locks the backlog list so two processes cannot change at the same time
        internal Stopwatch backlogTimer; //times when next to check the backlog
        internal int backlogCheckMs; //check the backlog every so many milliseconds
        internal int pixelJumpPerFrameJump; //set from the regulation formula. pixels jumped up to this value, then a frame is jumped
        internal int backlogSpeedup; //when this number is exceeded everything is sped up, to decrease the backlog
        internal int backlogSlowdown; //when this number is higher than backlog everything is slowed down, increasing accuracy

        internal MotionSensorBacklog()
        {
            backlogCheckMs = ConfigurationManager.AppSettings["backlogCheckMs"].ToString().StringToInt();
            backlogSpeedup = ConfigurationManager.AppSettings["backlogSpeedup"].ToString().StringToInt();
            backlogSlowdown = ConfigurationManager.AppSettings["backlogSlowdown"].ToString().StringToInt();
            backlogTimer = new Stopwatch();
            backlog = new List<int>();
            SetRegulationParameters();
        }

        /// <summary>
        /// Extracts the regulation formula from config files and sets the pixelJumpPerFrameJump variable
        /// this drives the logic on what metric to increase / decrease when changing speed
        /// </summary>
        private void SetRegulationParameters()
        {
            string[] split = Regex.Split(ConfigurationManager.AppSettings["regulationFormula"], ":");

            if (split.Length != 2)
            {
                throw new Exception("regulationFormula was not in expected format");
            }

            int framesToSkip;
            int pixelsToSkip;

            if (split[0].Substring(split[0].Length - 2).ToUpper() == "P")
            {
                pixelsToSkip = split[0].Substring(0, split[0].Length - 2).StringToInt();
                framesToSkip = split[1].Substring(0, split[1].Length - 2).StringToInt();
            }
            else
            {
                framesToSkip = split[0].Substring(0, split[0].Length - 1).StringToInt();
                pixelsToSkip = split[1].Substring(0, split[1].Length - 1).StringToInt();
            }

            if (framesToSkip <= 0 || pixelsToSkip <= 0)
            {
                throw new Exception("regulationFormula was not in expected format");
            }

            pixelJumpPerFrameJump = framesToSkip / pixelsToSkip;

        }//SetRegulationParameters



    }
}

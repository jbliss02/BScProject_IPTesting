using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using IPConnect_Testing;
using IPConnect_Testing.Images;
using IPConnect_Testing.Images.Bitmaps;
using IPConnect_Testing.Images.Jpeg;
using IPConnect_Testing.MotionSensor;
using IPConnect_Testing.Analysis;
using Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.Tools
{
    [TestClass]
    public class ExtensionMethods
    {
        /// <summary>
        /// checks a hex to int comparision
        /// </summary>
        [TestMethod]
        [TestCategory("Tools")]
        public void HexToInt()
        {
            BitmapWrapper bm1 = new BitmapWrapper(@"F:\temp\analysis\640x480\test_0.jpg");

            for(int i = 0; i < bm1.bitmap.Width; i++)
            {
                for(int n = 0; n < bm1.bitmap.Height; n++)
                {
                    Int64 num1 = Int64.Parse(bm1.bitmap.GetPixel(i, n).Name, System.Globalization.NumberStyles.HexNumber);
                    Int64 num2 = (bm1.bitmap.GetPixel(i, n).Name.HexToLong());
                    Assert.IsTrue(num1.Equals(num2));
                }
                        
            }
                    
        }
    }
}

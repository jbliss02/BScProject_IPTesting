using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Tools;

namespace ImageAnalysis.Data
{
    public class Movement
    {
        public Movement(DataRow dr)
        {
            frameStart = dr["movementFrameStart"].ToString().StringToInt();
            frameEnd = dr["movementFrameEnd"].ToString().StringToInt();
        }
        
        public Movement() { }

        public int frameStart { get; set; }
        public int frameEnd { get; set; }
    }
}

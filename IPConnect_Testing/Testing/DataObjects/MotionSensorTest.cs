using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPConnect_Testing.Testing.DataObjects
{
    /// <summary>
    /// Base class for MotionSensorTesting, implemented by
    /// classes that test specifici elements of the solution
    /// </summary>
    public abstract class MotionSensorTest
    {
        public CaptureListTesting captures { get; set; }

        /// <summary>
        /// Populates the captures List with all captures in the database
        /// </summary>
        protected void PopulateAllCaptures()
        {
            captures = new CaptureListTesting();
            captures.PopulateAllCaptures(true);
        }

        protected void PopulateCapture(string captureId)
        {
            captures = new CaptureListTesting();
            captures.PopulateCapture(true, captureId);
        }

    }
}

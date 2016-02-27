using System.Collections.Generic;

namespace IPConnect_Testing.Testing
{
    public interface IMotionSensorTest
    {
        List<int> movementFrames { get; }

        void Run(string captureId);
    }
}
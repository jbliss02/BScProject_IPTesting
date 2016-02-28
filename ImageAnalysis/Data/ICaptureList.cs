using System.Collections.Generic;
using System.Xml;

namespace ImageAnalysis.Data
{
    public interface ICaptureList
    {
        List<Capture> list { get; set; }

        XmlDocument SerialiseMe();
        void PopulateAllCaptures(bool allData);
        void PopulateMovement();
    }
}
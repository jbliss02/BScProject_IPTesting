using System.Data;
using System.Xml;

namespace ImageAnalysisDAL
{
    public interface ICaptureDb
    {
        DataTable ReturnAllCaptures();
        DataTable ReturnCapture(string captureId);
        DataTable ReturnCaptureMovement(XmlDocument captureXml);
        DataTable ReturnMotionSettingDefaults();
    }
}
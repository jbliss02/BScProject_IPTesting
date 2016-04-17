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
        void CreateCaptureSession(string captureId, string saveLocation);
        bool CaptureIdExists(string captureId);
        int CreateDetectionSession(string captureId);
    }
}
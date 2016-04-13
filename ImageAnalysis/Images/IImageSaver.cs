using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.Images
{
    public interface IImageSaver
    {
        string CaptureDirectory { get; set; }
        string captureId { get; set; }
        string fileStartName { get; set; }
        List<double> framerates { get; set; }
        int framesPerSection { get; set; }
        int initialFrameDetection { get; set; }
        string ParentDirectory { get; set; }
        string SaveDirectory { get; set; }

        event ImageSaver.ImageSavedEvent imageCreated;

        Task ImageCreatedAsync(ByteWrapper img);
        void ImageCreatedAsync(ByteWrapper img, EventArgs e);
        Task SaveFiles(List<ByteWrapper> imgs);
        void WriteBytesToFileAsync(ByteWrapper image, EventArgs e);
    }
}
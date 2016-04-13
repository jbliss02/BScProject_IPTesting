using System;
using ImageAnalysis.Images.Jpeg;
using ImageAnalysis.Streams;

namespace ImageAnalysis.Images
{
    public interface IImageValidator
    {
        event ImageValidator.ImageValidatedEvent imageValidated;

        void FileCreated(ByteWrapper img, EventArgs e);
        void ListenForImages(IImageExtractor imgClass);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageAnalysis.Streams;
using ImageAnalysis.Images.Jpeg;

namespace ImageAnalysis.Images
{
    /// <summary>
    /// Validates that a byte array passed in contains an image
    /// Initially only covers JPEG's - does this by checking the magic numbers
    /// JPegs start with ff d8 and end with ff d9 (or bytes - 255 216 ends with 255 217)
    /// </summary>
    public class ImageValidator
    {
        public event ImageValidatedEvent imageValidated;
        public delegate void ImageValidatedEvent(ByteWrapper img, EventArgs e);

        public byte[] JPEG_start = new byte[2] {255, 216};
        public byte[] JPEG_end = new byte[2] { 255, 217 };

        public void ListenForImages(ImageExtractor imgClass)
        {
            imgClass.imageCreated += new ImageExtractor.ImageCreatedEvent(FileCreated);
        }

        public void FileCreated(ByteWrapper img, EventArgs e)
        {
            if(img.bytes[0] == JPEG_start[0] && img.bytes[1] == JPEG_start[1] && img.bytes[img.bytes.Length -2] == JPEG_end[0] && img.bytes[img.bytes.Length -1] == JPEG_end[1])
            {
                OnImageValidated(img); //raise the event
            }
            else
            {
                //throw new Exception("JPEG image is not well formed");
                Console.WriteLine("Badly formed image");
            }

        }//FileCreated

        protected virtual void OnImageValidated(ByteWrapper img)
        {
            if(imageValidated != null)
            {
                imageValidated(img, EventArgs.Empty);
            }

        }//OnImageValidated


    }
}

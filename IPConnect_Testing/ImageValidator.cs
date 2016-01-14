using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPConnect_Testing
{
    /// <summary>
    /// Validates that a byte array passed in contains an image
    /// Initially only covers JPEG's - does this by checking the magic numbers
    /// JPegs start with ff d8 and end with ff d9 (or bytes - 255 216 ends with 255 217)
    /// </summary>
    public class ImageValidator
    {
        public byte[] JPEG_start = new byte[2] {255, 216};
        public byte[] JPEG_end = new byte[2] { 255, 217 };

        public void ListenForImages(ImageExtractor imgClass)
        {
            imgClass.imageCreated += new ImageExtractor.ImageCreateEvent(FileCreated);
        }

        public void FileCreated(byte[] img, EventArgs e)
        {
            if(img[0] == JPEG_start[0] && img[1] == JPEG_start[1] && img[img.Length -2] == JPEG_end[0] && img[img.Length -1] == JPEG_end[1])
            {
                Console.WriteLine("All good");
            }
            else
            {
                throw new Exception("JPEG image is not well formed");
            }

        }//FileCreated

    }
}

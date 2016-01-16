using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace IPConnect_Testing.Images
{
    /// <summary>
    /// A collection of BitmapWrapper with assorted methods to manage the components
    /// </summary>
    class BitmapWrappers
    {
        public BitmapWrappers(List<SingleBitmapWrapper> list) { this.list = list; }
        public BitmapWrappers(List<Bitmap> bitmaps)
        {
            list = new List<SingleBitmapWrapper>();
            for (int i = 0; i < bitmaps.Count; i++)
            {
                SingleBitmapWrapper b = new SingleBitmapWrapper(bitmaps[i]);
                b.pixelAnalysis.TotalPixels();
                list.Add(b);
            }

            SortByPixelTotal();
        }
        public List<SingleBitmapWrapper> list { get; set; }

        private bool pixelListSorted;

        public void SortByPixelTotal()
        {
            list = list.OrderByDescending(a => a.pixelAnalysis.totalPixelColors).ToList();
            pixelListSorted = true;
        }

        public double highestPixelTotal { get {
            if (!pixelListSorted) { SortByPixelTotal(); }
            return list[0].pixelAnalysis.totalPixelColors;
        }   }

         public double lowestPixelTotal{ get {
                if (!pixelListSorted) { SortByPixelTotal(); }
                return list[list.Count - 1].pixelAnalysis.totalPixelColors;
         }   }

    }

    class SingleBitmapWrapper
    {
        public UInt64 hash { get; set; }

        public Bitmap bitmap { get; set; }

        public SingleBitmapWrapper(Bitmap bitmap) { this.bitmap = bitmap; pixelAnalysis = new PixelAnalysis(bitmap); }

        public PixelAnalysis pixelAnalysis { get; set; }





    }


}

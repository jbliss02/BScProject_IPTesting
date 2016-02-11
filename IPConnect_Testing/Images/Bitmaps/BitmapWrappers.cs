    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace IPConnect_Testing.Images.Bitmaps
{
    /// <summary>
    /// A collection of BitmapWrapper with assorted methods to manage the components
    /// </summary>
    public class BitmapWrapperList
    {
        public BitmapWrapperList(List<BitmapWrapper> list) { this.list = list; }
        public BitmapWrapperList(List<Bitmap> bitmaps)
        {
            list = new List<BitmapWrapper>();
            for (int i = 0; i < bitmaps.Count; i++)
            {
                BitmapWrapper b = new BitmapWrapper(bitmaps[i]);
                b.pixelAnalysis.SumRGB();
                list.Add(b);
            }

            SortByPixelTotal();
        }
        public List<BitmapWrapper> list { get; set; }

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

    public class BitmapWrapper
    {
        public UInt64 hash { get; set; }

        public Bitmap bitmap { get; set; }

        public BitmapWrapper(string path) { bitmap = new Bitmap(path); pixelAnalysis = new PixelAnalysis(bitmap); }

        public BitmapWrapper(Bitmap bitmap) { this.bitmap = bitmap; pixelAnalysis = new PixelAnalysis(bitmap); }

        public PixelAnalysis pixelAnalysis { get; set; }

    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPConnect_Testing.Analysis
{
    /// <summary>
    /// Represents each pixel within an image, each pixel has a true or false
    /// </summary>
    public class PixelMatrix
    {
        public List<PixelColumn> columns { get; set; }

        public int numberChangedPixels { get
            {
                return (from c in columns select c.numberChangedPixels).Sum();
                             
            } }

        public void DumpToText(string textfile)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(textfile, true))
            {
                for(int i = 0; i < columns.Count; i++)
                {
                    for(int n = 0; n < columns[i].cells.Count; n++)
                    {
                        file.WriteLine(columns[i].cells[n].change);
                    }
                            
                }
            }
        }


    }

    public class PixelColumn
    {
        public List<PixelCell> cells { get; set; }

        public int numberChangedPixels { get
            {
                return (from p in cells where p.hasChanged select p).Count();
            } }

        public double maxChange { get
            {
                return (from p in cells orderby p.change descending select p.change).First();
            } }

    }

    public class PixelCell
    {
        public bool hasChanged { get; set; }

        public double change { get; set; }
    }

}

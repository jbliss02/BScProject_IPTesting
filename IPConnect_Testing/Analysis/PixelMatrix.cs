using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using IPConnect_Testing.Images;
using IPConnect_Testing.Images.Bitmaps;

namespace IPConnect_Testing.Analysis
{
    /// <summary>
    /// Represents each pixel within an image
    /// each pixel contains meta-data related to the equivalent pixel, 
    /// or comparision of that pixel across many images
    /// </summary>
    public class PixelMatrix
    {
        public List<PixelColumn> columns { get; set; }

        public List<PixelColumn> reducedColumns { get; set; } //the matrix, where the change values are reduced to a 0 - 255 range

        public int numberChangedPixels { get{return (from c in columns select c.numberChangedPixels).Sum(); } }

        public double maxChanged { get { return (from c in columns select c.maxChange).Max(); } }

        public double minChanged { get { return (from c in columns select c.minChange).Min(); } }

        public void Populate(string image1Path, string image2Path)
        {
            if (new FileInfo(image1Path).Extension == ".jpg" && new FileInfo(image2Path).Extension == ".jpg")
            {
                Populate(new JPEG(image1Path).ReturnBitmapWrapper(), new JPEG(image2Path).ReturnBitmapWrapper());
            }
            else if (new FileInfo(image1Path).Extension == ".bmp" && new FileInfo(image2Path).Extension == ".bmp")
            {
                Populate(new BitmapWrapper(image1Path), new BitmapWrapper(image2Path));
            }
            else
            {
                throw new Exception("File format not supported, or not equal");
            }

        }//Populate

        public void Populate(BitmapWrapper image1, BitmapWrapper image2)
        {
            columns = new List<PixelColumn>();

            for (int i = 0; i < image1.bitmap.Width; i++)
            {
                PixelColumn column = new PixelColumn();
                column.cells = new List<PixelCell>();

                for (int n = 0; n < image1.bitmap.Height; n++)
                {
                    PixelCell cell = new PixelCell();
                    if (image1.bitmap.GetPixel(i, n).Name != image2.bitmap.GetPixel(i, n).Name)
                    {
                        cell.hasChanged = true;
                        cell.change = Int64.Parse(image1.bitmap.GetPixel(i, n).Name, System.Globalization.NumberStyles.HexNumber) - Int64.Parse(image2.bitmap.GetPixel(i, n).Name, System.Globalization.NumberStyles.HexNumber);

                    }
                    else
                    {
                        cell.hasChanged = false;
                    }
                    column.cells.Add(cell);

                }//height

                columns.Add(column);

            }//width

        }//Populate

        /// <summary>
        /// Dumps the pixel data to a text file
        /// Dumps into column order (i.e. last entry is last column, last row)
        /// </summary>
        /// <param name="textfile"></param>
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
        }//DumpToText

        /// <summary>
        /// Dumps the reduced pixel data to a text file
        /// Dumps into column order (i.e. last entry is last column, last row)
        /// </summary>
        /// <param name="textfile"></param>
        public void DumpReducedToText(string textfile)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(textfile, true))
            {
                for (int i = 0; i < reducedColumns.Count; i++)
                {
                    for (int n = 0; n < reducedColumns[i].cells.Count; n++)
                    {
                        file.WriteLine(reducedColumns[i].cells[n].simpleColour);
                    }

                }
            }
        }//DumpToText

        /// <summary>
        /// Creates a new bitmap, each pixel represents the change number 
        /// from the appropriate cell. Divides into 255 seperate blocks
        /// </summary>
        /// <returns></returns>
        public Bitmap DrawPixelChanges()
        {
            if (columns == null) { throw new Exception("Matrix has no columns"); }
            if (reducedColumns == null) { SetReducedColumns();}

            Bitmap bm = new Bitmap(reducedColumns.Count, reducedColumns[0].cells.Count);

            for (int i = 0; i < bm.Width; i++)
            {
                for (int n = 0; n < bm.Height; n++)
                {
                    int color = Convert.ToInt16(reducedColumns[i].cells[n].simpleColour);
                    bm.SetPixel(i, n, Color.FromArgb(color, color, color, color));

                }//height

            }//width

            return bm;

        }//DrawPixelChanges

        /// <summary>
        /// Sets the reducedColumns structure, reduced the change value in each pixel
        /// to a range in between 0 and 255
        /// </summary>
        public void SetReducedColumns()
        {
            if (columns == null) { throw new Exception("Matrix has no columns"); }
            reducedColumns = new List<PixelColumn>();
          
            double min = minChanged;
            double max = maxChanged;
            double highest = -minChanged > maxChanged ? -minChanged : maxChanged; //everything goes to positive as interested in distance from 0

            double divisor = 255;
            double changesPerColor = highest / divisor;

            for (int i = 0; i < columns.Count; i++)
            {
                PixelColumn col = new PixelColumn();
                col.cells = new List<PixelCell>();

                for (int n = 0; n < columns[i].cells.Count ; n++)
                {
                    PixelCell cell = new PixelCell();
                    double change = columns[i].cells[n].change < 0 ? -columns[i].cells[n].change : columns[i].cells[n].change;
                    cell.simpleColour = 255 - Convert.ToInt16(change / changesPerColor);
                    col.cells.Add(cell);
                }//height

                reducedColumns.Add(col);

            }//width

        }//SetReducedColumns

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

        public double minChange
        {
            get
            {
                return (from p in cells orderby p.change ascending select p.change).First();
            }
        }

    }

    public class PixelCell
    {
        public bool hasChanged { get; set; }

        public double change { get; set; }

        public int simpleColour { get; set; }
    }

}

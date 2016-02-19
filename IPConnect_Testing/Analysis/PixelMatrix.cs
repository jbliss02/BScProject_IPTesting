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
    /// or comparision of that pixel across multiple images
    /// </summary>
    public class PixelMatrix
    {
        public PixelMatrix() { }
        public PixelMatrix(BitmapWrapper image1, BitmapWrapper image2) { Populate(image1, image2); }
        public PixelMatrix(string path1, string path2) { Populate(path1, path2); }
        //images being compared
        private BitmapWrapper image1 { get; set; }
        private BitmapWrapper image2 { get; set; }
        public List<PixelColumn> Columns { get; set; }
        public List<PixelColumn> ReducedColumns { get; set; } //the matrix, where the change values are reduced to a 0 - 255 range
        public ImageGrid imageGrid { get; set; }
        //public List<GridColumn> GridColumns { get; set; } //the matrix represented in a grid system
        public bool GridSystemOn { get; set; }
        /// <summary>
        /// The number of pixels to search horizontally. Defaults to the image width if not set
        /// </summary>
        public int SearchWidth { get; set; }
        /// <summary>
        /// The number of pixels to search vertically. Defaults to the image height if not set
        /// </summary>
        public int SearchHeight { get; set; }
        /// <summary>
        /// The number of pixels the algorithm will move to the right when scanning images
        /// </summary>
        public int WidthSearchOffset { get; set; }
        /// <summary>
        /// The number of pixels the algorithm will move downwards when scanning images
        /// </summary>
        public int HeightSearchOffset { get; set; }
        /// <summary>
        /// Each pixel has a value which contains the numeric different between the two images, this
        /// method returns the sum of those differences
        /// </summary>
        public double SumChangedPixels { get
            {
                double sum = 0;
                for (int n = 0; n < Columns.Count; n++)
                {
                    for (int k = 0; k < Columns[n].cells.Count; k++)
                    {
                        sum += Columns[n].cells[k].change;
                    }
                }

                return sum;
            } }
        public double SumChangedPixelsLINQ { get { return (from col in Columns from cell in col.cells select cell.positiveChange).Sum(); } }
        public double MaxChangedLINQ { get { return (from c in Columns select c.maxChange).Max(); } }
        public double MaxChanged { get
            {
                double max = Columns[0].cells[0].change;
                for (int n = 0; n < Columns.Count; n++)
                {
                    for (int k = 0; k < Columns[n].cells.Count; k++)
                    {
                        if (Columns[n].cells[k].change > max)
                        {
                            max = Columns[n].cells[k].change;
                        }
                    }
                }

                return max;
            } }
        public double MinChangedLINQ { get { return (from c in Columns select c.minChange).Min(); } }
        public double MinChanged { get
            {
                double min = Columns[0].cells[0].change; ;
                for (int n = 0; n < Columns.Count; n++)
                {
                    for (int k = 0; k < Columns[n].cells.Count; k++)
                    {
                        if (Columns[n].cells[k].change < min)
                        {
                            min = Columns[n].cells[k].change;
                        }
                    }
                }
                return min;

            }
        }
        /// <summary>
        /// The image being compared to, this stores the colour of the pixel so it can be used in
        /// a later PixelMatrix object, this saves calling the GetPixel() method twixe
        /// </summary>
        public List<PixelColumn> Comparator { get; set; } 
        /// <summary>
        /// This may be sent in from a constructor, in the place of image 1, the simple colour has already been
        /// turned into an integer
        /// </summary>
        public List<PixelColumn> Comparision { get; set; }

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

        /// <summary>
        /// Compares two images, pixel by pixel, with the analysis about the difference between pixels
        /// pixel by pixel analysis is set into pixelColumns. GridColumns are also populated if GridSystemOn
        /// </summary>
        /// <param name="image1"></param>
        /// <param name="image2"></param>
        public void Populate(BitmapWrapper image1, BitmapWrapper image2)
        {
            this.image1 = image1;
            this.image2 = image2;
            DoPopulate();
        }//Populate

        /// <summary>
        /// Takes a PixelMatrix as image 1, which is expected to have a numerical colour
        /// in each cell
        /// </summary>
        /// <param name="comparision"></param>
        /// <param name="image2"></param>
        public void Populate(List<PixelColumn> comparision, BitmapWrapper image2)
        {
            this.Comparision = comparision;
            DoPopulate();
        }

        /// <summary>
        /// Does the actual population of the Pixel Matrix
        /// </summary>
        private void DoPopulate()
        {
            Columns = new List<PixelColumn>();
            Comparator = new List<PixelColumn>();

            //set the search dimensions
            if (SearchWidth <= 0) { SearchWidth = image1.bitmap.Width; }
            if (SearchHeight <= 0) { SearchHeight = image1.bitmap.Height; }

            //set thepixel scanning dimensions
            if (WidthSearchOffset < 1) { WidthSearchOffset = 1; }
            if (HeightSearchOffset < 1) { HeightSearchOffset = 1; }

            if (GridSystemOn) { imageGrid = new ImageGrid(SearchWidth, SearchHeight); }

            //set some grid variables here, for scope reasons
            GridColumn gridColumn = null;
            Grid grid = null;

            //look at every pixel in each image, and compare the colours
            for (int i = 0; i < SearchWidth; i += WidthSearchOffset)
            {
                PixelColumn column = new PixelColumn();
                PixelColumn comparatorColumn = new PixelColumn();

                //set a new grid column, if required
                if (GridSystemOn)
                {
                    if (i == 0) { gridColumn = new GridColumn(); }
                    else if (i % imageGrid.GridWidth == 0) { imageGrid.Columns.Add(gridColumn); gridColumn = new GridColumn(); }
                }

                for (int n = 0; n < SearchHeight; n += HeightSearchOffset)
                {
                    PixelCell cell = new PixelCell();
                    PixelCell comparatorCell = new PixelCell();

                    //set a new grid, if required              
                    if (GridSystemOn)
                    {
                        if (n > 0 && n % imageGrid.GridHeight == 0 && i % imageGrid.GridWidth == 0)
                        {
                            gridColumn.grids.Add(grid);
                            grid = new Grid();
                        }
                        else if (n == 0 && i % imageGrid.GridWidth == 0)
                        {
                            grid = new Grid();
                        }
                    }

                    //get the pixel colours. image 1 pixel one either comes from image1, or the comparison PixelMatrix, if it exists
                    Int64 image1Pixel = Comparision == null ? image1.bitmap.GetPixel(i, n).Name.HexToLong() : image2.bitmap.GetPixel(i, n).Name.HexToLong();
                    Int64 image2Pixel = image2.bitmap.GetPixel(i, n).Name.HexToLong();

                    //set the comparator
                    comparatorCell.colour = image2Pixel;
                    comparatorColumn.cells.Add(comparatorCell);

                    if (image1Pixel != image2Pixel)
                    {
                        cell.hasChanged = true;
                        cell.change = image1Pixel - image2Pixel;
                        if (GridSystemOn) { grid.change += cell.positiveChange; }
                    }
                    else
                    {
                        cell.hasChanged = false;
                    }

                    column.cells.Add(cell);

                    if (GridSystemOn && n + 1 == image1.bitmap.Height && i % imageGrid.GridWidth == 0) { gridColumn.grids.Add(grid); }

                }//height

                Columns.Add(column);
                Comparator.Add(comparatorColumn);

                if (GridSystemOn && i + 1 == image1.bitmap.Width) { imageGrid.Columns.Add(gridColumn); }

            }//width
        }

        /// <summary>
        /// Dumps the pixel data to a text file
        /// Dumps into column order (i.e. last entry is last column, last row)
        /// </summary>
        /// <param name="textfile"></param>
        public void DumpToText(string textfile)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(textfile, true))
            {
                for(int i = 0; i < Columns.Count; i++)
                {
                    for(int n = 0; n < Columns[i].cells.Count; n++)
                    {
                        file.WriteLine(Columns[i].cells[n].change);
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
            if(ReducedColumns == null) { throw new Exception("Can't create text file as reduced analysis not complete"); }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(textfile, true))
            {
                for (int i = 0; i < ReducedColumns.Count; i++)
                {
                    for (int n = 0; n < ReducedColumns[i].cells.Count; n++)
                    {
                        file.WriteLine(ReducedColumns[i].cells[n].colour);
                    }

                }
            }
        }//DumpToText

        /// <summary>
        /// Dumps the grid data to a text file
        /// Dumps in column order (i.e. last entry is last column, last row)
        /// </summary>
        /// <param name="textfile"></param>
        public void DumpGridToText(string textfile)
        {
            if (imageGrid.Columns == null) { throw new Exception("Can't create text file as grid system is not on"); }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(textfile, true))
            {
                for (int i = 0; i < imageGrid.Columns.Count; i++)
                {
                    for (int n = 0; n < imageGrid.Columns[i].grids.Count; n++)
                    {
                        file.WriteLine(imageGrid.Columns[i].grids[n].change);
                    }

                }
            }
        }

        /// <summary>
        /// Creates a new bitmap, each pixel represents the change number 
        /// from the appropriate cell. Divides into 255 seperate blocks
        /// </summary>
        /// <returns></returns>
        public Bitmap DrawPixelChanges()
        {
            if (Columns == null) { throw new Exception("Matrix has no columns"); }
            if (ReducedColumns == null) { SetReducedColumns();}

            Bitmap bm = new Bitmap(ReducedColumns.Count, ReducedColumns[0].cells.Count);

            for (int i = 0; i < bm.Width; i++)
            {
                for (int n = 0; n < bm.Height; n++)
                {
                    int color = Convert.ToInt16(ReducedColumns[i].cells[n].colour);
                    bm.SetPixel(i, n, Color.FromArgb(color, color, color, color));

                }//height

            }//width

            return bm;

        }//DrawPixelChanges

        /// <summary>
        /// Sets the reducedColumns structure, reduced the change value in each pixel
        /// to a range in between 0 and 255
        /// </summary>
        private void SetReducedColumns()
        {
            if (Columns == null) { throw new Exception("Matrix has no columns"); }
            ReducedColumns = new List<PixelColumn>();
          
            double min = MinChanged;
            double max = MaxChanged;
            double highest = -MinChanged > MaxChanged ? -MinChanged : MaxChanged; //everything goes to positive as interested in distance from 0

            double divisor = 255;
            double changesPerColor = highest / divisor;

            for (int i = 0; i < Columns.Count; i++)
            {
                PixelColumn col = new PixelColumn();
                col.cells = new List<PixelCell>();

                for (int n = 0; n < Columns[i].cells.Count ; n++)
                {
                    PixelCell cell = new PixelCell();
                    double change = Columns[i].cells[n].change < 0 ? -Columns[i].cells[n].change : Columns[i].cells[n].change;
                    cell.colour = 255 - Convert.ToInt16(change / changesPerColor);
                    col.cells.Add(cell);
                }//height

                ReducedColumns.Add(col);

            }//width

        }//SetReducedColumns

    }

    public class PixelColumn
    {
        public PixelColumn() { cells = new List<PixelCell>(); }
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

    public class PixelCell : CellAnalysis
    {
        public bool hasChanged { get; set; }

        public Int64 colour { get; set; }
    }

    public class CellAnalysis
    {        
        /// <summary>
        /// the numeric change between the color representation of the 2 pixels, can be negative
        /// </summary>
        public double change { get; set; }

        /// <summary>
        /// the numeric change between the color representation of the 2 pixels, all number are positive
        /// </summary>
        public double positiveChange { get { return change < 0 ? -change : change; } }

    }

}

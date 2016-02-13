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
        public List<PixelColumn> Columns { get; set; }
        public List<PixelColumn> ReducedColumns { get; set; } //the matrix, where the change values are reduced to a 0 - 255 range

        public List<GridColumn> GridColumns { get; set; } //the matrix represented in a grid system
        public bool GridSystemOn { get; set; }
        /// <summary>
        /// The number of grids that fit on a single row / column
        /// </summary>
        public int GridSplit { get; set; } = 4;
        private int gridWidth;
        private int gridHeight;

        /// <summary>
        /// Each pixel has a value which contains the numeric different between the two images, this
        /// method returns the sum of those differences
        /// </summary>
        public double SumChangedPixels { get { return (from col in Columns from cell in col.cells select cell.positiveChange).Sum(); } }

        public double MaxChanged { get { return (from c in Columns select c.maxChange).Max(); } }

        public double MinChanged { get { return (from c in Columns select c.minChange).Min(); } }

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
            Columns = new List<PixelColumn>();
            if (GridSystemOn) { SetGrids(image1.bitmap.Height, image1.bitmap.Width); GridColumns = new List<GridColumn>();}

            //set some grid variables here, for scope reasons
            GridColumn gridColumn = null;
            Grid grid = null;

            //look at every pixel in each image, and compare the colours
            for (int i = 0; i < image1.bitmap.Width; i++)
            {
                PixelColumn column = new PixelColumn();

                //set a new grid column, if required
                if (GridSystemOn)
                {                
                    if (i == 0) { gridColumn = new GridColumn(); }
                    else if (i % gridWidth == 0) { GridColumns.Add(gridColumn); gridColumn = new GridColumn(); }
                }
             
                for (int n = 0; n < image1.bitmap.Height; n++)
                {
                    PixelCell cell = new PixelCell();

                    //set a new grid, if required              
                    if (GridSystemOn)
                    {                     
                        if (n > 0 && n % gridHeight == 0 && i % gridWidth == 0)
                        {
                            gridColumn.grids.Add(grid);
                            grid = new Grid();
                        }
                        else if (n == 0 && i % gridWidth == 0)
                        {
                            grid = new Grid();
                        }
                    }

                    if (image1.bitmap.GetPixel(i, n).Name != image2.bitmap.GetPixel(i, n).Name)
                    {
                        cell.hasChanged = true;
                        cell.change = Int64.Parse(image1.bitmap.GetPixel(i, n).Name, System.Globalization.NumberStyles.HexNumber) - Int64.Parse(image2.bitmap.GetPixel(i, n).Name, System.Globalization.NumberStyles.HexNumber);
                        if (GridSystemOn) { grid.change += cell.change; }
                    }
                    else
                    {
                        cell.hasChanged = false;
                    }

                    column.cells.Add(cell);

                    if (GridSystemOn && n + 1 == image1.bitmap.Height && i % gridWidth == 0) { gridColumn.grids.Add(grid); }

                }//height

                Columns.Add(column);

                if (GridSystemOn && i + 1 == image1.bitmap.Width) { GridColumns.Add(gridColumn); }

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
                        file.WriteLine(ReducedColumns[i].cells[n].simpleColour);
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
            if (GridColumns == null) { throw new Exception("Can't create text file as grid system is not on"); }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(textfile, true))
            {
                for (int i = 0; i < GridColumns.Count; i++)
                {
                    for (int n = 0; n < GridColumns[i].grids.Count; n++)
                    {
                        file.WriteLine(GridColumns[i].grids[n].change);
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
                    int color = Convert.ToInt16(ReducedColumns[i].cells[n].simpleColour);
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
                    cell.simpleColour = 255 - Convert.ToInt16(change / changesPerColor);
                    col.cells.Add(cell);
                }//height

                ReducedColumns.Add(col);

            }//width

        }//SetReducedColumns

        /// <summary>
        /// Sets the gridsheight and width based on the size of the images
        /// The grid is always 4 x 4
        /// </summary>
        private void SetGrids(int imageHeight, int imageWidth)
        {
            gridHeight = imageHeight / GridSplit;
            gridWidth = imageWidth / GridSplit;
        }

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

        public int simpleColour { get; set; }
    }

    public class GridColumn
    {
        public GridColumn() { grids = new List<Grid>(); }
        public List<Grid> grids;
    }

    public class Grid : CellAnalysis
    {
        public double numberPixels { get; set; }
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

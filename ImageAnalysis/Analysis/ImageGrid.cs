using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysis.Analysis
{
    /// <summary>
    /// Divides an image into grids. Each grid represents a summary of the pixels that
    /// where held within the pixels it has replaced. Closely coupled with the PixelMatrix class
    /// the grid is populated through the Pixel Matrix class
    /// </summary>
    public class ImageGrid
    {
        public List<GridColumn> Columns;
        public int GridSplit { get; set; } = 4;
        public int GridHeight { get; set; }
        public int GridWidth { get; set; }

        public ImageGrid() { Columns = new List<GridColumn>(); }
        public ImageGrid(List<GridColumn> columns) { this.Columns = columns; }
        public ImageGrid(int height, int width) { Columns = new List<GridColumn>(); Setgrid(height, width); }
        public ImageGrid(int GridSplit) { this.GridSplit = GridSplit; Columns = new List<GridColumn>(); }

        /// <summary>
        /// The number of grids that fit on a single row / column
        /// </summary>
        public void Setgrid(int imageWidth, int imageHeight)
        {
            GridHeight = imageHeight / GridSplit;
            GridWidth = imageWidth / GridSplit;
        }

        /// <summary>
        /// Returns an integer for each grid
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public int GridNumber(int column, int row)
        {
            return row * column;
        }
    }

    public class GridColumn
    {
        public GridColumn() { grids = new List<Grid>(); }
        public List<Grid> grids;
    }

    public class Grid : CellAnalysis
    {
        public double numberPixels { get; set; }

        public double threshold { get; set; } //used when this grid cell is being used to define movement threshold
    }
}

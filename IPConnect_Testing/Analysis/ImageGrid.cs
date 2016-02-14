using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPConnect_Testing.Analysis
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
        public void Setgrid(int imageHeight, int imageWidth)
        {
            GridHeight = imageHeight / GridSplit;
            GridWidth = imageWidth / GridSplit;
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

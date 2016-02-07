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
    }

    public class PixelColumn
    {
        public List<PixelCell> cells { get; set; }
    }

    public class PixelCell
    {
        public bool hasChanged { get; set; }
    }

}

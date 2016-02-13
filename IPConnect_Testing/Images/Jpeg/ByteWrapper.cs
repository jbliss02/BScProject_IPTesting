using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPConnect_Testing.Images.Jpeg
{
    /// <summary>
    /// Conatins a JPEG, in byte form, with metadata about that file
    /// </summary>
    public class ByteWrapper
    {
        public byte[] bytes { get; set; }
        public DateTime created { get; set; }
        public int sequenceNumber { get; set; }
        public string saveFilePath { get; set; }
        public ByteWrapper(byte[] bytes, int sequenceNumber) { this.bytes = bytes; this.sequenceNumber = sequenceNumber; }
    }
}

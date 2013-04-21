using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCMViewer.Entity
{
    class ImageEntity
    {
        public string PhotometricInterpretation { get; set; }
        public string NumberOfFrames { get; set; }
        public string Rows { get; set; }
        public string Columns { get; set; }
        public string BitsAllocated { get; set; }
    }
}

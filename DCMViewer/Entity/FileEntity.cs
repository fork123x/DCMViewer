using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCMViewer.Entity
{
    class FileEntity
    {
        public string TransferSyntax { get; set; }
        public string MediaStorageSOPClassUID { get; set; }
        public string MediaStorageSOPInstanceUID { get; set; }
        public string SourceApplicationEntityTitle { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCMViewer.Entity
{
    class PatientEntity
    {
        public string PatientID { get; set; }
        public string PatientName { get; set; }
        public string PatientSex { get; set; }
        public string PatientBirthDate { get; set; }
        public string Modality { get; set; }
        public string StudyDate { get; set; }
    }
}

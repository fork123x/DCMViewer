using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCMViewer.Entity;
using Dicom;

namespace DCMViewer.Utility
{
    class EntityUtility
    {
        public static PatientEntity getPatientEntity(DicomFile df)
        {
            string patientID = df.Dataset.Get<string>(DicomTag.PatientID);
            string patientName = df.Dataset.Get<string>(DicomTag.PatientName);
            string patientSex = df.Dataset.Get<string>(DicomTag.PatientSex);
            string patientBirthDate = df.Dataset.Get<string>(DicomTag.PatientBirthDate);
            string modality = df.Dataset.Get<string>(DicomTag.Modality);
            string studyDate = df.Dataset.Get<string>(DicomTag.StudyDate);
            PatientEntity pe = new PatientEntity()
            {
                PatientID = patientID,
                PatientName = patientName,
                PatientSex = patientSex,
                PatientBirthDate = patientBirthDate,
                Modality = modality,
                StudyDate = studyDate
            };

            return pe;
        }

        public static ImageEntity getImageEntity(DicomFile df)
        {
            string photometricInterpretation = df.Dataset.Get<string>(DicomTag.PhotometricInterpretation);
            string numberOfFrames = df.Dataset.Get<string>(DicomTag.NumberOfFrames);
            string rows = df.Dataset.Get<string>(DicomTag.Rows);
            string columns = df.Dataset.Get<string>(DicomTag.Columns);
            string bitsAllocated = df.Dataset.Get<string>(DicomTag.BitsAllocated);

            ImageEntity ie = new ImageEntity()
            {
                PhotometricInterpretation = photometricInterpretation,
                NumberOfFrames = numberOfFrames,
                Rows = rows,
                Columns = columns,
                BitsAllocated = bitsAllocated
            };

            return ie;
        }

        public static FileEntity getFileEntity(DicomFile df)
        {
            string transferSyntax = df.FileMetaInfo.TransferSyntax.ToString();
            string mediaStorageSOPClassUID = df.FileMetaInfo.MediaStorageSOPClassUID.ToString();
            string mediaStorageSOPInstanceUID = df.FileMetaInfo.MediaStorageSOPInstanceUID.ToString();
            string sourceApplicationEntityTitle = df.FileMetaInfo.SourceApplicationEntityTitle.ToString();
            FileEntity fe = new FileEntity()
            {
                TransferSyntax = transferSyntax,
                MediaStorageSOPClassUID = mediaStorageSOPClassUID,
                MediaStorageSOPInstanceUID = mediaStorageSOPInstanceUID,
                SourceApplicationEntityTitle = sourceApplicationEntityTitle
            };

            return fe;
        }
    }
}

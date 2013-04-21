using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using DCMViewer.Entity;
using System.IO;

namespace DCMViewer
{
    class DBUtility
    {
        string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\\wd\\pic.accdb;";

        public bool DeleteRecord(int ID)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string str = "DELETE FROM PicTable WHERE ID=?";
                OleDbCommand cmd = new OleDbCommand(str, conn);
                cmd.Parameters.AddWithValue("@ID", ID);

                conn.Open();
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool UpdateRecord(int ID, string name, string path)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string str = "UPDATE PicTable SET pic_name=?, pic_path=? WHERE ID=?";
                OleDbCommand cmd = new OleDbCommand(str, conn);
                cmd.Parameters.AddWithValue("@pic_name", name);
                cmd.Parameters.AddWithValue("@pic_path", path);
                cmd.Parameters.AddWithValue("@ID", ID);

                conn.Open();
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public DataSet ReadRecords()
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            string str = "Select * FROM PicTable ORDER BY ID ASC";
            OleDbDataAdapter da = new OleDbDataAdapter(str, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "pic");

            return ds;
        }

        public DataSet test()
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            string str = "Select * FROM PicTable ORDER BY ID ASC";
            OleDbDataAdapter da = new OleDbDataAdapter(str, conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "pic");

            return ds;
        }

        public bool AppendFile(string filePath, PatientEntity pe, ImageEntity ie, FileEntity fe)
        {
            string fileName = Path.GetFileName(filePath);
            if (InsertFileInfo(fileName, filePath))
            {
                int dcmID = GetDCMID(fileName);
                if (AddDCMInfo(pe, ie, fe, dcmID))
                {
                    return true;
                }
            }
            
            return false;   
        }

        public bool InsertFileInfo(string fileName, string filePath)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string str = "INSERT INTO [PicTable] ([pic_name], [pic_path]) VALUES (?, ?)";
                OleDbCommand cmd = new OleDbCommand(str, conn);
                cmd.Parameters.AddWithValue("@pic_name", fileName);
                cmd.Parameters.AddWithValue("@pic_path", filePath);

                conn.Open();
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private int GetDCMID(string fileName)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string str = "Select ID FROM [PicTable] WHERE pic_name=?";
                OleDbCommand cmd = new OleDbCommand(str, conn);
                cmd.Parameters.AddWithValue("@pic_name", fileName);

                conn.Open();
                OleDbDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return reader.GetInt32(0);
            }
        }

        private bool AddDCMInfo(PatientEntity pe, ImageEntity ie, FileEntity fe, int DcmID)
        {
            if (InsertPatientInfo(pe, DcmID) && InsertImageInfo(ie, DcmID) && InsertFileMetaInfo(fe, DcmID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool InsertPatientInfo(PatientEntity pe, int DcmID)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string str = "INSERT INTO PatientInfo ( [PatientID], [PatientName], [PatientSex], [PatientBirthDate], [Modality], [StudyDate], [DcmID] ) VALUES (?, ?, ?, ?, ?, ?, ?)";
                OleDbCommand cmd = new OleDbCommand(str, conn);
                cmd.Parameters.AddWithValue("@PatientID", pe.PatientID);
                cmd.Parameters.AddWithValue("@PatientName", pe.PatientName);
                cmd.Parameters.AddWithValue("@PatientSex", pe.PatientSex);
                cmd.Parameters.AddWithValue("@PatientBirthDate", pe.PatientBirthDate);
                cmd.Parameters.AddWithValue("@Modality", pe.Modality);
                cmd.Parameters.AddWithValue("@StudyDate", pe.StudyDate);
                cmd.Parameters.AddWithValue("@DcmID", DcmID);

                conn.Open();
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool InsertImageInfo(ImageEntity ie, int DcmID)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string str = "INSERT INTO [ImageInfo] ( [PhotometricInterpretation], [NumberOfFrames], [Rows], [Columns], [BitsAllocated], [DcmID] ) VALUES (?, ?, ?, ?, ?, ?)";
                OleDbCommand cmd = new OleDbCommand(str, conn);
                cmd.Parameters.AddWithValue("@PhotometricInterpretation", ie.PhotometricInterpretation);
                cmd.Parameters.AddWithValue("@NumberOfFrames", ie.NumberOfFrames);
                cmd.Parameters.AddWithValue("@Rows", ie.Rows);
                cmd.Parameters.AddWithValue("@Columns", ie.Columns);
                cmd.Parameters.AddWithValue("@BitsAllocated", ie.BitsAllocated);
                cmd.Parameters.AddWithValue("@DcmID", DcmID);

                conn.Open();
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool InsertFileMetaInfo(FileEntity fe, int DcmID)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string str = "INSERT INTO [FileInfo] ( [TransferSyntax], [MediaStorageSOPClassUID], [MediaStorageSOPInstanceUID], [SourceApplicationEntityTitle], [DcmID] ) VALUES (?, ?, ?, ?, ?)";
                OleDbCommand cmd = new OleDbCommand(str, conn);
                cmd.Parameters.AddWithValue("@TransferSyntax", fe.TransferSyntax);
                cmd.Parameters.AddWithValue("@MediaStorageSOPClassUID", fe.MediaStorageSOPClassUID);
                cmd.Parameters.AddWithValue("@MediaStorageSOPInstanceUID", fe.MediaStorageSOPInstanceUID);
                cmd.Parameters.AddWithValue("@MediaStorageSOPInstanceUID", fe.SourceApplicationEntityTitle);
                cmd.Parameters.AddWithValue("@SourceApplicationEntityTitle", DcmID);

                conn.Open();
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public DataSet GetAllPatientInfo()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string str = "Select PatientID, PatientName, PatientSex, PatientBirthDate, Modality, StudyDate FROM PatientInfo ORDER BY DcmID ASC";
                OleDbDataAdapter da = new OleDbDataAdapter(str, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "PatientInfo");

                return ds;
            }
        }
    }
}

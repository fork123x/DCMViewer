using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace DCMViewer
{
    class DBUtility
    {
        string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\\wd\\pic.accdb;";

        public bool AppendRecord(string name, string path)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string str = "INSERT INTO PicTable (pic_name, pic_path) VALUES (?, ?)";
                OleDbCommand cmd = new OleDbCommand(str, conn);
                cmd.Parameters.AddWithValue("@pic_name", name);
                cmd.Parameters.AddWithValue("@pic_path", path);

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
    }
}

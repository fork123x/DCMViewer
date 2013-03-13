using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data;

namespace DCMViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DCMDecoder decoder;
        const string WORKINGDIRECTORY = @"F:\wd\";
        const string TEMPDIRECTORY = @"F:\temp\";
        DBUtility db = new DBUtility();

        public MainWindow()
        {
            InitializeComponent();
            ListBox_PicList.ItemsSource = db.ReadRecords().Tables[0].DefaultView;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "选中要添加的文件";
            dlg.Filter = "DICOM Files (*.dcm)|*.dcm";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName;
                decoder = new DCMDecoder();
                decoder.DicomFileName = fileName;
                image1.Source = decoder.CreateBipmap(decoder.pixels16, decoder.width, decoder.height);
                image1.Height = decoder.height;
                image1.Width = decoder.width;
                List<string> str = decoder.dicomInfo;

                List<PicTag> picTags = new List<PicTag>();
                string s1, s4, s5, s11, s12;

                for (int i = 0; i < str.Count; ++i)
                {
                    s1 = str[i];
                    ExtractStrings(s1, out s4, out s5, out s11, out s12);
                    PicTag picTag = new PicTag() { GroupTag = s11, ElementTag = s12, TagDes = s4, TagValue = s5 };
                    picTags.Add(picTag);
                }
                
                this.dataGrid1.ItemsSource = picTags;
            }
        }

        void ExtractStrings(string s1, out string s4, out string s5, out string s11, out string s12)
        {
            int ind;
            string s2, s3;
            ind = s1.IndexOf("//");
            s2 = s1.Substring(0, ind);
            s11 = s1.Substring(0, 4);
            s12 = s1.Substring(4, 4);
            s3 = s1.Substring(ind + 2);
            ind = s3.IndexOf(":");
            s4 = s3.Substring(0, ind);
            s5 = s3.Substring(ind + 1);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            image1.Height = 400;
            image1.Width = 400;
        }

        private void button_import_Click(object sender, RoutedEventArgs e)
        {
            DBUtility db = new DBUtility();

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Title = "选中要添加的文件（可多选）";
            dlg.Filter = "DICOM Files (*.dcm)|*.dcm";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                foreach (string oldName in dlg.FileNames)
                {
                    string fileName = System.IO.Path.GetFileName(oldName);
                    string newName = WORKINGDIRECTORY + fileName;

                    try
                    {
                        File.Move(oldName, newName);
                        bool appendResult = db.AppendRecord(fileName, newName);
                        if (appendResult == true)
                        {
                            //UpdateDataSet();
                            ListBox_PicList.ItemsSource = db.ReadRecords().Tables[0].DefaultView;
                        }
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("工作目录中已存在同名文件：" + fileName);
                    }
                }
            }

        }

        private void ListBox_PicList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string fileName = ((DataRowView)ListBox_PicList.SelectedItem).Row[2].ToString();
            decoder = new DCMDecoder();
            decoder.DicomFileName = fileName;
            image1.Source = decoder.CreateBipmap(decoder.pixels16, decoder.width, decoder.height);
            image1.Height = decoder.height;
            image1.Width = decoder.width;
            List<string> str = decoder.dicomInfo;

            List<PicTag> picTags = new List<PicTag>();
            string s1, s4, s5, s11, s12;

            for (int i = 0; i < str.Count; ++i)
            {
                s1 = str[i];
                ExtractStrings(s1, out s4, out s5, out s11, out s12);
                PicTag picTag = new PicTag() { GroupTag = s11, ElementTag = s12, TagDes = s4, TagValue = s5 };
                picTags.Add(picTag);
            }
            this.dataGrid1.ItemsSource = picTags;
        }
    }

    public class PicTag
    {
        public string GroupTag
        {
            get;
            set;
        }
        public string ElementTag
        {
            get;
            set;
        }
        public string TagDes
        {
            get;
            set;
        }

        public string TagValue
        {
            get;
            set;
        }
    }

}

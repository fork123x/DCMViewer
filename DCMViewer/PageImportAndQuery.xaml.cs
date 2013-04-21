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
using Dicom;
using Dicom.Imaging;
using DCMViewer.Entity;
using System.Diagnostics;
using DCMViewer.Utility;
using System.Collections.ObjectModel;
using System.Data;

namespace DCMViewer
{
    /// <summary>
    /// Interaction logic for PageImportAndQuery.xaml
    /// </summary>
    public partial class PageImportAndQuery : Page
    {
        public event EventHandler<DisplayImageEventArgs> DisplayImageEvent;

        const string WORKINGDIRECTORY = @"F:\wd\dcms\";
        const string DEJPEGEXE = @"F:\wd\dcmdjpeg.exe";
        List<string> fileList;
        ObservableCollection<PatientEntity> pes = new ObservableCollection<PatientEntity>();
        ObservableCollection<ImageEntity> ies = new ObservableCollection<ImageEntity>();
        ObservableCollection<FileEntity> fes = new ObservableCollection<FileEntity>();

        public PageImportAndQuery()
        {
            InitializeComponent();
            DataGrid_Patient.ItemsSource = pes;
            DataGrid_Image.ItemsSource = ies;
            InitializeList();
        }

        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Title = "选中要添加的文件（可多选）";
            dlg.Filter = "DICOM Files (*.dcm)|*.dcm";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                foreach (string sourceNameFull in dlg.FileNames)
                {
                    string fileName = System.IO.Path.GetFileName(sourceNameFull);
                    string destNameFull = WORKINGDIRECTORY + fileName;

                    try
                    {
                        DicomFile df = DicomFile.Open(sourceNameFull);
                        if (df.FileMetaInfo.TransferSyntax.UID.UID == "1.2.840.10008.1.2.4.70")
                        {
                            string arg = sourceNameFull + " " + destNameFull;

                            ProcessStartInfo startInfo = new ProcessStartInfo();
                            startInfo.FileName = DEJPEGEXE;
                            startInfo.Arguments = arg;
                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                            using (Process proc = Process.Start(startInfo))
                            {
                                proc.WaitForExit();
                                int exitCode = proc.ExitCode;
                            }
                        }
                        else
                        {
                            File.Copy(sourceNameFull, destNameFull);
                        }
                        ImportFile(destNameFull);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("工作目录中已存在同名文件：" + fileName);
                    }
                }
                fileList = new List<string>(Directory.EnumerateFiles(WORKINGDIRECTORY));
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = DataGrid_Patient.SelectedIndex;
            if (index != -1)
            {
                string deletePath = fileList[index];
                File.Delete(deletePath);
            }
            InitializeList();
        }

        private void Button_OpenImage_Click(object sender, RoutedEventArgs e)
        {
            int index = DataGrid_Patient.SelectedIndex;

            if (index != -1)
            {
                DisplayImage(fileList[index]);
            }
        }

        private void DataGrid_Patient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid_Image.SelectedIndex = DataGrid_Patient.SelectedIndex;
            GetFileInfo(DataGrid_Patient.SelectedIndex);
        }

        private void DataGrid_Image_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid_Patient.SelectedIndex = DataGrid_Image.SelectedIndex;
            GetFileInfo(DataGrid_Image.SelectedIndex);
        }

        private void GetFileInfo(int index)
        {
            if (index != -1)
            {
                ListBox_FileInfo.Items.Clear();

                FileEntity fe = fes[index];
                ListBox_FileInfo.Items.Add(fe.TransferSyntax);
                ListBox_FileInfo.Items.Add(fe.MediaStorageSOPClassUID);
                ListBox_FileInfo.Items.Add(fe.MediaStorageSOPInstanceUID);
                ListBox_FileInfo.Items.Add(fe.SourceApplicationEntityTitle);
            }
        }

        private void DataGrid_Patient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DisplayImage(fileList[DataGrid_Patient.SelectedIndex]);
        }

        private void DisplayImage(string imagePath)
        {
            RaiseDisplayImageEvent(imagePath);
        }

        private void InitializeList()
        {
            fileList = new List<string>(Directory.EnumerateFiles(WORKINGDIRECTORY));

            pes.Clear();
            ies.Clear();
            fes.Clear();
            foreach (string filePath in fileList)
            {
                ImportFile(filePath);
            }
        }

        private void ImportFile(string filePath)
        {
            DicomFile df = DicomFile.Open(filePath);
            PatientEntity pe = EntityUtility.getPatientEntity(df);
            ImageEntity ie = EntityUtility.getImageEntity(df);
            FileEntity fe = EntityUtility.getFileEntity(df);

            pes.Add(pe);
            ies.Add(ie);
            fes.Add(fe);
        }

        private void RaiseDisplayImageEvent(string imagePath)
        {
            if (DisplayImageEvent != null)
            {
                DisplayImageEvent(this, new DisplayImageEventArgs(imagePath));
            }
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
        }

    }

    public class DisplayImageEventArgs : EventArgs
    {
        public DisplayImageEventArgs(string path)
        {
            ImagePath = path;
        }

        public string ImagePath
        {
            get;
            set;
        }
    }

}

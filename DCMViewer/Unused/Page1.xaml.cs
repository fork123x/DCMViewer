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
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        const string WORKINGDIRECTORY = @"F:\wd\dcms\";
        const string DEJPEGEXE = @"F:\wd\dcmdjpeg.exe";
        DBUtility db = new DBUtility();
        List<string> files;
        ObservableCollection<PatientEntity> pes = new ObservableCollection<PatientEntity>();
        ObservableCollection<ImageEntity> ies = new ObservableCollection<ImageEntity>();
        ObservableCollection<FileEntity> fes = new ObservableCollection<FileEntity>();

        public Page1()
        {
            InitializeComponent();
            DataGrid_Patient.ItemsSource = pes;
            DataGrid_Image.ItemsSource = ies;
            //InitializeList();
            //ReadAndDisplay();
            //TestAddFile();
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
                //files = new List<string>(Directory.EnumerateFiles(WORKINGDIRECTORY));
                //ReadAndDisplay();
            }
        }

        private void Button_Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "选中要添加的文件";
            dlg.Filter = "DICOM Files (*.dcm)|*.dcm";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                DisplayImageAndTags(dlg.FileName);
            }
        }

        private void DisplayImageAndTags(string fileName)
        {
            AddPicTags(fileName);
        }

        private void AddPicTags(string path)
        {
            DicomFile df = DicomFile.Open(path);
            string patientID = df.Dataset.Get<string>(DicomTag.PatientID);
            string patientName = df.Dataset.Get<string>(DicomTag.PatientName);
            string patientSex = df.Dataset.Get<string>(DicomTag.PatientSex);
            string patientBirthDate = df.Dataset.Get<string>(DicomTag.PatientBirthDate);
            string modality = df.Dataset.Get<string>(DicomTag.Modality);
            string studyDate = df.Dataset.Get<string>(DicomTag.StudyDate);
            PatientEntity patient = new PatientEntity() 
            {
                PatientID = patientID, 
                PatientName = patientName, 
                PatientSex = patientSex, 
                PatientBirthDate = patientBirthDate, 
                Modality = modality, 
                StudyDate = studyDate
            };
            List<PatientEntity> patients = new List<PatientEntity>();
            patients.Add(patient);
            DataGrid_Patient.ItemsSource = patients;

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
            List<ImageEntity> imageEntitys = new List<ImageEntity>();
            imageEntitys.Add(ie);
            DataGrid_Image.ItemsSource = imageEntitys;

            string i = df.FileMetaInfo.TransferSyntax.ToString();
            string ii = df.FileMetaInfo.MediaStorageSOPClassUID.ToString();
            string iii = df.FileMetaInfo.MediaStorageSOPInstanceUID.ToString();
            string iiii = df.FileMetaInfo.SourceApplicationEntityTitle.ToString();
            ListBox_FileInfo.Items.Add(i);
            ListBox_FileInfo.Items.Add(ii);
            ListBox_FileInfo.Items.Add(iii);
            ListBox_FileInfo.Items.Add(iiii);
        }

        private void Button_OpenImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "选中要添加的文件";
            dlg.Filter = "DICOM Files (*.dcm)|*.dcm";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                DisplayImage(dlg.FileName);
            }
        }

        private void DisplayImage(string path)
        {
            Window1.getPage2().GetImage(path);
        }

        private void ReadAndDisplay()
        {
            List<PatientEntity> patientEntitys = new List<PatientEntity>();
            List<ImageEntity> imageEntitys = new List<ImageEntity>();

            foreach (string fileName in files)
            {
                DicomFile df = DicomFile.Open(fileName);

                PatientEntity pe = EntityUtility.getPatientEntity(df); 
                patientEntitys.Add(pe);
                ImageEntity ie = EntityUtility.getImageEntity(df);
                imageEntitys.Add(ie);
            }
            DataGrid_Patient.ItemsSource = patientEntitys;
            DataGrid_Image.ItemsSource = imageEntitys;
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
            DicomFile df = DicomFile.Open(files[index]);
            ListBox_FileInfo.Items.Clear();
            string i = df.FileMetaInfo.TransferSyntax.ToString();
            string ii = df.FileMetaInfo.MediaStorageSOPClassUID.ToString();
            string iii = df.FileMetaInfo.MediaStorageSOPInstanceUID.ToString();
            string iiii = df.FileMetaInfo.SourceApplicationEntityTitle.ToString();
            ListBox_FileInfo.Items.Add(i);
            ListBox_FileInfo.Items.Add(ii);
            ListBox_FileInfo.Items.Add(iii);
            ListBox_FileInfo.Items.Add(iiii);
        }

        private void DataGrid_Patient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DisplayImage(files[DataGrid_Patient.SelectedIndex]);
        }

        private void TestAddFile()
        {
            DicomFile df = DicomFile.Open(@"F:\wd\dcms\Abdomen.dcm");
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
        }

        private void InitializeList()
        {
            DataSet ds =  db.GetAllPatientInfo();
            int i = 1;
            i++;
        }

        private bool ImportFile(string filePath)
        {
            DicomFile df = DicomFile.Open(filePath);
            PatientEntity pe = EntityUtility.getPatientEntity(df);
            ImageEntity ie = EntityUtility.getImageEntity(df);
            FileEntity fe = EntityUtility.getFileEntity(df);

            return db.AppendFile(filePath, pe, ie, fe);
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            DicomFile df1 = DicomFile.Open(@"F:\wd\dcms\Abdomen.dcm");
            PatientEntity pe = EntityUtility.getPatientEntity(df1);
            ImageEntity ie = EntityUtility.getImageEntity(df1);
            FileEntity fe = EntityUtility.getFileEntity(df1);
            MessageBox.Show(db.AppendFile(@"F:\wd\dcms\Abdomen.dcm", pe, ie, fe).ToString());
        }
    }
}

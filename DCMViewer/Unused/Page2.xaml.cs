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
using Dicom.Imaging;
using Dicom;

namespace DCMViewer
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        DicomImage di;
        int i = 0;

        public Page2()
        {
            InitializeComponent();
        }

        public void GetImage(string path)
        {
            di = new DicomImage(path);
            Image_DCMImage.Source = di.RenderImageSource();

            double radio = di.Width / 400.0;
            label1.Content = GetLength(radio).ToString() + " cm";
        }

        private void Button_NextFrame_Click(object sender, RoutedEventArgs e)
        {
            
            Image_DCMImage.Source = di.RenderImageSource(i++);
        }

        private double GetLength(double radio)
        {
            double scale = 0;
            double length;
            DicomTag dt = new DicomTag(0x0028, 0x0030);
            DicomItem pixelSpacing = di.Dataset.Get<DicomItem>(dt);

            DicomTag dt2 = new DicomTag(0x0018, 0x1164);
            DicomItem imagerPixelSpacing = di.Dataset.Get<DicomItem>(dt2);

            if (imagerPixelSpacing is DicomElement)
            {
                DicomElement de = imagerPixelSpacing as DicomElement;
                double[] s = de.Get<double[]>();
                scale = s[0];
            }

            if (pixelSpacing is DicomElement)
            {
                DicomElement de = pixelSpacing as DicomElement;
                double[] s = de.Get<double[]>();
                scale = s[0];
            }

            length = 10 * scale * radio;

            return length;
        }

        private void Button_DefaultSize_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_OpenImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = "选中要添加的文件";
            dlg.Filter = "DICOM Files (*.dcm)|*.dcm";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                GetImage(dlg.FileName);
            }
        }
    }
}

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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DCMViewer
{
    /// <summary>
    /// Interaction logic for MainFrameWindow.xaml
    /// </summary>
    public partial class MainFrameWindow : Window
    {
        private PageImportAndQuery pageImportAndQuery = new PageImportAndQuery();
        private PageImageDisplay pageImageDisplay = new PageImageDisplay();
        private DispatcherTimer dateTimer;

        public MainFrameWindow()
        {
            InitializeComponent();
            InitializeTimer();
            pageImportAndQuery.DisplayImageEvent += new EventHandler<DisplayImageEventArgs>(pageImportAndQuery_DisplayEvent);
            
            mainFrame.Navigate(pageImportAndQuery);
        }

        private void pageImportAndQuery_DisplayEvent(object sender, DisplayImageEventArgs e)
        {
            pageImageDisplay.DisplayImage(e.ImagePath);
            mainFrame.Navigate(pageImageDisplay);
        }

        private void InitializeTimer()
        {
            dateTimer = new DispatcherTimer();
            dateTimer.Interval = TimeSpan.FromMilliseconds(1000);
            dateTimer.Tick += new EventHandler(delegate(object s, EventArgs a)
            {
                TextBlock_Time.Text = DateTime.Now.ToString();
            });
            dateTimer.Start();
        }

        private void Button_DisplayFile_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(pageImportAndQuery);
        }

        private void Button_DisplayImage_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(pageImageDisplay);
        }
    }
}

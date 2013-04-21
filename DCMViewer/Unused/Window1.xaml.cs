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
using System.Windows.Navigation;

namespace DCMViewer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        static Page1 page1 = new Page1();
        static Page2 page2 = new Page2();

        public Window1()
        {
            InitializeComponent();
            frame1.Navigate(page1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            frame1.Navigate(page1);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            frame1.Navigate(page2);
        }

        public static Page1 getPage1()
        {
            return page1;
        }

        public static Page2 getPage2()
        {
            return page2;
        }

    }
}

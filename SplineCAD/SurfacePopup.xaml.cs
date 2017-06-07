using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SplineCAD
{
    /// <summary>
    /// Interaction logic for SurfacePopup.xaml
    /// </summary>
    public partial class SurfacePopup : Window
    {
        public bool cancelled;

        public SurfacePopup()
        {
            InitializeComponent();
            cancelled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cancelled = false;
            this.Close();
        }
    }
}

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using QSoft.ColorSpaceCOnvert;

namespace PixelViwer
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        MainUI m_MainUI;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.m_MainUI == null)
            {
                this.DataContext = this.m_MainUI = new MainUI();
            }
        }
    }

    public class MainUI : INotifyPropertyChanged
    {
        public List<ColorSpaces> ColorSpaces { set; get; } = new List<ColorSpaces>();
        public MainUI()
        {
           foreach(var oo in Enum.GetNames(typeof(ColorSpaces)))
            {
                //this.ColorSpaces.Add(oo);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void Update(string name)
        {
            if(this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}

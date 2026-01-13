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
using System.IO;
using QSoft.YUV;
using System.Numerics;
using System.Threading;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace PixelViwer
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {

        readonly MainUI m_MainUI;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this.m_MainUI = new MainUI();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InputScope scope = new InputScope();
            InputScopeName name = new InputScopeName();
            name.NameValue = InputScopeNameValue.AlphanumericHalfWidth;
            scope.Names.Add(name);
            this.InputScope = scope;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {

        }
    }


    //ffmpeg -i 123.jpg -pix_fmt yuyv422 123.yuv
    public class MainUI : INotifyPropertyChanged
    {
        int m_Width = 6000;
        int m_Height = 3376;
        WriteableBitmap m_Bmp;
        public int Width
        {
            get => m_Width;
            set { m_Width = value; Update(); }
        }
        public int Height
        {
            get => m_Height;
            set { m_Height = value; Update(); }
        }
        public WriteableBitmap Bmp
        {
            get => m_Bmp;
            set { m_Bmp = value; Update(); }
        }

        

        public MainUI()
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
        void Update([CallerMemberName]string name = "")
           =>this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

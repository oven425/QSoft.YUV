﻿using System;
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
            var yuy2_raw = File.ReadAllBytes("../../../720-404-yuy2.yuv");
            var y = yuy2_raw.Where((x, index) => { return index % 2 == 0; }).ToArray();
            var u = yuy2_raw.Where((x, index) => { return index % 3 == 0; }).ToArray();
            List<byte> ys = new List<byte>();
            for (int i=0; i<yuy2_raw.Length; i=i+2)
            {
                ys.Add(yuy2_raw[i]);
            }

            //var yuy2 = new YUY2(yuy2_raw, 720, 404);
            //var rgb = yuy2.ToRGB();

            //File.WriteAllBytes("rgb", rgb);

            //image.Source = yuy2.ToBitmapSource();

            return;


            //byte[] yuv444p_raw = File.ReadAllBytes("../../../720-404-yuv444p.yuv");

            
            //var yuv444p = new YUY444p(yuv444p_raw, 720, 404);
            //var rgb = yuv444p.ToRGB();
            
            //File.WriteAllBytes("rgb", rgb);

            //image.Source = yuv444p.ToBitmapSource();
            //return;

            //File.WriteAllBytes("y", yuv444p.Y.ToArray());
            //File.WriteAllBytes("u", yuv444p.U.ToArray());
            //File.WriteAllBytes("v", yuv444p.V.ToArray());
            if (this.m_MainUI == null)
            {
                this.DataContext = this.m_MainUI = new MainUI();
                byte[] bb = File.ReadAllBytes("../../../720-404-rgb24.rgb");
                int len = 720 * 404 * 3;
                for(int i=0; i<len; i=i+3)
                {
                    //bb[i] = 0;
                    bb[i+1] = 0;
                    bb[i + 2] = 0;
                }
                byte[] buffer = bb; 
                var width = 720; // for example
                var height = 404; // for example
                var dpiX = 96d;
                var dpiY = 96d;
                var pixelFormat = PixelFormats.Rgb24; // grayscale bitmap
                var bytesPerPixel = (pixelFormat.BitsPerPixel + 7) / 8;
                var stride = bytesPerPixel * width; // == width in this example
                var bmp = BitmapSource.Create(width, height, dpiX, dpiY,
                                                 pixelFormat, null, buffer, stride);
                this.image.Source = bmp;

                File.WriteAllBytes("111", bb);
                //bool exist = File.Exists("../../../720-404-yuy2.yuv");
                //FileStream fs = File.OpenRead("../../../720-404-yuy2.yuv");
                //byte[] buf = new byte[4];
                //int read_len = fs.Read(buf, 0, buf.Length);
            }


            //Color_Convert convert = new Color_Convert();
            //convert.ToRGB()
        }
    }
    //ffmpeg -i 123.jpg -pix_fmt yuyv422 123.yuv
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

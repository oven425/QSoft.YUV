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

        byte rgb11(double src)
        {
            if(src>byte.MaxValue)
            {
                return 255;
            }
            else if(src< byte.MinValue)
            {
                return 0;
            }
            
            return (byte)src;
        }

        byte rgb11(int src)
        {
            if (src > byte.MaxValue)
            {
                return 255;
            }
            else if (src < byte.MinValue)
            {
                return 0;
            }

            return (byte)src;
        }

        (byte r, byte g, byte b)  yuv2rgb_1((byte y, byte u, byte v)src)
        {
            double Y = src.y;
            double V = src.v;
            double U = src.u;
            Y -= 16;
            U -= 128;
            V -= 128;
            var R = 1.164 * Y + 1.596 * V;
            var G = 1.164 * Y - 0.392 * U - 0.813 * V;
            var B = 1.164 * Y + 2.017 * U;

            var r = rgb11(R);
            var g = rgb11(G);
            var b = rgb11(B);
            return (r, g, b);
        }

        (byte r, byte g, byte b) yuv2rgb_2((byte y, byte u, byte v) src)
        {
            //return (0, 0, 0);
            int Y = src.y;
            int V = src.v;
            int U = src.u;
            Y -= 16;
            U -= 128;
            V -= 128;
            var R = 74 * Y + 102 * V;
            var G = 74 * Y - 25 * U - 52 * V;
            var B = 74 * Y + 129 * U;

            var r = rgb11(R>>6);
            var g = rgb11(G>>6);
            var b = rgb11(B >> 6);
            return (r, g, b);
        }

        MainUI m_MainUI;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ssss = System.Numerics.Vector.IsHardwareAccelerated;
            var v1 = new Vector2(0.1f, 0.2f);
            var v2 = new Vector2(1.1f, 2.2f);
            var vResult = v1 + v2;
            BitmapImage bmp_src = new BitmapImage();
            bmp_src.BeginInit();
            bmp_src.StreamSource = File.OpenRead("../../../s1.jpg");
            bmp_src.EndInit();
            this.image_src.Source = bmp_src;

            byte[] yuv444p_raw = File.ReadAllBytes("../../../s1-yuv444p.yuv");
            var yuv444p = new QSoft.YUV.YUV444P(yuv444p_raw, 6000, 3376, yuv2rgb_2);
            var sw= System.Diagnostics.Stopwatch.StartNew();
            for(int i=0; i<10; i++)
            {
                var rgb111 = yuv444p.ToRGB();
            }
            sw.Stop();
            System.Diagnostics.Trace.WriteLine(sw.ElapsedMilliseconds / 10);
            this.image.Source = yuv444p.ToRGB().ToBitmapSource(yuv444p.Width, yuv444p.Height);

            return;
            
            int w = 720;
            int h = 404;
            //var yuv420p_raw = File.ReadAllBytes("../../../720-404-yuv420p.yuv");
            //var ys = yuv420p_raw.Take(w * h * 2);


            var nv12_raw = File.ReadAllBytes("../../../720-404-nv12.yuv");

            //var ys = nv12_raw.Take(720 * 404).ToList();
            //var uvs = nv12_raw.Skip(720 * 404).ToList();
            //var us = uvs.Where((x, index) => index % 2 == 0);
            //var vs = uvs.Where((x, index) => index % 2 == 0);

            int rgbindex = 0;
            var rgb = new byte[720 * 404 * 3];
            //NV12ToRGB(nv12_raw, rgb, 720, 404);
            int uv_offset = 720 * 404;
            int uv_index = uv_offset;
            int y_index = 0;
            List<(byte[], byte, byte)> llll = new List<(byte[], byte, byte)>();
            for (int y = 0; y < h; y=y+2)
            {
                for (int x = 0; x < w; x = x + 2)
                {
                    var y1 = nv12_raw[x + 0 + y * w];
                    var y2 = nv12_raw[x + 1 + y * w];
                    var y3 = nv12_raw[x + 0 + (y+1) * w];
                    var y4 = nv12_raw[x + 1 + (y+1) * w];

                    byte u = nv12_raw[uv_index + 0];
                    byte v = nv12_raw[uv_index + 1];
                    var rgb1 = (y: y1, u: u, v: v).ToRGB();
                    rgbindex = x + 0 + y * w;
                    rgbindex = rgbindex * 3;
                    rgb[rgbindex + 0] = rgb1.r;
                    rgb[rgbindex + 1] = rgb1.g;
                    rgb[rgbindex + 2] = rgb1.b;
                    var rgb2 = (y: y2, u: u, v: v).ToRGB();
                    rgb[rgbindex + 3] = rgb2.r;
                    rgb[rgbindex + 4] = rgb2.g;
                    rgb[rgbindex + 5] = rgb2.b;
                    var rgb3 = (y: y3, u: u, v: v).ToRGB();
                    rgbindex = x + 0 + (y + 1) * w;
                    rgbindex = rgbindex * 3;
                    rgb[rgbindex + 0] = rgb3.r;
                    rgb[rgbindex + 1] = rgb3.g;
                    rgb[rgbindex + 2] = rgb3.b;
                    var rgb4 = (y: y4, u: u, v: v).ToRGB();
                    rgb[rgbindex + 3] = rgb4.r;
                    rgb[rgbindex + 4] = rgb4.g;
                    rgb[rgbindex + 5] = rgb4.b;
                    rgbindex = rgbindex + 12;
                    uv_index = uv_index + 2;
                }
            }
                
            //for (int y=0; y<h; y++)
            //{
            //    int uv___ = y / 2;
            //    for(int x=0; x<w; x=x+2)
            //    {
            //        var j = y / 2;
            //        uv_index = uv_offset + j * 2;
            //        byte u = nv12_raw[uv_index + 0];
            //        byte v = nv12_raw[uv_index + 1];
            //        var rgb1 = (y: nv12_raw[y_index+0], u: u, v: v).ToRGB();
            //        rgb[rgbindex + 0] = rgb1.r;
            //        rgb[rgbindex + 1] = rgb1.g;
            //        rgb[rgbindex + 2] = rgb1.b;
            //        var rgb2 = (y: nv12_raw[y_index + 1], u: u, v: v).ToRGB();
            //        rgb[rgbindex + 3] = rgb2.r;
            //        rgb[rgbindex + 4] = rgb2.g;
            //        rgb[rgbindex + 5] = rgb2.b;
            //        y_index = y_index + 2;
            //        rgbindex = rgbindex + 6;
            //    }
            //}

            //int uv_offset = 720 * 404;
            //int uv_index = 0;
            //for (int i = 0; i < uv_offset; i = i + 4)
            //{
            //    var j = i / 4;
            //    uv_index = uv_offset + j * 2;
            //    byte u = nv12_raw[uv_index + 0];
            //    byte v = nv12_raw[uv_index + 1];
            //    var rgb1 = (y: nv12_raw[i + 0], u: u, v: v).ToRGB();
            //    rgb[rgbindex + 0] = rgb1.r;
            //    rgb[rgbindex + 1] = rgb1.g;
            //    rgb[rgbindex + 2] = rgb1.b;
            //    var rgb2 = (y: nv12_raw[i + 1], u: u, v: v).ToRGB();
            //    rgb[rgbindex + 3] = rgb2.r;
            //    rgb[rgbindex + 4] = rgb2.g;
            //    rgb[rgbindex + 5] = rgb2.b;
            //    var rgb3 = (y: nv12_raw[i + 2], u: u, v: v).ToRGB();
            //    rgb[rgbindex + 6] = rgb3.r;
            //    rgb[rgbindex + 7] = rgb3.g;
            //    rgb[rgbindex + 8] = rgb3.b;
            //    var rgb4 = (y: nv12_raw[i + 3], u: u, v: v).ToRGB();
            //    rgb[rgbindex + 9] = rgb4.r;
            //    rgb[rgbindex + 10] = rgb4.g;
            //    rgb[rgbindex + 11] = rgb4.b;
            //    rgbindex = rgbindex + 12;
            //}

            var bitmapsource = rgb.ToBitmapSource(720, 404);
            this.image.Source = bitmapsource;



            //List<byte> ys = new List<byte>();
            //for (int i=0; i<yuy2_raw.Length; i=i+2)
            //{
            //    ys.Add(yuy2_raw[i]);
            //}
            //List<byte> us = new List<byte>();
            //for(int i=1;i<yuy2_raw.Length;i=i+4)
            //{
            //    us.Add(yuy2_raw[i]);
            //}

            //List<byte> vs = new List<byte>();
            //for (int i = 3; i < yuy2_raw.Length; i = i + 4)
            //{
            //    vs.Add(yuy2_raw[i]);
            //}
            //List<byte> rgbs = new List<byte>();
            //for(int i=0; i<yuy2_raw.Length; i=i+4)
            //{
            //    var rgb1 = (yuy2_raw[i+0], yuy2_raw[i + 1], yuy2_raw[i + 3]).ToRGB();
            //    var rgb2 = (yuy2_raw[i+2], yuy2_raw[i + 1], yuy2_raw[i + 3]).ToRGB();
            //    rgbs.Add(rgb1.r);
            //    rgbs.Add(rgb1.g);
            //    rgbs.Add(rgb1.b);
            //    rgbs.Add(rgb2.r);
            //    rgbs.Add(rgb2.g);
            //    rgbs.Add(rgb2.b);

            //}

            //var bitmapsource = rgbs.ToArray().ToBitmapSource(720, 404);
            //this.image.Source = bitmapsource;
            //var ll = vs.Count + ys.Count + us.Count;

            //var yuy2 = new YUY2(yuy2_raw, 720, 404);
            //var rgb = yuy2.ToRGB();
            ////rgb.ToBitmapSource(720, 404);
            ////File.WriteAllBytes("rgb", rgb);

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

        //void NV12ToRGB(byte[] nv12, byte[] rgb, int width, int height)
        //{
        //    int size = width * height;
        //    int w, h, x, y, u, v, yIndex, uvIndex, rIndex, gIndex, bIndex;
        //    int y1192, r, g, b, uv448, uv_128;
        //    for (h = 0; h < height; h++)
        //    {
        //        for (w = 0; w < width; w++)
        //        {
        //            yIndex = h * width + w;
        //            uvIndex = (h / 2) * width + (w & (-2)) + size;
        //            u = nv12[uvIndex];
        //            v = nv12[uvIndex + 1];
        //            // YUV to RGB
        //            y1192 = 1192 * (nv12[yIndex] - 16);
        //            uv448 = 448 * (u - 128);
        //            uv_128 = 128 * (v - 128);
        //            r = (y1192 + uv448) >> 10;
        //            g = (y1192 - uv_128 - uv448) >> 10;
        //            b = (y1192 + uv_128) >> 10;
        //            // RGB clipping
        //            if (r < 0) r = 0;
        //            if (g < 0) g = 0;
        //            if (b < 0) b = 0;
        //            if (r > 255) r = 255;
        //            if (g > 255) g = 255;
        //            if (b > 255) b = 255;
        //            // Save RGB values
        //            rIndex = yIndex * 3;
        //            gIndex = rIndex + 1;
        //            bIndex = gIndex + 1;
        //            rgb[rIndex] = (byte)r;
        //            rgb[gIndex] = (byte)g;
        //            rgb[bIndex] = (byte)b;
        //        }
        //    }
        //}

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

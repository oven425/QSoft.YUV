using System.Collections.ObjectModel;
using System.IO;
using System.IO.MemoryMappedFiles;
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

namespace WpfApp_SIMD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainUI m_MainUI;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this.m_MainUI = new MainUI();
            //QSoft.YUV.Onnx.YUV.ExportYuvOnnx();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //using MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile("../../../../s1-yuv444p.yuv", FileMode.Open);
            //using var accessor = mmf.CreateViewAccessor(0, 100);
            //Span<byte> imageSpan = accessor.GetSpan<byte>(0, (int)totalByteCount);

            byte[] yuv444p_raw = File.ReadAllBytes("../../../../s1-yuv444p.yuv");
            QSoft.YUV.SIMD.YUV444P_SIMD yuv444p = new QSoft.YUV.SIMD.YUV444P_SIMD(yuv444p_raw, 6000, 3376);
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var rgb = yuv444p.ToBGRA_Vector128();
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"ToBGRA_Vector128: {sw.ElapsedMilliseconds} ms");
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var rgb256 = yuv444p.ToBGRA_Vector256();
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"ToBGRA_Vector256: {sw.ElapsedMilliseconds} ms");
            this.image.Source = rgb.ToBitmapSource1(6000, 3376);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            yuv444p.ToRGB();
            sw.Stop();
            System.Diagnostics.Trace.WriteLine($"ToRGB: {sw.ElapsedMilliseconds} ms");
            //this.image.Source = yuv444p.ToRGB_4().ToBitmapSource(6000, 3376);

        }
    }

    public class MainUI
    {
        public ObservableCollection<ImageData> ImageDatas { set; get; } = [];
    }

    public class ImageData
    {
        public string Name { set; get; }
        public BitmapSource Image { set; get; }
    }

    public static class YUVExtentsion
    {
        public static BitmapSource ToBitmapSource(this byte[] src, int width, int height)
        {
            PixelFormat pf = PixelFormats.Rgb24;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = src;
            BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);
            return bitmap;
        }

        public static BitmapSource ToBitmapSource1(this byte[] src, int width, int height)
        {
            PixelFormat pf = PixelFormats.Bgr32;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = src;
            BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);
            return bitmap;
        }
    }
}
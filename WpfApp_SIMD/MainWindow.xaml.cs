using System.IO;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            byte[] yuv444p_raw = File.ReadAllBytes("../../../../s1-yuv444p.yuv");
            QSoft.YUV.SIMD.YUV444P yuv444p = new QSoft.YUV.SIMD.YUV444P(yuv444p_raw, 6000, 3376);
            var rgb = yuv444p.ToRGB();
            this.image.Source = yuv444p.ToRGB().ToBitmapSource(6000, 3376);
        }
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
    }
}
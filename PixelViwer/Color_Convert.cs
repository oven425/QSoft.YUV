using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QSoft.ColorSpaceCOnvert
{
    public enum ColorSpaces
    {
        RGB8,
        ARGB,
        YUV444p,
        YUY2
    }
    //public class YUVChannel
    //{
    //    public byte[] Raw { protected set; get; }
    //    public ColorSpaces ColorSpace { protected set; get; }
    //    public List<byte> Y {protected set;get; }
    //    public List<byte> U { set; get; }
    //    public List<byte> V { set; get; }
    //    public YUVChannel(ColorSpaces colorspace, byte[] raw)
    //    {
    //        this.ColorSpace = colorspace;
    //        this.Raw = raw;
    //    }
    //}

    public interface IYUV
    {
        ColorSpaces ColorSpace { get; }
        IEnumerable<byte> Y { get; }
        IEnumerable<byte> U { get; }
        IEnumerable<byte> V { get; }
        byte[] Raw { get; }
    }

    public class NV12
    {
        public IEnumerable<byte> Y => m_Raw.Where((x, index) => index % 2 == 0);
        public IEnumerable<byte> U => m_Raw.Where((x, index) => index % 4 == 1);
        public IEnumerable<byte> V => m_Raw.Where((x, index) => index % 4 == 3);
        public int Width => m_Width;
        public int Height => m_Height;
        int m_Width;
        int m_Height;
        byte[] m_Raw;
        public NV12(byte[] raw, int width, int height)
        {
            this.m_Raw = raw;
            this.m_Width = width;
            this.m_Height = height;
        }
    }

    public class YUY444p : IYUV
    {
        public ColorSpaces ColorSpace => ColorSpaces.YUV444p;
        public IEnumerable<byte> Y => m_Raw.Take(this.m_Width * this.m_Height);
        public IEnumerable<byte> U => m_Raw.Skip(this.m_Width * this.m_Height).Take(this.m_Width * this.m_Height);
        public IEnumerable<byte> V => m_Raw.Skip(this.m_Width * this.m_Height * 2).Take(this.m_Width * this.m_Height);
        public byte[] Raw => m_Raw;
        public int Width => m_Width;
        public int Height => m_Height;
        int m_Width;
        int m_Height;
        byte[] m_Raw;
        public YUY444p(byte[] raw, int width, int height)
        {
            this.m_Raw = raw;
            this.m_Width = width;
            this.m_Height = height;
        }

        public byte[] ToRGB()
        {
            var rgb = new byte[m_Width * m_Height * 3];
            var y = this.Y.ToArray();
            var u = this.U.ToArray();
            var v = this.V.ToArray();
            int index = 0;
            for (int i = 0; i < y.Length; i++)
            {
                double Y = y[i];
                double V = v[i];
                double U = u[i];
                Y -= 16;
                U -= 128;
                V -= 128;
                var R = 1.164 * Y + 1.596 * V;
                var G = 1.164 * Y - 0.392 * U - 0.813 * V;
                var B = 1.164 * Y + 2.017 * U;
                rgb[index + 0] = (byte)(R > 255 ? 255 : R);
                rgb[index + 1] = (byte)(G > 255 ? 255 : G);
                rgb[index + 2] = (byte)(B > 255 ? 255 : B);
                index = index + 3;
            }

            return rgb;
        }
    }

    public class YUY2
    {
        public IEnumerable<byte> Y => m_Raw.Where((x, index) => index % 2 == 0);
        public IEnumerable<byte> U => m_Raw.Where((x, index) => index % 4 == 1);
        public IEnumerable<byte> V => m_Raw.Where((x, index) => index % 4 == 3);
        public int Width => m_Width;
        public int Height => m_Height;
        int m_Width;
        int m_Height;
        byte[] m_Raw;
        public YUY2(byte[] raw, int width, int height)
        {
            this.m_Raw = raw;
            this.m_Width = width;
            this.m_Height = height;
        }

        public byte[] ToRGB()
        {
            int rgbindex = 0;
            var rgb = new byte[m_Width * m_Height * 3];
            for (int i = 0; i < m_Raw.Length; i = i + 4)
            {
                var rgb1 = (y:m_Raw[i + 0], u:m_Raw[i + 1], v:m_Raw[i + 3]).ToRGB();
                var rgb2 = (y:m_Raw[i + 2], u:m_Raw[i + 1], v:m_Raw[i + 3]).ToRGB();
                rgb[rgbindex + 0] = rgb1.r;
                rgb[rgbindex + 1] = rgb1.g;
                rgb[rgbindex + 2] = rgb1.b;
                rgb[rgbindex + 3] = rgb2.r;
                rgb[rgbindex + 4] = rgb2.g;
                rgb[rgbindex + 5] = rgb2.b;
                rgbindex = rgbindex + 6;

            }

            return rgb;
        }

    }

    //https://blog.csdn.net/byhook/article/details/84037338
    public class YUV420P
    {

    }

    public static class YUVEx
    {
        public static (byte r,byte g,byte b) ToRGB(this (byte y, byte u, byte v) src)
        {
            var B = 1.164 * (src.y - 16) + 2.018 * (src.u - 128);

            var G = 1.164 * (src.y - 16) - 0.813 * (src.v - 128) - 0.391 * (src.u - 128);

            var R = 1.164 * (src.y - 16) + 1.596 * (src.v - 128);

            //double Y = src.y;
            //double V = src.v;
            //double U = src.u;
            //Y -= 16;
            //U -= 128;
            //V -= 128;
            //var R = 1.164 * Y + 1.596 * V;
            //var G = 1.164 * Y - 0.392 * U - 0.813 * V;
            //var B = 1.164 * Y + 2.017 * U;
            if (R > 255.0)
            {
                R = 255;
            }
            else if (R < 0)
            {
                R = 0;
            }
            if (G > 255.0)
            {
                G = 255;
            }
            else if (G < 0)
            {
                G = 0;
            }
            if (B > 255.0)
            {
                B = 255;
            }
            else if (B < 0)
            {
                B = 0;
            }
            var r = (byte)R;
            var g = (byte)G;
            var b = (byte)B;

            //var r = (byte)(R > 255 ? 255 : R);
            //var g = (byte)(G > 255 ? 255 : G);
            //var b = (byte)(B > 255 ? 255 : B);
            return (r,g,b);
        }
            
    }

    

    public static class YUVEx_WPF
    {
        public static BitmapSource ToBitmapSource(this YUY444p src)
        {
            PixelFormat pf = PixelFormats.Rgb24;
            int width = src.Width;
            int height = src.Height;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = src.ToRGB();
            BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);
            return bitmap;
        }

        public static BitmapSource ToBitmapSource(this YUY2 src)
        {
            PixelFormat pf = PixelFormats.Rgb24;
            int width = src.Width;
            int height = src.Height;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = src.ToRGB();
            BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, rawImage, rawStride);
            return bitmap;
        }

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

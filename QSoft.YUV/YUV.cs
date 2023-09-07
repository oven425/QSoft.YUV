using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.YUV
{
    abstract public class YUV
    {
        public int Width { protected set; get; }
        public int Height { protected set; get; }
        public byte[] Raw {  protected set; get; }
        abstract public IEnumerable<byte> Y {  get; }
        abstract public IEnumerable<byte> U { get; }
        abstract public IEnumerable<byte> V { get; }
        protected Func<(byte y, byte u, byte v), (byte r, byte g, byte b)> Func_yuv2rgb = YUVEx.ToRGB;
        public YUV(byte[] raw, int width, int height, Func<(byte y, byte u, byte v), (byte y, byte u, byte v)> yuv2rgbfunc)
        {
            this.Width = width;
            this.Height = height;
            Raw = new byte[raw.Length];
            Array.Copy(raw, Raw, raw.Length);
            if(yuv2rgbfunc != null)
            {
                this.Func_yuv2rgb = yuv2rgbfunc;
            }
        }

        abstract public byte[] ToRGB();
    }

    public static class YUVEx
    {
        public static (byte r, byte g, byte b) ToRGB(this (byte y, byte u, byte v) src)
        {
            //var B = 1.164 * (src.y - 16) + 2.018 * (src.u - 128);

            //var G = 1.164 * (src.y - 16) - 0.813 * (src.v - 128) - 0.391 * (src.u - 128);

            //var R = 1.164 * (src.y - 16) + 1.596 * (src.v - 128);

            double Y = src.y;
            double V = src.v;
            double U = src.u;
            Y -= 16;
            U -= 128;
            V -= 128;
            var R = 1.164 * Y + 1.596 * V;
            var G = 1.164 * Y - 0.392 * U - 0.813 * V;
            var B = 1.164 * Y + 2.017 * U;
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
            return (r, g, b);
        }

    }
}

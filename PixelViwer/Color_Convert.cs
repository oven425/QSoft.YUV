using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.ColorSpaceCOnvert
{
    public enum ColorSpaces
    {
        RGB8,
        ARGB,
        YUV444p,
        YUY2
    }
    public class YUVChannel
    {
        public byte[] Raw { protected set; get; }
        public ColorSpaces ColorSpace { protected set; get; }
        public List<byte> Y {protected set;get; }
        public List<byte> U { set; get; }
        public List<byte> V { set; get; }
        public YUVChannel(ColorSpaces colorspace, byte[] raw)
        {
            this.ColorSpace = colorspace;
            this.Raw = raw;
        }
    }

    public interface IYUV
    {
        ColorSpaces ColorSpace { get; }
        IEnumerable<byte> Y { get; }
        IEnumerable<byte> U { get; }
        IEnumerable<byte> V { get; }
        byte[] Raw { get; }
    }
    public class YUY444p : IYUV
    {
        public ColorSpaces ColorSpace => ColorSpaces.YUV444p;
        public IEnumerable<byte> Y => m_Raw.Take(this.m_Width * this.m_Height);
        public IEnumerable<byte> U => m_Raw.Skip(this.m_Width * this.m_Height).Take(this.m_Width * this.m_Height);
        public IEnumerable<byte> V => m_Raw.Skip(this.m_Width * this.m_Height*2).Take(this.m_Width * this.m_Height);
        public byte[] Raw => m_Raw;
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
            var rgb = new byte[m_Width*m_Height*3];
            var y = this.Y.ToArray();
            var u = this.U.ToArray();
            var v = this.V.ToArray();
            int index = 0;
            for(int i=0; i<y.Length; i++)
            {
                var Y = y[i];
                var V = v[i];
                var U = u[i];
                var R = Y + 1.400*V - 0.7;
                var G = Y - 0.343*U - 0.711*V + 0.526;
                var B = Y + 1.765*U - 0.883;
                
                rgb[index + 0] = (byte)(R>255?255:R);
                rgb[index + 1] = (byte)(G > 255 ? 255 : G);
                rgb[index + 2] = (byte)(B > 255 ? 255 : B);
                index = index + 3;
            }

            return rgb;
        }
    }

    //public static class YUVEx
    //{
    //    public static byte[] ToRGB(this YUY444p src)
    //    {
    //        src.Y.Zip(src.U, (x, y) => new { y=x,u=y });
    //        return null;
    //    }

    //}

    public class YUY2Raw
    {

    }

    

    public class Data
    {
        public byte Image { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }
        public ColorSpaces ColorSpace { set; get; }
    }

    public class Color_Convert
    {
        public void ToRGB(Data data)
        {

        }
        public byte[] ToRGB(byte[] src, ColorSpaces color, int width, int height)
        {
            byte[] dst = new byte[123];


            return dst;
        }
    }
}

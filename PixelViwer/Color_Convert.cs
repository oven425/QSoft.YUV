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
    }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.ColorSpaceCOnvert
{
    public class YUVChannel
    {
        public List<byte> Y { set; get; }
        public List<byte> U { set; get; }
        public List<byte> V { set; get; }
    }
    public class YUY2Raw
    {

    }

    public enum ColorSpaces
    {
        RGB8,
        ARGB,
        YUY2
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

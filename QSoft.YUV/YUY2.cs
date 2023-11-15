using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.YUV
{
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
                var rgb1 = (y: m_Raw[i + 0], u: m_Raw[i + 1], v: m_Raw[i + 3]).ToRGB();
                var rgb2 = (y: m_Raw[i + 2], u: m_Raw[i + 1], v: m_Raw[i + 3]).ToRGB();
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
}

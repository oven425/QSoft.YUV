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
        public YUV(byte[] raw, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Raw = new byte[raw.Length];
            Array.Copy(raw, Raw, raw.Length);
        }

        abstract public byte[] ToRGB();
    }
}

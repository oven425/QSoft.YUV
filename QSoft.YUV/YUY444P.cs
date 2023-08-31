using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.YUV
{
    public class YUV444P : YUV
    {
        public YUV444P(byte[] raw, int width, int height) 
            : base(raw, width, height)
        {
        }
        public override IEnumerable<byte> Y => Raw.Take(this.Width * this.Height);
        public override IEnumerable<byte> U => Raw.Skip(this.Width * this.Height).Take(this.Width * this.Height);
        public override IEnumerable<byte> V => Raw.Skip(this.Width * this.Height * 2).Take(this.Width * this.Height);


        public override byte[] ToRGB()
        {
            var rgb = new byte[this.Width * this.Height * 3];
            var y = this.Y.ToArray();
            var u = this.U.ToArray();
            var v = this.V.ToArray();
            int index = 0;
            for (int i = 0; i < y.Length; i++)
            {
                var rgbs = (y[i], u[i], v[i]).ToRGB();
                rgb[index + 0] = rgbs.r;
                rgb[index + 1] = rgbs.g;
                rgb[index + 2] = rgbs.b;
                index = index + 3;
            }

            return rgb;
        }
    }
}

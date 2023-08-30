using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.YUV
{
    public class YUY444P : YUV
    {
        public YUY444P(byte[] raw, int width, int height) 
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
}

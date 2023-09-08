using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.YUV
{
    public class YUV444P : YUV
    {
        public YUV444P(byte[] raw, int width, int height, Func<(byte y, byte u, byte v), (byte r, byte g, byte b)> yuv2rgbfunc=null) 
            : base(raw, width, height, yuv2rgbfunc)
        {
        }
        public override IEnumerable<byte> Y => Raw.Take(this.Width * this.Height);
        public override IEnumerable<byte> U => Raw.Skip(this.Width * this.Height).Take(this.Width * this.Height);
        public override IEnumerable<byte> V => Raw.Skip(this.Width * this.Height * 2).Take(this.Width * this.Height);
        
        public (byte y, byte u, byte v) MapRGB(int x, int y)
        {
            return (0, 0, 0);
        }
        
        public ((int x, int y)xy, (byte y, byte u, byte v)yuv) MapRGB()
        {
            return (xy: (0, 0), yuv: (0, 0, 0));
        }

        byte rgb11(double src)
        {
            if (src > byte.MaxValue)
            {
                return 255;
            }
            else if (src < byte.MinValue)
            {
                return 0;
            }

            return (byte)src;
        }

        byte rgb11(int src)
        {
            if (src > byte.MaxValue)
            {
                return 255;
            }
            else if (src < byte.MinValue)
            {
                return 0;
            }

            return (byte)src;
        }

        (byte r, byte g, byte b) yuv2rgb_2((byte y, byte u, byte v) src)
        {
            //return (0, 0, 0);
            int Y = src.y;
            int V = src.v;
            int U = src.u;
            Y -= 16;
            U -= 128;
            V -= 128;
            var R = 74 * Y + 102 * V;
            var G = 74 * Y - 25 * U - 52 * V;
            var B = 74 * Y + 129 * U;

            var r = rgb11(R >> 6);
            var g = rgb11(G >> 6);
            var b = rgb11(B >> 6);
            return (r, g, b);
        }

        void yuv2rgb_2(byte y, byte u, byte v, out byte r, out byte g, out byte b)
        {
            int Y = y;
            int V = v;
            int U = u;
            Y -= 16;
            U -= 128;
            V -= 128;
            var R = 74 * Y + 102 * V;
            var G = 74 * Y - 25 * U - 52 * V;
            var B = 74 * Y + 129 * U;

            r = rgb11(R >> 6);
            g = rgb11(G >> 6);
            b = rgb11(B >> 6);
            //return (r, g, b);
        }

        public override byte[] ToRGB()
        {
           
            var rgb = new byte[this.Width * this.Height * 3];
            List<byte> bytes = new List<byte>(rgb);
           int len = this.Width * this.Height;
            var y_index = 0;
            var u_index = this.Width * this.Height;
            var v_index = this.Width * this.Height * 2;

            int index = 0;
            for (int i = 0; i < len; i++)
            {
                //var rgbs = yuv2rgb_2((this.Raw[y_index + i], Raw[u_index + i], Raw[v_index + i]));
                //rgb[index + 0] = rgbs.r;
                //rgb[index + 1] = rgbs.g;
                //rgb[index + 2] = rgbs.b;

                //yuv2rgb_2(this.Raw[y_index + i], Raw[u_index + i], Raw[v_index + i], out var r, out var g, out var b);
                //rgb[index + 0] = r;
                //rgb[index + 1] = g;
                //rgb[index + 2] = b;
                bytes[index + 0] = 0;
                bytes[index + 1] = 0;
                bytes[index + 2] = 0;
                index = index + 3;
            }

            //var y = this.Y.ToArray();
            //var u = this.U.ToArray();
            //var v = this.V.ToArray();
            //int index = 0;
            //for (int i = 0; i < y.Length; i++)
            //{

            //    //var rgbs = (y[i], u[i], v[i]).ToRGB();
            //    var rgbs = this.Func_yuv2rgb((y[i], u[i], v[i]));
            //    rgb[index + 0] = rgbs.r;
            //    rgb[index + 1] = rgbs.g;
            //    rgb[index + 2] = rgbs.b;
            //    index = index + 3;
            //}

            return rgb;
        }
    }
}

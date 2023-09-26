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

        void RGB(byte y, byte u, byte v, out byte r, out byte g, out byte b)
        {
            r = 0;
            b = 0;g = 0;
        }

        public override byte[] ToRGB()
        {
            var rgb = new byte[this.Width * this.Height * 3];


            //var y = this.Y.ToArray();
            //var u = this.U.ToArray();
            //var v = this.V.ToArray();
            int index = 0;
            int y_index = 0;
            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height*2;
            //Span<byte> m_Span = new Span<byte>(Raw);
            for (int i = 0; i < u_index; i++)
            {
                RGB(Raw[i], Raw[i + u_index], Raw[i + v_index], out var r, out var g, out var b);
                //RGB(m_Span[i], m_Span[i + u_index], m_Span[i + v_index], out var r, out var g, out var b);

                //rgb[index + 0] = 0;
                //rgb[index + 1] = 0;
                //rgb[index + 2] = 0;
                //index = index + 3;


                //var rgbs = (y[i], u[i], v[i]).ToRGB();
                //var rgbs = this.Func_yuv2rgb((y[i], u[i], v[i]));
                //var rgbs = this.Func_yuv2rgb((Raw[i], Raw[i + u_index], Raw[i + v_index]));

                //rgb[index + 0] = rgbs.r;
                //rgb[index + 1] = rgbs.g;
                //rgb[index + 2] = rgbs.b;
                //index = index + 3;
            }

            return rgb;
        }
    }
}

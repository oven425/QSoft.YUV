using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.YUV
{
    public class YUV444P : YUV
    {
        [Obsolete]
        public YUV444P(byte[] raw, int width, int height, Func<(byte y, byte u, byte v), (byte r, byte g, byte b)> yuv2rgbfunc=null) 
            : base(raw, width, height, yuv2rgbfunc)
        {
        }
        protected event Yuv2RgbDelegate Yuv2Rgb = YUVEx.ToRGB1;
        public YUV444P(byte[] raw, int width, int height, Yuv2RgbDelegate yuv2rgbfunc)
            : base(raw, width, height, yuv2rgbfunc)
        {
            if(yuv2rgbfunc != null)
            {
                this.Yuv2Rgb = yuv2rgbfunc;
            }
        }


        public override IEnumerable<byte> Y => Raw.Take(this.Width * this.Height);
        public override IEnumerable<byte> U => Raw.Skip(this.Width * this.Height).Take(this.Width * this.Height);
        public override IEnumerable<byte> V => Raw.Skip(this.Width * this.Height * 2).Take(this.Width * this.Height);

        //override public byte[] ToRGB()
        //{
        //    var rgb = new byte[this.Width * this.Height * 3];
        //    int index = 0;
        //    int y_index = 0;
        //    int u_index = this.Width * this.Height;
        //    int v_index = this.Width * this.Height * 2;
        //    unsafe
        //    {
        //        fixed (byte* rgb_ptr = rgb)
        //        fixed (byte* yuv_ptr = this.Raw)
        //        {
        //            for (int i = 0; i < u_index; i++)
        //            {
        //                this.Yuv2Rgb(*(yuv_ptr + i), *(yuv_ptr + i + u_index), *(yuv_ptr + i + v_index), out var r, out var g, out var b);
        //                //ToRGB(Unsafe.Read<byte>(yuv_ptr + i), Unsafe.Read<byte>(yuv_ptr + i + u_index), Unsafe.Read<byte>(yuv_ptr + i + v_index), out var r, out var g, out var b);
        //                *(rgb_ptr + index + 0) = r;
        //                *(rgb_ptr + index + 1) = g;
        //                *(rgb_ptr + index + 2) = b;

        //                //Unsafe.Write(rgb_ptr + index + 0, r);
        //                //Unsafe.Write(rgb_ptr + index + 1, g);
        //                //Unsafe.Write(rgb_ptr + index + 2, b);
        //                index = index + 3;
        //            }
        //        }
        //    }
        //    return rgb;
        //}

        override public byte[] ToRGB()
        {
            var rgb = new byte[this.Width * this.Height * 3];

            int index = 0;
            int y_index = 0;
            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height * 2;


            for (int i = 0; i < u_index; i++)
            {
                this.Yuv2Rgb(Raw[i], Raw[i + u_index], Raw[i + v_index], out var r, out var g, out var b);
                rgb[index + 0] = r;
                rgb[index + 1] = g;
                rgb[index + 2] = b;
                index = index + 3;
            }
            return rgb;
        }


        public byte[] ToRGB_Old()
        {
            var rgb = new byte[this.Width * this.Height * 3];

            int index = 0;
            int y_index = 0;
            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height * 2;


            for (int i = 0; i < u_index; i++)
            {
                var rgbbuf = this.Func_yuv2rgb((Raw[i], Raw[i + u_index], Raw[i + v_index]));
                rgb[index + 0] = rgbbuf.r;
                rgb[index + 1] = rgbbuf.g;
                rgb[index + 2] = rgbbuf.b;
                index = index + 3;
            }
            return rgb;
        }



        //public void ToRGB(byte y, byte u, byte v, out byte r, out byte g, out byte b)
        //{
        //    r = g = b = 0;
        //    //var B = 1.164 * (src.y - 16) + 2.018 * (src.u - 128);

        //    //var G = 1.164 * (src.y - 16) - 0.813 * (src.v - 128) - 0.391 * (src.u - 128);

        //    //var R = 1.164 * (src.y - 16) + 1.596 * (src.v - 128);



        //    double Y = y;
        //    double V = v;
        //    double U = u;
        //    Y -= 16;
        //    U -= 128;
        //    V -= 128;

        //    var R = 1.164 * Y + 1.596 * V;
        //    var G = 1.164 * Y - 0.392 * U - 0.813 * V;
        //    var B = 1.164 * Y + 2.017 * U;
        //    if (R > 255.0)
        //    {
        //        R = 255;
        //    }
        //    else if (R < 0)
        //    {
        //        R = 0;
        //    }
        //    if (G > 255.0)
        //    {
        //        G = 255;
        //    }
        //    else if (G < 0)
        //    {
        //        G = 0;
        //    }
        //    if (B > 255.0)
        //    {
        //        B = 255;
        //    }
        //    else if (B < 0)
        //    {
        //        B = 0;
        //    }
        //    r = (byte)R;
        //    g = (byte)G;
        //    b = (byte)B;

        //    //var r = (byte)(R > 255 ? 255 : R);
        //    //var g = (byte)(G > 255 ? 255 : G);
        //    //var b = (byte)(B > 255 ? 255 : B);
        //}
    }
}

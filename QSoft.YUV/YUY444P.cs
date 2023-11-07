using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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




        public override IEnumerable<byte> Y => Raw.Take(this.Width * this.Height);
        public override IEnumerable<byte> U => Raw.Skip(this.Width * this.Height).Take(this.Width * this.Height);
        public override IEnumerable<byte> V => Raw.Skip(this.Width * this.Height * 2).Take(this.Width * this.Height);
       

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

            ////byte r = 0, g = 0, b = 0;
            //for (int i = 0; i < u_index; i++)
            //{

            //    //RGB1(Raw[i], Raw[i + u_index], Raw[i + v_index], ref r, ref g, ref b);


            //    //RGB(Raw[i], Raw[i + u_index], Raw[i + v_index], out var r, out var g, out var b);
            //    ////RGB(Buffer.GetByte(Raw, i), Buffer.GetByte(Raw, i + u_index), Buffer.GetByte(Raw, i + v_index), out var r, out var g, out var b);

            //    //rgb[index + 0] = r;
            //    //rgb[index + 1] = g;
            //    //rgb[index + 2] = b;
            //    //index = index + 3;


            //    //var rgbs = (y[i], u[i], v[i]).ToRGB();
            //    //var rgbs = this.Func_yuv2rgb((y[i], u[i], v[i]));
            //    var rgbs = this.Func_yuv2rgb((Raw[i], Raw[i + u_index], Raw[i + v_index]));

            //    rgb[index + 0] = rgbs.r;
            //    rgb[index + 1] = rgbs.g;
            //    rgb[index + 2] = rgbs.b;
            //    index = index + 3;
            //}

            //unsafe
            //{
            //    fixed (byte* rgb_ptr = rgb)
            //    fixed (byte* yuv_ptr = this.Raw)
            //    {
            //        for (int i = 0; i < u_index; i++)
            //        {

            //            //RGB(yuv_ptr[i], yuv_ptr[i + u_index], yuv_ptr[i + v_index], out var r, out var g, out var b);
            //            //rgb_ptr[index + 0] = r;
            //            //rgb_ptr[index + 1] = g;
            //            //rgb_ptr[index + 2] = b;
            //            //index = index + 3;


            //            //RGB(Unsafe.Read<byte>(yuv_ptr + i), Unsafe.Read<byte>(yuv_ptr + i + u_index), Unsafe.Read<byte>(yuv_ptr + i + v_index), out var r, out var g, out var b);
            //            //Unsafe.Write(yuv_ptr + index + 0, r);
            //            //Unsafe.Write(yuv_ptr + index + 1, g);
            //            //Unsafe.Write(yuv_ptr + index + 2, b);
            //            //index = index + 3;

            //            //var rgbs = (Unsafe.Read<byte>(yuv_ptr + i), Unsafe.Read<byte>(yuv_ptr + i + u_index), Unsafe.Read<byte>(yuv_ptr + i + v_index)).ToRGB();
            //            //Unsafe.Write(rgb_ptr + index + 0, rgbs.r);
            //            //Unsafe.Write(rgb_ptr + index + 1, rgbs.g);
            //            //Unsafe.Write(rgb_ptr + index + 2, rgbs.b);
            //            //index = index + 3;

            //            //(Unsafe.Read<byte>(yuv_ptr + i), Unsafe.Read<byte>(yuv_ptr + i + u_index), Unsafe.Read<byte>(yuv_ptr + i + v_index)).ToRGB(out var r, out var g, out var b);
            //            //Unsafe.Write(rgb_ptr + index + 0, r);
            //            //Unsafe.Write(rgb_ptr + index + 1, g);
            //            //Unsafe.Write(rgb_ptr + index + 2, b);
            //            //index = index + 3;

            //            YUVEx.ToRGB(Unsafe.Read<byte>(yuv_ptr + i), Unsafe.Read<byte>(yuv_ptr + i + u_index), Unsafe.Read<byte>(yuv_ptr + i + v_index),out var r, out var g, out var b);
            //            Unsafe.Write(rgb_ptr + index + 0, r);
            //            Unsafe.Write(rgb_ptr + index + 1, g);
            //            Unsafe.Write(rgb_ptr + index + 2, b);
            //            index = index + 3;
            //        }
            //    }

            //}

            //Vector<int>
            float[] ffs = new float[this.Raw.Length];
            Array.Copy(this.Raw, ffs, this.Raw.Length);
            var vector_count = Vector<float>.Count;
            var a_1_164 = new Vector<float>((float)1.164);
            for (int i=0; i<u_index; i=i+vector_count)
            {
                var ys = new Vector<float>(ffs, i);
                var us = new Vector<float>(ffs, u_index+i);
                var vs = new Vector<float>(ffs, v_index+i);
                
            }

            unsafe
            {
                fixed (byte* rgb_ptr = rgb)
                fixed (byte* yuv_ptr = this.Raw)
                {
                    for(int i=0;i < u_index;i++)
                    {
                        ToRGB(Unsafe.Read<byte>(yuv_ptr + i), Unsafe.Read<byte>(yuv_ptr + i + u_index), Unsafe.Read<byte>(yuv_ptr + i + v_index), out var r, out var g, out var b);
                        Unsafe.Write(rgb_ptr + index + 0, r);
                        Unsafe.Write(rgb_ptr + index + 1, g);
                        Unsafe.Write(rgb_ptr + index + 2, b);
                        index = index + 3;
                    }
                }
            }
            return rgb;
        }

       

        public void ToRGB(byte y, byte u, byte v, out byte r, out byte g, out byte b)
        {
            r = g = b = 0;
            //var B = 1.164 * (src.y - 16) + 2.018 * (src.u - 128);

            //var G = 1.164 * (src.y - 16) - 0.813 * (src.v - 128) - 0.391 * (src.u - 128);

            //var R = 1.164 * (src.y - 16) + 1.596 * (src.v - 128);



            double Y = y;
            double V = v;
            double U = u;
            Y -= 16;
            U -= 128;
            V -= 128;

            var R = 1.164 * Y + 1.596 * V;
            var G = 1.164 * Y - 0.392 * U - 0.813 * V;
            var B = 1.164 * Y + 2.017 * U;
            if (R > 255.0)
            {
                R = 255;
            }
            else if (R < 0)
            {
                R = 0;
            }
            if (G > 255.0)
            {
                G = 255;
            }
            else if (G < 0)
            {
                G = 0;
            }
            if (B > 255.0)
            {
                B = 255;
            }
            else if (B < 0)
            {
                B = 0;
            }
            r = (byte)R;
            g = (byte)G;
            b = (byte)B;

            //var r = (byte)(R > 255 ? 255 : R);
            //var g = (byte)(G > 255 ? 255 : G);
            //var b = (byte)(B > 255 ? 255 : B);
        }
    }
}

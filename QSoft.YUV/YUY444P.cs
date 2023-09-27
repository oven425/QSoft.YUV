using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        void RGB1(byte y, byte u, byte v, ref byte r, ref byte g, ref byte b)
        {
            r = 0;
            b = 0; g = 0;
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

            unsafe
            {
                fixed (byte* rgb_ptr = rgb)
                fixed (byte* yuv_ptr = this.Raw)
                {
                    for (int i = 0; i < u_index; i++)
                    {

                        //RGB(yuv_ptr[i], yuv_ptr[i + u_index], yuv_ptr[i + v_index], out var r, out var g, out var b);
                        //rgb_ptr[index + 0] = r;
                        //rgb_ptr[index + 1] = g;
                        //rgb_ptr[index + 2] = b;
                        //index = index + 3;


                        //RGB(Unsafe.Read<byte>(yuv_ptr + i), Unsafe.Read<byte>(yuv_ptr + i + u_index), Unsafe.Read<byte>(yuv_ptr + i + v_index), out var r, out var g, out var b);
                        //Unsafe.Write(yuv_ptr + index + 0, r);
                        //Unsafe.Write(yuv_ptr + index + 1, g);
                        //Unsafe.Write(yuv_ptr + index + 2, b);
                        //index = index + 3;

                        //var rgbs = (Unsafe.Read<byte>(yuv_ptr + i), Unsafe.Read<byte>(yuv_ptr + i + u_index), Unsafe.Read<byte>(yuv_ptr + i + v_index)).ToRGB();
                        //Unsafe.Write(rgb_ptr + index + 0, rgbs.r);
                        //Unsafe.Write(rgb_ptr + index + 1, rgbs.g);
                        //Unsafe.Write(rgb_ptr + index + 2, rgbs.b);
                        //index = index + 3;

                        //(Unsafe.Read<byte>(yuv_ptr + i), Unsafe.Read<byte>(yuv_ptr + i + u_index), Unsafe.Read<byte>(yuv_ptr + i + v_index)).ToRGB(out var r, out var g, out var b);
                        //Unsafe.Write(rgb_ptr + index + 0, r);
                        //Unsafe.Write(rgb_ptr + index + 1, g);
                        //Unsafe.Write(rgb_ptr + index + 2, b);
                        //index = index + 3;

                        YUVEx.ToRGB(Unsafe.Read<byte>(yuv_ptr + i), Unsafe.Read<byte>(yuv_ptr + i + u_index), Unsafe.Read<byte>(yuv_ptr + i + v_index),out var r, out var g, out var b);
                        Unsafe.Write(rgb_ptr + index + 0, r);
                        Unsafe.Write(rgb_ptr + index + 1, g);
                        Unsafe.Write(rgb_ptr + index + 2, b);
                        index = index + 3;
                    }
                }

            }

            return rgb;
        }
    }
}

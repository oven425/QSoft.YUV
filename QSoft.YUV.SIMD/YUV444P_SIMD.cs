using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;


//Gather index access
namespace QSoft.YUV.SIMD
{
    public class YUV444P_SIMD: YUV
    {
        public YUV444P_SIMD(byte[] raw, int width, int height) 
            : base(raw, width, height)
        {
            //var v1 = new Vector<float>(0.1f, 0.2f, 0.1f);
            //var v2 = new Vector<float>(1.1f, 2.2f);
            //var vResult1 = Vector.Dot(v1, v2);
        }

        public override IEnumerable<byte> Y => throw new NotImplementedException();

        public override IEnumerable<byte> U => throw new NotImplementedException();

        public override IEnumerable<byte> V => throw new NotImplementedException();
        override public byte[] ToRGB()
        {          
            if (Vector<float>.IsSupported == false)
            {
                throw new NotSupportedException();
            }
            int index = 0;
            int y_index = 0;
            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height * 2;
            var r_buf = new byte[Width * Height];
            var g_buf = new byte[Width * Height];
            var b_buf = new byte[Width * Height];
            var rgb = new byte[Width * Height * 3];

            var size = Vector<float>.Count;
            var vector_1164 = new Vector<float>((float)1.164);
            var vector_128 = new Vector<float>((float)128);
            var vector_16 = new Vector<float>((float)16);
            var vector_2018 = new Vector<float>((float)2.018);
            var vector_1596 = new Vector<float>((float)1.596);
            var vector_0813 = new Vector<float>((float)0.813);
            var vector_0319 = new Vector<float>((float)0.391);
            var vector_255 = new Vector<float>(255);
            var vector_0 = new Vector<float>(0);
            for (int i = 0; i < u_index; i = i + size)
            {
                Vector256<float> v1 = new();

                //Vector256.Shuffle(
                var y1 = new Vector<float>(Raw, i) - vector_16;
                var y = y1 * vector_1164;
                var u = new Vector<float>(Raw, i + u_index) - vector_128;
                var v = new Vector<float>(Raw, i + v_index) - vector_128;
                var bs = y + vector_2018 * u;
                var gs = y - vector_0813 * v - vector_0319 * u;
                var rs = y + vector_1596 * v;
                var bs_min = Vector.LessThan(bs, vector_0);
                var gs_min = Vector.LessThan(gs, vector_0);
                var rs_min = Vector.LessThan(rs, vector_0);
                var bs_max = Vector.LessThan(bs, vector_255);
                var gs_max = Vector.LessThan(gs, vector_255);
                var rs_max = Vector.LessThan(rs, vector_255);

                for (int j = 0; j < size; j++)
                {
                    if (rs_min[j] != 0)
                    {
                        rgb[index + 0] = 0;
                    }
                    else if (rs_max[j] != -1)
                    {
                        rgb[index + 0] = 255;
                    }
                    else
                    {
                        rgb[index + 0] = (byte)rs[j];
                    }

                    if (gs_min[j] != 0)
                    {
                        rgb[index + 1] = 0;
                    }
                    else if (gs_max[j] != -1)
                    {
                        rgb[index + 1] = 255;
                    }
                    else
                    {
                        rgb[index + 1] = (byte)gs[j];
                    }

                    if (bs_min[j] != 0)
                    {
                        rgb[index + 2] = 0;
                    }
                    else if (bs_max[j] != -1)
                    {
                        rgb[index + 2] = 255;
                    }
                    else
                    {
                        rgb[index + 2] = (byte)bs[j];
                    }

                    index = index + 3;
                }

            }
            return rgb;
        }

        public byte[] ToRGB_()
        {
            if (Vector<float>.IsSupported == false)
            {
                throw new NotSupportedException();
            }
            int index = 0;
            int y_index = 0;
            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height * 2;
            var r_buf = new byte[Width * Height];
            var g_buf = new byte[Width * Height];
            var b_buf = new byte[Width * Height];
            var rgb = new byte[Width * Height * 3];

            var size = Vector<float>.Count;
            var vector_1164 = new Vector<float>((float)1.164);
            var vector_128 = new Vector<float>((float)128);
            var vector_16 = new Vector<float>((float)16);
            var vector_2018 = new Vector<float>((float)2.018);
            var vector_1596 = new Vector<float>((float)1.596);
            var vector_0813 = new Vector<float>((float)0.813);
            var vector_0319 = new Vector<float>((float)0.391);
            var vector_255 = new Vector<float>(255);
            var vector_0 = new Vector<float>(0);
            for (int i = 0; i < u_index; i = i + size)
            {
                var y1 = new Vector<float>(Raw, i) - vector_16;
                var y = y1 * vector_1164;
                var u = new Vector<float>(Raw, i + u_index) - vector_128;
                var v = new Vector<float>(Raw, i + v_index) - vector_128;
                var bs = y + vector_2018 * u;
                var gs = y - vector_0813 * v - vector_0319 * u;
                var rs = y + vector_1596 * v;
                var bs_min = Vector.LessThan(bs, vector_0);
                var gs_min = Vector.LessThan(gs, vector_0);
                var rs_min = Vector.LessThan(rs, vector_0);
                var bs_max = Vector.LessThan(bs, vector_255);
                var gs_max = Vector.LessThan(gs, vector_255);
                var rs_max = Vector.LessThan(rs, vector_255);

                for (int j = 0; j < size; j++)
                {
                    if (rs_min[j] != 0)
                    {
                        rgb[index + 0] = 0;
                    }
                    else if (rs_max[j] != -1)
                    {
                        rgb[index + 0] = 255;
                    }
                    else
                    {
                        rgb[index + 0] = 0;
                    }

                    if (gs_min[j] != 0)
                    {
                        rgb[index + 1] = 0;
                    }
                    else if (gs_max[j] != -1)
                    {
                        rgb[index + 1] = 255;
                    }
                    else
                    {
                        rgb[index + 1] = 0;
                    }

                    if (bs_min[j] != 0)
                    {
                        rgb[index + 2] = 0;
                    }
                    else if (bs_max[j] != -1)
                    {
                        rgb[index + 2] = 255;
                    }
                    else
                    {
                        rgb[index + 2] = 0;
                    }

                    index = index + 3;
                }

            }
            return rgb;
        }

        public byte[] ToRGB_1()
        {
            
            if (Vector<float>.IsSupported == false)
            {
                throw new NotSupportedException();
            }
            int index = 0;
            int y_index = 0;
            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height * 2;
            var r_buf = new byte[Width*Height];
            var g_buf = new byte[Width * Height];
            var b_buf = new byte[Width * Height];
            var rgb = new byte[Width * Height * 3];
            
            var size = Vector<float>.Count;
            var vector_1164 = new Vector<float>((float)1.164);
            var vector_128 = new Vector<float>((float)128);
            var vector_16 = new Vector<float>((float)16);
            var vector_2018 = new Vector<float>((float)2.018);
            var vector_1596 = new Vector<float>((float)1.596);
            var vector_0813 = new Vector<float>((float)0.813);
            var vector_0319 = new Vector<float>((float)0.391);
            var vector_255 = new Vector<float>(255);
            var vector_0 = new Vector<float>(0);

            
            for (int i=0;i< u_index; i=i+size)
            {
                var y1 = new Vector<float>(Raw, i) - vector_16;
                var y = y1 * vector_1164;
                var u = new Vector<float>(Raw, i + u_index)-vector_128;
                var v = new Vector<float>(Raw, i + v_index) - vector_128;
                var bs = y + vector_2018 * u;
                var gs = y - vector_0813 * v-vector_0319*u;
                var rs = y + vector_1596 * v;
                var bs_min = Vector.LessThan(bs, vector_0);
                var gs_min = Vector.LessThan(gs, vector_0);
                var rs_min = Vector.LessThan(rs, vector_0);
                var bs_max = Vector.LessThan(bs, vector_255);
                var gs_max = Vector.LessThan(gs, vector_255);
                var rs_max = Vector.LessThan(rs, vector_255);
                for (int j = 0; j < size; j++)
                {
                    if (rs_min[j] != 0)
                    {
                        rgb.AsSpan(index + 0, 1)[0] = 0;
                    }
                    else if (rs_max[j] != -1)
                    {
                        rgb.AsSpan(index + 0, 1)[0] = 255;
                    }
                    else
                    {
                        rgb.AsSpan(index + 0, 1)[0] =(byte)rs[j];
                    }

                    if (gs_min[j] != 0)
                    {
                        rgb.AsSpan(index + 1, 1)[0] = 0;
                    }
                    else if (gs_max[j] != -1)
                    {
                        rgb.AsSpan(index + 1, 1)[0] = 255;
                    }
                    else
                    {
                        rgb.AsSpan(index + 1, 1)[0] = (byte)gs[j];
                    }

                    if (bs_min[j] != 0)
                    {
                        rgb.AsSpan(index + 2, 1)[0] = 0;
                    }
                    else if (bs_max[j] != -1)
                    {
                        rgb.AsSpan(index + 2, 1)[0] = 255;
                    }
                    else
                    {
                        rgb.AsSpan(index + 1, 1)[0] = (byte)bs[j];
                    }
                    
                    index = index + 3;
                }

            }
            return rgb;
        }

        public byte[] ToRGB_2()
        {

            if (Vector<float>.IsSupported == false)
            {
                throw new NotSupportedException();
            }
            int index = 0;
            int y_index = 0;
            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height * 2;

            var rgb = new byte[Width * Height * 3];

            var size = Vector<float>.Count;
            var vector_1164 = new Vector<float>((float)1.164);
            var vector_128 = new Vector<float>((float)128);
            var vector_16 = new Vector<float>((float)16);
            var vector_2018 = new Vector<float>((float)2.018);
            var vector_1596 = new Vector<float>((float)1.596);
            var vector_0813 = new Vector<float>((float)0.813);
            var vector_0319 = new Vector<float>((float)0.391);
            var vector_255 = new Vector<float>(255);
            var vector_0 = new Vector<float>(0);

            var rgb_temp = new byte[size * 3];
            var rgb_span = rgb_temp.AsSpan();
            int offset = 0;
            for (int i = 0; i < u_index; i = i + size)
            {
                var y1 = new Vector<float>(Raw, i) - vector_16;
                var y = y1 * vector_1164;
                var u = new Vector<float>(Raw, i + u_index) - vector_128;
                var v = new Vector<float>(Raw, i + v_index) - vector_128;
                var bs = y + vector_2018 * u;
                var gs = y - vector_0813 * v - vector_0319 * u;
                var rs = y + vector_1596 * v;
                var bs_min = Vector.LessThan(bs, vector_0);
                var gs_min = Vector.LessThan(gs, vector_0);
                var rs_min = Vector.LessThan(rs, vector_0);
                var bs_max = Vector.LessThan(bs, vector_255);
                var gs_max = Vector.LessThan(gs, vector_255);
                var rs_max = Vector.LessThan(rs, vector_255);
                index = 0;
                for (int j = 0; j < size; j++)
                {
                    if (rs_min[j] != 0)
                    {
                        rgb_span[index + 0] = 0;
                    }
                    else if (rs_max[j] != -1)
                    {
                        rgb_span[index + 0] = 255;
                    }
                    else
                    {
                        rgb_span[index + 0] = (byte)rs[j];
                    }

                    if (gs_min[j] != 0)
                    {
                        rgb_span[index + 1] = 0;
                    }
                    else if (gs_max[j] != -1)
                    {
                        rgb_span[index + 1] = 255;
                    }
                    else
                    {
                        rgb_span[index + 1] = (byte)gs[j];
                    }

                    if (bs_min[j] != 0)
                    {
                        rgb_span[index + 2] = 0;
                    }
                    else if (bs_max[j] != -1)
                    {
                        rgb_span[index + 2] = 255;
                    }
                    else
                    {
                        rgb_span[index + 2] = (byte)bs[j];
                    }

                    index = index + 3;
                }
                Array.Copy(rgb_temp, 0, rgb, offset, rgb_temp.Length);
                offset = offset + index;

            }
            return rgb;
        }

        public byte[] ToRGB_3()
        {

            if (Vector<float>.IsSupported == false)
            {
                throw new NotSupportedException();
            }
            int index = 0;
            int y_index = 0;
            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height * 2;

            var rgb = new byte[Width * Height * 3];
            var rgb_span = rgb.AsSpan();
            var size = Vector<float>.Count;
            var vector_1164 = new Vector<float>((float)1.164);
            var vector_128 = new Vector<float>((float)128);
            var vector_16 = new Vector<float>((float)16);
            var vector_2018 = new Vector<float>((float)2.018);
            var vector_1596 = new Vector<float>((float)1.596);
            var vector_0813 = new Vector<float>((float)0.813);
            var vector_0319 = new Vector<float>((float)0.391);
            var vector_255 = new Vector<float>(255);
            var vector_0 = new Vector<float>(0);

            var rgb_temp = new byte[size * 3];
            unsafe
            {
                fixed (byte* spanPtr = rgb_span)
                {
                    for (int i = 0; i < u_index; i = i + size)
                    {
                        var y1 = new Vector<float>(Raw, i) - vector_16;
                        var y = y1 * vector_1164;
                        var u = new Vector<float>(Raw, i + u_index) - vector_128;
                        var v = new Vector<float>(Raw, i + v_index) - vector_128;
                        var bs = y + vector_2018 * u;
                        var gs = y - vector_0813 * v - vector_0319 * u;
                        var rs = y + vector_1596 * v;
                        var bs_min = Vector.LessThan(bs, vector_0);
                        var gs_min = Vector.LessThan(gs, vector_0);
                        var rs_min = Vector.LessThan(rs, vector_0);
                        var bs_max = Vector.LessThan(bs, vector_255);
                        var gs_max = Vector.LessThan(gs, vector_255);
                        var rs_max = Vector.LessThan(rs, vector_255);
                        for (int j = 0; j < size; j++)
                        {
                            if (rs_min[j] != 0)
                            {
                                *(spanPtr + index + 0) = 0;
                            }
                            else if (rs_max[j] != -1)
                            {
                                *(spanPtr + index + 0) = 255;
                            }
                            else
                            {
                                *(spanPtr + index + 0) = (byte)rs[j];
                            }

                            if (gs_min[j] != 0)
                            {
                                *(spanPtr + index + 1) = 0;
                            }
                            else if (gs_max[j] != -1)
                            {
                                *(spanPtr + index + 1) = 255;
                            }
                            else
                            {
                                *(spanPtr + index + 1) = (byte)gs[j];
                            }

                            if (bs_min[j] != 0)
                            {
                                *(spanPtr + index + 2) = 0;
                            }
                            else if (bs_max[j] != -1)
                            {
                                *(spanPtr + index + 2) = 255;
                            }
                            else
                            {
                                *(spanPtr + index + 2) = (byte)bs[j];
                            }

                            index = index + 3;
                        }
                    }
                }


            }
            return rgb;
        }

        public byte[] ToRGB_4()
        {

            if (Vector<float>.IsSupported == false)
            {
                throw new NotSupportedException();
            }
            int index = 0;
            int y_index = 0;
            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height * 2;

            var rgb = new byte[Width * Height * 3];

            var size = Vector<float>.Count;
            var vector_1164 = new Vector<float>((float)1.164);
            var vector_128 = new Vector<float>((float)128);
            var vector_16 = new Vector<float>((float)16);
            var vector_2018 = new Vector<float>((float)2.018);
            var vector_1596 = new Vector<float>((float)1.596);
            var vector_0813 = new Vector<float>((float)0.813);
            var vector_0319 = new Vector<float>((float)0.391);
            var vector_255 = new Vector<float>(255);
            var vector_0 = new Vector<float>(0);

            
            for (int i = 0; i < u_index; i = i + size)
            {
                var y1 = new Vector<float>(Raw, i) - vector_16;
                var y = y1 * vector_1164;
                var u = new Vector<float>(Raw, i + u_index) - vector_128;
                var v = new Vector<float>(Raw, i + v_index) - vector_128;
                var bs = y + vector_2018 * u;
                var gs = y - vector_0813 * v - vector_0319 * u;
                var rs = y + vector_1596 * v;
                var bs_min = Vector.LessThan(bs, vector_0);
                var gs_min = Vector.LessThan(gs, vector_0);
                var rs_min = Vector.LessThan(rs, vector_0);
                var bs_max = Vector.LessThan(bs, vector_255);
                var gs_max = Vector.LessThan(gs, vector_255);
                var rs_max = Vector.LessThan(rs, vector_255);
                for (int j = 0; j < size; j++)
                {
                    var rgbspan = rgb.AsSpan(index, 3);
                    
                    if (rs_min[j] != 0)
                    {
                        rgbspan[0] = 0;
                    }
                    else if (rs_max[j] != -1)
                    {
                        rgbspan[0] = 255;
                    }
                    else
                    {
                        rgbspan[0] = (byte)rs[j];
                    }

                    if (gs_min[j] != 0)
                    {
                        rgbspan[1] = 0;
                    }
                    else if (gs_max[j] != -1)
                    {
                        rgbspan[1] = 255;
                    }
                    else
                    {
                        rgbspan[1] = (byte)gs[j];
                    }

                    if (bs_min[j] != 0)
                    {
                        rgbspan[2] = 0;
                    }
                    else if (bs_max[j] != -1)
                    {
                        rgbspan[2] = 255;
                    }
                    else
                    {
                        rgbspan[2] = (byte)bs[j];
                    }

                    index = index + 3;
                }
            }
            return rgb;
        }

    }
}

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
            if (!Vector<float>.IsSupported)
            {
                throw new NotSupportedException();
            }
            byte [] raw = new byte[64];
            Vector128<byte> r = Vector128.Create((byte)1, (byte)2, (byte)3, (byte)4, (byte)5, (byte)6, (byte)7, (byte)8, (byte)9, (byte)10, (byte)11, (byte)12, (byte)13, (byte)14, (byte)15, (byte)16);
            Vector128<byte> g = Vector128.Create((byte)21, (byte)22, (byte)23, (byte)24, (byte)25, (byte)26, (byte)27, (byte)28, (byte)29, (byte)30, (byte)31, (byte)32, (byte)33, (byte)34, (byte)35, (byte)36);
            Vector128<byte> b = Vector128.Create((byte)41, (byte)42, (byte)43, (byte)44, (byte)45, (byte)46, (byte)47, (byte)48, (byte)49, (byte)50, (byte)51, (byte)52, (byte)53, (byte)54, (byte)55, (byte)56);
            Vector128<byte> a = Vector128.Create((byte)61, (byte)62, (byte)63, (byte)64, (byte)65, (byte)66, (byte)67, (byte)68, (byte)69, (byte)70, (byte)71, (byte)72, (byte)73, (byte)74, (byte)75, (byte)76);
            var rg_low = Sse2.UnpackLow(r, g);   // 前 8 個 RG
            var rg_high = Sse2.UnpackHigh(r, g); // 後 8 個 RG

            var ba_low = Sse2.UnpackLow(b, a);   // 前 8 個 BA
            var ba_high = Sse2.UnpackHigh(b, a); // 後 8 個 BA

            var res0 = Sse2.UnpackLow(rg_low.AsUInt16(), ba_low.AsUInt16()).AsByte();
            var res1 = Sse2.UnpackHigh(rg_low.AsUInt16(), ba_low.AsUInt16()).AsByte();

            // 處理後 8 組像素 (8-15)
            // 分解成 8-11 (res2) 和 12-15 (res3)
            var res2 = Sse2.UnpackLow(rg_high.AsUInt16(), ba_high.AsUInt16()).AsByte();
            var res3 = Sse2.UnpackHigh(rg_high.AsUInt16(), ba_high.AsUInt16()).AsByte();
            unsafe
            {
                fixed (byte* ptr = raw)
                {
                    // 依序寫入 64 個 byte
                    Sse2.Store(ptr, res0);       // 寫入 bytes 0-15
                    Sse2.Store(ptr + 16, res1);  // 寫入 bytes 16-31
                    Sse2.Store(ptr + 32, res2);  // 寫入 bytes 32-47
                    Sse2.Store(ptr + 48, res3);  // 寫入 bytes 48-63
                }
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

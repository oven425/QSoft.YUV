using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;


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

        public byte[] ToBGRA_Vector128()
        {
            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height * 2;

            var rgb = new byte[Width * Height * 4];

            nuint rgbindex = 0;
            Vector128<byte> A = Vector128.Create((byte)255);
            ReadOnlySpan<byte> src = new(RawByte);
            ref byte dst = ref MemoryMarshal.GetReference(rgb.AsSpan());
                
            for (int i = 0; i < u_index; i = i + 16)
            {
                var y_vec_byte = Vector128.Create(src[i..]);
                var u_vec_byte = Vector128.Create(src[(i + u_index)..]);
                var v_vec_byte = Vector128.Create(src[(i + v_index)..]);

                var (y_vec_low_short, y_vec_high_short) = Vector128.Widen(y_vec_byte);
                var (u_vec_low_short, u_vec_high_short) = Vector128.Widen(u_vec_byte);
                var (v_vec_low_short, v_vec_high_short) = Vector128.Widen(v_vec_byte);

                var r_low_short = yuv_R(y_vec_low_short.AsInt16(), v_vec_low_short.AsInt16());
                var r_high_short = yuv_R(y_vec_high_short.AsInt16(), v_vec_high_short.AsInt16());
                var g_low_short = yuv_G(y_vec_low_short.AsInt16(), u_vec_low_short.AsInt16(), v_vec_low_short.AsInt16());
                var g_high_short = yuv_G(y_vec_high_short.AsInt16(), u_vec_high_short.AsInt16(), v_vec_high_short.AsInt16());
                var b_low_short = yuv_B(y_vec_low_short.AsInt16(), u_vec_low_short.AsInt16());
                var b_high_short = yuv_B(y_vec_high_short.AsInt16(), u_vec_high_short.AsInt16());

                Vector128<byte> R = Vector128.Narrow(r_low_short, r_high_short).AsByte();
                Vector128<byte> G = Vector128.Narrow(g_low_short, g_high_short).AsByte();
                Vector128<byte> B = Vector128.Narrow(b_low_short, b_high_short).AsByte();
                
                Vector128<byte> rg_lo = Sse2.UnpackLow(B, G);
                Vector128<byte> ba_lo = Sse2.UnpackLow(R, A);

                Vector128<byte> rg_hi = Sse2.UnpackHigh(B, G);
                Vector128<byte> ba_hi = Sse2.UnpackHigh(R, A);

                Vector128<byte> rgba_0_3 = Sse2.UnpackLow(rg_lo.AsUInt16(), ba_lo.AsUInt16()).AsByte();
                Vector128<byte> rgba_4_7 = Sse2.UnpackHigh(rg_lo.AsUInt16(), ba_lo.AsUInt16()).AsByte();
                Vector128<byte> rgba_8_11 = Sse2.UnpackLow(rg_hi.AsUInt16(), ba_hi.AsUInt16()).AsByte();
                Vector128<byte> rgba_15_15 = Sse2.UnpackHigh(rg_hi.AsUInt16(), ba_hi.AsUInt16()).AsByte();

                Vector128.StoreUnsafe(rgba_0_3, ref dst, rgbindex);
                Vector128.StoreUnsafe(rgba_4_7, ref dst, rgbindex+16);
                Vector128.StoreUnsafe(rgba_8_11, ref dst, rgbindex+32);
                Vector128.StoreUnsafe(rgba_15_15, ref dst, rgbindex+48);
                rgbindex = rgbindex + 64;
                
            }

            return rgb;
        }


        public byte[] ToBGRA_Sse()
        {


            int u_index = this.Width * this.Height;
            int v_index = this.Width * this.Height * 2;

            var rgb = new byte[Width * Height * 4];

            var rgbindex = 0;
            unsafe 
            {
                fixed (byte* dst = &rgb[0])
                fixed (byte* src = &RawByte[0])
                {
                    for(int i=0; i< u_index; i=i+16)
                    {
                        var y_vec_byte = Sse2.LoadVector128(src + i);
                        var u_vec_byte = Sse2.LoadVector128(src + i+u_index);
                        var v_vec_byte = Sse2.LoadVector128(src + i+v_index);
                        Vector128<byte> zero = Vector128<byte>.Zero;
                        Vector128<short> y_vec_low_short = Sse2.UnpackLow(y_vec_byte, zero).AsInt16();
                        Vector128<short> y_vec_high_short = Sse2.UnpackHigh(y_vec_byte, zero).AsInt16();
                        Vector128<short> u_vec_low_short = Sse2.UnpackLow(u_vec_byte, zero).AsInt16();
                        Vector128<short> u_vec_high_short = Sse2.UnpackHigh(u_vec_byte, zero).AsInt16();
                        Vector128<short> v_vec_low_short = Sse2.UnpackLow(v_vec_byte, zero).AsInt16();
                        Vector128<short> v_vec_high_short = Sse2.UnpackHigh(v_vec_byte, zero).AsInt16();
                        var r_low_short = yuv_R(y_vec_low_short, v_vec_low_short);
                        var r_high_short = yuv_R(y_vec_high_short, v_vec_high_short);
                        var g_low_short = yuv_G(y_vec_low_short, u_vec_low_short, v_vec_low_short);
                        var g_high_short = yuv_G(y_vec_high_short, u_vec_high_short, v_vec_high_short);
                        var b_low_short = yuv_B(y_vec_low_short, u_vec_low_short);
                        var b_high_short = yuv_B(y_vec_high_short, u_vec_high_short);

                        Vector128<byte> R = Sse2.PackUnsignedSaturate(r_low_short, r_high_short);
                        Vector128<byte> G = Sse2.PackUnsignedSaturate(g_low_short, g_high_short);
                        Vector128<byte> B = Sse2.PackUnsignedSaturate(b_low_short, b_high_short);

                        Vector128<byte> A = Vector128.Create((byte)255);

                        Vector128<byte> rg_lo = Sse2.UnpackLow(B, G);
                        Vector128<byte> ba_lo = Sse2.UnpackLow(R, A);

                        Vector128<byte> rg_hi = Sse2.UnpackHigh(B, G);
                        Vector128<byte> ba_hi = Sse2.UnpackHigh(R, A);

                        Vector128<byte> rgba_0_3 = Sse2.UnpackLow(rg_lo.AsUInt16(), ba_lo.AsUInt16()).AsByte();
                        Vector128<byte> rgba_4_7 = Sse2.UnpackHigh(rg_lo.AsUInt16(), ba_lo.AsUInt16()).AsByte();
                        Vector128<byte> rgba_8_11 = Sse2.UnpackLow(rg_hi.AsUInt16(), ba_hi.AsUInt16()).AsByte();
                        Vector128<byte> rgba_15_15 = Sse2.UnpackHigh(rg_hi.AsUInt16(), ba_hi.AsUInt16()).AsByte();

                        Sse2.Store(dst + rgbindex, rgba_0_3);
                        Sse2.Store(dst + 16 + rgbindex, rgba_4_7);
                        Sse2.Store(dst + 32 + rgbindex, rgba_8_11);
                        Sse2.Store(dst + 48 + rgbindex, rgba_15_15);
                        rgbindex = rgbindex + 64;
                    }
                    

                }
            }

            return rgb;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector128<short> yuv_R(Vector128<short> y, Vector128<short> v)
        {
            var v128 = Vector128.Create((short)128);
            var coeffR = Vector128.Create((short)179); // 359 / 2

            // v_diff = V - 128
            Vector128<short> v_diff = Sse2.Subtract(v, v128);

            // temp = (v_diff * 179) >> 7
            Vector128<short> prod = Sse2.MultiplyLow(v_diff, coeffR);
            Vector128<short> expr = Sse2.ShiftRightArithmetic(prod, 7);

            return Vector128.Add(y, expr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector128<short> yuv_G(Vector128<short> y, Vector128<short> u, Vector128<short> v)
        {
            var v128 = Vector128.Create((short)128);
            var coeffG_U = Vector128.Create((short)44); // 88 / 2
            var coeffG_V = Vector128.Create((short)91); // 183 / 2 (約略值)

            Vector128<short> u_diff = Sse2.Subtract(u, v128);
            Vector128<short> v_diff = Sse2.Subtract(v, v128);

            // (44 * u_diff + 91 * v_diff) >> 7
            Vector128<short> prodU = Sse2.MultiplyLow(u_diff, coeffG_U);
            Vector128<short> prodV = Sse2.MultiplyLow(v_diff, coeffG_V);
            Vector128<short> combined = Sse2.Add(prodU, prodV);

            Vector128<short> expr = Sse2.ShiftRightArithmetic(combined, 7);

            return Vector128.Subtract(y, expr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector128<short> yuv_B(Vector128<short> y, Vector128<short> u)
        {
            var v128 = Vector128.Create((short)128);
            var coeffB = Vector128.Create((short)227); // 454 / 2

            Vector128<short> u_diff = Sse2.Subtract(u, v128);

            // temp = (u_diff * 227) >> 7
            Vector128<short> prod = Sse2.MultiplyLow(u_diff, coeffB);
            Vector128<short> expr = Sse2.ShiftRightArithmetic(prod, 7);

            return Vector128.Add(y, expr);
        }

        public byte[] ToRGB_()
        {
            if (!Vector<float>.IsSupported)
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

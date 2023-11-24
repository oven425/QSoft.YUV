using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.YUV.SIMD
{
    public class YUV444P: YUV
    {
        public YUV444P(byte[] raw, int width, int height) 
            : base(raw, width, height)
        {
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
            unsafe
            {
                fixed (byte* prgb = rgb)
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
                                //Unsafe.Write(prgb + index + 0, 0);
                            }
                            else if (rs_max[j] != -1)
                            {
                                //Unsafe.Write(prgb + index + 0, 255);
                            }
                            else
                            {
                                *(prgb + index + 0) = (byte)rs[j];
                                Unsafe.Write(prgb + index + 0, rs[j]);
                            }

                            if (gs_min[j] != 0)
                            {
                                //span[index + 1] = 0;
                                //rgb[index + 1] = 0;
                            }
                            else if (gs_max[j] != -1)
                            {
                                //span[index + 1] = 255;
                                //rgb[index + 1] = 255;
                            }
                            else
                            {
                                *(prgb + index + 1) = (byte)gs[j];
                            }

                            if (bs_min[j] != 0)
                            {
                                //span[index + 2] = 0;
                                //rgb[index + 2] = 0;
                            }
                            else if (bs_max[j] != -1)
                            {
                                //span[index + 2] = 255;
                                //rgb[index + 2] = 255;
                            }
                            else
                            {
                                *(prgb + index + 2) = (byte)bs[j];
                            }

                            index = index + 3;
                        }

                        //for (int j = 0; j < size; j++)
                        //{
                        //    if (rs_min[j] != 0)
                        //    {
                        //        //span[index + 0] = 0;
                        //        rgb[index + 0] = 0;
                        //    }
                        //    else if (rs_max[j] != -1)
                        //    {
                        //        //span[index + 0] = 255;
                        //        rgb[index + 0] = 255;
                        //    }
                        //    else
                        //    {
                        //        //span[index + 0] = (byte)rs[j];
                        //        rgb[index + 0] = (byte)rs[j];
                        //    }

                        //    if (gs_min[j] != 0)
                        //    {
                        //        //span[index + 1] = 0;
                        //        rgb[index + 1] = 0;
                        //    }
                        //    else if (gs_max[j] != -1)
                        //    {
                        //        //span[index + 1] = 255;
                        //        rgb[index + 1] = 255;
                        //    }
                        //    else
                        //    {
                        //        //span[index + 1] = (byte)gs[j];
                        //        rgb[index + 1] = (byte)gs[j];
                        //    }

                        //    if (bs_min[j] != 0)
                        //    {
                        //        //span[index + 2] = 0;
                        //        rgb[index + 2] = 0;
                        //    }
                        //    else if (bs_max[j] != -1)
                        //    {
                        //        //span[index + 2] = 255;
                        //        rgb[index + 2] = 255;
                        //    }
                        //    else
                        //    {
                        //        //span[index + 2] = (byte)bs[j];
                        //        rgb[index + 2] = (byte)bs[j];
                        //    }

                        //    index = index + 3;
                        //}


                        //for (int j = 0; j < size; j++)
                        //{

                        //    //var r = rs[j];
                        //    //var g = gs[j];
                        //    //var b = bs[j];
                        //    rgb[index + 0] = Limit(rs[j]);
                        //    rgb[index + 1] = Limit(gs[j]);
                        //    rgb[index + 2] = Limit(bs[j]);

                        //    //rgb[index + 0] = 0;
                        //    //rgb[index + 1] = 0;
                        //    //rgb[index + 2] = 0;
                        //    index = index + 3;
                        //}

                    }
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
                        //span[index + 0] = 0;
                        rgb.AsSpan(index + 0, 1)[0] = 0;
                    }
                    else if (rs_max[j] != -1)
                    {
                        //span[index + 0] = 255;
                        //rgb[index + 0] = 255;
                        rgb.AsSpan(index + 0, 1)[0] = 255;
                    }
                    else
                    {
                        //span[index + 0] = (byte)rs[j];
                        //rgb[index + 0] = (byte)rs[j];
                        rgb.AsSpan(index + 0, 1)[0] =(byte)rs[j];
                    }

                    if (gs_min[j] != 0)
                    {
                        //span[index + 1] = 0;
                        //rgb[index + 1] = 0;
                    }
                    else if (gs_max[j] != -1)
                    {
                        //span[index + 1] = 255;
                        //rgb[index + 1] = 255;
                    }
                    else
                    {
                        //span[index + 1] = (byte)gs[j];
                        //rgb[index + 1] = (byte)gs[j];
                    }

                    if (bs_min[j] != 0)
                    {
                        //span[index + 2] = 0;
                        //rgb[index + 2] = 0;
                    }
                    else if (bs_max[j] != -1)
                    {
                        //span[index + 2] = 255;
                        //rgb[index + 2] = 255;
                    }
                    else
                    {
                        //span[index + 2] = (byte)bs[j];
                        //rgb[index + 2] = (byte)bs[j];
                    }

                    index = index + 3;
                }

                //for (int j = 0; j < size; j++)
                //{
                //    if (rs_min[j] != 0)
                //    {
                //        //span[index + 0] = 0;
                //        rgb[index + 0] = 0;
                //    }
                //    else if (rs_max[j] != -1)
                //    {
                //        //span[index + 0] = 255;
                //        rgb[index + 0] = 255;
                //    }
                //    else
                //    {
                //        //span[index + 0] = (byte)rs[j];
                //        rgb[index + 0] = (byte)rs[j];
                //    }

                //    if (gs_min[j] != 0)
                //    {
                //        //span[index + 1] = 0;
                //        rgb[index + 1] = 0;
                //    }
                //    else if (gs_max[j] != -1)
                //    {
                //        //span[index + 1] = 255;
                //        rgb[index + 1] = 255;
                //    }
                //    else
                //    {
                //        //span[index + 1] = (byte)gs[j];
                //        rgb[index + 1] = (byte)gs[j];
                //    }

                //    if (bs_min[j] != 0)
                //    {
                //        //span[index + 2] = 0;
                //        rgb[index + 2] = 0;
                //    }
                //    else if (bs_max[j] != -1)
                //    {
                //        //span[index + 2] = 255;
                //        rgb[index + 2] = 255;
                //    }
                //    else
                //    {
                //        //span[index + 2] = (byte)bs[j];
                //        rgb[index + 2] = (byte)bs[j];
                //    }

                //    index = index + 3;
                //}


                //for (int j = 0; j < size; j++)
                //{

                //    //var r = rs[j];
                //    //var g = gs[j];
                //    //var b = bs[j];
                //    rgb[index + 0] = Limit(rs[j]);
                //    rgb[index + 1] = Limit(gs[j]);
                //    rgb[index + 2] = Limit(bs[j]);

                //    //rgb[index + 0] = 0;
                //    //rgb[index + 1] = 0;
                //    //rgb[index + 2] = 0;
                //    index = index + 3;
                //}

            }
            return rgb;
        }
        float max = 255;
        float min = 0;
        byte Limit(float src)
        {
            
            return (byte)src;
            //if (src>255.0)
            //{
            //    return 255;
            //}
            //else if(src<0)
            //{
            //    return 0;
            //}
            //return (byte)src;
        }
        //var B = 1.164 * (src.y - 16) + 2.018 * (src.u - 128);
        //var G = 1.164 * (src.y - 16) - 0.813 * (src.v - 128) - 0.391 * (src.u - 128);
        //var R = 1.164 * (src.y - 16) + 1.596 * (src.v - 128);
        void yuv2rgb()
        {

        }
    }
}

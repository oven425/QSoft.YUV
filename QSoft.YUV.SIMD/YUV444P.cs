using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            
            if(Vector<float>.IsSupported == false)
            {
                throw new NotSupportedException();
            }
            var rgb = new byte[Width * Height * 3];
            var size = Vector<float>.Count;
            var vector_1164 = new Vector<float>((float)1.164);
            for(int i=0;i<Raw.Length;i=i+size)
            {

            }


            return rgb;
        }
        //var B = 1.164 * (src.y - 16) + 2.018 * (src.u - 128);
        //var G = 1.164 * (src.y - 16) - 0.813 * (src.v - 128) - 0.391 * (src.u - 128);
        //var R = 1.164 * (src.y - 16) + 1.596 * (src.v - 128);
        void yuv2rgb()
        {

        }
    }
}

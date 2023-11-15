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

            return rgb;
        }
    }
}

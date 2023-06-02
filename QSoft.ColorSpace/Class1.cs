using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.ColorSpace
{
    public class YUY2Raw
    {
        public byte[] Data { get; set; }
        
    }

    public class RgbRaw
    {
        public byte[] Datas { set; get; }
    }


    public enum ColorSpaces
    {
        YUY2,
        RGB24,
        RGB32
    }

    public interface ColorSpaceTransform
    {
        ColorSpaces ColorSpaces { get; }
        byte[] Convert(byte[] data);
    }
    public class ColorSpaceBase
    {
        
    }

    

    
}

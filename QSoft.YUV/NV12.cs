using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.YUV
{
    internal class NV12
    {
        //void NV12ToRGB(byte[] nv12, byte[] rgb, int width, int height)
        //{
        //    int size = width * height;
        //    int w, h, x, y, u, v, yIndex, uvIndex, rIndex, gIndex, bIndex;
        //    int y1192, r, g, b, uv448, uv_128;
        //    for (h = 0; h < height; h++)
        //    {
        //        for (w = 0; w < width; w++)
        //        {
        //            yIndex = h * width + w;
        //            uvIndex = (h / 2) * width + (w & (-2)) + size;
        //            u = nv12[uvIndex];
        //            v = nv12[uvIndex + 1];
        //            // YUV to RGB
        //            y1192 = 1192 * (nv12[yIndex] - 16);
        //            uv448 = 448 * (u - 128);
        //            uv_128 = 128 * (v - 128);
        //            r = (y1192 + uv448) >> 10;
        //            g = (y1192 - uv_128 - uv448) >> 10;
        //            b = (y1192 + uv_128) >> 10;
        //            // RGB clipping
        //            if (r < 0) r = 0;
        //            if (g < 0) g = 0;
        //            if (b < 0) b = 0;
        //            if (r > 255) r = 255;
        //            if (g > 255) g = 255;
        //            if (b > 255) b = 255;
        //            // Save RGB values
        //            rIndex = yIndex * 3;
        //            gIndex = rIndex + 1;
        //            bIndex = gIndex + 1;
        //            rgb[rIndex] = (byte)r;
        //            rgb[gIndex] = (byte)g;
        //            rgb[bIndex] = (byte)b;
        //        }
        //    }
        //}

    }
}

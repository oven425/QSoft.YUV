// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
var summary = BenchmarkRunner.Run<YUVTT>();
Console.WriteLine("Hello, World!");
public class YUVTT
{
    //YUV444P m_444p;
    readonly QSoft.YUV.SIMD.YUV444P_SIMD m_SIMD_444p;
    public YUVTT()
    {
        var p1 = System.AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine(p1);
        var pp = System.IO.Path.GetFullPath("s1-yuv444p.yuv");
        Console.WriteLine(pp);
        Console.ReadLine();
        byte[] yuv444p_raw = File.ReadAllBytes("../../../../s1-yuv444p.yuv");
        //this.m_444p = new YUV444P(yuv444p_raw, 6000, 3376);
        this.m_SIMD_444p = new QSoft.YUV.SIMD.YUV444P_SIMD(yuv444p_raw, 6000, 3376);
    }

    //[Benchmark]
    //public void New()
    //{
    //    this.m_444p.ToRGB();
    //}

    [Benchmark]
    public void Sse2()
    {
        System.Threading.Thread.Sleep(100);
        //this.m_SIMD_444p.ToBGRA_Sse();
    }

    //[Benchmark]
    //public void Old()
    //{
    //    this.m_444p.ToRGB_Old();
    //}
    //[Benchmark]
    //public void SIMD_ToRGB_3()
    //{
    //    this.m_SIMD_444p.ToRGB_3();
    //}
    //[Benchmark]
    //public void SIMD_ToRGB_2()
    //{
    //    this.m_SIMD_444p.ToRGB_2();
    //}
    //[Benchmark]
    //public void SIMD_ToRGB_4()
    //{
    //    this.m_SIMD_444p.ToRGB_4();
    //}
    //[Benchmark]
    //public void SIMD_ToRGB()
    //{
    //    this.m_SIMD_444p.ToRGB();
    //}

    //[Benchmark]
    //public void SIMD_ToRGB_()
    //{
    //    this.m_SIMD_444p.ToRGB_();
    //}

}

// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using QSoft.YUV;
using QSoft.YUV.SIMD;
using System.Numerics;



var m_444p1 = new YUV444P_SIMD(File.ReadAllBytes("../../../../s1-yuv444p.yuv"), 6000, 3376);
m_444p1.ToRGB();
Console.ReadLine();
var summary = BenchmarkRunner.Run<YUVTT>();
//[MemoryDiagnoser]
public class YUVTT
{
    YUV444P m_444p;
    QSoft.YUV.SIMD.YUV444P_SIMD m_SIMD_444p;
    public YUVTT()
    {
        var p1 = System.AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine(p1);
        var pp = System.IO.Path.GetFullPath("s1-yuv444p.yuv");
        Console.WriteLine(pp);
        Console.ReadLine();
        byte[] yuv444p_raw = File.ReadAllBytes("../../../../s1-yuv444p.yuv");
        this.m_444p = new YUV444P(yuv444p_raw, 6000, 3376);
        this.m_SIMD_444p = new QSoft.YUV.SIMD.YUV444P_SIMD(yuv444p_raw, 6000, 3376);
    }

    [Benchmark]
    public void New()
    {
        this.m_444p.ToRGB();
    }

    //[Benchmark]
    //public void Old()
    //{
    //    this.m_444p.ToRGB_Old();
    //}
    [Benchmark]
    public void SIMD_ToRGB_3()
    {
        this.m_SIMD_444p.ToRGB_3();
    }
    [Benchmark]
    public void SIMD_ToRGB_2()
    {
        this.m_SIMD_444p.ToRGB_2();
    }
    [Benchmark]
    public void SIMD_ToRGB_4()
    {
        this.m_SIMD_444p.ToRGB_4();
    }
    [Benchmark]
    public void SIMD_ToRGB()
    {
        this.m_SIMD_444p.ToRGB();
    }

    [Benchmark]
    public void SIMD_ToRGB_()
    {
        this.m_SIMD_444p.ToRGB_();
    }

    //[Benchmark]
    //public void Old_1()
    //{
    //    this.m_444p.ToRGB_Old_1();
    //}

    //[Benchmark]
    //public void Safe_Func()
    //{
    //    this.m_444p.ToRGB_Func();
    //}

    //[Benchmark]
    //public void Safe_Delegate()
    //{
    //    this.m_444p.ToRGB_Delegate();
    //}

    //[Benchmark]
    //public void ToRGB_Unsafe_Func()
    //{
    //    this.m_444p.ToRGB_Unsafe_Func();
    //}

    //[Benchmark]
    //public void ToRGB_Unsafe_Delegate()
    //{
    //    this.m_444p.ToRGB_Unsafe_Delegate();
    //}
}

[MemoryDiagnoser]
public class Counter
{
    private readonly int[] _left;
    private readonly int[] _right;

    public Counter()
    {
        _left = Faker.BuildArray(10000);
        _right = Faker.BuildArray(10000);
    }

    [Benchmark]
    public int[] VectorSum()
    {
        var vectorSize = Vector<int>.Count;
        var result = new Int32[_left.Length];
        for (int i = 0; i < _left.Length; i += vectorSize)
        {
            var v1 = new Vector<int>(_left, i);
            var v2 = new Vector<int>(_right, i);
            (v1 * v2).CopyTo(result, i);
        }
        return result;
    }

    [Benchmark]
    public int[] LinQSum()
    {
        var result = _left.Zip(_right, (l, r) => l * r).ToArray();
        return result;
    }

    [Benchmark]
    public int[] ForSum()
    {
        var result = new Int32[_left.Length];
        for (int i = 0; i <= _left.Length - 1; i++)
        {
            result[i] = _left[i] * _right[i];
        }
        return result;
    }
}

public static class Faker
{
    public static int[] BuildArray(int length)
    {
        var list = new List<int>();
        var rnd = new Random(DateTime.Now.Millisecond);
        for (int i = 1; i <= length; i++)
        {
            list.Add(rnd.Next(1, 99));
        }
        return list.ToArray();
    }
}

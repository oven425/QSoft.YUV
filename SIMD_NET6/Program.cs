// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Numerics;

Console.WriteLine("Hello, World!");
//var vv = new Counter();
//vv.VectorSum();
var summary = BenchmarkRunner.Run<Counter>();
Console.WriteLine("Hello, World!");
Console.ReadLine();

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

using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics.Tensors;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.YUV.Onnx
{
    public class YUV
    {
        public void ProcessImage(float[] yuvData, int width, int height)
        {
            // yuvData: [Y, Y, ..., U, U, ..., V, V, ...] (6000x6000x3)
            var yuvTensor = new ReadOnlySpan<float>(yuvData);
            var rgbOutput = new Span<float>(new float[width*height*3]);

            // 定義轉換矩陣
            ReadOnlySpan<float> matrix = [
        1.0f,  0.0f,    1.402f,
        1.0f, -0.3441f, -0.7141f,
        1.0f,  1.772f,  0.0f
    ];

            // 使用 .NET 8 的 TensorPrimitives 進行矩陣乘法
            // 這會在底層調用當前 CPU 最強大的向量指令集
            TensorPrimitives.Multiply(yuvTensor, matrix, rgbOutput);
        }
        public static void ExportYuvOnnx()
        {
            var mlContext = new MLContext();

            try
            {
                // 1. 定義資料結構 (空數據僅用於定義 Schema)
                var data = mlContext.Data.LoadFromEnumerable(new List<YuvData>());

                // 2. 建立運算鏈結
                // 注意：在 ML.NET 中，我們通常使用「自定義映射 (CustomMapping)」
                // 但為了匯出成純 ONNX 算子，我們使用矩陣相乘轉換
                var pipeline = mlContext.Transforms.CopyColumns("OutputY", "Y")
                    .Append(mlContext.Transforms.Expression("R", "(Y + 1.402 * (V - 128))"))
                    .Append(mlContext.Transforms.Expression("G", "(Y - 0.344136 * (U - 128) - 0.714136 * (V - 128))"))
                    .Append(mlContext.Transforms.Expression("B", "(Y + 1.772 * (U - 128))"));



                // 3. 擬合模型 (這只是為了建立計算圖)
                var model = pipeline.Fit(data);

                // 4. 匯出為 ONNX
                using (var file = File.Create("yuv2rgb_from_csharp.onnx"))
                {
                    // 這裡會將 C# 的表達式轉換為 ONNX 的 Add, Sub, Mul 節點
                    mlContext.Model.ConvertToOnnx(model, data, file);
                }

                Console.WriteLine("C# 已成功生成 ONNX 模型！");
            }
            catch (Exception ex)
            {
                Console.WriteLine("匯出 ONNX 模型失敗: " + ex.Message);
                return;
            }
            
        }

    }

    public class YuvData
    {
        // 假設輸入是三個 float 欄位
        public float Y;
        public float U;
        public float V;
    }

    public class RgbData
    {
        public float R;
        public float G;
        public float B;
    }
}

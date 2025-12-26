//using System.Runtime.CompilerServices;
//using System.Runtime.Intrinsics;
//using System.Runtime.Intrinsics.Arm;
//using System.Runtime.Intrinsics.X86;
//using System.Threading.Tasks;


//namespace QSoft.YUV.SIMD
//{ 
//public unsafe class YuvConverter
//{
//    // 定點數係數 (放大 64 倍，即 >> 6)
//    // R = Y + 1.402*(V-128)  => Coeff: 90
//    // G = Y - 0.344*(U-128) - 0.714*(V-128) => Coeffs: 22, 46
//    // B = Y + 1.772*(U-128) => Coeff: 114 (1.772 * 64 = 113.4, 取 114 補償精度)
//    private static readonly Vector256<short> Coeff_R_V = Vector256.Create((short)90);
//    private static readonly Vector256<short> Coeff_G_U = Vector256.Create((short)22);
//    private static readonly Vector256<short> Coeff_G_V = Vector256.Create((short)46);
//    private static readonly Vector256<short> Coeff_B_U = Vector256.Create((short)114);

//    // 偏移量
//    private static readonly Vector256<short> Vector128 = Vector256.Create((short)128);
//    private static readonly Vector256<byte> Alpha255 = Vector256.Create((byte)255);

//    /// <summary>
//    /// 將 YUV444P 轉換為 Bgra32 (8888)
//    /// </summary>
//    /// <param name="pY">Y 平面指標</param>
//    /// <param name="pU">U 平面指標</param>
//    /// <param name="pV">V 平面指標</param>
//    /// <param name="pDest">目標 RGB 緩衝區指標</param>
//    /// <param name="width">影像寬度</param>
//    /// <param name="height">影像高度</param>
//    /// <param name="strideDest">目標的 Stride (bytes per row)</param>
//    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
//    public static void Yuv444PToBgra32(byte* pY, byte* pU, byte* pV, byte* pDest, int width, int height, int strideDest)
//    {
//        // 使用 Parallel.For 加速，因為 YUV 轉換是 Embarrassingly Parallel
//        // 根據圖片高度進行分割
//        Parallel.For(0, height, y =>
//        {
//            // 計算當前行的指標位置
//            byte* rowY = pY + (y * width);
//            byte* rowU = pU + (y * width);
//            byte* rowV = pV + (y * width);
//            byte* rowDest = pDest + (y * strideDest);

//            int x = 0;

//            // SIMD 迴圈：每次處理 16 個像素 (Vector256<short> 的容量)
//            // 為什麼是 16？因為我們需要將 byte 擴充為 short 來做運算
//            int vectorWidth = Vector256<short>.Count; // 16

//            // 確保有足夠的寬度進行 SIMD 處理
//            for (; x <= width - vectorWidth; x += vectorWidth)
//            {
//                // 1. 載入數據 (Load)
//                // 讀取 16 bytes 的 Y, U, V。這裡使用 Vector128.Load 然後轉型擴充，
//                // 或者直接用 Vector256 讀取並只取下半部，但在 .NET 8 中有更優雅的 Widen 方法。

//                // 載入 16 bytes 到 Vector128，然後 Widen 到 Vector256<short>
//                Vector128<byte> yBytes = Vector128.Load(rowY + x);
//                Vector128<byte> uBytes = Vector128.Load(rowU + x);
//                Vector128<byte> vBytes = Vector128.Load(rowV + x);

//                // 2. 擴充與預處理 (Widen & Subtract 128)
//                // 將 U, V 減去 128 並轉為 short
//                Vector256<short> yShort = Vector256.Widen(yBytes);
//                Vector256<short> uShort = Vector256.Subtract(Vector256.Widen(uBytes), Vector128);
//                Vector256<short> vShort = Vector256.Subtract(Vector256.Widen(vBytes), Vector128);

//                // 3. 計算 RGB (Calculate) - 定點數運算 (Shift 6)

//                // R = Y + (V * 90) >> 6
//                var rCalc = Vector256.Add(yShort, Vector256.ShiftRightArithmetic(Vector256.Multiply(vShort, Coeff_R_V), 6));

//                // G = Y - (U * 22 + V * 46) >> 6
//                var gPartU = Vector256.Multiply(uShort, Coeff_G_U);
//                var gPartV = Vector256.Multiply(vShort, Coeff_G_V);
//                var gCalc = Vector256.Subtract(yShort, Vector256.ShiftRightArithmetic(Vector256.Add(gPartU, gPartV), 6));

//                // B = Y + (U * 114) >> 6
//                var bCalc = Vector256.Add(yShort, Vector256.ShiftRightArithmetic(Vector256.Multiply(uShort, Coeff_B_U), 6));

//                // 4. 壓縮回 Byte (Pack & Saturate)
//                // PackUnsignedSaturate 會自動處理 Clamp (0-255)
//                // 由於 Pack 是將兩個 Vec256<short> 壓成一個 Vec256<byte>，我們這邊只有一組數據，
//                // 為了方便，我們將結果壓縮到 Vec128，或者建立一個 0 向量來填充高位。
//                // 這裡我們用 Avx2 原生指令比較直觀，或是 .NET 8 的 Narrow 方法

//                // Narrow 會將 short 轉回 byte 並做飽和處理 (Saturate)
//                // 注意：Vector256.Narrow 會輸出 Vector256<byte>，其中低128位是第一個參數的結果，高128位是第二個參數的結果
//                // 我們這裡只關心前 16 個像素，所以第二個參數放 Zero 即可
//                Vector256<byte> rByte = Vector256.Narrow(rCalc, rCalc); // 這裡為了簡單，高位重複放一樣的，反正我們只取前16個
//                Vector256<byte> gByte = Vector256.Narrow(gCalc, gCalc);
//                Vector256<byte> bByte = Vector256.Narrow(bCalc, bCalc);
//                // 實際上 Narrow 的行為在不同硬體可能不同，但在 x64 AVX2 下，
//                // 建議使用 Avx2.PackUnsignedSaturate 若追求極致控制，但 .NET 8 Generic Narrow 已足夠優化。

//                // 修正：Narrow 在 .NET 8 行為是將兩個 Source 壓縮。
//                // 為了得到正確的排列 (因為 AVX2 Pack 指令會交錯 lanes)，
//                // 比較簡單的策略是：我們現在有 rByte (32 bytes，前16 byte有效)。
//                // 為了後續 Interleave 方便，我們其實需要保留 Vector256 的寬度。

//                // 5. 格式排列 (Interleave to BGRA)
//                // 我們有 R, G, B 的向量 (低16位元有效)。我们需要构造 BGRA BGRA...

//                // 取出低 128 bits (也就是我們計算的那 16 個像素)
//                Vector128<byte> r128 = rByte.GetLower();
//                Vector128<byte> g128 = gByte.GetLower();
//                Vector128<byte> b128 = bByte.GetLower();
//                Vector128<byte> a128 = Vector128.Create((byte)255);

//                // 開始拉鍊式合併 (Zip)
//                // 假設我們需要 B G R A 的順序 (Little Endian 的 Int32 對應 BGRA)

//                // Step A: 合併 B 和 G -> [B0 G0 B1 G1 ... B7 G7] (Lo) 和 [B8 G8 ... ] (Hi)
//                Vector128<byte> bgLo = InterleaveLower(b128, g128);
//                Vector128<byte> bgHi = InterleaveUpper(b128, g128);

//                // Step B: 合併 R 和 A -> [R0 A0 R1 A1 ... R7 A7] (Lo) 和 [R8 A8 ... ] (Hi)
//                Vector128<byte> raLo = InterleaveLower(r128, a128);
//                Vector128<byte> raHi = InterleaveUpper(r128, a128);

//                // Step C: 合併 (BG) 和 (RA) -> [B0 G0 R0 A0 ... ] (最終像素)
//                Vector128<byte> pixel0_3 = InterleaveLower(bgLo, raLo);  // Pixels 0-3
//                Vector128<byte> pixel4_7 = InterleaveUpper(bgLo, raLo);  // Pixels 4-7
//                Vector128<byte> pixel8_11 = InterleaveLower(bgHi, raHi); // Pixels 8-11
//                Vector128<byte> pixel12_15 = InterleaveUpper(bgHi, raHi);// Pixels 12-15

//                // 6. 寫入記憶體 (Store)
//                // 每次寫入 16 像素 * 4 bytes = 64 bytes
//                pixel0_3.Store(rowDest + x * 4);
//                pixel4_7.Store(rowDest + x * 4 + 16);
//                pixel8_11.Store(rowDest + x * 4 + 32);
//                pixel12_15.Store(rowDest + x * 4 + 48);
//            }

//            // 處理剩下的像素 (Scalar Fallback)
//            for (; x < width; x++)
//            {
//                int yVal = rowY[x];
//                int uVal = rowU[x] - 128;
//                int vVal = rowV[x] - 128;

//                // 使用與 SIMD 相同的定點邏輯保持顏色一致
//                int r = yVal + ((vVal * 90) >> 6);
//                int g = yVal - ((uVal * 22 + vVal * 46) >> 6);
//                int b = yVal + ((uVal * 114) >> 6);

//                byte* pixel = rowDest + (x * 4);
//                pixel[0] = Clamp(b); // Blue
//                pixel[1] = Clamp(g); // Green
//                pixel[2] = Clamp(r); // Red
//                pixel[3] = 255;      // Alpha
//            }
//        });
//    }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static Vector128<byte> InterleaveLower(Vector128<byte> left, Vector128<byte> right)
//        {
//            if (Sse2.IsSupported)
//            {
//                // x86/x64: UnpackLow 負責將兩個向量的低位元交叉合併
//                return Sse2.UnpackLow(left, right);
//            }
//            else if (AdvSimd.IsSupported)
//            {
//                // ARM64: ZipLow 是一樣的功能
//                //return AdvSimd.ZipLow(left, right);
//                throw new PlatformNotSupportedException();
//            }
//            else
//            {
//                // Fallback (軟體實作，極少用到但為了安全性)
//                // 這裡省略繁瑣的軟體實作，通常 .NET 8 環境必定支援以上兩者之一
//                throw new PlatformNotSupportedException();
//            }
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static Vector128<byte> InterleaveUpper(Vector128<byte> left, Vector128<byte> right)
//        {
//            if (Sse2.IsSupported)
//            {
//                return Sse2.UnpackHigh(left, right);
//            }
//            else if (AdvSimd.IsSupported)
//            {
//                return AdvSimd.ZipHigh(left, right);
//            }
//            throw new PlatformNotSupportedException();
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    private static byte Clamp(int val)
//    {
//        if (val < 0) return 0;
//        if (val > 255) return 255;
//        return (byte)val;
//    }
//}
//}
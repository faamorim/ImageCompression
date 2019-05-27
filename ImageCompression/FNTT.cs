using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Collections;
using System.ComponentModel;

// ASCII font reference http://patorjk.com/software/taag/#p=display&f=ANSI%20Shadow

namespace ImageCompression
{
    class FNTTOptions
    {
        public byte SubsampleCbCrWidth = 2;
        public byte SubsampleCbCrHeight = 2;
        public byte SubsampleYWidth = 1;
        public byte SubsampleYHeight = 1;
        public byte QuantizeCompressFactor = 1;
        public byte HuffCompressDelta = 0;
        public byte HuffCompressMinRatio = 0;
        public byte HuffCompressMaxCount = 0;
        public byte BlockSize = 8;
    }
    class FNTT
    {
        private static readonly byte ByteZero = 128;
        private static readonly double RootTwo = Math.Sqrt(2);
        private static readonly double OneOverRootTwo = 1 / Math.Sqrt(2);
        public System.Drawing.Imaging.PixelFormat PixelFormat;
        public int Width { get; private set; }
        public int Height { get; private set; }
        private FNTTOptions Options;
        private int YWidth;
        private int YHeight;
        private int CbCrWidth;
        private int CbCrHeight;
        public byte[] data { get; private set; }
        private class YCbCrArr
        {
            public byte[] YArr;
            public byte[] CbArr;
            public byte[] CrArr;
        }
        private class YCbCrBlocks
        {
            public double[][][][] YBlocks;
            public double[][][][] CbBlocks;
            public double[][][][] CrBlocks;
        }


        public FNTT(byte[] bytearr)
        {
            Options = new FNTTOptions();
            data = bytearr;
        }


        //██████╗ ██╗████████╗███╗   ███╗ █████╗ ██████╗         ████████╗ ██████╗         ███████╗███╗   ██╗████████╗████████╗
        //██╔══██╗██║╚══██╔══╝████╗ ████║██╔══██╗██╔══██╗        ╚══██╔══╝██╔═══██╗        ██╔════╝████╗  ██║╚══██╔══╝╚══██╔══╝
        //██████╔╝██║   ██║   ██╔████╔██║███████║██████╔╝           ██║   ██║   ██║        █████╗  ██╔██╗ ██║   ██║      ██║   
        //██╔══██╗██║   ██║   ██║╚██╔╝██║██╔══██║██╔═══╝            ██║   ██║   ██║        ██╔══╝  ██║╚██╗██║   ██║      ██║   
        //██████╔╝██║   ██║   ██║ ╚═╝ ██║██║  ██║██║                ██║   ╚██████╔╝        ██║     ██║ ╚████║   ██║      ██║   
        //╚═════╝ ╚═╝   ╚═╝   ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝                ╚═╝    ╚═════╝         ╚═╝     ╚═╝  ╚═══╝   ╚═╝      ╚═╝   
        //Bitmap to FNTT

        private FNTT() { Options = new FNTTOptions(); }

        public static FNTT FromBitmap(Bitmap bmp, FNTTOptions options = null, CompressStatus status = null)
        {
            options = options ?? new FNTTOptions();
            FNTT fntt = new FNTT();
            fntt.Options = options;
            fntt.PixelFormat = bmp.PixelFormat;
            fntt.Width = bmp.Width;
            fntt.Height = bmp.Height;
            fntt.GetSizes();

            status?.SetNumSteps(8);
            /*Step 1*/
            status?.FirstStep("Compressing", "ExtractSubsampledYCbCr");
            YCbCrArr ycbcrArr = fntt.ExtractSubsampledYCbCr(bmp, status);
            /*Step 2*/
            status?.NextStep("ExtractDoubleBlocksFromByteArrays");
            YCbCrBlocks ycbcrBlocks = fntt.ExtractDoubleBlocksFromByteArrays(ycbcrArr);
            /*Step 3*/
            status?.NextStep("PerformAllBlocks FuncDCT");
            YCbCrBlocks dctBlocks = PerformAllBlocks(ycbcrBlocks, 0, FuncDCT, status);
            /*Step 4*/
            status?.NextStep("PerformAllBlocks Quantize");
            YCbCrBlocks quantizedBlocks = PerformAllBlocks(dctBlocks, options.QuantizeCompressFactor, Quantize, status);
            /*Step 5*/
            status?.NextStep("DoZigZagOnAllBlocks");
            YCbCrArr compressed = fntt.DoZigZagOnAllBlocks(quantizedBlocks);
            /*Step 6*/
            status?.NextStep("DoRunLengthOnAllArrays");
            YCbCrArr runlength = fntt.DoRunLengthOnAllArrays(compressed);
            /*Step 7*/
            status?.NextStep("GetSingleByteArrayFromSizesAndArrays");
            byte[] singlearray = fntt.GetSingleByteArrayFromSizesAndArrays(runlength);
            /*Step 8*/
            status?.NextStep("HuffmanTree Encode");
            fntt.data = HuffmanTree.Encode(singlearray);

            //dafuq
            //YCbCrArr compressed2 = fntt.ExtractByteArraysFromDoubleBlocks(dctBlocks);
            //YCbCrArr compressed3 = fntt.DoRunLengthOnAllBlocks(dctBlocks);
            //byte[] singlearray2 = fntt.GetSingleByteArrayFromSizesAndArrays(compressed3);
            //fntt.data = singlearray2;
            //fntt.data = singlearray;


            return fntt;
        }

        private YCbCrArr ExtractSubsampledYCbCr(Bitmap bmp, CompressStatus status = null)
        {
            int stepLength = Width * Height;
            status?.SetStepLength(stepLength);
            YCbCrArr ycbcrArr = new YCbCrArr
            {
                YArr = new byte[YWidth * YHeight],
                CbArr = new byte[CbCrWidth * CbCrHeight],
                CrArr = new byte[CbCrWidth * CbCrHeight]
            };
            int[] YCount = new int[YWidth * YHeight];
            double[] YDoubArr = new double[YWidth * YHeight];
            int[] CbCrCount = new int[CbCrWidth * CbCrHeight];
            double[] CbDoubArr = new double[CbCrWidth * CbCrHeight];
            double[] CrDoubArr = new double[CbCrWidth * CbCrHeight];
            for (int j = 0; j < Height; ++j)
            {
                for (int i = 0; i < Width; ++i)
                {
                    YCbCr ycbcr = YCbCr.FromColor(bmp.GetPixel(i, j));
                    long yIndex = YIndex(i, j);
                    YCount[yIndex]++;
                    YDoubArr[yIndex] += ycbcr.Y;
                    long cbcrIndex = CbCrIndex(i, j);
                    CbCrCount[cbcrIndex]++;
                    CbDoubArr[cbcrIndex] += ycbcr.Cb;
                    CrDoubArr[cbcrIndex] += ycbcr.Cr;
                    status?.AdvanceInStep();
                }
            }
            for (int index = 0; index < YWidth * YHeight; ++index)
            {
                ycbcrArr.YArr[index] = YCbCr.DoubleToByte(YDoubArr[index] / YCount[index]);
            }
            for (int index = 0; index < CbCrWidth * CbCrHeight; ++index)
            {
                ycbcrArr.CbArr[index] = YCbCr.DoubleToByte(CbDoubArr[index] / CbCrCount[index]);
                ycbcrArr.CrArr[index] = YCbCr.DoubleToByte(CrDoubArr[index] / CbCrCount[index]);
            }
            return ycbcrArr;
        }

        private YCbCrBlocks ExtractDoubleBlocksFromByteArrays(YCbCrArr ycbcrArr)
        {
            YCbCrBlocks ycbcrBlocks = new YCbCrBlocks
            {
                YBlocks = ByteArrayToDoubleBlocks(ycbcrArr.YArr, YWidth, YHeight),
                CbBlocks = ByteArrayToDoubleBlocks(ycbcrArr.CbArr, CbCrWidth, CbCrHeight),
                CrBlocks = ByteArrayToDoubleBlocks(ycbcrArr.CrArr, CbCrWidth, CbCrHeight)
            };
            return ycbcrBlocks;
        }

        private double[][][][] ByteArrayToDoubleBlocks(byte[] arr, int width, int height)
        {
            // Prepare the Double Blocks
            int HBlocks = (int)Math.Ceiling(1.0 * width / Options.BlockSize);
            int VBlocks = (int)Math.Ceiling(1.0 * height / Options.BlockSize);
            double[][][][] blocks = new double[VBlocks][][][];
            for (int r = 0; r < VBlocks; ++r)
            {
                blocks[r] = new double[HBlocks][][];
                for (int c = 0; c < HBlocks; ++c)
                {
                    blocks[r][c] = new double[Options.BlockSize][];
                    for (int m = 0; m < Options.BlockSize; ++m)
                    {
                        blocks[r][c][m] = new double[Options.BlockSize];
                    }
                }
            }

            // Fill the Double Blocks
            for (int j = 0; j < height; ++j)
            {
                int subJ = j / Options.BlockSize;
                int innerJ = j % Options.BlockSize;
                for (int i = 0; i < width; ++i)
                {
                    int subI = i / Options.BlockSize;
                    int innerI = i % Options.BlockSize;
                    double value = (double)arr[ArrIndex(i, j, width)] - ByteZero;
                    blocks[subJ][subI][innerJ][innerI] = value;
                }
            }
            return blocks;
        }



        //███████╗███╗   ██╗████████╗████████╗        ████████╗ ██████╗         ██████╗ ██╗████████╗███╗   ███╗ █████╗ ██████╗ 
        //██╔════╝████╗  ██║╚══██╔══╝╚══██╔══╝        ╚══██╔══╝██╔═══██╗        ██╔══██╗██║╚══██╔══╝████╗ ████║██╔══██╗██╔══██╗
        //█████╗  ██╔██╗ ██║   ██║      ██║              ██║   ██║   ██║        ██████╔╝██║   ██║   ██╔████╔██║███████║██████╔╝
        //██╔══╝  ██║╚██╗██║   ██║      ██║              ██║   ██║   ██║        ██╔══██╗██║   ██║   ██║╚██╔╝██║██╔══██║██╔═══╝ 
        //██║     ██║ ╚████║   ██║      ██║              ██║   ╚██████╔╝        ██████╔╝██║   ██║   ██║ ╚═╝ ██║██║  ██║██║     
        //╚═╝     ╚═╝  ╚═══╝   ╚═╝      ╚═╝              ╚═╝    ╚═════╝         ╚═════╝ ╚═╝   ╚═╝   ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝     
        //FNTT to Bitmap

        public static Bitmap ToBitmap(FNTT fntt, CompressStatus status = null)
        {
            return fntt.ToBitmap(status);
        }

        public Bitmap ToBitmap(CompressStatus status = null)
        {
            status?.SetNumSteps(8);
            /*Step 8*/
            status?.FirstStep("Decompressing", "Performing Huffman Tree Decoding");
            byte[] singlearray = HuffmanTree.Decode(data);
            /*Step 7*/
            status?.NextStep("GetSizesAndArraysFromSingleByteArray");
            YCbCrArr runlength = GetSizesAndArraysFromSingleByteArray(singlearray);
            /*Step 6*/
            status?.NextStep("UndoRunLengthOnAllArrays");
            YCbCrArr compressed = UndoRunLengthOnAllArrays(runlength);
            /*Step 5*/
            status?.NextStep("UndoZigZagOnAllArrays");
            YCbCrBlocks quantizedBlocks = UndoZigZagOnAllArrays(compressed);
            /*Step 4*/
            status?.NextStep("PerformAllBlocks Dequantize");
            YCbCrBlocks dctBlocks = PerformAllBlocks(quantizedBlocks, Options.QuantizeCompressFactor, Dequantize);
            /*Step 3*/
            status?.NextStep("PerformAllBlocks FuncIDCT");
            YCbCrBlocks ycbcrBlocks = PerformAllBlocks(dctBlocks, 0, FuncIDCT, status);
            /*Step 2*/
            status?.NextStep("ExtractByteArraysFromDoubleBlocks");
            YCbCrArr ycbcrArr = ExtractByteArraysFromDoubleBlocks(ycbcrBlocks);
            /*Step 1*/
            status?.NextStep("ExtractBitmap");
            Bitmap result = ExtractBitmap(ycbcrArr);

            return result;
        }

        private Bitmap ExtractBitmap(YCbCrArr ycbcrArr)
        {
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat);
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    long k = YIndex(i, j);
                    byte y = ycbcrArr.YArr[YIndex(i, j)];
                    byte cb = ycbcrArr.CbArr[CbCrIndex(i, j)];
                    byte cr = ycbcrArr.CrArr[CbCrIndex(i, j)];
                    if (cb == cr)
                    {
                        int f = 0;
                    }
                    Color c = YCbCr.ToColor(y, cb, cr);
                    if (c.R == 0 && c.G == 0 && c.B == 0)
                    {

                    }
                    if (c.R == 255 && c.G == 255 && c.B == 255)
                    {

                    }
                    bmp.SetPixel(i, j, c);
                }
            }
            return bmp;
        }


        private YCbCrArr ExtractByteArraysFromDoubleBlocks(YCbCrBlocks ycbcrBlocks)
        {
            YCbCrArr ycbcrArr = new YCbCrArr
            {
                YArr = DoubleBlocksToByteArray(ycbcrBlocks.YBlocks, YWidth, YHeight),
                CbArr = DoubleBlocksToByteArray(ycbcrBlocks.CbBlocks, CbCrWidth, CbCrHeight),
                CrArr = DoubleBlocksToByteArray(ycbcrBlocks.CrBlocks, CbCrWidth, CbCrHeight)
            };
            return ycbcrArr;
        }

        private byte[] DoubleBlocksToByteArray(double[][][][] blocks, int width, int height)
        {
            // Prepare the Byte Array
            byte[] byteArr = new byte[width * height];

            // Fill the Byte Array
            for (int r = 0; r < blocks.Length; ++r)
            {
                for (int c = 0; c < blocks[r].Length; ++c)
                {
                    for (int j = 0; j < Options.BlockSize; ++j)
                    {
                        for (int i = 0; i < Options.BlockSize; ++i)
                        {
                            int x = c * Options.BlockSize + i;
                            int y = r * Options.BlockSize + j;
                            long index = y * width + x;

                            if (x < width && y < height && index < byteArr.Length)
                            {
                                byte value = YCbCr.DoubleToByte(blocks[r][c][j][i] + ByteZero);
                                byteArr[index] = value;
                            }
                        }
                    }
                }
            }
            return byteArr;
        }




        //███████╗██╗   ██╗██████╗ ██████╗  ██████╗ ██████╗ ████████╗
        //██╔════╝██║   ██║██╔══██╗██╔══██╗██╔═══██╗██╔══██╗╚══██╔══╝
        //███████╗██║   ██║██████╔╝██████╔╝██║   ██║██████╔╝   ██║   
        //╚════██║██║   ██║██╔═══╝ ██╔═══╝ ██║   ██║██╔══██╗   ██║   
        //███████║╚██████╔╝██║     ██║     ╚██████╔╝██║  ██║   ██║   
        //╚══════╝ ╚═════╝ ╚═╝     ╚═╝      ╚═════╝ ╚═╝  ╚═╝   ╚═╝   
        //Support

        void GetSizes()
        {
            CbCrWidth = (int)Math.Ceiling(1.0 * Width / Options.SubsampleCbCrWidth);
            CbCrHeight = (int)Math.Ceiling(1.0 * Height / Options.SubsampleCbCrHeight);
            YWidth = (int)Math.Ceiling(1.0 * Width / Options.SubsampleYWidth);
            YHeight = (int)Math.Ceiling(1.0 * Height / Options.SubsampleYHeight);
        }


        long YIndex(int x, int y)
        {
            int yx = (int)Math.Floor(1.0 * x / Options.SubsampleYWidth);
            int yy = (int)Math.Floor(1.0 * y / Options.SubsampleYHeight);
            return ArrIndex(yx, yy, YWidth);
        }

        long CbCrIndex(int x, int y)
        {

            int cx = x / Options.SubsampleCbCrWidth;
            int cy = y / Options.SubsampleCbCrHeight;
            return ArrIndex(cx, cy, CbCrWidth);
        }

        static long ArrIndex(int x, int y, int width)
        {
            return y * width + x;
        }


        static YCbCrBlocks PerformAllBlocks(YCbCrBlocks input, byte arg, Func<double[][], byte, double[][]> transform, CompressStatus status = null)
        {
            int stepLength = input.YBlocks.Length * input.YBlocks[0].Length;
            stepLength += input.CbBlocks.Length * input.CbBlocks[0].Length;
            stepLength += input.CrBlocks.Length * input.CrBlocks[0].Length;
            status?.SetStepLength(stepLength);
            YCbCrBlocks output = new YCbCrBlocks();
            output.YBlocks = PerformBlocks(input.YBlocks, arg, transform, status);
            output.CbBlocks = PerformBlocks(input.CbBlocks, arg, transform, status);
            output.CrBlocks = PerformBlocks(input.CrBlocks, arg, transform, status);
            return output;
        }

        static double[][][][] PerformBlocks(double[][][][] input, byte arg, Func<double[][], byte, double[][]> transform, CompressStatus status)
        {
            double[][][][] output = new double[input.Length][][][];
            for (int j = 0; j < input.Length; ++j)
            {
                output[j] = new double[input[j].Length][][];
                for (int i = 0; i < input[j].Length; ++i)
                {
                    output[j][i] = transform(input[j][i], arg);
                    status?.AdvanceInStep();
                }
            }
            return output;
        }




        static private double[][] FuncDCT(double[][] vals, byte b) { return DCT(vals); }
        static double[][] DCT(double[][] vals)
        {
            double sum = 0;
            int h = vals.Length;
            int w = vals[0].Length;
            double[][] dct = new double[h][];
            for (int r = 0; r < h; ++r)
            {
                dct[r] = new double[w];
            }

            for (int v = 0; v < h; ++v)
            {
                for (int u = 0; u < w; ++u)
                {
                    sum += vals[v][u];
                    double dctUV = 0;
                    for (int y = 0; y < h; ++y)
                    {
                        for (int x = 0; x < w; ++x)
                        {
                            dctUV += vals[y][x] * CosTransform(u, v, x, y, w, h);
                        }
                    }
                    dct[v][u] = 0.25 * alpha(u) * alpha(v) * dctUV;
                }
            }

            return dct;
        }


        static private double[][] FuncIDCT(double[][] dct, byte b) { return IDCT(dct); }
        static double[][] IDCT(double[][] dct)
        {
            double min = 255;
            double max = -255;
            int h = dct.Length;
            int w = dct[0].Length;
            double[][] vals = new double[h][];
            for (int r = 0; r < h; ++r)
            {
                vals[r] = new double[w];
            }

            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    double idctUV = 0;
                    for (int v = 0; v < h; ++v)
                    {
                        for (int u = 0; u < w; ++u)
                        {
                            idctUV += alpha(u) * alpha(v) * dct[v][u] * CosTransform(u, v, x, y, w, h);
                        }
                    }
                    vals[y][x] = 0.25 * idctUV;
                    max = Math.Max(vals[y][x], max);
                    min = Math.Min(vals[y][x], min);
                }
            }

            return vals;
        }

        static double CosTransform(int u, int v, int x, int y, int w, int h)
        {
            return Math.Cos(((2 * x + 1) * u * Math.PI) / (w + h)) * Math.Cos(((2 * y + 1) * v * Math.PI) / (w + h));
        }

        static double alpha(int u)
        {
            if (u == 0)
            {
                return OneOverRootTwo;
            }
            else
            {
                return 1;
            }
        }

        static int getRowAndColumnFromZigZag()
        {
            return 0;
        }

        static public bool CompareArrays<T>(string name, T[] arr1, T[] arr2) where T : IComparable<T>
        {
            bool result = true;
            if (arr1.Length != arr2.Length)
            {
                result = false;
            }
            for (int i = 0; i < arr1.Length && i < arr2.Length; i++)
            {
                if (arr1[i].CompareTo(arr2[i]) != 0)
                {
                    result = false;
                }
            }
            return result;
        }


        // ██████╗ ██╗   ██╗ █████╗ ███╗   ██╗████████╗██╗███████╗██╗███╗   ██╗ ██████╗ 
        //██╔═══██╗██║   ██║██╔══██╗████╗  ██║╚══██╔══╝██║╚══███╔╝██║████╗  ██║██╔════╝ 
        //██║   ██║██║   ██║███████║██╔██╗ ██║   ██║   ██║  ███╔╝ ██║██╔██╗ ██║██║  ███╗
        //██║▄▄ ██║██║   ██║██╔══██║██║╚██╗██║   ██║   ██║ ███╔╝  ██║██║╚██╗██║██║   ██║
        //╚██████╔╝╚██████╔╝██║  ██║██║ ╚████║   ██║   ██║███████╗██║██║ ╚████║╚██████╔╝
        // ╚══▀▀═╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═══╝   ╚═╝   ╚═╝╚══════╝╚═╝╚═╝  ╚═══╝ ╚═════╝ 
        //Quantizing

        static double[][] Quantize(double[][] input, byte quantizingCompression) { return BlockQuantization(input, quantizingCompression, true); }
        static double[][] Dequantize(double[][] input, byte quantizingCompression) { return BlockQuantization(input, quantizingCompression, false); }
        static double[][] BlockQuantization(double[][] input, byte quantizingCompression = 0, bool quantize = true)
        {
            if (quantizingCompression == 0)
            {
                return input;
            }
            double quantizingModifier = 1 + (quantizingCompression - 1) * 9 / (Byte.MaxValue - 1);
            quantizingModifier = Math.Max(1, Math.Min(quantizingModifier, 10));
            int h = input.Length;
            int w = input[0].Length;
            if (w == 8 && h == 8)
            {
                double[][] output = ApplyQuantization(input, QuantizingTable8x8, w, h, quantizingModifier, quantize);
                return output;
            }

            return input;
        }

        static double[][] ApplyQuantization(double[][] input, double[][] quantizing, int w, int h, double quantizingModifier, bool quantize)
        {
            double[][] output = new double[h][];
            for (int j = 0; j < h; ++j)
            {
                output[j] = new double[w];
                for (int i = 0; i < w; ++i)
                {
                    double quantizingRatio = quantizing[j][i] * quantizingModifier;
                    if (quantize)
                        output[j][i] = Math.Round(input[j][i] / quantizingRatio);
                    else
                        output[j][i] = input[j][i] * quantizingRatio;
                    /* TEST */
                    double test = output[j][i];
                    if (i + j > 0 && (test > 1000 || test < -1000))
                    {
                        output[j][i] = 0;
                    }
                }
            }
            return output;
        }


        static double[][] QuantizingTable8x8 = new double[][] {
            new double[]{16, 11, 10, 16, 24, 40, 51, 61 },
            new double[]{12, 12, 14, 19, 26, 58, 60, 55 },
            new double[]{14, 13, 16, 24, 40, 57, 69, 56 },
            new double[]{14, 17, 22, 29, 51, 87, 80, 62 },
            new double[]{18, 22, 37, 56, 68, 109, 103, 77 },
            new double[]{24, 35, 55, 64, 81, 104, 113, 92 },
            new double[]{49, 64, 78, 87, 103, 121, 120, 101 },
            new double[]{72, 92, 95, 98, 112, 100, 103, 99 }
        };



        //███████╗ █████╗ ██╗   ██╗███████╗        ██╗    ██╗      ██████╗  █████╗ ██████╗ 
        //██╔════╝██╔══██╗██║   ██║██╔════╝       ██╔╝    ██║     ██╔═══██╗██╔══██╗██╔══██╗
        //███████╗███████║██║   ██║█████╗        ██╔╝     ██║     ██║   ██║███████║██║  ██║
        //╚════██║██╔══██║╚██╗ ██╔╝██╔══╝       ██╔╝      ██║     ██║   ██║██╔══██║██║  ██║
        //███████║██║  ██║ ╚████╔╝ ███████╗    ██╔╝       ███████╗╚██████╔╝██║  ██║██████╔╝
        //╚══════╝╚═╝  ╚═╝  ╚═══╝  ╚══════╝    ╚═╝        ╚══════╝ ╚═════╝ ╚═╝  ╚═╝╚═════╝ 
        //Save / Load

        public int GetCompressedSize()
        {
            return data.Length;
        }

        YCbCrArr DoZigZagOnAllBlocks(YCbCrBlocks ycbcrblocks)
        {
            YCbCrArr ycbcrarr = new YCbCrArr
            {
                YArr = DoZigZagOnBlocks(ycbcrblocks.YBlocks),
                CbArr = DoZigZagOnBlocks(ycbcrblocks.CbBlocks),
                CrArr = DoZigZagOnBlocks(ycbcrblocks.CrBlocks)
            };
            return ycbcrarr;
        }

        byte[] DoZigZagOnBlocks(double[][][][] blocks)
        {
            byte[] bytearr;
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter bin = new BinaryWriter(mem))
                {
                    for (int dist = 0; dist < 2 * Options.BlockSize - 1; ++dist)
                    {
                        int maxChange = Math.Min(dist, 2 * Options.BlockSize - 2 - dist);
                        int spare = Math.Max(0, dist - Options.BlockSize + 1);
                        int maxDist = Math.Min(dist, Options.BlockSize - 1);
                        for (int change = 0; change <= maxChange; ++change)
                        {
                            int i = maxDist - change;
                            int j = spare + change;
                            if (dist % 2 == 0)
                            {
                                j = maxDist - change;
                                i = spare + change;
                            }
                            int rows = blocks.Length;
                            for (int r = 0; r < rows; ++r)
                            {
                                int cols = blocks[r].Length;
                                for (int c = 0; c < cols; ++c)
                                {
                                    byte val = YCbCr.DoubleToByte(blocks[r][c][j][i] + ByteZero);
                                    bin.Write(val);
                                }
                            }
                        }
                    }
                }
                bytearr = mem.ToArray();
            }
            return bytearr;
        }


        YCbCrArr DoRunLengthOnAllArrays(YCbCrArr ycbcrarr)
        {
            YCbCrArr runlength = new YCbCrArr
            {
                YArr = DoRunLengthOnArray(ycbcrarr.YArr),
                CbArr = DoRunLengthOnArray(ycbcrarr.CbArr),
                CrArr = DoRunLengthOnArray(ycbcrarr.CrArr)
            };
            return runlength;
        }

        byte[] DoRunLengthOnArray(byte[] bytearr)
        {
            byte[] runlength;
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter bin = new BinaryWriter(mem))
                {
                    byte run = 0;
                    for (int index = 0; index < bytearr.Length; index++)
                    {
                        byte val = bytearr[index];
                        if (run == 0)
                        {
                            bin.Write(val);
                            if (val == ByteZero)
                            {
                                run++;
                            }
                        }
                        else
                        {
                            if (run == Byte.MaxValue)
                            {
                                bin.Write(run);
                                run = 0;
                            }
                            if (val == ByteZero)
                            {
                                run++;
                            }
                            else
                            {
                                bin.Write(run);
                                run = 0;
                                bin.Write(val);
                            }
                        }
                    }
                    if (run != 0)
                    {
                        bin.Write(run);
                    }
                }
                runlength = mem.ToArray();
            }
            return runlength;
        }



        YCbCrBlocks UndoZigZagOnAllArrays(YCbCrArr ycbcrarr)
        {
            YCbCrBlocks ycbcrblocks = new YCbCrBlocks
            {
                YBlocks = UndoZigZagOnArray(ycbcrarr.YArr, YWidth, YHeight),
                CbBlocks = UndoZigZagOnArray(ycbcrarr.CbArr, CbCrWidth, CbCrHeight),
                CrBlocks = UndoZigZagOnArray(ycbcrarr.CrArr, CbCrWidth, CbCrHeight)
            };
            return ycbcrblocks;
        }

        double[][][][] UndoZigZagOnArray(byte[] bytearr, int width, int height)
        {
            int HBlocks = (int)Math.Ceiling(1.0 * width / Options.BlockSize);
            int VBlocks = (int)Math.Ceiling(1.0 * height / Options.BlockSize);
            double[][][][] blocks = new double[VBlocks][][][];
            for (int r = 0; r < VBlocks; ++r)
            {
                blocks[r] = new double[HBlocks][][];
                for (int c = 0; c < HBlocks; ++c)
                {
                    blocks[r][c] = new double[Options.BlockSize][];
                    for (int m = 0; m < Options.BlockSize; ++m)
                    {
                        blocks[r][c][m] = new double[Options.BlockSize];
                    }
                }
            }

            int index = 0;
            for (int dist = 0; dist < 2 * Options.BlockSize - 1; ++dist)
            {
                int maxChange = Math.Min(dist, 2 * Options.BlockSize - 2 - dist);
                int spare = Math.Max(0, dist - Options.BlockSize + 1);
                int maxDist = Math.Min(dist, Options.BlockSize - 1);
                for (int change = 0; change <= maxChange; ++change)
                {
                    int i = maxDist - change;
                    int j = spare + change;
                    if (dist % 2 == 0)
                    {
                        j = maxDist - change;
                        i = spare + change;
                    }
                    for (int r = 0; r < blocks.Length; ++r)
                    {
                        for (int c = 0; c < blocks[r].Length; ++c)
                        {
                            byte val = bytearr[index++];
                            blocks[r][c][j][i] = val - ByteZero;
                        }
                    }
                }
            }


            return blocks;
        }



        YCbCrArr UndoRunLengthOnAllArrays(YCbCrArr runlength)
        {
            YCbCrArr ycbcrblocks = new YCbCrArr
            {
                YArr = UndoRunLengthOnArray(runlength.YArr),
                CbArr = UndoRunLengthOnArray(runlength.CbArr),
                CrArr = UndoRunLengthOnArray(runlength.CrArr)
            };
            return ycbcrblocks;
        }

        byte[] UndoRunLengthOnArray(byte[] runlength)
        {
            byte[] bytearr;
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter bin = new BinaryWriter(mem))
                {
                    byte run = 0;
                    bool extraRun = false;
                    for (int index = 0; index < runlength.Length; index++)
                    {
                        byte val = runlength[index];
                        if (val == ByteZero)
                        {
                            do
                            {
                                run = runlength[++index];
                                extraRun = (run == Byte.MaxValue);
                                while (run-- > 0)
                                {
                                    bin.Write(ByteZero);
                                }
                            } while (extraRun);
                        }
                        else
                        {
                            bin.Write(val);
                        }
                    }
                }
                bytearr = mem.ToArray();
            }
            return bytearr;
        }




        byte[] GetSingleByteArrayFromSizesAndArrays(YCbCrArr ycbcrarr)
        {
            byte[] bytearr;
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter save = new BinaryWriter(mem))
                {
                    save.Write(Width);
                    save.Write(Height);
                    save.Write((int)PixelFormat);
                    save.Write(Options.SubsampleCbCrWidth);
                    save.Write(Options.SubsampleCbCrHeight);
                    save.Write(Options.SubsampleYWidth);
                    save.Write(Options.SubsampleYHeight);
                    save.Write(Options.BlockSize);
                    save.Write(Options.QuantizeCompressFactor);
                    save.Write(ycbcrarr.YArr.Length);
                    for (int i = 0; i < ycbcrarr.YArr.Length; ++i)
                    {
                        save.Write(ycbcrarr.YArr[i]);
                    }
                    save.Write(ycbcrarr.CbArr.Length);
                    for (int i = 0; i < ycbcrarr.CbArr.Length; ++i)
                    {
                        save.Write(ycbcrarr.CbArr[i]);
                    }
                    save.Write(ycbcrarr.CrArr.Length);
                    for (int i = 0; i < ycbcrarr.CrArr.Length; ++i)
                    {
                        save.Write(ycbcrarr.CrArr[i]);
                    }
                }
                bytearr = mem.ToArray();
            }
            return bytearr;
        }

        YCbCrArr GetSizesAndArraysFromSingleByteArray(byte[] bytearr)
        {
            int index = 0;
            int w = 0;
            GetNext(ref w, ref bytearr, ref index);
            Width = w;
            int h = 0;
            GetNext(ref h, ref bytearr, ref index);
            Height = h;
            int pf = 0;
            GetNext(ref pf, ref bytearr, ref index);
            PixelFormat = (System.Drawing.Imaging.PixelFormat)pf;
            GetNext(ref Options.SubsampleCbCrWidth, ref bytearr, ref index);
            GetNext(ref Options.SubsampleCbCrHeight, ref bytearr, ref index);
            GetNext(ref Options.SubsampleYWidth, ref bytearr, ref index);
            GetNext(ref Options.SubsampleYHeight, ref bytearr, ref index);
            GetNext(ref Options.BlockSize, ref bytearr, ref index);
            GetNext(ref Options.QuantizeCompressFactor, ref bytearr, ref index);
            GetSizes();
            int yarrSize = 0;
            int cbarrSize = 0;
            int crarrSize = 0;
            GetNext(ref yarrSize, ref bytearr, ref index);
            byte[] yarr = new byte[yarrSize];
            for (int i = 0; i < yarrSize; ++i)
            {
                yarr[i] = bytearr[index++];
            }
            GetNext(ref cbarrSize, ref bytearr, ref index);
            byte[] cbarr = new byte[cbarrSize];
            for (int i = 0; i < cbarrSize; ++i)
            {
                cbarr[i] = bytearr[index++];
            }
            GetNext(ref crarrSize, ref bytearr, ref index);
            byte[] crarr = new byte[crarrSize];
            for (int i = 0; i < crarrSize; ++i)
            {
                crarr[i] = bytearr[index++];
            }
            YCbCrArr ycbcrarr = new YCbCrArr
            {
                YArr = yarr,
                CbArr = cbarr,
                CrArr = crarr
            };
            return ycbcrarr;
        }


        // INCREASE AN INDEX BY A GIVEN AMOUNT
        static int ByteStep(ref int index, int step)
        {
            int i = index;
            index += step;
            return i;
        }

        // STATIC METHODS TO TRAVERSE A BYTE ARRAY AND READ A GIVEN DATA.

        // GET NEXT
        public static void GetNext(ref int i, ref byte[] bytes, ref int offset)
        {
            i = GetNextInt(ref bytes, ref offset);
        }

        public static void GetNext(ref short shrt, ref byte[] bytes, ref int offset)
        {
            shrt = GetNextShort(ref bytes, ref offset);
        }

        public static void GetNext(ref byte bt, ref byte[] bytes, ref int offset)
        {
            bt = GetNextByte(ref bytes, ref offset);
        }

        static void GetNext(ref string strg, ref byte[] bytes, ref int offset)
        {
            strg = GetNextStringFour(ref bytes, ref offset);
        }

        // SPECIFIC GET NEXT
        static int GetNextInt(ref byte[] bytes, ref int offset)
        {
            return BitConverter.ToInt32(bytes, ByteStep(ref offset, 4));
        }

        static short GetNextShort(ref byte[] bytes, ref int offset)
        {
            return BitConverter.ToInt16(bytes, ByteStep(ref offset, 2));
        }

        static byte GetNextByte(ref byte[] bytes, ref int offset)
        {
            return bytes[ByteStep(ref offset, 1)];
        }

        static string GetNextStringFour(ref byte[] bytes, ref int offset)
        {
            string str = "";
            for (int i = 0; i < 4; ++i)
            {
                str += (char)bytes[offset++];
            }
            return str;
        }


        //████████╗███████╗███████╗████████╗██╗███╗   ██╗ ██████╗ 
        //╚══██╔══╝██╔════╝██╔════╝╚══██╔══╝██║████╗  ██║██╔════╝ 
        //   ██║   █████╗  ███████╗   ██║   ██║██╔██╗ ██║██║  ███╗
        //   ██║   ██╔══╝  ╚════██║   ██║   ██║██║╚██╗██║██║   ██║
        //   ██║   ███████╗███████║   ██║   ██║██║ ╚████║╚██████╔╝
        //   ╚═╝   ╚══════╝╚══════╝   ╚═╝   ╚═╝╚═╝  ╚═══╝ ╚═════╝ 
        //Testing


        public static void RunDCTTest()
        {
            FNTT fntt = new FNTT();
            //byte[][] input = new byte[][]{new byte[]{52, 55, 61, 66, 70, 61, 64, 73},
            //    new byte[]{63, 59, 55, 90, 109, 85, 69, 72},
            //    new byte[]{62, 59, 68, 113, 144, 104, 66, 73},
            //    new byte[]{63, 58, 71, 122, 154, 106, 70, 69},
            //    new byte[]{67, 61, 68, 104, 126, 88, 68, 70},
            //    new byte[]{79, 65, 60, 70, 77, 68, 58, 75},
            //    new byte[]{85, 71, 64, 59, 55, 61, 65, 83},
            //    new byte[]{87, 79, 69, 68, 65, 76, 78, 94} };
            byte[] input = new byte[]{52, 55, 61, 66, 70, 61, 64, 73,
                63, 59, 55, 90, 109, 85, 69, 72,
                62, 59, 68, 113, 144, 104, 66, 73,
                63, 58, 71, 122, 154, 106, 70, 69,
                67, 61, 68, 104, 126, 88, 68, 70,
                79, 65, 60, 70, 77, 68, 58, 75,
                85, 71, 64, 59, 55, 61, 65, 83,
                87, 79, 69, 68, 65, 76, 78, 94 };
            double[][][][] doubles = fntt.ByteArrayToDoubleBlocks(input, 8, 8);
            byte[] step1undo = fntt.DoubleBlocksToByteArray(doubles, 8, 8);
            bool step1compare = CompareArrays("step1", input, step1undo);
            double[][] dct = DCT(doubles[0][0]);
            double[][][][] idct = new double[1][][][];
            idct[0] = new double[1][][];
            idct[0][0] = IDCT(dct);
            byte[] output = fntt.DoubleBlocksToByteArray(idct, 8, 8);
            bool step3compare = CompareArrays("step3", input, output);
        }

        public static void RunRLETest()
        {
            FNTT fntt = new FNTT();
            byte[] input = new byte[] { };
            //
        }

    }
}

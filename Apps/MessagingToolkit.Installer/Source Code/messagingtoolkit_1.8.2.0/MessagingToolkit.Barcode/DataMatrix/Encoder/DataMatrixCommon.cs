using System;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal static class DataMatrixCommon
    {
        internal static void GenReedSolEcc(DataMatrixMessage message, DataMatrixSymbolSize sizeIdx)
        {
            byte[] g = new byte[69];
            byte[] b = new byte[68];

            int symbolDataWords = GetSymbolAttribute(DataMatrixSymAttribute.SymAttribSymbolDataWords, sizeIdx);
            int symbolErrorWords = GetSymbolAttribute(DataMatrixSymAttribute.SymAttribSymbolErrorWords, sizeIdx);
            int symbolTotalWords = symbolDataWords + symbolErrorWords;
            int blockErrorWords = GetSymbolAttribute(DataMatrixSymAttribute.SymAttribBlockErrorWords, sizeIdx);
            int step = GetSymbolAttribute(DataMatrixSymAttribute.SymAttribInterleavedBlocks, sizeIdx);
            if (blockErrorWords != symbolErrorWords / step)
            {
                throw new Exception("Error generation reed solomon error correction");
            }

            for (int gI = 0; gI < g.Length; gI++)
            {
                g[gI] = 0x01;
            }

            // Generate ECC polynomia
            for (int i = 1; i <= blockErrorWords; i++)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    g[j] = GfDoublify(g[j], i);     // g[j] *= 2**i
                    if (j > 0)
                        g[j] = GfSum(g[j], g[j - 1]);  // g[j] += g[j-1]
                }
            }

            // Populate error codeword array
            for (int block = 0; block < step; block++)
            {
                for (int bI = 0; bI < b.Length; bI++)
                {
                    b[bI] = 0;
                }

                for (int i = block; i < symbolDataWords; i += step)
                {
                    int val = GfSum(b[blockErrorWords - 1], message.Code[i]);
                    for (int j = blockErrorWords - 1; j > 0; j--)
                    {
                        b[j] = GfSum(b[j - 1], GfProduct(g[j], val));
                    }
                    b[0] = GfProduct(g[0], val);
                }

                int blockDataWords = GetBlockDataSize(sizeIdx, block);
                int bIndex = blockErrorWords;

                for (int i = block + (step * blockDataWords); i < symbolTotalWords; i += step)
                {
                    message.Code[i] = b[--bIndex];
                }

                if (bIndex != 0)
                {
                    throw new Exception("Error generation error correction code!");
                }
            }
        }

        private static byte GfProduct(byte a, int b)
        {
            if (a == 0 || b == 0)
                return 0;
            
            return (byte)DataMatrixConstants.aLogVal[(DataMatrixConstants.logVal[a] + DataMatrixConstants.logVal[b]) % 255];
        }

        private static byte GfSum(byte a, byte b)
        {
            return (byte)(a ^ b);
        }

        private static byte GfDoublify(byte a, int b)
        {
            if (a == 0) /* XXX this is right, right? */
                return 0;
            if (b == 0)
                return a; /* XXX this is right, right? */
            
            return (byte)DataMatrixConstants.aLogVal[(DataMatrixConstants.logVal[a] + b) % 255];
        }

        internal static int GetSymbolAttribute(DataMatrixSymAttribute attribute, DataMatrixSymbolSize sizeIdx)
        {
            if (sizeIdx < 0 || (int)sizeIdx >= DataMatrixConstants.DataMatrixSymbolSquareCount + DataMatrixConstants.DataMatrixSymbolRectCount)
                return DataMatrixConstants.DataMatrixUndefined;

            switch (attribute)
            {
                case DataMatrixSymAttribute.SymAttribSymbolRows:
                    return DataMatrixConstants.SymbolRows[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribSymbolCols:
                    return DataMatrixConstants.SymbolCols[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribDataRegionRows:
                    return DataMatrixConstants.DataRegionRows[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribDataRegionCols:
                    return DataMatrixConstants.DataRegionCols[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribHorizDataRegions:
                    return DataMatrixConstants.HorizDataRegions[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribVertDataRegions:
                    return ((int)sizeIdx < DataMatrixConstants.DataMatrixSymbolSquareCount) ? DataMatrixConstants.HorizDataRegions[(int)sizeIdx] : 1;
                case DataMatrixSymAttribute.SymAttribMappingMatrixRows:
                    return DataMatrixConstants.DataRegionRows[(int)sizeIdx] *
                          GetSymbolAttribute(DataMatrixSymAttribute.SymAttribVertDataRegions, sizeIdx);
                case DataMatrixSymAttribute.SymAttribMappingMatrixCols:
                    return DataMatrixConstants.DataRegionCols[(int)sizeIdx] * DataMatrixConstants.HorizDataRegions[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribInterleavedBlocks:
                    return DataMatrixConstants.InterleavedBlocks[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribBlockErrorWords:
                    return DataMatrixConstants.BlockErrorWords[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribBlockMaxCorrectable:
                    return DataMatrixConstants.BlockMaxCorrectable[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribSymbolDataWords:
                    return DataMatrixConstants.SymbolDataWords[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribSymbolErrorWords:
                    return DataMatrixConstants.BlockErrorWords[(int)sizeIdx] * DataMatrixConstants.InterleavedBlocks[(int)sizeIdx];
                case DataMatrixSymAttribute.SymAttribSymbolMaxCorrectable:
                    return DataMatrixConstants.BlockMaxCorrectable[(int)sizeIdx] * DataMatrixConstants.InterleavedBlocks[(int)sizeIdx];
            }
            return DataMatrixConstants.DataMatrixUndefined;
        }

        internal static int GetBlockDataSize(DataMatrixSymbolSize sizeIdx, int blockIdx)
        {
            int symbolDataWords = GetSymbolAttribute(DataMatrixSymAttribute.SymAttribSymbolDataWords, sizeIdx);
            int interleavedBlocks = GetSymbolAttribute(DataMatrixSymAttribute.SymAttribInterleavedBlocks, sizeIdx);
            int count = symbolDataWords / interleavedBlocks;

            if (symbolDataWords < 1 || interleavedBlocks < 1)
                return DataMatrixConstants.DataMatrixUndefined;

            return (sizeIdx == DataMatrixSymbolSize.Symbol144x144 && blockIdx < 8) ? count + 1 : count;
        }

        internal static DataMatrixSymbolSize FindCorrectSymbolSize(int dataWords, DataMatrixSymbolSize sizeIdxRequest)
        {
            DataMatrixSymbolSize sizeIdx;
            if (dataWords <= 0)
            {
                return DataMatrixSymbolSize.SymbolShapeAuto;
            }

            if (sizeIdxRequest == DataMatrixSymbolSize.SymbolSquareAuto || sizeIdxRequest == DataMatrixSymbolSize.SymbolRectAuto)
            {
                DataMatrixSymbolSize idxBeg;
                DataMatrixSymbolSize idxEnd;
                if (sizeIdxRequest == DataMatrixSymbolSize.SymbolSquareAuto)
                {
                    idxBeg = 0;
                    idxEnd = (DataMatrixSymbolSize)DataMatrixConstants.DataMatrixSymbolSquareCount;
                }
                else
                {
                    idxBeg = (DataMatrixSymbolSize)DataMatrixConstants.DataMatrixSymbolSquareCount;
                    idxEnd = (DataMatrixSymbolSize)(DataMatrixConstants.DataMatrixSymbolSquareCount + DataMatrixConstants.DataMatrixSymbolRectCount);
                }

                for (sizeIdx = idxBeg; sizeIdx < idxEnd; sizeIdx++)
                {
                    if (GetSymbolAttribute(DataMatrixSymAttribute.SymAttribSymbolDataWords, sizeIdx) >= dataWords)
                        break;
                }

                if (sizeIdx == idxEnd)
                {
                    return DataMatrixSymbolSize.SymbolShapeAuto;
                }
            }
            else
            {
                sizeIdx = sizeIdxRequest;
            }

            if (dataWords > GetSymbolAttribute(DataMatrixSymAttribute.SymAttribSymbolDataWords, sizeIdx))
            {
                return DataMatrixSymbolSize.SymbolShapeAuto;
            }

            return sizeIdx;
        }

        internal static int GetBitsPerPixel(DataMatrixPackOrder pack)
        {
            switch (pack)
            {
                case DataMatrixPackOrder.Pack1bppK:
                    return 1;
                case DataMatrixPackOrder.Pack8bppK:
                    return 8;
                case DataMatrixPackOrder.Pack16bppRGB:
                case DataMatrixPackOrder.Pack16bppRGBX:
                case DataMatrixPackOrder.Pack16bppXRGB:
                case DataMatrixPackOrder.Pack16bppBGR:
                case DataMatrixPackOrder.Pack16bppBGRX:
                case DataMatrixPackOrder.Pack16bppXBGR:
                case DataMatrixPackOrder.Pack16bppYCbCr:
                    return 16;
                case DataMatrixPackOrder.Pack24bppRGB:
                case DataMatrixPackOrder.Pack24bppBGR:
                case DataMatrixPackOrder.Pack24bppYCbCr:
                    return 24;
                case DataMatrixPackOrder.Pack32bppRGBX:
                case DataMatrixPackOrder.Pack32bppXRGB:
                case DataMatrixPackOrder.Pack32bppBGRX:
                case DataMatrixPackOrder.Pack32bppXBGR:
                case DataMatrixPackOrder.Pack32bppCMYK:
                    return 32;
            }

            return DataMatrixConstants.DataMatrixUndefined;
        }

        internal static T Min<T>(T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) < 0 ? x : y;
        }

        internal static T Max<T>(T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) < 0 ? y : x;
        }

        internal static bool DecodeCheckErrors(byte[] code, int codeIndex, DataMatrixSymbolSize sizeIdx, int fix)
        {
            byte[] data = new byte[255];

            int interleavedBlocks = GetSymbolAttribute(DataMatrixSymAttribute.SymAttribInterleavedBlocks, sizeIdx);
            int blockErrorWords = GetSymbolAttribute(DataMatrixSymAttribute.SymAttribBlockErrorWords, sizeIdx);

            const int fixedErr = 0;
            int fixedErrSum = 0;
            for (int i = 0; i < interleavedBlocks; i++)
            {
                int blockTotalWords = blockErrorWords + GetBlockDataSize(sizeIdx, i);

                int j;
                for (j = 0; j < blockTotalWords; j++)
                    data[j] = code[j * interleavedBlocks + i];

                fixedErrSum += fixedErr;

                for (j = 0; j < blockTotalWords; j++)
                    code[j * interleavedBlocks + i] = data[j];
            }

            if (fix != DataMatrixConstants.DataMatrixUndefined && fix >= 0 && fix < fixedErrSum)
            {
                return false;
            }

            return true;
        }

        internal static double RightAngleTrueness(DataMatrixVector2 c0, DataMatrixVector2 c1, DataMatrixVector2 c2, double angle)
        {
            DataMatrixVector2 vA = (c0 - c1);
            DataMatrixVector2 vB = (c2 - c1);
            vA.Norm();
            vB.Norm();

            DataMatrixMatrix3 m = DataMatrixMatrix3.Rotate(angle);
            vB *= m;

            return vA.Dot(vB);
        }

    }
}

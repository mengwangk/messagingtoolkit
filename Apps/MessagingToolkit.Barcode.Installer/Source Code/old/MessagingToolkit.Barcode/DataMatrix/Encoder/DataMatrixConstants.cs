﻿namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal static class DataMatrixConstants
    {
        internal static readonly double DataMatrixAlmostZero = 0.000001;

        internal static readonly int DataMatrixModuleOff = 0x00;
        internal static readonly int DataMatrixModuleOnRed = 0x01;
        internal static readonly int DataMatrixModuleOnGreen = 0x02;
        internal static readonly int DataMatrixModuleOnBlue = 0x04;
        internal static readonly int DataMatrixModuleOnRGB = 0x07; /* OnRed | OnGreen | OnBlue */
        internal static readonly int DataMatrixModuleOn = 0x07;
        internal static readonly int DataMatrixModuleUnsure = 0x08;
        internal static readonly int DataMatrixModuleAssigned = 0x10;
        internal static readonly int DataMatrixModuleVisited = 0x20;
        internal static readonly int DmtxModuleData = 0x40;

        internal static readonly byte DataMatrixCharAsciiPad = 129;
        internal static readonly byte DataMatrixCharAsciiUpperShift = 235;
        internal static readonly byte DataMatrixCharTripletShift1 = 0;
        internal static readonly byte DataMatrixCharTripletShift2 = 1;
        internal static readonly byte DataMatrixCharTripletShift3 = 2;
        internal static readonly byte DataMatrixCharFNC1 = 232;
        internal static readonly byte DataMatrixCharStructuredAppend = 233;
        internal static readonly byte DataMatrixChar05Macro = 236;
        internal static readonly byte DataMatrixChar06Macro = 237;

        internal static readonly int DataMatrixC40TextBasicSet = 0;
        internal static readonly int DataMatrixC40TextShift1 = 1;
        internal static readonly int DataMatrixC40TextShift2 = 2;
        internal static readonly int DataMatrixC40TextShift3 = 3;

        internal static readonly int DataMatrixCharTripletUnlatch = 254;
        internal static readonly int DataMatrixCharEdifactUnlatch = 31;

        internal static readonly byte DataMatrixCharC40Latch = 230;
        internal static readonly byte DataMatrixCharTextLatch = 239;
        internal static readonly byte DataMatrixCharX12Latch = 238;
        internal static readonly byte DataMatrixCharEdifactLatch = 240;
        internal static readonly byte DataMatrixCharBase256Latch = 231;

        internal static readonly int[] SymbolRows = new int[] { 10, 12, 14, 16, 18, 20,  22,  24,  26,
                                                 32, 36, 40,  44,  48,  52,
                                                 64, 72, 80,  88,  96, 104,
                                                        120, 132, 144,
                                                  8,  8, 12,  12,  16,  16 };

        internal static readonly int[] SymbolCols = new int[] { 10, 12, 14, 16, 18, 20,  22,  24,  26,
                                                 32, 36, 40,  44,  48,  52,
                                                 64, 72, 80,  88,  96, 104,
                                                        120, 132, 144,
                                                 18, 32, 26,  36,  36,  48 };

        internal static readonly int[] DataRegionRows = new int[] { 8, 10, 12, 14, 16, 18, 20, 22, 24,
                                                    14, 16, 18, 20, 22, 24,
                                                    14, 16, 18, 20, 22, 24,
                                                            18, 20, 22,
                                                     6,  6, 10, 10, 14, 14 };

        internal static readonly int[] DataRegionCols = new int[] { 8, 10, 12, 14, 16, 18, 20, 22, 24,
                                                    14, 16, 18, 20, 22, 24,
                                                    14, 16, 18, 20, 22, 24,
                                                            18, 20, 22,
                                                    16, 14, 24, 16, 16, 22 };

        internal static readonly int[] HorizDataRegions = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                                    2, 2, 2, 2, 2, 2,
                                                    4, 4, 4, 4, 4, 4,
                                                          6, 6, 6,
                                                    1, 2, 1, 2, 2, 2 };

        internal static readonly int[] InterleavedBlocks = new int[] { 1, 1, 1, 1, 1, 1, 1,  1, 1,
                                                     1, 1, 1, 1,  1, 2,
                                                     2, 4, 4, 4,  4, 6,
                                                           6, 8, 10,
                                                     1, 1, 1, 1,  1, 1 };

        internal static readonly int[] SymbolDataWords = new int[] { 3, 5, 8,  12,   18,   22,   30,   36,  44,
                                                    62,   86,  114,  144,  174, 204,
                                                   280,  368,  456,  576,  696, 816,
                                                              1050, 1304, 1558,
                                                     5,   10,   16,   22,   32,  49 };

        internal static readonly int[] BlockErrorWords = new int[] { 5, 7, 10, 12, 14, 18, 20, 24, 28,
                                                    36, 42, 48, 56, 68, 42,
                                                    56, 36, 48, 56, 68, 56,
                                                            68, 62, 62,
                                                     7, 11, 14, 18, 24, 28 };

        internal static readonly int[] BlockMaxCorrectable = new int[] { 2, 3, 5,  6,  7,  9,  10,  12,  14,
                                                       18, 21, 24,  28,  34,  21,
                                                       28, 18, 24,  28,  34,  28,
                                                               34,  31,  31,
                                                   3,  5,  7,   9,  12,  14 };
        internal static readonly int DataMatrixSymbolSquareCount = 24;
        internal static readonly int DataMatrixSymbolRectCount = 6;
        internal static readonly int DataMatrixUndefined = -1;

        internal static readonly int[] DataMatrixPatternX = new int[] { -1, 0, 1, 1, 1, 0, -1, -1 };
        internal static readonly int[] DataMatrixPatternY = new int[] { -1, -1, -1, 0, 1, 1, 1, 0 };
        internal static readonly DataMatrixPointFlow DataMatrixBlankEdge = new DataMatrixPointFlow() { Plane = 0, Arrive = 0, Depart = 0, Mag = DataMatrixConstants.DataMatrixUndefined, Loc = new DataMatrixPixelLoc() { X = -1, Y = -1 } };

        internal static readonly int DataMatrixHoughRes = 180;
        internal static readonly int DataMatrixNeighborNone = 8;

        internal static readonly int[] rHvX =
    {  256,  256,  256,  256,  255,  255,  255,  254,  254,  253,  252,  251,  250,  249,  248,
       247,  246,  245,  243,  242,  241,  239,  237,  236,  234,  232,  230,  228,  226,  224,
       222,  219,  217,  215,  212,  210,  207,  204,  202,  199,  196,  193,  190,  187,  184,
       181,  178,  175,  171,  168,  165,  161,  158,  154,  150,  147,  143,  139,  136,  132,
       128,  124,  120,  116,  112,  108,  104,  100,   96,   92,   88,   83,   79,   75,   71,
        66,   62,   58,   53,   49,   44,   40,   36,   31,   27,   22,   18,   13,    9,    4,
         0,   -4,   -9,  -13,  -18,  -22,  -27,  -31,  -36,  -40,  -44,  -49,  -53,  -58,  -62,
       -66,  -71,  -75,  -79,  -83,  -88,  -92,  -96, -100, -104, -108, -112, -116, -120, -124,
      -128, -132, -136, -139, -143, -147, -150, -154, -158, -161, -165, -168, -171, -175, -178,
      -181, -184, -187, -190, -193, -196, -199, -202, -204, -207, -210, -212, -215, -217, -219,
      -222, -224, -226, -228, -230, -232, -234, -236, -237, -239, -241, -242, -243, -245, -246,
      -247, -248, -249, -250, -251, -252, -253, -254, -254, -255, -255, -255, -256, -256, -256 };

        internal static readonly int[] rHvY =
    {    0,    4,    9,   13,   18,   22,   27,   31,   36,   40,   44,   49,   53,   58,   62,
        66,   71,   75,   79,   83,   88,   92,   96,  100,  104,  108,  112,  116,  120,  124,
       128,  132,  136,  139,  143,  147,  150,  154,  158,  161,  165,  168,  171,  175,  178,
       181,  184,  187,  190,  193,  196,  199,  202,  204,  207,  210,  212,  215,  217,  219,
       222,  224,  226,  228,  230,  232,  234,  236,  237,  239,  241,  242,  243,  245,  246,
       247,  248,  249,  250,  251,  252,  253,  254,  254,  255,  255,  255,  256,  256,  256,
       256,  256,  256,  256,  255,  255,  255,  254,  254,  253,  252,  251,  250,  249,  248,
       247,  246,  245,  243,  242,  241,  239,  237,  236,  234,  232,  230,  228,  226,  224,
       222,  219,  217,  215,  212,  210,  207,  204,  202,  199,  196,  193,  190,  187,  184,
       181,  178,  175,  171,  168,  165,  161,  158,  154,  150,  147,  143,  139,  136,  132,
       128,  124,  120,  116,  112,  108,  104,  100,   96,   92,   88,   83,   79,   75,   71,
        66,   62,   58,   53,   49,   44,   40,   36,   31,   27,   22,   18,   13,    9,    4 };

        internal static readonly int[] aLogVal =   
        {   1,   2,   4,   8,  16,  32,  64, 128,  45,  90, 180,  69, 138,  57, 114, 228,
     229, 231, 227, 235, 251, 219, 155,  27,  54, 108, 216, 157,  23,  46,  92, 184,
      93, 186,  89, 178,  73, 146,   9,  18,  36,  72, 144,  13,  26,  52, 104, 208,
     141,  55, 110, 220, 149,   7,  14,  28,  56, 112, 224, 237, 247, 195, 171, 123,
     246, 193, 175, 115, 230, 225, 239, 243, 203, 187,  91, 182,  65, 130,  41,  82,
     164, 101, 202, 185,  95, 190,  81, 162, 105, 210, 137,  63, 126, 252, 213, 135,
      35,  70, 140,  53, 106, 212, 133,  39,  78, 156,  21,  42,  84, 168, 125, 250,
     217, 159,  19,  38,  76, 152,  29,  58, 116, 232, 253, 215, 131,  43,  86, 172,
     117, 234, 249, 223, 147,  11,  22,  44,  88, 176,  77, 154,  25,  50, 100, 200,
     189,  87, 174, 113, 226, 233, 255, 211, 139,  59, 118, 236, 245, 199, 163, 107,
     214, 129,  47,  94, 188,  85, 170, 121, 242, 201, 191,  83, 166,  97, 194, 169,
     127, 254, 209, 143,  51, 102, 204, 181,  71, 142,  49,  98, 196, 165, 103, 206,
     177,  79, 158,  17,  34,  68, 136,  61, 122, 244, 197, 167,  99, 198, 161, 111,
     222, 145,  15,  30,  60, 120, 240, 205, 183,  67, 134,  33,  66, 132,  37,  74,
     148,   5,  10,  20,  40,  80, 160, 109, 218, 153,  31,  62, 124, 248, 221, 151,
       3,   6,  12,  24,  48,  96, 192, 173, 119, 238, 241, 207, 179,  75, 150,   1 };

        internal static readonly int[] logVal =
   {-255, 255,   1, 240,   2, 225, 241,  53,   3,  38, 226, 133, 242,  43,  54, 210,
       4, 195,  39, 114, 227, 106, 134,  28, 243, 140,  44,  23,  55, 118, 211, 234,
       5, 219, 196,  96,  40, 222, 115, 103, 228,  78, 107, 125, 135,   8,  29, 162,
     244, 186, 141, 180,  45,  99,  24,  49,  56,  13, 119, 153, 212, 199, 235,  91,
       6,  76, 220, 217, 197,  11,  97, 184,  41,  36, 223, 253, 116, 138, 104, 193,
     229,  86,  79, 171, 108, 165, 126, 145, 136,  34,   9,  74,  30,  32, 163,  84,
     245, 173, 187, 204, 142,  81, 181, 190,  46,  88, 100, 159,  25, 231,  50, 207,
      57, 147,  14,  67, 120, 128, 154, 248, 213, 167, 200,  63, 236, 110,  92, 176,
       7, 161,  77, 124, 221, 102, 218,  95, 198,  90,  12, 152,  98,  48, 185, 179,
      42, 209,  37, 132, 224,  52, 254, 239, 117, 233, 139,  22, 105,  27, 194, 113,
     230, 206,  87, 158,  80, 189, 172, 203, 109, 175, 166,  62, 127, 247, 146,  66,
     137, 192,  35, 252,  10, 183,  75, 216,  31,  83,  33,  73, 164, 144,  85, 170,
     246,  65, 174,  61, 188, 202, 205, 157, 143, 169,  82,  72, 182, 215, 191, 251,
      47, 178,  89, 151, 101,  94, 160, 123,  26, 112, 232,  21,  51, 238, 208, 131,
      58,  69, 148,  18,  15,  16,  68,  17, 121, 149, 129,  19, 155,  59, 249,  70,
     214, 250, 168,  71, 201, 156,  64,  60, 237, 130, 111,  20,  93, 122, 177, 150 };

    }

    internal enum DataMatrixFormat
    {
        Matrix,
        Mosaic,
    }

    internal enum DataMatrixSymAttribute
    {
        SymAttribSymbolRows,
        SymAttribSymbolCols,
        SymAttribDataRegionRows,
        SymAttribDataRegionCols,
        SymAttribHorizDataRegions,
        SymAttribVertDataRegions,
        SymAttribMappingMatrixRows,
        SymAttribMappingMatrixCols,
        SymAttribInterleavedBlocks,
        SymAttribBlockErrorWords,
        SymAttribBlockMaxCorrectable,
        SymAttribSymbolDataWords,
        SymAttribSymbolErrorWords,
        SymAttribSymbolMaxCorrectable
    }

    public enum DataMatrixSymbolSize
    {
        SymbolRectAuto = -3,
        SymbolSquareAuto = -2,
        SymbolShapeAuto = -1,
        Symbol10x10 = 0,
        Symbol12x12,
        Symbol14x14,
        Symbol16x16,
        Symbol18x18,
        Symbol20x20,
        Symbol22x22,
        Symbol24x24,
        Symbol26x26,
        Symbol32x32,
        Symbol36x36,
        Symbol40x40,
        Symbol44x44,
        Symbol48x48,
        Symbol52x52,
        Symbol64x64,
        Symbol72x72,
        Symbol80x80,
        Symbol88x88,
        Symbol96x96,
        Symbol104x104,
        Symbol120x120,
        Symbol132x132,
        Symbol144x144,
        Symbol8x18,
        Symbol8x32,
        Symbol12x26,
        Symbol12x36,
        Symbol16x36,
        Symbol16x48
    }

    internal enum DataMatrixFlip
    {
        FlipNone = 0x00,
        FlipX = 0x01 << 0,
        FlipY = 0x01 << 1
    }

    internal enum DataMatrixPackOrder
    {
        /* Custom format */
        PackCustom = 100,
        /* 1 bpp */
        Pack1bppK = 200,
        /* 8 bpp grayscale */
        Pack8bppK = 300,
        /* 16 bpp formats */
        Pack16bppRGB = 400,
        Pack16bppRGBX,
        Pack16bppXRGB,
        Pack16bppBGR,
        Pack16bppBGRX,
        Pack16bppXBGR,
        Pack16bppYCbCr,
        /* 24 bpp formats */
        Pack24bppRGB = 500,
        Pack24bppBGR,
        Pack24bppYCbCr,
        /* 32 bpp formats */
        Pack32bppRGBX = 600,
        Pack32bppXRGB,
        Pack32bppBGRX,
        Pack32bppXBGR,
        Pack32bppCMYK
    }

    internal enum DataMatrixRange
    {
        RangeGood,
        RangeBad,
        RangeEnd
    }

    internal enum DataMatrixDirection
    {
        DirNone = 0x00,
        DirUp = 0x01 << 0,
        DirLeft = 0x01 << 1,
        DirDown = 0x01 << 2,
        DirRight = 0x01 << 3,
        DirHorizontal = DirLeft | DirRight,
        DirVertical = DirUp | DirDown,
        DirRightUp = DirRight | DirUp,
        DirLeftDown = DirLeft | DirDown
    }

    public enum DataMatrixScheme
    {
        SchemeAutoFast = -2,
        SchemeAutoBest = -1,
        SchemeAscii = 0,
        SchemeC40,
        SchemeText,
        SchemeX12,
        SchemeEdifact,
        SchemeBase256,
        SchemeAsciiGS1
    }

    internal enum DataMatrixMaskBit
    {
        MaskBit8 = 0x01 << 0,
        MaskBit7 = 0x01 << 1,
        MaskBit6 = 0x01 << 2,
        MaskBit5 = 0x01 << 3,
        MaskBit4 = 0x01 << 4,
        MaskBit3 = 0x01 << 5,
        MaskBit2 = 0x01 << 6,
        MaskBit1 = 0x01 << 7
    }

    internal enum DataMatrixEdge
    {
        EdgeTop = 0x01 << 0,
        EdgeBottom = 0x01 << 1,
        EdgeLeft = 0x01 << 2,
        EdgeRight = 0x01 << 3
    }

    enum DataMatrixChannelStatus
    {
        ChannelValid = 0x00,
        ChannelUnsupportedChar = 0x01 << 0,
        ChannelCannotUnlatch = 0x01 << 1
    }

    enum DmtxUnlatch
    {
        Explicit,
        Implicit
    }
}

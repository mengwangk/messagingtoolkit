using System;
using QRCodeDecoder = MessagingToolkit.QRCode.Codec.QRCodeDecoder;
using InvalidDataBlockException = MessagingToolkit.QRCode.ExceptionHandler.InvalidDataBlockException;
using DebugCanvas = MessagingToolkit.QRCode.Helper.DebugCanvas;
using SystemUtils = MessagingToolkit.QRCode.Helper.SystemUtils;

namespace MessagingToolkit.QRCode.Codec.Reader
{

    public class QRCodeDataBlockReader
    {
        virtual internal int NextMode
        {
            get
            {
                //canvas.println("data blocks:"+ (blocks.length - numErrorCorrectionCode));
                if ((blockPointer > blocks.Length - numErrorCorrectionCode - 2))
                    return 0;
                else
                    return GetNextBits(4);
            }
        }

        virtual public sbyte[] DataByte
        {
            get
            {
                canvas.Print("Reading data blocks.");
                System.IO.MemoryStream output = new System.IO.MemoryStream();

                try
                {
                    do
                    {
                        int mode = NextMode;
                        //canvas.println("mode: " + mode);
                        if (mode == 0)
                        {
                            if (output.Length > 0)
                                break;
                            else
                                throw new InvalidDataBlockException("Empty data block");
                        }
                        //if (mode != 1 && mode != 2 && mode != 4 && mode != 8)
                        //	break;
                        //}
                        if (mode != MODE_NUMBER && mode != MODE_ROMAN_AND_NUMBER && mode != MODE_8BIT_BYTE && mode != MODE_KANJI)
                        {
                            //canvas.Print("Invalid mode: " + mode);
                            mode = GuessMode(mode);

                            //canvas.println("Guessed mode: " + mode); */
                            //throw new InvalidDataBlockException("Invalid mode: " + mode + " in (block:" + blockPointer + " bit:" + bitPointer + ")");
                        }
                        dataLength = GetDataLength(mode);
                        if (dataLength < 1)
                            throw new InvalidDataBlockException("Invalid data length: " + dataLength);
                        //canvas.println("length: " + dataLength);
                        switch (mode)
                        {

                            case MODE_NUMBER:
                                //canvas.println("Mode: Figure");
                                sbyte[] temp_sbyteArray;
                                temp_sbyteArray = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(GetFigureString(dataLength)));
                                output.Write(SystemUtils.ToByteArray(temp_sbyteArray), 0, temp_sbyteArray.Length);
                                break;

                            case MODE_ROMAN_AND_NUMBER:
                                //canvas.println("Mode: Roman&Figure");
                                sbyte[] temp_sbyteArray2;
                                temp_sbyteArray2 = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(GetRomanAndFigureString(dataLength)));
                                output.Write(SystemUtils.ToByteArray(temp_sbyteArray2), 0, temp_sbyteArray2.Length);
                                break;

                            case MODE_8BIT_BYTE:
                                //canvas.println("Mode: 8bit Byte");
                                sbyte[] temp_sbyteArray3;
                                temp_sbyteArray3 = Get8bitByteArray(dataLength);
                                output.Write(SystemUtils.ToByteArray(temp_sbyteArray3), 0, temp_sbyteArray3.Length);
                                break;

                            case MODE_KANJI:
                                //canvas.println("Mode: Kanji");
                                sbyte[] temp_sbyteArray4;
                                temp_sbyteArray4 = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(GetKanjiString(dataLength)));
                                output.Write(SystemUtils.ToByteArray(temp_sbyteArray4), 0, temp_sbyteArray4.Length);
                                break;
                            case MODE_FNC1_FIRST_POSITION:
                                //canvas.println("Mode: FNC1 first position");
                                sbyte[] temp_sbyteArray5;
                                temp_sbyteArray5 = Get8bitByteArray(dataLength);
                                output.Write(SystemUtils.ToByteArray(temp_sbyteArray5), 0, temp_sbyteArray5.Length);
                                break;
                        }
                        //			
                        //canvas.println("DataLength: " + dataLength);
                        //Console.out.println(dataString);
                    }
                    while (true);
                }
                catch (IndexOutOfRangeException e)
                {
                    SystemUtils.WriteStackTrace(e, Console.Error);
                    throw new InvalidDataBlockException("Data Block Error in (block:" + blockPointer + " bit:" + bitPointer + ")");
                }
                catch (System.IO.IOException e)
                {
                    throw new InvalidDataBlockException(e.Message);
                }
                return SystemUtils.ToSByteArray(output.ToArray());
            }

        }
        virtual public String DataString
        {
            get
            {
                canvas.Print("Reading data blocks...");
                String dataString = "";
                do
                {
                    int mode = NextMode;
                    canvas.Print("mode: " + mode);
                    if (mode == 0)
                        break;
                    //if (mode != 1 && mode != 2 && mode != 4 && mode != 8)
                    //	break;
                    //}
                    if (mode != MODE_NUMBER && mode != MODE_ROMAN_AND_NUMBER && mode != MODE_8BIT_BYTE && mode != MODE_KANJI)
                    {
                        mode = GuessMode(mode);
                        // do not guesswork
                        //Console.out.println("guessed mode: " + mode);
                    }

                    dataLength = GetDataLength(mode);
                    canvas.Print(System.Convert.ToString(blocks[blockPointer]));
                    System.Console.Out.WriteLine("length: " + dataLength);
                    switch (mode)
                    {

                        case MODE_NUMBER:
                            //canvas.println("Mode: Figure");
                            dataString += GetFigureString(dataLength);
                            break;

                        case MODE_ROMAN_AND_NUMBER:
                            //canvas.println("Mode: Roman&Figure");
                            dataString += GetRomanAndFigureString(dataLength);
                            break;

                        case MODE_8BIT_BYTE:
                            //canvas.println("Mode: 8bit Byte");
                            dataString += Get8bitByteString(dataLength);
                            break;

                        case MODE_KANJI:
                            //canvas.println("Mode: Kanji");
                            dataString += GetKanjiString(dataLength);
                            break;

                        case MODE_FNC1_FIRST_POSITION:
                            //canvas.println("Mode: 8bit Byte");
                            dataString += Get8bitByteString(dataLength);
                            break;
                    }
                    //canvas.println("DataLength: " + dataLength);
                    //Console.out.println(dataString);
                }
                while (true);
                System.Console.Out.WriteLine("");
                return dataString;
            }

        }
        internal int[] blocks;
        internal int dataLengthMode;
        internal int blockPointer;
        internal int bitPointer;
        internal int dataLength;
        internal int numErrorCorrectionCode;
        internal DebugCanvas canvas;

        const int MODE_NUMBER = 1;
        const int MODE_ROMAN_AND_NUMBER = 2;
        const int MODE_8BIT_BYTE = 4;
        const int MODE_KANJI = 8;
        const int MODE_FNC1_FIRST_POSITION = 5;
        const int MODE_FNC1_SECOND_POSITION = 9;

        int[][] sizeOfDataLengthInfo = new int[][] { new int[] { 10, 9, 8, 8 }, new int[] { 12, 11, 16, 10 }, new int[] { 14, 13, 16, 12 } };

        public QRCodeDataBlockReader(int[] blocks, int version, int numErrorCorrectionCode)
        {
            blockPointer = 0;
            bitPointer = 7;
            dataLength = 0;
            this.blocks = blocks;
            this.numErrorCorrectionCode = numErrorCorrectionCode;
            if (version <= 9)
                dataLengthMode = 0;
            else if (version >= 10 && version <= 26)
                dataLengthMode = 1;
            else if (version >= 27 && version <= 40)
                dataLengthMode = 2;
            canvas = QRCodeDecoder.Canvas;
        }

        internal virtual int GetNextBits(int numBits)
        {

            int bits = 0;
            if (numBits < bitPointer + 1)
            {
                // next word fits into current data block
                int mask = 0;
                for (int i = 0; i < numBits; i++)
                {
                    mask += (1 << i);
                }
                mask <<= (bitPointer - numBits + 1);

                bits = (blocks[blockPointer] & mask) >> (bitPointer - numBits + 1);
                bitPointer -= numBits;
                return bits;
            }
            else if (numBits < bitPointer + 1 + 8)
            {
                // next word crosses 2 data blocks
                int mask1 = 0;
                for (int i = 0; i < bitPointer + 1; i++)
                {
                    mask1 += (1 << i);
                }

                if (blockPointer < blocks.Length)
                {
                    bits = (blocks[blockPointer] & mask1) << (numBits - (bitPointer + 1));
                }

                blockPointer++;

                if (blockPointer < blocks.Length)
                {
                    bits += ((blocks[blockPointer]) >> (8 - (numBits - (bitPointer + 1))));
                }

                bitPointer = bitPointer - numBits % 8;

                if (bitPointer < 0)
                {
                    bitPointer = 8 + bitPointer;
                }
                return bits;
            }
            else if (numBits < bitPointer + 1 + 16)
            {
                // next word crosses 3 data blocks
                int mask1 = 0; // mask of first block
                int mask3 = 0; // mask of 3rd block
                //bitPointer + 1 : number of bits of the 1st block
                //8 : number of the 2nd block (note that use already 8bits because next word uses 3 data blocks)
                //numBits - (bitPointer + 1 + 8) : number of bits of the 3rd block 
                for (int i = 0; i < bitPointer + 1; i++)
                {
                    mask1 += (1 << i);
                }

                int bitsFirstBlock = 0;
                int bitsSecondBlock = 0;
                if (blockPointer < blocks.Length)
                {
                    bitsFirstBlock = (blocks[blockPointer] & mask1) << (numBits - (bitPointer + 1));
                    blockPointer++;

                    bitsSecondBlock = blocks[blockPointer] << (numBits - (bitPointer + 1 + 8));
                    blockPointer++;
                }

                for (int i = 0; i < numBits - (bitPointer + 1 + 8); i++)
                {
                    mask3 += (1 << i);
                }
                mask3 <<= 8 - (numBits - (bitPointer + 1 + 8));


                int bitsThirdBlock = 0;
                if (blockPointer < blocks.Length)
                {
                    bitsThirdBlock = (blocks[blockPointer] & mask3) >> (8 - (numBits - (bitPointer + 1 + 8)));
                }

                bits = bitsFirstBlock + bitsSecondBlock + bitsThirdBlock;
                bitPointer = bitPointer - (numBits - 8) % 8;
                if (bitPointer < 0)
                {
                    bitPointer = 8 + bitPointer;
                }
                return bits;
            }
            else
            {
                System.Console.Out.WriteLine("ERROR!");
                return 0;
            }


        }

        internal virtual int GuessMode(int mode)
        {
            //correct modes: 0001 0010 0100 1000
            //possible data: 0000 0011 0101 1001 0110 1010 1100
            //               0111 1101 1011 1110 1111
            //		MODE_NUMBER = 1;
            //		MODE_ROMAN_AND_NUMBER = 2;
            //		MODE_8BIT_BYTE = 4;
            //		MODE_KANJI = 8;
            switch (mode)
            {

                case 3:
                    return MODE_NUMBER;

                case 5:
                    return MODE_8BIT_BYTE;

                case 6:
                    return MODE_8BIT_BYTE;

                case 7:
                    return MODE_8BIT_BYTE;

                case 9:
                    return MODE_KANJI;

                case 10:
                    return MODE_KANJI;

                case 11:
                    return MODE_KANJI;

                case 12:
                    return MODE_8BIT_BYTE;

                case 13:
                    return MODE_8BIT_BYTE;

                case 14:
                    return MODE_8BIT_BYTE;

                case 15:
                    return MODE_8BIT_BYTE;

                default:
                    return MODE_KANJI;

            }
        }

        internal virtual int GetDataLength(int modeIndicator)
        {
            int index = 0;
            while (true)
            {
                if ((modeIndicator >> index) == 1)
                    break;
                index++;
            }

            return GetNextBits(sizeOfDataLengthInfo[dataLengthMode][index]);
        }


        internal virtual String GetFigureString(int dataLength)
        {
            int length = dataLength;
            int intData = 0;
            String strData = "";
            do
            {
                if (length >= 3)
                {
                    intData = GetNextBits(10);
                    if (intData < 100)
                        strData += "0";
                    if (intData < 10)
                        strData += "0";
                    length -= 3;
                }
                else if (length == 2)
                {
                    intData = GetNextBits(7);
                    if (intData < 10)
                        strData += "0";
                    length -= 2;
                }
                else if (length == 1)
                {
                    intData = GetNextBits(4);
                    length -= 1;
                }
                strData += System.Convert.ToString(intData);
            }
            while (length > 0);

            return strData;
        }

        internal virtual String GetRomanAndFigureString(int dataLength)
        {
            int length = dataLength;
            int intData = 0;
            String strData = "";
            char[] tableRomanAndFigure = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ', '$', '%', '*', '+', '-', '.', '/', ':' };
            do
            {
                if (length > 1)
                {
                    intData = GetNextBits(11);
                    int firstLetter = intData / 45;
                    int secondLetter = intData % 45;
                    strData += System.Convert.ToString(tableRomanAndFigure[firstLetter]);
                    strData += System.Convert.ToString(tableRomanAndFigure[secondLetter]);
                    length -= 2;
                }
                else if (length == 1)
                {
                    intData = GetNextBits(6);
                    strData += System.Convert.ToString(tableRomanAndFigure[intData]);
                    length -= 1;
                }
            }
            while (length > 0);

            return strData;
        }

        public virtual sbyte[] Get8bitByteArray(int dataLength)
        {
            int length = dataLength;
            int intData = 0;
            System.IO.MemoryStream output = new System.IO.MemoryStream();

            do
            {
                canvas.Print("Length: " + length);
                intData = GetNextBits(8);
                output.WriteByte((byte)intData);
                length--;
            }
            while (length > 0);
            return SystemUtils.ToSByteArray(output.ToArray());
        }

        internal virtual String Get8bitByteString(int dataLength)
        {
            int length = dataLength;
            int intData = 0;
            String strData = "";
            do
            {
                intData = GetNextBits(8);
                strData += (char)intData;
                length--;
            }
            while (length > 0);
            return strData;
        }


        internal virtual String GetKanjiString(int dataLength)
        {
            int length = dataLength;
            int intData = 0;
            String unicodeString = "";
            do
            {
                intData = GetNextBits(13);
                int lowerByte = intData % 0xC0;
                int higherByte = intData / 0xC0;

                int tempWord = (higherByte << 8) + lowerByte;
                int shiftjisWord = 0;
                if (tempWord + 0x8140 <= 0x9FFC)
                {
                    // between 8140 - 9FFC on Shift_JIS character set
                    shiftjisWord = tempWord + 0x8140;
                }
                else
                {
                    // between E040 - EBBF on Shift_JIS character set
                    shiftjisWord = tempWord + 0xC140;
                }

                sbyte[] tempByte = new sbyte[2];
                tempByte[0] = (sbyte)(shiftjisWord >> 8);
                tempByte[1] = (sbyte)(shiftjisWord & 0xFF);
                unicodeString += new String(SystemUtils.ToCharArray(SystemUtils.ToByteArray(tempByte)));
                length--;
            }
            while (length > 0);


            return unicodeString;
        }
    }
}
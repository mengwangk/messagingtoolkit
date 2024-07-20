using MessagingToolkit.Barcode.Common;
using System;
using System.Text;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    public sealed class GeneralAppIdDecoder
    {

        private readonly BitArray information;
        private readonly CurrentParsingState current;
        private readonly StringBuilder buffer;

        public GeneralAppIdDecoder(BitArray information)
        {
            this.current = new CurrentParsingState();
            this.buffer = new StringBuilder();
            this.information = information;
        }

        internal String DecodeAllCodes(StringBuilder buff, int initialPosition)
        {
            int currentPosition = initialPosition;
            String remaining = null;
            do
            {
                DecodedInformation info = this.DecodeGeneralPurposeField(
                        currentPosition, remaining);
                String parsedFields = MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.FieldParser.ParseFieldsInGeneralPurpose(info
                        .NewString);
                buff.Append(parsedFields);
                if (info.IsRemaining)
                {
                    remaining = info.RemainingValue.ToString();
                }
                else
                {
                    remaining = null;
                }

                if (currentPosition == info.NewPosition)
                {// No step forward!
                    break;
                }
                currentPosition = info.NewPosition;
            } while (true);

            return buff.ToString();
        }

        private bool IsStillNumeric(int pos)
        {
            // It's numeric if it still has 7 positions
            // and one of the first 4 bits is "1".
            if (pos + 7 > this.information.Size)
            {
                return pos + 4 <= this.information.Size;
            }

            for (int i = pos; i < pos + 3; ++i)
            {
                if (this.information.Get(i))
                {
                    return true;
                }
            }

            return this.information.Get(pos + 3);
        }

        private DecodedNumeric DecodeNumeric(int pos)
        {
            if (pos + 7 > this.information.Size)
            {
                int numeric = ExtractNumericValueFromBitArray(pos, 4);
                if (numeric == 0)
                {
                    return new DecodedNumeric(this.information.Size,
                            MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.DecodedNumeric.Fnc1, MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.DecodedNumeric.Fnc1);
                }
                return new DecodedNumeric(this.information.Size, numeric - 1,
                        MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.DecodedNumeric.Fnc1);
            }
            int numeric_0 = ExtractNumericValueFromBitArray(pos, 7);

            int digit1 = (numeric_0 - 8) / 11;
            int digit2 = (numeric_0 - 8) % 11;

            return new DecodedNumeric(pos + 7, digit1, digit2);
        }

        internal int ExtractNumericValueFromBitArray(int pos, int bits)
        {
            return ExtractNumericValueFromBitArray(this.information, pos, bits);
        }

        static internal int ExtractNumericValueFromBitArray(BitArray information_0, int pos,
                int bits)
        {
            if (bits > 32)
            {
                throw new ArgumentException(
                        "extractNumberValueFromBitArray can't handle more than 32 bits");
            }

            int val = 0;
            for (int i = 0; i < bits; ++i)
            {
                if (information_0.Get(pos + i))
                {
                    val |= (1 << (bits - i - 1));
                }
            }

            return val;
        }

        internal DecodedInformation DecodeGeneralPurposeField(int pos, String remaining)
        {
            this.buffer.Length = 0;

            if (remaining != null)
            {
                this.buffer.Append(remaining);
            }

            this.current.position = pos;

            DecodedInformation lastDecoded = ParseBlocks();
            if (lastDecoded != null && lastDecoded.IsRemaining)
            {
                return new DecodedInformation(this.current.position,
                        this.buffer.ToString(), lastDecoded.RemainingValue);
            }
            return new DecodedInformation(this.current.position,
                    this.buffer.ToString());
        }

        private DecodedInformation ParseBlocks()
        {
            bool isFinished;
            BlockParsedResult result;
            do
            {
                int initialPosition = current.position;

                if (current.IsAlpha)
                {
                    result = ParseAlphaBlock();
                    isFinished = result.IsFinished;
                }
                else if (current.IsIsoIec646)
                {
                    result = ParseIsoIec646Block();
                    isFinished = result.IsFinished;
                }
                else
                { // it must be numeric
                    result = ParseNumericBlock();
                    isFinished = result.IsFinished;
                }

                bool positionChanged = initialPosition != current.position;
                if (!positionChanged && !isFinished)
                {
                    break;
                }
            } while (!isFinished);

            return result.DecodedInformation;
        }

        private BlockParsedResult ParseNumericBlock()
        {
            while (IsStillNumeric(current.position))
            {
                DecodedNumeric numeric = DecodeNumeric(current.position);
                current.position = numeric.NewPosition;

                if (numeric.IsFirstDigitFnc1())
                {
                    DecodedInformation information_0;
                    if (numeric.IsSecondDigitFnc1())
                    {
                        information_0 = new DecodedInformation(current.position,
                                buffer.ToString());
                    }
                    else
                    {
                        information_0 = new DecodedInformation(current.position,
                                buffer.ToString(), numeric.SecondDigit);
                    }
                    return new BlockParsedResult(information_0, true);
                }
                buffer.Append(numeric.FirstDigit);

                if (numeric.IsSecondDigitFnc1())
                {
                    DecodedInformation information_1 = new DecodedInformation(
                            current.position, buffer.ToString());
                    return new BlockParsedResult(information_1, true);
                }
                buffer.Append(numeric.SecondDigit);
            }

            if (IsNumericToAlphaNumericLatch(current.position))
            {
                current.SetAlpha();
                current.position += 4;
            }
            return new BlockParsedResult(false);
        }

        private BlockParsedResult ParseIsoIec646Block()
        {
            while (IsStillIsoIec646(current.position))
            {
                DecodedChar iso = DecodeIsoIec646(current.position);
                current.position = iso.NewPosition;

                if (iso.IsFnc1)
                {
                    DecodedInformation information_0 = new DecodedInformation(
                            current.position, buffer.ToString());
                    return new BlockParsedResult(information_0, true);
                }
                buffer.Append(iso.Value);
            }

            if (IsAlphaOr646ToNumericLatch(current.position))
            {
                current.position += 3;
                current.SetNumeric();
            }
            else if (IsAlphaTo646ToAlphaLatch(current.position))
            {
                if (current.position + 5 < this.information.Size)
                {
                    current.position += 5;
                }
                else
                {
                    current.position = this.information.Size;
                }

                current.SetAlpha();
            }
            return new BlockParsedResult(false);
        }

        private BlockParsedResult ParseAlphaBlock()
        {
            while (IsStillAlpha(current.position))
            {
                DecodedChar alpha = DecodeAlphanumeric(current.position);
                current.position = alpha.NewPosition;

                if (alpha.IsFnc1)
                {
                    DecodedInformation information = new DecodedInformation(
                            current.position, buffer.ToString());
                    return new BlockParsedResult(information, true); //end of the char block
                }

                buffer.Append(alpha.Value);
            }

            if (IsAlphaOr646ToNumericLatch(current.position))
            {
                current.position += 3;
                current.SetNumeric();
            }
            else if (IsAlphaTo646ToAlphaLatch(current.position))
            {
                if (current.position + 5 < this.information.Size)
                {
                    current.position += 5;
                }
                else
                {
                    current.position = this.information.Size;
                }

                current.SetIsoIec646();
            }
            return new BlockParsedResult(false);
        }

        private bool IsStillIsoIec646(int pos)
        {
            if (pos + 5 > this.information.Size)
            {
                return false;
            }

            int fiveBitValue = ExtractNumericValueFromBitArray(pos, 5);
            if (fiveBitValue >= 5 && fiveBitValue < 16)
            {
                return true;
            }

            if (pos + 7 > this.information.Size)
            {
                return false;
            }

            int sevenBitValue = ExtractNumericValueFromBitArray(pos, 7);
            if (sevenBitValue >= 64 && sevenBitValue < 116)
            {
                return true;
            }

            if (pos + 8 > this.information.Size)
            {
                return false;
            }

            int eightBitValue = ExtractNumericValueFromBitArray(pos, 8);
            return eightBitValue >= 232 && eightBitValue < 253;

        }

        private DecodedChar DecodeIsoIec646(int pos)
        {
            int fiveBitValue = ExtractNumericValueFromBitArray(pos, 5);
            if (fiveBitValue == 15)
            {
                return new DecodedChar(pos + 5, MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.DecodedChar.Fnc1);
            }

            if (fiveBitValue >= 5 && fiveBitValue < 15)
            {
                return new DecodedChar(pos + 5, (char)('0' + fiveBitValue - 5));
            }

            int sevenBitValue = ExtractNumericValueFromBitArray(pos, 7);

            if (sevenBitValue >= 64 && sevenBitValue < 90)
            {
                return new DecodedChar(pos + 7, (char)(sevenBitValue + 1));
            }

            if (sevenBitValue >= 90 && sevenBitValue < 116)
            {
                return new DecodedChar(pos + 7, (char)(sevenBitValue + 7));
            }

            int eightBitValue = ExtractNumericValueFromBitArray(pos, 8);
            switch (eightBitValue)
            {
                case 232:
                    return new DecodedChar(pos + 8, '!');
                case 233:
                    return new DecodedChar(pos + 8, '"');
                case 234:
                    return new DecodedChar(pos + 8, '%');
                case 235:
                    return new DecodedChar(pos + 8, '&');
                case 236:
                    return new DecodedChar(pos + 8, '\'');
                case 237:
                    return new DecodedChar(pos + 8, '(');
                case 238:
                    return new DecodedChar(pos + 8, ')');
                case 239:
                    return new DecodedChar(pos + 8, '*');
                case 240:
                    return new DecodedChar(pos + 8, '+');
                case 241:
                    return new DecodedChar(pos + 8, ',');
                case 242:
                    return new DecodedChar(pos + 8, '-');
                case 243:
                    return new DecodedChar(pos + 8, '.');
                case 244:
                    return new DecodedChar(pos + 8, '/');
                case 245:
                    return new DecodedChar(pos + 8, ':');
                case 246:
                    return new DecodedChar(pos + 8, ';');
                case 247:
                    return new DecodedChar(pos + 8, '<');
                case 248:
                    return new DecodedChar(pos + 8, '=');
                case 249:
                    return new DecodedChar(pos + 8, '>');
                case 250:
                    return new DecodedChar(pos + 8, '?');
                case 251:
                    return new DecodedChar(pos + 8, '_');
                case 252:
                    return new DecodedChar(pos + 8, ' ');
            }

            throw new Exception("Decoding invalid ISO/IEC 646 value: "
                    + eightBitValue);
        }

        private bool IsStillAlpha(int pos)
        {
            if (pos + 5 > this.information.Size)
            {
                return false;
            }

            // We now check if it's a valid 5-bit value (0..9 and FNC1)
            int fiveBitValue = ExtractNumericValueFromBitArray(pos, 5);
            if (fiveBitValue >= 5 && fiveBitValue < 16)
            {
                return true;
            }

            if (pos + 6 > this.information.Size)
            {
                return false;
            }

            int sixBitValue = ExtractNumericValueFromBitArray(pos, 6);
            return sixBitValue >= 16 && sixBitValue < 63; // 63 not included
        }

        private DecodedChar DecodeAlphanumeric(int pos)
        {
            int fiveBitValue = ExtractNumericValueFromBitArray(pos, 5);
            if (fiveBitValue == 15)
            {
                return new DecodedChar(pos + 5, MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.DecodedChar.Fnc1);
            }

            if (fiveBitValue >= 5 && fiveBitValue < 15)
            {
                return new DecodedChar(pos + 5, (char)('0' + fiveBitValue - 5));
            }

            int sixBitValue = ExtractNumericValueFromBitArray(pos, 6);

            if (sixBitValue >= 32 && sixBitValue < 58)
            {
                return new DecodedChar(pos + 6, (char)(sixBitValue + 33));
            }

            switch (sixBitValue)
            {
                case 58:
                    return new DecodedChar(pos + 6, '*');
                case 59:
                    return new DecodedChar(pos + 6, ',');
                case 60:
                    return new DecodedChar(pos + 6, '-');
                case 61:
                    return new DecodedChar(pos + 6, '.');
                case 62:
                    return new DecodedChar(pos + 6, '/');
            }

            throw new Exception("Decoding invalid alphanumeric value: "
                    + sixBitValue);
        }

        private bool IsAlphaTo646ToAlphaLatch(int pos)
        {
            if (pos + 1 > this.information.Size)
            {
                return false;
            }

            for (int i = 0; i < 5 && i + pos < this.information.Size; ++i)
            {
                if (i == 2)
                {
                    if (!this.information.Get(pos + 2))
                    {
                        return false;
                    }
                }
                else if (this.information.Get(pos + i))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsAlphaOr646ToNumericLatch(int pos)
        {
            // Next is alphanumeric if there are 3 positions and they are all zeros
            if (pos + 3 > this.information.Size)
            {
                return false;
            }

            for (int i = pos; i < pos + 3; ++i)
            {
                if (this.information.Get(i))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsNumericToAlphaNumericLatch(int pos)
        {
            // Next is alphanumeric if there are 4 positions and they are all zeros, or
            // if there is a subset of this just before the end of the symbol
            if (pos + 1 > this.information.Size)
            {
                return false;
            }

            for (int i = 0; i < 4 && i + pos < this.information.Size; ++i)
            {
                if (this.information.Get(pos + i))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

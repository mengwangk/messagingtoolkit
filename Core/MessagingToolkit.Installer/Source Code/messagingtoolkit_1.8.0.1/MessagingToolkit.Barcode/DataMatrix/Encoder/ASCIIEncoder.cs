using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class ASCIIEncoder : Encoder
    {
        public virtual int EncodingMode
        {
            get
            {
                return HighLevelEncoder.ASCII_ENCODATION;
            }
        }

        public virtual void Encode(EncoderContext context)
        {
            // step B
            int n = HighLevelEncoder.DetermineConsecutiveDigitCount(context.Message, context.pos);
            if (n >= 2)
            {
                context.WriteCodeword(encodeASCIIDigits(context.Message[context.pos], context.Message[context.pos + 1]));
                context.pos += 2;
            }
            else
            {
                char c = context.CurrentChar;
                int newMode = HighLevelEncoder.LookAheadTest(context.Message, context.pos, EncodingMode);
                if (newMode != EncodingMode)
                {
                    switch (newMode)
                    {
                        case HighLevelEncoder.BASE256_ENCODATION:
                            context.WriteCodeword(HighLevelEncoder.LATCH_TO_BASE256);
                            context.SignalEncoderChange(HighLevelEncoder.BASE256_ENCODATION);
                            return;
                        case HighLevelEncoder.C40_ENCODATION:
                            context.WriteCodeword(HighLevelEncoder.LATCH_TO_C40);
                            context.SignalEncoderChange(HighLevelEncoder.C40_ENCODATION);
                            return;
                        case HighLevelEncoder.X12_ENCODATION:
                            context.WriteCodeword(HighLevelEncoder.LATCH_TO_ANSIX12);
                            context.SignalEncoderChange(HighLevelEncoder.X12_ENCODATION);
                            break;
                        case HighLevelEncoder.TEXT_ENCODATION:
                            context.WriteCodeword(HighLevelEncoder.LATCH_TO_TEXT);
                            context.SignalEncoderChange(HighLevelEncoder.TEXT_ENCODATION);
                            break;
                        case HighLevelEncoder.EDIFACT_ENCODATION:
                            context.WriteCodeword(HighLevelEncoder.LATCH_TO_EDIFACT);
                            context.SignalEncoderChange(HighLevelEncoder.EDIFACT_ENCODATION);
                            break;
                        default:
                            throw new ArgumentException("Illegal mode: " + newMode);
                    }
                }
                else if (HighLevelEncoder.IsExtendedASCII(c))
                {
                    context.WriteCodeword(HighLevelEncoder.UPPER_SHIFT);
                    context.WriteCodeword((char)(c - 128 + 1));
                    context.pos++;
                }
                else
                {
                    context.WriteCodeword((char)(c + 1));
                    context.pos++;
                }

            }
        }

        private static char encodeASCIIDigits(char digit1, char digit2)
        {
            if (HighLevelEncoder.IsDigit(digit1) && HighLevelEncoder.IsDigit(digit2))
            {
                int num = (digit1 - 48) * 10 + (digit2 - 48);
                return (char)(num + 130);
            }
            throw new System.ArgumentException("not digits: " + digit1 + digit2);
        }

    }
}

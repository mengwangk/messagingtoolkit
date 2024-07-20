using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal sealed class X12Encoder : C40Encoder
    {

        public override int EncodingMode
        {
            get
            {
                return HighLevelEncoder.X12_ENCODATION;
            }
        }

        public override void Encode(EncoderContext context)
        {
            //step C
            StringBuilder buffer = new StringBuilder();
            while (context.HasMoreCharacters())
            {
                char c = context.CurrentChar;
                context.pos++;

                EncodeChar(c, buffer);

                int count = buffer.Length;
                if ((count % 3) == 0)
                {
                    WriteNextTriplet(context, buffer);

                    int newMode = HighLevelEncoder.LookAheadTest(context.msg, context.pos, EncodingMode);
                    if (newMode != EncodingMode)
                    {
                        context.SignalEncoderChange(newMode);
                        break;
                    }
                }
            }
            HandleEOD(context, buffer);
        }

        internal override int EncodeChar(char c, StringBuilder sb)
        {
            if (c == '\r')
            {
                sb.Append('\0');
            }
            else if (c == '*')
            {
                sb.Append((char)1);
            }
            else if (c == '>')
            {
                sb.Append((char)2);
            }
            else if (c == ' ')
            {
                sb.Append((char)3);
            }
            else if (c >= '0' && c <= '9')
            {
                sb.Append((char)(c - 48 + 4));
            }
            else if (c >= 'A' && c <= 'Z')
            {
                sb.Append((char)(c - 65 + 14));
            }
            else
            {
                HighLevelEncoder.IllegalCharacter(c);
            }
            return 1;
        }

        internal override void HandleEOD(EncoderContext context, StringBuilder buffer)
        {
            context.UpdateSymbolInfo();
            int available = context.symbolInfo.dataCapacity - context.CodewordCount;
            int count = buffer.Length;
            if (count == 2)
            {
                context.WriteCodeword(HighLevelEncoder.X12_UNLATCH);
                context.pos -= 2;
                context.SignalEncoderChange(HighLevelEncoder.ASCII_ENCODATION);
            }
            else if (count == 1)
            {
                context.pos--;
                if (available > 1)
                {
                    context.WriteCodeword(HighLevelEncoder.X12_UNLATCH);
                }
                else
                {
                    //NOP - No unlatch necessary
                }
                context.SignalEncoderChange(HighLevelEncoder.ASCII_ENCODATION);
            }
        }
    }
}


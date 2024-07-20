using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class C40Encoder : Encoder
    {

        public virtual int EncodingMode
        {
            get
            {
                return HighLevelEncoder.C40_ENCODATION;
            }
        }

        public virtual void Encode(EncoderContext context)
        {
            //step C
            StringBuilder buffer = new StringBuilder();
            while (context.HasMoreCharacters())
            {
                char c = context.CurrentChar;
                context.pos++;

                int lastCharSize = EncodeChar(c, buffer);

                int unwritten = (buffer.Length / 3) * 2;

                int curCodewordCount = context.CodewordCount + unwritten;
                context.UpdateSymbolInfo(curCodewordCount);
                int available = context.symbolInfo.dataCapacity - curCodewordCount;

                if (!context.HasMoreCharacters())
                {
                    //Avoid having a single C40 value in the last triplet
                    StringBuilder removed = new StringBuilder();
                    if ((buffer.Length % 3) == 2)
                    {
                        if (available < 2 || available > 2)
                        {
                            lastCharSize = BacktrackOneCharacter(context, buffer, removed, lastCharSize);
                        }
                    }
                    while ((buffer.Length % 3) == 1 && ((lastCharSize <= 3 && available != 1) || lastCharSize > 3))
                    {
                        lastCharSize = BacktrackOneCharacter(context, buffer, removed, lastCharSize);
                    }
                    break;
                }

                int count = buffer.Length;
                if ((count % 3) == 0)
                {
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

        private int BacktrackOneCharacter(EncoderContext context, StringBuilder buffer, StringBuilder removed, int lastCharSize)
        {
            int count = buffer.Length;
            //buffer.Remove(count - lastCharSize, count);
            buffer.Remove(count - lastCharSize, lastCharSize);
            context.pos--;
            char c = context.CurrentChar;
            lastCharSize = EncodeChar(c, removed);
            context.ResetSymbolInfo(); //Deal with possible reduction in symbol size
            return lastCharSize;
        }

        internal static void WriteNextTriplet(EncoderContext context, StringBuilder buffer)
        {
            context.WriteCodewords(EncodeToCodewords(buffer, 0));
            buffer.Remove(0, 3);
        }

        /// <summary>
        /// Handle "end of data" situations
        /// </summary>
        /// <param name="context"> the encoder context </param>
        /// <param name="buffer">  the buffer with the remaining encoded characters </param>
        internal virtual void HandleEOD(EncoderContext context, StringBuilder buffer)
        {
            int unwritten = (buffer.Length / 3) * 2;
            int rest = buffer.Length % 3;

            int curCodewordCount = context.CodewordCount + unwritten;
            context.UpdateSymbolInfo(curCodewordCount);
            int available = context.symbolInfo.dataCapacity - curCodewordCount;

            if (rest == 2)
            {
                buffer.Append('\0'); //Shift 1
                while (buffer.Length >= 3)
                {
                    WriteNextTriplet(context, buffer);
                }
                if (context.HasMoreCharacters())
                {
                    context.WriteCodeword(HighLevelEncoder.C40_UNLATCH);
                }
            }
            else if (available == 1 && rest == 1)
            {
                while (buffer.Length >= 3)
                {
                    WriteNextTriplet(context, buffer);
                }
                if (context.HasMoreCharacters())
                {
                    context.WriteCodeword(HighLevelEncoder.C40_UNLATCH);
                }
                else
                {
                    //No unlatch
                }
                context.pos--;
            }
            else if (rest == 0)
            {
                while (buffer.Length >= 3)
                {
                    WriteNextTriplet(context, buffer);
                }
                if (available > 0 || context.HasMoreCharacters())
                {
                    context.WriteCodeword(HighLevelEncoder.C40_UNLATCH);
                }
            }
            else
            {
                throw new ArgumentException("Unexpected case. Please report!");
            }
            context.SignalEncoderChange(HighLevelEncoder.ASCII_ENCODATION);
        }

        internal virtual int EncodeChar(char c, StringBuilder sb)
        {
            if (c == ' ')
            {
                sb.Append((char)3);
                return 1;
            }
            else if (c >= '0' && c <= '9')
            {
                sb.Append((char)(c - 48 + 4));
                return 1;
            }
            else if (c >= 'A' && c <= 'Z')
            {
                sb.Append((char)(c - 65 + 14));
                return 1;
            }
            else if (c >= '\0' && c <= '\u001f')
            {
                sb.Append('\0'); //Shift 1 Set
                sb.Append(c);
                return 2;
            }
            else if (c >= '!' && c <= '/')
            {
                sb.Append((char)1); //Shift 2 Set
                sb.Append((char)(c - 33));
                return 2;
            }
            else if (c >= ':' && c <= '@')
            {
                sb.Append((char)1); //Shift 2 Set
                sb.Append((char)(c - 58 + 15));
                return 2;
            }
            else if (c >= '[' && c <= '_')
            {
                sb.Append((char)1); //Shift 2 Set
                sb.Append((char)(c - 91 + 22));
                return 2;
            }
            else if (c >= '\u0060' && c <= '\u007f')
            {
                sb.Append((char)2); //Shift 3 Set
                sb.Append((char)(c - 96));
                return 2;
            }
            else if (c >= '\u0080')
            {
                sb.Append((char)1);
                sb.Append('\u001e'); //Shift 2, Upper Shift
                int len = 2;
                len += EncodeChar((char)(c - 128), sb);
                return len;
            }
            else
            {
                throw new ArgumentException("Illegal character: " + c);
            }
        }

        private static string EncodeToCodewords(StringBuilder sb, int startPos)
        {
            char c1 = sb[startPos];
            char c2 = sb[startPos + 1];
            char c3 = sb[startPos + 2];
            int v = (1600 * c1) + (40 * c2) + c3 + 1;
            char cw1 = (char)(v / 256);
            char cw2 = (char)(v % 256);
            return new string(new char[] { cw1, cw2 });
        }

    }
}

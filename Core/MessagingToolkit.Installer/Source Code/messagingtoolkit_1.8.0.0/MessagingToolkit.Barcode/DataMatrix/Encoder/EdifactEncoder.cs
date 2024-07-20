using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class EdifactEncoder : Encoder
    {

        public virtual int EncodingMode
        {
            get
            {
                return HighLevelEncoder.EDIFACT_ENCODATION;
            }
        }

        public virtual void Encode(EncoderContext context)
        {
            //step F
            StringBuilder buffer = new StringBuilder();
            while (context.HasMoreCharacters())
            {
                char c = context.CurrentChar;
                encodeChar(c, buffer);
                context.pos++;

                int count = buffer.Length;
                if (count >= 4)
                {
                    context.WriteCodewords(EncodeToCodewords(buffer, 0));
                    buffer.Remove(0, 4);

                    int newMode = HighLevelEncoder.LookAheadTest(context.Message, context.pos, EncodingMode);
                    if (newMode != EncodingMode)
                    {
                        context.SignalEncoderChange(HighLevelEncoder.ASCII_ENCODATION);
                        break;
                    }
                }
            }
            buffer.Append((char)31); //Unlatch
            HandleEOD(context, buffer);
        }

        /// <summary>
        /// Handle "end of data" situations
        /// </summary>
        /// <param name="context"> the encoder context </param>
        /// <param name="buffer">  the buffer with the remaining encoded characters </param>
        private static void HandleEOD(EncoderContext context, StringBuilder buffer)
        {
            try
            {
                int count = buffer.Length;
                if (count == 0)
                {
                    return; //Already finished
                }
                if (count == 1)
                {
                    //Only an unlatch at the end
                    context.UpdateSymbolInfo();
                    int available = context.SymbolInfo.DataCapacity - context.CodewordCount;
                    int remaining = context.RemainingCharacters;
                    if (remaining == 0 && available <= 2)
                    {
                        return; //No unlatch
                    }
                }

                if (count > 4)
                {
                    throw new ArgumentException("Count must not exceed 4");
                }
                int restChars = count - 1;
                string encoded = EncodeToCodewords(buffer, 0);
                bool endOfSymbolReached = !context.HasMoreCharacters();
                bool restInAscii = endOfSymbolReached && restChars <= 2;

                if (restChars <= 2)
                {
                    context.UpdateSymbolInfo(context.CodewordCount + restChars);
                    int available = context.SymbolInfo.DataCapacity - context.CodewordCount;
                    if (available >= 3)
                    {
                        restInAscii = false;
                        context.UpdateSymbolInfo(context.CodewordCount + encoded.Length);
                        //available = context.symbolInfo.dataCapacity - context.getCodewordCount();
                    }
                }

                if (restInAscii)
                {
                    context.ResetSymbolInfo();
                    context.pos -= restChars;
                }
                else
                {
                    context.WriteCodewords(encoded);
                }
            }
            finally
            {
                context.SignalEncoderChange(HighLevelEncoder.ASCII_ENCODATION);
            }
        }

        private static void encodeChar(char c, StringBuilder sb)
        {
            if (c >= ' ' && c <= '?')
            {
                sb.Append(c);
            }
            else if (c >= '@' && c <= '^')
            {
                sb.Append((char)(c - 64));
            }
            else
            {
                HighLevelEncoder.IllegalCharacter(c);
            }
        }

        private static string EncodeToCodewords(StringBuilder sb, int startPos)
        {
            int len = sb.Length - startPos;
            if (len == 0)
            {
                throw new ArgumentException("StringBuilder must not be empty");
            }
            char c1 = sb[startPos];
            char c2 = (char)(len >= 2 ? sb[startPos + 1] : 0);
            char c3 = (char)(len >= 3 ? sb[startPos + 2] : 0);
            char c4 = (char)(len >= 4 ? sb[startPos + 3] : 0);

            int v = (c1 << 18) + (c2 << 12) + (c3 << 6) + c4;
            char cw1 = (char)((v >> 16) & 255);
            char cw2 = (char)((v >> 8) & 255);
            char cw3 = (char)(v & 255);
            StringBuilder res = new StringBuilder(3);
            res.Append(cw1);
            if (len >= 2)
            {
                res.Append(cw2);
            }
            if (len >= 3)
            {
                res.Append(cw3);
            }
            return res.ToString();
        }

    }
}

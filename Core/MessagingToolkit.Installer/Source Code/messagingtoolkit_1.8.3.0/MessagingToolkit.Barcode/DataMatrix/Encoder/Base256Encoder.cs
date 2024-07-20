using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class Base256Encoder : Encoder
    {

        public virtual int EncodingMode
        {
            get
            {
                return HighLevelEncoder.BASE256_ENCODATION;
            }
        }

        public virtual void Encode(EncoderContext context)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append('\0'); //Initialize length field
            while (context.HasMoreCharacters())
            {
                char c = context.CurrentChar;
                buffer.Append(c);

                context.pos++;

                int newMode = HighLevelEncoder.LookAheadTest(context.Message, context.pos, EncodingMode);
                if (newMode != EncodingMode)
                {
                    context.SignalEncoderChange(newMode);
                    break;
                }
            }
            int dataCount = buffer.Length - 1;
            int lengthFieldSize = 1;
            int currentSize = context.CodewordCount + dataCount + lengthFieldSize;
            context.UpdateSymbolInfo(currentSize);
            bool mustPad = (context.SymbolInfo.DataCapacity - currentSize) > 0;
            if (context.HasMoreCharacters() || mustPad)
            {
                if (dataCount <= 249)
                {
                    buffer[0] = (char)dataCount;
                }
                else if (dataCount > 249 && dataCount <= 1555)
                {
                    buffer[0] = (char)((dataCount / 250) + 249);
                    buffer.Insert(1, Convert.ToString(dataCount % 250));
                }
                else
                {
                    throw new ArgumentException("Message length not in valid ranges: " + dataCount);
                }
            }
            for (int i = 0, c = buffer.Length; i < c; i++)
            {
                context.WriteCodeword(Randomize255State(buffer[i], context.CodewordCount + 1));
            }
        }

        private static char Randomize255State(char ch, int codewordPosition)
        {
            int pseudoRandom = ((149 * codewordPosition) % 255) + 1;
            int tempVariable = ch + pseudoRandom;
            if (tempVariable <= 255)
            {
                return (char)tempVariable;
            }
            else
            {
                return (char)(tempVariable - 256);
            }
        }

    }
}

using MessagingToolkit.Barcode.Common;
using System;
using System.Text;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded
{
    public sealed class BinaryUtil
    {

        private BinaryUtil()
        {
        }

        /*
        * Constructs a BitArray from a String like the one returned from BitArray.toString()
        */
        public static BitArray BuildBitArrayFromString(String data)
        {
            String dotsAndXs = data.Replace("1", "X").Replace("0", ".");
            BitArray binary = new BitArray(dotsAndXs.Replace(" ", "").Length);
            int counter = 0;

            for (int i = 0; i < dotsAndXs.Length; ++i)
            {
                if (i % 9 == 0)
                { // spaces
                    if (dotsAndXs[i] != ' ')
                    {
                        throw new InvalidOperationException("space expected");
                    }
                    continue;
                }

                char currentChar = dotsAndXs[i];
                if (currentChar == 'X' || currentChar == 'x')
                {
                    binary.Set(counter);
                }
                counter++;
            }
            return binary;
        }

        public static BitArray BuildBitArrayFromStringWithoutSpaces(String data)
        {
            StringBuilder sb = new StringBuilder();

            String dotsAndXs = data.Replace("1", "X").Replace("0", ".");

            int current = 0;
            while (current < dotsAndXs.Length)
            {
                sb.Append(' ');
                for (int i = 0; i < 8 && current < dotsAndXs.Length; ++i)
                {
                    sb.Append(dotsAndXs[current]);
                    current++;
                }
            }

            return BuildBitArrayFromString(sb.ToString());
        }


    }
}

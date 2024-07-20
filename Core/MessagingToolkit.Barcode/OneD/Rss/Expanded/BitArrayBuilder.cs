using MessagingToolkit.Barcode.Common;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded
{

    internal sealed class BitArrayBuilder
    {

        private BitArrayBuilder()
        {
        }

        static internal BitArray BuildBitArray(List<ExpandedPair> pairs)
        {
            int charNumber = (pairs.Count << 1) - 1;
            if (((ExpandedPair)pairs[pairs.Count - 1]).RightChar == null)
            {
                charNumber -= 1;
            }

            int size = 12 * charNumber;

            BitArray binary = new BitArray(size);
            int accPos = 0;

            ExpandedPair firstPair = (ExpandedPair)pairs[0];
            int firstValue = firstPair.RightChar.Value;
            for (int i = 11; i >= 0; --i)
            {
                if ((firstValue & (1 << i)) != 0)
                {
                    binary.Set(accPos);
                }
                accPos++;
            }

            for (int i = 1; i < pairs.Count; ++i)
            {
                ExpandedPair currentPair = (ExpandedPair)pairs[i];

                int leftValue = currentPair.LeftChar.Value;
                for (int j = 11; j >= 0; --j)
                {
                    if ((leftValue & (1 << j)) != 0)
                    {
                        binary.Set(accPos);
                    }
                    accPos++;
                }

                if (currentPair.RightChar != null)
                {
                    int rightValue = currentPair.RightChar.Value;
                    for (int j = 11; j >= 0; --j)
                    {
                        if ((rightValue & (1 << j)) != 0)
                        {
                            binary.Set(accPos);
                        }
                        accPos++;
                    }
                }
            }
            return binary;
        }
    }
}

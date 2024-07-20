using MessagingToolkit.Barcode.OneD;
using System;

namespace MessagingToolkit.Barcode.OneD.Rss
{
    public abstract class AbstractRssDecoder : OneDDecoder
    {

        private static int MaxAvgVariance = (int)(MessagingToolkit.Barcode.OneD.OneDDecoder.PatternMatchResultScaleFactor * 0.2f);
        private static int MaxIndividualVariance = (int)(MessagingToolkit.Barcode.OneD.OneDDecoder.PatternMatchResultScaleFactor * 0.4f);

        private const float MinFinderPatternRatio = 9.5f / 12.0f;
        private const float MaxFinderPatternRatio = 12.5f / 14.0f;

        protected internal readonly int[] decodeFinderCounters;
        protected internal readonly int[] dataCharacterCounters;
        protected internal readonly float[] oddRoundingErrors;
        protected internal readonly float[] evenRoundingErrors;
        protected internal readonly int[] oddCounts;
        protected internal readonly int[] evenCounts;

        protected internal AbstractRssDecoder()
        {
            decodeFinderCounters = new int[4];
            dataCharacterCounters = new int[8];
            oddRoundingErrors = new float[4];
            evenRoundingErrors = new float[4];
            oddCounts = new int[dataCharacterCounters.Length / 2];
            evenCounts = new int[dataCharacterCounters.Length / 2];
        }

        protected int[] DecodeFinderCounters
        {
            get
            {
                return decodeFinderCounters;
            }
        }

        protected int[] DataCharacterCounters
        {
            get
            {
                return dataCharacterCounters;
            }
        }

        protected float[] OddRoundingErrors
        {
            get
            {
                return oddRoundingErrors;
            }
        }

        protected float[] EvenRoundingErrors 
        {
            get
            {
                return evenRoundingErrors;
            }
        }

        protected int[] OddCounts
        {
            get
            {
                return oddCounts;
            }
        }

        protected int[] EvenCounts 
        {
            get
            {
                return evenCounts;
            }
        
        }

        protected static internal int ParseFinderValue(int[] counters, int[][] finderPatterns)
        {
            for (int value = 0; value < finderPatterns.Length; value++)
            {
                if (MessagingToolkit.Barcode.OneD.OneDDecoder.PatternMatchVariance(counters, finderPatterns[value],
                        MaxIndividualVariance) < MaxAvgVariance)
                {
                    return value;
                }
            }
            throw NotFoundException.Instance;
        }

        protected static internal int Count(int[] array)
        {
            int count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                count += array[i];
            }
            return count;
        }

        protected static internal void Increment(int[] array, float[] errors)
        {
            int index = 0;
            float biggestError = errors[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (errors[i] > biggestError)
                {
                    biggestError = errors[i];
                    index = i;
                }
            }
            array[index]++;
        }

        protected static internal void Decrement(int[] array, float[] errors)
        {
            int index = 0;
            float biggestError = errors[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (errors[i] < biggestError)
                {
                    biggestError = errors[i];
                    index = i;
                }
            }
            array[index]--;
        }

        protected static internal bool IsFinderPattern(int[] counters)
        {
            int firstTwoSum = counters[0] + counters[1];
            int sum = firstTwoSum + counters[2] + counters[3];
            float ratio = (float)firstTwoSum / (float)sum;
            if (ratio >= MinFinderPatternRatio
                    && ratio <= MaxFinderPatternRatio)
            {
                // passes ratio test in spec, but see if the counts are unreasonable
                int minCounter = Int32.MaxValue;
                int maxCounter = Int32.MinValue;
                for (int i = 0; i < counters.Length; i++)
                {
                    int counter = counters[i];
                    if (counter > maxCounter)
                    {
                        maxCounter = counter;
                    }
                    if (counter < minCounter)
                    {
                        minCounter = counter;
                    }
                }
                return maxCounter < 10 * minCounter;
            }
            return false;
        }
    }
}

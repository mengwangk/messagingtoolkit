
namespace MessagingToolkit.Barcode.OneD.Rss
{
    internal sealed class Pair : DataCharacter
    {

        private readonly FinderPattern finderPattern;
        private int count;

        internal Pair(int value, int checksumPortion, FinderPattern finderPattern)
            : base(value, checksumPortion)
        {
            this.finderPattern = finderPattern;
        }

        internal FinderPattern FinderPattern
        {
            get
            {
                return finderPattern;
            }
        }

        internal int Count
        {
            get
            {
                return count;
            }
        }

        internal void IncrementCount()
        {
            count++;
        }

    }
}

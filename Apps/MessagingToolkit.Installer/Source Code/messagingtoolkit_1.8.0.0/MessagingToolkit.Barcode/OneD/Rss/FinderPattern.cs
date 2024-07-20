using System;

namespace MessagingToolkit.Barcode.OneD.Rss
{
    public sealed class FinderPattern
    {

        private readonly int value;
        private readonly int[] startEnd;
        private readonly ResultPoint[] resultPoints;

        public FinderPattern(int value, int[] startEnd, int start, int end, int rowNumber)
        {
            this.value = value;
            this.startEnd = startEnd;
            this.resultPoints = new ResultPoint[] {
					new ResultPoint((float) start, (float) rowNumber),
					new ResultPoint((float) end, (float) rowNumber), };
        }

        public int Value
        {
            get
            {
                return value;
            }
        }

        public int[] StartEnd
        {
            get
            {
                return startEnd;
            }
        }

        public ResultPoint[] ResultPoints
        {
            get
            {
                return resultPoints;
            }
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="o">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        override public bool Equals(Object o)
        {
            if (!(o is FinderPattern))
            {
                return false;
            }
            FinderPattern that = (FinderPattern)o;
            return Value == that.Value;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        override public int GetHashCode()
        {
            return Value;
        }
    }
}
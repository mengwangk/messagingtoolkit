using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded
{
    /// <summary>
    /// One row of an RSS Expanded Stacked symbol, consisting of 1+ expanded pairs.
    /// </summary>
    internal sealed class ExpandedRow
    {
        internal ExpandedRow(List<ExpandedPair> pairs, int rowNumber, bool wasReversed)
        {
            Pairs = new List<ExpandedPair>(pairs);
            RowNumber = rowNumber;
            IsReversed = wasReversed;
        }

        internal List<ExpandedPair> Pairs { get; private set; }

        internal int RowNumber { get; private set; }

        /// <summary>
        /// Did this row of the image have to be reversed (mirrored) to recognize the pairs?
        /// </summary>
        internal bool IsReversed { get; private set; }

        internal bool IsEquivalent(List<ExpandedPair> otherPairs)
        {
            return Pairs.Equals(otherPairs);
        }

        override public String ToString()
        {
            return "{ " + Pairs + " }";
        }

        /// <summary>
        /// Two rows are equal if they contain the same pairs in the same order.
        /// </summary>
        override public bool Equals(Object o)
        {
            if (!(o is ExpandedRow))
            {
                return false;
            }
            ExpandedRow that = (ExpandedRow)o;
            return Pairs.Equals(that.Pairs) && IsReversed == that.IsReversed;
        }

        override public int GetHashCode()
        {
            return Pairs.GetHashCode() ^ IsReversed.GetHashCode();
        }
    }
}
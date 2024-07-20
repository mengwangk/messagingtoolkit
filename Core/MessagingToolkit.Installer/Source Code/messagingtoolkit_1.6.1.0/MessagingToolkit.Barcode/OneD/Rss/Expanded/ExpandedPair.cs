namespace MessagingToolkit.Barcode.OneD.Rss.Expanded
{
    internal sealed class ExpandedPair
    {

        private readonly bool mayBeLast;
        private readonly DataCharacter leftChar;
        private readonly DataCharacter rightChar;
        private readonly FinderPattern finderPattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandedPair"/> class.
        /// </summary>
        /// <param name="leftChar">The left char.</param>
        /// <param name="rightChar">The right char.</param>
        /// <param name="finderPattern">The finder pattern.</param>
        /// <param name="mayBeLast">if set to <c>true</c> [may be last].</param>
        internal ExpandedPair(DataCharacter leftChar, DataCharacter rightChar, FinderPattern finderPattern, bool mayBeLast)
        {
            this.leftChar = leftChar;
            this.rightChar = rightChar;
            this.finderPattern = finderPattern;
            this.mayBeLast = mayBeLast;
        }

        internal bool MayBeLast
        {
            get
            {
                return this.mayBeLast;
            }
        }

        internal DataCharacter LeftChar
        {
            get
            {
                return this.leftChar;
            }
        }

        internal DataCharacter RightChar
        {
            get
            {
                return this.rightChar;
            }
        }

        internal FinderPattern FinderPattern
        {
            get
            {
                return this.finderPattern;
            }
        }

        public bool MustBeLast
        {
            get
            {
                return this.rightChar == null;
            }
        }
    }
}

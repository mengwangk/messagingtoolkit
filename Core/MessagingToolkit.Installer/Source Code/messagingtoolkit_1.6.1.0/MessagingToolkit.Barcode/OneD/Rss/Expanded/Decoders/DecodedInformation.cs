using System;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    internal sealed class DecodedInformation : DecodedObject
    {
        private readonly String newString;
        private readonly int remainingValue;
        private readonly bool remaining;

        internal DecodedInformation(int newPosition, String newString)
            : base(newPosition)
        {
            this.newString = newString;
            this.remaining = false;
            this.remainingValue = 0;
        }

        internal DecodedInformation(int newPosition, String newString, int remainingValue)
            : base(newPosition)
        {
            this.remaining = true;
            this.remainingValue = remainingValue;
            this.newString = newString;
        }

        internal String NewString
        {
            get
            {
                return this.newString;
            }
        }

        internal bool IsRemaining
        {
            get
            {
                return this.remaining;
            }
        }

        internal int RemainingValue
        {
            get
            {
                return this.remainingValue;
            }
        }
    }
}

using System;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    internal sealed class DecodedNumeric : DecodedObject
    {

        private readonly int firstDigit;
        private readonly int secondDigit;

        internal const int Fnc1 = 10;

        internal DecodedNumeric(int newPosition, int firstDigit, int secondDigit)
            : base(newPosition)
        {
            this.firstDigit = firstDigit;
            this.secondDigit = secondDigit;

            if (this.firstDigit < 0 || this.firstDigit > 10)
            {
                throw new ArgumentException("Invalid firstDigit: " + firstDigit);
            }

            if (this.secondDigit < 0 || this.secondDigit > 10)
            {
                throw new ArgumentException("Invalid secondDigit: " + secondDigit);
            }
        }

        internal int FirstDigit
        {
            get
            {
                return this.firstDigit;
            }
        }

        internal int SecondDigit
        {
            get
            {
                return this.secondDigit;
            }
        }

        internal int GetValue()
        {
            return this.firstDigit * 10 + this.secondDigit;
        }

        internal bool IsFirstDigitFnc1()
        {
            return this.firstDigit == Fnc1;
        }

        internal bool IsSecondDigitFnc1()
        {
            return this.secondDigit == Fnc1;
        }

        internal bool IsAnyFnc1()
        {
            return this.firstDigit == Fnc1 || this.secondDigit == Fnc1;
        }

    }
}

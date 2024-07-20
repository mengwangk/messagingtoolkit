
namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    internal sealed class DecodedChar : DecodedObject
    {

        private readonly char value;

        internal const char Fnc1 = '$'; // It's not in Alphanumeric neither in ISO/IEC 646 charset

        internal DecodedChar(int newPosition, char value)
            : base(newPosition)
        {
            this.value = value;
        }

        internal char Value
        {
            get
            {
                return this.value;
            }
        }

        internal bool IsFnc1
        {
            get
            {
                return this.value == Fnc1;
            }
        }

    }
}

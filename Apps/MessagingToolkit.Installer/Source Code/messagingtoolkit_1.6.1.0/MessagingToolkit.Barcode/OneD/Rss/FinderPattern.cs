
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

    }
}

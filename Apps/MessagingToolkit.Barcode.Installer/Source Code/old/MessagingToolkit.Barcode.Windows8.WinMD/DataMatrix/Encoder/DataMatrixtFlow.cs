
namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class DataMatrixPointFlow
    {
        #region Properties

        internal int Plane { get; set; }

        internal int Arrive { get; set; }

        internal int Depart { get; set; }

        internal int Mag { get; set; }

        internal DataMatrixPixelLoc Loc { get; set; }

        #endregion
    }
}

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal struct DataMatrixBestLine
    {
        internal int Angle { get; set; }

        internal int HOffset { get; set; }

        internal int Mag { get; set; }

        internal int StepBeg { get; set; }

        internal int StepPos { get; set; }

        internal int StepNeg { get; set; }

        internal int DistSq { get; set; }

        internal double Devn { get; set; }

        internal DataMatrixPixelLoc LocBeg { get; set; }

        internal DataMatrixPixelLoc LocPos { get; set; }

        internal DataMatrixPixelLoc LocNeg { get; set; }
    }
}

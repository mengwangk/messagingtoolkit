namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal struct DataMatrixFollow
    {
        #region Fields

        int _ptrIndex;
        #endregion

        #region Properties
        internal int PtrIndex
        {
            set
            {
                _ptrIndex = value;
            }
        }

        internal byte CurrentPtr
        {
            get
            {
                return this.Ptr[_ptrIndex];
            }
            set
            {
                this.Ptr[_ptrIndex] = value;
            }
        }

        internal byte[] Ptr { get; set; }

        internal byte Neighbor
        {
            get
            {
                return this.Ptr[_ptrIndex];
            }
            set
            {
                this.Ptr[_ptrIndex] = value;
            }
        }

        internal int Step { get; set; }

        internal DataMatrixPixelLoc Loc { get; set; }

        #endregion
    }
}
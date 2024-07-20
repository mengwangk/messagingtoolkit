namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal struct DataMatrixTriplet
    {
        byte[] _value;

        internal byte[] Value
        {
            get { return this._value ?? (this._value = new byte[3]); }
        }
    }

    /**
     * @struct DmtxQuadruplet
     * @brief DmtxQuadruplet
     */
    internal struct DataMatrixQuadruplet
    {
        byte[] _value;

        internal byte[] Value
        {
            get { return this._value ?? (this._value = new byte[4]); }
        }
    }
}

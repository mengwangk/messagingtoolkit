namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class DataMatrixRay2
    {
        #region Fields

        DataMatrixVector2 _p;
        DataMatrixVector2 _v;
        #endregion

        #region Properties
        internal DataMatrixVector2 P
        {
            get { return this._p ?? (this._p = new DataMatrixVector2()); }
            set
            {
                _p = value;
            }
        }

        internal DataMatrixVector2 V
        {
            get { return this._v ?? (this._v = new DataMatrixVector2()); }
            set
            {
                _v = value;
            }
        }


        internal double TMin { get; set; }

        internal double TMax { get; set; }

        #endregion
    }
}

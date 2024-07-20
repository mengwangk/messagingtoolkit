using System;

namespace MessagingToolkit.Barcode.QRCode.Encoder
{
    /// <summary>
    /// Modified April 28 2012
    /// </summary>
    internal sealed class BlockPair
    {

        private readonly byte[] dataBytes;
        private readonly byte[] errorCorrectionBytes;

        internal BlockPair(byte[] data, byte[] errorCorrection)
        {
            dataBytes = data;
            errorCorrectionBytes = errorCorrection;
        }

        public byte[] GetDataBytes()
        {
            return dataBytes;
        }

        public byte[] GetErrorCorrectionBytes()
        {
            return errorCorrectionBytes;
        }

    }
}

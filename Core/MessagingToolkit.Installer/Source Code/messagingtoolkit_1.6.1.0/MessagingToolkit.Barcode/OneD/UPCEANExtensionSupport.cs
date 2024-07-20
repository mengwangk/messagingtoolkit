using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using BitArray = MessagingToolkit.Barcode.Common.BitArray;


namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// Modified: May 26 2012
    /// </summary>
    internal sealed class UPCEANExtensionSupport
    {

        public UPCEANExtensionSupport()
        {
            this.twoSupport = new UPCEANExtension2Support();
            this.fiveSupport = new UPCEANExtension5Support();
        }

        private static readonly int[] EXTENSION_START_PATTERN = { 1, 1, 2 };

        private readonly UPCEANExtension2Support twoSupport;
        private readonly UPCEANExtension5Support fiveSupport;

        internal Result DecodeRow(int rowNumber, BitArray row, int rowOffset)
        {
            int[] extensionStartRange = UPCEANDecoder.FindGuardPattern(row, rowOffset, false, EXTENSION_START_PATTERN);
            try
            {
                return fiveSupport.DecodeRow(rowNumber, row, extensionStartRange);
            }
            catch (BarcodeDecoderException re)
            {
                return twoSupport.DecodeRow(rowNumber, row, extensionStartRange);
            }
        }
    }
}

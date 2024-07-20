using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    /// <summary>
    /// Constants for DataMatrix.
    /// </summary>
    internal class Constants
    {
        /// <summary>
        /// Padding character </summary>
        internal static char PAD = (char)129;
        /// <summary>
        /// mode latch to C40 encodation mode </summary>
        internal static char LATCH_TO_C40 = (char)230;
        /// <summary>
        /// mode latch to Base 256 encodation mode </summary>
        internal static char LATCH_TO_BASE256 = (char)231;
        /// <summary>
        /// FNC1 Codeword </summary>
        internal static char FNC1 = (char)232;
        /// <summary>
        /// Structured Append Codeword </summary>
        internal static char STRUCTURED_APPEND = (char)233;
        /// <summary>
        /// Reader Programming </summary>
        internal static char READER_PROGRAMMING = (char)234;
        /// <summary>
        /// Upper Shift </summary>
        internal static char UPPER_SHIFT = (char)235;
        /// <summary>
        /// 05 Macro </summary>
        internal static char MACRO_05 = (char)236;
        /// <summary>
        /// 06 Macro </summary>
        internal static char MACRO_06 = (char)237;
        /// <summary>
        /// mode latch to ANSI X.12 encodation mode </summary>
        internal static char LATCH_TO_ANSIX12 = (char)238;
        /// <summary>
        /// mode latch to Text encodation mode </summary>
        internal static char LATCH_TO_TEXT = (char)239;
        /// <summary>
        /// mode latch to EDIFACT encodation mode </summary>
        internal static char LATCH_TO_EDIFACT = (char)240;
        /// <summary>
        /// ECI character (Extended Channel Interpretation) </summary>
        internal static char ECI = (char)241;

        /// <summary>
        /// Unlatch from C40 encodation </summary>
        internal static char C40_UNLATCH = (char)254;
        /// <summary>
        /// Unlatch from X12 encodation </summary>
        internal static char X12_UNLATCH = (char)254;

        /// <summary>
        /// 05 Macro header </summary>
        internal static string MACRO_05_HEADER = "[)>\u001E05\u001D";
        /// <summary>
        /// 06 Macro header </summary>
        internal static string MACRO_06_HEADER = "[)>\u001E06\u001D";
        /// <summary>
        /// Macro trailer </summary>
        internal static string MACRO_TRAILER = "\u001E\u0004";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Provider
{
    /// <summary>
    /// Output provider interface.
    /// </summary>
    public interface IOutputProvider<TResult>
    {

        /// <summary>
        /// Generates the raw data into binary form representing bars and spaces.
        /// Also generates an Image of the barcode.
        /// </summary>
        /// <param name="bitMatrix">The bit matrix.</param>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="content">Raw data to encode.</param>
        /// <returns>
        /// Output representing the barcode.
        /// </returns>
        TResult Generate(BitMatrix bitMatrix, BarcodeFormat format, string content) ;

        /// <summary>
        /// Generates the raw data into binary form representing bars and spaces.
        /// Also generates an Image of the barcode.
        /// </summary>
        /// <param name="bitMatrix">The bit matrix.</param>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="content">Raw data to encode.</param>
        /// <param name="encodingOptions">The encoding options.</param>
        /// <returns>
        /// Output representing the barcode.
        /// </returns>
        TResult Generate(BitMatrix bitMatrix, BarcodeFormat format, string content, Dictionary<EncodeOptions, object> encodingOptions);
    }
}

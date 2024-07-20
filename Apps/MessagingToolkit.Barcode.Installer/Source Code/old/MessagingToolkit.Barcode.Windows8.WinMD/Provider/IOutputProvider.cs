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
    public interface IOutputProvider
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
        IOutput Generate(BitMatrix bitMatrix, BarcodeFormat format, string content) ;

        /// <summary>
        /// Generates the raw data into binary form representing bars and spaces.
        /// Also generates an Image of the barcode.
        /// </summary>
        /// <param name="bitMatrix">The bit matrix.</param>
        /// <param name="format">Type of encoding to use.</param>
        /// <param name="content">Raw data to encode.</param>
        /// <param name="options">The encoding options.</param>
        /// <returns>
        /// Output representing the barcode.
        /// </returns>
        IOutput Generate(BitMatrix bitMatrix, BarcodeFormat format, string content, IDictionary<EncodeOptions, object> options);
    }
}

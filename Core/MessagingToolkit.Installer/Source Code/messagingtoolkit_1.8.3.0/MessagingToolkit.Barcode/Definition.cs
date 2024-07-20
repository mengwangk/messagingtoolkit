
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MessagingToolkit.Barcode
{
    /// <summary>
    /// Barcode type
    /// </summary>
    public enum BarcodeFormat : int
    {
        Unknown,
        QRCode,
        DataMatrix,
        PDF417,
        UPCA,
        UPCE,
        UPCSupplemental2Digit,
        UPCSupplemental5Digit,
        EAN13,
        EAN8,
        Interleaved2of5,
        Standard2of5,
        Industrial2of5,
        Code39,
        Code39Extended,
        Codabar,
        PostNet,
        Bookland,
        ISBN,
        JAN13,
        MSIMod10,
        MSI2Mod10,
        MSIMod11,
        MSIMod11Mod10,
        ModifiedPlessey,
        Code11,
        USD8,
        UCC12,
        UCC13,
        LOGMARS,
        Code128,
        Code128A,
        Code128B,
        Code128C,
        ITF14,
        Code93,
        Telepen,
        FIM,
        UPCEANExtension,
        Aztec,
        RSS14,
        RSSExpanded,
        MaxiCode
    };


    /// <summary>
    /// Save options
    /// </summary>
    public enum SaveOptions : int
    {
        Jpg,
        Bmp,
        Png,
        Gif,
        Tiff,
        Unspecified
    };

    /// <summary>
    /// Alignment positions
    /// </summary>
    public enum AlignmentPositions : int { Center, Left, Right };

    /// <summary>
    /// Label positions
    /// </summary>
    public enum LabelPositions : int { TopLeft, TopCenter, TopRight, BottomLeft, BottomCenter, BottomRight };


    /// <summary>
    /// These are a set of hints that you may pass to encoders to specify their behavior.
    /// </summary>
    public enum EncodeOptions
    {
        /// <summary>
        /// Specifies what degree of error correction to use, for example in QR Codes.
        /// Type depends on the encoder. For example for QR codes it's type <see cref="MessagingToolkit.QRCode.Decoder.ErrorCorrectionLevel"/>
        /// For Aztec it is of type Integer, representing the minimal percentage of error correction words.
        /// Note: an Aztec symbol should have a minimum of 25% EC words.
        /// </summary>
        ErrorCorrection,
        /// <summary>
        /// Character set
        /// </summary>
        CharacterSet,
        /// <summary>
        /// Specifies margin, in pixels, to use when generating the barcode. The meaning can vary 
        /// by format; for example it controls margin before and after the barcode horizontally for
        /// most 1D formats.
        /// </summary>
        Margin,
        /// Specifies whether to use compact mode for PDF417
        /// </summary>
        Pdf417Compact,
        /// <summary>
        /// Specifies what compaction mode to use for PDF417 type
        /// </summary>
        Pdf417Compaction,
        /// <summary>
        /// Specifies the minimum and maximum number of rows and columns for PDF417 
        /// </summary>
        Pdf417Dimensions,
        /// <summary>
        /// A byte array containing the image or logo that you want to overlay on the generated QRCode
        /// </summary>
        QRCodeLogo,
        /// <summary>
        /// Specifies the matrix shape (type SymbolShapeHint)
        /// </summary>
        DataMatrixShape,
        /// <summary>
        /// Specifies which is the minimum size of the Data Matrix (type Dimension)
        /// </summary>
        MinimumSize,
        /// <summary>
        /// Specifies which is the minimum size of the Data Matrix (type Dimension)
        /// </summary>
        MaximumSize
    }


    /// <summary>
    /// Encapsulates a type of hint that a caller may pass to a barcode reader to help it
    /// more quickly or accurately decode it. It is up to implementations to decide what,
    /// if anything, to do with the information that is supplied.
    /// </summary>
    public enum DecodeOptions
    {
        /// <summary>
        /// Unspecified, application-specific options. 
        /// </summary>
        Other,
        /// <summary> 
        /// Image is a pure monochrome image of a barcode.
        /// </summary>
        PureBarcode,
        /// <summary> 
        /// Image is known to be of one of a few possible formats.
        /// </summary>
        PossibleFormats,
        /// <summary> 
        /// Spend more time to try to find a barcode; optimize for accuracy, not speed.
        /// </summary>
        TryHarder,
        /// <summary>
        /// Specifies what character encoding to use when decoding, where applicable (type String)
        /// </summary>
        CharacterSet,
        /// <summary> 
        /// Allowed lengths of encoded data -- reject anything else. 
        /// </summary>
        AllowedLengths,
        /// <summary> 
        /// Assume Code 39 codes employ a check digit. 
        /// </summary>
        AssumeCode39CheckDigit,
        /// <summary>
        /// Assume the barcode is being processed as a GS1 barcode, and modify behavior as needed.
        /// For example this affects FNC1 handling for Code 128 (aka GS1-128). Doesn't matter what it maps to;
        /// use TRUE
        /// </summary>
        ASSUME_GS1,
        /// <summary> 
        /// The caller needs to be notified via callback when a possible result point
        /// is found.
        /// </summary>
        NeedResultPointCallback,

        /// <summary>
        /// Decode multiple barcode
        /// </summary>
        MultipleBarcode,

        /// <summary>
        /// Assume MSI codes employ a check digit. Maps to <see cref="bool" />.
        /// </summary>
        AssumeMsiCheckDigit,

        /// <summary>
        /// Perform auto rotation on image source 
        /// </summary>
        AutoRotate,

    }


    /// <summary>
    /// Result metadata type
    /// </summary>
    public enum ResultMetadataType
    {
        /// <summary>
        /// Unspecified, application-specific metadata. Maps to an unspecified <see cref="T:System.Object"/>.
        /// </summary>
        Other,
        /// <summary>
        /// Denotes the likely approximate orientation of the barcode in the image. This value
        /// is given as degrees rotated clockwise from the normal, upright orientation.
        /// For example a 1D barcode which was found by reading top-to-bottom would be
        /// said to have orientation "90". This key maps to an <see cref="T:System.Int32"/> whose
        /// value is in the range [0,360].
        /// </summary>
        Orientation,
        /// <summary>
        /// <p>2D barcode formats typically encode text, but allow for a sort of 'byte mode'
        /// which is sometimes used to encode binary data. While <see cref="T:MessagingToolkit.Barcode.Result"/> makes available
        /// the complete raw bytes in the barcode for these formats, it does not offer the bytes
        /// from the byte segments alone.</p>
        /// </summary>
        ByteSegments,
        /// <summary>
        /// Error correction level used, if applicable. The value type depends on the
        /// format, but is typically a String.
        /// </summary>
        ErrorCorrectionLevel,
        /// <summary>
        /// For some periodicals, indicates the issue number as an <see cref="T:System.Int32"/>.
        /// </summary>
        IssueNumber,
        /// <summary>
        /// For some products, indicates the suggested retail price in the barcode as a
        /// formatted <see cref="T:System.String"/>.
        /// </summary>
        SuggestedPrice,
        /// <summary>
        /// For some products, the possible country of manufacture as a <see cref="T:System.String"/> denoting the
        /// ISO country code. Some map to multiple possible countries, like "US/CA".
        /// </summary>
        PossibleCountry,
        /// <summary>
        /// For some products, the extension text
        /// </summary>
        UPCEANExtension,
        /// <summary>
        /// PDF417-specific metadata
        /// </summary>
        PDF417ExtraMetadata
    }
}

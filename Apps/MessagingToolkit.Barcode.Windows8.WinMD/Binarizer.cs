using System;
using BitArray = MessagingToolkit.Barcode.Common.BitArray;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;

namespace MessagingToolkit.Barcode
{

    /// <summary>
    /// This class hierarchy provides a set of methods to convert luminance data to 1 bit data.
    /// It allows the algorithm to vary polymorphically, for example allowing a very expensive
    /// thresholding technique for servers and a fast one for mobile. It also permits the implementation
    /// to vary
    /// </summary>
	internal abstract class Binarizer
	{
        private readonly LuminanceSource source;
		
        virtual public LuminanceSource LuminanceSource
		{
			get
			{
				return source;
			}			
		}
        /// <summary>
        /// Converts a 2D array of luminance data to 1 bit data. As above, assume this method is expensive
        /// and do not call it repeatedly. This method is intended for decoding 2D barcodes and may or
        /// may not apply sharpening. Therefore, a row from this matrix may not be identical to one
        /// fetched using GetBlackRow(), so don't mix and match between them.
        /// </summary>
        /// <value>The black matrix.</value>
        /// <returns> The 2D array of bits for the image (true means black).
        /// </returns>
		public abstract BitMatrix BlackMatrix
        { 
            get;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Binarizer"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
		protected internal Binarizer(LuminanceSource source)
		{
			if (source == null)
			{
				throw new ArgumentException("Source must be non-null.");
			}
			this.source = source;
		}

        /// <summary>
        /// Converts one row of luminance data to 1 bit data. May actually do the conversion, or return
        /// cached data. Callers should assume this method is expensive and call it as seldom as possible.
        /// This method is intended for decoding 1D barcodes and may choose to apply sharpening.
        /// For callers which only examine one row of pixels at a time, the same BitArray should be reused
        /// and passed in with each call for performance. However it is legal to keep more than one row
        /// at a time if needed.
        /// </summary>
        /// <param name="y">The row to fetch, y is greater than or equals 0 less than bitmap height.</param>
        /// <param name="row">An optional preallocated array. If null or too small, it will be ignored.
        /// If used, the Binarizer will call BitArray.clear(). Always use the returned object.</param>
        /// <returns>
        /// The array of bits for this row (true means black).
        /// </returns>
		public abstract BitArray GetBlackRow(int y, BitArray row);

        /// <summary>
        /// Creates a new object with the same type as this Binarizer implementation, but with pristine
        /// state. This is needed because Binarizer implementations may be stateful, e.g. keeping a cache
        /// of 1 bit data. 
        /// </summary>
        /// <param name="source">The LuminanceSource this Binarizer will operate on.</param>
        /// <returns>
        /// A new concrete Binarizer implementation object.
        /// </returns>
		public abstract Binarizer CreateBinarizer(LuminanceSource source);

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width
        {
            get
            {
                return source.Width;
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height
        {
            get
            {
                return source.Height;
            }
        }
	}
}
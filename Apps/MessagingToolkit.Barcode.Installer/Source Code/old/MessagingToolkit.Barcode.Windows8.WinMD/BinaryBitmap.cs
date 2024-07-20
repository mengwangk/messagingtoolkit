//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using BitArray = MessagingToolkit.Barcode.Common.BitArray;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;

namespace MessagingToolkit.Barcode
{

    /// <summary>
    /// This class is the core bitmap class used to represent 1 bit data. Decoder objects
    /// accept a BinaryBitmap and attempt to decode it.
    /// </summary>
	internal sealed class BinaryBitmap
	{
        private Binarizer binarizer;
        private BitMatrix matrix;

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        /// <returns> The width of the bitmap.
        /// </returns>
		public int Width
		{
			get
			{
				return binarizer.LuminanceSource.Width;
			}
			
		}
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        /// <returns> The height of the bitmap.
        /// </returns>
		public int Height
		{
			get
			{
				return binarizer.LuminanceSource.Height;
			}
			
		}
        /// <summary>
        /// Converts a 2D array of luminance data to 1 bit. As above, assume this method is expensive
        /// and do not call it repeatedly. This method is intended for decoding 2D barcodes and may or
        /// may not apply sharpening. Therefore, a row from this matrix may not be identical to one
        /// fetched using getBlackRow(), so don't mix and match between them.
        /// </summary>
        /// <value>The black matrix.</value>
        /// <returns> The 2D array of bits for the image (true means black).
        /// </returns>
		public BitMatrix BlackMatrix
		{
			get
			{
				// The matrix is created on demand the first time it is requested, then cached. There are two
				// reasons for this:
				// 1. This work will never be done if the caller only installs 1D Reader objects, or if a
				//    1D Reader finds a barcode before the 2D Readers run.
				// 2. This work will only be done once even if the caller installs multiple 2D Readers.
				if (matrix == null)
				{
					matrix = binarizer.BlackMatrix;
				}
				return matrix;
			}
			
		}
        /// <summary>
        /// Gets a value indicating whether [crop supported].
        /// </summary>
        /// <value><c>true</c> if [crop supported]; otherwise, <c>false</c>.</value>
        /// <returns> Whether this bitmap can be cropped.
        /// </returns>
		public bool CropSupported
		{
			get
			{
				return binarizer.LuminanceSource.CropSupported;
			}
			
		}
        /// <summary>
        /// Gets a value indicating whether [rotate supported].
        /// </summary>
        /// <value><c>true</c> if [rotate supported]; otherwise, <c>false</c>.</value>
        /// <returns> Whether this bitmap supports counter-clockwise rotation.
        /// </returns>
		public bool RotateSupported
		{
			get
			{
				return binarizer.LuminanceSource.RotateSupported;
			}
			
		}


        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryBitmap"/> class.
        /// </summary>
        /// <param name="binarizer">The binarizer.</param>
		public BinaryBitmap(Binarizer binarizer)
		{
			if (binarizer == null)
			{
				throw new System.ArgumentException("Binarizer must be non-null.");
			}
			this.binarizer = binarizer;
			matrix = null;
		}
		
		/// <summary> 
        /// Converts one row of luminance data to 1 bit data. May actually do the conversion, or return
		/// cached data. Callers should assume this method is expensive and call it as seldom as possible.
		/// This method is intended for decoding 1D barcodes and may choose to apply sharpening.
		/// </summary>
		/// <param name="y">The row to fetch, 0 <= y < bitmap height.
		/// </param>
		/// <param name="row">An optional preallocated array. If null or too small, it will be ignored.
		/// If used, the Binarizer will call BitArray.clear(). Always use the returned object.
		/// </param>
		/// <returns> The array of bits for this row (true means black).
		/// </returns>
		public BitArray GetBlackRow(int y, BitArray row)
		{
			return binarizer.GetBlackRow(y, row);
		}
		
		/// <summary> 
        /// Returns a new object with cropped image data. Implementations may keep a reference to the
		/// original data rather than a copy. Only callable if isCropSupported() is true.
		/// </summary>
		/// <param name="left">The left coordinate, 0 <= left < getWidth().
		/// </param>
		/// <param name="top">The top coordinate, 0 <= top <= getHeight().
		/// </param>
		/// <param name="width">The width of the rectangle to crop.
		/// </param>
		/// <param name="height">The height of the rectangle to crop.
		/// </param>
		/// <returns> A cropped version of this object.
		/// </returns>
		public BinaryBitmap Crop(int left, int top, int width, int height)
		{
			LuminanceSource newSource = binarizer.LuminanceSource.Crop(left, top, width, height);
			return new BinaryBitmap(binarizer.CreateBinarizer(newSource));
		}

        /// <summary>
        /// Returns a new object with rotated image data. Only callable if isRotateSupported() is true.
        /// </summary>
        /// <returns>A rotated version of this object.</returns>
		public BinaryBitmap RotateCounterClockwise()
		{
			LuminanceSource newSource = binarizer.LuminanceSource.RotateCounterClockwise();
			return new BinaryBitmap(binarizer.CreateBinarizer(newSource));
		}

        /// <summary>
        /// Returns a new object with rotated image data by 45 degrees counterclockwise.
        /// Only callable if RotateSupported is true.
        /// </summary>
        /// <returns>
        /// A rotated version of this object.
        /// </returns>
	  public BinaryBitmap RotateCounterClockwise45() {
	    LuminanceSource newSource = binarizer.LuminanceSource.RotateCounterClockwise45();
	    return new BinaryBitmap(binarizer.CreateBinarizer(newSource));
	  }
	}
}
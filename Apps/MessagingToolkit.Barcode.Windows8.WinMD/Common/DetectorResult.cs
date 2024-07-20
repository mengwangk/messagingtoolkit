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
using ResultPoint = MessagingToolkit.Barcode.ResultPoint;

namespace MessagingToolkit.Barcode.Common
{

    /// <summary>
    /// Encapsulates the result of detecting a barcode in an image. This includes the raw
    /// matrix of black/white pixels corresponding to the barcode, and possibly points of interest
    /// in the image, like the location of finder patterns or corners of the barcode in the image.
    /// 
    /// Modified: April 21 2012
    /// </summary>
    internal class DetectorResult
	{
        private BitMatrix bits;
        private ResultPoint[] points;

		public BitMatrix Bits
		{
			get
			{
				return bits;
			}
			
		}
		public ResultPoint[] Points
		{
			get
			{
				return points;
			}			
		}
		
		public DetectorResult(BitMatrix bits, ResultPoint[] points)
		{
			this.bits = bits;
			this.points = points;
		}
	}
}
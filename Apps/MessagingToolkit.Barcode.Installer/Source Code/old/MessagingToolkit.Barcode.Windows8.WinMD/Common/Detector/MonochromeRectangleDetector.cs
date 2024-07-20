//===============================================================================
// OSML - Open Source Messaging Library
//
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
using ReaderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using ResultPoint = MessagingToolkit.Barcode.ResultPoint;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;

namespace MessagingToolkit.Barcode.Common.Detector
{

    /// <summary>
    /// A somewhat generic detector that looks for a barcode-like rectangular region within an image.
    /// It looks within a mostly white region of an image for a region of black and white, but mostly
    /// black. It returns the four corners of the region, as best it can determine.
    /// </summary>
    public sealed class MonochromeRectangleDetector
	{		
		private const int MaxModules = 32;
		private BitMatrix image;
		
		public MonochromeRectangleDetector(BitMatrix image)
		{
			this.image = image;
		}

        /// <summary>
        /// Detects a rectangular region of black and white -- mostly black -- with a region of mostly
        /// white, in an image.
        /// </summary>
        /// <returns>
        /// {@link ResultPoint}[] describing the corners of the rectangular region. The first and
        /// last points are opposed on the diagonal, as are the second and third. The first point will be
        /// the topmost point and the last, the bottommost. The second point will be leftmost and the
        /// third, the rightmost
        /// </returns>
        /// <throws>  ReaderException if no Data Matrix Code can be found </throws>
		public ResultPoint[] Detect()
		{
			int height = image.Height;
			int width = image.Width;
			int halfHeight = height >> 1;
			int halfWidth = width >> 1;
			int deltaY = System.Math.Max(1, height / (MaxModules << 3));
			int deltaX = System.Math.Max(1, width / (MaxModules << 3));
			
			int top = 0;
			int bottom = height;
			int left = 0;
			int right = width;
			ResultPoint pointA = FindCornerFromCenter(halfWidth, 0, left, right, halfHeight, - deltaY, top, bottom, halfWidth >> 1);
			top = (int) pointA.Y - 1;
			ResultPoint pointB = FindCornerFromCenter(halfWidth, - deltaX, left, right, halfHeight, 0, top, bottom, halfHeight >> 1);
			left = (int) pointB.X - 1;
			ResultPoint pointC = FindCornerFromCenter(halfWidth, deltaX, left, right, halfHeight, 0, top, bottom, halfHeight >> 1);
			right = (int) pointC.X + 1;
			ResultPoint pointD = FindCornerFromCenter(halfWidth, 0, left, right, halfHeight, deltaY, top, bottom, halfWidth >> 1);
			bottom = (int) pointD.Y + 1;
			
			// Go try to find point A again with better information -- might have been off at first.
			pointA = FindCornerFromCenter(halfWidth, 0, left, right, halfHeight, - deltaY, top, bottom, halfWidth >> 2);
			
			return new ResultPoint[]{pointA, pointB, pointC, pointD};
		}

        /// <summary>
        /// Attempts to locate a corner of the barcode by scanning up, down, left or right from a center
        /// point which should be within the barcode.
        /// </summary>
        /// <param name="centerX">center's x component (horizontal)</param>
        /// <param name="deltaX">same as deltaY but change in x per step instead</param>
        /// <param name="left">minimum value of x</param>
        /// <param name="right">maximum value of x</param>
        /// <param name="centerY">center's y component (vertical)</param>
        /// <param name="deltaY">change in y per step. If scanning up this is negative; down, positive;
        /// left or right, 0</param>
        /// <param name="top">minimum value of y to search through (meaningless when di == 0)</param>
        /// <param name="bottom">maximum value of y</param>
        /// <param name="maxWhiteRun">maximum run of white pixels that can still be considered to be within
        /// the barcode</param>
        /// <returns>
        /// a {@link MessagingToolkit.Barcode.ResultPoint} encapsulating the corner that was found
        /// </returns>
        /// <throws>  MessagingToolkit.Barcode.ReaderException if such a point cannot be found </throws>
		private ResultPoint FindCornerFromCenter(int centerX, int deltaX, int left, int right, int centerY, int deltaY, int top, int bottom, int maxWhiteRun)
		{
			int[] lastRange = null;
			for (int y = centerY, x = centerX; y < bottom && y >= top && x < right && x >= left; y += deltaY, x += deltaX)
			{
				int[] range;
				if (deltaX == 0)
				{
					// horizontal slices, up and down
					range = BlackWhiteRange(y, maxWhiteRun, left, right, true);
				}
				else
				{
					// vertical slices, left and right
					range = BlackWhiteRange(x, maxWhiteRun, top, bottom, false);
				}
				if (range == null)
				{
					if (lastRange == null)
					{
                        throw NotFoundException.Instance;
					}
					// lastRange was found
					if (deltaX == 0)
					{
						int lastY = y - deltaY;
						if (lastRange[0] < centerX)
						{
							if (lastRange[1] > centerX)
							{
								// straddle, choose one or the other based on direction
								return new ResultPoint(deltaY > 0?lastRange[0]:lastRange[1], lastY);
							}
							return new ResultPoint(lastRange[0], lastY);
						}
						else
						{
							return new ResultPoint(lastRange[1], lastY);
						}
					}
					else
					{
						int lastX = x - deltaX;
						if (lastRange[0] < centerY)
						{
							if (lastRange[1] > centerY)
							{
								return new ResultPoint(lastX, deltaX < 0?lastRange[0]:lastRange[1]);
							}
							return new ResultPoint(lastX, lastRange[0]);
						}
						else
						{
							return new ResultPoint(lastX, lastRange[1]);
						}
					}
				}
				lastRange = range;
			}
            throw NotFoundException.Instance;
		}

        /// <summary>
        /// Computes the start and end of a region of pixels, either horizontally or vertically, that could
        /// be part of a Data Matrix barcode.
        /// </summary>
        /// <param name="fixedDimension">if scanning horizontally, this is the row (the fixed vertical location)
        /// where we are scanning. If scanning vertically it's the column, the fixed horizontal location</param>
        /// <param name="maxWhiteRun">largest run of white pixels that can still be considered part of the
        /// barcode region</param>
        /// <param name="minDim">minimum pixel location, horizontally or vertically, to consider</param>
        /// <param name="maxDim">maximum pixel location, horizontally or vertically, to consider</param>
        /// <param name="horizontal">if true, we're scanning left-right, instead of up-down</param>
        /// <returns>
        /// int[] with start and end of found range, or null if no such range is found
        /// (e.g. only white was found)
        /// </returns>
		private int[] BlackWhiteRange(int fixedDimension, int maxWhiteRun, int minDim, int maxDim, bool horizontal)
		{
			
			int center = (minDim + maxDim) >> 1;
			
			// Scan left/up first
			int start = center;
			while (start >= minDim)
			{
				if (horizontal?image.GetValue(start, fixedDimension):image.GetValue(fixedDimension, start))
				{
					start--;
				}
				else
				{
					int whiteRunStart = start;
					do 
					{
						start--;
					}
					while (start >= minDim && !(horizontal?image.GetValue(start, fixedDimension):image.GetValue(fixedDimension, start)));
					int whiteRunSize = whiteRunStart - start;
					if (start < minDim || whiteRunSize > maxWhiteRun)
					{
						start = whiteRunStart;
						break;
					}
				}
			}
			start++;
			
			// Then try right/down
			int end = center;
			while (end < maxDim)
			{
				if (horizontal?image.GetValue(end, fixedDimension):image.GetValue(fixedDimension, end))
				{
					end++;
				}
				else
				{
					int whiteRunStart = end;
					do 
					{
						end++;
					}
					while (end < maxDim && !(horizontal?image.GetValue(end, fixedDimension):image.GetValue(fixedDimension, end)));
					int whiteRunSize = end - whiteRunStart;
					if (end >= maxDim || whiteRunSize > maxWhiteRun)
					{
						end = whiteRunStart;
						break;
					}
				}
			}
			end--;
			
			return end > start?new int[]{start, end}:null;
		}
	}
}
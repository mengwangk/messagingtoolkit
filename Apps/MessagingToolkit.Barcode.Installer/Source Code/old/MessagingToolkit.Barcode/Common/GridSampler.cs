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
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;

namespace MessagingToolkit.Barcode.Common
{

    /// <summary>
    /// Implementations of this class can, given locations of finder patterns for a QR code in an
    /// image, sample the right points in the image to reconstruct the QR code, accounting for
    /// perspective distortion. It is abstracted since it is relatively expensive and should be allowed
    /// to take advantage of platform-specific optimized implementations, but which may not be 
    /// available in other environments such as J2ME, and vice
    /// versa.
    /// The implementation used can be controlled by calling <see cref="M:GridSampler.SetGridSampler(GridSampler)"/>with an instance of a class which implements this interface.
    ///
    /// Modified: April 21 2012
    /// </summary>
    public abstract class GridSampler
    {

        private static GridSampler gridSampler = new DefaultGridSampler();

        /// <summary>
        /// Sets the implementation of GridSampler used by the library. One global
        /// instance is stored, which may sound problematic. But, the implementation provided
        /// ought to be appropriate for the entire platform, and all uses of this library
        /// in the whole lifetime of the JVM. For instance, an Android activity can swap in
        /// an implementation that takes advantage of native platform libraries.
        /// </summary>
        ///
        /// <param name="newGridSampler">The platform-specific object to install.</param>
        public static void SetGridSampler(GridSampler newGridSampler)
        {
            gridSampler = newGridSampler;
        }


        /// <returns>the current implementation of GridSampler</returns>
        public static GridSampler GetInstance()
        {
            return gridSampler;
        }

        /// <summary>
        /// Samples an image for a rectangular matrix of bits of the given dimension.
        /// </summary>
        /// <param name="image">image to sample</param>
        /// <param name="dimensionX">to sample from image</param>
        /// <param name="dimensionY">to sample from image</param>
        /// <param name="p1ToX">The p1 to X.</param>
        /// <param name="p1ToY">The p1 to Y.</param>
        /// <param name="p2ToX">The p2 to X.</param>
        /// <param name="p2ToY">The p2 to Y.</param>
        /// <param name="p3ToX">The p3 to X.</param>
        /// <param name="p3ToY">The p3 to Y.</param>
        /// <param name="p4ToX">The p4 to X.</param>
        /// <param name="p4ToY">The p4 to Y.</param>
        /// <param name="p1FromX">The p1 from X.</param>
        /// <param name="p1FromY">The p1 from Y.</param>
        /// <param name="p2FromX">The p2 from X.</param>
        /// <param name="p2FromY">The p2 from Y.</param>
        /// <param name="p3FromX">The p3 from X.</param>
        /// <param name="p3FromY">The p3 from Y.</param>
        /// <param name="p4FromX">The p4 from X.</param>
        /// <param name="p4FromY">The p4 from Y.</param>
        /// <returns>
        /// /// <see cref="T:Com.Google.Zxing.Common.BitMatrix"/>
        /// representing a grid of points sampled from the image within a region
        /// defined by the "from" parameters
        /// </returns>
        /// <exception cref="NotFoundException">if image can't be sampled, for example, if the transformation definedby the given points is invalid or results in sampling outside the image boundaries</exception>
        public abstract BitMatrix SampleGrid(BitMatrix image, int dimensionX, int dimensionY, float p1ToX, float p1ToY, float p2ToX, float p2ToY, float p3ToX, float p3ToY, float p4ToX, float p4ToY,
                float p1FromX, float p1FromY, float p2FromX, float p2FromY, float p3FromX, float p3FromY, float p4FromX, float p4FromY);

        public abstract BitMatrix SampleGrid(BitMatrix image, int dimensionX, int dimensionY, PerspectiveTransform transform);

        /// <summary>
        /// <p>Checks a set of points that have been transformed to sample points on an image against
        /// the image's dimensions to see if the point are even within the image.</p>
        /// <p>This method will actually "nudge" the endpoints back onto the image if they are found to be
        /// barely (less than 1 pixel) off the image. This accounts for imperfect detection of finder
        /// patterns in an image where the QR Code runs all the way to the image border.</p>
        /// <p>For efficiency, the method will check points from either end of the line until one is found
        /// to be within the image. Because the set of points are assumed to be linear, this is valid.</p>
        /// </summary>
        ///
        /// <param name="image">image into which the points should map</param>
        /// <param name="points">actual points in x1,y1,...,xn,yn form</param>
        /// <exception cref="NotFoundException">if an endpoint is lies outside the image boundaries</exception>
        protected static internal void CheckAndNudgePoints(BitMatrix image, float[] points)
        {
            int width = image.GetWidth();
            int height = image.GetHeight();
            // Check and nudge points from start until we see some that are OK:
            bool nudged = true;
            for (int offset = 0; offset < points.Length && nudged; offset += 2)
            {
                int x = (int)points[offset];
                int y = (int)points[offset + 1];
                if (x < -1 || x > width || y < -1 || y > height)
                {
                    throw NotFoundException.Instance;
                }
                nudged = false;
                if (x == -1)
                {
                    points[offset] = 0.0f;
                    nudged = true;
                }
                else if (x == width)
                {
                    points[offset] = width - 1;
                    nudged = true;
                }
                if (y == -1)
                {
                    points[offset + 1] = 0.0f;
                    nudged = true;
                }
                else if (y == height)
                {
                    points[offset + 1] = height - 1;
                    nudged = true;
                }
            }
            // Check and nudge points from end:
            nudged = true;
            for (int offset_0 = points.Length - 2; offset_0 >= 0 && nudged; offset_0 -= 2)
            {
                int x_1 = (int)points[offset_0];
                int y_2 = (int)points[offset_0 + 1];
                if (x_1 < -1 || x_1 > width || y_2 < -1 || y_2 > height)
                {
                    throw NotFoundException.Instance;
                }
                nudged = false;
                if (x_1 == -1)
                {
                    points[offset_0] = 0.0f;
                    nudged = true;
                }
                else if (x_1 == width)
                {
                    points[offset_0] = width - 1;
                    nudged = true;
                }
                if (y_2 == -1)
                {
                    points[offset_0 + 1] = 0.0f;
                    nudged = true;
                }
                else if (y_2 == height)
                {
                    points[offset_0 + 1] = height - 1;
                    nudged = true;
                }
            }
        }

    }
}
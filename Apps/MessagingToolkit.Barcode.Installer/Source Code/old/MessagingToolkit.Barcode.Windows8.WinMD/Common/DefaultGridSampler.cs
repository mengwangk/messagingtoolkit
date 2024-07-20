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
    /// Default grid sampler
    /// 
    /// Modified: April 21 2012
    /// </summary>
    internal sealed class DefaultGridSampler : GridSampler
    {
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
        /// /// <see cref="T:MessagingToolkit.Barcode.Common.BitMatrix"/>
        /// representing a grid of points sampled from the image within a region
        /// defined by the "from" parameters
        /// </returns>
        /// <exception cref="NotFoundException">if image can't be sampled, for example, if the transformation definedby the given points is invalid or results in sampling outside the image boundaries</exception>
        public override BitMatrix SampleGrid(BitMatrix image, int dimensionX, int dimensionY, float p1ToX, float p1ToY, float p2ToX, float p2ToY, float p3ToX, float p3ToY, float p4ToX, float p4ToY, float p1FromX,
                 float p1FromY, float p2FromX, float p2FromY, float p3FromX, float p3FromY, float p4FromX, float p4FromY)
        {

            PerspectiveTransform transform = PerspectiveTransform.QuadrilateralToQuadrilateral(p1ToX, p1ToY, p2ToX, p2ToY, p3ToX, p3ToY, p4ToX, p4ToY, p1FromX, p1FromY, p2FromX, p2FromY, p3FromX,
                    p3FromY, p4FromX, p4FromY);

            return SampleGrid(image, dimensionX, dimensionY, transform);
        }

        public override BitMatrix SampleGrid(BitMatrix image, int dimensionX, int dimensionY, PerspectiveTransform transform)
        {
            if (dimensionX <= 0 || dimensionY <= 0)
            {
                throw NotFoundException.Instance;
            }
            BitMatrix bits = new BitMatrix(dimensionX, dimensionY);
            float[] points = new float[dimensionX << 1];
            for (int y = 0; y < dimensionY; y++)
            {
                int max = points.Length;
                float iValue = (float)y + 0.5f;
                for (int x = 0; x < max; x += 2)
                {
                    points[x] = (float)(x >> 1) + 0.5f;
                    points[x + 1] = iValue;
                }
                transform.TransformPoints(points);
                // Quick check to see if points transformed to something inside the image;
                // sufficient to check the endpoints
                GridSampler.CheckAndNudgePoints(image, points);
                try
                {
                    for (int x_0 = 0; x_0 < max; x_0 += 2)
                    {
                        if (image.Get((int)points[x_0], (int)points[x_0 + 1]))
                        {
                            // Black(-ish) pixel
                            bits.Set(x_0 >> 1, y);
                        }
                    }
                }
                catch (IndexOutOfRangeException aioobe)
                {
                    // This feels wrong, but, sometimes if the finder patterns are misidentified, the resulting
                    // transform gets "twisted" such that it maps a straight line of points to a set of points
                    // whose endpoints are in bounds, but others are not. There is probably some mathematical
                    // way to detect this about the transformation that I don't know yet.
                    // This results in an ugly runtime exception despite our clever checks above -- can't have
                    // that. We could check each point's coordinates but that feels duplicative. We settle for
                    // catching and wrapping ArrayIndexOutOfBoundsException.
                    throw NotFoundException.Instance;
                }
            }
            return bits;
        }

    }
}
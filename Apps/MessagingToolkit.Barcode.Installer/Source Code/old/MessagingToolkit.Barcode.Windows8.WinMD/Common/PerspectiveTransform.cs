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

namespace MessagingToolkit.Barcode.Common
{

    /// <summary>
    /// 	<p>This class implements a perspective transform in two dimensions. Given four source and four
    /// destination points, it will compute the transformation implied between them. The code is based
    /// directly upon section 3.4.2 of George Wolberg's "Digital Image Warping"; see pages 54-56.</p>
    /// 
    /// Modified: April 21 2012
    /// </summary>
    internal sealed class PerspectiveTransform
	{
        private readonly float a11;
        private readonly float a12;
        private readonly float a13;
        private readonly float a21;
        private readonly float a22;
        private readonly float a23;
        private readonly float a31;
        private readonly float a32;
        private readonly float a33;

        private PerspectiveTransform(float a11_0, float a21_1, float a31_2, float a12_3, float a22_4, float a32_5, float a13_6, float a23_7, float a33_8)
        {
            this.a11 = a11_0;
            this.a12 = a12_3;
            this.a13 = a13_6;
            this.a21 = a21_1;
            this.a22 = a22_4;
            this.a23 = a23_7;
            this.a31 = a31_2;
            this.a32 = a32_5;
            this.a33 = a33_8;
        }

        public static PerspectiveTransform QuadrilateralToQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, float x0p, float y0p, float x1p, float y1p,
                float x2p, float y2p, float x3p, float y3p)
        {

            PerspectiveTransform qToS = QuadrilateralToSquare(x0, y0, x1, y1, x2, y2, x3, y3);
            PerspectiveTransform sToQ = SquareToQuadrilateral(x0p, y0p, x1p, y1p, x2p, y2p, x3p, y3p);
            return sToQ.Times(qToS);
        }

        public void TransformPoints(float[] points)
        {
            int max = points.Length;
            float a11_0 = this.a11;
            float a12_1 = this.a12;
            float a13_2 = this.a13;
            float a21_3 = this.a21;
            float a22_4 = this.a22;
            float a23_5 = this.a23;
            float a31_6 = this.a31;
            float a32_7 = this.a32;
            float a33_8 = this.a33;
            for (int i = 0; i < max; i += 2)
            {
                float x = points[i];
                float y = points[i + 1];
                float denominator = a13_2 * x + a23_5 * y + a33_8;
                points[i] = (a11_0 * x + a21_3 * y + a31_6) / denominator;
                points[i + 1] = (a12_1 * x + a22_4 * y + a32_7) / denominator;
            }
        }

        /// <summary>
        /// Convenience method, not optimized for performance. 
        /// </summary>
        ///
        public void TransformPoints(float[] xValues, float[] yValues)
        {
            int n = xValues.Length;
            for (int i = 0; i < n; i++)
            {
                float x = xValues[i];
                float y = yValues[i];
                float denominator = a13 * x + a23 * y + a33;
                xValues[i] = (a11 * x + a21 * y + a31) / denominator;
                yValues[i] = (a12 * x + a22 * y + a32) / denominator;
            }
        }

        public static PerspectiveTransform SquareToQuadrilateral(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float dx3 = x0 - x1 + x2 - x3;
            float dy3 = y0 - y1 + y2 - y3;
            if (dx3 == 0.0f && dy3 == 0.0f)
            {
                // Affine
                return new PerspectiveTransform(x1 - x0, x2 - x1, x0, y1 - y0, y2 - y1, y0, 0.0f, 0.0f, 1.0f);
            }
            else
            {
                float dx1 = x1 - x2;
                float dx2 = x3 - x2;
                float dy1 = y1 - y2;
                float dy2 = y3 - y2;
                float denominator = dx1 * dy2 - dx2 * dy1;
                float a13_0 = (dx3 * dy2 - dx2 * dy3) / denominator;
                float a23_1 = (dx1 * dy3 - dx3 * dy1) / denominator;
                return new PerspectiveTransform(x1 - x0 + a13_0 * x1, x3 - x0 + a23_1 * x3, x0, y1 - y0 + a13_0 * y1, y3 - y0 + a23_1 * y3, y0, a13_0, a23_1, 1.0f);
            }
        }

        public static PerspectiveTransform QuadrilateralToSquare(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            // Here, the adjoint serves as the inverse:
            return SquareToQuadrilateral(x0, y0, x1, y1, x2, y2, x3, y3).BuildAdjoint();
        }

        internal PerspectiveTransform BuildAdjoint()
        {
            // Adjoint is the transpose of the cofactor matrix:
            return new PerspectiveTransform(a22 * a33 - a23 * a32, a23 * a31 - a21 * a33, a21 * a32 - a22 * a31, a13 * a32 - a12 * a33, a11 * a33 - a13 * a31, a12 * a31 - a11 * a32,
                    a12 * a23 - a13 * a22, a13 * a21 - a11 * a23, a11 * a22 - a12 * a21);
        }

        internal PerspectiveTransform Times(PerspectiveTransform other)
        {
            return new PerspectiveTransform(a11 * other.a11 + a21 * other.a12 + a31 * other.a13, a11 * other.a21 + a21 * other.a22 + a31 * other.a23, a11 * other.a31 + a21 * other.a32 + a31 * other.a33,
                    a12 * other.a11 + a22 * other.a12 + a32 * other.a13, a12 * other.a21 + a22 * other.a22 + a32 * other.a23, a12 * other.a31 + a22 * other.a32 + a32 * other.a33, a13 * other.a11 + a23
                            * other.a12 + a33 * other.a13, a13 * other.a21 + a23 * other.a22 + a33 * other.a23, a13 * other.a31 + a23 * other.a32 + a33 * other.a33);

        }
	}
}
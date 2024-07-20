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

namespace MessagingToolkit.Barcode.Common.ReedSolomon
{

    /// <summary>
    /// This class contains utility methods for performing mathematical operations over
    /// the Galois Field GF(256). Operations use a given primitive polynomial in calculations.
    /// Throughout this package, elements of GF(256) are represented as an <code>int</code>
    /// for convenience and speed (but at the cost of memory).
    /// Only the bottom 8 bits are really used.
    /// </summary>
	public sealed class Gf256
	{
        public static readonly Gf256 QrCodeField = new Gf256(0x011D); // x^8 + x^4 + x^3 + x^2 + 1
        public static readonly Gf256 DataMatrixField = new Gf256(0x012D); // x^8 + x^5 + x^3 + x^2 + 1
        private int[] expTable;
        private int[] logTable;
        private GF256Poly zero;
        private GF256Poly one;

		internal GF256Poly Zero
		{
			get
			{
				return zero;
			}
			
		}
        internal GF256Poly One
        {
            get
            {
                return one;
            }
        }

        /// <summary>
        /// Create a representation of GF(256) using the given primitive polynomial.
        /// </summary>
        /// <param name="primitive">irreducible polynomial whose coefficients are represented by
        /// the bits of an int, where the least-significant bit represents the constant
        /// coefficient</param>
		private Gf256(int primitive)
		{
			expTable = new int[256];
			logTable = new int[256];
			int x = 1;
			for (int i = 0; i < 256; i++)
			{
				expTable[i] = x;
				x <<= 1; // x = x * 2; we're assuming the generator alpha is 2
				if (x >= 0x100)
				{
					x ^= primitive;
				}
			}
			for (int i = 0; i < 255; i++)
			{
				logTable[expTable[i]] = i;
			}
			// logTable[0] == 0 but this should never be used
			zero = new GF256Poly(this, new int[]{0});
			one = new GF256Poly(this, new int[]{1});
		}

        /// <summary>
        /// Builds the monomial.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <param name="coefficient">The coefficient.</param>
        /// <returns>
        /// The monomial representing coefficient * x^degree
        /// </returns>
		internal GF256Poly BuildMonomial(int degree, int coefficient)
		{
			if (degree < 0)
			{
				throw new System.ArgumentException();
			}
			if (coefficient == 0)
			{
				return zero;
			}
			int[] coefficients = new int[degree + 1];
			coefficients[0] = coefficient;
			return new GF256Poly(this, coefficients);
		}

        /// <summary>
        /// Implements both addition and subtraction -- they are the same in GF(256).
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>sum/difference of a and b</returns>
		internal static int AddOrSubtract(int a, int b)
		{
			return a ^ b;
		}

        /// <summary>
        /// Exps the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <returns>2 to the power of a in GF(256)</returns>
		internal int Exp(int a)
		{
			return expTable[a];
		}

        /// <summary>
        /// Logs the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <returns>base 2 log of a in GF(256)</returns>
		internal int Log(int a)
		{
			if (a == 0)
			{
				throw new System.ArgumentException();
			}
			return logTable[a];
		}

        /// <summary>
        /// Inverses the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <returns>multiplicative inverse of a</returns>
		internal int Inverse(int a)
		{
			if (a == 0)
			{
				throw new System.ArithmeticException();
			}
			return expTable[255 - logTable[a]];
		}

        /// <summary>
        /// Multiplies the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>product of a and b in GF(256)</returns>
		internal int Multiply(int a, int b)
		{
			if (a == 0 || b == 0)
			{
				return 0;
			}
			if (a == 1)
			{
				return b;
			}
			if (b == 1)
			{
				return a;
			}
			return expTable[(logTable[a] + logTable[b]) % 255];
		}
	}
}
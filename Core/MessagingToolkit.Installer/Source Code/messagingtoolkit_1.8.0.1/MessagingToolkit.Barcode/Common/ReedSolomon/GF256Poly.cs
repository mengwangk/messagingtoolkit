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
    /// Represents a polynomial whose coefficients are elements of GF(256).
	/// Instances of this class are immutable.	
	/// 
    /// Much credit is due to William Rucklidge since portions of this code are an indirect
	/// port of his C++ Reed-Solomon implementation.	/// 
	/// </summary>
	sealed class GF256Poly
	{
        private Gf256 field;
        private int[] coefficients;
		
        /// <summary>
        /// Gets the coefficients.
        /// </summary>
        /// <value>The coefficients.</value>
		internal int[] Coefficients
		{
			get
			{
				return coefficients;
			}			
		}

        /// <summary>
        /// Gets the degree.
        /// </summary>
        /// <value>The degree.</value>
        /// <returns> degree of this polynomial
        /// </returns>
		internal int Degree
		{
			get
			{
				return coefficients.Length - 1;
			}
			
		}
        /// <summary>
        /// Gets a value indicating whether this <see cref="GF256Poly"/> is zero.
        /// </summary>
        /// <value><c>true</c> if zero; otherwise, <c>false</c>.</value>
        /// <returns> true iff this polynomial is the monomial "0"
        /// </returns>
		internal bool Zero
		{
			get
			{
				return coefficients[0] == 0;
			}			
		}


        /// <summary>
        /// or if leading coefficient is 0 and this is not a
        /// constant polynomial (that is, it is not the monomial "0")
        /// </summary>
        /// <param name="field">the {@link GF256} instance representing the field to use
        /// to perform computations</param>
        /// <param name="coefficients">coefficients as ints representing elements of GF(256), arranged
        /// from most significant (highest-power term) coefficient to least significant</param>
        /// <throws>  IllegalArgumentException if argument is null or empty, </throws>
        internal GF256Poly(Gf256 field, int[] coefficients)
		{
			if (coefficients == null || coefficients.Length == 0)
			{
				throw new System.ArgumentException();
			}
			this.field = field;
			int coefficientsLength = coefficients.Length;
			if (coefficientsLength > 1 && coefficients[0] == 0)
			{
				// Leading term must be non-zero for anything except the constant polynomial "0"
				int firstNonZero = 1;
				while (firstNonZero < coefficientsLength && coefficients[firstNonZero] == 0)
				{
					firstNonZero++;
				}
				if (firstNonZero == coefficientsLength)
				{
					this.coefficients = field.Zero.coefficients;
				}
				else
				{
					this.coefficients = new int[coefficientsLength - firstNonZero];
					Array.Copy(coefficients, firstNonZero, this.coefficients, 0, this.coefficients.Length);
				}
			}
			else
			{
				this.coefficients = coefficients;
			}
		}

        /// <summary>
        /// Gets the coefficient.
        /// </summary>
        /// <param name="degree">The degree.</param>
        /// <returns>
        /// coefficient of x^degree term in this polynomial
        /// </returns>
		internal int GetCoefficient(int degree)
		{
			return coefficients[coefficients.Length - 1 - degree];
		}

        /// <summary>
        /// Evaluates at.
        /// </summary>
        /// <param name="a">A.</param>
        /// <returns>
        /// evaluation of this polynomial at a given point
        /// </returns>
		internal int EvaluateAt(int a)
		{
			if (a == 0)
			{
				// Just return the x^0 coefficient
				return GetCoefficient(0);
			}
			int size = coefficients.Length;
			if (a == 1)
			{
				// Just the sum of the coefficients
				int result = 0;
				for (int i = 0; i < size; i++)
				{
					result = Gf256.AddOrSubtract(result, coefficients[i]);
				}
				return result;
			}
			int result2 = coefficients[0];
			for (int i = 1; i < size; i++)
			{
				result2 = Gf256.AddOrSubtract(field.Multiply(a, result2), coefficients[i]);
			}
			return result2;
		}
		
		internal GF256Poly AddOrSubtract(GF256Poly other)
		{
			if (!field.Equals(other.field))
			{
				throw new System.ArgumentException("GF256Polys do not have same GF256 field");
			}
			if (Zero)
			{
				return other;
			}
			if (other.Zero)
			{
				return this;
			}
			
			int[] smallerCoefficients = this.coefficients;
			int[] largerCoefficients = other.coefficients;
			if (smallerCoefficients.Length > largerCoefficients.Length)
			{
				int[] temp = smallerCoefficients;
				smallerCoefficients = largerCoefficients;
				largerCoefficients = temp;
			}
			int[] sumDiff = new int[largerCoefficients.Length];
			int lengthDiff = largerCoefficients.Length - smallerCoefficients.Length;
			// Copy high-order terms only found in higher-degree polynomial's coefficients
			Array.Copy(largerCoefficients, 0, sumDiff, 0, lengthDiff);
			
			for (int i = lengthDiff; i < largerCoefficients.Length; i++)
			{
				sumDiff[i] = Gf256.AddOrSubtract(smallerCoefficients[i - lengthDiff], largerCoefficients[i]);
			}
			
			return new GF256Poly(field, sumDiff);
		}
		
		internal GF256Poly Multiply(GF256Poly other)
		{
			if (!field.Equals(other.field))
			{
				throw new System.ArgumentException("GF256Polys do not have same GF256 field");
			}
			if (Zero || other.Zero)
			{
				return field.Zero;
			}
			int[] aCoefficients = this.coefficients;
			int aLength = aCoefficients.Length;
			int[] bCoefficients = other.coefficients;
			int bLength = bCoefficients.Length;
			int[] product = new int[aLength + bLength - 1];
			for (int i = 0; i < aLength; i++)
			{
				int aCoeff = aCoefficients[i];
				for (int j = 0; j < bLength; j++)
				{
					product[i + j] = Gf256.AddOrSubtract(product[i + j], field.Multiply(aCoeff, bCoefficients[j]));
				}
			}
			return new GF256Poly(field, product);
		}
		
		internal GF256Poly Multiply(int scalar)
		{
			if (scalar == 0)
			{
				return field.Zero;
			}
			if (scalar == 1)
			{
				return this;
			}
			int size = coefficients.Length;
			int[] product = new int[size];
			for (int i = 0; i < size; i++)
			{
				product[i] = field.Multiply(coefficients[i], scalar);
			}
			return new GF256Poly(field, product);
		}
		
		internal GF256Poly MultiplyByMonomial(int degree, int coefficient)
		{
			if (degree < 0)
			{
				throw new System.ArgumentException();
			}
			if (coefficient == 0)
			{
				return field.Zero;
			}
			int size = coefficients.Length;
			int[] product = new int[size + degree];
			for (int i = 0; i < size; i++)
			{
				product[i] = field.Multiply(coefficients[i], coefficient);
			}
			return new GF256Poly(field, product);
		}

        /// <summary>
        /// Divides the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
		internal GF256Poly[] Divide(GF256Poly other)
		{
			if (!field.Equals(other.field))
			{
				throw new System.ArgumentException("GF256Polys do not have same GF256 field");
			}
			if (other.Zero)
			{
				throw new System.ArgumentException("Divide by 0");
			}
			
			GF256Poly quotient = field.Zero;
			GF256Poly remainder = this;
			
			int denominatorLeadingTerm = other.GetCoefficient(other.Degree);
			int inverseDenominatorLeadingTerm = field.Inverse(denominatorLeadingTerm);
			
			while (remainder.Degree >= other.Degree && !remainder.Zero)
			{
				int degreeDifference = remainder.Degree - other.Degree;
				int scale = field.Multiply(remainder.GetCoefficient(remainder.Degree), inverseDenominatorLeadingTerm);
				GF256Poly term = other.MultiplyByMonomial(degreeDifference, scale);
				GF256Poly iterationQuotient = field.BuildMonomial(degreeDifference, scale);
				quotient = quotient.AddOrSubtract(iterationQuotient);
				remainder = remainder.AddOrSubtract(term);
			}
			
			return new GF256Poly[]{quotient, remainder};
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
		public override string ToString()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder(8 * Degree);
			for (int degree = Degree; degree >= 0; degree--)
			{
				int coefficient = GetCoefficient(degree);
				if (coefficient != 0)
				{
					if (coefficient < 0)
					{
						result.Append(" - ");
						coefficient = - coefficient;
					}
					else
					{
						if (result.Length > 0)
						{
							result.Append(" + ");
						}
					}
					if (degree == 0 || coefficient != 1)
					{
						int alphaPower = field.Log(coefficient);
						if (alphaPower == 0)
						{
							result.Append('1');
						}
						else if (alphaPower == 1)
						{
							result.Append('a');
						}
						else
						{
							result.Append("a^");
							result.Append(alphaPower);
						}
					}
					if (degree != 0)
					{
						if (degree == 1)
						{
							result.Append('x');
						}
						else
						{
							result.Append("X^");
							result.Append(degree);
						}
					}
				}
			}
			return result.ToString();
		}
	}
}
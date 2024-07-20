using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MessagingToolkit.Barcode.Common.ReedSolomon
{
        /// <summary>
    /// <p>Represents a polynomial whose coefficients are elements of a GF.
    /// Instances of this class are immutable.</p>
    /// <p>Much credit is due to William Rucklidge since portions of this code are an indirect
    /// port of his C++ Reed-Solomon implementation.</p>
    /// </summary>
    ///
    internal sealed class GenericGFPoly
    {

        private readonly GenericGF field;
        private readonly int[] coefficients;


        /// <param name="field">instance representing the field to use to perform computations</param>
        /// <param name="coefficients"></param>
        /// <exception cref="System.ArgumentException">if argument is null or empty,or if leading coefficient is 0 and this is not aconstant polynomial (that is, it is not the monomial "0")</exception>
        internal GenericGFPoly(GenericGF field, int[] coefficients)
        {
            if (coefficients == null || coefficients.Length == 0)
            {
                throw new ArgumentException();
            }
            this.field = field;
            int coefficientsLength = coefficients.Length;
            if (coefficientsLength > 1 && coefficients[0] == 0)
            {
                // Leading term must be non-zero for anything except the constant polynomial "0"
                int firstNonZero = 1;
                while (firstNonZero < coefficientsLength
                        && coefficients[firstNonZero] == 0)
                {
                    firstNonZero++;
                }
                if (firstNonZero == coefficientsLength)
                {
                    this.coefficients = field.GetZero().coefficients;
                }
                else
                {
                    this.coefficients = new int[coefficientsLength - firstNonZero];
                    System.Array.Copy((Array)(coefficients), firstNonZero, (Array)(this.coefficients), 0, this.coefficients.Length);
                }
            }
            else
            {
                this.coefficients = coefficients;
            }
        }

        internal int[] GetCoefficients()
        {
            return coefficients;
        }


        /// <returns>degree of this polynomial</returns>
        internal int GetDegree()
        {
            return coefficients.Length - 1;
        }


        /// <returns>true iff this polynomial is the monomial "0"</returns>
        internal bool IsZero()
        {
            return coefficients[0] == 0;
        }


        /// <returns>coefficient of x^degree term in this polynomial</returns>
        internal int GetCoefficient(int degree)
        {
            return coefficients[coefficients.Length - 1 - degree];
        }


        /// <returns>evaluation of this polynomial at a given point</returns>
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
                    result = GenericGF.AddOrSubtract(result, coefficients[i]);
                }
                return result;
            }
            int result_0 = coefficients[0];
            for (int i_1 = 1; i_1 < size; i_1++)
            {
                result_0 = GenericGF.AddOrSubtract(field.Multiply(a, result_0),
                        coefficients[i_1]);
            }
            return result_0;
        }

        internal GenericGFPoly AddOrSubtract(GenericGFPoly other)
        {
            if (!field.Equals(other.field))
            {
                throw new ArgumentException("GenericGFPolys do not have same GenericGF field");
            }
            if (IsZero())
            {
                return other;
            }
            if (other.IsZero())
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
            System.Array.Copy((Array)(largerCoefficients), 0, (Array)(sumDiff), 0, lengthDiff);

            for (int i = lengthDiff; i < largerCoefficients.Length; i++)
            {
                sumDiff[i] = GenericGF.AddOrSubtract(smallerCoefficients[i
                        - lengthDiff], largerCoefficients[i]);
            }

            return new GenericGFPoly(field, sumDiff);
        }

        internal GenericGFPoly Multiply(GenericGFPoly other)
        {
            if (!field.Equals(other.field))
            {
                throw new ArgumentException(
                        "GenericGFPolys do not have same GenericGF field");
            }
            if (IsZero() || other.IsZero())
            {
                return field.GetZero();
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
                    product[i + j] = GenericGF.AddOrSubtract(product[i + j],
                            field.Multiply(aCoeff, bCoefficients[j]));
                }
            }
            return new GenericGFPoly(field, product);
        }

        internal GenericGFPoly Multiply(int scalar)
        {
            if (scalar == 0)
            {
                return field.GetZero();
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
            return new GenericGFPoly(field, product);
        }

        internal GenericGFPoly MultiplyByMonomial(int degree, int coefficient)
        {
            if (degree < 0)
            {
                throw new ArgumentException();
            }
            if (coefficient == 0)
            {
                return field.GetZero();
            }
            int size = coefficients.Length;
            int[] product = new int[size + degree];
            for (int i = 0; i < size; i++)
            {
                product[i] = field.Multiply(coefficients[i], coefficient);
            }
            return new GenericGFPoly(field, product);
        }

        internal GenericGFPoly[] Divide(GenericGFPoly other)
        {
            if (!field.Equals(other.field))
            {
                throw new ArgumentException(
                        "GenericGFPolys do not have same GenericGF field");
            }
            if (other.IsZero())
            {
                throw new ArgumentException("Divide by 0");
            }

            GenericGFPoly quotient = field.GetZero();
            GenericGFPoly remainder = this;

            int denominatorLeadingTerm = other.GetCoefficient(other.GetDegree());
            int inverseDenominatorLeadingTerm = field
                    .Inverse(denominatorLeadingTerm);

            while (remainder.GetDegree() >= other.GetDegree()
                    && !remainder.IsZero())
            {
                int degreeDifference = remainder.GetDegree() - other.GetDegree();
                int scale = field.Multiply(
                        remainder.GetCoefficient(remainder.GetDegree()),
                        inverseDenominatorLeadingTerm);
                GenericGFPoly term = other.MultiplyByMonomial(degreeDifference,
                        scale);
                GenericGFPoly iterationQuotient = field.BuildMonomial(
                        degreeDifference, scale);
                quotient = quotient.AddOrSubtract(iterationQuotient);
                remainder = remainder.AddOrSubtract(term);
            }

            return new GenericGFPoly[] { quotient, remainder };
        }

        public override String ToString()
        {
            StringBuilder result = new StringBuilder(8 * GetDegree());
            for (int degree = GetDegree(); degree >= 0; degree--)
            {
                int coefficient = GetCoefficient(degree);
                if (coefficient != 0)
                {
                    if (coefficient < 0)
                    {
                        result.Append(" - ");
                        coefficient = -coefficient;
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

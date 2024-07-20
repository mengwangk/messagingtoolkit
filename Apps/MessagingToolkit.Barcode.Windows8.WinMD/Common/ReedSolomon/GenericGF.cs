using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MessagingToolkit.Barcode.Common.ReedSolomon
{
    /// <summary>
    ///   <p>This class contains utility methods for performing mathematical operations over
    /// the Galois Fields. Operations use a given primitive polynomial in calculations.</p>
    ///   <p>Throughout this package, elements of the GF are represented as an <c>int</c>
    /// for convenience and speed (but at the cost of memory).
    ///   </p>
    ///   
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class GenericGF
    {
        public static readonly GenericGF AztecData12 = new GenericGF(0x1069, 4096, 1); // x^12 + x^6 + x^5 + x^3 + 1
        public static readonly GenericGF AztecData10 = new GenericGF(0x409, 1024, 1); // x^10 + x^3 + 1
        public static readonly GenericGF AztecData6 = new GenericGF(0x43, 64, 1); // x^6 + x + 1
        public static readonly GenericGF AztecParam = new GenericGF(0x13, 16, 1); // x^4 + x + 1
        public static readonly GenericGF QRCodeField256 = new GenericGF(0x011D, 256, 0); // x^8 + x^4 + x^3 + x^2 + 1
        public static readonly GenericGF DataMatrixField256 = new GenericGF(0x012D, 256, 1); // x^8 + x^5 + x^3 + x^2 + 1
        public static readonly GenericGF AztecData8 = DataMatrixField256;
        public static readonly GenericGF MaxicodeField64 = AztecData6;

        private const int InitializationThreshold = 0;

        private int[] expTable;
        private int[] logTable;
        private GenericGFPoly zero;
        private GenericGFPoly one;
        private readonly int size;
        private readonly int primitive;
        private readonly int generatorBase;
        private bool initialized = false;

        /// <summary>
        /// Create a representation of GF(size) using the given primitive polynomial.
        /// </summary>
        /// <param name="primitive">
        /// Irreducible polynomial whose coefficients are represented by the bits of an int, where the least-significant bit represents the constant
        /// coefficient
        /// </param>
        /// <param name="size">Size the size of the field</param>
        /// <param name="b">
        /// The factor b in the generator polynomial can be 0- or 1-based
        /// (g(x) = (x+a^b)(x+a^(b+1))...(x+a^(b+2t-1))).
        ///  In most cases it should be 1, but for QR code it is 0.
        /// </param>
        public GenericGF(int primitive, int size, int b)
        {
            this.primitive = primitive;
            this.size = size;
            this.generatorBase = b;

            if (size <= InitializationThreshold)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            expTable = new int[size];
            logTable = new int[size];
            int x = 1;
            for (int i = 0; i < size; i++)
            {
                expTable[i] = x;
                x <<= 1; // x = x * 2; we're assuming the generator alpha is 2
                if (x >= size)
                {
                    x ^= primitive;
                    x &= size - 1;
                }
            }
            for (int i_0 = 0; i_0 < size - 1; i_0++)
            {
                logTable[expTable[i_0]] = i_0;
            }
            // logTable[0] == 0 but this should never be used
            zero = new GenericGFPoly(this, new int[] { 0 });
            one = new GenericGFPoly(this, new int[] { 1 });
            initialized = true;
        }

        private void CheckInit()
        {
            if (!initialized)
            {
                Initialize();
            }
        }

        internal GenericGFPoly GetZero()
        {
            CheckInit();

            return zero;
        }

        internal GenericGFPoly GetOne()
        {
            CheckInit();

            return one;
        }


        /// <returns>the monomial representing coefficient /// x^degree</returns>
        internal GenericGFPoly BuildMonomial(int degree, int coefficient)
        {
            CheckInit();

            if (degree < 0)
            {
                throw new ArgumentException();
            }
            if (coefficient == 0)
            {
                return zero;
            }
            int[] coefficients = new int[degree + 1];
            coefficients[0] = coefficient;
            return new GenericGFPoly(this, coefficients);
        }

        /// <summary>
        /// Implements both addition and subtraction -- they are the same in GF(size).
        /// </summary>
        ///
        /// <returns>sum/difference of a and b</returns>
        static internal int AddOrSubtract(int a, int b)
        {
            return a ^ b;
        }


        /// <returns>2 to the power of a in GF(size)</returns>
        internal int Exp(int a)
        {
            CheckInit();

            return expTable[a];
        }


        /// <returns>base 2 log of a in GF(size)</returns>
        internal int Log(int a)
        {
            CheckInit();

            if (a == 0)
            {
                throw new ArgumentException();
            }
            return logTable[a];
        }


        /// <returns>multiplicative inverse of a</returns>
        internal int Inverse(int a)
        {
            CheckInit();

            if (a == 0)
            {
                throw new ArithmeticException();
            }
            return expTable[size - logTable[a] - 1];
        }


        /// <returns>product of a and b in GF(size)</returns>
        internal int Multiply(int a, int b)
        {
            CheckInit();

            if (a == 0 || b == 0)
            {
                return 0;
            }
            return expTable[(logTable[a] + logTable[b]) % (size - 1)];
        }

        public int GetSize()
        {
            return size;
        }

        public int GetGeneratorBase()
        {
            return generatorBase;
        }

        public override String ToString()
        {
            return "GF(0x" + primitive.ToString("X2") + ',' + size + ')';
        }
    }
}

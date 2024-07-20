using System;

namespace MessagingToolkit.Barcode.Pdf417.Decoder.Ec
{
    /// <summary>
    /// <p>A field based on powers of a generator integer, modulo some modulus.</p>
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class ModulusGF
    {
        public static ModulusGF PDF417_GF = new ModulusGF(Pdf417Common.NUMBER_OF_CODEWORDS, 3);

        private int[] expTable;
        private int[] logTable;
        private ModulusPoly zero;
        private ModulusPoly one;
        private int modulus;

        private ModulusGF(int modulus, int generator)
        {
            this.modulus = modulus;
            expTable = new int[modulus];
            logTable = new int[modulus];
            int x = 1;
            for (int i = 0; i < modulus; i++)
            {
                expTable[i] = x;
                x = (x * generator) % modulus;
            }
            for (int i = 0; i < modulus - 1; i++)
            {
                logTable[expTable[i]] = i;
            }
            // logTable[0] == 0 but this should never be used
            zero = new ModulusPoly(this, new int[] { 0 });
            one = new ModulusPoly(this, new int[] { 1 });
        }


        internal ModulusPoly GetZero()
        {
            return zero;
        }

        internal ModulusPoly GetOne()
        {
            return one;
        }

        internal ModulusPoly BuildMonomial(int degree, int coefficient)
        {
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
            return new ModulusPoly(this, coefficients);
        }

        internal int Add(int a, int b)
        {
            return (a + b) % modulus;
        }

        internal int Subtract(int a, int b)
        {
            return (modulus + a - b) % modulus;
        }

        internal int Exp(int a)
        {
            return expTable[a];
        }

        internal int Log(int a)
        {
            if (a == 0)
            {
                throw new ArgumentException();
            }
            return logTable[a];
        }

        internal int Inverse(int a)
        {
            if (a == 0)
            {
                throw new ArithmeticException();
            }
            return expTable[modulus - logTable[a] - 1];
        }

        internal int Multiply(int a, int b)
        {
            if (a == 0 || b == 0)
            {
                return 0;
            }
            return expTable[(logTable[a] + logTable[b]) % (modulus - 1)];
        }

        internal int Size
        {
            get
            {
                return modulus;
            }
        }
    }
}
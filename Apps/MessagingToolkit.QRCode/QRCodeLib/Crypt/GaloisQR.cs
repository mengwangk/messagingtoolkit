
using System;

namespace MessagingToolkit.QRCode.Crypt
{
    public sealed class GaloisQR : Galois
    {
        public const int POLYNOMIAL = 0x1d; // x^8 + x^4 + x^3 + x^2 + 1
        private static readonly Galois instance = new GaloisQR();

        private GaloisQR()
            : base(POLYNOMIAL, 0)
        {
        }

        public static Galois GetInstance()
        {
            return instance;
        }
    }
}



using System;

namespace MessagingToolkit.QRCode.Crypt
{
    public sealed class BCH_15_5
    {
        private static readonly int GX = 0x137;
        private static readonly BCH_15_5 instance = new BCH_15_5();
        private int[] trueCodes = new int[32];

        /// <summary>
        /// Prevents a default instance of the <see cref="BCH_15_5"/> class from being created.
        /// </summary>
        private BCH_15_5()
        {
            MakeTrueCodes();
        }

        public static BCH_15_5 GetInstance()
        {
            return instance;
        }


        private void MakeTrueCodes()
        {
            for (int i = 0; i < trueCodes.Length; i++)
            {
                trueCodes[i] = SlowEncode(i);
            }
        }

        private int SlowEncode(int data)
        {
            int wk = 0;
            data <<= 5;
            for (int i = 0; i < 5; i++)
            {
                wk <<= 1;
                data <<= 1;
                if (((wk ^ data) & 0x400) != 0)
                {
                    wk ^= GX;
                }
            }
            return (data & 0x7c00) | (wk & 0x3ff);
        }

        /// <summary>
        /// Encodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public int Encode(int data)
        {
            return trueCodes[data & 0x1f];
        }


        /// <summary>
        /// Calcs the distance.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <returns></returns>
        private static int CalcDistance(int c1, int c2)
        {
            int n = 0;
            int wk = c1 ^ c2;
            while (wk != 0)
            {
                if ((wk & 1) != 0)
                {
                    n++;
                }
                wk >>= 1;
            }
            return n;
        }


        /// <summary>
        /// Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public int Decode(int data)
        {
            data &= 0x7fff;
            for (int i = 0; i < trueCodes.Length; i++)
            {
                int code = trueCodes[i];
                if (CalcDistance(data, code) <= 3)
                {
                    return code;
                }
            }
            return -1;
        }
    }
}

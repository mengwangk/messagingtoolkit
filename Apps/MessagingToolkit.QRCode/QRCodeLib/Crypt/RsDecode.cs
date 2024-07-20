using System;

namespace MessagingToolkit.QRCode.Crypt
{
    /// <summary>
    /// ReedSolomon Decoder
    /// </summary>
    public class RsDecode
    {
        public static readonly int RS_PERM_ERROR = -1;
        public const int RS_CORRECT_ERROR = -2;
        private Galois galois;
        private int npar;

        /// <summary>
        /// Initializes a new instance of the <see cref="RsDecode"/> class.
        /// </summary>
        /// <param name="galois">The galois.</param>
        /// <param name="npar">The npar.</param>
        public RsDecode(Galois galois, int npar)
        {
            this.galois = GaloisQR.GetInstance();
            this.galois = galois;
            this.npar = npar;
            if (npar <= 0 || npar >= 128)
            {
                throw new ArgumentException("bad npar");
            }
        }

        public RsDecode(int npar)
        {
            this.galois = GaloisQR.GetInstance();
            this.npar = npar;
            if (npar <= 0 || npar >= 128)
            {
                throw new ArgumentException("bad npar");
            }
        }

        /// <summary>
        /// Calculates Sigma(z), Omega(z) from Syndrome
        /// (Modified Berlekamp-Massey Algorithm)
        /// </summary>
        ///
        /// <param name="syn">s0,s1,s2, ... s<npar-1></param>
        /// <returns>int[]
        /// null: fail
        /// int[]: sigma(z)</returns>
        public int[] CalcSigmaMBM(int[] syn)
        {
            int[] sg0 = new int[npar + 1];
            int[] sg1 = new int[npar + 1];
            int[] wk = new int[npar + 1];
            sg0[1] = 1;
            sg1[0] = 1;
            int jisu0 = 1;
            int jisu1 = 0;
            int m = -1;

            for (int n = 0; n < npar; n++)
            {
                int d = syn[n];
                for (int i = 1; i <= jisu1; i++)
                {
                    d ^= galois.Mul(sg1[i], syn[n - i]);
                }
                if (d != 0)
                {
                    int logd = galois.ToLog(d);
                    for (int i_0 = 0; i_0 <= n; i_0++)
                    {
                        wk[i_0] = sg1[i_0] ^ galois.MulExp(sg0[i_0], logd);
                    }
                    int js = n - m;
                    if (js > jisu1)
                    {
                        for (int i_1 = 0; i_1 <= jisu0; i_1++)
                        {
                            sg0[i_1] = galois.DivExp(sg1[i_1], logd);
                        }
                        m = n - jisu1;
                        jisu1 = js;
                        jisu0 = js;
                    }
                    int[] tmp = sg1;
                    sg1 = wk;
                    wk = tmp;
                }
                System.Array.Copy((Array)(sg0), 0, (Array)(sg0), 1, jisu0);
                sg0[0] = 0;
                jisu0++;
            }
            if (sg1[jisu1] == 0)
            {
                return null;
            }
            int[] sigma = new int[jisu1 + 1];
            System.Array.Copy((Array)(sg1), 0, (Array)(sigma), 0, jisu1 + 1);
            return sigma;
        }

        private int[] ChienSearch(int length, int start, int wa, int seki)
        {
            for (int i = start; i < length; i++)
            {
                int z0 = galois.ToExp(i);
                int z1 = wa ^ z0;
                if (galois.MulExp(z1, i) == seki)
                {
                    int idx = galois.ToLog(z1);
                    if (idx <= i || idx >= length)
                    {
                        return null;
                    }
                    return new int[] { z1, z0 };
                }
            }
            return null;
        }

        /// <summary>
        /// Calculates Error Location(s)
        /// (Chien Search Algorithm)
        /// </summary>
        ///
        /// <param name="length"></param>
        /// <param name="sigma">int[]</param>
        /// <returns>int
        /// null: fail
        /// int[] error locations</returns>
        private int[] ChienSearch(int length, int[] sigma)
        {
            int jisu = sigma.Length - 1;
            int wa = sigma[1];
            int seki = sigma[jisu];
            if (jisu == 1)
            {
                if (galois.ToLog(wa) >= length)
                {
                    return null;
                }
                return new int[] { wa };
            }
            if (jisu == 2)
            {
                return ChienSearch(length, 0, wa, seki);
            }

            int[] pos = new int[jisu];
            int posIdx = jisu - 1;
            for (int i = 0, z = 255; i < length; i++, z--)
            {
                int wk = 1;
                for (int j = 1, wz = z; j <= jisu; j++, wz = (wz + z) % 255)
                {
                    wk ^= galois.MulExp(sigma[j], wz);
                }
                if (wk == 0)
                {
                    int pv = galois.ToExp(i);
                    wa ^= pv;
                    seki = galois.Div(seki, pv);
                    pos[posIdx--] = pv;
                    if (posIdx == 1)
                    {
                        int[] pos2 = ChienSearch(length, i + 1, wa, seki);
                        if (pos2 == null)
                        {
                            return null;
                        }
                        pos[0] = pos2[0];
                        pos[1] = pos2[1];
                        return pos;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Calculates Error Magnitude(s) and Corrects Error(s)
        /// (Forney Algorithm)
        /// </summary>
        ///
        /// <param name="data">int[]</param>
        /// <param name="length"></param>
        /// <param name="pos"></param>
        /// <param name="sigma">int[]</param>
        /// <param name="omega">int[]</param>
        private void DoForney(int[] data, int length, int[] pos, int[] sigma, int[] omega)
        {
            /* foreach */
            foreach (int ps in pos)
            {
                int zlog = 255 - galois.ToLog(ps);
                int ov = galois.CalcOmegaValue(omega, zlog);
                int dv = galois.CalcSigmaDashValue(sigma, zlog);
                data[galois.ToPos(length, ps)] ^= galois.DivExp(galois.Div(ov, dv), zlog);
            }
        }

        /// <summary>
        /// Decoding ReedSolomon Code
        /// </summary>
        ///
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="noCorrect"></param>
        /// <returns>int
        /// 0: has no error
        /// > 0: # of corrected
        /// < 0: fail</returns>
        public int Decode(int[] data, int length, bool noCorrect)
        {
            if (length < npar || length > 255)
            {
                return RS_PERM_ERROR;
            }
            int[] syn = new int[npar];
            if (galois.CalcSyndrome(data, length, syn))
            {
                return 0;
            }

            int[] sigma = CalcSigmaMBM(syn);
            if (sigma == null)
            {
                return RS_CORRECT_ERROR;
            }

            int[] pos = ChienSearch(length, sigma);
            if (pos == null)
            {
                return RS_CORRECT_ERROR;
            }

            if (!noCorrect)
            {
                int[] omega = galois.MulPoly(syn, sigma, sigma.Length - 1);
                DoForney(data, length, pos, sigma, omega);
            }
            return sigma.Length - 1;
        }

        public int Decode(int[] data, int length)
        {
            return Decode(data, length, false);
        }

        public int Decode(int[] data, bool noCorrect)
        {
            return Decode(data, data.Length, noCorrect);
        }

        public int Decode(int[] data)
        {
            return Decode(data, data.Length, false);
        }

       
    }

}
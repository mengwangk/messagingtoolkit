
using System;

namespace MessagingToolkit.QRCode.Crypt
{
    public class Galois
    {
        private readonly int polynomial;
        private readonly int symStart;
        private int[] expTbl;
        private int[] logTbl;

        protected internal Galois(int polynomial_0, int symStart_1)
        {
            this.expTbl = new int[255 * 2];
            this.logTbl = new int[255 + 1];
            this.polynomial = polynomial_0;
            this.symStart = symStart_1;
            InitGaloisTable();
        }

        private void InitGaloisTable()
        {
            int d = 1;
            for (int i = 0; i < 255; i++)
            {
                expTbl[i] = expTbl[255 + i] = d;
                logTbl[d] = i;
                d <<= 1;
                if ((d & 0x100) != 0)
                {
                    d = (d ^ polynomial) & 0xff;
                }
            }
        }

        public int ToExp(int a)
        {
            return expTbl[a];
        }

        public int ToLog(int a)
        {
            return logTbl[a];
        }

        public int ToPos(int length, int a)
        {
            return length - 1 - logTbl[a];
        }

        public int Mul(int a, int b)
        {
            return (a == 0 || b == 0) ? 0 : expTbl[logTbl[a] + logTbl[b]];
        }

        public int MulExp(int a, int b)
        {
            return (a == 0) ? 0 : expTbl[logTbl[a] + b];
        }

        public int Div(int a, int b)
        {
            return (a == 0) ? 0 : expTbl[logTbl[a] - logTbl[b] + 255];
        }

        public int DivExp(int a, int b)
        {
            return (a == 0) ? 0 : expTbl[logTbl[a] - b + 255];
        }

        public int Inv(int a)
        {
            return expTbl[255 - logTbl[a]];
        }

        public int[] MulPoly(int[] a, int[] b, int jisu)
        {
            int[] seki = new int[jisu];
            int ia2 = Math.Min(jisu, a.Length);
            for (int ia = 0; ia < ia2; ia++)
            {
                if (a[ia] != 0)
                {
                    int loga = logTbl[a[ia]];
                    int ib2 = Math.Min(b.Length, jisu - ia);
                    for (int ib = 0; ib < ib2; ib++)
                    {
                        if (b[ib] != 0)
                        {
                            seki[ia + ib] ^= expTbl[loga + logTbl[b[ib]]];
                        }
                    }
                }
            }
            return seki;
        }

        public bool CalcSyndrome(int[] data, int length, int[] syn)
        {
            int hasErr = 0;
            for (int i = 0, s = symStart; i < syn.Length; i++, s++)
            {
                int wk = 0;
                for (int idx = 0; idx < length; idx++)
                {
                    if (wk != 0)
                    {
                        wk = expTbl[logTbl[wk] + s];
                    }
                    wk ^= data[idx];
                }
                syn[i] = wk;
                hasErr |= wk;
            }
            return hasErr == 0;
        }

        public int[] MakeEncodeGx(int npar)
        {
            int[] encodeGx = new int[npar];
            encodeGx[npar - 1] = 1;
            for (int i = 0, kou = symStart; i < npar; i++, kou++)
            {
                int ex = ToExp(kou);
                for (int j = 0; j < npar - 1; j++)
                {
                    encodeGx[j] = Mul(encodeGx[j], ex) ^ encodeGx[j + 1];
                }
                encodeGx[npar - 1] = Mul(encodeGx[npar - 1], ex);
            }
            return encodeGx;
        }

        public int CalcOmegaValue(int[] omega, int zlog)
        {
            int wz = zlog;
            int ov = omega[0];
            for (int i = 1; i < omega.Length; i++)
            {
                ov ^= MulExp(omega[i], wz);
                wz = (wz + zlog) % 255;
            }
            if (symStart != 0)
            {
                ov = MulExp(ov, (zlog * symStart) % 255);
            }
            return ov;
        }

        public int CalcSigmaDashValue(int[] sigma, int zlog)
        {
            int jisu = sigma.Length - 1;
            int zlog2 = (zlog * 2) % 255;
            int wz = zlog2;
            int dv = sigma[1];
            for (int i = 3; i <= jisu; i += 2)
            {
                dv ^= MulExp(sigma[i], wz);
                wz = (wz + zlog2) % 255;
            }
            return dv;
        }
    }
}


using System;

namespace MessagingToolkit.QRCode.Crypt
{
    public sealed class Galois
    {
        public static readonly int POLYNOMIAL = 0x1d;
        private static readonly Galois instance = new Galois();
        private int[] expTbl = new int[255 * 2];	
        private int[] logTbl = new int[255 + 1];

        private Galois()
        {
            InitGaloisTable();
        }

        public static Galois GetInstance()
        {
            return instance;
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
                    d = (d ^ POLYNOMIAL) & 0xff;
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

     
        public void MulPoly(int[] seki, int[] a, int[] b)
        {
            for (int i = 0; i < seki.Length; i++)
            {
                seki[i] = 0;
            }           
            for (int ia = 0; ia < a.Length; ia++)
            {
                if (a[ia] != 0)
                {
                    int loga = logTbl[a[ia]];
                    int ib2 = Math.Min(b.Length, seki.Length - ia);
                    for (int ib = 0; ib < ib2; ib++)
                    {
                        if (b[ib] != 0)
                        {
                            seki[ia + ib] ^= expTbl[loga + logTbl[b[ib]]];	// = a[ia] * b[ib]
                        }
                    }
                }
            }
        }
       
        public bool CalcSyndrome(int[] data, int length, int[] syn)
        {
            int hasErr = 0;
            for (int i = 0; i < syn.Length; i++)
            {
                int wk = 0;
                for (int idx = 0; idx < length; idx++)
                {
                    wk = data[idx] ^ ((wk == 0) ? 0 : expTbl[logTbl[wk] + i]);		// wk = data + wk * α^i
                }
                syn[i] = wk;
                hasErr |= wk;
            }
            return hasErr == 0;
        }
    }
}
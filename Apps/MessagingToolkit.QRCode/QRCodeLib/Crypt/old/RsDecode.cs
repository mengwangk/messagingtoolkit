using System;

namespace MessagingToolkit.QRCode.Crypt
{
    public class RsDecode
    {
        public static readonly int RS_PERM_ERROR = -1;
        public static readonly int RS_CORRECT_ERROR = -2;
        private static readonly Galois galois = Galois.GetInstance();
        private int npar;

        /// <summary>
        /// Initializes a new instance of the <see cref="RsDecode"/> class.
        /// </summary>
        /// <param name="npar">The npar.</param>
        public RsDecode(int npar)
        {
            this.npar = npar;
        }

        public int CalcSigmaMBM(int[] sigma, int[] omega, int[] syn)
        {
            int[] sg0 = new int[npar];
            int[] sg1 = new int[npar];
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
                    int[] wk = new int[npar];
                    for (int i = 0; i <= n; i++)
                    {
                        wk[i] = sg1[i] ^ galois.MulExp(sg0[i], logd);
                    }
                    int js = n - m;
                    if (js > jisu1)
                    {
                        m = n - jisu1;
                        jisu1 = js;
                        if (jisu1 > npar / 2)
                        {
                            return -1;				
                        }
                        for (int i = 0; i <= jisu0; i++)
                        {
                            sg0[i] = galois.DivExp(sg1[i], logd);
                        }
                        jisu0 = jisu1;
                    }
                    sg1 = wk;
                }
                Array.Copy(sg0, 0, sg0, 1, Math.Min(sg0.Length - 1, jisu0));
                sg0[0] = 0;
                jisu0++;
            }
            galois.MulPoly(omega, sg1, syn);
            Array.Copy(sg1, 0, sigma, 0, Math.Min(sg1.Length, sigma.Length));
            return jisu1;
        }

       
        private int ChienSearch(int[] pos, int n, int jisu, int[] sigma)
        {  
            int last = sigma[1];

            if (jisu == 1)
            {
                
                if (galois.ToLog(last) >= n)
                {
                    return RS_CORRECT_ERROR;	
                }
                pos[0] = last;
                return 0;
            }

            int posIdx = jisu - 1;		
            for (int i = 0; i < n; i++)
            {
               
                int z = 255 - i;					
                int wk = 1;
                for (int j = 1; j <= jisu; j++)
                {
                    wk ^= galois.MulExp(sigma[j], (z * j) % 255);
                }
                if (wk == 0)
                {
                    int pv = galois.ToExp(i);		
                    last ^= pv;					
                    pos[posIdx--] = pv;
                    if (posIdx == 0)
                    {                       
                        if (galois.ToLog(last) >= n)
                        {
                            return RS_CORRECT_ERROR;	
                        }
                        pos[0] = last;
                        return 0;
                    }
                }
            }
            
            return RS_CORRECT_ERROR;
        }

      
        private void DoForney(int[] data, int length, int jisu, int[] pos, int[] sigma, int[] omega)
        {
            for (int i = 0; i < jisu; i++)
            {
                int ps = pos[i];
                int zlog = 255 - galois.ToLog(ps);					

            
                int ov = omega[0];
                for (int j = 1; j < jisu; j++)
                {
                    ov ^= galois.MulExp(omega[j], (zlog * j) % 255);		
                }

             
                int dv = sigma[1];
                for (int j = 2; j < jisu; j += 2)
                {
                    dv ^= galois.MulExp(sigma[j + 1], (zlog * j) % 255);	
                }

               
                data[galois.ToPos(length, ps)] ^= galois.Mul(ps, galois.Div(ov, dv));
            }
        }

      
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
          
            int[] sigma = new int[npar / 2 + 2];
            int[] omega = new int[npar / 2 + 1];
            int jisu = CalcSigmaMBM(sigma, omega, syn);
            if (jisu <= 0)
            {
                return RS_CORRECT_ERROR;
            }
           
            int[] pos = new int[jisu];
            int r = ChienSearch(pos, length, jisu, sigma);
            if (r < 0)
            {
                return r;
            }
            if (!noCorrect)
            {
                DoForney(data, length, jisu, pos, sigma, omega);
            }
            return jisu;
        }

        public int Decode(int[] data, int length)
        {
            return Decode(data, length, false);
        }

        public int Decode(int[] data)
        {
            return Decode(data, data.Length, false);
        }


    }
}

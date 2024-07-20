using System;

namespace MessagingToolkit.QRCode.Crypt
{
    public class RsEncode
    {
        public static readonly int RS_PERM_ERROR = -1;
        private static readonly Galois galois = Galois.GetInstance();
        private int npar;
        private int[] encodeGx;

        public RsEncode(int npar)
        {
            this.npar = npar;
            MakeEncodeGx();
        }

       
        private void MakeEncodeGx()
        {
            encodeGx = new int[npar];
            encodeGx[npar - 1] = 1;
            for (int kou = 0; kou < npar; kou++)
            {
                int ex = galois.ToExp(kou);		
              
                for (int i = 0; i < npar - 1; i++)
                {                   
                    encodeGx[i] = galois.Mul(encodeGx[i], ex) ^ encodeGx[i + 1];
                }
                encodeGx[npar - 1] = galois.Mul(encodeGx[npar - 1], ex);		
            }
        }

        
        public int Encode(int[] data, int length, int[] parity, int parityStartPos)
        {
            if (length < 0 || length + npar > 255)
            {
                return RS_PERM_ERROR;
            }

           
            int[] wr = new int[npar];

            for (int idx = 0; idx < length; idx++)
            {
                int code = data[idx];
                int ib = wr[0] ^ code;
                for (int i = 0; i < npar - 1; i++)
                {
                    wr[i] = wr[i + 1] ^ galois.Mul(ib, encodeGx[i]);
                }
                wr[npar - 1] = galois.Mul(ib, encodeGx[npar - 1]);
            }
            if (parity != null)
            {
                Array.Copy(wr, 0, parity, parityStartPos, npar);
            }
            return 0;
        }

        public int Encode(int[] data, int length, int[] parity)
        {
            return Encode(data, length, parity, 0);
        }

        public int Encode(int[] data, int[] parity)
        {
            return Encode(data, data.Length, parity, 0);
        }      
    }
}


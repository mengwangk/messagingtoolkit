using System;

namespace MessagingToolkit.QRCode.Codec.Ecc
{
    public class ReedSolomon
    {
        virtual public bool CorrectionSucceeded
        {
            get
            {
                return correctionSucceeded;
            }

        }
        virtual public int NumCorrectedErrors
        {
            get
            {
                return NErrors;
            }

        }
        //G(x)=a^8+a^4+a^3+a^2+1
        internal int[] y;

        internal int[] gexp = new int[512];
        internal int[] glog = new int[256];
        internal int NPAR;
        //final int NPAR = 15;
        internal int MAXDEG;
        internal int[] synBytes;

        /* The Error Locator Polynomial, also known as Lambda or Sigma. Lambda[0] == 1 */
        internal int[] Lambda;

        /* The Error Evaluator Polynomial */
        internal int[] Omega;

        /* local ANSI declarations */

        /* error locations found using Chien's search*/
        internal int[] ErrorLocs = new int[256];
        internal int NErrors;

        /* erasure flags */
        internal int[] ErasureLocs = new int[256];
        internal int NErasures = 0;

        internal bool correctionSucceeded = true;

        public ReedSolomon(int[] source, int NPAR)
        {
            InitializeGaloisTables();
            y = source;
            this.NPAR = NPAR;
            MAXDEG = NPAR * 2;
            synBytes = new int[MAXDEG];
            Lambda = new int[MAXDEG];
            Omega = new int[MAXDEG];
        }

        internal virtual void InitializeGaloisTables()
        {
            int i, z;
            int pinit, p1, p2, p3, p4, p5, p6, p7, p8;

            pinit = p2 = p3 = p4 = p5 = p6 = p7 = p8 = 0;
            p1 = 1;

            gexp[0] = 1;
            gexp[255] = gexp[0];
            glog[0] = 0; /* shouldn't log[0] be an error? */

            for (i = 1; i < 256; i++)
            {
                pinit = p8;
                p8 = p7;
                p7 = p6;
                p6 = p5;
                p5 = p4 ^ pinit;
                p4 = p3 ^ pinit;
                p3 = p2 ^ pinit;
                p2 = p1;
                p1 = pinit;
                gexp[i] = p1 + p2 * 2 + p3 * 4 + p4 * 8 + p5 * 16 + p6 * 32 + p7 * 64 + p8 * 128;
                gexp[i + 255] = gexp[i];
            }

            for (i = 1; i < 256; i++)
            {
                for (z = 0; z < 256; z++)
                {
                    if (gexp[z] == i)
                    {
                        glog[i] = z;
                        break;
                    }
                }
            }
        }

        /* multiplication using logarithms */
        internal virtual int GMult(int a, int b)
        {
            int i, j;
            if (a == 0 || b == 0)
                return (0);
            i = glog[a];
            j = glog[b];
            return (gexp[i + j]);
        }


        internal virtual int GInv(int elt)
        {
            return (gexp[255 - glog[elt]]);
        }



        internal virtual void DecodeData(int[] data)
        {
            int i, j, sum;
            for (j = 0; j < MAXDEG; j++)
            {
                sum = 0;
                for (i = 0; i < data.Length; i++)
                {
                    sum = data[i] ^ GMult(gexp[j + 1], sum);
                }
                synBytes[j] = sum;
            }
        }

        public virtual void Correct()
        {
            //	
            DecodeData(y);
            correctionSucceeded = true;
            bool hasError = false;
            for (int i = 0; i < synBytes.Length; i++)
            {
                //Console.out.println("SyndromeS"+String.valueOf(i) + " = " + synBytes[i]);
                if (synBytes[i] != 0)
                    hasError = true;
            }
            if (hasError)
                correctionSucceeded = CorrectErrorsErasures(y, y.Length, 0, new int[1]);
        }

        internal virtual void ModifiedBerlekampMassey()
        {
            int n, L, L2, k, d, i;
            int[] psi = new int[MAXDEG];
            int[] psi2 = new int[MAXDEG];
            int[] D = new int[MAXDEG];
            int[] gamma = new int[MAXDEG];

            /* initialize Gamma, the erasure locator polynomial */
            InitGamma(gamma);

            /* initialize to z */
            CopyPoly(D, gamma);
            MulZPoly(D);

            CopyPoly(psi, gamma);
            k = -1; L = NErasures;

            for (n = NErasures; n < 8; n++)
            {

                d = ComputeDiscrepancy(psi, synBytes, L, n);

                if (d != 0)
                {

                    /* psi2 = psi - d*D */
                    for (i = 0; i < MAXDEG; i++)
                        psi2[i] = psi[i] ^ GMult(d, D[i]);


                    if (L < (n - k))
                    {
                        L2 = n - k;
                        k = n - L;
                        /* D = scale_poly(ginv(d), psi); */
                        for (i = 0; i < MAXDEG; i++)
                            D[i] = GMult(psi[i], GInv(d));
                        L = L2;
                    }

                    /* psi = psi2 */
                    for (i = 0; i < MAXDEG; i++)
                        psi[i] = psi2[i];
                }

                MulZPoly(D);
            }

            for (i = 0; i < MAXDEG; i++)
                Lambda[i] = psi[i];
            ComputeModifiedOmega();
        }

        /* given Psi (called Lambda in Modified_Berlekamp_Massey) and synBytes,
        compute the combined erasure/error evaluator polynomial as 
        Psi*S mod z^4
        */
        internal virtual void ComputeModifiedOmega()
        {
            int i;
            int[] product = new int[MAXDEG * 2];

            MultPolys(product, Lambda, synBytes);
            ZeroPoly(Omega);
            for (i = 0; i < NPAR; i++)
                Omega[i] = product[i];
        }

        /* polynomial multiplication */
        internal virtual void MultPolys(int[] dst, int[] p1, int[] p2)
        {
            int i, j;
            int[] tmp1 = new int[MAXDEG * 2];

            for (i = 0; i < (MAXDEG * 2); i++)
                dst[i] = 0;

            for (i = 0; i < MAXDEG; i++)
            {
                for (j = MAXDEG; j < (MAXDEG * 2); j++)
                    tmp1[j] = 0;

                /* scale tmp1 by p1[i] */
                for (j = 0; j < MAXDEG; j++)
                    tmp1[j] = GMult(p2[j], p1[i]);
                /* and mult (shift) tmp1 right by i */
                for (j = (MAXDEG * 2) - 1; j >= i; j--)
                    tmp1[j] = tmp1[j - i];
                for (j = 0; j < i; j++)
                    tmp1[j] = 0;

                /* add into partial product */
                for (j = 0; j < (MAXDEG * 2); j++)
                    dst[j] ^= tmp1[j];
            }
        }

        /* gamma = product (1-z*a^Ij) for erasure locs Ij */
        internal virtual void InitGamma(int[] gamma)
        {
            int e;
            int[] tmp = new int[MAXDEG];

            ZeroPoly(gamma);
            ZeroPoly(tmp);
            gamma[0] = 1;

            for (e = 0; e < NErasures; e++)
            {
                CopyPoly(tmp, gamma);
                ScalePoly(gexp[ErasureLocs[e]], tmp);
                MulZPoly(tmp);
                AddPolys(gamma, tmp);
            }
        }



        internal virtual void ComputeNextOmega(int d, int[] A, int[] dst, int[] src)
        {
            int i;
            for (i = 0; i < MAXDEG; i++)
            {
                dst[i] = src[i] ^ GMult(d, A[i]);
            }
        }



        internal virtual int ComputeDiscrepancy(int[] lambda, int[] S, int L, int n)
        {
            int i, sum = 0;

            for (i = 0; i <= L; i++)
                sum ^= GMult(lambda[i], S[n - i]);
            return (sum);
        }

        /// <summary>******* polynomial arithmetic ******************</summary>

        internal virtual void AddPolys(int[] dst, int[] src)
        {
            int i;
            for (i = 0; i < MAXDEG; i++)
                dst[i] ^= src[i];
        }

        internal virtual void CopyPoly(int[] dst, int[] src)
        {
            int i;
            for (i = 0; i < MAXDEG; i++)
                dst[i] = src[i];
        }

        internal virtual void ScalePoly(int k, int[] poly)
        {
            int i;
            for (i = 0; i < MAXDEG; i++)
                poly[i] = GMult(k, poly[i]);
        }


        internal virtual void ZeroPoly(int[] poly)
        {
            int i;
            for (i = 0; i < MAXDEG; i++)
                poly[i] = 0;
        }


        /* multiply by z, i.e., shift right by 1 */
        internal virtual void MulZPoly(int[] src)
        {
            int i;
            for (i = MAXDEG - 1; i > 0; i--)
                src[i] = src[i - 1];
            src[0] = 0;
        }


        /* Finds all the roots of an error-locator polynomial with coefficients
        * Lambda[j] by evaluating Lambda at successive values of alpha. 
        * 
        * This can be tested with the decoder's equations case.
        */
        internal virtual void FindRoots()
        {
            int sum, r, k;
            NErrors = 0;

            for (r = 1; r < 256; r++)
            {
                sum = 0;
                /* evaluate lambda at r */
                for (k = 0; k < NPAR + 1; k++)
                {
                    sum ^= GMult(gexp[(k * r) % 255], Lambda[k]);
                }
                if (sum == 0)
                {
                    ErrorLocs[NErrors] = (255 - r); NErrors++;
                    //if (DEBUG) fprintf(stderr, "Root found at r = %d, (255-r) = %d\n", r, (255-r));
                }
            }
        }

        /* 
        * Combined Erasure And Error Magnitude Computation 
        * 
        * Pass in the codeword, its size in bytes, as well as
        * an array of any known erasure locations, along the number
        * of these erasures.
        * 
        * Evaluate Omega(actually Psi)/Lambda' at the roots
        * alpha^(-i) for error locs i. 
        *
        * Returns 1 if everything ok, or 0 if an out-of-bounds error is found
        *
        */
        internal virtual bool CorrectErrorsErasures(int[] codeword, int csize, int nerasures, int[] erasures)
        {
            int r, i, j, err;

            /* If you want to take advantage of erasure correction, be sure to
            set NErasures and ErasureLocs[] with the locations of erasures. 
            */
            NErasures = nerasures;
            for (i = 0; i < NErasures; i++)
                ErasureLocs[i] = erasures[i];

            ModifiedBerlekampMassey();
            FindRoots();


            if ((NErrors <= NPAR) || NErrors > 0)
            {

                /* first check for illegal error locs */
                for (r = 0; r < NErrors; r++)
                {
                    if (ErrorLocs[r] >= csize)
                    {
                        //if (DEBUG) fprintf(stderr, "Error loc i=%d outside of codeword length %d\n", i, csize);
                        //Console.out.println("Error loc i="+ErrorLocs[r]+" outside of codeword length"+csize);
                        return false;
                    }
                }

                for (r = 0; r < NErrors; r++)
                {
                    int num, denom;
                    i = ErrorLocs[r];
                    /* evaluate Omega at alpha^(-i) */

                    num = 0;
                    for (j = 0; j < MAXDEG; j++)
                        num ^= GMult(Omega[j], gexp[((255 - i) * j) % 255]);

                    /* evaluate Lambda' (derivative) at alpha^(-i) ; all odd powers disappear */
                    denom = 0;
                    for (j = 1; j < MAXDEG; j += 2)
                    {
                        denom ^= GMult(Lambda[j], gexp[((255 - i) * (j - 1)) % 255]);
                    }

                    err = GMult(num, GInv(denom));
                    //if (DEBUG) fprintf(stderr, "Error magnitude %#x at loc %d\n", err, csize-i);

                    codeword[csize - i - 1] ^= err;
                }
                //for (int p = 0; p < codeword.length; p++)
                //	Console.out.println(codeword[p]);
                //Console.out.println("correction succeeded");
                return true;
            }
            else
            {
                //if (DEBUG && NErrors) fprintf(stderr, "Uncorrectable codeword\n");
                //Console.out.println("Uncorrectable codeword");
                return false;
            }
        }
    }
}
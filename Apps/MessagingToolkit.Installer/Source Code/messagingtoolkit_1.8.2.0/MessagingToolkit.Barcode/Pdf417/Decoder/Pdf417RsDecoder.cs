using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    /// <summary>
    /// Refer here
    /// 
    /// http://code.google.com/p/zxing/issues/detail?id=817
    /// 
    /// </summary>
    public sealed class Pdf417RsDecoder
    {
        private static readonly int PRIM = 1;
        private static readonly int GPRIME = 929;
        private static readonly int A0 = 928;

        private int[] AlphaTo = new int[1024];
        private int[] IndexOf = new int[1024];

        public Pdf417RsDecoder()
        {
            int ii;
            int powerOf3;
            powerOf3 = 1;
            IndexOf[1] = GPRIME - 1;

            for (ii = 0; ii < GPRIME - 1; ii += 1)
            {
                AlphaTo[ii] = powerOf3;
                if (powerOf3 < GPRIME)
                {
                    if (ii != GPRIME - 1) IndexOf[powerOf3] = ii;
                }

                powerOf3 = (powerOf3 * 3) % GPRIME;
            }
            IndexOf[0] = GPRIME - 1;
            AlphaTo[GPRIME - 1] = 1;
            IndexOf[GPRIME] = A0;
        }

        private int Modbase(int x)
        {
            return ((x) % (GPRIME - 1));
        }



        /// <summary>
        /// Performs ERRORS+ERASURES decoding of RS codes. If decoding is successful,
        /// writes the codeword into data[] itself. Otherwise data[] is unaltered.
        ///
        /// Return number of symbols corrected, or -1 if codeword is illegal
        /// or uncorrectable. If eras_pos is non-null, the detected error locations
        /// are written back. NOTE! This array must be at least NN-KK elements long.
        /// 
        /// First "no_eras" erasures are declared by the calling program. Then, the
        /// maximum # of errors correctable is t_after_eras = floor((NN-KK-no_eras)/2).
        /// If the number of channel errors is not greater than "t_after_eras" the
        /// transmitted codeword will be recovered. Details of algorithm can be found
        /// in R. Blahut's "Theory ... of Error-Correcting Codes".
        ///
        /// Warning: the eras_pos[] array must not contain duplicate entries; decoder failure
        /// will result. The decoder could check for this condition, but it would involve
        /// extra time on every decoding operation.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="eras_pos">The eras_pos.</param>
        /// <param name="no_eras">The no_eras.</param>
        /// <param name="data_len">The data_len.</param>
        /// <param name="syndLen">The synd len.</param>
        /// <returns></returns>
        public int CorrectErrors(int[] data, int[] eras_pos, int no_eras, int data_len, int syndLen)
        {
            int degLambda, el, degOmega;
            int i, j, r, k;
            int u, q, tmp, num1, num2, den, discrR;
            int[] lambda = new int[2048 + 1];
            int[] s = new int[2048 + 1];	/* Err+Eras Locator poly
                                            * and syndrome poly */
            int[] b = new int[2048 + 1];
            int[] t = new int[2048 + 1];
            int[] omega = new int[2048 + 1];
            int[] root = new int[2048];
            int[] reg = new int[2048 + 1];
            int[] loc = new int[2048];
            int synError, count;
            int ci;
            int errorVal;
            int fixLoc;

            /* form the syndromes; i.e. evaluate data(x) at roots of g(x)
             namely @**(1+i)*PRIM, i = 0, ... , (NN-KK-1) */
            for (i = 1; i <= syndLen; i++)
            {
                s[i] = 0;//data[data_len];
            }

            for (j = 1; j <= data_len; j++)
            {

                if (data[data_len - j] == 0) continue;

                tmp = IndexOf[data[data_len - j]];

                /*  s[i] ^= Alpha_to[modbase(tmp + (1+i-1)*j)]; */
                for (i = 1; i <= syndLen; i++)
                {
                    s[i] = (s[i] + AlphaTo[Modbase(tmp + (i) * j)]) % GPRIME;
                }
            }

            /* Convert syndromes to index form, checking for nonzero condition */
            synError = 0;
            for (i = 1; i <= syndLen; i++)
            {
                synError |= s[i];
                s[i] = IndexOf[s[i]];
            }

            if (synError == 0)
            {
                /* if syndrome is zero, data[] is a codeword and there are no
                 * errors to correct. So return data[] unmodified
                 */
                count = 0;
                goto finish;
            }

            for (ci = syndLen - 1; ci >= 0; ci--) lambda[ci + 1] = 0;

            lambda[0] = 1;

            if (no_eras > 0)
            {
                /* Init lambda to be the erasure locator polynomial */
                lambda[1] = AlphaTo[Modbase(PRIM * eras_pos[0])];
                for (i = 1; i < no_eras; i++)
                {
                    u = Modbase(PRIM * eras_pos[i]);
                    for (j = i + 1; j > 0; j--)
                    {
                        tmp = IndexOf[lambda[j - 1]];
                        if (tmp != A0)
                            lambda[j] = (lambda[j] + AlphaTo[Modbase(u + tmp)]) % GPRIME;
                    }
                }
            }
            for (i = 0; i < syndLen + 1; i++)
                b[i] = IndexOf[lambda[i]];

            /*
             * Begin Berlekamp-Massey algorithm to determine error+erasure
             * locator polynomial
             */
            r = no_eras;
            el = no_eras;
            while (++r <= syndLen)
            {	/* r is the step number */
                /* Compute discrepancy at the r-th step in poly-form */
                discrR = 0;
                for (i = 0; i < r; i++)
                {
                    if ((lambda[i] != 0) && (s[r - i] != A0))
                    {
                        if (i % 2 == 1)
                        {
                            discrR = (discrR + AlphaTo[Modbase((IndexOf[lambda[i]] + s[r - i]))]) % GPRIME;
                        }
                        else
                        {
                            discrR = (discrR + GPRIME - AlphaTo[Modbase((IndexOf[lambda[i]] + s[r - i]))]) % GPRIME;
                        }
                    }
                }
                discrR = IndexOf[discrR];	/* Index form */

                if (discrR == A0)
                {
                    /* 2 lines below: B(x) <-- x*B(x) */
                    //  COPYDOWN(&b[1],b,synd_len);
                    for (ci = syndLen - 1; ci >= 0; ci--) b[ci + 1] = b[ci];
                    b[0] = A0;
                }
                else
                {
                    /* 7 lines below: T(x) <-- lambda(x) - discr_r*x*b(x) */
                    /*  the T(x) will become the next lambda */

                    t[0] = lambda[0];
                    for (i = 0; i < syndLen; i++)
                    {
                        if (b[i] != A0)
                        {

                            //  t[i+1] =  (lambda[i+1] + GPRIME -
                            //              Alpha_to[modbase(discr_r + GPRIME - 1 -  b[i])]) % GPRIME;
                            t[i + 1] = (lambda[i + 1] + AlphaTo[Modbase(discrR + b[i])]) % GPRIME;
                        }
                        else
                        {
                            t[i + 1] = lambda[i + 1];
                        }
                    }
                    el = 0;
                    if (2 * el <= r + no_eras - 1)
                    {
                        el = r + no_eras - el;
                        /*
                         * 2 lines below: B(x) <-- inv(discr_r) *
                         * lambda(x)
                         */
                        for (i = 0; i <= syndLen; i++)
                        {

                            if (lambda[i] == 0)
                            {
                                b[i] = A0;
                            }
                            else
                            {
                                b[i] = Modbase(IndexOf[lambda[i]] - discrR + GPRIME - 1);
                            }
                        }

                    }
                    else
                    {
                        /* 2 lines below: B(x) <-- x*B(x) */
                        //      COPYDOWN(&b[1],b,synd_len);
                        for (ci = syndLen - 1; ci >= 0; ci--) b[ci + 1] = b[ci];
                        b[0] = A0;
                    }
                    //      COPY(lambda,t,synd_len+1);

                    for (ci = syndLen + 1 - 1; ci >= 0; ci--)
                    {
                        lambda[ci] = t[ci];
                    }
                }
            }

            /* Convert lambda to index form and compute deg(lambda(x)) */
            degLambda = 0;
            for (i = 0; i < syndLen + 1; i++)
            {
                lambda[i] = IndexOf[lambda[i]];
                if (lambda[i] != A0) degLambda = i;
            }

            /*
             * Find roots of the error+erasure locator polynomial by Chien
             * Search
             */
            for (ci = syndLen - 1; ci >= 0; ci--)
                reg[ci + 1] = lambda[ci + 1];

            count = 0;			/* Number of roots of lambda(x) */
            for (i = 1, k = data_len - 1; i <= GPRIME; i++)
            {
                q = 1;
                for (j = degLambda; j > 0; j--)
                {

                    if (reg[j] != A0)
                    {
                        reg[j] = Modbase(reg[j] + j);
                        //      q = modbase( q +  Alpha_to[reg[j]]);
                        if (degLambda != 1)
                        {
                            if (j % 2 == 0)
                            {
                                q = (q + AlphaTo[reg[j]]) % GPRIME;
                            }
                            else
                            {
                                q = (q + GPRIME - AlphaTo[reg[j]]) % GPRIME;
                            }
                        }
                        else
                        {
                            q = AlphaTo[reg[j]] % GPRIME;
                            if (q == 1) --q;
                        }
                    }
                }

                if (q == 0)
                {
                    /* store root (index-form) and error location number */
                    root[count] = i;

                    loc[count] = GPRIME - 1 - i;
                    if (count < syndLen)
                    {
                        count += 1;
                    }
                }
                if (k == 0)
                {
                    k = data_len - 1;
                }
                else
                {
                    k -= 1;
                }

                /* If we've already found max possible roots,
                 * abort the search to save time
                 */

                if (count == degLambda) break;

            }


            if (degLambda != count)
            {
                // deg(lambda) unequal to number of roots => uncorrectable
                // error detected
                count = -1;
                goto finish;
            }


            /*
             * Compute err+eras evaluator poly omega(x) = s(x)*lambda(x) (modulo
             * x**(synd_len)). in index form. Also find deg(omega).
             */
            degOmega = 0;
            for (i = 0; i < syndLen; i++)
            {
                tmp = 0;
                j = (degLambda < i) ? degLambda : i;
                for (; j >= 0; j--)
                {
                    if ((s[i + 1 - j] != A0) && (lambda[j] != A0))
                    {
                        if (j % 2 == 1)
                        {
                            tmp = (tmp + GPRIME - AlphaTo[Modbase(s[i + 1 - j] + lambda[j])]) % GPRIME;
                        }
                        else
                        {

                            tmp = (tmp + AlphaTo[Modbase(s[i + 1 - j] + lambda[j])]) % GPRIME;
                        }
                    }
                }

                if (tmp != 0) degOmega = i;
                omega[i] = IndexOf[tmp];
            }
            omega[syndLen] = A0;

            /*
             * Compute error values in poly-form. num1 = omega(inv(X(l))), num2 =
             * inv(X(l))**(B0-1) and den = lambda_pr(inv(X(l))) all in poly-form
             */
            for (j = count - 1; j >= 0; j--)
            {
                num1 = 0;
                for (i = degOmega; i >= 0; i--)
                {
                    if (omega[i] != A0)
                    {
                        //    num1  = ( num1 + Alpha_to[modbase(omega[i] + (i * root[j])]) % GPRIME;
                        num1 = (num1 + AlphaTo[Modbase(omega[i] + ((i + 1) * root[j]))]) % GPRIME;
                    }
                }
                //  num2 = Alpha_to[modbase(root[j] * (1 - 1) + data_len)];

                num2 = 1;
                den = 0;

                // denominator if product of all (1 - Bj Bk) for k != j
                // if count = 1, then den = 1

                den = 1;
                for (k = 0; k < count; k += 1)
                {
                    if (k != j)
                    {
                        tmp = (1 + GPRIME - AlphaTo[Modbase(GPRIME - 1 - root[k] + root[j])]) % GPRIME;
                        den = AlphaTo[Modbase(IndexOf[den] + IndexOf[tmp])];
                    }
                }


                if (den == 0)
                {

                    /* Convert to dual- basis */
                    count = -1;
                    goto finish;
                }

                errorVal = AlphaTo[Modbase(IndexOf[num1] + IndexOf[num2] +
                                             GPRIME - 1 - IndexOf[den])] % GPRIME;
                /* Apply error to data */
                if (num1 != 0)
                {
                    if (loc[j] < data_len + 1)
                    {
                        fixLoc = data_len - loc[j];

                        if (fixLoc < data_len + 1)
                        {
                            data[fixLoc] = (data[fixLoc] + GPRIME - errorVal) % GPRIME;
                        }
                    }
                }
            }
        finish:
            if (eras_pos != null)
            {
                for (i = 0; i < count; i++)
                {
                    if (eras_pos != null) eras_pos[i] = loc[i];
                }
            }
            return count;
        }
    }
}

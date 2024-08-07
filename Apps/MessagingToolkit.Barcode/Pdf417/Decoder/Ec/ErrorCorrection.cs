
using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Pdf417.Decoder.Ec
{
    /// <summary>
    /// <p>PDF417 error correction implementation.</p>
    /// 
    /// <p>This <a href="http://en.wikipedia.org/wiki/Reed%E2%80%93Solomon_error_correction#Example">example</a>
    /// is quite useful in understanding the algorithm.</p>
    /// 
    /// </summary>
    public sealed class ErrorCorrection
    {
        private readonly ModulusGF field;

        public ErrorCorrection()
        {
            this.field = ModulusGF.PDF417_GF;
        }

        /// <summary>
        /// Return number of errors.
        /// </summary>
        /// <param name="received"></param>
        /// <param name="numECCodewords"></param>
        /// <param name="erasures"></param>
        /// <returns></returns>
        public int Decode(int[] received,
                           int numECCodewords,
                           int[] erasures)
        {

            ModulusPoly poly = new ModulusPoly(field, received);
            int[] S = new int[numECCodewords];
            bool error = false;
            for (int i = numECCodewords; i > 0; i--)
            {
                int eval = poly.EvaluateAt(field.Exp(i));
                S[numECCodewords - i] = eval;
                if (eval != 0)
                {
                    error = true;
                }
            }

            if (!error)
            {
                return 0;
            }

            ModulusPoly knownErrors = field.GetOne();
            foreach (int erasure in erasures)
            {
                int b = field.Exp(received.Length - 1 - erasure);
                // Add (1 - bx) term:
                ModulusPoly term = new ModulusPoly(field, new int[] { field.Subtract(0, b), 1 });
                knownErrors = knownErrors.Multiply(term);
            }

            ModulusPoly syndrome = new ModulusPoly(field, S);
            //syndrome = syndrome.multiply(knownErrors);

            ModulusPoly[] sigmaOmega = RunEuclideanAlgorithm(field.BuildMonomial(numECCodewords, 1), syndrome, numECCodewords);
            ModulusPoly sigma = sigmaOmega[0];
            ModulusPoly omega = sigmaOmega[1];

            //sigma = sigma.multiply(knownErrors);

            int[] errorLocations = FindErrorLocations(sigma);
            int[] errorMagnitudes = FindErrorMagnitudes(omega, sigma, errorLocations);

            for (int i = 0; i < errorLocations.Length; i++)
            {
                int position = received.Length - 1 - field.Log(errorLocations[i]);
                if (position < 0)
                {
                    throw ChecksumException.Instance;
                }
                received[position] = field.Subtract(received[position], errorMagnitudes[i]);
            }
            return errorLocations.Length;
        }


        private ModulusPoly[] RunEuclideanAlgorithm(ModulusPoly a, ModulusPoly b, int R)
        {
            // Assume a's degree is >= b's
            if (a.Degree < b.Degree)
            {
                ModulusPoly temp = a;
                a = b;
                b = temp;
            }

            ModulusPoly rLast = a;
            ModulusPoly r = b;
            //ModulusPoly sLast = field.GetOne();
            //ModulusPoly s = field.GetZero();
            ModulusPoly tLast = field.GetZero();
            ModulusPoly t = field.GetOne();

            // Run Euclidean algorithm until r's degree is less than R/2
            while (r.Degree >= R / 2)
            {
                ModulusPoly rLastLast = rLast;
                //ModulusPoly sLastLast = sLast;
                ModulusPoly tLastLast = tLast;
                rLast = r;
                //sLast = s;
                tLast = t;

                // Divide rLastLast by rLast, with quotient in q and remainder in r
                if (rLast.IsZero)
                {
                    // Oops, Euclidean algorithm already terminated?
                    throw ChecksumException.Instance;
                }
                r = rLastLast;
                ModulusPoly q = field.GetZero();
                int denominatorLeadingTerm = rLast.GetCoefficient(rLast.Degree);
                int dltInverse = field.Inverse(denominatorLeadingTerm);
                while (r.Degree >= rLast.Degree && !r.IsZero)
                {
                    int degreeDiff = r.Degree - rLast.Degree;
                    int scale = field.Multiply(r.GetCoefficient(r.Degree), dltInverse);
                    q = q.Add(field.BuildMonomial(degreeDiff, scale));
                    r = r.Subtract(rLast.MultiplyByMonomial(degreeDiff, scale));
                }

                //s = q.multiply(sLast).subtract(sLastLast).negative();
                t = q.Multiply(tLast).Subtract(tLastLast).Negative();
            }

            int sigmaTildeAtZero = t.GetCoefficient(0);
            if (sigmaTildeAtZero == 0)
            {
                throw ChecksumException.Instance;
            }

            int inverse = field.Inverse(sigmaTildeAtZero);
            ModulusPoly sigma = t.Multiply(inverse);
            ModulusPoly omega = r.Multiply(inverse);
            return new ModulusPoly[] { sigma, omega };
        }

        private int[] FindErrorLocations(ModulusPoly errorLocator)
        {
            // This is a direct application of Chien's search
            int numErrors = errorLocator.Degree;
            int[] result = new int[numErrors];
            int e = 0;
            for (int i = 1; i < field.Size && e < numErrors; i++)
            {
                if (errorLocator.EvaluateAt(i) == 0)
                {
                    result[e] = field.Inverse(i);
                    e++;
                }
            }
            if (e != numErrors)
            {
                throw ChecksumException.Instance;
            }
            return result;
        }

        private int[] FindErrorMagnitudes(ModulusPoly errorEvaluator,
                                          ModulusPoly errorLocator,
                                          int[] errorLocations)
        {
            int errorLocatorDegree = errorLocator.Degree;
            int[] formalDerivativeCoefficients = new int[errorLocatorDegree];
            for (int i = 1; i <= errorLocatorDegree; i++)
            {
                formalDerivativeCoefficients[errorLocatorDegree - i] =
                    field.Multiply(i, errorLocator.GetCoefficient(i));
            }
            ModulusPoly formalDerivative = new ModulusPoly(field, formalDerivativeCoefficients);

            // This is directly applying Forney's Formula
            int s = errorLocations.Length;
            int[] result = new int[s];
            for (int i = 0; i < s; i++)
            {
                int xiInverse = field.Inverse(errorLocations[i]);
                int numerator = field.Subtract(0, errorEvaluator.EvaluateAt(xiInverse));
                int denominator = field.Inverse(formalDerivative.EvaluateAt(xiInverse));
                result[i] = field.Multiply(numerator, denominator);
            }
            return result;
        }
    }
}
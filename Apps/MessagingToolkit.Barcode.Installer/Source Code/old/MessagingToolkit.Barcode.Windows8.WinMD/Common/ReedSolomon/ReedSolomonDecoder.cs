using System;

namespace MessagingToolkit.Barcode.Common.ReedSolomon
{

    /// <summary>
    /// Implements Reed-Solomon decoding, as the name implies.
    /// The algorithm will not be explained here, but the following references were helpful
    /// in creating this implementation:
    /// <ul>
    /// 		<li>Bruce Maggs.
    /// <a href="http://www.cs.cmu.edu/afs/cs.cmu.edu/project/pscico-guyb/realworld/www/rs_decode.ps">
    /// "Decoding Reed-Solomon Codes"</a> (see discussion of Forney's Formula)</li>
    /// 		<li>J.I. Hall. <a href="www.mth.msu.edu/~jhall/classes/codenotes/GRS.pdf">
    /// "Chapter 5. Generalized Reed-Solomon Codes"</a>
    /// (see discussion of Euclidean algorithm)</li>
    /// 	</ul>
    /// 	
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class ReedSolomonDecoder
	{
		private GenericGF field;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReedSolomonDecoder"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
		public ReedSolomonDecoder(GenericGF field)
		{
			this.field = field;
		}

        /// <summary>
        /// Decodes given set of received codewords, which include both data and error-correction
        /// codewords. Really, this means it uses Reed-Solomon to detect and correct errors, in-place,
        /// in the input.
        /// </summary>
        /// <param name="received">data and error-correction codewords</param>
        /// <param name="twoS">number of error-correction codewords available</param>
        /// <throws>  ReedSolomonException if decoding fails for any reason </throws>
		public void Decode(int[] received, int twoS)
		{
			GenericGFPoly poly = new GenericGFPoly(field, received);
			int[] syndromeCoefficients = new int[twoS];
			bool dataMatrix = field.Equals(GenericGF.DataMatrixField256);
			bool noError = true;
			for (int i = 0; i < twoS; i++)
			{
				// Thanks to sanfordsquires for this fix:
				int eval = poly.EvaluateAt(field.Exp(dataMatrix?i + 1:i));
				syndromeCoefficients[syndromeCoefficients.Length - 1 - i] = eval;
				if (eval != 0)
				{
					noError = false;
				}
			}
			if (noError)
			{
				return ;
			}
			GenericGFPoly syndrome = new GenericGFPoly(field, syndromeCoefficients);
			GenericGFPoly[] sigmaOmega = RunEuclideanAlgorithm(field.BuildMonomial(twoS, 1), syndrome, twoS);
			GenericGFPoly sigma = sigmaOmega[0];
			GenericGFPoly omega = sigmaOmega[1];
			int[] errorLocations = FindErrorLocations(sigma);
			int[] errorMagnitudes = FindErrorMagnitudes(omega, errorLocations, dataMatrix);
			for (int i = 0; i < errorLocations.Length; i++)
			{
				int position = received.Length - 1 - field.Log(errorLocations[i]);
				if (position < 0)
				{
					throw new ReedSolomonException("Bad error location");
				}
				received[position] = GenericGF.AddOrSubtract(received[position], errorMagnitudes[i]);
			}
		}
		
		private GenericGFPoly[] RunEuclideanAlgorithm(GenericGFPoly a, GenericGFPoly b, int R)
		{
			// Assume a's degree is >= b's
			if (a.GetDegree() < b.GetDegree())
			{
				GenericGFPoly temp = a;
				a = b;
				b = temp;
			}
			
			GenericGFPoly rLast = a;
			GenericGFPoly r = b;
			//GenericGFPoly sLast = field.GetOne();
			//GenericGFPoly s = field.GetZero();
			GenericGFPoly tLast = field.GetZero();
			GenericGFPoly t = field.GetOne();
			
			// Run Euclidean algorithm until r's degree is less than R/2
			while (r.GetDegree() >= R / 2)
			{
				GenericGFPoly rLastLast = rLast;
				//GenericGFPoly sLastLast = sLast;
				GenericGFPoly tLastLast = tLast;
				rLast = r;
				//sLast = s;
				tLast = t;
				
				// Divide rLastLast by rLast, with quotient in q and remainder in r
				if (rLast.IsZero())
				{
					// Oops, Euclidean algorithm already terminated?
					throw new ReedSolomonException("r_{idx-1} was zero");
				}
				r = rLastLast;
				GenericGFPoly q = field.GetZero();
				int denominatorLeadingTerm = rLast.GetCoefficient(rLast.GetDegree());
				int dltInverse = field.Inverse(denominatorLeadingTerm);
				while (r.GetDegree() >= rLast.GetDegree() && !r.IsZero())
				{
					int degreeDiff = r.GetDegree() - rLast.GetDegree();
					int scale = field.Multiply(r.GetCoefficient(r.GetDegree()), dltInverse);
					q = q.AddOrSubtract(field.BuildMonomial(degreeDiff, scale));
					r = r.AddOrSubtract(rLast.MultiplyByMonomial(degreeDiff, scale));
				}
				
				//s = q.Multiply(sLast).AddOrSubtract(sLastLast);
				t = q.Multiply(tLast).AddOrSubtract(tLastLast);
			}
			
			int sigmaTildeAtZero = t.GetCoefficient(0);
			if (sigmaTildeAtZero == 0)
			{
				throw new ReedSolomonException("sigmaTilde(0) was zero");
			}
			
			int inverse = field.Inverse(sigmaTildeAtZero);
			GenericGFPoly sigma = t.Multiply(inverse);
			GenericGFPoly omega = r.Multiply(inverse);
			return new GenericGFPoly[]{sigma, omega};
		}
		
		private int[] FindErrorLocations(GenericGFPoly errorLocator)
		{
			// This is a direct application of Chien's search
			int numErrors = errorLocator.GetDegree();
			if (numErrors == 1)
			{
				// shortcut
				return new int[]{errorLocator.GetCoefficient(1)};
			}
			int[] result = new int[numErrors];
			int e = 0;
			for (int i = 1; i < field.GetSize() && e < numErrors; i++)
			{
				if (errorLocator.EvaluateAt(i) == 0)
				{
					result[e] = field.Inverse(i);
					e++;
				}
			}
			if (e != numErrors)
			{
				throw new ReedSolomonException("Error locator degree does not match number of roots");
			}
			return result;
		}
		
		private int[] FindErrorMagnitudes(GenericGFPoly errorEvaluator, int[] errorLocations, bool dataMatrix)
		{
			// This is directly applying Forney's Formula
			int s = errorLocations.Length;
			int[] result = new int[s];
			for (int i = 0; i < s; i++)
			{
				int xiInverse = field.Inverse(errorLocations[i]);
				int denominator = 1;
				for (int j = 0; j < s; j++)
				{
					if (i != j)
					{
						//denominator = field.Multiply(denominator, GenericGF.AddOrSubtract(1, field.Multiply(errorLocations[j], xiInverse)));
                        int term = field.Multiply(errorLocations[j], xiInverse);
                        int termPlus1 = (term & 0x1) == 0 ? term | 1 : term & ~1;
                        denominator = field.Multiply(denominator, termPlus1);
					}
				}
				result[i] = field.Multiply(errorEvaluator.EvaluateAt(xiInverse), field.Inverse(denominator));
				// Thanks to sanfordsquires for this fix:
				if (dataMatrix)
				{
					result[i] = field.Multiply(result[i], xiInverse);
				}
			}
			return result;
		}
	}
}
using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.Common.ReedSolomon
{

    /// <summary>
    /// Implements Reed-Solomon encoding, as the name implies.
    /// </summary>
    internal sealed class ReedSolomonEncoder
	{
        private GenericGF field;
		private List<GenericGFPoly> cachedGenerators;
		
		public ReedSolomonEncoder(GenericGF field)
		{
			if (!GenericGF.QRCodeField256.Equals(field))
			{
				throw new ArgumentException("Only QR Code is supported at this time");
			}
			this.field = field;
			this.cachedGenerators = new List<GenericGFPoly>(10);
			cachedGenerators.Add(new GenericGFPoly(field, new int[]{1}));
		}
		
		private GenericGFPoly BuildGenerator(int degree)
		{
			if (degree >= cachedGenerators.Count)
			{
				GenericGFPoly lastGenerator = (GenericGFPoly) cachedGenerators[cachedGenerators.Count - 1];
				for (int d = cachedGenerators.Count; d <= degree; d++)
				{
					GenericGFPoly nextGenerator = lastGenerator.Multiply(new GenericGFPoly(field, new int[]{1, field.Exp(d - 1)}));
					cachedGenerators.Add(nextGenerator);
					lastGenerator = nextGenerator;
				}
			}
			return (GenericGFPoly) cachedGenerators[degree];
		}
		
		public void  Encode(int[] toEncode, int ecBytes)
		{
			if (ecBytes == 0)
			{
				throw new System.ArgumentException("No error correction bytes");
			}
			int dataBytes = toEncode.Length - ecBytes;
			if (dataBytes <= 0)
			{
				throw new System.ArgumentException("No data bytes provided");
			}
			GenericGFPoly generator = BuildGenerator(ecBytes);
			int[] infoCoefficients = new int[dataBytes];
			Array.Copy(toEncode, 0, infoCoefficients, 0, dataBytes);
			GenericGFPoly info = new GenericGFPoly(field, infoCoefficients);
			info = info.MultiplyByMonomial(ecBytes, 1);
			GenericGFPoly remainder = info.Divide(generator)[1];
			int[] coefficients = remainder.GetCoefficients();
			int numZeroCoefficients = ecBytes - coefficients.Length;
			for (int i = 0; i < numZeroCoefficients; i++)
			{
				toEncode[dataBytes + i] = 0;
			}
			Array.Copy(coefficients, 0, toEncode, dataBytes + numZeroCoefficients, coefficients.Length);
		}
	}
}
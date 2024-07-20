using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Aztec
{

	public sealed class AztecDetectorResult : DetectorResult {
	
		private readonly bool compact;
		private readonly int nbDatablocks;
		private readonly int nbLayers;
	
		public AztecDetectorResult(BitMatrix bits, ResultPoint[] points,
				bool compact, int nbDatablocks, int nbLayers) : base(bits, points) {
			this.compact = compact;
			this.nbDatablocks = nbDatablocks;
			this.nbLayers = nbLayers;
		}
	
		public int GetNbLayers() {
			return nbLayers;
		}
	
		public int GetNbDatablocks() {
			return nbDatablocks;
		}
	
		public bool IsCompact() {
			return compact;
		}
	
	}
}

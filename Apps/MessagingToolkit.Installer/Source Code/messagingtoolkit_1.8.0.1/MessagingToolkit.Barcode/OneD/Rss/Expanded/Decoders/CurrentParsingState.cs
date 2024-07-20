using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    internal sealed class CurrentParsingState
    {

        internal int position;
        private int encoding;

        private const int Numeric = 1;
        private const int Alpha = 2;
        private const int IsoIec646 = 4;

        internal CurrentParsingState()
        {
            this.position = 0;
            this.encoding = Numeric;
        }

        internal bool IsAlpha
        {
            get
            {
                return this.encoding == Alpha;
            }
        }

        internal bool IsNumeric
        {
            get
            {
                return this.encoding == Numeric;
            }
        }

        internal bool IsIsoIec646
        {
            get
            {
                return this.encoding == IsoIec646;
            }
        }

        internal void SetNumeric()
        {
            this.encoding = Numeric;
        }

        internal void SetAlpha()
        {
            this.encoding = Alpha;
        }

        internal void SetIsoIec646()
        {
            this.encoding = IsoIec646;
        }
    }
}


using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    internal sealed class BlockParsedResult
    {

        private readonly DecodedInformation decodedInformation;
        private readonly bool finished;

        internal BlockParsedResult()
        {
            this.finished = true;
            this.decodedInformation = null;
        }

        internal BlockParsedResult(bool finished)
        {
            this.finished = finished;
            this.decodedInformation = null;
        }

        internal BlockParsedResult(DecodedInformation information, bool finished)
        {
            this.finished = finished;
            this.decodedInformation = information;
        }

        internal DecodedInformation DecodedInformation
        {
            get
            {
                return this.decodedInformation;
            }
        }

        internal bool IsFinished
        {
            get
            {
                return this.finished;
            }
        }
    }
}

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    abstract internal class DecodedObject
    {

        protected internal readonly int newPosition;

        internal DecodedObject(int newPosition)
        {
            this.newPosition = newPosition;
        }

        internal int NewPosition
        {
            get
            {
                return this.newPosition;
            }
        }

    }
}

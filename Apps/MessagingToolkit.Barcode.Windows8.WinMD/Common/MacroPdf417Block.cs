using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Common
{
    public sealed class MacroPdf417Block
    {
        public int SegmentIndex { get; set; }

        public String FileId { get; set; }

        public int[] OptionalData { get; set; }

    }
}

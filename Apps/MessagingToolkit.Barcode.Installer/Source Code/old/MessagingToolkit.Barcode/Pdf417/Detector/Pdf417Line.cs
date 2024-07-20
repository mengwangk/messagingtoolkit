using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    public sealed class Pdf417Line
    {
        private Pdf417Symbol leftColumnIndicator;
        private Pdf417Symbol rightColumnIndicator;
        private List<Pdf417Symbol> symbols;

        public Pdf417Line()
        {
            this.leftColumnIndicator = null;
            this.rightColumnIndicator = null;
        }

        public Pdf417Symbol LeftColumnIndicator
        {
            get
            {
                return this.leftColumnIndicator;
            }
            set
            {
                this.leftColumnIndicator = value;
            }
        }

        public Pdf417Symbol RightColumnIndicator
        {
            get
            {
                return this.rightColumnIndicator;
            }
            set
            {
                this.rightColumnIndicator = value;
            }
        }
    }
}

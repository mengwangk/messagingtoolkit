using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    /// <summary>
    /// Enumeration for DataMatrix symbol shape hint. It can be used to force square or rectangular
    /// symbols.
    /// </summary>
    public enum SymbolShapeHint
    {
        ForceNone,
        ForceSquare,
        ForceRectangle,

    }
}

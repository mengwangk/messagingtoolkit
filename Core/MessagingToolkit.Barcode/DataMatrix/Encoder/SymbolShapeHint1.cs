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
    public class SymbolShapeHint
    {
        // The human-readable part is suppressed.
        public static readonly SymbolShapeHint ForceNone = new SymbolShapeHint("force-none");

        // The human-readable part is placed at the top of the barcode.
        public static readonly SymbolShapeHint ForceSquare = new SymbolShapeHint("force-square");

        // The human-readable part is placed at the bottom of the barcode. 
        public static readonly SymbolShapeHint ForceRectangle = new SymbolShapeHint("force-rectangle");

        private String name;

      
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolShapeHint" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected SymbolShapeHint(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Return the name of the instance.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return this.name;
        }

        /// <summary>
        /// Returns a SymbolShapeHint instance by name.
        /// </summary>
        /// <param name="name">the name of the instance</param>
        /// <returns>The requested instance</returns>
        /// <exception cref="System.ArgumentException">Invalid SymbolShapeHint:  + name</exception>
        public static SymbolShapeHint ByName(string name)
        {
            if (name.Equals(SymbolShapeHint.ForceNone.GetName()))
            {
                return SymbolShapeHint.ForceNone;
            }
            else if (name.Equals(SymbolShapeHint.ForceSquare.GetName()))
            {
                return SymbolShapeHint.ForceSquare;
            }
            else if (name.Equals(SymbolShapeHint.ForceRectangle.GetName()))
            {
                return SymbolShapeHint.ForceRectangle;
            }
            else
            {
                throw new ArgumentException(
                    "Invalid SymbolShapeHint: " + name);
            }
        }

        public override String ToString()
        {
            return GetName();
        }
    }

}


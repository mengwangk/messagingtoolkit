using System;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Aztec.Encoder
{

    /// <summary>
    /// Aztec 2D code representation
    /// </summary>
    public sealed class AztecCode
    {
        private bool compact;
        private int size;
        private int layers;
        private int codeWords;
        private BitMatrix matrix;

        /// <summary>
        /// Compact or full symbol indicator
        /// </summary>
        public bool Compact
        {
            get
            {
                return compact;
            }
            set
            {
                this.compact = value;
            }
        }


        /// <summary>
        /// Size in pixels (width and height)
        /// </summary>
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                this.size = value;
            }
        }


        /// <summary>
        /// Number of levels
        /// </summary>
        public int Layers
        {
            get
            {
                return layers;
            }
            set
            {
                this.layers = value;
            }
        }


        /// <summary>
        /// Number of data codewords
        /// </summary>
        public int CodeWords
        {
            get
            {
                return codeWords;
            }
            set
            {
                this.codeWords = value;
            }
        }


        /// <summary>
        /// The symbol image
        /// </summary>
        public BitMatrix Matrix
        {
            get
            {
                return matrix;
            }
            set
            {
                this.matrix = value;
            }
        }
    }

}
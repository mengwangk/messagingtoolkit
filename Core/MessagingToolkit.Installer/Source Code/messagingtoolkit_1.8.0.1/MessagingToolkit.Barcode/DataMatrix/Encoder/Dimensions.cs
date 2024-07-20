using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    public sealed class Dimensions
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Dimensions" /> class.
        /// </summary>
        public Dimensions(): this(100,100)
        {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Dimensions" /> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Dimensions(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; }
    }
}

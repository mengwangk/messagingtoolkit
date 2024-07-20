using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    public class DataMatrix
    {
        /// <summary>
        /// Net height of bars in mm </summary>
        protected internal double height = 15.0; // mm

        /// <summary>
        /// Width of narrow module in mm </summary>
        protected internal double moduleWidth;

        /// <summary>
        /// True if quiet zone should be rendered </summary>
        protected internal bool doQuietZone = true;
        /// <summary>
        /// Width of the quiet zone left and right of the barcode in mm </summary>
        protected internal double quietZone;
        /// <summary>
        /// Height of the vertical quiet zone above and below the barcode in mm </summary>
        protected internal double? quietZoneVertical;
        /// <summary>
        /// pattern to be applied over the human readable message </summary>
        protected internal string pattern;

        /// <summary>
        /// The default module width (dot size) for DataMatrix. </summary>
        protected internal static readonly double DEFAULT_MODULE_WIDTH = (1.0 / 72) * 25.4; //1px at 72dpi

        /// <summary>
        /// The requested shape. May be <code>FORCE_NONE</code>,
        /// <code>FORCE_SQUARE</code> or <code>FORCE_RECTANGLE</code>.
        /// </summary>
        private SymbolShapeHint shape;

        /// <summary>
        /// Optional: the minimum size of the symbol. </summary>
        private Dimensions minSize;
        /// <summary>
        /// Optional: the maximum size of the symbol. </summary>
        private Dimensions maxSize;

        /// <summary>
        /// Create a new instance. </summary>
        public DataMatrix()
        {
            this.height = 0.0; //not used by DataMatrix
            this.moduleWidth = DEFAULT_MODULE_WIDTH;
            this.quietZone = 1 * moduleWidth;
            this.shape = SymbolShapeHint.ForceNone;
        }


        /// <summary>
        /// returns the pattern to be applied over the human readable message </summary>
        /// <returns> String </returns>
        public virtual string Pattern
        {
            get
            {
                return this.pattern;
            }
            set
            {
                this.pattern = value;
            }
        }


        /// <summary>
        /// Returns the height of the bars. </summary>
        /// <returns> the height of the bars (in mm) </returns>
        public virtual double BarHeight
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }


        /// <summary>
        /// Returns the width of the narrow module. </summary>
        /// <returns> the width of the narrow module (in mm) </returns>
        public virtual double ModuleWidth
        {
            get
            {
                return this.moduleWidth;
            }
            set
            {
                this.moduleWidth = value;
            }
        }

        /// <summary>
        /// Indicates whether a quiet zone is included. </summary>
        /// <returns> true if a quiet zone is included </returns>
        public virtual bool HasQuietZone()
        {
            return this.doQuietZone;
        }

        /// <summary>
        /// Controls whether a quiet zone should be included or not. </summary>
        /// <param name="value"> true if a quiet zone should be included </param>
        public virtual void DoQuietZone(bool value)
        {
            this.doQuietZone = value;
        }

        /// <returns> the width of the quiet zone (in mm) </returns>
        public virtual double QuietZone
        {
            get
            {
                return this.quietZone;
            }
            set
            {
                this.quietZone = value;
            }
        }

        /// <summary>
        /// Returns the vertical quiet zone. If no vertical quiet zone is set explicitely, the value
        /// if <seealso cref="#getQuietZone()"/> is returned. </summary>
        /// <returns> the height of the vertical quiet zone (in mm) </returns>
        public virtual double VerticalQuietZone
        {
            get
            {
                if (this.quietZoneVertical != null)
                {
                    return (double)this.quietZoneVertical;
                }
                else
                {
                    return QuietZone;
                }
            }
            set
            {
                this.quietZoneVertical = new double?(value);
            }
        }


        /// <summary>
        /// Sets the requested shape for the generated barcodes. </summary>
        /// <param name="shape"> requested shape. May be <code>SymbolShapeHint.FORCE_NONE</code>,
        /// <code>SymbolShapeHint.FORCE_SQUARE</code> or <code>SymbolShapeHint.FORCE_RECTANGLE</code>. </param>
        public virtual SymbolShapeHint Shape
        {
            set
            {
                this.shape = value;
            }
            get
            {
                return shape;
            }
        }


        /// <summary>
        /// Sets the minimum symbol size that is to be produced. </summary>
        /// <param name="minSize"> the minimum size (in pixels), or null for no constraint </param>
        public virtual Dimensions MinSize
        {
            set
            {
                this.minSize = (value != null ? new Dimensions(value) : null);
            }
            get
            {
                if (this.minSize != null)
                {
                    return new Dimension(this.minSize);
                }
                else
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// Sets the maximum symbol size that is to be produced.
        /// </summary>
        /// <value>
        /// The maximum size (in pixels), or null for no constraint 
        /// </value>
        public virtual Dimensions MaxSize
        {
            set
            {
                this.maxSize = (value != null ? new Dimensions(value) : null);
            }
            get
            {
                if (this.maxSize != null)
                {
                    return new Dimensions(this.maxSize);
                }
                else
                {
                    return null;
                }
            }
        }


        public override void GenerateBarcode(CanvasProvider canvas, string msg)
        {
            if ((msg == null) || (msg.Length == 0))
            {
                throw new ArgumentNullException("Parameter msg must not be empty");
            }

            TwoDimBarcodeLogicHandler handler = new DefaultTwoDimCanvasLogicHandler(this, new Canvas(canvas));

            DataMatrixLogicImpl impl = new DataMatrixLogicImpl();
            impl.generateBarcodeLogic(handler, msg, Shape, MinSize, MaxSize);
        }


        public override BarcodeDimension CalcDimensions(string msg)
        {
            string encoded;
            try
            {
                encoded = DataMatrixHighLevelEncoder.encodeHighLevel(msg, shape, MinSize, MaxSize);
            }
            catch (IOException e)
            {
                throw new System.ArgumentException("Cannot fetch data: " + e.LocalizedMessage);
            }
            DataMatrixSymbolInfo symbolInfo = DataMatrixSymbolInfo.lookup(encoded.Length, shape);

            double width = symbolInfo.SymbolWidth * ModuleWidth;
            double height = symbolInfo.SymbolHeight * BarHeight;
            double qzh = (hasQuietZone() ? QuietZone : 0);
            double qzv = (hasQuietZone() ? VerticalQuietZone : 0);
            return new BarcodeDimension(width, height, width + (2 * qzh), height + (2 * qzv), qzh, qzv);
        }

        public override double VerticalQuietZone
        {
            get
            {
                return QuietZone;
            }
        }

        public override double BarWidth
        {
            get
            {
                return moduleWidth;
            }
        }

        public override double BarHeight
        {
            get
            {
                return moduleWidth;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{

    /// <summary>
    /// Data Matrix drawing canvas logic handler.
    /// </summary>
    public class TwoDimCanvasLogicHandler
    {
        private AbstractBarcodeBean bcBean;
        private Canvas canvas;
        private double x = 0.0;
        private double y = 0.0;

        /// <summary>
        /// Main constructor. </summary>
        /// <param name="bcBean"> the barcode implementation class </param>
        /// <param name="canvas"> the canvas to paint to </param>
        public TwoDimCanvasLogicHandler(AbstractBarcodeBean bcBean, Canvas canvas)
        {
            this.bcBean = bcBean;
            this.canvas = canvas;
        }

        private double StartX
        {
            get
            {
                if (bcBean.hasQuietZone())
                {
                    return bcBean.QuietZone;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        private double StartY
        {
            get
            {
                if (bcBean.hasQuietZone())
                {
                    return bcBean.VerticalQuietZone;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public virtual void StartBarcode(string msg, string formattedMsg)
        {
            //Calculate extents
            DataMatrixDimension dim = bcBean.calcDimensions(msg);

            canvas.establishDimensions(dim);
            y = StartY;
        }

        public virtual void StartRow()
        {
            x = StartX;
        }



        public virtual void AddBar(bool black, int width)
        {
            double w = bcBean.getBarWidth(width);
            if (black)
            {
                canvas.drawRectWH(x, y, w, bcBean.BarHeight);
            }
            x += w;
        }
    }
}

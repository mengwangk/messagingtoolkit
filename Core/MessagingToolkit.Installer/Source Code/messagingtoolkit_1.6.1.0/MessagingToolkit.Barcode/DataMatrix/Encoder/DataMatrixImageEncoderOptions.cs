using System;
using System.Collections.Generic;
using System.Text;

#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal sealed class DataMatrixImageEncoderOptions
    {
        #region Constructor

        public DataMatrixImageEncoderOptions()
        {
#if !SILVERLIGHT
            BackColor = Color.White;
            ForeColor = Color.Black;
#else
            BackColor = Color.FromArgb(255, 255, 255, 255);
            ForeColor = Color.FromArgb(0, 0, 0, 0);
#endif
            SizeIdx = DataMatrixSymbolSize.SymbolSquareAuto;
            Scheme = DataMatrixScheme.SchemeAscii;
            ModuleSize = 5;
            MarginSize = 10;
            Width = 250;
            Height = 250;
            QuietZone = 4;
            CharacterSet = "ISO-8859-1";
        }

        #endregion

        #region Properties

        public int MarginSize { get; set; }

        public int ModuleSize { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public DataMatrixScheme Scheme { get; set; }

        public DataMatrixSymbolSize SizeIdx { get; set; }

        public Color ForeColor { get; set; }

        public Color BackColor { get; set; }

        public int QuietZone { get; set; }

        public string CharacterSet { get; set; }

        #endregion
    }
}

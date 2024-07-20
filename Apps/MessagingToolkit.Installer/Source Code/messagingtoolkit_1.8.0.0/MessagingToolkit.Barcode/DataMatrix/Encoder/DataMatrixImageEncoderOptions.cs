using System;
using System.Collections.Generic;
using System.Text;

#if !SILVERLIGHT && !NETFX_CORE

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Drawing.Imaging;

#else

#if WPF

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#else

#if NETFX_CORE

using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


#else

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

#endif

#endif

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal sealed class DataMatrixImageEncoderOptions
    {
        #region Constructor

        public DataMatrixImageEncoderOptions()
        {
#if !SILVERLIGHT && !NETFX_CORE
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

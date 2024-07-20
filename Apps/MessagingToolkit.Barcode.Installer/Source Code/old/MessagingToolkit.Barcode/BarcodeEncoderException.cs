//===============================================================================
using System;

namespace MessagingToolkit.Barcode
{

    /// <summary>
    /// A base class which covers the range of exceptions which may occur when encoding a barcode using
    /// the Writer framework.
    /// </summary>
#if !SILVERLIGHT && !NETFX_CORE
	[Serializable]
#endif

	public sealed class BarcodeEncoderException: Exception
	{
		
		public BarcodeEncoderException():base()
		{
		}
		
		public BarcodeEncoderException(string message):base(message)
		{
		}
	}
}
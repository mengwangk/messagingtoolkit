using System;

namespace MessagingToolkit.QRCode.Codec.Data
{
	public interface QRCodeImage
	{
        int Width
        {
            get;

        }
        int Height
        {
            get;

        }

        int GetPixel(int x, int y);
	}
}
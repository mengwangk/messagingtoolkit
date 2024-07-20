using System.Collections.Generic;

namespace MessagingToolkit.Barcode.Multi
{
    /// <summary>
    /// This class attempts to decode a barcode from an image, not by scanning the whole image,
    /// but by scanning subsets of the image. This is important when there may be multiple barcodes in
    /// an image, and detecting a barcode may find parts of multiple barcode and fail to decode
    /// (e.g. QR Codes). Instead this scans the four quadrants of the image -- and also the center
    /// 'quadrant' to cover the case where a barcode is found in the center.
    /// 
    /// Modified: April 21 2012
    /// </summary>
    public sealed class ByQuadrantDecoder : IDecoder
    {

        private readonly IDecoder decoder;

        public ByQuadrantDecoder(IDecoder delegat0)
        {
            this.decoder = delegat0;
        }

        public Result Decode(BinaryBitmap image)
        {
            return Decode(image, null);
        }

        public Result Decode(BinaryBitmap image, Dictionary<DecodeOptions, object> decodingOptions)
        {

            int width = image.Width;
            int height = image.Height;
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            BinaryBitmap topLeft = image.Crop(0, 0, halfWidth, halfHeight);
            try
            {
                return decoder.Decode(topLeft, decodingOptions);
            }
            catch (NotFoundException)
            {
                // continue
            }

            BinaryBitmap topRight = image.Crop(halfWidth, 0, halfWidth, halfHeight);
            try
            {
                return decoder.Decode(topRight, decodingOptions);
            }
            catch (NotFoundException)
            {
                // continue
            }

            BinaryBitmap bottomLeft = image.Crop(0, halfHeight, halfWidth, halfHeight);
            try
            {
                return decoder.Decode(bottomLeft, decodingOptions);
            }
            catch (NotFoundException)
            {
                // continue
            }

            BinaryBitmap bottomRight = image.Crop(halfWidth, halfHeight, halfWidth, halfHeight);
            try
            {
                return decoder.Decode(bottomRight, decodingOptions);
            }
            catch (NotFoundException)
            {
                // continue
            }

            int quarterWidth = halfWidth / 2;
            int quarterHeight = halfHeight / 2;
            BinaryBitmap center = image.Crop(quarterWidth, quarterHeight, halfWidth, halfHeight);
            return decoder.Decode(center, decodingOptions);
        }

        public void Reset()
        {
            decoder.Reset();
        }
    }
}

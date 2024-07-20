using System;
using System.Collections.Generic;
using System.Text;

#if !SILVERLIGHT
using System.Drawing;
using System.Drawing.Imaging;
#else
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal class DataMatrixImageDecoder
    {
        /// <summary>
        /// returns a list of all decoded DataMatrix codes in the image provided
        /// </summary>
        public List<string> DecodeImage(Bitmap image)
        {
            return DecodeImage(image, int.MaxValue, TimeSpan.MaxValue);
        }

        /// <summary>
        /// returns a list of all decoded DataMatrix codes in the image provided
        /// that can be found in the given time span
        /// </summary>
        public List<string> DecodeImage(Bitmap image, TimeSpan timeSpan)
        {
            return DecodeImage(image, int.MaxValue, timeSpan);
        }

        /// <summary>
        /// returns a list of all decoded DataMatrix codes in the image provided
        /// </summary>
        public List<string> DecodeImageMosaic(Bitmap image)
        {
            return DecodeImageMosaic(image, int.MaxValue, TimeSpan.MaxValue);
        }

        /// <summary>
        /// returns a list of DataMatrix codes in the image provided that can be
        /// found in the given time span, but no more than maxResultCount codes
        /// (useful, if you e.g. expect only one code to be in the image)
        /// </summary>
        public List<string> DecodeImageMosaic(Bitmap image, int maxResultCount, TimeSpan timeOut)
        {
            return DecodeImage(image, maxResultCount, timeOut, true);
        }

        /// <summary>
        /// returns a list of all decoded DataMatrix codes in the image provided
        /// that can be found in the given time span
        /// </summary>
        public List<string> DecodeImageMosaic(Bitmap image, TimeSpan timeSpan)
        {
            return DecodeImage(image, int.MaxValue, timeSpan);
        }

        /// <summary>
        /// returns a list of DataMatrix codes in the image provided that can be
        /// found in the given time span, but no more than maxResultCount codes
        /// (useful, if you e.g. expect only one code to be in the image)
        /// </summary>
        public List<string> DecodeImage(Bitmap image, int maxResultCount, TimeSpan timeOut)
        {
            return DecodeImage(image, maxResultCount, timeOut, false);
        }

        private List<string> DecodeImage(Bitmap image, int maxResultCount, TimeSpan timeOut, bool isMosaic)
        {
            List<string> result = new List<string>();
            int stride;
            byte[] rawImg = ImageToByteArray(image, out stride);
            DataMatrixImage dmtxImg = new DataMatrixImage(rawImg, image.Width, image.Height, DataMatrixPackOrder.Pack24bppRGB);
            dmtxImg.RowPadBytes = stride % 3;
            DataMatrixDecode decode = new DataMatrixDecode(dmtxImg, 1);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                if (stopWatch.Elapsed > timeOut)
                {
                    break;
                }
                DataMatrixRegion region = decode.RegionFindNext(timeOut);
                if (region != null)
                {
                    DataMatrixMessage msg = isMosaic ? decode.MosaicRegion(region, -1) : decode.MatrixRegion(region, -1);
                    string message = Encoding.ASCII.GetString(msg.Output, 0, msg.Output.Length);
                    message = message.Substring(0, message.IndexOf('\0'));
                    if (!result.Contains(message))
                    {
                        result.Add(message);
                        if (result.Count >= maxResultCount)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return result;
        }


        private byte[] ImageToByteArray(Bitmap b, out int stride)
        {
            Rectangle rect = new Rectangle(0, 0, b.Width, b.Height);
            BitmapData bd = b.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                byte[] pxl = new byte[bd.Stride * b.Height];
                Marshal.Copy(bd.Scan0, pxl, 0, bd.Stride * b.Height);
                stride = bd.Stride;
                return pxl;
            }
            finally
            {
                b.UnlockBits(bd);
            }
        }
    }
}

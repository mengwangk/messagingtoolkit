using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.Foundation;

using MessagingToolkit.Barcode.Common;



namespace MessagingToolkit.Barcode.Helper
{
    internal sealed class MatrixToStreamHelper
    {
        /// <summary>
        /// Async operation to invoke the stream generation function.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        /// <returns></returns>
        public static IAsyncOperation<IRandomAccessStream> Generate(BitMatrix matrix, Color foreColor, Color backColor)
        {
            return Task.Run(() => ToStream(matrix, foreColor, backColor)).AsAsyncOperation();
        }


        /// <summary>
        /// Renders a <see cref="BitMatrix"/> as an PNG image stream, where "false" bits are rendered
        /// as white, and "true" bits are rendered as black.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="foreColor">Color of the foreground</param>
        /// <param name="backColor">Color of the background</param>
        /// <returns></returns>
        internal static async Task<IRandomAccessStream> ToStream(BitMatrix matrix, Color foreColor, Color backColor)
        {
            var foreground = new byte[] { foreColor.B, foreColor.G, foreColor.R, foreColor.A };
            var background = new byte[] { backColor.B, backColor.G, backColor.R, backColor.A };
            int width = matrix.Width;
            int height = matrix.Height;
            var length = width * height;
            int emptyArea = 0;

            InMemoryRandomAccessStream pixelStream = new InMemoryRandomAccessStream();
            using (DataWriter writer = new DataWriter(pixelStream))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    for (int y = 0; y < height - emptyArea; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            var color = matrix.Get(x, y) ? foreground : background;
                            ms.Write(color, 0, 4);
                        }
                    }
                    writer.WriteBytes(ms.ToArray());
                }
                await writer.StoreAsync();
                await writer.FlushAsync();
                writer.DetachStream();

                //return pixelStream;

                // Convert the pixel data stream into PNG
                using (var inputStream = pixelStream.GetInputStreamAt(0))
                {
                    using (DataReader dataReader = new DataReader(inputStream))
                    {
                        // Once we have written the contents successfully we load the stream.
                        await dataReader.LoadAsync((uint)pixelStream.Size);
                        byte[] pixels = new byte[pixelStream.Size];
                        dataReader.ReadBytes(pixels);
                        dataReader.DetachStream();

                        using (IRandomAccessStream ras = new InMemoryRandomAccessStream())
                        {
                            ras.Seek(0);
                            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, ras);
                            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)width, (uint)height, 4, 4, pixels);
                            await encoder.FlushAsync();

                            // Convert to PNG
                            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(ras);
                            var pixelData = await decoder.GetPixelDataAsync();
                            InMemoryRandomAccessStream outStream = new InMemoryRandomAccessStream();
                            BitmapEncoder pngEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, outStream);
                            pngEncoder.SetPixelData(decoder.BitmapPixelFormat, BitmapAlphaMode.Ignore, decoder.PixelWidth, decoder.PixelHeight, decoder.DpiX, decoder.DpiY, pixelData.DetachPixelData());
                            await pngEncoder.FlushAsync();
                            await outStream.FlushAsync();
                            outStream.Seek(0);
                            return outStream;
                        }
                    }
                }
            }
        }

    }
}

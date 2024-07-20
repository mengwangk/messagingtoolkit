using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using MessagingToolkit.Barcode.QRCode.Decoder;
using MessagingToolkit.Barcode.Pdf417.Encoder;
using MessagingToolkit.Barcode.QRCode;
using MessagingToolkit.Barcode.Helper;
using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Client.Results;
using MessagingToolkit.Barcode.Multi;

namespace MessagingToolkit.Barcode.Demo.CommandLineRunner
{
    internal sealed class DecodeThread
    {
        private int successful;
        private Config config;
        private Inputs inputs;

        public DecodeThread(Config config, Inputs inputs)
        {
            this.config = config;
            this.inputs = inputs;
        }


        public void Run()
        {
            while (true)
            {
                String input = inputs.GetNextInput();
                if (input == null)
                {
                    break;
                }

                if (File.Exists(input))
                {
                    try
                    {
                        if (config.Multi)
                        {
                            Result[] results = DecodeMulti(new Uri(input), config.DecodeOptions);
                            if (results != null)
                            {
                                successful++;
                                if (config.DumpResults)
                                {
                                    DumpResultMulti(input, results);
                                }
                            }
                        }
                        else
                        {
                            Result result = Decode(new Uri(input), config.DecodeOptions);
                            if (result != null)
                            {
                                successful++;
                                if (config.DumpResults)
                                {
                                    DumpResult(input, result);
                                }
                            }
                        }
                    }
                    catch (IOException e)
                    {
                    }
                }
                else
                {
                    try
                    {
                        if (Decode(new Uri(input), config.DecodeOptions) != null)
                        {
                            successful++;
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        public int IsSuccessful()
        {
            return successful;
        }

        private static void DumpResult(string input, Result result)
        {
            int pos = input.LastIndexOf('.');
            if (pos > 0)
            {
                input = input.Substring(0, pos);
            }
            using (StreamWriter sw = File.CreateText(input + ".txt"))
            {
                sw.Write(result.Text);
            }

        }

        private static void DumpResultMulti(String input, Result[] results)
        {
            int pos = input.LastIndexOf('.');
            if (pos > 0)
            {
                input = input.Substring(0, pos);
            }

            using (StreamWriter sw = File.CreateText(input + ".txt"))
            {
                foreach (Result result in results)
                {
                    sw.WriteLine(result.Text);
                }
            }
        }


        private Result Decode(Uri uri, Dictionary<DecodeOptions, object> hints)
        {
            Image image;
            try
            {
                image = Image.FromFile(uri.LocalPath);
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Resource not found: " + uri);
            }
            if (image == null)
            {
                Console.WriteLine(uri.ToString() + ": Could not load image");
                return null;
            }

            try
            {
                LuminanceSource source;
                if (config.Crop == null)
                {
                    source = new BitmapLuminanceSource(new Bitmap(image));
                }
                else
                {
                    int[] crop = config.Crop;
                    source = new BitmapLuminanceSource(new Bitmap(image)).Crop(crop[0], crop[1], crop[2], crop[3]);
                }
                BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                if (config.DumpBlackPoint)
                {
                    DumpBlackPoint(uri, new Bitmap(image), bitmap);
                }
                Result result = new BarcodeDecoder().Decode(bitmap, hints);
                if (config.Brief)
                {
                    Console.Out.WriteLine(uri.ToString() + ": Success");
                }
                else
                {
                    ParsedResult parsedResult = ResultParser.ParseResult(result);
                    Console.Out.WriteLine(uri.ToString() + " (format: " + result.BarcodeFormat + ", type: " +
                        parsedResult.Type + "):\nRaw result:\n" + result.Text + "\nParsed result:\n" +
                        parsedResult.DisplayResult);

                    Console.Out.WriteLine("Found " + result.ResultPoints.Length + " result points.");
                    for (int i = 0; i < result.ResultPoints.Length; i++)
                    {
                        ResultPoint rp = result.ResultPoints[i];
                        Console.Out.WriteLine("  Point " + i + ": (" + rp.X + ',' + rp.Y + ')');
                    }
                }

                return result;
            }
            catch (NotFoundException nfe)
            {
                Console.Out.WriteLine(uri.ToString() + ": No barcode found");
                return null;
            }
        }

        private Result[] DecodeMulti(Uri uri, Dictionary<DecodeOptions, object> hints)
        {
            Image image;
            try
            {
                image = Image.FromFile(uri.LocalPath);
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("Resource not found: " + uri);
            }
            if (image == null)
            {
                Console.Out.WriteLine(uri.ToString() + ": Could not load image");
                return null;
            }
            try
            {
                LuminanceSource source;
                if (config.Crop == null)
                {
                    source = new BitmapLuminanceSource(new Bitmap(image));
                }
                else
                {
                    int[] crop = config.Crop;
                    source = new BitmapLuminanceSource(new Bitmap(image)).Crop(crop[0], crop[1], crop[2], crop[3]);
                }
                BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                if (config.DumpBlackPoint)
                {
                    DumpBlackPoint(uri, new Bitmap(image), bitmap);
                }

                BarcodeDecoder barcodeDecoder = new BarcodeDecoder();
                GenericMultipleBarcodeDecoder decoder = new GenericMultipleBarcodeDecoder(
                    barcodeDecoder);
                Result[] results = decoder.DecodeMultiple(bitmap, hints);

                if (config.Brief)
                {
                    Console.Out.WriteLine(uri.ToString() + ": Success");
                }
                else
                {
                    foreach (Result result in results)
                    {
                        ParsedResult parsedResult = ResultParser.ParseResult(result);
                        Console.Out.WriteLine(uri.ToString() + " (format: "
                            + result.BarcodeFormat + ", type: "
                            + parsedResult.Type + "):\nRaw result:\n"
                            + result.Text + "\nParsed result:\n"
                            + parsedResult.DisplayResult);
                        Console.Out.WriteLine("Found " + result.ResultPoints.Length + " result points.");
                        for (int i = 0; i < result.ResultPoints.Length; i++)
                        {
                            ResultPoint rp = result.ResultPoints[i];
                            Console.Out.WriteLine("  Point " + i + ": (" + rp.X + ',' + rp.Y + ')');
                        }
                    }
                }
                return results;
            }
            catch (NotFoundException nfe)
            {
                Console.Out.WriteLine(uri.ToString() + ": No barcode found");
                return null;
            }
        }

        /// <summary>
        /// Writes out a single PNG which is three times the width of the input image, containing from left
        /// to right: the original image, the row sampling monochrome version, and the 2D sampling
        /// monochrome version.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="image">The image.</param>
        /// <param name="bitmap">The bitmap.</param>
        private static void DumpBlackPoint(Uri uri, Bitmap image, BinaryBitmap bitmap)
        {
            String inputName = uri.LocalPath;
            if (inputName.Contains(".mono.png"))
            {
                return;
            }

            // Use the current working directory for URLs
            String resultName = inputName;
            int pos;
            if ("http".Equals(uri.Scheme))
            {
                pos = resultName.LastIndexOf('/');
                if (pos > 0)
                {
                    resultName = '.' + resultName.Substring(pos);
                }
            }
            pos = resultName.LastIndexOf('.');
            if (pos > 0)
            {
                resultName = resultName.Substring(0, pos);
            }
            resultName += ".mono.png";

            int width = bitmap.Width;
            int height = bitmap.Height;
            int stride = width * 3;
            var result = new Bitmap(stride, height, PixelFormat.Format32bppArgb);
            var offset = 0;

            // The original image
            for (int indexH = 0; indexH < height; indexH++)
            {
                for (int indexW = 0; indexW < width; indexW++)
                {
                    result.SetPixel(indexW, indexH, image.GetPixel(indexW, indexH));
                }
            }

            // Row sampling
            BitArray row = new BitArray(width);
            offset += width;
            for (int y = 0; y < height; y++)
            {
                row = bitmap.GetBlackRow(y, row);
                if (row == null)
                {
                    // If fetching the row failed, draw a red line and keep going.
                    for (int x = 0; x < width; x++)
                    {
                        result.SetPixel(offset + x, y, Color.Red);
                    }
                    continue;
                }

                for (int x = 0; x < width; x++)
                {
                    result.SetPixel(offset + x, y, row.Get(x) ? Color.Black : Color.White);
                }
            }

            // 2D sampling
            offset += width;
            for (int y = 0; y < height; y++)
            {
                BitMatrix matrix = bitmap.BlackMatrix;
                for (int x = 0; x < width; x++)
                {
                    result.SetPixel(offset + x, y, matrix.Get(x, y) ? Color.Black : Color.White);
                }
            }

            result.Save(resultName, ImageFormat.Png);
        }

        private static void WriteResultImage(int stride,
                                             int height,
                                             int[] pixels,
                                             Uri uri,
                                             String inputName,
                                             String suffix)
        {
            // Write the result
            Bitmap result = new Bitmap(stride, height, PixelFormat.Format24bppRgb);
            SetRGB(result, 0, 0, stride, height, pixels, 0, stride);

            // Use the current working directory for URLs
            String resultName = inputName;
            int pos = 0;
            if ("http".Equals(uri.Scheme))
            {
                pos = resultName.LastIndexOf('/');
                if (pos > 0)
                {
                    resultName = '.' + resultName.Substring(pos);
                }
            }
            pos = resultName.LastIndexOf('.');
            if (pos > 0)
            {
                resultName = resultName.Substring(0, pos);
            }
            resultName += suffix;
            try
            {
                result.Save(resultName, ImageFormat.Png);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }

        public static void GetRGB(Bitmap image, int startX, int startY, int w, int h, int[] rgbArray, int offset, int scansize)
        {
            const int PixelWidth = 3;
            const PixelFormat PixelFormat = PixelFormat.Format24bppRgb;

            if (image == null) throw new ArgumentNullException("image");
            if (rgbArray == null) throw new ArgumentNullException("rgbArray");
            if (startX < 0 || startX + w > image.Width) throw new ArgumentOutOfRangeException("startX");
            if (startY < 0 || startY + h > image.Height) throw new ArgumentOutOfRangeException("startY");
            if (w < 0 || w > scansize || w > image.Width) throw new ArgumentOutOfRangeException("w");
            if (h < 0 || (rgbArray.Length < offset + h * scansize) || h > image.Height) throw new ArgumentOutOfRangeException("h");

            BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat);
            try
            {
                byte[] pixelData = new Byte[data.Stride];
                for (int scanline = 0; scanline < data.Height; scanline++)
                {
                    Marshal.Copy(data.Scan0 + (scanline * data.Stride), pixelData, 0, data.Stride);
                    for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                    {
                        // PixelFormat.Format32bppRgb means the data is stored
                        // in memory as BGR. We want RGB, so we must do some 
                        // bit-shuffling.
                        rgbArray[offset + (scanline * scansize) + pixeloffset] =
                            (pixelData[pixeloffset * PixelWidth + 2] << 16) +   // R 
                            (pixelData[pixeloffset * PixelWidth + 1] << 8) +    // G
                            pixelData[pixeloffset * PixelWidth];                // B
                    }
                }
            }
            finally
            {
                image.UnlockBits(data);
            }
        }

        public static void SetRGB(Bitmap image, int startX, int startY, int w, int h, int[] rgbArray, int offset, int scansize)
        {
            for (int i = startX; i < w; i++)
            {
                for (int j = startY; j < h; j++)
                {
                    int p = j * scansize + (i >> 5);
                    bool c = (((int)(((uint)rgbArray[offset]) >> (i & 0x1f))) & 1) != 0;
                    if (c)
                        image.SetPixel(i, j, Color.Black);
                    else
                        image.SetPixel(i, j, Color.White);
                }
            }
        }
    }
}

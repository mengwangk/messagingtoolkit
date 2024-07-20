//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;
using System.Collections;

using MessagingToolkit.Barcode.Multi;
using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Demo
{
    public partial class frmMain : Form
    {
        private Bitmap bitmap = null;
        private MultiFormatReader barcodeReader = new MultiFormatReader();
        private MultiFormatWriter barcodeWriter = new MultiFormatWriter();

        private Dictionary<string, BarcodeFormat> barcodeFormat = new Dictionary<string, BarcodeFormat>
        {
            { "QRCode", BarcodeFormat.QrCode },
            { "EAN 8", BarcodeFormat.Ean8},
            { "EAN 13", BarcodeFormat.Ean13},                       
        };

        public frmMain()
        {            
            InitializeComponent();
        }

      

        private void btnEncode_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboBarcodeType.Text) && !string.IsNullOrEmpty(txtBarcodeData.Text))
            {
                try
                {
                    BarcodeFormat fm;
                    if (barcodeFormat.TryGetValue(cboBarcodeType.Text, out fm)) 
                    {
                        ByteMatrix bt = barcodeWriter.Encode(txtBarcodeData.Text, fm, (int)numWidth.Value, (int)numHeight.Value);
                        picEncode.Image = ConvertByteMatrixToImage(bt);
                    }                      
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            InitializeApp();
        }

        private void InitializeApp()
        {           
            cboBarcodeType.Items.AddRange(barcodeFormat.Keys.ToArray());
            cboBarcodeType.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Bitmap bm = (Bitmap)picEncode.Image;
            if (bm != null)
            {
                SaveFileDialog sdlg = new SaveFileDialog();
                sdlg.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
                if (sdlg.ShowDialog() == DialogResult.OK)
                {
                    bm.Save(sdlg.FileName, ImageFormat.Png);
                }
            }
        }




        public enum RGBcoefficientType
        {
            BT709,
        }
        public class GrayScaleCoef
        {
            public double cr;
            public double cg;
            public double cb;

            public GrayScaleCoef(double cr, double cg, double cb)
            {
                this.cr = cr;
                this.cg = cg;
                this.cb = cb;
            }
        }


        private static unsafe void GetGrayScaleImage(BitmapData sourceData, BitmapData destinationData, GrayScaleCoef coef)
        {
            // get width and height
            int width = sourceData.Width;
            int height = sourceData.Height;


            PixelFormat srcPixelFormat = sourceData.PixelFormat;

            if (
                (srcPixelFormat == PixelFormat.Format24bppRgb) ||
                (srcPixelFormat == PixelFormat.Format32bppRgb) ||
                (srcPixelFormat == PixelFormat.Format32bppArgb))
            {
                int pixelSize = (srcPixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
                int srcOffset = sourceData.Stride - width * pixelSize;
                int dstOffset = destinationData.Stride - width;

                // do the job
                byte* src = (byte*)sourceData.Scan0.ToPointer();
                byte* dst = (byte*)destinationData.Scan0.ToPointer();

                // for each line
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    for (int x = 0; x < width; x++, src += pixelSize, dst++)
                    {
                        *dst = (byte)(coef.cr * src[2] + coef.cg * src[1] + coef.cb * src[0]);
                    }
                    src += srcOffset;
                    dst += dstOffset;
                }
            }
        }
        public static Bitmap GrayScale(Bitmap image, RGBcoefficientType otype)
        {
            Bitmap dstimage = null;
            try
            {
                GrayScaleCoef coef;
                if (otype == RGBcoefficientType.BT709)
                    coef = new GrayScaleCoef(0.2125, 0.7154, 0.0721);
                else
                    coef = new GrayScaleCoef(0.5, 0.419, 0.081);


                BitmapData sourceData, destinationData;

                sourceData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
                dstimage = CreateGrayscaleImage(image.Width, image.Height);
                destinationData = dstimage.LockBits(new Rectangle(0, 0, dstimage.Width, dstimage.Height), ImageLockMode.ReadWrite, dstimage.PixelFormat);
                GetGrayScaleImage(sourceData, destinationData, coef);
                dstimage.UnlockBits(destinationData);
                image.UnlockBits(sourceData);
            }
            catch
            {

            }
            return dstimage;
        }
        public static Bitmap CreateGrayscaleImage(int width, int height)
        {
            // create new image
            Bitmap image = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            // set palette to grayscale
            // get palette
            ColorPalette cp = image.Palette;
            // init palette
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }
            // set palette back
            image.Palette = cp;
            // return new image
            return image;
        }


        private unsafe Bitmap ConvertByteMatrixToImage(ByteMatrix bm)
        {
            Bitmap image = CreateGrayscaleImage(bm.Width, bm.Height);
            BitmapData sourceData;

            sourceData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            int width = sourceData.Width;
            int height = sourceData.Height;
            int srcOffset = sourceData.Stride - width;
            byte* src = (byte*)sourceData.Scan0.ToPointer();
            for (int y = 0; y < height; y++)
            {
                // for each pixel
                for (int x = 0; x < width; x++, src++)
                {
                    *src = (byte)bm.Array[y][x];
                }
                src += srcOffset;
            }

            image.UnlockBits(sourceData);
            return image;
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                bitmap = new Bitmap(fd.FileName);
                picBoxDecode.Image = (Image)bitmap;
                txtFile.Text = fd.FileName;
                txtBarcodeDirection.Text = string.Empty;
                txtBarcodeResult.Text = string.Empty;
                txtBarcodeType.Text = string.Empty;
            }
        }

        private unsafe string DecodeBarcode(Bitmap bmpo, int BarFormat, int W, int H)
        {
            Hashtable hints = new Hashtable();
            ArrayList fmts = new ArrayList();

            if (chkDataMatrix.Checked == true)
            {
                fmts.Add(BarcodeFormat.DataMatrix);
            }
            if (chkQRCode.Checked == true)
            {
                fmts.Add(BarcodeFormat.QrCode);
            }
            if (chkPdf417.Checked == true)
            {
                fmts.Add(BarcodeFormat.Pdf417);
            }
            if (chkUpcE.Checked == true)
            {
                fmts.Add(BarcodeFormat.UpcE);
            }
            if (chkUpcA.Checked == true)
            {
                fmts.Add(BarcodeFormat.UpcA);
            }
            if (chkCode128.Checked == true)
            {
                fmts.Add(BarcodeFormat.Code128);
            }
            if (chkCode39.Checked == true)
            {
                fmts.Add(BarcodeFormat.Code39);
            }
            if (chkItf.Checked == true)
            {
                fmts.Add(BarcodeFormat.Itf);
            }
            if (chkEan8.Checked == true)
            {
                fmts.Add(BarcodeFormat.Ean8);
            }
            if (chkEan13.Checked == true)
            {
                fmts.Add(BarcodeFormat.Ean13);
            }
            if (chkTryHarder.Checked == true)
            {
                hints.Add(DecodeHintType.TryHarder, true);
            }
            if (chkTryAllFormats.Checked == true)
            {
                hints.Add(DecodeHintType.PossibleFormats, fmts);
            }
            Result rawResult;
            RGBLuminanceSource r = new RGBLuminanceSource(bmpo, W, H);
            GlobalHistogramBinarizer x = new GlobalHistogramBinarizer(r);
            BinaryBitmap bitmap = new BinaryBitmap(x);
            int count = 0;
            {
                try
                {
                    rawResult = barcodeReader.Decode(bitmap, hints);
                    if (rawResult != null)
                    {
                        count++;
                        txtBarcodeResult.Text = rawResult.Text;
                        txtBarcodeType.Text = rawResult.BarcodeFormat.ToString();
                    }
                }
                catch (ReaderException e)
                {
                    MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return null;
        }

        private void btnDecode_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            DecodeBarcode(bitmap, -1, bitmap.Width, bitmap.Height);
            TimeSpan ts = DateTime.Now.Subtract(dt);
            lblTime.Text = ts.Milliseconds.ToString();
        }

        private void chkAll1D_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAll1D.Checked == true)
            {
                chkCode128.Checked = true;
                chkCode39.Checked = true;
                chkUpcA.Checked = true;
                chkUpcE.Checked = true;
                chkEan13.Checked = true;
                chkEan8.Checked = true;
                chkItf.Checked = true;
            }
            else
            {
                chkCode128.Checked = false;
                chkCode39.Checked = false;
                chkUpcA.Checked = false;
                chkUpcE.Checked = false;
                chkEan13.Checked = false;
                chkEan8.Checked = false;
                chkItf.Checked = false;
            }
        }

        private void chkAll2D_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAll2D.Checked == true)
            {
                chkDataMatrix.Checked = true;
                chkQRCode.Checked = true;
                chkPdf417.Checked = true;
            }
            else
            {
                chkDataMatrix.Checked = false;
                chkQRCode.Checked = false;
                chkPdf417.Checked = false;
            }
        }
    }
}

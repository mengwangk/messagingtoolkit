using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;

using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using MessagingToolkit.QRCode.Helper;

namespace QRCodeSample
{
    public partial class QrCodeSampleApp : Form
    {

        private static Dictionary<string, string> CharacterSets =
                        new Dictionary<string, string>
                        {
                             { "Default UTF-8", "UTF-8"},
                             { "ISO-8859-1", "ISO-8859-1"},
                             { "SHIFT-JIS", "SHIFT-JIS"},
                             { "CP437", "CP437"},
                             { "ISO-8859-2", "ISO-8859-2"},
                             { "ISO-8859-3", "ISO-8859-3"},
                             { "ISO-8859-4", "ISO-8859-4"},
                             { "ISO-8859-5", "ISO-8859-5"},
                             { "ISO-8859-6", "ISO-8859-6"},
                             { "ISO-8859-7", "ISO-8859-7"},
                             { "ISO-8859-8", "ISO-8859-8"},
                             { "ISO-8859-9", "ISO-8859-9"},
                             { "ISO-8859-10", "ISO-8859-10"},
                             { "ISO-8859-11", "ISO-8859-11"},
                             { "ISO-8859-13", "ISO-8859-13"},
                             { "ISO-8859-14", "ISO-8859-14"},
                             { "ISO-8859-15", "ISO-8859-15"},
                             { "ISO-8859-16", "ISO-8859-16"},
                        };

        public QrCodeSampleApp()
        {
            InitializeComponent();
        }

        private void frmSample_Load(object sender, EventArgs e)
        {
            cboEncoding.SelectedIndex = 2;
            cboVersion.SelectedIndex = 0;
            cboCorrectionLevel.SelectedIndex = 1;

            // Populate the character set
            cboCharacterSet.Items.AddRange(CharacterSets.Keys.ToArray());
            cboCharacterSet.SelectedIndex = 0;
        }

     
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnEncode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEncodeData.Text.Trim()))
            {
                MessageBox.Show("Data must not be empty.");
                return;
            }
            
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeBackgroundColor = btnQRCodeBackColor.BackColor;
            qrCodeEncoder.QRCodeForegroundColor = btnQRCodeForeColor.BackColor;
            qrCodeEncoder.CharacterSet = CharacterSets[cboCharacterSet.Text];
            String encoding = cboEncoding.Text ;
            if (encoding == "Byte") {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            } else if (encoding == "AlphaNumeric") {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;            
            } else if (encoding == "Numeric") {
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;            
            }
            try {
                int scale = Convert.ToInt16(txtSize.Text);
                qrCodeEncoder.QRCodeScale = scale;
            } catch (Exception ex) {
                MessageBox.Show("Invalid size!");
                return;
            }
            try {
                int version = Convert.ToInt16(cboVersion.Text) ;
                qrCodeEncoder.QRCodeVersion = version;
            } catch (Exception ex) {
                MessageBox.Show("Invalid version !");
            }

            string errorCorrect = cboCorrectionLevel.Text;
            if (errorCorrect == "L")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (errorCorrect == "M")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (errorCorrect == "Q")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (errorCorrect == "H")
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            Image image;
            String data = txtEncodeData.Text;
            image = qrCodeEncoder.Encode(data);                      
            picEncode.Image = image;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png";
            saveFileDialog1.Title = "Save";
            saveFileDialog1.FileName = string.Empty;
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                System.IO.FileStream fs =
                   (System.IO.FileStream)saveFileDialog1.OpenFile();
                // Saves the Image in the appropriate ImageFormat based upon the
                // File type selected in the dialog box.
                // NOTE that the FilterIndex property is one-based.
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        this.picEncode.Image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        this.picEncode.Image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        this.picEncode.Image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case 4:
                        this.picEncode.Image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }

                fs.Close();
            }

            //openFileDialog1.InitialDirectory = "c:\\";
            //openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //openFileDialog1.FilterIndex = 2;
            //openFileDialog1.RestoreDirectory = true;

            //if (openFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    MessageBox.Show(openFileDialog1.FileName); 
            //}

        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1 ;
            DialogResult r = printDialog1.ShowDialog();
            if ( r == DialogResult.OK ) {
                printDocument1.Print();
            }            
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(picEncode.Image,0,0);          
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FileName = string.Empty;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String fileName = openFileDialog1.FileName;               
                picDecode.Image = new Bitmap(fileName);
                
            }
        }

        private void btnDecode_Click(object sender, EventArgs e)
        {           
            try
            {
                if (picDecode.Image == null) return;
                QRCodeDecoder decoder = new QRCodeDecoder();
                String decodedString = decoder.Decode(new QRCodeBitmapImage(new Bitmap(picDecode.Image)));
                txtDecodedData.Text = decodedString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnQRCodeBackColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnQRCodeBackColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnQRCodeBackColor.BackColor = colorDialog1.Color;
            }
        }

        private void btnQRCodeForeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.AllowFullOpen = true;
            colorDialog1.ShowHelp = true;
            colorDialog1.Color = this.btnQRCodeForeColor.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.btnQRCodeForeColor.BackColor = colorDialog1.Color;
            }
        }
     
     }
}
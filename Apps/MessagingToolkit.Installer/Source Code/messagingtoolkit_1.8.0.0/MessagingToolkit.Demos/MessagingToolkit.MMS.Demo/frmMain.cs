//===============================================================================
// OSML - Open Source Messaging Library
//
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

using MessagingToolkit.MMS;

namespace MessagingToolkit.MMS.Demo
{
    public partial class frmMain : Form
    {
        /// <summary>
        /// Content types
        /// </summary>
        private Dictionary<string, string> ContentType =
             new Dictionary<string, string> { 
                                             { "Plain Text", MultimediaMessageConstants.ContentTypeTextPlain },
                                             { "HTML", MultimediaMessageConstants.ContentTypeTextHtml },                                              
                                             { "WML", MultimediaMessageConstants.ContentTypeTextWml },
                                             { "GIF", MultimediaMessageConstants.ContentTypeImageGif },
                                             { "JPEG", MultimediaMessageConstants.ContentTypeImageJpeg },
                                             { "TIFF", MultimediaMessageConstants.ContentTypeImageTiff },
                                             { "PNG", MultimediaMessageConstants.ContentTypeImagePng },
                                             { "WBMP", MultimediaMessageConstants.ContentTypeImageWbmp },
                                             { "AMR", MultimediaMessageConstants.ContentTypeAudioAmr }
                                            };


        /// <summary>
        /// List of message contents
        /// </summary>
        private List<MessageContent> contents = new List<MessageContent>(3);

        public frmMain()
        {            
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
           InitializeApp();
        }

        private void InitializeApp()
        {
            txtTransactionId.Text = "1234567890";
            txtPresentationId.Text = "<0000>";

            // Add parity        
            cboContentType.Items.Clear();
            cboContentType.Items.AddRange(ContentType.Keys.ToArray());
            cboContentType.SelectedIndex = 0;

            lstContent.Items.Clear();

            contents.Clear();

            txtFrom.Text = string.Empty;
            txtTo.Text = string.Empty;
            txtSubject.Text = string.Empty;
            txtContentId.Text = string.Empty;
            txtContentFileName.Text = string.Empty;
        }
                
        private void ShowError(string errorMessage)
        {
            MessageBox.Show(errorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnBrowseContent_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "An Files (*.*)|*.*";
            openFileDialog1.FileName = string.Empty;
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                txtContentFileName.Text = fileName;
            }
        }

        private void btnBrowserMMS_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "MMS File (*.mms)|*.mms";
            openFileDialog1.FileName = string.Empty;
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;

                // Display the MMS file location
                txtMMSFileLocation.Text = fileName;
            }
        }

        private void btnDecode_Click(object sender, EventArgs e)
        {
            string fileName = txtMMSFileLocation.Text;
            if (string.IsNullOrEmpty(fileName))
            {
                ShowError("MMS file is not specified");
                return;
            }
            MultimediaMessageDecoder decoder = new MultimediaMessageDecoder(fileName);
            decoder.DecodeMessage();
            MultimediaMessage message = decoder.GetMessage();

            txtResults.Text = message.ToString() + "\n\n";

            txtResults.AppendText("Content" + Environment.NewLine);
            txtResults.AppendText(string.Empty.PadRight("Content".Length + 5, '=') + "\n");
            Control[] controls = new Control[message.NumContents];
            int imageCount = 0;
            for (int i =0; i < message.NumContents; i++) 
            {
                txtResults.AppendText("\n");
                MultimediaMessageContent content = message.GetContent(i);
                txtResults.AppendText(content.ToString());
                txtResults.AppendText("\n");

                // Image type - GIF, JPEG, etc...
                if (content.Type.StartsWith("image"))
                {
                    Image image = ByteArrayToImage(content.GetContent());
                    if (image != null)
                    {
                        PictureBox pic = new PictureBox();
                        pic.Width = image.Width;
                        pic.Height = image.Height;
                        pic.Image = image;
                        controls[imageCount] = pic;
                        pic.Top = imageCount * image.Height;
                        imageCount++;
                    }
                }
            }
            panelImages.Controls.Clear();            
            panelImages.Controls.AddRange(controls);

            
        }

        public Image ByteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            catch (Exception ex)
            {
                ShowError(ex.ToString());
            }
            return null;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTransactionId.Text))
            {
                ShowError("Transaction id cannot be empty");
                return;
            }
            if (string.IsNullOrEmpty(txtPresentationId.Text))
            {
                ShowError("Presentation id cannot be empty");
                return;
            }
            if (string.IsNullOrEmpty(txtFrom.Text))
            {
                ShowError("From cannot be empty");
                return;
            }
            if (string.IsNullOrEmpty(txtTo.Text))
            {
                ShowError("To cannot be empty");
                return;
            }
            if (string.IsNullOrEmpty(txtSubject.Text))
            {
                ShowError("Subject cannot be empty");
                return;
            }

            if (lstContent.Items.Count <= 0)
            {
                ShowError("At least 1 content must be added");
                return;
            }

            // Set the headers
            MultimediaMessage message = new MultimediaMessage();
            SetHeaders(message);

            // Add the contents
            AddContents(message);

            MultimediaMessageEncoder encoder = new MultimediaMessageEncoder();
            encoder.SetMessage(message);

            try
            {
                // 3)Encode the message
                encoder.EncodeMessage();
                byte[] output = encoder.GetMessage();

                saveFileDialog1.Filter = "MMS File (*.mms)|*.mms";
                saveFileDialog1.FileName = string.Empty;
                DialogResult dialogResult = saveFileDialog1.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string saveFileName = saveFileDialog1.FileName;
                    encoder.SaveToFile(saveFileName);
                    MessageBox.Show("File is saved successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }                              
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                ShowError(ex.StackTrace);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            InitializeApp();
        }


        /// <summary>
        /// Adds the contents.
        /// </summary>
        /// <param name="message">The message.</param>
        private void AddContents(MultimediaMessage message)
        {
            foreach (MessageContent content in contents)
            {
                MultimediaMessageContent multimediaMessageContent = new MultimediaMessageContent();
                multimediaMessageContent.SetContent(content.FileName);
                multimediaMessageContent.ContentId = content.ContentId;  //If "<>" are not used with this method, the result is Content-Location
                multimediaMessageContent.Type = content.ContentType;
                message.AddContent(multimediaMessageContent);
            }         
        }

        /// <summary>
        /// Sets the headers.
        /// </summary>
        /// <param name="message">Multimedia message</param>
        private void SetHeaders(MultimediaMessage message)
        {         
            message.MessageType = MultimediaMessageConstants.MessageTypeMSendReq;
            message.TransactionId = txtTransactionId.Text;
            message.Version = MultimediaMessageConstants.MmsVersion10;

            message.SetFrom(txtFrom.Text + "/TYPE=PLMN");
            message.AddToAddress(txtTo.Text + "/TYPE=PLMN");

            message.Subject = txtSubject.Text;    // Subject is optional

            // ContentType is mandatory, and must be last header!  These last 3 lines set the ContentType to
            // application/vnd.wml.multipart.related;type="application/smil";start="<0000>"
            // In case of multipart.mixed, only the first line is needed (and change the constant)

            // Any string will do for the Content-ID, but it must match that used for the presentation part,
            // and it must be Content-ID, it cannot be Content-Location.

            message.ContentType = MultimediaMessageConstants.ContentTypeApplicationMultipartRelated;

            // presentation part is written in SMIL
            message.MultipartRelatedType = MultimediaMessageConstants.ContentTypeApplicationSmil;
            
            // presentation part has Content-ID=<0000>
            message.PresentationId = txtPresentationId.Text; 
        }

        private void btnAddContent_Click(object sender, EventArgs e)
        {        
            if (string.IsNullOrEmpty(txtContentFileName.Text))
            {
                ShowError("Content file name must be specified");
                return;
            }
            if (string.IsNullOrEmpty(txtContentId.Text))
            {
                ShowError("Content id cannot be empty");
                return;
            }

            if (!File.Exists(txtContentFileName.Text))
            {
                ShowError(txtContentFileName.Text + " does not exist");
                return;
            }

            string contentType;
            if (ContentType.TryGetValue(cboContentType.Text, out contentType))
            {
                MessageContent content = new MessageContent(contentType, txtContentId.Text.Trim(), txtContentFileName.Text.Trim());
                contents.Add(content);
                lstContent.Items.Add(cboContentType.Text + "| " + txtContentId.Text + "| " + txtContentFileName.Text);
            }
        }

    }
}

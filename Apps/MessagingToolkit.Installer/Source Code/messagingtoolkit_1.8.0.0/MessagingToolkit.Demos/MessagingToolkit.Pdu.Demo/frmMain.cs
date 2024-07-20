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
using System.IO.Ports;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using MessagingToolkit.Pdu;
using MessagingToolkit.Pdu.Ie;
using MessagingToolkit.Pdu.WapPush;

namespace MessagingToolkit.Pdu.Demo
{
    /// <summary>
    /// Main form
    /// </summary>
    public partial class frmMain : Form
    {
        /// <summary>
        /// Custom date time format
        /// </summary>
        private static string CustomDateTimeFormat = "dd MMM yyyy, hh:mm:ss tt";

        // Message class
        private Dictionary<string, int> MessageClass =
              new Dictionary<string, int>
            {
                {"None", 0},
                {"ME", PduUtils.DcsMessageClassMe},
                {"SIM", PduUtils.DcsMessageClassSim},
                {"TE", PduUtils.DcsMessageClassTe}                                                 
            };

        /// <summary>
        /// Message encoding
        /// </summary>
        private Dictionary<string, int> MessageEncoding =
            new Dictionary<string, int>
            {
                {"Default Alphabet - 7 Bits", PduUtils.DcsEncoding7Bit},
                {"ANSI - 8 Bits",PduUtils.DcsEncoding8Bit},
                {"Unicode - 16 Bits", PduUtils.DcsEncodingUcs2}
            };


        // WAP push signal
        private Dictionary<string, int> ServiceIndication =
            new Dictionary<string, int>
            {
                {"None", WapSiPdu.WapSignalNone},
                {"Low", WapSiPdu.WapSignalLow },
                {"Medium",WapSiPdu.WapSignalMedium},
                {"High", WapSiPdu.WapSignalHigh}                         
            };

        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the frmSMS control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Add message encoding
            cboPduEncoding.Items.AddRange(MessageEncoding.Keys.ToArray());
            cboPduEncoding.SelectedIndex = 0;

            cboMessageClass.Items.AddRange(MessageClass.Keys.ToArray());
            cboMessageClass.SelectedIndex = 0;
            
            cboWapPushSignal.Items.AddRange(ServiceIndication.Keys.ToArray());
            cboWapPushSignal.SelectedIndex = 0;

            dtpWapPushCreated.CustomFormat = CustomDateTimeFormat;
            dtpWapPushCreated.MinDate = DateTime.Now;
            dtpWapPushExpiryDate.CustomFormat = CustomDateTimeFormat;
            dtpWapPushExpiryDate.MinDate = DateTime.Now.AddMinutes(30);

        }


        /// <summary>
        /// Handles the Click event of the btnDecodePdu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnDecodePdu_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPdu.Text))
                {
                    PduParser pduParser = new PduParser();
                    PduFactory pduFactory = new PduFactory();
                    PduGenerator pduGenerator = new PduGenerator();
                    Pdu pdu;
                    pdu = pduParser.ParsePdu(txtPdu.Text.Trim());
                    if (pdu.Binary)
                    {
                        pdu.SetDataBytes(pdu.UserDataAsBytes);
                    }

                    txtDecodedPdu.Text = pdu.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnGetPduCode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnGetPduCode_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPduDestinationNumber.Text) &&
                    !string.IsNullOrEmpty(txtUserData.Text)
                    )
                {
                    btnGetPduCode.Enabled = false;
                    SmsSubmitPdu pdu;
                    if (chkPduStatusReport.Checked)
                    {
                        pdu = PduFactory.NewSmsSubmitPdu(PduUtils.TpSrrReport | PduUtils.TpVpfInteger);
                    }
                    else
                    {
                        pdu = PduFactory.NewSmsSubmitPdu();
                    }
                    if (!string.IsNullOrEmpty(txtPduServiceCentreAddress.Text))
                    {
                        pdu.SmscAddress = txtPduServiceCentreAddress.Text;
                        string smscNumberForLengthCheck = pdu.SmscAddress;
                        if (pdu.SmscAddress.StartsWith("+"))
                        {
                            smscNumberForLengthCheck = pdu.SmscAddress.Substring(1);
                        }
                        pdu.SmscInfoLength = 1 + (smscNumberForLengthCheck.Length / 2) + ((smscNumberForLengthCheck.Length % 2 == 1) ? 1 : 0);
                        pdu.SmscAddressType = PduUtils.GetAddressTypeFor(txtPduServiceCentreAddress.Text);
                    }
                    else
                    {
                        pdu.SmscAddress = string.Empty;
                    }

                    pdu.Address = txtPduDestinationNumber.Text;
                    string userData = txtUserData.Text;

                    // Set message encoding
                    int encoding;
                    if (MessageEncoding.TryGetValue(cboPduEncoding.Text, out encoding))
                    {
                        pdu.DataCodingScheme = encoding;
                    }

                    // Set the message class
                    int messageClass;
                    if (MessageClass.TryGetValue(cboMessageClass.Text, out messageClass))
                    {
                        if (messageClass != 0)
                        {
                            pdu.DataCodingScheme = pdu.DataCodingScheme | messageClass;
                        }
                    }

                    if (encoding == PduUtils.DcsEncoding8Bit)
                    {
                        if (GetDataCodingScheme(userData) == PduUtils.DcsEncodingUcs2)
                        {
                            pdu.SetDataBytes(Encoding.GetEncoding("UTF-16").GetBytes(userData));
                        }
                        else
                        {
                            pdu.SetDataBytes(Encoding.ASCII.GetBytes(userData));
                        }
                    }
                    else
                    {
                        pdu.DecodedText = userData;
                    }

                    pdu.ValidityPeriod = Convert.ToDouble(nupdPduValidityPeriod.Value);
                    pdu.ProtocolIdentifier = 0;
                    pdu.MessageReference = Convert.ToInt32(nupdPduMessageReferenceNo.Value);
                    if (nupdPduDestinationPort.Value > 0 || nupdPduSourcePort.Value > 0)
                    {
                        pdu.AddInformationElement(InformationElementFactory.GeneratePortInfo(Convert.ToInt32(nupdPduDestinationPort.Value), Convert.ToInt32(nupdPduSourcePort.Value)));
                    }
                    if (chkPduFlashMessage.Checked) pdu.DataCodingScheme = pdu.DataCodingScheme | PduUtils.DcsMessageClassFlash;

                    if (!string.IsNullOrEmpty(txtReplyPath.Text))
                    {
                        if (txtReplyPath.Text.StartsWith("+"))
                        {
                            pdu.AddReplyPath(txtReplyPath.Text.Substring(1), PduUtils.AddressTypeInternationFormat);
                        }
                        else
                        {
                            pdu.AddReplyPath(txtReplyPath.Text, PduUtils.AddressTypeDomesticFormat);
                        }
                    }
                    int maxMessage = pdu.MpMaxNo;
                    PduGenerator pduGenerator = new PduGenerator();
                    List<string> pduList = pduGenerator.GeneratePduList(pdu, Convert.ToInt32(nupdPduMessageReferenceNo.Value));
                    txtPduCode.Text = string.Empty;
                    foreach (string pduString in pduList)
                    {
                        txtPduCode.AppendText("PDU\r\n");
                        txtPduCode.AppendText("---------\r\n");
                        txtPduCode.AppendText(pduString);
                        txtPduCode.AppendText("\r\n");
                        txtPduCode.AppendText("Length: " + GetAtLength(pduString));
                        txtPduCode.AppendText("\r\n\r\n");
                    }
                }
                else
                {
                    MessageBox.Show("Destination number and user data cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGetPduCode.Enabled = true;
            }
        }

        /// <summary>
        /// Determine the message data coding scheme
        /// </summary>
        /// <param name="content">Message content</param>
        /// <returns>Message data coding scheme.</returns>
        private static int GetDataCodingScheme(string content)
        {
            int i = 0;
            for (i = 1; i <= content.Length; i++)
            {
                int code = Convert.ToInt32(Convert.ToChar(content.Substring(i - 1, 1)));
                if (code < 0 || code > 255)
                {
                    return PduUtils.DcsEncodingUcs2;
                }
            }
            return PduUtils.DcsEncoding7Bit;
        }
        /// <summary>
        /// Calculate message length
        /// </summary>
        /// <param name="pduString">PDU string</param>
        /// <returns>Message length</returns>
        protected int GetAtLength(string pduString)
        {
            // Get AT command length
            return (pduString.Length - Convert.ToInt32(pduString.Substring(0, 2), 16) * 2 - 2) / 2;
        }

        private void tabMain_Click(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            if (tc.SelectedTab == tabAbout)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Pdu));
                string name = assembly.GetName().Name;
                string version = assembly.GetName().Version.ToString();
                string title = string.Empty;
                string description = string.Empty;
                object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length == 1)
                {
                    title = ((AssemblyTitleAttribute)attributes[0]).Title;
                }
                attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 1)
                {
                    description = ((AssemblyDescriptionAttribute)attributes[0]).Description;
                }
                lblAbout.Text = title + "\n" + version;

            }
        }

        private void btnGetWapPushPDU_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtWapPushPhoneNumber.Text) &&
                    !string.IsNullOrEmpty(txtWapPushUrl.Text) &&
                    !string.IsNullOrEmpty(txtWapPushMessage.Text)
                    )
                {
                    btnGetWapPushPDU.Enabled = false;
                    WapSiPdu pdu;
                    pdu = PduFactory.NewWapSiPdu();
                    pdu.Address = txtWapPushPhoneNumber.Text;
                    pdu.Url = txtWapPushUrl.Text;
                    pdu.IndicationText = txtWapPushMessage.Text;
                    int signal = ServiceIndication[cboWapPushSignal.Text];
                    pdu.WapSignal = signal;
                    if (chkWapPushCreated.Checked)
                        pdu.CreateDate = dtpWapPushCreated.Value;
                    
                    if (chkWapPushExpiry.Checked)
                        pdu.ExpireDate = dtpWapPushExpiryDate.Value;

                    PduGenerator pduGenerator = new PduGenerator();

                    // You can assign a reference number here
                    List<string> pduList = pduGenerator.GeneratePduList(pdu, 0);
                    txtWapPushPDU.Text = string.Empty;
                    foreach (string pduString in pduList)
                    {
                        txtWapPushPDU.AppendText("PDU\r\n");
                        txtWapPushPDU.AppendText("---------\r\n");
                        txtWapPushPDU.AppendText(pduString);
                        txtWapPushPDU.AppendText("\r\n");
                        txtWapPushPDU.AppendText("Length: " + GetAtLength(pduString));
                        txtWapPushPDU.AppendText("\r\n\r\n");
                    }
                }
                else
                {
                    MessageBox.Show("Destination number, URL and message cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGetWapPushPDU.Enabled = true;
            }
        }
    }
}
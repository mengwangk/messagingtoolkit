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
using System.Reflection;

using MessagingToolkit.Core.Smpp;
using MessagingToolkit.Core.Smpp.Packet;
using MessagingToolkit.Core.Smpp.Utility;
using MessagingToolkit.Core.Smpp.Packet.Request;
using MessagingToolkit.Core.Smpp.Packet.Response;
using MessagingToolkit.Core.Smpp.EventObjects;

namespace MessagingToolkit.Core.Utilities
{
    public partial class frmSMPP : Form
    {
        /// <summary>
        /// SMPP gateway interface
        /// </summary>
        private ISmppGateway smppGateway = SmppGatewayFactory.Default;

        /// <summary>
        /// System mode - bind type
        /// </summary>
        private Dictionary<string, BindingType> SystemMode =
            new Dictionary<string, BindingType> { { "Transceiver", BindingType.BindAsTransceiver }, { "Transmitter", BindingType.BindAsTransmitter }, { "Receiver", BindingType.BindAsReceiver } };

        /// <summary>
        /// Interface version
        /// </summary>
        private Dictionary<string, SmppVersionType> InterfaceVersion =
            new Dictionary<string, SmppVersionType> { { "3.4", SmppVersionType.Version3_4 }, { "3.3", SmppVersionType.Version3_3 } };

        /// <summary>
        /// Interface version
        /// </summary>
        private Dictionary<string, PDU.DataCodingType> DataCoding =
            new Dictionary<string, PDU.DataCodingType> 
            { 
                { "SMSC Default", PDU.DataCodingType.SMSCDefault },
                { "IA5 Ascii", PDU.DataCodingType.IA5_ASCII },
                { "Octet Unspecified B", PDU.DataCodingType.OctetUnspecifiedB },
                { "Latin 1", PDU.DataCodingType.Latin1 },
                { "Octet Unspecified A", PDU.DataCodingType.OctetUnspecifiedA },
                { "JIS", PDU.DataCodingType.JIS },
                { "Cyrillic", PDU.DataCodingType.Cyrillic },
                { "Latin Hebrew", PDU.DataCodingType.Latin_Hebrew },
                { "UCS2", PDU.DataCodingType.Ucs2 },
                { "Pictogram", PDU.DataCodingType.Pictogram },
                { "Default Flash SMS", PDU.DataCodingType.DefaultFlashSms },
                { "Unicode Flash SMS", PDU.DataCodingType.UnicodeFlashSms },
                { "MusicCodes", PDU.DataCodingType.MusicCodes },
                { "Extended Kanji JIS", PDU.DataCodingType.ExtendedKanjiJIS },
                { "KS C", PDU.DataCodingType.KS_C }
            };


        /// <summary>
        /// Type of number
        /// </summary>
        private Dictionary<string, TonType> TypeOfNumber =
            new Dictionary<string, TonType> 
            { 
                { "Default", TonType.Unknown },
                { "International", TonType.International },
                { "National", TonType.National },
                { "Network Specific", TonType.NetworkSpecific },
                { "Subscriber Number", TonType.SubscriberNumber },
                { "Alphanumeric", TonType.Alphanumeric },
                { "Abbreviated", TonType.Abbreviated }
            };


        /// <summary>
        /// Numbering plan indicator
        /// </summary>
        private Dictionary<string, NpiType> NumberType =
            new Dictionary<string, NpiType> 
            { 
                { "Default", NpiType.Unknown },
                { "ISDN", NpiType.ISDN },
                { "Data", NpiType.Data },
                { "Telex", NpiType.Telex },
                { "Land Mobile", NpiType.LandMobile },
                { "National", NpiType.National },
                { "Private", NpiType.Private },
                { "ERMES", NpiType.ERMES },
                { "Internet", NpiType.Internet }             
            };


        private delegate void SetTextCallback(string text);

        public frmSMPP()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the error.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        private void ShowError(string errorMessage)
        {
            MessageBox.Show(errorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Handles the Load event of the frmSMPP control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void frmSMPP_Load(object sender, EventArgs e)
        {
            cboSystemMode.Items.AddRange(SystemMode.Keys.ToArray());
            cboSystemMode.SelectedIndex = 0;

            cboInterfaceVersion.Items.AddRange(InterfaceVersion.Keys.ToArray());
            cboInterfaceVersion.SelectedIndex = 0;

            cboDataCoding.Items.AddRange(DataCoding.Keys.ToArray());
            cboDataCoding.SelectedIndex = 0;

            cboSourceTon.Items.AddRange(TypeOfNumber.Keys.ToArray());
            cboSourceTon.SelectedIndex = 1;

            cboTon.Items.AddRange(TypeOfNumber.Keys.ToArray());
            cboTon.SelectedIndex = 1;

            cboDestinationTon.Items.AddRange(TypeOfNumber.Keys.ToArray());
            cboDestinationTon.SelectedIndex = 0;

            cboSourceNpi.Items.AddRange(NumberType.Keys.ToArray());
            cboSourceNpi.SelectedIndex = 1;

            cboNpi.Items.AddRange(NumberType.Keys.ToArray());
            cboNpi.SelectedIndex = 1;

            cboDestinationNpi.Items.AddRange(NumberType.Keys.ToArray());
            cboDestinationNpi.SelectedIndex = 0;

        }

        /// <summary>
        /// Handles the Click event of the btnConnect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                btnConnect.Enabled = false;

                // Get the message gateway
                MessageGateway<ISmppGateway, SmppGatewayConfiguration> messageGateway = MessageGateway<ISmppGateway, SmppGatewayConfiguration>.NewInstance();

                // Create the configuration instance
                SmppGatewayConfiguration smppGatewayConfiguration = SmppGatewayConfiguration.NewInstance();

                // Set the port
                try
                {
                    smppGatewayConfiguration.Port = Convert.ToInt16(txtPort.Text);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                    return;
                }

                // Set the bind type
                BindingType bindingType;
                if (SystemMode.TryGetValue(cboSystemMode.Text, out bindingType))
                {
                    smppGatewayConfiguration.BindType = bindingType;
                }

                // Set other connection information
                smppGatewayConfiguration.Host = txtServer.Text;
                smppGatewayConfiguration.Password = txtPassword.Text;
                smppGatewayConfiguration.SystemId = txtSystemId.Text;
                smppGatewayConfiguration.SystemType = txtSystemType.Text;

                if (!string.IsNullOrEmpty(txtAddressRange.Text))
                    smppGatewayConfiguration.AddressRange = txtAddressRange.Text;

                // Set the version
                SmppVersionType smppVersionType;
                if (InterfaceVersion.TryGetValue(cboInterfaceVersion.Text, out smppVersionType))
                {
                    smppGatewayConfiguration.Version = smppVersionType;
                }

                // Server keep alive
                smppGatewayConfiguration.EnquireLinkInterval = Convert.ToInt32(txtServerKeepAlive.Text);

                // Sleep after socket failure
                smppGatewayConfiguration.SleepTimeAfterSocketFailure = Convert.ToInt32(txtSleepAfterSocketFailure.Text);


                // Set Ton type
                TonType tonType;
                if (TypeOfNumber.TryGetValue(cboTon.Text, out tonType))
                {
                    smppGatewayConfiguration.TonType = tonType;
                }

                // Set NPI
                NpiType npiType;
                if (NumberType.TryGetValue(cboNpi.Text, out npiType))
                {
                    smppGatewayConfiguration.NpiType = npiType;
                }

                // Set to verbose
                smppGatewayConfiguration.LogLevel = MessagingToolkit.Core.Log.LogLevel.Verbose;
                smppGatewayConfiguration.LogNameFormat = Log.LogNameFormat.Name;

                // Get the gateway
                smppGateway = messageGateway.Find(smppGatewayConfiguration);

                // Bind error event handler
                smppGateway.OnError += new ErrorEventHandler(smppGateway_OnError);

                // Display the log file  path
                txtLogFile.Text = smppGateway.LogFile;

                // Now we bind
                if (smppGateway.Bind())
                {

                    // Bind the events                
                    smppGateway.OnDeliverSm += new DeliverSmEventHandler(smppGateway_OnDeliverSm);
                    smppGateway.OnAlert += new AlertEventHandler(smppGateway_OnAlert);
                    smppGateway.OnClose += new ClosingEventHandler(smppGateway_OnClose);


                    // You can bind additional events if you want

                    MessageBox.Show("Connected to gateway successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(smppGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                btnConnect.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the OnClose event of the smppGateway control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void smppGateway_OnClose(object source, EventArgs e)
        {
            Output("Connection Close");
            Output("-------------------------------");
            Output("\r\n");
        }

        /// <summary>
        /// Handles the OnError event of the smppGateway control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Smpp.EventObjects.CommonErrorEventArgs"/> instance containing the event data.</param>
        void smppGateway_OnError(object source, CommonErrorEventArgs e)
        {
            Output("Error");
            Output("-------------------------------");
            Output("Message: {0}", e.ThrownException.Message);
            Output("Stack trace: {0}", e.ThrownException.StackTrace);
            Output("\r\n");
        }


        /// <summary>
        /// Handles the OnAlert event of the smppGateway control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Smpp.EventObjects.AlertEventArgs"/> instance containing the event data.</param>
        void smppGateway_OnAlert(object source, AlertEventArgs e)
        {
            Output("AlertPdu");
            Output("-------------------------------");
            Output("Source address: {0}", e.AlertPdu.SourceAddress);
            Output("Command status: {0}", e.AlertPdu.CommandStatus);
            Output("\r\n");
        }

        /// <summary>
        /// Handles the OnDeliverSm event of the smppGateway control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Smpp.EventObjects.DeliverSmEventArgs"/> instance containing the event data.</param>
        void smppGateway_OnDeliverSm(object source, DeliverSmEventArgs e)
        {
            Output("DeliveredSmPdu");
            Output("-------------------------------");
            Output("Source address: {0}, Destination address: {1}, Sequence no: {2}", 
                e.DeliverSmPdu.SourceAddress, e.DeliverSmPdu.DestinationAddress, e.DeliverSmPdu.SequenceNumber);
            if (e.DeliverSmPdu.SmLength > 1)
            {
                Output("Content: {0} ", e.DeliverSmPdu.ShortMessage);
            }
            else
            {
                try
                {
                    Output("Content: {0} ", e.DeliverSmPdu.MessagePayload);
                }
                catch (Exception ex) { }
            }
            Output("\r\n");

            //default a response
            SmppDeliverSmResp pdu = new SmppDeliverSmResp();
            pdu.SequenceNumber = e.DeliverSmPdu.SequenceNumber;
            pdu.CommandStatus = 0;

            smppGateway.SendPdu(pdu);
        }

        /// <summary>
        /// Handles the FormClosing event of the frmSMPP control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void frmSMPP_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (smppGateway != null)
            {
                smppGateway.Unbind();
                smppGateway = SmppGatewayFactory.Default;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnDisconnect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (smppGateway != null)
            {
                smppGateway.Unbind();
                smppGateway = SmppGatewayFactory.Default;

                MessageBox.Show("Disconnected from gateway", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Handles the Click event of the tabMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tabMain_Click(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            if (tc.SelectedTab == tabAbout)
            {
                Assembly assembly = Assembly.GetAssembly(smppGateway.GetType());
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
                if (smppGateway.License.Valid)
                {
                    lblLicense.Text = "Licensed Copy";
                }
                else
                {
                    lblLicense.Text = "Community Copy";
                }

            }
        }

        private void btnSendSms_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtRecipients.Text))
                {
                    ShowError("Recipients must not be empty");
                    return;
                }

                if (string.IsNullOrEmpty(txtMessage.Text))
                {
                    ShowError("Message must not be empty");
                    return;
                }

                if (string.IsNullOrEmpty(txtSourceAddress.Text))
                {
                    ShowError("Source address must not be empty");
                    return;
                }

                btnSendSms.Enabled = false;

                // Check if the message is longer than 160 characters.
                // If YES, break it into chunk of 150 characters each
                List<String> messages;
                if (txtMessage.Text.Length >= 160)
                {
                    messages = Split(txtMessage.Text, 150);
                }
                else
                {
                    messages = new List<String>();
                    messages.Add(txtMessage.Text);
                }

                string[] recipients = txtRecipients.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (recipients.Count() == 1)
                {
                    // Only 1 recipient
                    SmppSubmitSm submitSm = new SmppSubmitSm();
                    submitSm.SourceAddress = txtSourceAddress.Text;

                    // Set the version
                    SmppVersionType smppVersionType;
                    if (InterfaceVersion.TryGetValue(cboInterfaceVersion.Text, out smppVersionType))
                    {
                        submitSm.ProtocolId = smppVersionType;
                    }

                    // Set Ton type
                    TonType tonType;
                    if (TypeOfNumber.TryGetValue(cboSourceTon.Text, out tonType))
                    {
                        submitSm.SourceAddressTon = tonType;
                    }

                    // Set NPI
                    NpiType npiType;
                    if (NumberType.TryGetValue(cboSourceNpi.Text, out npiType))
                    {
                        submitSm.SourceAddressNpi = npiType;
                    }

                    // Set the destination address
                    submitSm.DestinationAddress = recipients[0];

                    // Set Ton type                
                    if (TypeOfNumber.TryGetValue(cboDestinationTon.Text, out tonType))
                    {
                        submitSm.DestinationAddressTon = tonType;
                    }

                    // Set NPI              
                    if (NumberType.TryGetValue(cboDestinationNpi.Text, out npiType))
                    {
                        submitSm.DestinationAddressNpi = npiType;
                    }

                    // Set data coding
                    PDU.DataCodingType dataCoding;
                    if (DataCoding.TryGetValue(cboDataCoding.Text, out dataCoding))
                    {
                        submitSm.DataCoding = dataCoding;
                    }

                   
                    if (chkDeliveryReport.Checked)
                    {
                        submitSm.AlertOnMsgDelivery = 1;
                    }


                    // Set optional parameters
                    //submitSm.SetOptionalParamString(0x1407, "23569");
                    if (chkPayload.Checked)
                    {
                        submitSm.MessagePayload = txtMessage.Text;
                        if (!smppGateway.SendPdu(submitSm))
                        {
                            MessageBox.Show(smppGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        if (messages.Count == 1)
                        {
                            submitSm.ShortMessage = txtMessage.Text;
                            if (!smppGateway.SendPdu(submitSm))
                            {
                                MessageBox.Show(smppGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            submitSm.SarMsgRefNumber = GenerateRefNumber();
                            submitSm.SarTotalSegments = (byte)messages.Count;
                            byte msgCount = 0;
                            foreach (string msg in messages)
                            {
                                submitSm.ShortMessage = msg;
                                submitSm.SarSegmentSeqnum = ++msgCount;
                                if (!smppGateway.SendPdu(submitSm))
                                {
                                    MessageBox.Show(smppGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                           
                        }
                    }
                }
                else
                {
                    // Multiple recipients
                    SmppSubmitMulti submitMulti = new SmppSubmitMulti();

                    // Set Ton type
                    TonType tonType;
                    if (TypeOfNumber.TryGetValue(cboSourceTon.Text, out tonType))
                    {
                        submitMulti.SourceAddressTon = tonType;
                    }

                    // Set NPI
                    NpiType npiType;
                    if (NumberType.TryGetValue(cboSourceNpi.Text, out npiType))
                    {
                        submitMulti.SourceAddressNpi = npiType;
                    }

                    // Set Ton type                
                    if (TypeOfNumber.TryGetValue(cboDestinationTon.Text, out tonType))
                    {

                    }

                    // Set NPI              
                    if (NumberType.TryGetValue(cboDestinationNpi.Text, out npiType))
                    {

                    }

                    DestinationAddress[] destinationAddressList = new DestinationAddress[recipients.Count()];
                    int i = 0;
                    foreach (string recipient in recipients)
                    {
                        DestinationAddress destAddress = new DestinationAddress(tonType, npiType, recipient);
                        destinationAddressList[i++] = destAddress;
                    }
                    submitMulti.DestinationAddresses = destinationAddressList;

                    // Set the version
                    SmppVersionType smppVersionType;
                    if (InterfaceVersion.TryGetValue(cboInterfaceVersion.Text, out smppVersionType))
                    {
                        submitMulti.ProtocolId = smppVersionType;
                    }

                    // Set data coding
                    PDU.DataCodingType dataCoding;
                    if (DataCoding.TryGetValue(cboDataCoding.Text, out dataCoding))
                    {
                        submitMulti.DataCoding = dataCoding;
                    }

                    if (chkDeliveryReport.Checked)
                    {
                        submitMulti.AlertOnMsgDelivery = 1;
                    }


                    if (chkPayload.Checked)
                    {
                        submitMulti.MessagePayload = txtMessage.Text;
                        if (!smppGateway.SendPdu(submitMulti))
                        {
                            MessageBox.Show(smppGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        if (messages.Count == 1)
                        {
                            submitMulti.ShortMessage = txtMessage.Text;
                            if (!smppGateway.SendPdu(submitMulti))
                            {
                                MessageBox.Show(smppGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            submitMulti.SarMsgRefNumber = GenerateRefNumber();
                            submitMulti.SarTotalSegments = (byte)messages.Count;
                            byte msgCount = 0;
                            foreach (string msg in messages)
                            {
                                submitMulti.ShortMessage = msg;
                                submitMulti.SarSegmentSeqnum = ++msgCount;
                                if (!smppGateway.SendPdu(submitMulti))
                                {
                                    MessageBox.Show(smppGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                btnSendSms.Enabled = true;
            }
        }

        /// <summary>
        /// Outputs the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void Output(string text)
        {
            if (this.txtReceivedMessage.InvokeRequired)
            {
                SetTextCallback stc = new SetTextCallback(Output);
                this.Invoke(stc, new object[] { text });
            }
            else
            {
                txtReceivedMessage.AppendText(text);
                txtReceivedMessage.AppendText("\r\n");
            }
        }

        /// <summary>
        /// Outputs the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="args">The args.</param>
        private void Output(string text, params object[] args)
        {
            string msg = string.Format(text, args);
            Output(msg);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.twit88.com");
        }


        public static List<string> Split(string value, int length)
        {
            int strLength = value.Length;
            int strCount = (strLength + length - 1) / length;
            string[] result = new string[strCount];
            for (int i = 0; i < strCount; ++i)
            {
                result[i] = value.Substring(i * length, Math.Min(length, strLength));
                strLength -= length;
            }
            return new List<String>(result);
        }

        public static ushort GenerateRefNumber()
        {
            Random rnd = new Random();
            return (ushort)(rnd.Next(1, 65535)); 
        }
    }
}

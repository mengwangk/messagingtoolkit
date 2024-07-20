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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Reflection;
using System.Drawing.Imaging;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Helper;
using MessagingToolkit.MMS;
using MessagingToolkit.Barcode;

namespace MessagingToolkit.Core.Utilities
{
    /// <summary>
    /// MMS MM1 form
    /// </summary>
    public partial class frmMM1 : Form
    {

        /// <summary>
        /// Mobile gateway interface
        /// </summary>
        private IMobileGateway mobileGateway = MobileGatewayFactory.Default;

        /// <summary>
        /// Mobile gateway configuration
        /// </summary>
        private MobileGatewayConfiguration config = MobileGatewayConfiguration.NewInstance();

        /// <summary>
        /// Port parity lookup
        /// </summary>
        private Dictionary<string, PortParity> Parity =
            new Dictionary<string, PortParity> { { "None", PortParity.None }, { "Odd", PortParity.Odd }, { "Even", PortParity.Even }, { "Mark", PortParity.Mark }, { "Space", PortParity.Space } };

        /// <summary>
        /// Port stop bits lookup
        /// </summary>
        private Dictionary<string, PortStopBits> StopBits =
          new Dictionary<string, PortStopBits> { { "1", PortStopBits.One }, { "1.5", PortStopBits.OnePointFive }, { "2", PortStopBits.Two }, { "None", PortStopBits.None } };

        /// <summary>
        /// Port handshake lookup
        /// </summary>
        private Dictionary<string, PortHandshake> Handshake =
          new Dictionary<string, PortHandshake> { { "None", PortHandshake.None }, { "RequestToSendXOnXOff", PortHandshake.RequestToSendXOnXOff }, { "XOnXOff", PortHandshake.XOnXOff }, { "RequestToSend", PortHandshake.RequestToSend } };



        /// <summary>
        /// Message priority in queue
        /// </summary>
        private Dictionary<string, MessageQueuePriority> QueuePriority =
                          new Dictionary<string, MessageQueuePriority>
                            {
                                {"Low", MessageQueuePriority.Low},
                                {"Normal", MessageQueuePriority.Normal},
                                {"High", MessageQueuePriority.High}
                            };


        /// <summary>
        /// MMS providers
        /// </summary>
        private SortedDictionary<string, List<string>> mmsProviders;

        /// <summary>
        /// MMS provider file extension
        /// </summary>
        private const string ProviderFileExtension = ".mm1";

        /// <summary>
        /// Path which points to the embedded resource in the assembly
        /// </summary>
        private const string MMSProviderPath = "MMSProviders";



        /// <summary>
        /// Content types
        /// </summary>
        private Dictionary<string, string> ContentTypeMapping =
             new Dictionary<string, string> {
                                             { "Plain Text", MmsConstants.ContentTypeTextPlain },
                                             { "HTML", MmsConstants.ContentTypeTextHtml },
                                             { "WML", MmsConstants.ContentTypeTextWml },
                                             { "GIF", MmsConstants.ContentTypeImageGif },
                                             { "JPEG", MmsConstants.ContentTypeImageJpeg },
                                             { "TIFF", MmsConstants.ContentTypeImageTiff },
                                             { "PNG", MmsConstants.ContentTypeImagePng },
                                             { "WBMP", MmsConstants.ContentTypeImageWbmp },
                                             { "AMR", MmsConstants.ContentTypeAudioAmr },
                                             { "SMIL", MmsConstants.ContentTypeApplicationSmil },
                                             { "IMELODY", MmsConstants.ContentTypeaAudioIMelody },
                                             { "MIDI", MmsConstants.ContentTypeAudioMidi },
                                             { "vCalendar", MmsConstants.ContentTypevCalendar},
                                             { "vCard", MmsConstants.ContentTypevCard },
                                             { "MPEG", MmsConstants.ContentTypeMpeg },
                                             { "AVI", MmsConstants.ContentTypeAvi },
                                             { "Quicktime", MmsConstants.ContentTypeQuicktime },
                                             { "WAV", MmsConstants.ContentTypeWav },
                                             { "Audio Basic", MmsConstants.ContentTypeAu },
                                             { "AIFF", MmsConstants.ContentTypeAiff },
                                             { "MP3", MmsConstants.ContentTypeMp3 },
                                             { "OGG", MmsConstants.ContentTypeOgg }
                                            };


        /// <summary>
        /// Text content type mapping
        /// </summary>
        private Dictionary<string, ContentType> TextContentTypeMapping =
             new Dictionary<string, ContentType> {
                                             { ".txt", ContentType.TextPlain },
                                             { ".html", ContentType.TextHtml },
                                             { ".wml", ContentType.TextWml},
                                             { ".vcs", ContentType.vCalendar},
                                             { ".vcf", ContentType.vCard}
                                            };


        /// <summary>
        /// Image content type mapping
        /// </summary>
        private Dictionary<string, ContentType> ImageContentTypeMapping =
             new Dictionary<string, ContentType> {
                                             { ".gif", ContentType.ImageGif },
                                             { ".jpg", ContentType.ImageJpeg },
                                             { ".tiff", ContentType.ImageTiff },
                                             { ".png", ContentType.ImagePng },
                                             { ".wbmp", ContentType.ImageWbmp },
                                            };

        /// <summary>
        /// Audio content type mapping
        /// </summary>
        private Dictionary<string, ContentType> AudioContentTypeMapping =
             new Dictionary<string, ContentType> {
                                             { ".amr", ContentType.AudioAmr },
                                             { ".imelody", ContentType.AudioIMelody},
                                             { ".mid", ContentType.AudioMidi},
                                             { ".wav", ContentType.AudioWav},
                                             { ".ogg", ContentType.AudioOgg},
                                             { ".au", ContentType.AudioAu},
                                             { ".aiff", ContentType.AudioAiff},
                                             { ".mp3", ContentType.AudioMp3}
                                            };

        /// <summary>
        /// Video content type mapping
        /// </summary>
        private Dictionary<string, ContentType> VideoContentTypeMapping =
             new Dictionary<string, ContentType> {
                                             { ".mpg", ContentType.VideoMpg },
                                             { ".qt", ContentType.VideoQuicktime},
                                             { ".avi", ContentType.VideoAvi}
                                            };

        /// <summary>
        /// List of message contents
        /// </summary>
        private List<MessageContent> mmsContents = new List<MessageContent>(3);


        /// <summary>
        /// Initializes a new instance of the <see cref="frmMM1"/> class.
        /// </summary>
        public frmMM1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the frmMM1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void frmMM1_Load(object sender, EventArgs e)
        {
            // Add the port
            string[] portNames = SerialPort.GetPortNames();
            var sortedList = portNames.OrderBy(port => Convert.ToInt32(port.Replace("COM", string.Empty)));
            foreach (string port in sortedList)
            {
                if (!cboPort.Items.Contains(port))
                    cboPort.Items.Add(port);
            }
            if (cboPort.Items.Count > 0)
            {
                cboPort.SelectedIndex = 0;
            }

            // Add baud rate
            foreach (string baudRate in Enum.GetNames(typeof(PortBaudRate)))
            {
                cboBaudRate.Items.Add((int)Enum.Parse(typeof(PortBaudRate), baudRate));
            }
            cboBaudRate.Text = "115200";

            // Add data bits
            foreach (string dataBit in Enum.GetNames(typeof(PortDataBits)))
            {
                cboDataBits.Items.Add((int)Enum.Parse(typeof(PortDataBits), dataBit));
            }
            cboDataBits.Text = "8";

            // Add parity            
            cboParity.Items.AddRange(Parity.Keys.ToArray());
            cboParity.SelectedIndex = 0;

            // Add stop bits         
            cboStopBits.Items.AddRange(StopBits.Keys.ToArray());
            cboStopBits.SelectedIndex = 0;

            // Add handshake          
            cboHandshake.Items.AddRange(Handshake.Keys.ToArray());
            cboHandshake.SelectedIndex = 0;

            // Queue priority
            cboQueuePriority.Items.AddRange(QueuePriority.Keys.ToArray());
            cboQueuePriority.SelectedIndex = 1;

            // Populate MMS providers
            PopulateMMSProviders();
            InitializeMMS();

            // Add available devices
            //cboDevice.Items.AddRange(GatewayHelper.GetActiveDevices().ToArray());
            // Use this if you want to list all available devices
            cboDevice.Items.AddRange(GatewayHelper.GetAllDevices().ToArray());
            if (cboDevice.Items.Count > 0)
                cboDevice.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates the MMS providers from embedded resources
        /// </summary>
        private void PopulateMMSProviders()
        {
            string assemblyName = typeof(frmMM1).Assembly.GetName().Name;
            Assembly assembly = Assembly.Load(assemblyName);
            string[] resources = assembly.GetManifestResourceNames();


            mmsProviders = new SortedDictionary<string, List<string>>();
            foreach (string resource in resources)
            {
                if (resource.EndsWith(ProviderFileExtension))
                {
                    // This is a MMS provider
                    string provider = resource.Replace(ProviderFileExtension, string.Empty);
                    string[] split = provider.Split('.');
                    string country = split[split.Length - 2];
                    string providerName = split[split.Length - 1];

                    List<string> providerList;
                    if (mmsProviders.TryGetValue(country, out providerList))
                    {
                        providerList.Add(providerName);
                    }
                    else
                    {
                        providerList = new List<string>();
                        providerList.Add(providerName);
                        mmsProviders.Add(country, providerList);
                    }
                }
            }
            if (mmsProviders.Count() > 0)
            {
                SortedDictionary<string, List<string>>.KeyCollection keys = mmsProviders.Keys;
                cboCountry.Items.AddRange(keys.ToArray());
            }

        }


        /// <summary>
        /// Extracts an embedded file out of a given assembly.
        /// </summary>
        /// <param name="fileName">The name of the file to extract.</param>
        /// <returns>A stream containing the file data.</returns>
        public Stream GetEmbeddedFile(string fileName)
        {
            string assemblyName = typeof(frmMM1).Assembly.GetName().Name;
            try
            {
                Assembly a = Assembly.Load(assemblyName);
                Stream str = a.GetManifestResourceStream(assemblyName + "." + fileName);

                if (str == null)
                    throw new Exception("Could not locate embedded resource '" + fileName + "' in assembly '" + assemblyName + "'");
                return str;
            }
            catch (Exception e)
            {
                throw new Exception(assemblyName + ": " + e.Message);
            }
        }

        private void cboCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            string country = cboCountry.Items[cboCountry.SelectedIndex].ToString();
            if (!string.IsNullOrEmpty(country))
            {
                // Get the providers
                List<string> providers = mmsProviders[country];
                providers.Sort();
                cboOperator.Items.Clear();
                cboOperator.Items.AddRange(providers.ToArray());
                cboParity.SelectedIndex = 0;
            }
        }

        private void cboOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            string provider = cboOperator.Items[cboOperator.SelectedIndex].ToString();
            string country = cboCountry.Items[cboCountry.SelectedIndex].ToString();
            if (!string.IsNullOrEmpty(provider))
            {
                string resourcePath = MMSProviderPath + "." + country + "." + provider + ProviderFileExtension;
                Stream stream = GetEmbeddedFile(resourcePath);
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                string[] lines = content.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                string[] columns = lines[0].Split('=');
                if (columns.Length > 1) txtMMSC.Text = columns[1];

                columns = lines[1].Split('=');
                if (columns.Length > 1) txtWAPGateway.Text = columns[1];

                columns = lines[2].Split('=');
                if (columns.Length > 1) txtAPN.Text = columns[1];

                columns = lines[3].Split('=');
                if (columns.Length > 1) txtAPNAccount.Text = columns[1];

                columns = lines[4].Split('=');
                if (columns.Length > 1) txtAPNPassword.Text = columns[1];

            }
        }

        private void cboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            config.DeviceName = cboDevice.Items[cboDevice.SelectedIndex].ToString();
            if (GatewayHelper.GetDeviceConfiguration(config))
            {
                cboPort.Text = config.PortName;
                cboBaudRate.Text = Convert.ToString((int)Enum.Parse(typeof(PortBaudRate), Enum.GetName(typeof(PortBaudRate), config.BaudRate)));
            }
            else
            {
                MessageBox.Show("Unable to get port settings. Please configure them manually", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMMSC.Text) || string.IsNullOrEmpty(txtWAPGateway.Text) || string.IsNullOrEmpty(txtAPN.Text))
            {
                MessageBox.Show("MMSC, WAP gateway and APN must be configured", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            config.PortName = cboPort.Text;
            config.BaudRate = (PortBaudRate)Enum.Parse(typeof(PortBaudRate), cboBaudRate.Text);
            config.DataBits = (PortDataBits)Enum.Parse(typeof(PortDataBits), cboDataBits.Text);
            config.ProviderAPN = txtAPN.Text;
            config.ProviderAPNAccount = txtAPNAccount.Text;
            config.ProviderAPNPassword = txtAPNPassword.Text;
            config.ProviderMMSC = txtMMSC.Text;
            config.ProviderWAPGateway = txtWAPGateway.Text;
            config.DataCompressionControl = chkDataCompression.Checked;
            config.HeaderCompressionControl = chkHeaderCompression.Checked;
            config.UseHttpTransportForMMS = chkHttpTransport.Checked;       // Default to true
            config.SupportHttpCharsetHeader = chkHttpCharsetHeader.Checked; // Default to false

            if (!string.IsNullOrEmpty(txtPin.Text))
            {
                config.Pin = txtPin.Text;
            }
            if (!string.IsNullOrEmpty(txtModelConfig.Text))
            {
                config.Model = txtModelConfig.Text;
            }

            PortParity parity;
            if (Parity.TryGetValue(cboParity.Text, out parity))
            {
                config.Parity = parity;
            }

            PortStopBits stopBits;
            if (StopBits.TryGetValue(cboStopBits.Text, out stopBits))
            {
                config.StopBits = stopBits;
            }

            PortHandshake handshake;
            if (Handshake.TryGetValue(cboHandshake.Text, out handshake))
            {
                config.Handshake = handshake;
            }

            // Default not to check for the PIN
            config.DisablePinCheck = chkDisablePinCheck.Checked;

            // Default to verbose by default
            config.LogLevel = LogLevel.Verbose;

            // Send retries
            if (updSendRetries.Value > 0)
            {
                config.SendRetries = Convert.ToInt32(updSendRetries.Value);
            }

            // Send retry interval
            if (updSendWaitInterval.Value > 0)
            {
                config.SendWaitInterval = Convert.ToInt32(updSendWaitInterval.Value);
            }

            // Set a different log file prefix and path
            // config.LogFile = "mm1";
            //config.LogLocation = @"c:\temp";

            // Set the log file name without the date
            config.LogNameFormat = LogNameFormat.Name;

            // Create the gateway for mobile
            MessageGateway<IMobileGateway, MobileGatewayConfiguration> messageGateway =
                MessageGateway<IMobileGateway, MobileGatewayConfiguration>.NewInstance();
            try
            {
                btnConnect.Enabled = false;
                mobileGateway = messageGateway.Find(config);
                if (mobileGateway == null)
                {
                    ShowError("Error connecting to gateway. Check the log file");
                    return;
                }
                //mobileGateway.LogLevel = LogLevel.Verbose;
                updSendRetries.Value = mobileGateway.Configuration.SendRetries;
                updSendWaitInterval.Value = mobileGateway.Configuration.SendWaitInterval;
                txtLogFile.Text = mobileGateway.LogFile;

                // Initialize the data connection
                if (mobileGateway.InitializeDataConnection())
                {
                    // Attach the events
                    mobileGateway.MessageSendingFailed += OnMessageFailed;
                    mobileGateway.MessageSent += OnMessageSent;

                    MessageBox.Show("Gateway is initialized successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ShowError("Unable to initialize gateway. Error is \n" + mobileGateway.LastError.Message);
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
        /// Handles the Click event of the tabMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tabMain_Click(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            if (tc.SelectedTab == tabAbout)
            {
                Assembly assembly = Assembly.GetAssembly(mobileGateway.GetType());
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
                if (mobileGateway.License.Valid)
                {
                    lblLicense.Text = "Licensed Copy";
                }
                else
                {
                    lblLicense.Text = "Community Copy";
                }

            }
        }

        /// <summary>
        /// Initializes the MMS.
        /// </summary>
        private void InitializeMMS()
        {
            txtTransactionId.Text = "1234567890";
            txtPresentationId.Text = "<0000>";

            // Add parity 
            cboContentType.Items.Clear();
            cboContentType.Items.AddRange(ContentTypeMapping.Keys.ToArray());
            cboContentType.SelectedIndex = 0;

            lstContent.Items.Clear();

            mmsContents.Clear();

            txtFrom.Text = string.Empty;
            txtTo.Text = string.Empty;
            txtSubject.Text = string.Empty;
            txtContentId.Text = string.Empty;
            txtContentFileName.Text = string.Empty;
            chkDeliveryReport.Checked = false;
            chkReadReceipt.Checked = false;
        }

        /// <summary>
        /// Handles the Click event of the btnSendMMS control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSendMMS_Click(object sender, EventArgs e)
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
            Mms mms = Mms.NewInstance(txtSubject.Text, txtFrom.Text);

            // If it is SMIL based
            // mms.MultipartRelatedType = MultimediaMessageConstants.ContentTypeApplicationSmil;

            // Multipart mixed
            mms.MultipartRelatedType = MultimediaMessageConstants.ContentTypeApplicationMultipartMixed;

            mms.PresentationId = txtPresentationId.Text;
            mms.TransactionId = txtTransactionId.Text;
            mms.AddToAddress(txtTo.Text, MmsAddressType.PhoneNumber);
            mms.DeliveryReport = chkDeliveryReport.Checked;
            mms.ReadReply = chkReadReceipt.Checked;

            // Add the contents
            AddContents(mms);

            try
            {
                btnSendMMS.Enabled = false;

                if (chkSendToQueue.Checked)
                {
                    MessageQueuePriority priority;
                    if (QueuePriority.TryGetValue(cboQueuePriority.Text, out priority))
                    {
                        mms.QueuePriority = priority;

                        // You can also set the MMS message priority here
                        if (priority == MessageQueuePriority.High)
                            mms.Priority = MmsConstants.PriorityHigh;
                        else if (priority == MessageQueuePriority.Normal)
                            mms.Priority = MmsConstants.PriorityNormal;
                        else if (priority == MessageQueuePriority.Low)
                            mms.Priority = MmsConstants.PriorityLow;

                    }

                    if (mobileGateway.SendToQueue(mms))
                    {
                        MessageBox.Show("Message is queued successfully for " + mms.To[0].GetAddress(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (mobileGateway.Send(mms))
                    {
                        MessageBox.Show("Message is sent successfully. Message id is " + mms.MessageId, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                ShowError(ex.StackTrace);
            }
            finally
            {
                btnSendMMS.Enabled = true;
            }

        }

        /// <summary>
        /// Adds the contents.
        /// </summary>
        /// <param name="message">The message.</param>
        private void AddContents(MultimediaMessage message)
        {
            foreach (MessageContent content in mmsContents)
            {
                MultimediaMessageContent multimediaMessageContent = new MultimediaMessageContent();
                multimediaMessageContent.SetContent(content.FileName);
                multimediaMessageContent.ContentId = content.ContentId;  //If "<>" are not used with this method, the result is Content-Location
                multimediaMessageContent.Type = content.ContentType;
                message.AddContent(multimediaMessageContent);
            }
        }

        private void btnResetMMS_Click(object sender, EventArgs e)
        {
            InitializeMMS();
        }

        private void ShowError(string errorMessage)
        {
            MessageBox.Show(errorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void frmMM1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mobileGateway != null)
            {
                mobileGateway.Disconnect();
                mobileGateway.Dispose();
                mobileGateway = null;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (mobileGateway != null)
                {
                    btnDisconnect.Enabled = false;
                    if (mobileGateway.Disconnect())
                    {
                        // Added July 28th 2015
                        mobileGateway.Dispose();
                        mobileGateway = null;
                        mobileGateway = MobileGatewayFactory.Default;
                        MessageBox.Show("Gateway is disconnected successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        ShowError(mobileGateway.LastError.Message);
                    }
                }
            }
            finally
            {
                btnDisconnect.Enabled = true;
            }
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
            if (ContentTypeMapping.TryGetValue(cboContentType.Text, out contentType))
            {
                MessageContent content = new MessageContent(contentType, txtContentId.Text.Trim(), txtContentFileName.Text.Trim());
                mmsContents.Add(content);
                lstContent.Items.Add(cboContentType.Text + "| " + txtContentId.Text + "| " + txtContentFileName.Text);
            }
        }

        private void btnBrowseContent_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.FileName = string.Empty;
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                txtContentFileName.Text = fileName;
            }
        }

        private void btnSaveMMS_Click(object sender, EventArgs e)
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


            try
            {
                // Set the headers
                Mms mms = Mms.NewInstance(txtSubject.Text, txtFrom.Text);
                mms.PresentationId = txtPresentationId.Text;
                mms.TransactionId = txtTransactionId.Text;
                mms.AddToAddress(txtTo.Text, MmsAddressType.PhoneNumber);
                mms.DeliveryReport = chkDeliveryReport.Checked;
                mms.ReadReply = chkReadReceipt.Checked;

                // Add the contents
                AddContents(mms);

                saveFileDialog1.Filter = "MMS File (*.mms)|*.mms";
                saveFileDialog1.FileName = string.Empty;
                DialogResult dialogResult = saveFileDialog1.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string saveFileName = saveFileDialog1.FileName;

                    // Save MMS file
                    if (mms.SaveToFile(saveFileName))
                    {

                        MessageBox.Show("File is saved successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        ShowError("Error saving file " + saveFileName + ": " + mms.LastError.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                ShowError(ex.StackTrace);
            }
        }

        private void btnBrowserMMSFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.FileName = string.Empty;
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                txtMMSFile.Text = fileName;
            }
        }

        private void btnSendMMSFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMMSFile.Text))
            {
                ShowError("MMS file cannot be empty");
                return;
            }

            try
            {
                btnSendMMSFile.Enabled = false;

                Mms mms = Mms.LoadFromFile(txtMMSFile.Text);
                if (chkSendToQueue.Checked)
                {
                    MessageQueuePriority priority;
                    if (QueuePriority.TryGetValue(cboQueuePriority.Text, out priority))
                    {
                        mms.QueuePriority = priority;

                        // You can also set the MMS message priority here
                        if (priority == MessageQueuePriority.High)
                            mms.Priority = MmsConstants.PriorityHigh;
                        else if (priority == MessageQueuePriority.Normal)
                            mms.Priority = MmsConstants.PriorityNormal;
                        else if (priority == MessageQueuePriority.Low)
                            mms.Priority = MmsConstants.PriorityLow;

                    }

                    if (mobileGateway.SendToQueue(mms))
                    {
                        MessageBox.Show("Message is queued successfully for " + mms.To[0].GetAddress(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (mobileGateway.Send(mms))
                    {
                        MessageBox.Show("Message is sent successfully. Message id is " + mms.MessageId, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                ShowError(ex.StackTrace);
            }
            finally
            {
                btnSendMMSFile.Enabled = true;
            }

        }

        private void btnRefreshStatistics_Click(object sender, EventArgs e)
        {
            try
            {
                btnRefreshStatistics.Enabled = false;
                txtIncomingMessage.Text = Convert.ToString(mobileGateway.Statistics.IncomingMms);
                txtOutgoingMessage.Text = Convert.ToString(mobileGateway.Statistics.OutgoingMms);
            }
            finally
            {
                btnRefreshStatistics.Enabled = true;
            }
        }

        private void btnRefreshQueue_Click(object sender, EventArgs e)
        {
            lblMessageQueueCount.Text = "Messages in Queue: " + mobileGateway.GetQueueCount();
        }

        private void btnClearQueue_Click(object sender, EventArgs e)
        {
            mobileGateway.ClearQueue();
        }

        private void chkEnableQueue_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableQueue.Checked)
            {
                MessageBox.Show("Messages will be queued and sent out", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("Messages will be queued but NOT sent out", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            mobileGateway.IsMessageQueueEnabled = chkEnableQueue.Checked;
        }

        private void chkSendToQueue_CheckedChanged(object sender, EventArgs e)
        {
            cboQueuePriority.Enabled = chkSendToQueue.Checked;
        }

        private void OnMessageSent(object sender, MessageEventArgs e)
        {
            Mms mms = (Mms)e.Message;
            MessageBox.Show("Message is sent successfully to " + mms.To[0].GetAddress() + ". Message id is  " + mms.MessageId, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnMessageFailed(object sender, MessageErrorEventArgs e)
        {
            Mms mms = (Mms)e.Message;
            MessageBox.Show("Failed to send message to " + mms.To[0].GetAddress(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnBrowse1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.FileName = string.Empty;
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                txtAttachment1.Text = fileName;
            }
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.FileName = string.Empty;
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                txtAttachment2.Text = fileName;
            }
        }

        private void btnSendMMSSlide_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMMSSlideFrom.Text))
            {
                ShowError("From cannot be empty");
                return;
            }
            if (string.IsNullOrEmpty(txtMMSSlideTo.Text))
            {
                ShowError("To cannot be empty");
                return;
            }
            if (string.IsNullOrEmpty(txtMMSSlideSubject.Text))
            {
                ShowError("Subject cannot be empty");
                return;
            }

            try
            {
                Mms mms = Mms.NewInstance(txtMMSSlideSubject.Text, txtMMSSlideFrom.Text);
                mms.AddToAddress(txtMMSSlideTo.Text, MmsAddressType.PhoneNumber);
                mms.DeliveryReport = chkMMSSlideDeliveryReport.Checked;
                mms.ReadReply = chkMMSSlideReadReceipt.Checked;

                // Add the contents
                if (!string.IsNullOrEmpty(txtBody1.Text) || !string.IsNullOrEmpty(txtAttachment1.Text))
                {
                    MmsSlide slide1 = MmsSlide.NewInstance();
                    if (!string.IsNullOrEmpty(txtBody1.Text))
                    {
                        slide1.AddText(txtBody1.Text);
                    }
                    if (!string.IsNullOrEmpty(txtAttachment1.Text))
                    {
                        ContentType contentType;

                        // Derive the content type based on file extension
                        string fileExtension = System.IO.Path.GetExtension(txtAttachment1.Text).ToLower();
                        if (TextContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide1.AddAttachment(txtAttachment1.Text, AttachmentType.Text, contentType);
                        }
                        else if (ImageContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide1.AddAttachment(txtAttachment1.Text, AttachmentType.Image, contentType);
                        }
                        else if (AudioContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide1.AddAttachment(txtAttachment1.Text, AttachmentType.Audio, contentType);
                        }
                        else if (VideoContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide1.AddAttachment(txtAttachment1.Text, AttachmentType.Video, contentType);
                        }
                        else
                        {
                            ShowError("The file " + txtAttachment1.Text + " is not a valid MMS content file");
                            return;
                        }

                    }
                    mms.AddSlide(slide1);
                }


                // Add the contents
                if (!string.IsNullOrEmpty(txtBody2.Text) || !string.IsNullOrEmpty(txtAttachment2.Text))
                {
                    MmsSlide slide2 = MmsSlide.NewInstance();
                    if (!string.IsNullOrEmpty(txtBody2.Text))
                    {
                        slide2.AddText(txtBody2.Text);
                    }
                    if (!string.IsNullOrEmpty(txtAttachment2.Text))
                    {
                        ContentType contentType;

                        // Derive the content type based on file extension
                        string fileExtension = System.IO.Path.GetExtension(txtAttachment2.Text).ToLower();
                        if (TextContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide2.AddAttachment(txtAttachment2.Text, AttachmentType.Text, contentType);
                        }
                        else if (ImageContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide2.AddAttachment(txtAttachment2.Text, AttachmentType.Image, contentType);
                        }
                        else if (AudioContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide2.AddAttachment(txtAttachment2.Text, AttachmentType.Audio, contentType);
                        }
                        else if (VideoContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide2.AddAttachment(txtAttachment2.Text, AttachmentType.Video, contentType);
                        }
                        else
                        {
                            ShowError("The file " + txtAttachment2.Text + " is not a valid MMS content file");
                            return;
                        }
                    }
                    mms.AddSlide(slide2);
                }


                // If you want to save the MMS to a file, do the following
                // mms.SaveToFile("filename");

                btnSendMMSSlide.Enabled = false;

                if (chkSendToQueue.Checked)
                {
                    MessageQueuePriority priority;
                    if (QueuePriority.TryGetValue(cboQueuePriority.Text, out priority))
                    {
                        mms.QueuePriority = priority;

                        // You can also set the MMS message priority here
                        if (priority == MessageQueuePriority.High)
                            mms.Priority = MmsConstants.PriorityHigh;
                        else if (priority == MessageQueuePriority.Normal)
                            mms.Priority = MmsConstants.PriorityNormal;
                        else if (priority == MessageQueuePriority.Low)
                            mms.Priority = MmsConstants.PriorityLow;

                    }

                    if (mobileGateway.SendToQueue(mms))
                    {
                        MessageBox.Show("Message is queued successfully for " + mms.To[0].GetAddress(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    if (mobileGateway.Send(mms))
                    {
                        MessageBox.Show("Message is sent successfully. Message id is " + mms.MessageId, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                ShowError(ex.StackTrace);
            }
            finally
            {
                btnSendMMSSlide.Enabled = true;
            }


        }

        /// <summary>
        /// Handles the Click event of the btnSaveMMSSlide control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSaveMMSSlide_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMMSSlideFrom.Text))
            {
                ShowError("From cannot be empty");
                return;
            }
            if (string.IsNullOrEmpty(txtMMSSlideTo.Text))
            {
                ShowError("To cannot be empty");
                return;
            }
            if (string.IsNullOrEmpty(txtMMSSlideSubject.Text))
            {
                ShowError("Subject cannot be empty");
                return;
            }

            try
            {
                Mms mms = Mms.NewInstance(txtMMSSlideSubject.Text, txtMMSSlideFrom.Text);
                mms.AddToAddress(txtMMSSlideTo.Text, MmsAddressType.PhoneNumber);
                mms.DeliveryReport = chkMMSSlideDeliveryReport.Checked;
                mms.ReadReply = chkMMSSlideReadReceipt.Checked;

                // Add the contents
                if (!string.IsNullOrEmpty(txtBody1.Text) || !string.IsNullOrEmpty(txtAttachment1.Text))
                {
                    MmsSlide slide1 = MmsSlide.NewInstance();
                    if (!string.IsNullOrEmpty(txtBody1.Text))
                    {
                        slide1.AddText(txtBody1.Text);
                    }
                    if (!string.IsNullOrEmpty(txtAttachment1.Text))
                    {
                        ContentType contentType;

                        // Derive the content type based on file extension
                        string fileExtension = System.IO.Path.GetExtension(txtAttachment1.Text).ToLower();
                        if (TextContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide1.AddAttachment(txtAttachment1.Text, AttachmentType.Text, contentType);
                        }
                        else if (ImageContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide1.AddAttachment(txtAttachment1.Text, AttachmentType.Image, contentType);
                        }
                        else if (AudioContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide1.AddAttachment(txtAttachment1.Text, AttachmentType.Audio, contentType);
                        }
                        else if (VideoContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide1.AddAttachment(txtAttachment1.Text, AttachmentType.Video, contentType);
                        }
                        else
                        {
                            ShowError("The file " + txtAttachment1.Text + " is not a valid MMS content file");
                            return;
                        }

                    }
                    mms.AddSlide(slide1);
                }


                // Add the contents
                if (!string.IsNullOrEmpty(txtBody2.Text) || !string.IsNullOrEmpty(txtAttachment2.Text))
                {
                    MmsSlide slide2 = MmsSlide.NewInstance();
                    if (!string.IsNullOrEmpty(txtBody2.Text))
                    {
                        slide2.AddText(txtBody2.Text);
                    }
                    if (!string.IsNullOrEmpty(txtAttachment2.Text))
                    {
                        ContentType contentType;

                        // Derive the content type based on file extension
                        string fileExtension = System.IO.Path.GetExtension(txtAttachment2.Text).ToLower();
                        if (TextContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide2.AddAttachment(txtAttachment2.Text, AttachmentType.Text, contentType);
                        }
                        else if (ImageContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide2.AddAttachment(txtAttachment2.Text, AttachmentType.Image, contentType);
                        }
                        else if (AudioContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide2.AddAttachment(txtAttachment2.Text, AttachmentType.Audio, contentType);
                        }
                        else if (VideoContentTypeMapping.TryGetValue(fileExtension, out contentType))
                        {
                            slide2.AddAttachment(txtAttachment2.Text, AttachmentType.Video, contentType);
                        }
                        else
                        {
                            ShowError("The file " + txtAttachment2.Text + " is not a valid MMS content file");
                            return;
                        }
                    }
                    mms.AddSlide(slide2);
                }

                saveFileDialog1.Filter = "MMS File (*.mms)|*.mms";
                saveFileDialog1.FileName = string.Empty;
                DialogResult dialogResult = saveFileDialog1.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string saveFileName = saveFileDialog1.FileName;

                    // Save MMS file
                    if (mms.SaveToFile(saveFileName))
                    {
                        MessageBox.Show("File is saved successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        ShowError("Error saving file " + saveFileName + ": " + mms.LastError.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                ShowError(ex.StackTrace);
            }
        }

        private void btnResetMMSSlide_Click(object sender, EventArgs e)
        {
            txtMMSSlideFrom.Text = string.Empty;
            txtMMSSlideTo.Text = string.Empty;
            txtMMSSlideSubject.Text = string.Empty;
            chkMMSSlideDeliveryReport.Checked = false;
            chkMMSSlideReadReceipt.Checked = false;
            txtBody1.Text = "Hello World !!!";
            txtAttachment1.Text = string.Empty;
            txtBody2.Text = string.Empty;
            txtAttachment2.Text = string.Empty;
        }

        /// <summary>
        /// Handles the Click event of the btnRetrieveMMSNotification control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRetrieveMMSNotification_Click(object sender, EventArgs e)
        {
            try
            {
                btnRetrieveMMSNotification.Enabled = false;

                if (radPhone.Checked)
                {
                    mobileGateway.MessageStorage = MessageStorage.Phone;
                }

                if (radSim.Checked)
                {
                    mobileGateway.MessageStorage = MessageStorage.Sim;
                }

                List<MessageInformation> mmsNotifications = null;
                if (radStatusNew.Checked)
                    // If you want to retrieve new MMS notification, then use
                    mmsNotifications = mobileGateway.GetNotifications(NotificationType.Mms, NotificationStatus.New);
                else
                    // Retrieve all MMS notifications
                    mmsNotifications = mobileGateway.GetNotifications(NotificationType.Mms, NotificationStatus.None);

                dgdMMSNotifications.DataSource = mmsNotifications;

                if (mmsNotifications.Count > 0)
                {
                    lblStatusRetrieveMMS.Text = "Retrieving MMS. Please wait....";

                    // Folder to save the received MMS
                    string saveFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    saveFolder += System.IO.Path.DirectorySeparatorChar + "Received MMS" + System.IO.Path.DirectorySeparatorChar;

                    if (!System.IO.Directory.Exists(saveFolder))
                    {
                        System.IO.Directory.CreateDirectory(saveFolder);
                    }

                    foreach (MessageInformation messageInformation in mmsNotifications)
                    {
                        Mms mms = null;
                        if (mobileGateway.GetMms(messageInformation, ref mms))
                        {
                            string fullPathToFile = saveFolder + mms.MessageId + ".mms";
                            if (mms.SaveToFile(fullPathToFile))
                            {
                                lblStatusRetrieveMMS.Text = "MMS with message id " + mms.MessageId + " is retrieved and save as " + fullPathToFile;
                            }
                            else
                            {
                                ShowError("Error saving file " + fullPathToFile + ": " + mms.LastError.Message);
                            }
                        }
                    }
                    lblStatusRetrieveMMS.Text = "All MMS are saved under " + saveFolder;
                }
            }
            finally
            {
                btnRetrieveMMSNotification.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnEncode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnEncode_Click(object sender, EventArgs e)
        {
            try
            {
                BarcodeEncoder barcodeEncoder = new BarcodeEncoder();
                barcodeEncoder.Width = (int)numWidth.Value;
                barcodeEncoder.Height = (int)numHeight.Value;
                picEncode.Image = barcodeEncoder.Encode(BarcodeFormat.QRCode, txtBarcodeData.Text);

            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bm = (Bitmap)picEncode.Image;
                if (bm != null)
                {
                    SaveFileDialog sdlg = new SaveFileDialog();
                    sdlg.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
                    if (sdlg.ShowDialog() == DialogResult.OK)
                    {
                        bm.Save(sdlg.FileName, ImageFormat.Png);
                        MessageBox.Show("File is saved successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnApplyProfile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnApplyProfile_Click(object sender, EventArgs e)
        {
            // Setting the user agent and profile
            // For certain MMSC, to send or receive large MMS, you may need to set these values
            mobileGateway.Configuration.UserAgent = txtUserAgent.Text;
            mobileGateway.Configuration.XWAPProfile = txtProfile.Text;
            MessageBox.Show("Profile settings are configured and will be sent as part of the headers", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.twit88.com");
        }
    }
}

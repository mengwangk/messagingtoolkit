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
using System.Threading;

using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Helper;

using MessagingToolkit.Pdu;
using MessagingToolkit.Pdu.Ie;
using MessagingToolkit.Pdu.WapPush;


namespace MessagingToolkit.Core.Utilities
{
    /// <summary>
    /// Main form
    /// </summary>
    public partial class frmSMS : Form
    {

        /// <summary>
        /// Mobile gateway interface
        /// </summary>
        private IMobileGateway mobileGateway = MobileGatewayFactory.Default;

        /// <summary>
        /// Custom date time format
        /// </summary>
        private static string CustomDateTimeFormat = "dd MMM yyyy, hh:mm:ss tt";

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
        /// Message type lookup
        /// </summary>
        private Dictionary<string, MessageStatusType> MessageType =
            new Dictionary<string, MessageStatusType> { 
                                                      { "Received Unread Message", MessageStatusType.ReceivedUnreadMessages},
                                                      { "Received Read Message", MessageStatusType.ReceivedReadMessages},
                                                      { "Stored Unsent Message", MessageStatusType.StoredUnsentMessages},
                                                      { "Stored Sent Message", MessageStatusType.StoredSentMessages},
                                                      { "All Message", MessageStatusType.AllMessages}
                                                      };


        /// <summary>
        /// Message encoding
        /// </summary>
        private Dictionary<string, MessageDataCodingScheme> MessageEncoding =
            new Dictionary<string, MessageDataCodingScheme>
            {
                {"Auto Detect", MessageDataCodingScheme.Undefined},
                {"Default Alphabet - 7 Bits", MessageDataCodingScheme.DefaultAlphabet},
                {"ANSI - 8 Bits", MessageDataCodingScheme.EightBits},
                {"Unicode - 16 Bits", MessageDataCodingScheme.Ucs2}
            };

        /// <summary>
        /// Message split option
        /// </summary>
        private Dictionary<string, MessageSplitOption> MessageSplit =
            new Dictionary<string, MessageSplitOption>
            {
                {"Truncate", MessageSplitOption.Truncate},
                {"Simple Split", MessageSplitOption.SimpleSplit},
                {"Concatenate", MessageSplitOption.Concatenate}
            };

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
        /// Message priority in queue
        /// </summary>
        private Dictionary<string, MessageValidPeriod> ValidityPeriod =
          new Dictionary<string, MessageValidPeriod>
            {
                {"1 Hour", MessageValidPeriod.OneHour},
                {"3 Hours", MessageValidPeriod.ThreeHours},
                {"6 Hours", MessageValidPeriod.SixHours},
                {"12 Hours", MessageValidPeriod.TwelveHours},
                {"1 Day", MessageValidPeriod.OneDay},
                {"1 Week", MessageValidPeriod.OneWeek},
                {"Maximum", MessageValidPeriod.Maximum}               
            };

        // WAP push signal
        private Dictionary<string, ServiceIndicationAction> ServiceIndication =
            new Dictionary<string, ServiceIndicationAction>
            {
                {"None", ServiceIndicationAction.SignalNone},
                {"Low", ServiceIndicationAction.SignalLow},
                {"Medium", ServiceIndicationAction.SignalMedium},
                {"High", ServiceIndicationAction.SignalHigh}                         
            };

        // vCard home work types
        private Dictionary<string, HomeWorkTypes> vCardHomeWorkTypes =
            new Dictionary<string, HomeWorkTypes>
            {
                {"None", HomeWorkTypes.None},
                {"Home", HomeWorkTypes.Home},
                {"Work", HomeWorkTypes.Work}                                 
            };

        // vCard phone types
        private Dictionary<string, PhoneTypes> vCardPhoneTypes =
            new Dictionary<string, PhoneTypes>
            {
                {"None", PhoneTypes.None},
                {"Voice", PhoneTypes.Voice},
                {"Fax", PhoneTypes.Fax},
                {"Msg", PhoneTypes.Msg},
                {"Cell", PhoneTypes.Cell},
                {"Pager", PhoneTypes.Pager}                                           
            };

        // Log level
        private Dictionary<string, LogLevel> LoggingLevel =
            new Dictionary<string, LogLevel>
            {
                {"Error", LogLevel.Error},
                {"Warn", LogLevel.Warn},
                {"Info", LogLevel.Info},
                {"Verbose", LogLevel.Verbose}                                                 
            };

        // Message class
        private Dictionary<string, int> MessageClass =
              new Dictionary<string, int>
            {
                {"None", 0},
                {"ME", PduUtils.DcsMessageClassMe},
                {"SIM", PduUtils.DcsMessageClassSim},
                {"TE", PduUtils.DcsMessageClassTe}                                                 
            };

        // DCS Message class enum lookup
        private Dictionary<string, MessageClasses> DcsMessageClass =
              new Dictionary<string, MessageClasses>
            {
                {"None", MessageClasses.None},
                {"ME", MessageClasses.Me},
                {"SIM", MessageClasses.Sim},
                {"TE", MessageClasses.Te}                                                 
            };


        /// <summary>
        /// All available storages
        /// </summary>
        private List<MessageStorage> AllStorages = new List<MessageStorage>() { MessageStorage.Sim, MessageStorage.Phone, MessageStorage.MobileTerminating };

        /// <summary>
        /// Gateway modesl
        /// </summary>
        private List<string> Models = new List<string>() { "Wavecom", "Huawei", "Sony Ericsson", "Siemens" };

        private delegate void DisplayMessageLog(MessageReceivedEventArgs e);
        private delegate void DisplayCallLog(IncomingCallEventArgs e);
        private delegate void DisplayCallEventLog(OutgoingCallEventArgs e);
        private delegate void DisplayUssdResponse(UssdReceivedEventArgs e);

        private DisplayMessageLog displayMessageLog;
        private DisplayUssdResponse displayUssdResponse;
        private DisplayCallLog displayCallLog;
        private DisplayCallEventLog displayCallEventLog;

        /// <summary>
        /// MMS providers
        /// </summary>
        //private SortedDictionary<string, List<string>> mmsProviders;

        /// <summary>
        /// MMS provider file extension
        /// </summary>
        private const string ProviderFileExtension = ".mm1";

        /// <summary>
        /// Path which points to the embedded resource in the assembly
        /// </summary>
        private const string MMSProviderPath = "MMSProviders";

        public frmSMS()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the frmSMS control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void frmSMS_Load(object sender, EventArgs e)
        {
            // Add the port
            string[] portNames = SerialPort.GetPortNames();
            if (portNames.Length > 0)
            {
                // There is a bug that for high COM port there is a "c" at the back, e.g. "COM11c"
                var sortedList = portNames.OrderBy(port => Convert.ToInt32(port.Replace("COM", string.Empty).Replace("c", string.Empty)));
                //var sortedList = portNames.OrderBy(port => port.Replace("COM", string.Empty));
                foreach (string port in sortedList)
                {
                    if (!cboPort.Items.Contains(port))
                        cboPort.Items.Add(port);
                }
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

            // Add message type
            cboMessageType.Items.AddRange(MessageType.Keys.ToArray());
            cboMessageType.SelectedIndex = 0;

            // Add message encoding
            cboMessageEncoding.Items.AddRange(MessageEncoding.Keys.ToArray());
            cboMessageEncoding.SelectedIndex = 0;

            // Add message encoding
            cboPduEncoding.Items.AddRange(MessageEncoding.Keys.ToArray());
            cboPduEncoding.SelectedIndex = 0;

            // Queue priority
            cboQueuePriority.Items.AddRange(QueuePriority.Keys.ToArray());
            cboQueuePriority.SelectedIndex = 1;

            // Long message option
            cboLongMessage.Items.AddRange(MessageSplit.Keys.ToArray());
            cboLongMessage.SelectedIndex = 2;

            // Message validity period
            cboValidityPeriod.Items.AddRange(ValidityPeriod.Keys.ToArray());
            cboValidityPeriod.SelectedIndex = 0;


            // Message indication option
            cboMessageIndicationOption.Items.AddRange(new string[] { "Trigger", "Polling" });
            cboMessageIndicationOption.SelectedIndex = 0;

            // Service indication signal for WAP push
            cboWapPushSignal.Items.AddRange(ServiceIndication.Keys.ToArray());
            cboWapPushSignal.SelectedIndex = 2;

            displayMessageLog = new DisplayMessageLog(this.ShowMessageLog);
            displayCallLog = new DisplayCallLog(this.ShowCallLog);
            displayCallEventLog = new DisplayCallEventLog(this.ShowCallEventLog);
            displayUssdResponse = new DisplayUssdResponse(this.ShowUssdResponse);

            dtpWapPushCreated.MaxDate = DateTime.Now;
            dtpWapPushCreated.Value = DateTime.Now.AddDays(-1);

            dtpvCalendarStartDateTime.CustomFormat = CustomDateTimeFormat;
            dtpvCalendarStartDateTime.MinDate = DateTime.Now;
            dtpvCalendarEndDateTime.CustomFormat = CustomDateTimeFormat;
            dtpvCalendarEndDateTime.MinDate = DateTime.Now.AddMinutes(30);

            cbovCardAddressType.Items.AddRange(vCardHomeWorkTypes.Keys.ToArray());
            cbovCardAddressType.SelectedIndex = 0;

            cbovCardPhoneNumberHomeWorkType.Items.AddRange(vCardHomeWorkTypes.Keys.ToArray());
            cbovCardPhoneNumberHomeWorkType.SelectedIndex = 0;

            cbovCardPhoneNumberType.Items.AddRange(vCardPhoneTypes.Keys.ToArray());
            cbovCardPhoneNumberType.SelectedIndex = 0;

            cboLoggingLevel.Items.AddRange(LoggingLevel.Keys.ToArray());
            cboLoggingLevel.SelectedIndex = 0;

            cboMessageClass.Items.AddRange(MessageClass.Keys.ToArray());
            cboMessageClass.SelectedIndex = 0;

            cboDcsMessageClass.Items.AddRange(MessageClass.Keys.ToArray());
            cboDcsMessageClass.SelectedIndex = 0;

            // Enable by default
            chkEnableQueue.Checked = true;

            // Scheduled delivery date
            dtpScheduledDeliveryDate.CustomFormat = CustomDateTimeFormat;
            dtpScheduledDeliveryDate.MinDate = DateTime.Now;

            // Gateway model
            cboModel.Items.AddRange(Models.ToArray());

        }


        /// <summary>
        /// Handles the Click event of the btnConnect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            MobileGatewayConfiguration config = MobileGatewayConfiguration.NewInstance();
            //string portName = cboPort.Text;
            //if (portName.EndsWith("c")) portName = portName.Substring(0, portName.Length - 1);
            config.PortName = cboPort.Text;
            config.BaudRate = (PortBaudRate)Enum.Parse(typeof(PortBaudRate), cboBaudRate.Text);
            config.DataBits = (PortDataBits)Enum.Parse(typeof(PortDataBits), cboDataBits.Text);
            if (!string.IsNullOrEmpty(txtPin.Text))
            {
                config.Pin = txtPin.Text;
            }
            if (!string.IsNullOrEmpty(cboModel.Text))
            {
                config.Model = cboModel.Text;
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

            //  Check for the PIN status during connect. Default to false
            config.DisablePinCheck = chkDisablePinCheck.Checked;

            //  If set to true, then a dialog box will be popped up for any API errors
            config.DebugMode = chkDebugMode.Checked;

            // Default to verbose by default
            config.LogLevel = LogLevel.Verbose;

            // Set the connected event
            config.GatewayConnected += OnGatewayConnect;

            // Default to true for DTR
            config.DtrEnable = chkEnableDTR.Checked;

            // Default to true for RTS
            config.RtsEnable = chkEnableRTS.Checked;

            // Set the license key
            //config.LicenseKey = "XXXXXX-XX-XXXX";

            // If we want persistence queue. Default to false
            //config.PersistenceQueue = true;

            // Set a different log file prefix and path
            //config.LogFile = "mylog";
            //config.LogLocation = @"c:\temp";

            // Set the log file name without the date
            config.LogNameFormat = LogNameFormat.Name;

            // Set watch dog interval to 5 seconds. Default to 60 seconds
            //config.WatchDogInterval = 5000;



            // You can also specify the total number of milliseconds for AT command time out
            // Multiply these 2 parameters you can the number of milliseconds for command time out. 
            // E.g. below is the default of 30 seconds
            //config.CommandWaitInterval = 300;
            //config.CommandWaitRetryCount = 100;

            // Do this if there is a problem you faced when device is unplug and plug in again to the server
            // Most of the time you don't need to set these 2 parameter at all. They should be defaulted to FALSE.
            config.SafeConnect = chkSafeConnect.Checked;
            config.SafeDisconnect = chkSafeDisconnect.Checked;

            // Create the gateway for mobile
            MessageGateway<IMobileGateway, MobileGatewayConfiguration> messageGateway =
                MessageGateway<IMobileGateway, MobileGatewayConfiguration>.NewInstance();
            try
            {
                btnConnect.Enabled = false;
                mobileGateway = messageGateway.Find(config);
                if (mobileGateway == null)
                {
                    MessageBox.Show("Error connecting to gateway. Check the log file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //mobileGateway.LogLevel = LogLevel.Verbose;
                txtSmsc.Text = mobileGateway.ServiceCentreAddress.Number;
                updSendRetries.Value = mobileGateway.Configuration.SendRetries;
                updPollingInterval.Value = mobileGateway.Configuration.MessagePollingInterval;
                updSendWaitInterval.Value = mobileGateway.Configuration.SendWaitInterval;

                chkDeleteReceivedMessage.Checked = mobileGateway.Configuration.DeleteReceivedMessage;
                txtLogFile.Text = mobileGateway.LogFile;

                if (mobileGateway.MessageStorage == MessageStorage.Phone)
                {
                    radPhone.Checked = true;
                }
                else if (mobileGateway.MessageStorage == MessageStorage.Sim)
                {
                    radSim.Checked = true;
                }

                mobileGateway.MessageReceived += OnMessageReceived;
                mobileGateway.MessageSendingFailed += OnMessageFailed;
                mobileGateway.MessageSent += OnMessageSent;
                mobileGateway.CallReceived += OnCallReceived;
                mobileGateway.GatewayDisconnected += OnGatewayDisconnect;
                mobileGateway.WatchDogFailed += OnWatchDogFailed;

                cboCharacterSets.Items.Clear();
                cboCharacterSets.Items.AddRange(mobileGateway.SupportedCharacterSets);

                Subscriber[] subscribers = mobileGateway.Subscribers;
                if (subscribers.Length > 0)
                {
                    txtPhoneNumber.Text = subscribers[0].Number;
                }

                MessageBox.Show("Connected to gateway successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConnect.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnDisconnect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (mobileGateway != null)
                {
                    btnDisconnect.Enabled = false;
                    if (mobileGateway.Disconnect())
                    {
                        mobileGateway.Dispose();
                        mobileGateway = null;
                        mobileGateway = MobileGatewayFactory.Default;
                        MessageBox.Show("Gateway is disconnected successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                btnDisconnect.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the KeyUp event of the txtTerminal control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void txtTerminal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string command = txtTerminal.Lines[txtTerminal.Lines.Length - 2];
                if (!string.IsNullOrEmpty(command))
                {
                    string output = mobileGateway.SendCommand(command);
                    string[] lines = output.Split(new char[] { '\r', '\n' });
                    ScrollToEnd();
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrEmpty(line))
                            txtTerminal.AppendText(line + "\n");
                    }
                    txtTerminal.AppendText("\n");
                }
            }
            ScrollToEnd();
        }

        /// <summary>
        /// Scrolls to end.
        /// </summary>
        private void ScrollToEnd()
        {
            txtTerminal.SelectionStart = txtTerminal.Text.Length;
            txtTerminal.ScrollToCaret();
        }

        /// <summary>
        /// Handles the Click event of the tabMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tabMain_Click(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            if (tc.SelectedTab == tabTerminal)
            {
                txtTerminal.Focus();
                ScrollToEnd();
            }
            else if (tc.SelectedTab == tabPhonebook)
            {
                if (cboPhoneBookStorage.Items.Count == 0)
                {
                    // Add the phone book storages
                    string[] phoneBookStorages = mobileGateway.PhoneBookStorages;
                    cboPhoneBookStorage.Items.AddRange(phoneBookStorages.OrderBy(storage => storage).ToArray());
                }
            }
            else if (tc.SelectedTab == tabAbout)
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
        /// Handles the Click event of the btnRefreshPhoneBook control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRefreshPhoneBook_Click(object sender, EventArgs e)
        {
            try
            {
                btnRefreshPhoneBook.Enabled = false;
                string storage = (string)cboPhoneBookStorage.SelectedItem;
                if (!string.IsNullOrEmpty(storage))
                {
                    PhoneBookEntry[] phoneBookEntries = mobileGateway.GetPhoneBook(storage);
                    dgdPhoneBook.DataSource = phoneBookEntries;
                    lblPhoneBookEntryCount.Text = phoneBookEntries.Count() + " record(s) found";
                }
            }
            finally
            {
                btnRefreshPhoneBook.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnDeleteEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnDeleteEntry_Click(object sender, EventArgs e)
        {
            DataGridViewRow currentRow = dgdPhoneBook.CurrentRow;
            try
            {
                btnDeleteEntry.Enabled = false;
                int index = (int)dgdPhoneBook.CurrentRow.Cells[0].Value;
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove entry with index " + index + " ?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    string storage = (string)cboPhoneBookStorage.SelectedItem;
                    if (!string.IsNullOrEmpty(storage))
                    {
                        if (mobileGateway.DeletePhoneBookEntry(index, storage))
                        {
                            MessageBox.Show("Phonebook entry is deleted successfully. Click Refresh button to refresh the view", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Unable to delete phonebook entry: " + mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDeleteEntry.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnExportToXml control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnExportToXml_Click(object sender, EventArgs e)
        {
            try
            {
                btnExportToXml.Enabled = false;
                string storage = (string)cboPhoneBookStorage.SelectedItem;
                if (!string.IsNullOrEmpty(storage))
                {
                    string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string outputFile = currentDirectory + Path.DirectorySeparatorChar + "phonebook.xml";

                    PhoneBookEntry[] phoneBookEntries = mobileGateway.GetPhoneBook(storage);
                    XmlSerializer s = new XmlSerializer(typeof(PhoneBookEntry[]));
                    TextWriter w = new StreamWriter(outputFile, false);
                    s.Serialize(w, phoneBookEntries);
                    w.Close();
                    MessageBox.Show("Phonebook entries are saved into " + outputFile, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnExportToXml.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnExportTovCard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnExportTovCard_Click(object sender, EventArgs e)
        {
            try
            {
                btnExportTovCard.Enabled = false;
                string storage = (string)cboPhoneBookStorage.SelectedItem;
                if (!string.IsNullOrEmpty(storage))
                {
                    string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string outputFile = currentDirectory + Path.DirectorySeparatorChar + "phonebook.vcard";
                    PhoneBookEntry[] phoneBookEntries = mobileGateway.GetPhoneBook(storage);
                    vCard[] vCards = mobileGateway.ExportPhoneBookTovCard(phoneBookEntries);
                    TextWriter w = new StreamWriter(outputFile, false);
                    foreach (vCard vCard in vCards)
                    {
                        w.WriteLine(vCard.ToString());
                        w.WriteLine("");
                    }
                    w.Close();
                    MessageBox.Show("Phonebook entries are saved into " + outputFile, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnExportTovCard.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnRefreshDeviceInformation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRefreshDeviceInformation_Click(object sender, EventArgs e)
        {
            try
            {
                btnRefreshDeviceInformation.Enabled = false;
                DeviceInformation deviceInformation = mobileGateway.DeviceInformation;
                txtModel.Text = deviceInformation.Model;
                txtManufacturer.Text = deviceInformation.Manufacturer;
                txtFirmware.Text = deviceInformation.FirmwareVersion;
                txtSerialNo.Text = deviceInformation.SerialNo;
                txtImsi.Text = deviceInformation.Imsi;
            }
            finally
            {
                btnRefreshDeviceInformation.Enabled = true;
            }
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
                    Pdu.Pdu pdu;
                    pdu = pduParser.ParsePdu(txtPdu.Text.Trim());
                    if (pdu.Binary)
                    {
                        pdu.SetDataBytes(pdu.UserDataAsBytes);
                    }
                    /*
                    string generatedPduString = pduGenerator.GeneratePduString(pdu);
                    pdu = pduParser.ParsePdu(generatedPduString);                
                    */
                    txtDecodedPdu.Text = pdu.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnApplyConfiguration control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnApplyConfiguration_Click(object sender, EventArgs e)
        {
            int sendRetries = Convert.ToInt32(updSendRetries.Value);
            int pollingInterval = Convert.ToInt32(updPollingInterval.Value);
            int sendWaitInterval = Convert.ToInt32(updSendWaitInterval.Value);
            bool deleteReceivedMessage = chkDeleteReceivedMessage.Checked;
            bool debugMode = chkDebugMode.Checked;

            if (sendRetries > 0)
            {
                mobileGateway.Configuration.SendRetries = sendRetries;
            }
            if (pollingInterval > 0)
            {
                mobileGateway.Configuration.MessagePollingInterval = pollingInterval;
            }
            if (sendWaitInterval > 0)
            {
                mobileGateway.Configuration.SendWaitInterval = sendWaitInterval;
            }

            mobileGateway.Configuration.DebugMode = debugMode;
            mobileGateway.Configuration.DeleteReceivedMessage = deleteReceivedMessage;
        }

        /// <summary>
        /// Handles the Click event of the btnRefreshGatewayInformation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRefreshGatewayInformation_Click(object sender, EventArgs e)
        {
            try
            {
                btnRefreshGatewayInformation.Enabled = false;
                txtIncomingMessage.Text = Convert.ToString(mobileGateway.Statistics.IncomingSms);
                txtOutgoingMessage.Text = Convert.ToString(mobileGateway.Statistics.OutgoingSms);
                txtIncomingCall.Text = Convert.ToString(mobileGateway.Statistics.IncomingCall);
                txtOutgoingCall.Text = Convert.ToString(mobileGateway.Statistics.OutgoingCall);
            }
            finally
            {
                btnRefreshGatewayInformation.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the radPhone control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void radPhone_CheckedChanged(object sender, EventArgs e)
        {
            if (radPhone.Checked)
            {
                mobileGateway.MessageStorage = MessageStorage.Phone;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the radSim control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void radSim_CheckedChanged(object sender, EventArgs e)
        {
            if (radSim.Checked)
            {
                mobileGateway.MessageStorage = MessageStorage.Sim;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the radBoth control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void radBoth_CheckedChanged(object sender, EventArgs e)
        {
            if (radMT.Checked)
            {
                mobileGateway.MessageStorage = MessageStorage.MobileTerminating;
            }

        }

        /// <summary>
        /// Handles the Click event of the btnRetrieveMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRetrieveMessage_Click(object sender, EventArgs e)
        {
            try
            {
                btnRetrieveMessage.Enabled = false;
                MessageStatusType messageType;
                if (MessageType.TryGetValue(cboMessageType.Text, out messageType))
                {
                    List<MessageInformation> messages = null;
                    if (radAll.Checked)
                    {
                        messages = mobileGateway.GetMessages(messageType, AllStorages);
                    }
                    else
                    {
                        messages = mobileGateway.GetMessages(messageType);
                    }
                    dgdMessages.DataSource = messages;
                    lblMessageCount.Text = messages.Count() + " message(s) found";
                }
            }
            finally
            {
                btnRetrieveMessage.Enabled = true;
            }

        }

        private void btnExportMessageToXml_Click(object sender, EventArgs e)
        {
            try
            {
                btnExportMessageToXml.Enabled = false;
                MessageStatusType messageType;
                if (MessageType.TryGetValue(cboMessageType.Text, out messageType))
                {
                    string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string outputFile = currentDirectory + Path.DirectorySeparatorChar + "message.xml";

                    List<MessageInformation> messages = (List<MessageInformation>)mobileGateway.GetMessages(messageType);
                    XmlSerializer s = new XmlSerializer(typeof(List<MessageInformation>));
                    TextWriter w = new StreamWriter(outputFile, false);
                    s.Serialize(w, messages);
                    w.Close();
                    MessageBox.Show("Messages are saved into " + outputFile, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnExportMessageToXml.Enabled = true;
            }
        }

        private void btnDeleteMessage_Click(object sender, EventArgs e)
        {
            DataGridViewRow currentRow = dgdMessages.CurrentRow;
            try
            {
                btnDeleteMessage.Enabled = false;
                int index = (int)dgdMessages.CurrentRow.Cells[10].Value;
                int totalPiece = (int)dgdMessages.CurrentRow.Cells[5].Value;

                if (totalPiece > 1)
                {
                    MessageBox.Show("This is a long message. In order to delete it, you should use MessageInformation.Indexes to retrieve the message indexes and delete it one by one", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove message with index " + index + " ?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    MessageStatusType messageType;
                    if (MessageType.TryGetValue(cboMessageType.Text, out messageType))
                    {
                        // This is assume that the message is not multipart. If it is a long message, then you should
                        // use MessageInformation.Indexes property to get all indexes, and delete them 1 by 1

                        if (mobileGateway.DeleteMessage(MessageDeleteOption.ByIndex, index))
                        {
                            MessageBox.Show("Message is deleted successfully. Click Refresh button to refresh the view", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Unable to delete message: " + mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDeleteMessage.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnClearQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnClearQueue_Click(object sender, EventArgs e)
        {
            mobileGateway.ClearQueue();
        }

        /// <summary>
        /// Handles the Click event of the btnSendMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPhoneNumberList.Text) && !string.IsNullOrEmpty(txtMessage.Text))
                {
                    btnSendMessage.Enabled = false;

                    string[] phoneNumberList = txtPhoneNumberList.Text.Split(',');
                    foreach (string phoneNumber in phoneNumberList)
                    {
                        Sms sms = Sms.NewInstance();
                        // This is a unique identifier you can set for your message
                        // You can use this identifier to identify the message if you queue up the message for sending
                        // Alternatively, if you do not assign a value, the identifier will be generated for you automatically
                        // sms.Identifier = "1234567890";                        
                        sms.DestinationAddress = phoneNumber.Trim();
                        sms.Content = txtMessage.Text;

                        MessageDataCodingScheme encoding;
                        if (MessageEncoding.TryGetValue(cboMessageEncoding.Text, out encoding))
                        {
                            sms.DataCodingScheme = encoding;
                        }
                        MessageValidPeriod validity;
                        if (ValidityPeriod.TryGetValue(cboValidityPeriod.Text, out validity))
                        {
                            // Do this if you want custom validity period, e.g. 5 minutes
                            //sms.ValidityPeriod = MessageValidPeriod.Custom;
                            //sms.CustomValidityPeriod = 5;

                            sms.ValidityPeriod = validity;

                        }
                        MessageSplitOption splitOption;
                        if (MessageSplit.TryGetValue(cboLongMessage.Text, out splitOption))
                        {
                            sms.LongMessageOption = splitOption;
                        }

                        if (chkStatusReport.Checked) sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;

                        // If batch message mode is not supported, you can set it to not supported. By default it is set to not supported.
                        // mobileGateway.BatchMessageMode = BatchMessageMode.NotSupported;

                        if (chkBatchMessageMode.Checked)
                            mobileGateway.BatchMessageMode = BatchMessageMode.Temporary;
                        else
                            mobileGateway.BatchMessageMode = BatchMessageMode.Disabled;



                        if (chkUseDefaultSmsc.Checked)
                        {
                            sms.ServiceCenterNumber = Sms.DefaultSmscAddress;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(txtMessageSmsc.Text))
                            {
                                sms.ServiceCenterNumber = txtMessageSmsc.Text;
                            }
                        }

                        // Set the message class
                        MessageClasses messageClass;
                        if (DcsMessageClass.TryGetValue(cboDcsMessageClass.Text, out messageClass))
                        {
                            sms.DcsMessageClass = messageClass;
                        }

                        // Put this after message class to override the message class setting
                        // Alternatively, set sms.DcsMessageClass = MessageClasses.Flash and it will do the same thing
                        if (chkAlertMessage.Checked) sms.Flash = true;

                        // If you want to save the sent message, set it to true. Default is false
                        sms.SaveSentMessage = chkSaveSentMessage.Checked;

                        if (chkScheduled.Checked)
                        {
                            // Set scheduled delivery date
                            sms.ScheduledDeliveryDate = dtpScheduledDeliveryDate.Value;
                            //sms.ScheduledDeliveryDate = DateTime.Now.AddMinutes(1); // for testing
                        }

                        if (chkSendToQueue.Checked)
                        {
                            MessageQueuePriority priority;
                            if (QueuePriority.TryGetValue(cboQueuePriority.Text, out priority))
                            {
                                sms.QueuePriority = priority;
                            }

                            if (mobileGateway.SendToQueue(sms))
                            {
                                Logger.LogThis("Queued message identifier: " + sms.Identifier, LogLevel.Verbose);
                                //MessageBox.Show("Message is queued successfully for " + sms.DestinationAddress, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            if (mobileGateway.Send(sms))
                            {
                                MessageBox.Show("Message is sent successfully to " + sms.DestinationAddress + ". Message index is  " + (sms.Indexes.Count() > 0 ? string.Join(",", (sms.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()) : "[Not available]"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Phone number and message content cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnSendMessage.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkSendToQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkSendToQueue_CheckedChanged(object sender, EventArgs e)
        {
            cboQueuePriority.Enabled = chkSendToQueue.Checked;
        }

        /// <summary>
        /// Handles the Click event of the btnRefreshQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRefreshQueue_Click(object sender, EventArgs e)
        {
            lblMessageQueueCount.Text = "Messages in Queue: " + mobileGateway.GetQueueCount();
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkEnableMessageIndication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkEnableMessageIndication_CheckedChanged(object sender, EventArgs e)
        {
            cboMessageIndicationOption.Enabled = chkEnableMessageIndication.Checked;
        }

        /// <summary>
        /// Handles the Click event of the btnApplyMessageSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnApplyMessageSettings_Click(object sender, EventArgs e)
        {
            try
            {
                btnApplyMessageSettings.Enabled = false;
                bool result = false;
                if (chkEnableMessageIndication.Checked)
                {
                    if (cboMessageIndicationOption.Text.Equals("Trigger"))
                    {
                        // Incoming SMS and status report messages will be using trigger
                        result = mobileGateway.EnableNewMessageNotification(MessageNotification.ReceivedMessage | MessageNotification.StatusReport);

                        // Set to false to disable polling
                        mobileGateway.PollNewMessages = false;
                    }
                    else
                    {
                        // If you fail to receive status report indication, then try to comment this line,
                        // and uncomment out DisableMessageNotifications
                        result = mobileGateway.EnableNewMessageNotification(MessageNotification.StatusReport);

                        // Disable incoming notifications since we want to use polling
                        //result = mobileGateway.DisableMessageNotifications();

                        // Incoming SMS will be polled periodically
                        mobileGateway.PollNewMessages = true;

                    }
                }
                else
                {
                    // Disable all incoming notifications
                    result = mobileGateway.DisableMessageNotifications();

                    // No polling will be performed
                    mobileGateway.PollNewMessages = false;
                }
                mobileGateway.Configuration.DeleteReceivedMessage = chkDeleteAfterReceive.Checked;
                if (result)
                {
                    MessageBox.Show("Message settings are applied", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error applying settings", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnApplyMessageSettings.Enabled = true;
            }
        }

        /// <summary>
        /// Called when message is received
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Mobile.Event.MessageReceivedEventArgs"/> instance containing the event data.</param>
        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            txtMessageLog.BeginInvoke(displayMessageLog, e);
        }

        /// <summary>
        /// Called when message is sent
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Mobile.Event.MessageEventArgs"/> instance containing the event data.</param>
        private void OnMessageSent(object sender, MessageEventArgs e)
        {
            Sms sms = (Sms)e.Message;
            MessageBox.Show("Message is sent successfully to " + sms.DestinationAddress + ". Message index is  " + string.Join(",", (sms.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()) + ". Message identifier is " + sms.Identifier, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Called when message sending failed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Mobile.Event.MessageErrorEventArgs"/> instance containing the event data.</param>
        private void OnMessageFailed(object sender, MessageErrorEventArgs e)
        {
            Sms sms = (Sms)e.Message;
            MessageBox.Show("Failed to send message to " + sms.DestinationAddress, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            MessageBox.Show(e.Error.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        /// <summary>
        /// Shows the message log.
        /// </summary>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Mobile.Event.MessageReceivedEventArgs"/> instance containing the event data.</param>
        private void ShowMessageLog(MessageReceivedEventArgs e)
        {
            if (e.Message.MessageType == MessageTypeIndicator.MtiSmsDeliver)
            {
                txtMessageLog.AppendText("Received message from " + e.Message.PhoneNumber + "\n");
                txtMessageLog.AppendText("Received date: " + e.Message.ReceivedDate + "\n");
                txtMessageLog.AppendText("Message type: " + e.Message.MessageType + "\n");
                txtMessageLog.AppendText("Received content: " + e.Message.Content + "\n");
                txtMessageLog.AppendText("\n");
            }
            else
            {
                if (e.Message.DeliveryStatus == MessageStatusReportStatus.Success)
                {
                    txtMessageLog.AppendText("Message is delivered to " + e.Message.PhoneNumber + "\n");
                    txtMessageLog.AppendText("Delivered date: " + e.Message.ReceivedDate + "\n");
                    txtMessageLog.AppendText("Message type: " + e.Message.MessageType + "\n");
                    txtMessageLog.AppendText("Content: " + e.Message.Content + "\n");
                    txtMessageLog.AppendText("\n");
                }
                else
                {
                    txtMessageLog.AppendText("Message is not delivered to " + e.Message.PhoneNumber + "\n");
                    txtMessageLog.AppendText("Message type: " + e.Message.MessageType + "\n");
                    txtMessageLog.AppendText("Delivery status: " + e.Message.DeliveryStatus + "\n");
                    txtMessageLog.AppendText("Content: " + e.Message.Content + "\n");
                    txtMessageLog.AppendText("\n");
                }
            }
        }

        /// <summary>
        /// Shows the call log.
        /// </summary>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Mobile.Event.IncomingCallEventArgs"/> instance containing the event data.</param>
        private void ShowCallLog(IncomingCallEventArgs e)
        {
            txtIncomingCallIndication.AppendText("Call received from " + e.CallInformation.Number);
            txtIncomingCallIndication.AppendText("\n");
            txtIncomingCallIndication.AppendText("Number type - " + e.CallInformation.NumberType);
            txtIncomingCallIndication.AppendText("\n\n");
        }

        /// <summary>
        /// Handles the Click event of the btnSendWapPush control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSendWapPush_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtWapPushPhoneNumber.Text) &&
                    !string.IsNullOrEmpty(txtWapPushUrl.Text) &&
                    !string.IsNullOrEmpty(txtWapPushMessage.Text))
                {

                    btnSendWapPush.Enabled = false;
                    Wappush wappush = Wappush.NewInstance(txtWapPushPhoneNumber.Text, txtWapPushUrl.Text, txtWapPushMessage.Text);

                    ServiceIndicationAction signal;
                    if (ServiceIndication.TryGetValue(cboWapPushSignal.Text, out signal))
                    {
                        wappush.Signal = signal;
                    }
                    if (chkWapPushCreated.Checked) wappush.CreateDate = dtpWapPushCreated.Value;
                    if (chkWapPushExpiry.Checked) wappush.ExpireDate = dtpWapPushExpiryDate.Value;

                    if (mobileGateway.Send(wappush))
                    {
                        MessageBox.Show("WAP push message is sent successfully to " + wappush.DestinationAddress + ". Message index is  " + string.Join(",", (wappush.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Phone number, URL and message cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnSendWapPush.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkWapPushCreated control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkWapPushCreated_CheckedChanged(object sender, EventArgs e)
        {
            dtpWapPushCreated.Enabled = chkWapPushCreated.Checked;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkWapPushExpiry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkWapPushExpiry_CheckedChanged(object sender, EventArgs e)
        {
            dtpWapPushExpiryDate.Enabled = chkWapPushExpiry.Checked;
        }

        /// <summary>
        /// Handles the Click event of the btnSendvCalendar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSendvCalendar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtvCalendarPhoneNumber.Text))
                {
                    btnSendvCalendar.Enabled = false;

                    if (!string.IsNullOrEmpty(txtvCalendarEventSummary.Text))
                    {
                        vEvent vEvent = vEvent.NewInstance();
                        vEvent.DtStart = dtpvCalendarStartDateTime.Value;
                        vEvent.DtEnd = dtpvCalendarEndDateTime.Value;
                        vEvent.Summary = txtvCalendarEventSummary.Text;
                        vEvent.Location = txtvCalendarEventLocation.Text;
                        vEvent.Description = txtvCalendarEventDescription.Text;
                        vCalendar vCalendar = vCalendar.NewInstance(vEvent);
                        vCalendar.DestinationAddress = txtvCalendarPhoneNumber.Text;

                        // You can add an vAlarm 
                        //vAlarm alarm = new vAlarm(DateTime.Now, new TimeSpan(1, 0, 0, 0), 2, "reminder");
                        //vEvent.Alarms.Add(alarm);

                        // You can add a recurrence rule
                        //vRecurrenceRule recurrenceRule = new vRecurrenceRule(EventRepeat.Daily, vEvent.DtEnd);
                        //vEvent.RecurrenceRule = recurrenceRule;

                        if (mobileGateway.Send(vCalendar))
                        {
                            MessageBox.Show("vCalendar message is sent successfully to " + vCalendar.DestinationAddress + ". Message index is  " + string.Join(",", (vCalendar.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (!string.IsNullOrEmpty(txtvCalendarFileLocation.Text))
                    {
                        string fileName = txtvCalendarFileLocation.Text;
                        if (File.Exists(fileName))
                        {
                            string fileContent = File.ReadAllText(fileName, Encoding.UTF8);
                            vCalendar vCalendar = vCalendar.NewInstance();
                            vCalendar.LoadString(fileContent);
                            vCalendar.DestinationAddress = txtvCalendarPhoneNumber.Text;
                            if (mobileGateway.Send(vCalendar))
                            {
                                MessageBox.Show("vCalendar message is sent successfully to " + vCalendar.DestinationAddress + ". Message index is  " + string.Join(",", (vCalendar.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Phone number cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnSendvCalendar.Enabled = true;
            }
        }

        private void btnBrowservCalendarFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "vCalendar File (*.vcs)|*.vcs";
            openFileDialog1.FileName = string.Empty;
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;

                // Read the file content and send as vCalendar
                txtvCalendarFileLocation.Text = fileName;
            }
        }

        private void frmSMS_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mobileGateway != null)
            {
                mobileGateway.Disconnect();
                mobileGateway.Dispose();
                mobileGateway = null;
            }
        }

        private void btnSendSmartSms_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSmartSmsPhoneNumber.Text))
                {
                    btnSendSmartSms.Enabled = false;

                    if (picOtaBitmap.Image != null)
                    {
                        // Send OTA bitmap message
                        OtaBitmap otaBitmap = OtaBitmap.NewInstance(new Bitmap(picOtaBitmap.Image));
                        otaBitmap.DestinationAddress = txtSmartSmsPhoneNumber.Text;
                        if (mobileGateway.Send(otaBitmap))
                        {
                            MessageBox.Show("Bitmap message is sent successfully to " + otaBitmap.DestinationAddress + ". Message index is  " + string.Join(",", (otaBitmap.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // Send operator logo
                        if (!string.IsNullOrEmpty(txtMCC.Text) && !string.IsNullOrEmpty(txtMNC.Text))
                        {
                            OperatorLogo operatorLogo = OperatorLogo.NewInstance(new Bitmap(picOtaBitmap.Image), txtMCC.Text, txtMNC.Text);
                            operatorLogo.DestinationAddress = txtSmartSmsPhoneNumber.Text;
                            if (mobileGateway.Send(operatorLogo))
                            {
                                MessageBox.Show("Operator logo is sent successfully to " + operatorLogo.DestinationAddress + ". Message index is  " + string.Join(",", (operatorLogo.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    // Send ringtone
                    if (!string.IsNullOrEmpty(txtSmartSmsRingtone.Text))
                    {
                        Ringtone ringtone = Ringtone.NewInstance();
                        ringtone.Content = txtSmartSmsRingtone.Text;
                        ringtone.DestinationAddress = txtSmartSmsPhoneNumber.Text;
                        if (mobileGateway.Send(ringtone))
                        {
                            MessageBox.Show("Ringtone message is sent successfully to " + ringtone.DestinationAddress + ". Message index is  " + string.Join(",", (ringtone.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    // Send custom Smart SMS
                    if (!string.IsNullOrEmpty(txtSmartSmsMessage.Text))
                    {
                        // Construt a Smart SMS
                        Sms smartSms = Sms.NewInstance();
                        smartSms.Content = txtSmartSmsMessage.Text;
                        smartSms.DataCodingScheme = MessageDataCodingScheme.EightBits;
                        smartSms.DestinationAddress = txtSmartSmsPhoneNumber.Text;
                        smartSms.SourcePort = Convert.ToInt32(nupdSmartSmsSourcePort.Value);
                        smartSms.DestinationPort = Convert.ToInt32(nupdSmartSmsDestinationPort.Value);

                        if (mobileGateway.Send(smartSms))
                        {
                            MessageBox.Show("Smart SMS message is sent successfully to " + smartSms.DestinationAddress + ". Message index is  " + string.Join(",", (smartSms.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Phone number cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnSendSmartSms.Enabled = true;
            }

        }

        /// <summary>
        /// Handles the Click event of the btnSendUsdd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSendUsdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtUssdCommand.Text))
                {
                    btnSendUsdd.Enabled = false;

                    string response = string.Empty;
                    if (string.IsNullOrEmpty(txtDcs.Text))
                    {
                        response = mobileGateway.SendUssd(txtUssdCommand.Text);
                    }
                    else
                    {
                        int dcs = 0;
                        try
                        {
                            dcs = Convert.ToInt32(txtDcs.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        // Here you can control the DCS
                        UssdRequest ussdRequest = new UssdRequest(txtUssdCommand.Text);
                        ussdRequest.Dcs = UssdDcs.GetByNumeric(dcs);
                        UssdResponse ussdResponse = mobileGateway.SendUssd(ussdRequest);
                        response = ussdResponse.Content;
                    }
                    if (string.IsNullOrEmpty(response) && !mobileGateway.EnableUssdEvent)
                    {
                        // Error
                        txtUssdResponse.AppendText(mobileGateway.LastError.Message);
                        txtUssdResponse.AppendText("\n");
                    }
                    else
                    {
                        txtUssdResponse.AppendText(response);
                        txtUssdResponse.AppendText("\n");
                    }
                }
                else
                {
                    MessageBox.Show("USSD command cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnSendUsdd.Enabled = true;
            }
        }

        private void chkIncomingCallIndication_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIncomingCallIndication.Checked)
            {
                mobileGateway.EnableCallNotifications();
            }
            else
            {
                mobileGateway.DisableCallNotifications();
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkdEnableClir control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkdEnableClir_CheckedChanged(object sender, EventArgs e)
        {
            if (chkdEnableClir.Checked)
            {
                mobileGateway.EnableCLIR();
            }
            else
            {
                mobileGateway.DisableCLIR();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnMakeCall control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnMakeCall_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtCallingNo.Text))
                {
                    btnMakeCall.Enabled = false;
                    mobileGateway.Dial(txtCallingNo.Text);
                }
                else
                {
                    MessageBox.Show("Calling number cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnMakeCall.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnHangUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnHangUp_Click(object sender, EventArgs e)
        {
            try
            {
                btnHangUp.Enabled = false;
                mobileGateway.HangUp();
            }
            finally
            {
                btnHangUp.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnAnswerCall control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnAnswerCall_Click(object sender, EventArgs e)
        {
            try
            {
                btnAnswerCall.Enabled = false;
                mobileGateway.Answer();
            }
            finally
            {
                btnAnswerCall.Enabled = true;
            }
        }

        /// <summary>
        /// Called when [call received].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Mobile.Event.IncomingCallEventArgs"/> instance containing the event data.</param>
        private void OnCallReceived(object sender, IncomingCallEventArgs e)
        {
            txtCallingNo.BeginInvoke(displayCallLog, e);
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
                    MessageDataCodingScheme encoding;
                    if (MessageEncoding.TryGetValue(cboPduEncoding.Text, out encoding))
                    {
                        if (encoding == MessageDataCodingScheme.DefaultAlphabet)
                        {
                            pdu.DataCodingScheme = PduUtils.DcsEncoding7Bit;
                        }
                        else if (encoding == MessageDataCodingScheme.EightBits)
                        {
                            pdu.DataCodingScheme = PduUtils.DcsEncoding8Bit;
                        }
                        else if (encoding == MessageDataCodingScheme.Ucs2)
                        {
                            pdu.DataCodingScheme = PduUtils.DcsEncodingUcs2;
                        }
                        else
                        {
                            // Derive the encoding. Added Dec 31 2014
                            pdu.DataCodingScheme = PduUtils.GetDataCodingScheme(txtUserData.Text);
                        }
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

                    if (encoding == MessageDataCodingScheme.EightBits)
                    {
                        if (GetDataCodingScheme(userData) == MessageDataCodingScheme.Ucs2)
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
        /// Calculate message length
        /// </summary>
        /// <param name="pduString">PDU string</param>
        /// <returns>Message length</returns>
        protected int GetAtLength(string pduString)
        {
            // Get AT command length
            return (pduString.Length - Convert.ToInt32(pduString.Substring(0, 2), 16) * 2 - 2) / 2;
        }

        private void btnSendvCard_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtvCardDestinationNumber.Text))
                {
                    btnSendvCard.Enabled = false;

                    if (
                        !string.IsNullOrEmpty(txtvCardPhoneNumber.Text) ||
                        !string.IsNullOrEmpty(txtvCardFormattedName.Text) ||
                        !string.IsNullOrEmpty(txtvCardSurname.Text))
                    {
                        vCard vCard = vCard.NewInstance();
                        vCard.FormattedName = txtvCardFormattedName.Text;
                        vCard.Surname = txtvCardSurname.Text;
                        vCard.GivenName = txtvCardGivenName.Text;
                        vCard.MiddleName = txtvCardMiddleName.Text;
                        vCard.Prefix = txtvCardPrefix.Text;
                        vCard.Suffix = txtvCardSuffix.Text;
                        vCard.Title = txtvCardTitle.Text;
                        vCard.Birthday = dtpvCardBirthday.Value;
                        vCard.Org = txtvCardOrg.Text;
                        vCard.Department = txtvCardDepartment.Text;
                        vCard.Note = txtvCardNote.Text;
                        vCard.Role = txtvCardRole.Text;

                        if (!string.IsNullOrEmpty(txtvCardUrl.Text))
                        {
                            URL url = new URL();
                            url.Address = txtvCardUrl.Text;
                            vCard.URLs.Add(url);
                        }

                        vCard.DestinationAddress = txtvCardDestinationNumber.Text;

                        if (!string.IsNullOrEmpty(txtvCardStreet.Text))
                        {
                            Address address = new Address();
                            address.Street = txtvCardStreet.Text;
                            address.Postcode = txtvCardPostcode.Text;
                            address.Region = txtvCardRegion.Text;
                            address.Country = txtvCardCountry.Text;
                            HomeWorkTypes homeWorkType;
                            if (vCardHomeWorkTypes.TryGetValue(cbovCardAddressType.Text, out homeWorkType))
                            {
                                address.HomeWorkType = homeWorkType;
                            }
                            vCard.Addresses.Add(address);
                        }

                        if (!string.IsNullOrEmpty(txtvCardEmail.Text))
                        {
                            EmailAddress email = new EmailAddress();
                            email.Address = txtvCardEmail.Text;
                            vCard.Emails.Add(email);
                        }

                        if (!string.IsNullOrEmpty(txtvCardPhoneNumber.Text))
                        {
                            PhoneNumber phoneNumber = new PhoneNumber();
                            phoneNumber.Pref = chkvCardPhoneNumberPreferred.Checked;
                            phoneNumber.Number = txtvCardPhoneNumber.Text;
                            HomeWorkTypes homeWorkType;
                            if (vCardHomeWorkTypes.TryGetValue(cbovCardPhoneNumberHomeWorkType.Text, out homeWorkType))
                            {
                                phoneNumber.HomeWorkType = homeWorkType;
                            }
                            PhoneTypes phoneType;
                            if (vCardPhoneTypes.TryGetValue(cbovCardPhoneNumberType.Text, out phoneType))
                            {
                                phoneNumber.PhoneType = phoneType;
                            }
                            vCard.Phones.Add(phoneNumber);
                        }

                        if (mobileGateway.Send(vCard))
                        {
                            MessageBox.Show("vCard message is sent successfully to " + vCard.DestinationAddress + ". Message index is  " + string.Join(",", (vCard.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (!string.IsNullOrEmpty(txtvCardFileLocation.Text))
                    {
                        string fileName = txtvCardFileLocation.Text;
                        if (File.Exists(fileName))
                        {
                            string fileContent = File.ReadAllText(fileName, Encoding.UTF8);
                            vCard vCard = vCard.NewInstance();
                            vCard.LoadString(fileContent);
                            vCard.DestinationAddress = txtvCardDestinationNumber.Text;
                            if (mobileGateway.Send(vCard))
                            {
                                MessageBox.Show("vCard message is sent successfully to " + vCard.DestinationAddress + ". Message index is  " + string.Join(",", (vCard.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Destination phone number and contact name cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnSendvCard.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnBrowservCardFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnBrowservCardFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "vCard File (*.vcf)|*.vcf";
            openFileDialog1.FileName = string.Empty;
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;

                // Read the file content and send as vCard
                txtvCardFileLocation.Text = fileName;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnApplyLoggingLevel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnApplyLoggingLevel_Click(object sender, EventArgs e)
        {
            LogLevel logLevel;
            if (LoggingLevel.TryGetValue(cboLoggingLevel.Text, out logLevel))
            {
                try
                {
                    btnApplyLoggingLevel.Enabled = false;
                    mobileGateway.LogLevel = logLevel;
                    MessageBox.Show("Log level is applied", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    btnApplyLoggingLevel.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnApplyCharacterSet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnApplyCharacterSet_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboCharacterSets.Text))
            {
                try
                {
                    btnApplyCharacterSet.Enabled = false;
                    if (mobileGateway.SetCharacterSet(cboCharacterSets.Text))
                    {
                        MessageBox.Show("Character set is applied", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Unable to set character set", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                finally
                {
                    btnApplyCharacterSet.Enabled = true;
                }
            }
        }

        private void btnRefreshGatewayStatus_Click(object sender, EventArgs e)
        {
            try
            {
                btnRefreshGatewayStatus.Enabled = false;

                BatteryCharge batteryCharge = mobileGateway.BatteryCharge;
                progressBarBatteryLevel.Value = batteryCharge.BatteryChargeLevel;

                SignalQuality signalQuality = mobileGateway.SignalQuality;
                progressBarSignalQuality.Value = signalQuality.SignalStrengthPercent;
            }
            finally
            {
                btnRefreshGatewayStatus.Enabled = true;
            }
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

        private void OnGatewayConnect(object sender, ConnectionEventArgs e)
        {
            // Called when gateway is connected
            //MessageBox.Show("Connect");
            Console.WriteLine(e.GatewayId);
        }

        private void OnGatewayDisconnect(object sender, ConnectionEventArgs e)
        {
            // Called when gateway is disconnected
            //MessageBox.Show("Disconnect");
            Console.WriteLine(e.GatewayId);
        }

        private void OnWatchDogFailed(object sender, WatchDogEventArgs e)
        {
            // Called when gateway is disconnected
            Console.WriteLine(e.GatewayId);
            if (e.Error != null)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void chkEnableUssdEvent_CheckedChanged(object sender, EventArgs e)
        {
            mobileGateway.EnableUssdEvent = this.chkEnableUssdEvent.Checked;
            if (chkEnableUssdEvent.Checked)
            {
                mobileGateway.UssdResponseReceived += OnUssdResponseReceived;
            }
            else
            {
                mobileGateway.UssdResponseReceived -= OnUssdResponseReceived;
            }
        }

        private void OnUssdResponseReceived(object sender, UssdReceivedEventArgs e)
        {
            txtUssdResponse.BeginInvoke(displayUssdResponse, e);
        }

        private void ShowUssdResponse(UssdReceivedEventArgs e)
        {
            UssdResponse ussdResponse = e.UssdResponse;
            txtUssdResponse.AppendText(ussdResponse.Content);
            txtUssdResponse.AppendText("\n");
        }


        /// <summary>
        /// Determine the message data coding scheme
        /// </summary>
        /// <param name="content">Message content</param>
        /// <returns>Message data coding scheme. See <see cref="MessageDataCodingScheme"/></returns>
        private static MessageDataCodingScheme GetDataCodingScheme(string content)
        {
            int i = 0;
            for (i = 1; i <= content.Length; i++)
            {
                int code = Convert.ToInt32(Convert.ToChar(content.Substring(i - 1, 1)));
                if (code < 0 || code > 255)
                {
                    return MessageDataCodingScheme.Ucs2;
                }
            }
            return MessageDataCodingScheme.DefaultAlphabet;
        }

        /// <summary>
        /// Handles the Click event of the btnViewLogFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnViewLogFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLogFile.Text) && File.Exists(txtLogFile.Text))
            {
                System.Diagnostics.Process.Start(txtLogFile.Text);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnClearLogFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnClearLogFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLogFile.Text) && File.Exists(txtLogFile.Text))
            {
                mobileGateway.ClearLog();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSendPictureSms control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSendPictureSms_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPictureSmsPhoneNumber.Text))
                {
                    btnSendPictureSms.Enabled = false;

                    if (picPictureSms.Image != null)
                    {
                        // Send Picture
                        PictureSms pictureSms = PictureSms.NewInstance(new Bitmap(picPictureSms.Image), txtPictureSmsMessage.Text);
                        pictureSms.DestinationAddress = txtPictureSmsPhoneNumber.Text;
                        if (mobileGateway.Send(pictureSms))
                        {
                            MessageBox.Show("Picture SMS is sent successfully to " + pictureSms.DestinationAddress + ". Message index is  " + string.Join(",", (pictureSms.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Phone number cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnSendPictureSms.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnBrowsePictureSms control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnBrowsePictureSms_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image file (*.*)|*.*";
            openFileDialog1.FileName = string.Empty;
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;

                // Read the file content and send as vCalendar
                txtPictureSms.Text = fileName;
                PreviewPictureSms();
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkPreviewPictureSms control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkPreviewPictureSms_CheckedChanged(object sender, EventArgs e)
        {
            PreviewPictureSms();
        }

        /// <summary>
        /// Previews the picture SMS.
        /// </summary>
        private void PreviewPictureSms()
        {
            if (chkPreviewPictureSms.Checked)
            {
                if (!string.IsNullOrEmpty(txtPictureSms.Text))
                {
                    if (File.Exists(txtPictureSms.Text))
                    {
                        Bitmap bitmap = new Bitmap(txtPictureSms.Text);
                        if (!GatewayHelper.IsBlackAndWhite(bitmap))
                        {
                            bitmap = GatewayHelper.ConvertBlackAndWhite(bitmap);
                        }

                        if (bitmap.Height > 0xff || bitmap.Width > 0xff)
                        {
                            bitmap = GatewayHelper.ResizeImage(bitmap, 255, 255);
                        }

                        picPictureSms.Image = bitmap;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancelUssdSession control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnCancelUssdSession_Click(object sender, EventArgs e)
        {
            try
            {
                btnCancelUssdSession.Enabled = false;
                if (!mobileGateway.CancelUssdSession())
                {
                    // Cannot cancel the session, check mobileGateway.LastError

                }
            }
            finally
            {
                btnCancelUssdSession.Enabled = true;
            }
        }


        /// <summary>
        /// Handles the CheckedChanged event of the chkScheduled control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkScheduled_CheckedChanged(object sender, EventArgs e)
        {
            dtpScheduledDeliveryDate.Enabled = chkScheduled.Checked;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkPersistenceQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkPersistenceQueue_CheckedChanged(object sender, EventArgs e)
        {
            mobileGateway.PersistenceQueue = chkPersistenceQueue.Checked;
            txtPersistenceFolder.Enabled = chkPersistenceQueue.Checked;
            if (chkPersistenceQueue.Checked)
            {
                if (!string.IsNullOrEmpty(txtPersistenceFolder.Text))
                {
                    mobileGateway.PersistenceFolder = txtPersistenceFolder.Text;
                }
                MessageBox.Show("You can specify the persistence folder. Default to current folder if not specified.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
                MessageStatusType messageType;
                if (MessageType.TryGetValue(cboMessageType.Text, out messageType))
                {
                    List<MessageInformation> messages = null;
                    if (radAll.Checked)
                    {
                        messages = mobileGateway.GetMessages(messageType, AllStorages).FindAll((delegate(MessageInformation m) { return (m.GetType() == typeof(MmsNotification)); }));
                    }
                    else
                    {
                        messages = mobileGateway.GetMessages(messageType).FindAll((delegate(MessageInformation m) { return (m.GetType() == typeof(MmsNotification)); }));
                    }
                    List<MmsNotification> notifications = new List<MmsNotification>();
                    foreach (MessageInformation msg in messages)
                        notifications.Add(msg as MmsNotification);
                    dgdMessages.DataSource = notifications;
                    lblMessageCount.Text = messages.Count() + " message(s) found";
                }
            }
            finally
            {
                btnRetrieveMMSNotification.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnRetrieveMMSReadReport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRetrieveMMSReadReport_Click(object sender, EventArgs e)
        {
            try
            {
                btnRetrieveMMSReadReport.Enabled = false;
                MessageStatusType messageType;
                if (MessageType.TryGetValue(cboMessageType.Text, out messageType))
                {
                    List<MessageInformation> messages = null;
                    if (radAll.Checked)
                    {
                        messages = mobileGateway.GetMessages(messageType, AllStorages).FindAll((delegate(MessageInformation m) { return (m.GetType() == typeof(MmsReadReport)); }));
                    }
                    else
                    {
                        messages = mobileGateway.GetMessages(messageType).FindAll((delegate(MessageInformation m) { return (m.GetType() == typeof(MmsReadReport)); }));
                    }

                    List<MmsReadReport> reports = new List<MmsReadReport>();
                    foreach (MessageInformation msg in messages)
                        reports.Add(msg as MmsReadReport);
                    dgdMessages.DataSource = reports;
                    lblMessageCount.Text = messages.Count() + " message(s) found";
                }
            }
            finally
            {
                btnRetrieveMMSReadReport.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnRetrieveMMSDeliveryReport control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnRetrieveMMSDeliveryReport_Click(object sender, EventArgs e)
        {
            try
            {
                btnRetrieveMMSDeliveryReport.Enabled = false;
                MessageStatusType messageType;
                if (MessageType.TryGetValue(cboMessageType.Text, out messageType))
                {
                    List<MessageInformation> messages = null;
                    if (radAll.Checked)
                    {
                        messages = mobileGateway.GetMessages(messageType, AllStorages).FindAll((delegate(MessageInformation m) { return (m.GetType() == typeof(MmsDeliveryNotification)); }));
                    }
                    else
                    {
                        messages = mobileGateway.GetMessages(messageType).FindAll((delegate(MessageInformation m) { return (m.GetType() == typeof(MmsDeliveryNotification)); }));
                    }
                    List<MmsDeliveryNotification> reports = new List<MmsDeliveryNotification>();
                    foreach (MessageInformation msg in messages)
                        reports.Add(msg as MmsDeliveryNotification);
                    dgdMessages.DataSource = reports;
                    lblMessageCount.Text = messages.Count() + " message(s) found";
                }
            }
            finally
            {
                btnRetrieveMMSDeliveryReport.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Leave event of the txtPersistenceFolder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtPersistenceFolder_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPersistenceFolder.Text))
            {
                if (Directory.Exists(txtPersistenceFolder.Text))
                {
                    mobileGateway.PersistenceFolder = txtPersistenceFolder.Text;
                }
                else
                {
                    MessageBox.Show("Persistence folder does not exist", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSendQRCodeSms control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnSendQRCodeSms_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtQRCodeDestPhoneNo.Text))
                {
                    if (string.IsNullOrEmpty(txtQRCodeMessage.Text))
                    {
                        MessageBox.Show("Message cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    btnSendQRCodeSms.Enabled = false;
                    QRCodeSms qrCodeSms = QRCodeSms.NewInstance(txtQRCodeMessage.Text, (int)numQRCodeWidth.Value, (int)numQRCodeHeight.Value);
                    qrCodeSms.DestinationAddress = txtQRCodeDestPhoneNo.Text;

                    if (mobileGateway.Send(qrCodeSms))
                    {
                        MessageBox.Show("QR Code SMS is sent successfully to " + qrCodeSms.DestinationAddress + ". Message index is  " + string.Join(",", (qrCodeSms.Indexes.ConvertAll<string>(delegate(int i) { return i.ToString(); })).ToArray()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                else
                {
                    MessageBox.Show("Phone number cannot be empty", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnSendQRCodeSms.Enabled = true;
            }
        }

        private void chkEncodeUssdCommand_CheckedChanged(object sender, EventArgs e)
        {
            // Encode/decode USSD command and response
            mobileGateway.EncodedUssdCommand = chkEncodeUssdCommand.Checked;
        }

        private void chkEnableColp_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableColp.Checked)
            {
                mobileGateway.EnableCOLP();
            }
            else
            {
                mobileGateway.DisableCOLP();
            }
        }

        private void chkEnableOutgoingCallEvent_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnableOutgoingCallEvent.Checked)
            {
                mobileGateway.CallDialled += new OutgoingCallEventHandler(OnCallDialled);
            }
            else
            {
                mobileGateway.CallDialled -= OnCallDialled;
            }
        }

        /// <summary>
        /// Called when call is dialled
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Mobile.Event.OutgoingCallEventArgs"/> instance containing the event data.</param>
        private void OnCallDialled(object sender, OutgoingCallEventArgs e)
        {
            txtOutgoingCallEvent.BeginInvoke(displayCallEventLog, e);
        }

        /// <summary>
        /// Shows the call event log.
        /// </summary>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Mobile.Event.OutgoingCallEventArgs"/> instance containing the event data.</param>
        private void ShowCallEventLog(OutgoingCallEventArgs e)
        {
            txtOutgoingCallEvent.AppendText("Dialled " + e.CallInformation.Number);
            txtOutgoingCallEvent.AppendText("\r\n");
            txtOutgoingCallEvent.AppendText("Number type - " + e.CallInformation.NumberType);
            txtOutgoingCallEvent.AppendText("\r\n\r\n");
        }

        /// <summary>
        /// Handles the Click event of the btnStatus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnStatus_Click(object sender, EventArgs e)
        {
            if (mobileGateway.ValidateConnection())
            {
                MessageBox.Show("Gateway is connected", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Gateway is disconnected", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.twit88.com");
        }


        /*
        private Sms CreateSms(string content, MessageQueuePriority priority)
        {
            Sms sms = Sms.NewInstance();
            sms.DestinationAddress = "123456789";
            sms.Content = content;
            sms.QueuePriority = priority;
            return sms;
        }
        private void TestSendWithPriority()
        {
            mobileGateway.SendToQueue(CreateSms("I1", MessageQueuePriority.High));
            mobileGateway.SendToQueue(CreateSms("N2", MessageQueuePriority.Normal));
            mobileGateway.SendToQueue(CreateSms("N3", MessageQueuePriority.Normal));
            mobileGateway.SendToQueue(CreateSms("I2", MessageQueuePriority.High));
            mobileGateway.SendToQueue(CreateSms("N4", MessageQueuePriority.Normal));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestSendWithPriority();
        }
        */

    }
}
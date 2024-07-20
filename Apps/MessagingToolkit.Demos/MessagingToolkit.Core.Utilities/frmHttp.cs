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
using System.IO;
using System.Runtime.Remoting.Messaging;

using MessagingToolkit.Core.Mobile.Http;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Mobile.Http.Event;
using MessagingToolkit.Core.Log;


namespace MessagingToolkit.Core.Utilities
{
    /// <summary>
    /// HTTP gateway demo form.
    /// </summary>
    public partial class frmHttp : Form
    {

        /// <summary>
        /// HTTP gateway interface.
        /// </summary>
        private IHttpGateway httpGateway = HttpGatewayFactory.Default;

        /// <summary>
        /// Call back delegate to add chat room message
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        private delegate void AddChatRoomMessageCallback(string text, Color color);

        /// <summary>
        /// Delegate to display the messages belonging to the same thread
        /// </summary>
        /// <param name="threadId">The thread identifier.</param>
        private delegate void OnMessageClicked(string threadId);

        /// <summary>
        /// Delegate to display the message log
        /// </summary>
        /// <param name="text">The text.</param>
        private delegate void DisplayMessageLog(string text);

        /// <summary>
        /// On message clicked event.
        /// </summary>
        private OnMessageClicked onMessageClicked;

        /// <summary>
        /// Messages for the current selected thread
        /// </summary>
        private List<DeviceMessage> currentThreadMessages;

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
        /// Initializes a new instance of the <see cref="frmHttp"/> class.
        /// </summary>
        public frmHttp()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the frmHttp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void frmHttp_Load(object sender, EventArgs e)
        {
            onMessageClicked = new OnMessageClicked(this.HandleMessageClicked);

            // Queue priority
            cboQueuePriority.Items.AddRange(QueuePriority.Keys.ToArray());
            cboQueuePriority.SelectedIndex = 1;

            // Set the cursor focus
            this.ActiveControl = txtIPAddress;

        }

        /// <summary>
        /// Handles the Click event of the btnConnect control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                btnConnect.Enabled = false;
                SetupDevice();
                DisplayDeviceInfo();

                // Log file location
                if (!HttpGatewayFactory.IsDefaultOrNull(httpGateway))
                    txtLogFile.Text = httpGateway.LogFile;
            }
            finally
            {
                btnConnect.Enabled = true;
            }
        }

        /// <summary>
        /// Displays the device information.
        /// </summary>
        private void DisplayDeviceInfo()
        {
            try
            {
                if (HttpGatewayFactory.IsDefaultOrNull(httpGateway)) return;

                // Retrieve the device network information
                DeviceNetworkInformation networkInfo = httpGateway.DeviceNetworkInformation;

                // You can check the last error to see if there is any error
                if (httpGateway.LastError != null)
                {
                    MessageBox.Show(httpGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Retrieve the device battery information
                DeviceBatteryInformation batteryInfo = httpGateway.DeviceBatteryInformation;

                // You can check the last error to see if there is any error
                if (httpGateway.LastError != null)
                {
                    MessageBox.Show(httpGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                // Display GSM cell location information
                if (networkInfo.GsmCellLocation != null)
                {
                    txtCellId.Text = networkInfo.GsmCellLocation.Cid.ToString();
                    txtLac.Text = networkInfo.GsmCellLocation.Lac.ToString();
                    txtPsc.Text = networkInfo.GsmCellLocation.Psc.ToString();
                }
                // Display CDMA cell location information
                if (networkInfo.CdmaCellLocation != null)
                {
                    txtBaseStationId.Text = networkInfo.CdmaCellLocation.BaseStationdId.ToString();
                    txtBaseStationLatitude.Text = networkInfo.CdmaCellLocation.BaseStationLatitude.ToString();
                    txtBaseStationLongitude.Text = networkInfo.CdmaCellLocation.BaseStationLongitude.ToString();
                    txtNetworkId.Text = networkInfo.CdmaCellLocation.NetworkId.ToString();
                    txtSystemId.Text = networkInfo.CdmaCellLocation.SystemId.ToString();
                }
                txtCellLocationType.Text = networkInfo.CellLocationType;
                txtConnectionType.Text = networkInfo.ConnectionType;
                txtDeviceId.Text = networkInfo.DeviceId;
                txtDeviceSoftwareVersion.Text = networkInfo.DeviceSoftwareVersion;
                txtDeviceIPAddress.Text = networkInfo.IPAddress;
                txtVoiceMailNumber.Text = networkInfo.VoiceMailNumber;
                txtSubscriberId.Text = networkInfo.SubscriberId;
                txtNetworkCountryIso.Text = networkInfo.NetworkCountryIso;
                txtNetworkOperator.Text = networkInfo.NetworkOperator;
                txtNetworkOperatorName.Text = networkInfo.NetworkOperatorName;
                txtNetworkType.Text = networkInfo.NetworkType;
                txtPhoneNumber.Text = networkInfo.PhoneNumber;
                txtPhoneType.Text = networkInfo.PhoneType;
                txtSimCountryISO.Text = networkInfo.SimCountryIso;
                txtSimOperator.Text = networkInfo.SimOperator;
                txtSimOperatorName.Text = networkInfo.SimOperatorName;
                txtSimSerialNumber.Text = networkInfo.SimSerialNumber;
                txtSimState.Text = networkInfo.SimState;
                chkIsNetworkRoaming.Checked = networkInfo.IsNetworkRoaming;
                chkIsConnected.Checked = networkInfo.IsConnected;

                if (networkInfo.SignalStrength != null)
                {
                    txtCdmaRssi.Text = networkInfo.SignalStrength.CdmaDbm.ToString();
                    txtCdmaEcIo.Text = networkInfo.SignalStrength.CdmaEcio.ToString();
                    txtEvdoRssi.Text = networkInfo.SignalStrength.EvdoDbm.ToString();
                    txtEvdoEcIo.Text = networkInfo.SignalStrength.EvdoEcio.ToString();
                    txtEvdoSnr.Text = networkInfo.SignalStrength.EvdoSnr.ToString();
                    txtGsmBitErrorRate.Text = networkInfo.SignalStrength.GsmBitErrorRate.ToString();
                    txtGsmSignalStrength.Text = networkInfo.SignalStrength.GsmSignalStrength.ToString();
                    chkIsGsm.Checked = networkInfo.SignalStrength.IsGsm;
                }

                if (batteryInfo != null)
                {
                    txtHealth.Text = batteryInfo.Health;
                    txtTechnology.Text = batteryInfo.Technology;
                    txtStatus.Text = batteryInfo.Status;
                    txtPlugType.Text = batteryInfo.PlugType;
                    txtLevel.Text = batteryInfo.Level.ToString();
                    chkIsPresent.Checked = batteryInfo.IsPresent;
                    txtTemperature.Text = (batteryInfo.Temperature / 10).ToString();
                    txtVoltage.Text = batteryInfo.Voltage.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Setups the device by creating a new instance of the HTTP gateway
        /// </summary>
        /// <returns></returns>
        private bool SetupDevice()
        {
            if (string.IsNullOrEmpty(txtIPAddress.Text))
            {
                MessageBox.Show("Please enter an IP address", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtIPAddress.Focus();
                return false;
            }

            HttpGatewayConfiguration config = HttpGatewayConfiguration.NewInstance();
            config.IPAddress = txtIPAddress.Text.Trim();

            if (!string.IsNullOrEmpty(txtUserName.Text))
            {
                config.UserName = txtUserName.Text.Trim();
            }

            if (!string.IsNullOrEmpty(txtPassword.Text))
            {
                config.Password = txtPassword.Text.Trim();
            }

            if (!string.IsNullOrEmpty(txtPort.Text))
            {
                try
                {
                    config.Port = Convert.ToInt32(txtPort.Text);
                }
                catch
                {
                    MessageBox.Show("Not a valid port", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Default to verbose by default
            config.LogLevel = LogLevel.Verbose;

            // Set the log file name without the date
            config.LogNameFormat = LogNameFormat.Name;

            // Set a different log file prefix and path
            //config.LogFile = "mylog";
            //config.LogLocation = @"c:\temp";


            try
            {

                // Create the gateway for mobile
                MessageGateway<IHttpGateway, HttpGatewayConfiguration> messageGateway = MessageGateway<IHttpGateway, HttpGatewayConfiguration>.NewInstance();
                httpGateway = messageGateway.Find(config);
                if (httpGateway == null)
                {
                    MessageBox.Show("Unable to create HTTP gateway. Check the log file.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Events handling
                httpGateway.MessageReceived += httpGateway_MessageReceived;
                httpGateway.MessageSending += httpGateway_MessageSending;
                httpGateway.MessageSendingFailed += httpGateway_MessageSendingFailed;
                httpGateway.MessageDelivered += httpGateway_MessageDelivered;
                httpGateway.MessageSent += httpGateway_MessageSent;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private void httpGateway_MessageReceived(object sender, NewMessageReceivedEventArgs e)
        {
            DeviceMessage message = e.Message;
            AppendMessageLog(string.Format("Received message:\r\n{0}", message.ToString()));
        }


        private void httpGateway_MessageSent(object sender, MessageEventArgs e)
        {
            PostMessage message = e.Message as PostMessage;
            AppendMessageLog(string.Format("Device SENT the message to [{0}]. Library unique identifer is [{1}], Device message id is [{2}]. ", message.To, message.Identifier, message.MessageIdFromDevice));
        }

        private void httpGateway_MessageDelivered(object sender, MessageEventArgs e)
        {
            PostMessage message = e.Message as PostMessage;
            AppendMessageLog(string.Format("Device DELIVERED the message to [{0}]. Library unique identifer is [{1}], Device message id is [{2}]. ", message.To, message.Identifier, message.MessageIdFromDevice));
        }

        private void httpGateway_MessageSendingFailed(object sender, MessageErrorEventArgs e)
        {
            PostMessage message = e.Message as PostMessage;
            AppendMessageLog(string.Format("Device FAILED to send message to [{0}]. Library unique identifer is [{1}], Device message id is [{2}]. {3}. ", message.To, message.Identifier, message.MessageIdFromDevice, e.Error.Message));
        }

        private void httpGateway_MessageSending(object sender, MessageEventArgs e)
        {
            PostMessage message = e.Message as PostMessage;
            AppendMessageLog(string.Format("SENDING message to [{0}]. Library unique identifer is [{1}]", message.To, message.Identifier));
        }

        /// <summary>
        /// Handles the Click event of the tabMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void tabMain_Click(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            if (tc.SelectedTab == tabAbout)
            {
                Assembly assembly = Assembly.GetAssembly(httpGateway.GetType());
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
                if (httpGateway.License.Valid)
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
        /// Displays the message threads.
        /// </summary>
        private void DisplayMessageThreads()
        {
            if (HttpGatewayFactory.IsDefaultOrNull(httpGateway)) return;

            lvwMessageThreads.Items.Clear();

            // This will retrieve all messages
            List<DeviceMessage> messages = httpGateway.GetMessages();
                                   
            // --- This will retrieve the first 50 messsages ----
            //GetMessageQuery query = new GetMessageQuery();
            //query.Offset = 0;       // Start from position 0;
            //query.RowCount = 50;    // Number of messages to return
            //List<DeviceMessage> messages = httpGateway.GetMessages(query);            


            // Check last error if you want to see if there is any error - Optional 
            if (httpGateway.LastError != null)
            {
                MessageBox.Show(httpGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Display the info for each message thread
            List<int> threadIds = messages.Select(m => m.ThreadId).Distinct().ToList<int>();
            foreach (int threadId in threadIds)
            {
                var message = messages.First(m => m.ThreadId == threadId);
                lvwMessageThreads.Items.Add(new ListViewItem(new string[] { message.ThreadId.ToString(), message.Id.ToString(), message.Sender, message.TimeStampString, message.Message }));
            }
        }


        /**
        private void DeleteMessage()
        {
            if (HttpGatewayFactory.IsDefaultOrNull(httpGateway)) return;

            if (lvwMessageThreads.SelectedItems.Count == 0) return;

            string id = lvwMessageThreads.SelectedItems[1].Text;

            if (!string.IsNullOrEmpty(id))
            {
               // Delete the message
            }
        }
        **/

        /// <summary>
        /// Handles the FormClosing event of the frmHttp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void frmHttp_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If not null or default
            if (!HttpGatewayFactory.IsDefaultOrNull(httpGateway))
            {
                httpGateway.Disconnect();
                httpGateway.Dispose();
                httpGateway = null;
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the lvwMessageThreads control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void lvwMessageThreads_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwMessageThreads.SelectedItems.Count == 0) return;

            string threadId = lvwMessageThreads.SelectedItems[0].Text;
            if (!string.IsNullOrEmpty(threadId))
            {
                rtbChatRoom.Text = string.Empty;
                onMessageClicked.BeginInvoke(threadId, AsyncCallback, null);
            }
        }


        private void AsyncCallback(IAsyncResult result)
        {
            var asyncResult = (AsyncResult)result;
            var callback = (OnMessageClicked)asyncResult.AsyncDelegate;
            callback.EndInvoke(asyncResult);
        }

        private void HandleMessageClicked(string threadId)
        {

            // Display messages for the thread
            GetMessageQuery query = new GetMessageQuery();
            query.ThreadId = Convert.ToInt32(threadId);

            // Get messages for this thread
            this.currentThreadMessages = httpGateway.GetMessages(query);

            // Display all the messages belonging to this thread
            foreach (var message in this.currentThreadMessages)
            {
                if (message.Sender.Equals("me", StringComparison.InvariantCultureIgnoreCase))
                {
                    AddChatRoomMessage(message.Sender + "\r\n" + message.Message + "\r\n" + message.TimeStamp.ToString() + "\r\n\r\n", Color.Blue);
                }
                else
                {
                    AddChatRoomMessage(message.Sender + "\r\n" + message.Message + "\r\n" + message.TimeStamp.ToString() + "\r\n\r\n", Color.DarkGreen);
                }

            }
        }

        /// <summary>
        /// Adds the chat room message.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        public void AddChatRoomMessage(string text, Color color)
        {
            if (rtbChatRoom.InvokeRequired)
            {
                AddChatRoomMessageCallback cb = new AddChatRoomMessageCallback(AddChatRoomMessage);
                this.Invoke(cb, new object[] { text, color });
            }
            else
            {
                rtbChatRoom.SelectionStart = rtbChatRoom.TextLength;
                rtbChatRoom.SelectionLength = 0;

                rtbChatRoom.SelectionColor = color;
                rtbChatRoom.AppendText(text);
                rtbChatRoom.SelectionColor = rtbChatRoom.ForeColor;
            }
        }


        /// <summary>
        /// Appends the message log.
        /// </summary>
        /// <param name="logText">The log text.</param>
        private void AppendMessageLog(string logText)
        {
            if (txtLog.InvokeRequired)
            {
                DisplayMessageLog cb = new DisplayMessageLog(AppendMessageLog);
                this.Invoke(cb, new object[] { logText });
            }
            else
            {
                DateTime dt = DateTime.Now;
                txtLog.AppendText(dt.ToString("yyyy.MM.dd") + "-" + dt.ToString("hh.mm.ss") + ": " + logText + "\r\n");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSendMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                btnSendMessage.Enabled = false;
                if (HttpGatewayFactory.IsDefaultOrNull(httpGateway)) return;

                if (!string.IsNullOrEmpty(txtMessage.Text) && currentThreadMessages != null && currentThreadMessages.Count > 0)
                {
                    // Create the message instance
                    PostMessage message = PostMessage.NewInstance();
                    message.Message = txtMessage.Text;
                    // message.Slot = "1";     // Specify the SIM slot to use
                    
                    DeviceMessage deviceMessage = currentThreadMessages[0];

                    // NOTE: This can be a contact name available in your phone
                    message.To = deviceMessage.PhoneNumber;

                    // For testing only- delayed message sending
                    // message.ScheduledDeliveryDate = DateTime.Now.AddDays(1);

                    if (chkSendToQueue.Checked)
                    {
                        // Message queue priority
                        MessageQueuePriority priority;
                        if (QueuePriority.TryGetValue(cboQueuePriority.Text, out priority))
                        {
                            message.QueuePriority = priority;
                        }

                        // Send to queue
                        if (httpGateway.SendToQueue(message))
                        {
                            AppendMessageLog(string.Format("Successfully QUEUED the message. Library unique identifier is [{0}].", message.Identifier));
                        }
                        else
                        {
                            MessageBox.Show(httpGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        // Send the message
                        if (!httpGateway.Send(message))
                        {
                            MessageBox.Show(httpGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            AppendMessageLog(string.Format("Message is posted to device. Library unique identifer is [{0}], Device message id is [{1}]. ", message.Identifier, message.MessageIdFromDevice));

                        }
                    }
                }
            }
            finally
            {
                btnSendMessage.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnRetrieveMessages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnRetrieveMessages_Click(object sender, EventArgs e)
        {
            try
            {
                btnRetrieveMessages.Enabled = false;
                DisplayMessageThreads();
            }
            finally
            {
                btnRetrieveMessages.Enabled = true;
            }
        }


        private void chkEnableQueue_CheckedChanged(object sender, EventArgs e)
        {
            if (HttpGatewayFactory.IsDefaultOrNull(httpGateway))
            {
                MessageBox.Show("Make sure device is connected before selecting this option", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                chkEnableQueue.CheckedChanged -= chkEnableQueue_CheckedChanged;
                chkEnableQueue.Checked = true;
                chkEnableQueue.CheckedChanged += chkEnableQueue_CheckedChanged;
                return;
            }

            if (chkEnableQueue.Checked)
            {
                MessageBox.Show("Messages will be queued and sent out", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Messages will be queued but NOT sent out", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            httpGateway.IsMessageQueueEnabled = chkEnableQueue.Checked;
        }

        private void chkSendToQueue_CheckedChanged(object sender, EventArgs e)
        {
            if (HttpGatewayFactory.IsDefaultOrNull(httpGateway))
            {
                MessageBox.Show("Make sure device is connected before selecting this option", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                chkSendToQueue.CheckedChanged -= chkSendToQueue_CheckedChanged;
                chkSendToQueue.Checked = false;
                chkSendToQueue.CheckedChanged += chkSendToQueue_CheckedChanged;
                return;
            }

            cboQueuePriority.Enabled = chkSendToQueue.Checked;
        }

        private void chkPersistenceQueue_CheckedChanged(object sender, EventArgs e)
        {
            if (HttpGatewayFactory.IsDefaultOrNull(httpGateway))
            {
                MessageBox.Show("Make sure device is connected before selecting this option", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                chkPersistenceQueue.CheckedChanged -= chkPersistenceQueue_CheckedChanged;
                chkPersistenceQueue.Checked = false;
                chkPersistenceQueue.CheckedChanged += chkPersistenceQueue_CheckedChanged;
                return;
            }

            httpGateway.PersistenceQueue = chkPersistenceQueue.Checked;
            txtPersistenceFolder.Enabled = chkPersistenceQueue.Checked;
            if (chkPersistenceQueue.Checked)
            {
                if (!string.IsNullOrEmpty(txtPersistenceFolder.Text))
                {
                    httpGateway.PersistenceFolder = txtPersistenceFolder.Text;
                }
                MessageBox.Show("You can specify the persistence folder. Default to current folder if not specified.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnRefreshQueue_Click(object sender, EventArgs e)
        {
            lblMessageQueueCount.Text = "Messages in Queue: " + httpGateway.GetQueueCount();
        }

        private void btnClearQueue_Click(object sender, EventArgs e)
        {
            httpGateway.ClearQueue();
        }

        private void btnViewLogFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLogFile.Text) && File.Exists(txtLogFile.Text))
            {
                System.Diagnostics.Process.Start(txtLogFile.Text);
            }
        }

        private void btnClearLogFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLogFile.Text) && File.Exists(txtLogFile.Text))
            {
                httpGateway.ClearLog();
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkListenNewMessages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void chkListenNewMessages_CheckedChanged(object sender, EventArgs e)
        {
            if (HttpGatewayFactory.IsDefaultOrNull(httpGateway))
            {
                MessageBox.Show("Make sure device is connected before selecting this option", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                chkListenNewMessages.CheckedChanged -= chkListenNewMessages_CheckedChanged;
                chkListenNewMessages.Checked = false;
                chkListenNewMessages.CheckedChanged += chkListenNewMessages_CheckedChanged;
                return;
            }

            httpGateway.PollNewMessages = chkListenNewMessages.Checked;
            if (httpGateway.PollNewMessages)
            {
                chkDeleteAfterReceive.Enabled = true;
                MessageBox.Show("New incoming messages will be retrieved and logged.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                chkDeleteAfterReceive.Checked = false;
                chkDeleteAfterReceive.Enabled = false;
            }
        }

        private void chkDeleteAfterReceive_CheckedChanged(object sender, EventArgs e)
        {
            if (HttpGatewayFactory.IsDefaultOrNull(httpGateway))
            {
                MessageBox.Show("Make sure device is connected before selecting this option", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                chkDeleteAfterReceive.CheckedChanged -= chkDeleteAfterReceive_CheckedChanged;
                chkDeleteAfterReceive.Checked = false;
                chkDeleteAfterReceive.CheckedChanged += chkDeleteAfterReceive_CheckedChanged;
                return;
            }
            httpGateway.Configuration.DeleteReceivedMessage = chkDeleteAfterReceive.Checked;
            if (httpGateway.Configuration.DeleteReceivedMessage)
            {
                MessageBox.Show("New incoming messages will be deleted.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        /// <summary>
        /// Handles the Click event of the btnDeleteMessages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnDeleteMessages_Click(object sender, EventArgs e)
        {
            try
            {
                btnDeleteMessages.Enabled = false;
                if (HttpGatewayFactory.IsDefaultOrNull(httpGateway)) return;

                if (lvwMessageThreads.SelectedItems.Count == 0) return;

                string threadId = lvwMessageThreads.SelectedItems[0].Text;
                if (!string.IsNullOrEmpty(threadId))
                {
                    int rowCount = httpGateway.DeleteMessage(threadId);
                    if (rowCount >= 0)
                    {
                        MessageBox.Show(string.Format("Deleted {0} messages for this message thread.", rowCount), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(httpGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                btnDeleteMessages.Enabled = true;
            }
        }

        private void chkDebugMode_CheckedChanged(object sender, EventArgs e)
        {
            if (HttpGatewayFactory.IsDefaultOrNull(httpGateway))
            {
                MessageBox.Show("Make sure device is connected before selecting this option", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                chkDebugMode.CheckedChanged -= chkDebugMode_CheckedChanged;
                chkDebugMode.Checked = false;
                chkDebugMode.CheckedChanged += chkDebugMode_CheckedChanged;
                return;
            }

            httpGateway.Configuration.DebugMode = chkDebugMode.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.twit88.com");
        }
    }
}

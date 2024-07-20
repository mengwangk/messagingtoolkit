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
using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Service;

using MessagingToolkit.BulkGateway.Properties;

namespace MessagingToolkit.BulkGateway
{
    /// <summary>
    /// Main form
    /// </summary>
    public partial class frmMain : Form
    {
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
        /// Message encoding
        /// </summary>
        private Dictionary<string, MessageDataCodingScheme> MessageEncoding =
            new Dictionary<string, MessageDataCodingScheme>
            {
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

        // Log level
        private Dictionary<string, LogLevel> LoggingLevel =
            new Dictionary<string, LogLevel>
            {
                {"Error", LogLevel.Error},
                {"Warn", LogLevel.Warn},
                {"Info", LogLevel.Info},
                {"Verbose", LogLevel.Verbose}                                                 
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


        // Constants for load balancer
        private const string DefaultLoadBalancer = "Default Load Balancer";
        private const string RoundRobinLoadBalancer = "Round Robin Load Balancer";

        // Constants for router
        private const string DefaultRouter = "Default Router";
        private const string NumberPrefixRouter = "Number Prefix Router";

           
        // Message gateway service
        private MessageGatewayService messageGatewayService;


        /// <summary>
        /// Message group
        /// </summary>
        private const string BulkGatewayGroup = "bulk";

        
        /// <summary>
        /// License file
        /// </summary>
        private const string LicenseFile = "license.lic";

        /// <summary>
        /// Message sending thread
        /// </summary>
        private Thread messageSender;

        /// <summary>
        /// Thread to add gateway
        /// </summary>
        private Thread gatewayAdder;

        /// <summary>
        /// Thread to remove gateway
        /// </summary>
        private Thread gatewayRemover;

        /// <summary>
        /// Thread to check gateway status
        /// </summary>
        private Thread gatewayStatus;

        /// <summary>
        /// Exit flag
        /// </summary>
        private bool exitApplication;
      
        /// <summary>
        /// License key
        /// </summary>
        private string licenseKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="frmMain"/> class.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Load event of the frmMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void frmMain_Load(object sender, EventArgs e)
        {           
            // Add the port
            string[] portNames = SerialPort.GetPortNames();
            var sortedList = portNames.OrderBy(port => Convert.ToInt32(port.Replace("COM", string.Empty)));
            foreach (string port in sortedList)
            {
                if (!cboPort.Items.Contains(port))
                    cboPort.Items.Add(port);
            }

            // Add baud rate
            foreach (string baudRate in Enum.GetNames(typeof(PortBaudRate)))
            {
                cboBaudRate.Items.Add((int)Enum.Parse(typeof(PortBaudRate), baudRate));
            }

            // Add data bits
            foreach (string dataBit in Enum.GetNames(typeof(PortDataBits)))
            {
                cboDataBits.Items.Add((int)Enum.Parse(typeof(PortDataBits), dataBit));
            }

            // Add parity            
            cboParity.Items.AddRange(Parity.Keys.ToArray());

            // Add stop bits         
            cboStopBits.Items.AddRange(StopBits.Keys.ToArray());

            // Add message encoding            
            cboMessageEncoding.Items.AddRange(MessageEncoding.Keys.ToArray());
            cboMessageEncoding.SelectedIndex = 0;
                       
            // Long message option
            cboLongMessage.Items.AddRange(MessageSplit.Keys.ToArray());

            // Message validity period
            cboValidityPeriod.Items.AddRange(ValidityPeriod.Keys.ToArray());


            // Queue priority
            cboQueuePriority.Items.AddRange(QueuePriority.Keys.ToArray());
            cboQueuePriority.SelectedIndex = 1;


            // Logging level
            cboLoggingLevel.Items.AddRange(LoggingLevel.Keys.ToArray());
            cboLoggingLevel.SelectedIndex = 0;

            // Load balancer
            cboLoadBalancer.Items.Add(DefaultLoadBalancer);
            cboLoadBalancer.Items.Add(RoundRobinLoadBalancer);
            cboLoadBalancer.SelectedIndex = 0;

            // Router
            cboRouter.Items.Add(DefaultRouter);
            cboRouter.Items.Add(NumberPrefixRouter);
            cboRouter.SelectedIndex = 0;
            
            // Set the default values
            ResetGatewaySettings();

            // Initialize the application
            InitializeApp();            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to exit?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                exitApplication = true;
                Application.Exit();
            }
        }

        private void chkWapPush_CheckedChanged(object sender, EventArgs e)
        {
            txtWapPushUrl.Enabled = chkWapPush.Checked;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            tabSubMain.SelectedIndex = 0;
            grpBoxGatewaySettings.Enabled = true;
            txtGatewayId.Focus();
            toolStripStatus.Text = "Enter gateway settings and click Add Gateway..";
        }


        private void btnAddGateway_Click(object sender, EventArgs e)
        {
            if (messageGatewayService.Gateways.Count() == 2)
            {
                // Need to check for licensing
                IMobileGateway mobileGateway = (IMobileGateway)messageGatewayService.Gateways[0];
                if (!mobileGateway.License.Valid) 
                {
                    toolStripStatus.Text = "Community copy can only have a maximum of 2 gateways";
                    return;
                }
            }

            if (chkPersistenceQueue.Checked && string.IsNullOrEmpty(txtPersistenceFolder.Text))
            {
                MessageBox.Show("Please specify a persistence folder", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPersistenceFolder.Focus();
                return;
            }

            if (gatewayAdder == null || !gatewayAdder.IsAlive)
            {
                gatewayAdder = new Thread(new ThreadStart(this.AddGateway));
                gatewayAdder.Start();
            }
            else
            {
                toolStripStatus.Text = "Gateway adding is in progress. Try again later";
            }
        }

        private void btnCancelAddGateway_Click(object sender, EventArgs e)
        {
            ResetGatewaySettings();
            grpBoxGatewaySettings.Enabled = false;
        }

        private void ResetGatewaySettings()
        {
            cboPort.SelectedIndex = 0;
            cboBaudRate.Text = "115200";
            cboDataBits.Text = "8";
            cboParity.SelectedIndex = 0;
            cboStopBits.SelectedIndex = 0;
            cboLongMessage.SelectedIndex = 2;
            cboValidityPeriod.SelectedIndex = 0;
            txtGatewayId.Text  = string.Empty;
            txtPin.Text = string.Empty;
            txtModelConfig.Text = string.Empty;
            txtNumberPrefix.Text = string.Empty;

            MobileGatewayConfiguration config = MobileGatewayConfiguration.NewInstance();
            updSendRetries.Value = config.SendRetries;
            updSendWaitInterval.Value = config.SendWaitInterval;
        }

        private void InitializeApp()
        {
            // Create the gateway service instance
            messageGatewayService = MessageGatewayService.NewInstance();      
            
            // Set to false
            frmMain.CheckForIllegalCrossThreadCalls = false;

            exitApplication = false;
                        
            if (gatewayStatus == null || !gatewayStatus.IsAlive)
            {
                gatewayStatus = new Thread(new ThreadStart(this.CheckGatewayStatus));
                gatewayStatus.Start();
            }

            // Check for license key
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string licenseFile = currentDirectory + Path.DirectorySeparatorChar + LicenseFile;
            if (File.Exists(licenseFile))
            {
                string fileContent = File.ReadAllText(licenseFile, Encoding.ASCII);
                fileContent = fileContent.Trim();
                this.licenseKey = fileContent;
            }
            else
            {
                this.licenseKey = string.Empty;
            }
        }

        private void ShutdownApp()
        {
            messageGatewayService.Shutdown();
            messageGatewayService = null;
            try
            {
                if (gatewayStatus != null && gatewayStatus.IsAlive)
                {
                    //gatewayStatus.Interrupt();
                    //if (!gatewayStatus.Join(10))
                    //{
                        gatewayStatus.Abort();
                    //}
                    gatewayStatus = null;
                }
            }
            catch (Exception e) { }

            try
            {
                if (gatewayAdder != null && gatewayAdder.IsAlive)
                {
                    //gatewayAdder.Interrupt();
                    //if (!gatewayAdder.Join(10))
                    //{
                        gatewayAdder.Abort();
                    //}
                    gatewayAdder = null;
                }
            }
            catch (Exception e) { }

            try
            {
                if (gatewayRemover != null && gatewayRemover.IsAlive)
                {
                    //gatewayRemover.Interrupt();
                    //if (!gatewayRemover.Join(10))
                    //{
                        gatewayRemover.Abort();
                    //}
                    gatewayRemover = null;
                }
            }
            catch (Exception e) { }

            try
            {
                if (messageSender != null && messageSender.IsAlive)
                {
                    //messageSender.Interrupt();
                    //if (!messageSender.Join(10))
                    //{
                        messageSender.Abort();
                    //}
                    messageSender = null;
                }
            }
            catch (Exception e) { }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetGatewaySettings();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            ShutdownApp();
        }

        private void OnMessageSending(object sender, MessageEventArgs e)
        {
            Sms sms = (Sms)e.Message;
            string msg = "Sending message to " + sms.DestinationAddress;
            toolStripMessage.Text = msg;
            Logger.LogThis(msg, LogLevel.Verbose);
        }

        private void OnMessageSent(object sender, MessageEventArgs e)
        {
            Sms sms = (Sms)e.Message;
            string msg = "Message is sent to " + sms.DestinationAddress + " using gateway " + sms.GatewayId;
            toolStripMessage.Text = msg;
            Logger.LogThis(msg, LogLevel.Info);
        }

        private void OnMessageFailed(object sender, MessageErrorEventArgs e)
        {
            Sms sms = (Sms)e.Message;
            string msg = "Failed to send message to " + sms.DestinationAddress + " using gateway " + sms.GatewayId;
            toolStripMessage.Text = msg;
            Logger.LogThis(msg, LogLevel.Error);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MessageInformation sms = (MessageInformation)e.Message;
            string msg = "Received message from " + sms.PhoneNumber + ". Content: " + sms.Content;
            Logger.LogThis(msg, LogLevel.Info);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                btnRemove.Enabled = false;
                ListView.SelectedListViewItemCollection selectedItems = lstGateways.SelectedItems;
                if (selectedItems.Count == 0)
                {
                    toolStripStatus.Text = "No gateway is selected";
                    return;
                }
                string gatewayId = selectedItems[0].Text;
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove gateway " + gatewayId + " ?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    if (gatewayRemover == null || !gatewayRemover.IsAlive)
                    {
                        gatewayRemover = new Thread(new ThreadStart(this.RemoveGateway));
                        gatewayRemover.Start();
                    }
                    else
                    {
                        toolStripStatus.Text = "Gateway removal is in progress. Try again later";
                    }                  
                }
            }
            catch (Exception ex)
            {
                toolStripStatus.Text = ex.Message;
            }
            finally
            {
                btnRemove.Enabled = true;
            }
        }

        private void btnStartSending_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMessage.Text))
            {
                toolStripStatus.Text = "Message content cannot be blank";
                return;
            }

            if (chkWapPush.Checked && string.IsNullOrEmpty(txtWapPushUrl.Text))
            {
                toolStripStatus.Text = "WAP Push URL cannot be blank";
                return;
            }

            if (lstPhoneBook.Items.Count == 0)
            {
                toolStripStatus.Text = "Phone book is empty";
                return;
            }

            if (messageSender != null && messageSender.IsAlive)
            {
                toolStripStatus.Text = "Messaging sending is in progress. Please try again later";
                return;
            }
            try
            {
                btnStartSending.Enabled = false;
                messageSender = new Thread(new ThreadStart(this.SendMessage));
                messageSender.Start();
                toolStripStatus.Text = "Message sending process is started";
            }
            catch (Exception ex)
            {
                toolStripStatus.Text = ex.Message;
            }
            finally
            {
                btnStartSending.Enabled = true;
            }
        }

        private void txtPhoneBookEntry_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddPhoneBookEntry();
            }
        }

        private void btnAddPhoneBookEntry_Click(object sender, EventArgs e)
        {
            AddPhoneBookEntry();
        }

        private void AddPhoneBookEntry()
        {
            if (!string.IsNullOrEmpty(txtPhoneBookEntry.Text))
            {
                // Check if entry already exists
                string number = txtPhoneBookEntry.Text;
                if (lstPhoneBook.Items.Contains(number))
                {
                    toolStripStatus.Text = number + " already exists";
                    return;
                }
                lstPhoneBook.Items.Add(number);
                txtPhoneBookEntry.Text = string.Empty;
            }
            else
            {
                toolStripStatus.Text = "Phone number cannot be empty";
            }
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        private void SendMessage()
        {
            try
            {
                ListBox.ObjectCollection phoneBook = lstPhoneBook.Items;
                messageGatewayService.CreateGroup(BulkGatewayGroup);
                foreach (string number in phoneBook)
                {
                    messageGatewayService.AddToGroup(BulkGatewayGroup, number);
                }
                if (!chkWapPush.Checked)
                {
                    Sms sms = Sms.NewInstance();
                    sms = SetMessageDetails(sms);
                    sms.DestinationAddress = BulkGatewayGroup;
                    messageGatewayService.SendMessage(sms);
                }
                else
                {
                    Wappush wapPush = Wappush.NewInstance(BulkGatewayGroup, txtWapPushUrl.Text, txtMessage.Text);
                    messageGatewayService.SendMessage(wapPush);
                }
                messageGatewayService.RemoveGroup(BulkGatewayGroup);
            }
            catch (Exception e)
            {
                toolStripStatus.Text = e.Message;
            }          
        }

        /// <summary>
        /// Sets the message details.
        /// </summary>
        /// <param name="sms">The SMS.</param>
        /// <returns></returns>
        private Sms SetMessageDetails(Sms sms)
        {
            sms.Content = txtMessage.Text;
            MessageDataCodingScheme encoding;
            if (MessageEncoding.TryGetValue(cboMessageEncoding.Text, out encoding))
            {
                sms.DataCodingScheme = encoding;
            }
            MessageValidPeriod validity;
            if (ValidityPeriod.TryGetValue(cboValidityPeriod.Text, out validity))
            {
                sms.ValidityPeriod = validity;
            }
            MessageSplitOption splitOption;
            if (MessageSplit.TryGetValue(cboLongMessage.Text, out splitOption))
            {
                sms.LongMessageOption = splitOption;
            }
            if (chkAlertMessage.Checked) sms.Flash = true;

            if (!string.IsNullOrEmpty(txtSmsc.Text))
            {
                sms.ServiceCenterNumber = txtSmsc.Text;
            }

            MessageQueuePriority priority;
            if (QueuePriority.TryGetValue(cboQueuePriority.Text, out priority))
            {
                sms.QueuePriority = priority;
            }

            return sms;
        }

        private void btnStopSending_Click(object sender, EventArgs e)
        {
            try
            {
                btnStopSending.Enabled = false;
                if (messageSender != null && messageSender.IsAlive)
                {
                    toolStripStatus.Text = "Stopping message sending";
                    //messageSender.Interrupt();
                    //if (!messageSender.Join(100))
                    //{
                        messageSender.Abort();
                    //}
                    messageSender = null;
                    toolStripStatus.Text = "Message sending is stopped";
                }
                else
                {
                    toolStripStatus.Text = "No message sending process is detected";
                }

            }
            catch (Exception ex)
            {
                toolStripStatus.Text = ex.Message;
            }
            finally
            {
                btnStopSending.Enabled = true;
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!exitApplication)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to exit?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnRemovePhoneBookEntry_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstPhoneBook.Items.Count == 0)
                {
                    toolStripStatus.Text = "Phone book is empty";
                    return;
                }
                btnRemovePhoneBookEntry.Enabled = false;
                if (lstPhoneBook.SelectedIndex >= 0)
                {
                    string number = (string)lstPhoneBook.SelectedItem;
                    lstPhoneBook.Items.RemoveAt(lstPhoneBook.SelectedIndex);
                    toolStripStatus.Text = number + " is removed";
                } else 
                {
                    toolStripStatus.Text = "No entry is selected";
                }
            }
            finally
            {
                btnRemovePhoneBookEntry.Enabled = true;
            }
        }

        private void btnRemoveAllPhoneBook_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstPhoneBook.Items.Count == 0)
                {
                    toolStripStatus.Text = "Phone book is empty";
                    return;
                }
                btnRemoveAllPhoneBook.Enabled = false;
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all phone book entries ?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    lstPhoneBook.Items.Clear();
                    toolStripStatus.Text = "Phone book is cleared";
                }
            }
            finally
            {
                btnRemoveAllPhoneBook.Enabled = true;
            }
        }

        private void btnImportPhoneBook_Click(object sender, EventArgs e)
        {
            try
            {
                btnImportPhoneBook.Enabled = false;

                openFileDialog.Filter = "Phone Book Contact (*.txt)|*.txt";
                openFileDialog.FileName = string.Empty;
                DialogResult dialogResult = openFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string fileName = openFileDialog.FileName;
                    string[] fileContent = File.ReadAllLines(fileName, Encoding.UTF8);
                    foreach (string line in fileContent)
                    {
                        if (string.IsNullOrEmpty(line)) continue;
                        if (!lstPhoneBook.Items.Contains(line))
                        {
                            lstPhoneBook.Items.Add(line);
                        }
                    }
                    toolStripStatus.Text = fileName + " is imported successfully";
                }
            }
            finally
            {
                btnImportPhoneBook.Enabled = true;
            }
        }

        private void btnSavePhoneBook_Click(object sender, EventArgs e)
        {
            try
            {
                btnSavePhoneBook.Enabled = false;
                if (lstPhoneBook.Items.Count == 0)
                {
                    toolStripStatus.Text = "Phone book is empty";
                    return;
                }
                saveFileDialog.Filter = "Phone Book Contact (*.txt)|*.txt";
                saveFileDialog.FileName = string.Empty;
                DialogResult dialogResult = saveFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string fileName = saveFileDialog.FileName;
                    if (!fileName.Contains(".")) fileName = fileName + ".txt";
                    TextWriter w = new StreamWriter(fileName, false);
                    foreach (string number in lstPhoneBook.Items)
                    {
                        w.WriteLine(number);
                    }
                    w.Close();
                }                 
            }
            finally
            {
                btnSavePhoneBook.Enabled = true;
            }

        }

        private void btnApplyLoggingLevel_Click(object sender, EventArgs e)
        {
            LogLevel logLevel;
            if (LoggingLevel.TryGetValue(cboLoggingLevel.Text, out logLevel))
            {
                try
                {
                    btnApplyLoggingLevel.Enabled = false;

                    List<IGateway> gateways = messageGatewayService.Gateways;
                    if (gateways.Count() > 0)
                    {
                        foreach (IGateway gateway in gateways)
                        {
                            gateway.LogLevel = logLevel;
                        }
                        toolStripStatus.Text = "Log level is applied";
                    }
                    else
                    {
                        toolStripStatus.Text = "No gateway is available";
                    }                    
                }
                finally
                {
                    btnApplyLoggingLevel.Enabled = true;
                }
            }       
        }

        private void btnRefreshLoggingInformation_Click(object sender, EventArgs e)
        {
            try
            {
                if (Logger.LogPath != null)
                {
                    string fileContent = File.ReadAllText(Logger.LogPath, Encoding.UTF8);
                    txtLoggingInformation.Text = fileContent;
                    ScrollToEnd();
                    toolStripStatus.Text = "Logging information is refreshed";
                }
            }
            catch (Exception ex)
            {
                toolStripStatus.Text = ex.Message;
            }
            
        }

        private void btnClearLoggingInformation_Click(object sender, EventArgs e)
        {
            TextWriter w = null;
            try
            {
                w = new StreamWriter(Logger.LogPath, false);
                w.Write(string.Empty);
                w.Close();
                w = null;
                txtLoggingInformation.Text = string.Empty;
                toolStripStatus.Text = "Logging information is cleared";
            }
            catch (Exception ex)
            {
                toolStripStatus.Text = ex.Message;
            }
            finally
            {
                if (w != null)
                {
                    try
                    {
                        w.Close();
                    }
                    catch (Exception ex) { }
                }
            }

        }

        private void ScrollToEnd()
        {
            txtLoggingInformation.SelectionStart = txtLoggingInformation.Text.Length;
            txtLoggingInformation.ScrollToCaret();
        }

        private void tabMain_Click(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            if (tc.SelectedTab == tabAbout)
            {
                Assembly assembly = Assembly.GetAssembly(this.GetType());
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

                if (messageGatewayService.Gateways.Count() > 0)
                {
                    IMobileGateway mobileGateway = (IMobileGateway)messageGatewayService.Gateways[0];
                    lblLicense.Text = mobileGateway.License.Information;
                }
            }
        }

        private void AddGateway()
        {
            if (string.IsNullOrEmpty(txtGatewayId.Text))
            {
                toolStripStatus.Text = "Gateway id must not be blank !";
                return;
            }

            // Check if gateway id is unique
            string gatewayId = txtGatewayId.Text.Trim();
            IGateway gateway;
            if (messageGatewayService.Find(gatewayId, out gateway))
            {
                toolStripStatus.Text = "Gateway id is not unique !";
                return;
            }

            toolStripStatus.Text = "Adding gateway " + txtGatewayId.Text;
            MobileGatewayConfiguration config = MobileGatewayConfiguration.NewInstance();
            config.PortName = cboPort.Text;
            config.BaudRate = (PortBaudRate)Enum.Parse(typeof(PortBaudRate), cboBaudRate.Text);
            config.DataBits = (PortDataBits)Enum.Parse(typeof(PortDataBits), cboDataBits.Text);
                      
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

            config.SendRetries = Convert.ToInt32(updSendRetries.Value);
            config.SendWaitInterval = Convert.ToInt32(updSendWaitInterval.Value);
            

            // Set the license key
            config.LicenseKey = this.licenseKey;

            // Set logging level
            LogLevel logLevel;
            if (LoggingLevel.TryGetValue(cboLoggingLevel.Text, out logLevel))
            {
                config.LogLevel = logLevel;
            }

            // Set gateway id
            config.GatewayId = gatewayId;

           
            MessageGateway<IMobileGateway, MobileGatewayConfiguration> messageGateway =
               MessageGateway<IMobileGateway, MobileGatewayConfiguration>.NewInstance();
            try
            {
                IMobileGateway mobileGateway;
                btnAddGateway.Enabled = false;
                mobileGateway = messageGateway.Find(config);
                if (mobileGateway == null) throw new Exception("Error connecting to gateway. Check the log file");

                if (radSim.Checked)
                    mobileGateway.MessageStorage = MessageStorage.Sim;
                else if (radPhone.Checked)
                    mobileGateway.MessageStorage = MessageStorage.Phone;

                // Set persistence queue
                if (chkPersistenceQueue.Checked)
                {
                    mobileGateway.PersistenceQueue = true;
                    mobileGateway.PersistenceFolder = txtPersistenceFolder.Text;
                }

                mobileGateway.MessageSending += OnMessageSending;
                mobileGateway.MessageSendingFailed += OnMessageFailed;
                mobileGateway.MessageSent += OnMessageSent;

                // Uncomment this part to enable message receiving
                //mobileGateway.MessageReceived += new MessageReceivedEventHandler(OnMessageReceived);
                //mobileGateway.EnableNewMessageNotification(MessageNotification.StatusReport);
                //mobileGateway.PollNewMessages = true;
                
                                            
                if (!messageGatewayService.Add(mobileGateway))
                {
                    throw messageGatewayService.LastError;
                }
                
                // If the router is number prefix router, then assign the prefixes
                string[] prefixes = new string[]{};
                if (!string.IsNullOrEmpty(txtNumberPrefix.Text))
                {
                    prefixes = txtNumberPrefix.Text.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries);
                    for (int i =0;i< prefixes.Length; i++)
                    {
                        prefixes[i] = prefixes[i].Trim();
                    }

                    // Add the prefixes to the configuration
                    mobileGateway.Configuration.Prefixes.AddRange(prefixes);

                      // Check if it is a number prefix based router
                    if (messageGatewayService.Router is NumberRouter)
                    {
                        foreach (string prefix in prefixes)
                        {
                            // Assign prefix to the gateway                            
                            ((NumberRouter)messageGatewayService.Router).Assign(prefix, mobileGateway);
                        }
                    }
                }

                // Add to display panel
                ListViewItem gatewayItem = new ListViewItem(gatewayId);
                gatewayItem.ImageIndex = 0;
                lstGateways.Items.Add(gatewayItem);

                toolStripStatus.Text = "Connected to gateway " + gatewayId + " successfully";
                grpBoxGatewaySettings.Enabled = false;  
            }
            catch (Exception ex)
            {
                toolStripStatus.Text = ex.Message;
            }
            finally
            {
                btnAddGateway.Enabled = true;
            }
        }

        

        private void RemoveGateway()
        {
            try
            {
                ListView.SelectedListViewItemCollection selectedItems = lstGateways.SelectedItems;
                if (selectedItems.Count == 0)
                {
                    toolStripStatus.Text = "No gateway is selected";
                    return;
                }
                string gatewayId = selectedItems[0].Text;
                IGateway gateway;
                if (messageGatewayService.Find(gatewayId, out gateway))
                {
                    IMobileGateway mobileGateway = (IMobileGateway)gateway;

                    if (mobileGateway.GetQueueCount() > 0)
                    {
                        toolStripStatus.Text = "There are still " + mobileGateway.GetQueueCount() + " message(s) in the queue. Cannot remove the gateway now";
                        return;
                    }

                    mobileGateway.Disconnect();
                    mobileGateway = null;
                  
                    // Remove from service
                    messageGatewayService.Remove(gatewayId);

                    lstGateways.Items.RemoveAt(selectedItems[0].Index);
                    toolStripStatus.Text = "Gateway " + gatewayId + " is removed";
                }
            }
            catch (Exception e)
            {
                toolStripStatus.Text = e.Message;
            }
            finally
            {
            }
        }

        private void lstGateways_ItemActivate(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selectedItems = lstGateways.SelectedItems;
            if (selectedItems.Count == 0)
            {
                toolStripStatus.Text = "No gateway is selected";
                return;
            }
            string gatewayId = selectedItems[0].Text;
            toolStripStatus.Text = "Displaying details for gateway " + gatewayId;

            IGateway gateway;
            if (messageGatewayService.Find(gatewayId, out gateway))
            {
                frmGatewayDetails detailsForm = new frmGatewayDetails();
                detailsForm.Text = "Gateway Details - " + gatewayId;
                detailsForm.Gateway = gateway;
                detailsForm.GatewayService = messageGatewayService;
                detailsForm.Show(this);
            }
            else
            {
                toolStripStatus.Text = "Gateway " + gatewayId + " is not found";
            }
        }

        private void chkMonitorGatewayStatus_CheckedChanged(object sender, EventArgs e)
        {
            messageGatewayService.MonitorService = chkMonitorGatewayStatus.Checked;
        }

        private void CheckGatewayStatus()
        {
            while (true)
            {
                string status = string.Empty;
                try
                {
                    Thread.Sleep(5000);
                    foreach (IGateway gateway in messageGatewayService.Gateways)
                    {
                        if (gateway.Status != GatewayStatus.Started)
                        {
                            if (string.IsNullOrEmpty(status))
                            {
                                status = gateway.Id + " is unplugged";
                            }
                            else
                            {
                                status += ", " + gateway.Id + " is unplugged";
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(status))
                        toolStripGateway.Text = status;
                }
                catch (ThreadInterruptedException tiEx)
                {                 
                    break;
                }
                catch (Exception e)
                {
                    break;
                }
            }
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnSaveToFile.Enabled = false;

                List<IGateway> gateways = messageGatewayService.Gateways;
                if (gateways.Count() == 0)
                {
                    ShowError("No gateway is configured");
                    return;
                }

                saveFileDialog.Filter = "XML File (*.xml)|*.xml";
                saveFileDialog.FileName = string.Empty;
                DialogResult dialogResult = saveFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string saveFileName = saveFileDialog.FileName;

                    List<MobileGatewayConfiguration> configList = new List<MobileGatewayConfiguration>(gateways.Count());
                    foreach (IGateway gw in gateways)
                    {
                        configList.Add(((IMobileGateway)gw).Configuration);
                    }

                    // Save the configuration into XML
                    XmlSerializer s = new XmlSerializer(typeof(List<MobileGatewayConfiguration>));
                    TextWriter w = new StreamWriter(saveFileName, false);
                    s.Serialize(w, configList);
                    w.Close();
 
                    MessageBox.Show("File is saved successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }
            }
            catch (Exception ex)
            {
                ShowError("Error saving configuration: " + ex.Message);
            }
            finally
            {
                btnSaveToFile.Enabled = true;

            }
        }

        private void btnLoadFromFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnLoadFromFile.Enabled = false;

                openFileDialog.Filter = "XML File (*.xml)|*.xml";
                openFileDialog.FileName = string.Empty;
                DialogResult dialogResult = openFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    string fileName = openFileDialog.FileName;

                    // Create a new file stream for reading the XML file
                    FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    XmlSerializer s = new XmlSerializer(typeof(List<MobileGatewayConfiguration>));

                    // Load the object saved above by using the Deserialize function
                    List<MobileGatewayConfiguration> configList = (List<MobileGatewayConfiguration>)s.Deserialize(fileStream);                    
                    foreach (MobileGatewayConfiguration config in configList)
                    {
                        IGateway gateway;
                        if (messageGatewayService.Find(config.GatewayId, out gateway))
                        {
                            ShowError(string.Format("Gateway id {0} already exists", config.GatewayId));
                            continue;
                        }

                        try
                        {
                            MessageGateway<IMobileGateway, MobileGatewayConfiguration> messageGateway =
                                MessageGateway<IMobileGateway, MobileGatewayConfiguration>.NewInstance();

                            IMobileGateway mobileGateway;
                            mobileGateway = messageGateway.Find(config);
                            if (mobileGateway == null) throw new Exception("Error connecting to gateway. Check the log file");
                            mobileGateway.MessageSending += OnMessageSending;
                            mobileGateway.MessageSendingFailed += OnMessageFailed;
                            mobileGateway.MessageSent += OnMessageSent;

                            if (!messageGatewayService.Add(mobileGateway))
                            {
                                throw messageGatewayService.LastError;
                            }

                            // Add to display panel
                            ListViewItem gatewayItem = new ListViewItem(mobileGateway.Id);
                            gatewayItem.ImageIndex = 0;
                            lstGateways.Items.Add(gatewayItem);

                            toolStripStatus.Text = "Connected to gateway " + mobileGateway.Id + " successfully";
                        }
                        catch (Exception exception)
                        {
                            ShowError(string.Format("Error connecting to gateway {0}. {1}", config.GatewayId, exception.Message));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading configuration: " + ex.Message);
            }
            finally
            {
                btnLoadFromFile.Enabled = true;
            }
        }

        private void ShowError(string errorMessage)
        {
            MessageBox.Show(errorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void lnkLabelSampleBalancer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmCodeViewer codeViewer = new frmCodeViewer();
            codeViewer.SourceCode = Resources.RoundRobinLoadBalancer;
            codeViewer.ShowDialog(this);
        }

        private void lnkLabelSampleRouter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmCodeViewer codeViewer = new frmCodeViewer();
            codeViewer.SourceCode = Resources.NumberRouter;
            codeViewer.ShowDialog(this);
        }

        private void btnApplyServiceSettings_Click(object sender, EventArgs e)
        {
            try
            {
                btnApplyServiceSettings.Enabled = false;

                string loadBalancerValue = cboLoadBalancer.Text;
                string routerValue = cboRouter.Text;

                // Apply load balancer settings
                if (loadBalancerValue == DefaultLoadBalancer)
                {                    
                    LoadBalancer loadBalancer = new LoadBalancer(messageGatewayService);
                    messageGatewayService.LoadBalancer = loadBalancer;
                }
                else if (loadBalancerValue == RoundRobinLoadBalancer)
                {
                    // Apply only if existing is not a round robin load balancer
                    if (!(messageGatewayService.LoadBalancer is RoundRobinLoadBalancer)) 
                    {
                        RoundRobinLoadBalancer roundRobinLoadBalancer = new RoundRobinLoadBalancer(messageGatewayService);
                        messageGatewayService.LoadBalancer = roundRobinLoadBalancer;
                    }
                }

                // Apply router settings
                if (routerValue == DefaultRouter)
                {
                    Router router = new Router(messageGatewayService);
                    messageGatewayService.Router = router;
                }
                else if (routerValue == NumberPrefixRouter)
                {
                    // Apply only if existing is not a number router
                    if (!(messageGatewayService.Router is NumberRouter))
                    {
                        NumberRouter numberRouter = new NumberRouter(messageGatewayService);
                        messageGatewayService.Router = numberRouter;

                        // Set the prefixes for each gateway
                        List<IGateway> gateways = messageGatewayService.Gateways;
                        foreach (IGateway gateway in gateways)
                        {
                            IMobileGateway mobileGateway = (IMobileGateway)gateway;
                            if (mobileGateway.Configuration.Prefixes.Count() > 0)
                            {
                                foreach (string prefix in mobileGateway.Configuration.Prefixes)
                                {
                                    // Assign prefix to the gateway
                                    ((NumberRouter)messageGatewayService.Router).Assign(prefix, mobileGateway);
                                }
                            }
                            else
                            {
                                ShowError(string.Format("No prefixes configured for gateway {0}. Please configure the prefixes!", mobileGateway.Id));
                            }
                        }
                    }
                }

                MessageBox.Show("Service settings are applied", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                ShowError("Error applying service settings. " + ex.Message);
            }
            finally
            {
                btnApplyServiceSettings.Enabled = true;
            }
            
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkPersistenceQueue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkPersistenceQueue_CheckedChanged(object sender, EventArgs e)
        {
            txtPersistenceFolder.Enabled = chkPersistenceQueue.Checked;
            lnkBrowse.Enabled = chkPersistenceQueue.Checked;
            if (chkPersistenceQueue.Checked)
            {
                MessageBox.Show("For each gateway please specify a different persistence folder", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Handles the LinkClicked event of the lnkBrowse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void lnkBrowse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                txtPersistenceFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkPauseMessageSending control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkPauseMessageSending_CheckedChanged(object sender, EventArgs e)
        {
            List<IGateway> gateways = messageGatewayService.Gateways;
            foreach (IGateway gw in gateways)
            {
                IMobileGateway mobileGateway = gw as IMobileGateway;
                mobileGateway.IsMessageQueueEnabled = !chkPauseMessageSending.Checked;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.twit88.com");
        }

        /*
        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                Sms sms = Sms.NewInstance();
                sms.Content = "testing " + i;
                sms.DestinationAddress = "12345678";
                messageGatewayService.SendMessage(sms);
            }
        }
        */
    }
}

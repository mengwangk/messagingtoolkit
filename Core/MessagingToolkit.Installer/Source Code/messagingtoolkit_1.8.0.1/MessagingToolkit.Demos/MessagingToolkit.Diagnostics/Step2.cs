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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

using MessagingToolkit.UI;
using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Diagnostics
{
    public partial class Step2 : BaseStep
    {
        /// <summary>
        /// Mobile gateway interface
        /// </summary>
        private IMobileGateway mobileGateway = MobileGatewayFactory.Default;

        /// <summary>
        /// Main form
        /// </summary>
        private frmMain mainForm;


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


        public Step2(frmMain mainForm):base()
        {
            InitializeComponent();

            this.mainForm = mainForm;

            // Add the port
            string[] portNames = SerialPort.GetPortNames();
            var sortedList = portNames.OrderBy(port => Convert.ToInt32(port.Replace("COM", string.Empty)));
            foreach (string port in sortedList)
            {
                if (!cboPort.Items.Contains(port))
                     cboPort.Items.Add(port);
            }
            cboPort.SelectedIndex = 0;

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
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {    
            MobileGatewayConfiguration config = MobileGatewayConfiguration.NewInstance();
            config.PortName = cboPort.Text;
            config.BaudRate = (PortBaudRate)Enum.Parse(typeof(PortBaudRate), cboBaudRate.Text);
            config.DataBits = (PortDataBits)Enum.Parse(typeof(PortDataBits), cboDataBits.Text);
           
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

            config.DisablePinCheck = chkDisablePinCheck.Checked;

            if (!string.IsNullOrEmpty(txtPIN.Text))
            {
                config.Pin = txtPIN.Text;
            }

            // Create the gateway for mobile
            MessageGateway<IMobileGateway, MobileGatewayConfiguration> messageGateway =
                MessageGateway<IMobileGateway, MobileGatewayConfiguration>.NewInstance();
            try
            {
                btnConnect.Enabled = false;

                if (mobileGateway.Connected)
                {
                    mobileGateway.Disconnect();
                    mobileGateway = null;
                    mobileGateway = MobileGatewayFactory.Default;
                }

                mobileGateway = messageGateway.Find(config);
                mobileGateway.LogLevel = LogLevel.Verbose;              
                MessageBox.Show("Connected to gateway successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                mainForm.Gateway = mobileGateway;
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
    }
}

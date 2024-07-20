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
using System.Threading;

using MessagingToolkit.Core;
using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Service;

namespace MessagingToolkit.BulkGateway
{
    public partial class frmGatewayDetails : Form
    {
        /// <summary>
        /// Message sending thread
        /// </summary>
        private Thread displayThread;

        public frmGatewayDetails()
        {
            InitializeComponent();
        }

        private void frmGatewayDetails_Load(object sender, EventArgs e)
        {
            DisplayInformation();
        }

        public IGateway Gateway
        {
            get;
            set;
        }

        public MessageGatewayService GatewayService
        {
            get;
            set;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex) { }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DisplayInformation();
        }

        private void DisplayInformation()
        {
            IMobileGateway mobileGateway = (IMobileGateway)Gateway;
            MobileGatewayConfiguration config = mobileGateway.Configuration;
            txtPrefixes.Text = string.Join(",", config.Prefixes.ToArray());

            if (displayThread == null || !displayThread.IsAlive)
            {
                displayThread = new Thread(new ThreadStart(this.RetrieveInformation));
                displayThread.Start();
            }
        }

        private void RetrieveInformation()
        {
            try
            {
                if (Gateway != null)
                {
                    btnRefresh.Enabled = false;
                    IMobileGateway mobileGateway = (IMobileGateway)Gateway;
                    DeviceInformation deviceInformation = mobileGateway.DeviceInformation;
                    txtModel.Text = deviceInformation.Model;
                    txtManufacturer.Text = deviceInformation.Manufacturer;
                    txtFirmware.Text = deviceInformation.FirmwareVersion;
                    txtSerialNo.Text = deviceInformation.SerialNo;
                    txtImsi.Text = deviceInformation.Imsi;

                    BatteryCharge batteryCharge = mobileGateway.BatteryCharge;
                    progressBarBatteryLevel.Value = batteryCharge.BatteryChargeLevel;

                    SignalQuality signalQuality = mobileGateway.SignalQuality;
                    progressBarSignalQuality.Value = signalQuality.SignalStrengthPercent;

                    txtOutgoingMessage.Text = Convert.ToString(mobileGateway.Statistics.OutgoingSms);

                    txtGatewayStatus.Text = mobileGateway.Status.ToString();

                    
                }
            }
            catch (Exception e)
            {
                toolStripGateway.Text = e.Message;
            }
            finally
            {
                btnRefresh.Enabled = true;
            }
            
        }

        private void frmGatewayDetails_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (displayThread != null && displayThread.IsAlive)
                {
                    //displayThread.Interrupt();
                    //if (!displayThread.Join(10))
                    //{
                        displayThread.Abort();
                    //}
                    displayThread = null;
                }
            }
            catch (Exception ex) { }
        }

        private void btnApplyPrefixes_Click(object sender, EventArgs e)
        {
            try
            {
                btnApplyPrefixes.Enabled = false;

                string prefixes = txtPrefixes.Text;
                if (!string.IsNullOrEmpty(prefixes))
                {
                    if (GatewayService.Router is NumberRouter)
                    {
                        string[] prefixList = prefixes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        NumberRouter numberRouter = (NumberRouter)GatewayService.Router;
                        numberRouter.Remove(Gateway);

                        foreach (string prefix in prefixList)
                        {
                            if (!string.IsNullOrEmpty(prefix))
                            {
                                numberRouter.Assign(prefix.Trim(), Gateway);
                            }
                        }
                        ((IMobileGateway)Gateway).Configuration.Prefixes = new List<string>(prefixList);
                        MessageBox.Show("Prefixes are applied", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);                        
                    }
                    else
                    {
                        MessageBox.Show("Not using a number prefix router", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            finally
            {
                btnApplyPrefixes.Enabled = true;
            }
        }
    }
}

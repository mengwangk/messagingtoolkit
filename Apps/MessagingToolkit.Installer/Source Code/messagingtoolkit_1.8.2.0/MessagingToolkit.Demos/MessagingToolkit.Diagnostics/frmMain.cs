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

using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile;

namespace MessagingToolkit.Diagnostics
{
    public partial class frmMain : Form
    {
        /// <summary>
        /// Mobile gateway interface
        /// </summary>
        private IMobileGateway mobileGateway = MobileGatewayFactory.Default;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            
        }
        
      
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GatewayConnectionWizard wizard = new GatewayConnectionWizard(this);
            wizard.ShowDialog(this);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog(this);
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                disconnectToolStripMenuItem.Enabled = false;

                if (mobileGateway != null && mobileGateway.Connected)
                {
                    if (mobileGateway.Disconnect())
                    {
                        mobileGateway = null;
                        mobileGateway = MobileGatewayFactory.Default;
                        MessageBox.Show("Gateway is disconnected successfully", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(mobileGateway.LastError.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Gateway is not connected", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                disconnectToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Gets or sets the gateway.
        /// </summary>
        /// <value>The gateway.</value>
        public IMobileGateway Gateway
        {
            get
            {
                return this.mobileGateway;
            }
            set
            {
                this.mobileGateway = value;
            }
        }

       
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mobileGateway != null && mobileGateway.Connected)
            {
                mobileGateway.Disconnect();
                mobileGateway = null;                
            }
        }
    }
}

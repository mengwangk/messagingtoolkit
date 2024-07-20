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

using MessagingToolkit.UI;
using MessagingToolkit.Diagnostics.Properties;

using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile;

namespace MessagingToolkit.Diagnostics
{
    /// <summary>
    /// Connection wizard to configure the connection to the gateway
    /// </summary>
    public partial class GatewayConnectionWizard : BaseWizard
    {
        private frmMain mainForm;

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayConnectionWizard"/> class.
        /// </summary>
        public GatewayConnectionWizard(frmMain mainForm):base()
        {
            InitializeComponent();

            // Uncomment and change this if you want to have a custom logo
            Logo = Resources.doctor;
            SideBarLogo = Resources.diagnose;
            SideBarImage = Resources.sidebar;

            this.mainForm = mainForm;     
        }

        private void GatewayConnectionWizard_LoadSteps(object sender, EventArgs e)
        {
            AddStep("Step1", new Step1());
            AddStep("Step2", new Step2(mainForm));
            AddStep("Step3", new Step3(mainForm));
        }

        private void next_Click(object sender, EventArgs e)
        {
            if (currentStep.Name == "Step3")
            {
                if (mainForm.Gateway == null || !mainForm.Gateway.Connected)
                {
                    MessageBox.Show("Gateway is not connected. Unable to proceed", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.MoveBack();    
                }                
            }
        }                     
    }
}

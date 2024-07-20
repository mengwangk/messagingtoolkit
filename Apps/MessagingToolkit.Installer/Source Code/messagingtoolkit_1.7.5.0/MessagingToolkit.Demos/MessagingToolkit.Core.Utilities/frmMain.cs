﻿//===============================================================================
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

namespace MessagingToolkit.Core.Utilities
{
    /// <summary>
    /// Main form
    /// </summary>
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnGsmModem_Click(object sender, EventArgs e)
        {
            frmSMS smsForm = new frmSMS();
            smsForm.ShowDialog(this);
        }

        private void btnMMSMM1_Click(object sender, EventArgs e)
        {
            frmMM1 mm1Form = new frmMM1();
            mm1Form.ShowDialog(this);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
        }

        private void btnSmpp_Click(object sender, EventArgs e)
        {
            frmSMPP smppForm = new frmSMPP();
            smppForm.ShowDialog(this);
        }
    }
}
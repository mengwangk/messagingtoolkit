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

using MessagingToolkit.UI;
using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Diagnostics
{
    public partial class Step1 : BaseExteriorStep
    {              
        private const string InstructionUrl = "http://twit88.com/blog/2007/09/22/use-the-net-sms-library-to-retrieve-phone-settings/";
        
        public Step1():base()
        {
            InitializeComponent();
        }

        private void linkGuide_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(InstructionUrl);
        }
    }
}

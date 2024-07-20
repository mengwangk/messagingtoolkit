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

namespace MessagingToolkit.SmartGateway.SmartClient
{
    public partial class frmMain : Form
    {
        private NavigateBar navigatePane;
        private MTSplitter splitterNavigateMenu;
        private NavigateBarButton nvbMessage;
        private NavigateBarButton nvbGateways;
        private NavigateBarButton nvbScheduler;
        private NavigateBarButton nvbContacts;         
      

        public frmMain()
        {
            InitializeComponent();
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       
        private void frmMain_Load(object sender, EventArgs e)
        {
            InitNavigateBar();
        }

        private void InitNavigateBar()
        {
            navigatePane = new NavigateBar();
            //navigatePane.SaveAndRestoreSettings = true;
            navigatePane.Dock = DockStyle.Left;
            navigatePane.IsShowCollapsibleScreen = true;
            navigatePane.CollapsedScreenWidth = 200; // For all buttons
            navigatePane.CollapsibleWidth = 32;
            navigatePane.OnNavigateBarButtonSelected += new NavigateBar.OnNavigateBarButtonEventHandler(navigatePane_OnNavigateBarButtonSelected);
            navigatePane.IsShowCollapseButton = true;         
            navigatePane.Width = 150;
         
            nvbMessage = new NavigateBarButton();
            nvbMessage.RelatedControl = new MessageTree();
            nvbMessage.Caption = "Message";
            nvbMessage.CaptionDescription = "My Messages";
            nvbMessage.Image = Properties.Resources.Mail24;
            nvbMessage.Enabled = true;
            nvbMessage.Key = "MESSAGE";
            nvbMessage.IsShowCaptionImage = true;  
            nvbMessage.IsSelected = true;
            
            /*
            nvbGateways = new NavigateBarButton();
            nvbGateways.RelatedControl = new WebConsole();
            nvbGateways.Caption = "Gateways";
            nvbGateways.CaptionDescription = "Gateways Configuration";
            nvbGateways.Image = Properties.Resources.Gateways24;
            nvbGateways.Enabled = true;
            nvbGateways.Key = "GATEWAYS";
            nvbGateways.IsShowCaptionImage = true;
            */

            nvbScheduler = new NavigateBarButton();
            nvbScheduler.RelatedControl = new MessageTree();
            nvbScheduler.Caption = "Scheduler";
            nvbScheduler.CaptionDescription = "Message Scheduler";
            nvbScheduler.Image = Properties.Resources.Calendar24;
            nvbScheduler.Enabled = true;
            nvbScheduler.Key = "SCHEDULER";
            nvbScheduler.IsShowCaptionImage = true;
           
            DataGridView dgv = new DataGridView();
            nvbContacts = new NavigateBarButton();
            nvbContacts.Caption = "Contacts";
            nvbContacts.CaptionDescription = "Contact Management";
            nvbContacts.Image = Properties.Resources.Contacts24;
            nvbContacts.Enabled  = true;
            nvbContacts.RelatedControl = dgv;           
            nvbContacts.Key = "CONTACTS";
            nvbContacts.IsShowCaptionImage = true;
       
         
            navigatePane.NavigateBarButtons.AddRange( new NavigateBarButton[] {nvbMessage,nvbScheduler, nvbContacts});
            
            splitterNavigateMenu = new MTSplitter();
            splitterNavigateMenu.Size = new Size(7, 100);
            splitterNavigateMenu.SplitterPointCount = 10;
            splitterNavigateMenu.SplitterPaintAngle = 360F;

            
            Controls.AddRange(new Control[] { splitterNavigateMenu, navigatePane, txtBanner, toolStrip, menuStrip});            

        }
                    
        /// <summary>
        /// Any button selected
        /// </summary>
        /// <param name="tNavigateBarButton"></param>
        void navigatePane_OnNavigateBarButtonSelected(NavigateBarButton tNavigateBarButton)
        {
            if (navigatePane == null)
                return;
            /*
            if (navigatePane.SelectedButton != null)
                this.Text = navigatePane.SelectedButton.Caption;
            */
        }        
    }
}

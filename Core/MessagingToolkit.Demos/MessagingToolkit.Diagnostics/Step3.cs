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
using System.Text.RegularExpressions;

using MessagingToolkit.UI;
using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Diagnostics
{
    public partial class Step3 : BaseExteriorStep
    {
        private frmMain mainForm;

        private const string Supported = " [YES]";
        private const string NotSupported = " [NO] ";
        private const int LineLength = 50;
        private const string PadCharacter = ".";
        private const string OK = "OK";

        /// <summary>
        /// Initializes a new instance of the <see cref="Step3"/> class.
        /// </summary>
        /// <param name="mainForm">The main form.</param>
        public Step3(frmMain mainForm):base()
        {
            InitializeComponent();

            this.mainForm = mainForm;
        }

        /// <summary>
        /// Handles the Click event of the btnDiagnose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnDiagnose_Click(object sender, EventArgs e)
        {
            txtDiagnoseResults.AppendText("Start diagnosing...\r\n");

            IMobileGateway mobileGateway = mainForm.Gateway;
            string serviceCenterAddress = mobileGateway.ServiceCentreAddress.Number;

            CheckFeatureSupport("Service Center Address (CSCA)", !string.IsNullOrEmpty(serviceCenterAddress));
            CheckFeatureSupport("Message notification (CMTI,CDSI)", mobileGateway.EnableMessageNotifications());
            CheckFeatureSupport("Message routing (CMT,CDS)", mobileGateway.EnableMessageRouting());
            CheckFeatureSupport("Call notification (CLIP)", mobileGateway.EnableCallNotifications());
            CheckFeatureSupport("Message PDU mode (AT+CMGF=0)", mobileGateway.SetMessageProtocol(MessageProtocol.PduMode));
            CheckFeatureSupport("Message Text mode (AT+CMGF=1)", mobileGateway.SetMessageProtocol(MessageProtocol.TextMode));
                                 
            string[] phoneBookStorages = mobileGateway.PhoneBookStorages;
            if (phoneBookStorages != null && phoneBookStorages.Count() > 0) 
            {
                txtDiagnoseResults.AppendText("Supported phone book storage (CPBS) [" );
                foreach (string storage in phoneBookStorages) 
                {
                    txtDiagnoseResults.AppendText(storage + ", " );
                }
                txtDiagnoseResults.AppendText("]\r\n");
            }  else 
            {
                AppendText("Phone book storage", NotSupported);
            }
            Subscriber[] subscribers = mobileGateway.Subscribers;
            if (subscribers != null && subscribers.Length > 0)
            {
                AppendText("Subscriber information (CNUM)", Supported);
            }
            else
            {
                AppendText("Subscriber information (CNUM)", NotSupported);
            }

            Diagnose("Sending message (CMGS)",DiagnosticsCommand.SendMessage);
            Diagnose("List message (CMGL)", DiagnosticsCommand.ListMessage);
            Diagnose("Read message (CMGR)", DiagnosticsCommand.ReadMessage);  
        
            txtDiagnoseResults.AppendText("\r\nFINISHED\r\n");
            
        }

        private void CheckFeatureSupport(string feature, bool isSupported)
        {
            if (isSupported)
            {
                AppendText(feature, Supported);
            }
            else
            {
                AppendText(feature, NotSupported);
            }
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        private void AppendText(string prefix, string suffix)
        {
            int totalLength = prefix.Length + suffix.Length;
            int padLength = 0;
            if (totalLength < LineLength)
            {
                padLength = LineLength - totalLength;                
            }
            txtDiagnoseResults.AppendText(prefix + Pad(padLength) + suffix + "\r\n");
        }


        /// <summary>
        /// Pads the specified length.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        private string Pad(int length)
        {
            string result = string.Empty;
            for (int i = 1; i < length; i++) 
            {
                result += PadCharacter;
            }
            return result;
        }

        /// <summary>
        /// Diagnoses the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="expectedResponse">The expected response.</param>
        private void Diagnose(string feature, string command)
        {
            IMobileGateway mobileGateway = mainForm.Gateway;
            Log("Sending command " + command);         
            string response = mobileGateway.SendCommand(command);
            Log("Response: " + response);
            Regex pattern = new Regex(OK);
            if (pattern.IsMatch(response))
            {
                AppendText(feature, Supported);
            }
            else
            {
                AppendText(feature, NotSupported);
            }

        }

        /// <summary>
        /// Logs the specified output.
        /// </summary>
        /// <param name="output">The output</param>
        private void Log(string output)
        {
            if (chkVerbose.Checked)
            {
                txtDiagnoseResults.AppendText(output + "\r\n");
            }
        }
    }
}

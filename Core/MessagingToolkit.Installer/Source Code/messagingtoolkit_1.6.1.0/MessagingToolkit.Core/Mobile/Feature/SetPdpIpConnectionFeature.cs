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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Create the Packet Data Protocol (PDP) IP based connection.
    /// 
    /// <code>
    /// Command: AT+CGDCONT=<cid> [,<pdptype> [,<apn>[,<pdpaddr> [,<dcomp>[,<hcomp]]]]]
    /// Response: OK | ERROR
    /// Command: AT+CGDCONT?
    /// Response: +CGDCONT: <cid1>,<pdptype1>,<apn1>,<pdpaddr1><dcomp1>,<hcomp1>, …, <cidN>,<pdptypeN>,<apnN>,<pdpaddrN><dcompN>
    ///
    /// Command: AT+CGDCONT=?
    /// Response: +CGDCONT: (<cid_range>),<pdptype>,,,(<dcomp_range>),(<hcomp_range>)
    /// 
    /// Description: Allows configuration of one or several packet data protocol context which forms the base of a data connection.
    /// <cid> PDP context ID, minimum value is 1, maximum value depends on device and can be found with the =? command.
    /// 
    /// <pdptype> String parameter identifying the protocol type
    /// IP – Internet Protocol
    /// IPV6 – Internet Protocol, version 6
    /// PPP – Point to Point Protocol
    ///
    /// <apn> String that identifies the Access Point Name in the packet data network.
    /// 
    /// <pdpaddr> Requested address, if null (0.0.0.0) an address is requested dynamically.
    ///
    /// <dcomp> PDP data compression control, off by default.
    ///
    /// <hcomp> PDP header compression control, off by default.
    /// </code>
    /// 
    /// <example>
    ///    > AT+CGDCONT=1,"IP","apn.net"
    ///    OK
    ///
    ///    >AT+CGDCONT?
    ///    +CGDCONT: 1,"IP","apn.net","0.0.0.0″,0,0
    ///
    ///    >AT+CGDCONT=?
    ///    +CGDCONT: (1-16),"IP",,,(0-2),(0-4)
    ///    +CGDCONT: (1-16),"PPP",,,(0-2),(0-4)
    ///    +CGDCONT: (1-16),"IPV6″,,,(0-2),(0-4)
    /// </example>
    /// </summary>
    internal class SetPdpIpConnectionFeature : BaseFeature<SetPdpIpConnectionFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

        /// <summary>
        /// Command to retrieve the SIM status
        /// </summary>
        private const string GetPdpConnectionCommand = "AT+CGDCONT?";

        /// <summary>
        /// Command to create a PDP connection
        /// </summary>
        private const string DefaultCreatePdpConnectionCommand = "AT+CGDCONT={0},\"{1}\",\"{2}\"";

        /// <summary>
        /// Command to create a PDP connection
        /// </summary>
        private const string CustomCreatePdpConnectionCommand = "AT+CGDCONT={0},\"{1}\",\"{2}\",\"0.0.0.0\",{3},{4}" ;

        /// <summary>
        /// PDP connection pattern
        /// </summary>
        private const string PdpConnectionPattern = "\\+CGDCONT: (\\d+),\"(.*)\",\"(.*)\",\"(.*)\",(\\d+),(\\d+)";


        /// <summary>
        /// Set GPRS attach mode
        /// </summary>
        //private const string SetGprsAttachCommand = "AT+CGATT=1";

        #endregion =========================================================================================


        #region ================ Private Variables =========================================================

        // Last context id
        private int lastContextId;


        #endregion =========================================================================================

        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private SetPdpIpConnectionFeature() : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(GetPdpConnectionCommand, Response.Ok, DoNothing, GetPdpPostprocess));
            AddCommand(Command.NewInstance(DefaultCreatePdpConnectionCommand, Response.Ok, CreatePdpPreprocess, CreatePdpPostprocess));
            //AddCommand(Command.NewInstance(SetGprsAttachCommand, Response.Ok, DoNothing, DoNothing, true));

            lastContextId = 0;          
            APN = string.Empty;
        }


        #endregion ==========================================================================================


        #region =========== Private Methods =================================================================

       
        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool GetPdpPostprocess(IContext context, ICommand command)
        {
            string[] results = ResultParser.ParseResponse(context.GetData());
            List<PdpConnection> connections = new List<PdpConnection>(1);

            Regex regex = new Regex(PdpConnectionPattern);
            foreach (string line in results)
            {
                Match match = regex.Match(line);
                if (match.Success)
                {
                    PdpConnection pdpConnection = new PdpConnection();
                    pdpConnection.ContextId = int.Parse(match.Groups[1].Value);
                    pdpConnection.PdpType = match.Groups[2].Value;
                    pdpConnection.APN = match.Groups[3].Value;
                    pdpConnection.PdpAddress = match.Groups[4].Value;
                    pdpConnection.DataCompressionControl = match.Groups[5].Value;
                    pdpConnection.HeaderCompressionControl = match.Groups[6].Value;
                    connections.Add(pdpConnection);
                }
            }

            lastContextId = 0;
            string dataCompressionValue = PdpConnection.DisabledFlag;
            string headerCompressionValue = PdpConnection.DisabledFlag;
            if (this.DataCompressionControl) dataCompressionValue = PdpConnection.EnabledFlag;
            if (this.HeaderCompressionControl) headerCompressionValue = PdpConnection.EnabledFlag;
            foreach (PdpConnection connection in connections)
            {
                if (connection.ContextId > lastContextId) lastContextId = connection.ContextId;

                if (connection.APN.Equals(APN, StringComparison.OrdinalIgnoreCase) && 
                    connection.HeaderCompressionControl.Equals(headerCompressionValue) &&
                    connection.DataCompressionControl.Equals(dataCompressionValue)                     
                    )
                {
                    this.PdpConnection = connection;
                    // Connection is found, stop the execution
                    this.StopExecution = true;
                    break;
                }
            }
            
            return true;
        }


        /// <summary>
        /// Command preprocessing
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool CreatePdpPreprocess(IContext context, ICommand command)
        {
            if (this.DataCompressionControl || this.HeaderCompressionControl)
            {
                string dataCompressionValue = PdpConnection.DisabledFlag;
                string headerCompressionValue = PdpConnection.DisabledFlag;
                if (this.DataCompressionControl) dataCompressionValue = PdpConnection.EnabledFlag;
                if (this.HeaderCompressionControl) headerCompressionValue = PdpConnection.EnabledFlag;
                command.Request = string.Format(CustomCreatePdpConnectionCommand, lastContextId + 1, StringEnum.GetStringValue(this.InternetProtocol), APN, dataCompressionValue, headerCompressionValue);
            }
            else
            {
                command.Request = string.Format(DefaultCreatePdpConnectionCommand, lastContextId + 1, StringEnum.GetStringValue(this.InternetProtocol),APN);
            }
            return true;
        }

        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool CreatePdpPostprocess(IContext context, ICommand command)
        {
            this.PdpConnection = new PdpConnection();
            this.PdpConnection.ContextId = lastContextId + 1;
            this.PdpConnection.APN = this.APN;

            context.PutResult(Response.Ok);
            return true;
        }



        #endregion ===========================================================================================

        #region =========== Public Properties ===============================================================

        /// <summary>
        /// Gets or sets the APN.
        /// </summary>
        /// <value>The APN.</value>
        public string APN
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets a value indicating whether data compression control is on or off.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if data compression control is on; otherwise, <c>false</c>.
        /// </value>
        public bool DataCompressionControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether header compression control is on or off
        /// </summary>
        /// <value>
        /// 	<c>true</c> if header compression control is on; otherwise, <c>false</c>.
        /// </value>
        public bool HeaderCompressionControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the internet protocol.
        /// </summary>
        /// <value>
        /// The internet protocol.
        /// </value>
        public InternetProtocol InternetProtocol
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the PDP connection.
        /// </summary>
        /// <value>The PDP connection.</value>
        public PdpConnection PdpConnection
        {
            get;
            private set;
        }

        #endregion ==========================================================================================



        #region =========== Public Methods ===================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "SetPdpIpConnectionFeature: Create the PDP IP connection if it does not exist";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of SetPdpIpConnectionFeature
        /// </summary>
        /// <returns>SetPdpIpConnectionFeature instance</returns>
        public static SetPdpIpConnectionFeature NewInstance()
        {
            return new SetPdpIpConnectionFeature();
        }

        #endregion ===========================================================================================

    }
}

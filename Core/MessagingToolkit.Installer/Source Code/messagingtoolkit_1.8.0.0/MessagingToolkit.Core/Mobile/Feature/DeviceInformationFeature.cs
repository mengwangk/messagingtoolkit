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

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Retrieve device information. The followings are retrieved
    /// - model, manufacturer, firmware
    /// </summary>
    internal class DeviceInformationFeature: BaseFeature<DeviceInformationFeature>, IFeature
    {
        #region =========== Variables ====================================================================

        /// <summary>
        /// Device information
        /// </summary>
        private DeviceInformation deviceInformation;

        #endregion ======================================================================================


        #region ================ Private Constants =======================================================

        /// <summary>
        /// Command to retrieve the model
        /// </summary>
        private const string ModelCommand = "AT+CGMM";
        private const string ModelCommand2 = "AT+GMM";

        /// <summary>
        /// Command to retrieve manufacturer
        /// </summary>
        private const string ManufacturerCommand = "AT+CGMI";
        private const string ManufacturerCommand2 = "AT+GMI";

        /// <summary>
        /// Command to retrieve serial no/IMEI
        /// </summary>
        private const string SerialNoCommand = "AT+CGSN";

        /// <summary>
        /// Command to retrieve IMSI
        /// </summary>
        private const string ImsiCommand = "AT+CIMI";

        /// <summary>
        /// Command to retrieve firmware version
        /// </summary>
        private const string FirmwareVersionCommand = "AT+CGMR";
        private const string FirmwareVersionCommand2 = "AT+GMR";




        #endregion =======================================================================================


        #region ====================== Constructor =======================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private DeviceInformationFeature() : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(ModelCommand, Response.Ok, DoNothing, ParseModel, true));
            AddCommand(Command.NewInstance(ModelCommand2, Response.Ok, DoNothing, ParseModel, true));
            AddCommand(Command.NewInstance(ManufacturerCommand, Response.Ok, DoNothing, ParseManufacturer, true));
            AddCommand(Command.NewInstance(ManufacturerCommand2, Response.Ok, DoNothing, ParseManufacturer, true));
            AddCommand(Command.NewInstance(SerialNoCommand, Response.Ok, DoNothing, ParseSerialNo, true));
            AddCommand(Command.NewInstance(ImsiCommand, Response.Ok, DoNothing, ParseImsi, true));
            AddCommand(Command.NewInstance(FirmwareVersionCommand, Response.Ok, DoNothing, ParseFirmwareVersion, true));
            AddCommand(Command.NewInstance(FirmwareVersionCommand2, Response.Ok, DoNothing, ParseFirmwareVersion, true));


            // Create the default device information instance
            deviceInformation = new DeviceInformation();
        }


        #endregion ========================================================================================


        #region =========== Private Methods ===============================================================

        /// <summary>
        /// Parse the result to get the model
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool ParseModel(IContext context, ICommand command)
        {
            if (string.IsNullOrEmpty(deviceInformation.Model))
            {
                string[] results = ResultParser.ParseResponse(context.GetData());
                deviceInformation.Model = results[0];
            }
            return true;
        }

        /// <summary>
        /// Parse the result to get the manufacturer
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool ParseManufacturer(IContext context, ICommand command)
        {
            if (string.IsNullOrEmpty(deviceInformation.Manufacturer))
            {
                string[] results = ResultParser.ParseResponse(context.GetData());
                deviceInformation.Manufacturer = results[0];
            }
            return true;
        }

        /// <summary>
        /// Parse the result to get the serial number
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool ParseSerialNo(IContext context, ICommand command)
        {
            string[] results = ResultParser.ParseResponse(context.GetData());
            deviceInformation.SerialNo = results[0];
            return true;
        }

        /// <summary>
        /// Parse the result to get the IMSI
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool ParseImsi(IContext context, ICommand command)
        {
            string[] results = ResultParser.ParseResponse(context.GetData());
            deviceInformation.Imsi = results[0];
            return true;
        }

        /// <summary>
        /// Parse the result to get the firmware version
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool ParseFirmwareVersion(IContext context, ICommand command)
        {
            if (string.IsNullOrEmpty(deviceInformation.FirmwareVersion))
            {
                string[] results = ResultParser.ParseResponse(context.GetData());
                deviceInformation.FirmwareVersion = results[0];
            }
            return true;            
        }
        #endregion ========================================================================================

        #region =========== Public Methods ================================================================

       
        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "DeviceInformationFeature: Get the device information";
        }

        #endregion =======================================================================================


        #region =========== Public Properties ===========================================================


        /// <summary>
        /// Return the device information
        /// </summary>
        /// <value>Device information</value>
        public DeviceInformation DeviceInformation
        {
            get
            {
                return deviceInformation;
            }
        }

        #endregion ========================================================================================


        #region =========== Public Static Methods ========================================================

        /// <summary>
        /// Return an instance of DeviceInformationFeature
        /// </summary>
        /// <returns>DeviceInformationFeature instance</returns>
        public static DeviceInformationFeature NewInstance()
        {
            return new DeviceInformationFeature();
        }

        #endregion =======================================================================================
       
    }
}


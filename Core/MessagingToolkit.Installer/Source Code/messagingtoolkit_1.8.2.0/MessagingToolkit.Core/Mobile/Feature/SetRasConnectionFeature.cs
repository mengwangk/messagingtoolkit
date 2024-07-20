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

using MessagingToolkit.Core.Ras;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Configure RAS entry
    /// </summary>
    internal class SetRasConnectionFeature : BaseFeature<SetRasConnectionFeature>, IFeature
    {

        #region ================ Private Variables ==================================================================



        #endregion ==================================================================================================


        #region ================ Private Constants ===================================================================


        /// <summary>
        /// Prefix for the dial up connection, used for MMS sending
        /// </summary>
        private const string DataConnectionPrefix = "MessagingToolkit on {0} #{1}";

        /// <summary>
        /// RAS dial up phone number pattern
        /// </summary>
        private const string DataConnectionPhonePattern = "*99***{0}#";


        #endregion ==================================================================================================


        #region ====================== Constructor =================================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        protected SetRasConnectionFeature()
            : base()
        {

        }


        #endregion ====================================================================================================


        #region =========== Public Methods =============================================================================


        /// <summary>
        /// Send the SMS
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <returns>true if successful</returns>
        /// <exception cref="GatewayException">GatewayException is thrown when there is error in execution</exception>
        public override bool Execute(IContext context)
        {           
            // Check if the connection exists
            int connectionIndex = 1;
            while (true)
            {
                string entryName = string.Format(DataConnectionPrefix, Configuration.DeviceName, connectionIndex++);
                string phoneNumber = string.Format(DataConnectionPhonePattern, PdpConnection.ContextId);
                RasPhoneBook rasPhoneBook = new RasPhoneBook();
                rasPhoneBook.Open();
                if (rasPhoneBook.Entries.Contains(entryName))
                {
                    // Verify the existing connection
                    RasEntry entry = rasPhoneBook.Entries[entryName];
                    if (phoneNumber.Equals(entry.PhoneNumber))
                    {
                        this.RasEntryName = entryName;
                        break;   // If the phone number is the same, then exit
                    }
                }
                else
                {
                    // Create the connection                        
                    RasEntry entry = RasEntry.CreateDialUpEntry(entryName, phoneNumber, RasDevice.GetDeviceByName(Configuration.DeviceName, RasDeviceType.Modem));

                    // Add the new entry to the phone book.
                    rasPhoneBook.Entries.Add(entry);
                    if (string.IsNullOrEmpty(Configuration.ProviderAPNAccount))
                    {
                        entry.UpdateCredentials(new System.Net.NetworkCredential(Configuration.ProviderAPNAccount, Configuration.ProviderAPNPassword));
                        entry.Update();
                    }
                    this.RasEntryName = entryName;
                    break;
                }
            }
        
            return true;
        }



        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "SetRasConnectionFeature: Create the RAS connection";
        }

        #endregion ========================================================================================================


        #region =========== Private Methods ===============================================================================


        #endregion ========================================================================================================



        #region =========== Public Properties =============================================================================

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public MobileGatewayConfiguration Configuration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the PDP connection.
        /// </summary>
        /// <value>The PDP connection.</value>
        public PdpConnection PdpConnection
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the name of the RAS entry.
        /// </summary>
        /// <value>The name of the RAS entry.</value>
        public string RasEntryName
        {
            get;
            private set;
        }

        #endregion =========================================================================================================


        #region =========== Protected Methods ==============================================================================



        #endregion =========================================================================================================


        #region =========== Public Static Methods ==========================================================================

        /// <summary>
        /// Return an instance of SetRasConnectionFeature
        /// </summary>
        /// <returns>SetRasConnectionFeature instance</returns>
        public static SetRasConnectionFeature NewInstance()
        {
            return new SetRasConnectionFeature();
        }

        #endregion =========================================================================================================
    }
}

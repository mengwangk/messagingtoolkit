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
namespace MessagingToolkit.Core.Ras
{
    using System;
    using System.ComponentModel;
    using MessagingToolkit.Core.Ras.Internal;

    /// <summary>
    /// Represents options for dialing a remote access service (RAS) entry. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(RasDialOptionsConverter))]
    public sealed class RasDialOptions
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasDialOptions"/> class.
        /// </summary>
        public RasDialOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasDialOptions"/> class.
        /// </summary>
        /// <param name="usePrefixSuffix"><b>true</b> if the prefix and suffix that is in the phone book should be used, otherwise <b>false</b>.</param>
        /// <param name="pausedStates"><b>true</b> if paused states should be accepted, otherwise <b>false</b>.</param>
        /// <param name="setModemSpeaker"><b>true</b> if the modem speaker should be set, otherwise <b>false</b>.</param>
        /// <param name="setSoftwareCompression"><b>true</b> if software compression should be set, otherwise <b>false</b>.</param>
        /// <param name="disableConnectedUI"><b>true</b> if the connected user interface should be disabled, otherwise <b>false</b>.</param>
        /// <param name="disableReconnectUI"><b>true</b> if the reconnect user interface should be disabled, otherwise <b>false</b>.</param>
        /// <param name="disableReconnect"><b>true</b> if reconnect should be disabled, otherwise <b>false</b>.</param>
        /// <param name="noUser"><b>true</b> if no user is present, otherwise <b>false</b>.</param>
        /// <param name="router"><b>true</b> if the connecting device is a router, otherwise <b>false</b>.</param>
        /// <param name="customDial"><b>true</b> if the connection should be dialed normally rather than calling the custom dial entry point of the custom dialer, otherwise <b>false</b>.</param>
        public RasDialOptions(bool usePrefixSuffix, bool pausedStates, bool setModemSpeaker, bool setSoftwareCompression, bool disableConnectedUI, bool disableReconnectUI, bool disableReconnect, bool noUser, bool router, bool customDial)
        {
            this.UsePrefixSuffix = usePrefixSuffix;
            this.PausedStates = pausedStates;
            this.SetModemSpeaker = setModemSpeaker;
            this.SetSoftwareCompression = setSoftwareCompression;
            this.DisableConnectedUI = disableConnectedUI;
            this.DisableReconnectUI = disableReconnectUI;
            this.DisableReconnect = disableReconnect;
            this.NoUser = noUser;
            this.Router = router;
            this.CustomDial = customDial;
        }

#if (WINXP || WIN2K8 || WIN7)

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasDialOptions"/> class.
        /// </summary>
        /// <param name="usePrefixSuffix"><b>true</b> if the prefix and suffix that is in the phone book should be used, otherwise <b>false</b>.</param>
        /// <param name="pausedStates"><b>true</b> if paused states should be accepted, otherwise <b>false</b>.</param>
        /// <param name="setModemSpeaker"><b>true</b> if the modem speaker should be set, otherwise <b>false</b>.</param>
        /// <param name="setSoftwareCompression"><b>true</b> if software compression should be set, otherwise <b>false</b>.</param>
        /// <param name="disableConnectedUI"><b>true</b> if the connected user interface should be disabled, otherwise <b>false</b>.</param>
        /// <param name="disableReconnectUI"><b>true</b> if the reconnect user interface should be disabled, otherwise <b>false</b>.</param>
        /// <param name="disableReconnect"><b>true</b> if reconnect should be disabled, otherwise <b>false</b>.</param>
        /// <param name="noUser"><b>true</b> if no user is present, otherwise <b>false</b>.</param>
        /// <param name="router"><b>true</b> if the connecting device is a router, otherwise <b>false</b>.</param>
        /// <param name="customDial"><b>true</b> if the connection should be dialed normally rather than calling the custom dial entry point of the custom dialer, otherwise <b>false</b>.</param>
        /// <param name="useCustomScripting"><b>true</b> if the dialer should invoke a custom-scripting DLL after establishing the connection to the server, otherwise <b>false</b>.</param>
        public RasDialOptions(bool usePrefixSuffix, bool pausedStates, bool setModemSpeaker, bool setSoftwareCompression, bool disableConnectedUI, bool disableReconnectUI, bool disableReconnect, bool noUser, bool router, bool customDial, bool useCustomScripting)
        {
            this.UsePrefixSuffix = usePrefixSuffix;
            this.PausedStates = pausedStates;
            this.SetModemSpeaker = setModemSpeaker;
            this.SetSoftwareCompression = setSoftwareCompression;
            this.DisableConnectedUI = disableConnectedUI;
            this.DisableReconnectUI = disableReconnectUI;
            this.DisableReconnect = disableReconnect;
            this.NoUser = noUser;
            this.Router = router;
            this.CustomDial = customDial;
            this.UseCustomScripting = useCustomScripting;
        }

#endif

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the prefix and suffix that is in the phone book should be used.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("RDOUsePrefixSuffixDesc")]
        public bool UsePrefixSuffix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to accept paused states.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("RDOPausedStatesDesc")]
        public bool PausedStates
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to set the modem speaker.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("RDOSetModemSpeakerDesc")]
        public bool SetModemSpeaker
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable software compression.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("RDOSetSoftwareCompressionDesc")]
        public bool SetSoftwareCompression
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to disable the connected user interface.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("RDODisableConnectedUIDesc")]
        public bool DisableConnectedUI
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to disable the reconnect user interface.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("RDODisableReconnectUIDesc")]
        public bool DisableReconnectUI
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to disable reconnect.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("RDODisableReconnectDesc")]
        public bool DisableReconnect
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether no user is present.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("RDONoUserDesc")]
        public bool NoUser
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the connecting device is a router.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("RDORouterDesc")]
        public bool Router
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to dial normally instead of calling the custom dial entry point of the custom dialer.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("RDOCustomDialDesc")]
        public bool CustomDial
        {
            get;
            set;
        }

#if (WINXP || WIN2K8 || WIN7)

        /// <summary>
        /// Gets or sets a value indicating whether the dialer should invoke a custom-scripting DLL after establishing the connection to the server.
        /// </summary>
        /// <remarks><b>Windows XP and later:</b> This property is available.</remarks>
        [DefaultValue(false)]
        [SRDescription("RDOUseCustomScriptingDesc")]
        public bool UseCustomScripting
        {
            get;
            set;
        }

#endif

        #endregion
    }
}
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
namespace MessagingToolkit.Core.Ras
{
    using System;
    using System.ComponentModel;
    using MessagingToolkit.Core.Ras.Internal;

    /// <summary>
    /// Represents extensible authentication protocol (EAP) options for dialing a remote access service (RAS) entry. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(RasEapOptionsConverter))]
    public class RasEapOptions
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasEapOptions"/> class.
        /// </summary>
        public RasEapOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasEapOptions"/> class.
        /// </summary>
        /// <param name="nonInteractive"><b>true</b> if the authentication protocol should not display a graphical user interface, otherwise <b>false</b>.</param>
        /// <param name="logOn"><b>true</b> if the user data is obtained from WinLogon, otherwise <b>false</b>.</param>
        /// <param name="preview"><b>true</b> if the user should be prompted for identity information before dialing, otherwise <b>false</b>.</param>
        public RasEapOptions(bool nonInteractive, bool logOn, bool preview)
        {
            this.NonInteractive = nonInteractive;
            this.LogOn = logOn;
            this.Preview = preview;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the authentication protocol should not display a graphical user interface.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("REAPONonInteractiveDesc")]
        public bool NonInteractive
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user data is obtained from WinLogon.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("REAPOLogOnDesc")]
        public bool LogOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user should be prompted for identity information before dialing.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription("REAPOPreviewDesc")]
        public bool Preview
        {
            get;
            set;
        }

        #endregion
    }
}
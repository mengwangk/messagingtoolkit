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

    /// <summary>
    /// Provides data for remote access service (RAS) error events.
    /// </summary>
    [Serializable]
    public class RasErrorEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasErrorEventArgs"/> class.
        /// </summary>
        /// <param name="errorCode">The error code that occurred.</param>
        /// <param name="errorMessage">The error message associated with the error code.</param>
        public RasErrorEventArgs(int errorCode, string errorMessage)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the extended error code (if any) that occurred.
        /// </summary>
        public int ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error message for the error code that occurred.
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        #endregion
    }
}
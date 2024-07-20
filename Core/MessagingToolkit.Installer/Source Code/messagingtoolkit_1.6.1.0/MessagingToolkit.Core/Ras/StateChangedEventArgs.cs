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

    /// <summary>
    /// Provides data for the <see cref="RasDialer.StateChanged"/> event.
    /// </summary>
    public class StateChangedEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.StateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="callbackId">The application defined value that was specified during dialing.</param>
        /// <param name="subEntryId">The one-based index for the phone book entry associated with this connection.</param>
        /// <param name="handle">The handle of the connection.</param>
        /// <param name="state">The state the remote access connection is about to enter.</param>
        /// <param name="errorCode">The error code (if any) that occurred.</param>
        /// <param name="errorMessage">The error message of the <paramref name="errorCode"/> that occurred.</param>
        /// <param name="extendedErrorCode">The extended error code (if any) that occurred.</param>
        internal StateChangedEventArgs(IntPtr callbackId, int subEntryId, RasHandle handle, RasConnectionState state, int errorCode, string errorMessage, int extendedErrorCode)
        {
            this.CallbackId = callbackId;
            this.SubEntryId = subEntryId;
            this.Handle = handle;
            this.State = state;
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
            this.ExtendedErrorCode = extendedErrorCode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the application defined callback id.
        /// </summary>
        public IntPtr CallbackId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the one-based index for the phone book entry associated with this connection.
        /// </summary>
        public int SubEntryId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the handle of the connection.
        /// </summary>
        public RasHandle Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the state the remote access connection is about to enter.
        /// </summary>
        public RasConnectionState State
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error code (if any) that occurred.
        /// </summary>
        public int ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error message for the <see cref="ErrorCode"/> that occurred.
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the extended error code (if any) that occurred.
        /// </summary>
        public int ExtendedErrorCode
        {
            get;
            private set;
        }

        #endregion
    }
}
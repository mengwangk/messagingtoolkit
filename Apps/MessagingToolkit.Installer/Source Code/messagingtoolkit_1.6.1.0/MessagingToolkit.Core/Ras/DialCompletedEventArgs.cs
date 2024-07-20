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
    /// Provides data for the <see cref="RasDialer.DialCompleted"/> event.
    /// </summary>
    public class DialCompletedEventArgs : AsyncCompletedEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.DialCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="handle">The handle whose connection attempt completed.</param>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="cancelled"><b>true</b> if the asynchronous operation was cancelled, otherwise <b>false</b>.</param>
        /// <param name="timedOut"><b>true</b> if the operation timed out, otherwise <b>false</b>.</param>
        /// <param name="connected"><b>true</b> if the connection attempt successfully connected, otherwise <b>false</b>.</param>
        /// <param name="userState">The optional user-supplied state object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "cancelled", Justification = "The name is ok. Matching the argument name in the base constructor.")]
        public DialCompletedEventArgs(RasHandle handle, Exception error, bool cancelled, bool timedOut, bool connected, object userState)
            : base(error, cancelled, userState)
        {
            this.Handle = handle;
            this.TimedOut = timedOut;
            this.Connected = connected;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the handle whose connection attempt completed.
        /// </summary>
        public RasHandle Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the asynchronous dial attempt timed out.
        /// </summary>
        public bool TimedOut
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the connection attempt successfully connected.
        /// </summary>
        public bool Connected
        {
            get;
            private set;
        }

        #endregion
    }
}
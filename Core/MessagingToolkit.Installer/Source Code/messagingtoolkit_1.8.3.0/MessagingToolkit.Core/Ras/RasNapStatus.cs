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

#if (WIN2K8 || WIN7)

    /// <summary>
    /// Represents the current network access protection (NAP) status of a remote access connection. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Known Limitations:</b>
    /// <list type="bullet">
    /// <item>This type is only available on Windows Vista and later operating systems.</item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class RasNapStatus
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasNapStatus"/> class.
        /// </summary>
        /// <param name="isolationState">The isolation state for the remote access connection.</param>
        /// <param name="probationTime">The time required for the connection to come out of quarantine.</param>
        internal RasNapStatus(RasIsolationState isolationState, DateTime probationTime)
        {
            this.IsolationState = isolationState;
            this.ProbationTime = probationTime;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the isolation state.
        /// </summary>
        public RasIsolationState IsolationState
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the probation time.
        /// </summary>
        /// <remarks>Specifies the time required for the connection to come out of quarantine after which the connection will be dropped.</remarks>
        public DateTime ProbationTime
        {
            get;
            private set;
        }

        #endregion
    }

#endif
}
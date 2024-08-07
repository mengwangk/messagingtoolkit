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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Net;
    using MessagingToolkit.Core.Ras.Internal;
    using MessagingToolkit.Core.Properties;

    /// <summary>
    /// Represents a remote access connection. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Known Limitations</b>
    /// <list type="bullet">
    /// <item>A connection is not considered active until the connection has completed connecting successfully. If the connection attempt is still under way, the connection will not be available from <see cref="GetActiveConnections"/> or other related methods.</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows how to disconnect all active connections on the machine.
    /// <code lang="C#">
    /// <![CDATA[
    /// ReadOnlyCollection<RasConnection> connections = RasConnection.GetActiveConnections();
    /// foreach (RasConnection connection in connections)
    /// {
    ///     connection.HangUp();
    /// }
    /// ]]>        
    /// </code>
    /// <code lang="VB.NET">
    /// <![CDATA[
    /// Dim connections As ReadOnlyCollection(Of RasConnection) = RasConnection.GetActiveConnections()
    /// For Each connection As RasConnection In connections
    ///     connection.HangUp()
    /// Next
    /// ]]>
    /// </code>
    /// </example>
    [DebuggerDisplay("EntryName = {EntryName}")]
    public sealed class RasConnection : MarshalByRefObject
    {
        #region Fields

#if (WINXP || WIN2K8 || WIN7)

        private RasConnectionOptions connectionOptions;

#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasConnection"/> class.
        /// </summary>
        internal RasConnection()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the handle of the connection.
        /// </summary>
        public RasHandle Handle
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the name of the phone book entry used to establish the remote access connection.
        /// </summary>
        /// <remarks>If the connection was established without using an entry name, this member contains a PERIOD (.) followed by the phone number.</remarks>
        public string EntryName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the device through which the connection has been established.
        /// </summary>
        public RasDevice Device
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the full path and filename to the phone book (PBK) containing the entry for this connection.
        /// </summary>
        public string PhoneBookPath
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the one-based subentry index of the connected link in a multilink connection.
        /// </summary>
        public int SubEntryId
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the <see cref="System.Guid"/> that represents the phone book entry.
        /// </summary>
        public Guid EntryId
        {
            get;
            internal set;
        }

#if (WINXP || WIN2K8 || WIN7)

        /// <summary>
        /// Gets the connection options.
        /// </summary>
        /// <remarks><b>Windows XP and later:</b> This property is available.</remarks>
        public RasConnectionOptions ConnectionOptions
        {
            get
            {
                if (this.connectionOptions == null)
                {
                    this.connectionOptions = new RasConnectionOptions();
                }

                return this.connectionOptions;
            }

            internal set
            {
                this.connectionOptions = value;
            }
        }

        /// <summary>
        /// Gets the logon session id in which the connection was established.
        /// </summary>
        /// <remarks><b>Windows XP and later:</b> This property is available.</remarks>
        public Luid SessionId
        {
            get;
            internal set;
        }

#endif

#if (WIN2K8 || WIN7)

        /// <summary>
        /// Gets the correlation id.
        /// </summary>
        /// <remarks><b>Windows Vista and later:</b> This property is available.</remarks>
        public Guid CorrelationId
        {
            get;
            internal set;
        }

#endif

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves an active connection by the handle.
        /// </summary>
        /// <param name="handle">The connection handle.</param>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasConnection"/> if the connection was found; otherwise, a null reference (<b>Nothing</b> in Visual Basic).</returns>
        /// <example>
        /// The following example shows how to retrieve an active connection from the handle.
        /// <code lang="C#">
        /// <![CDATA[
        /// RasHandle handle = null;
        /// using (RasDialer dialer = new RasDialer())
        /// {
        ///     dialer.EntryName = "VPN Connection";
        ///     dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
        ///     handle = dialer.Dial();
        /// }
        /// RasConnection connection = RasConnection.GetActiveConnectionByHandle(handle);
        /// ]]>
        /// </code>
        /// <code lang="VB.NET">
        /// <![CDATA[
        /// Dim handle As RasHandle
        /// Dim dialer As RasDialer
        /// Try
        ///     dialer = New RasDialer
        ///     dialer.EntryName = "VPN Connection"
        ///     dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers)
        ///     handle = dialer.Dial()
        /// Finally
        ///     If dialer IsNot Nothing Then
        ///         dialer.Dispose()
        ///     End If
        /// End Try
        /// RasConnection connection = RasConnection.GetActiveConnectionByHandle(handle)
        /// ]]>
        /// </code>
        /// </example>
        public static RasConnection GetActiveConnectionByHandle(RasHandle handle)
        {
            if (handle == null)
            {
                ThrowHelper.ThrowArgumentNullException("handle");
            }

            RasConnection retval = null;

            foreach (RasConnection connection in GetActiveConnections())
            {
                if (connection.Handle == handle)
                {
                    retval = connection;
                    break;
                }
            }

            return retval;
        }

        /// <summary>
        /// Retrieves an active connection by the entry name and phone book path.
        /// </summary>
        /// <param name="entryName">The name of the entry.</param>
        /// <param name="phoneBookPath">The path (including filename) of the phone book containing the entry.</param>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasConnection"/> if the connection was found; otherwise, a null reference (<b>Nothing</b> in Visual Basic).</returns>
        public static RasConnection GetActiveConnectionByName(string entryName, string phoneBookPath)
        {
            return GetActiveConnectionByName(entryName, phoneBookPath, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Retrieves an active connection by the entry name and phone book path.
        /// </summary>
        /// <param name="entryName">The name of the entry.</param>
        /// <param name="phoneBookPath">The path (including filename) of the phone book containing the entry.</param>
        /// <param name="comparisonType">The type of string comparison to perform.</param>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasConnection"/> if the connection was found; otherwise, a null reference (<b>Nothing</b> in Visual Basic).</returns>
        public static RasConnection GetActiveConnectionByName(string entryName, string phoneBookPath, StringComparison comparisonType)
        {
            if (string.IsNullOrEmpty(entryName))
            {
                ThrowHelper.ThrowArgumentException("entryName", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            if (string.IsNullOrEmpty(phoneBookPath))
            {
                ThrowHelper.ThrowArgumentException("phoneBookPath", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            // Determine the full path to the phone book, relative paths will not work here since Windows reports the full path.
            string path = System.IO.Path.GetFullPath(phoneBookPath);

            RasConnection retval = null;

            foreach (RasConnection connection in GetActiveConnections())
            {
                if (connection.EntryName != null && connection.EntryName.Equals(entryName, comparisonType) && connection.PhoneBookPath != null && connection.PhoneBookPath.Equals(path, comparisonType))
                {
                    retval = connection;
                    break;
                }
            }

            return retval;
        }

        /// <summary>
        /// Retrieves an active connection by the entry id.
        /// </summary>
        /// <param name="entryId">The entry id of the connection.</param>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasConnection"/> if the connection was found; otherwise, a null reference (<b>Nothing</b> in Visual Basic).</returns>
        public static RasConnection GetActiveConnectionById(Guid entryId)
        {
            RasConnection retval = null;

            foreach (RasConnection connection in GetActiveConnections())
            {
                if (connection.EntryId == entryId)
                {
                    retval = connection;
                    break;
                }
            }

            return retval;
        }

        /// <summary>
        /// Retrieves a read-only list of active connections.
        /// </summary>
        /// <returns>A new read-only collection of <see cref="MessagingToolkit.Core.Ras.RasConnection"/> objects, or an empty collection if no active connections were found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This should not be a property.")]
        public static ReadOnlyCollection<RasConnection> GetActiveConnections()
        {
            return RasHelper.Instance.GetActiveConnections();
        }

        /// <summary>
        /// Retrieves the connection status.
        /// </summary>
        /// <returns>An <see cref="MessagingToolkit.Core.Ras.RasConnectionStatus"/> containing connection status information.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This should not be a property.")]
        public RasConnectionStatus GetConnectionStatus()
        {
            return RasHelper.Instance.GetConnectionStatus(this.Handle);
        }

        /// <summary>
        /// Retrieves accumulated statistics for the connection.
        /// </summary>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasLinkStatistics"/> object containing connection statistics.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This should not be a property.")]
        public RasLinkStatistics GetConnectionStatistics()
        {
            return RasHelper.Instance.GetConnectionStatistics(this.Handle);
        }

        /// <summary>
        /// Clears any accumulated statistics for the connection.
        /// </summary>
        /// <returns><b>true</b> if the function succeeds, otherwise <b>false</b>.</returns>
        public bool ClearConnectionStatistics()
        {
            return RasHelper.Instance.ClearConnectionStatistics(this.Handle);
        }

        /// <summary>
        /// Clears any accumulated statistics for the link in a multilink connection.
        /// </summary>
        /// <returns><b>true</b> if the function succeeds, otherwise <b>false</b>.</returns>
        public bool ClearLinkStatistics()
        {
            return RasHelper.Instance.ClearLinkStatistics(this.Handle, this.SubEntryId);
        }

        /// <summary>
        /// Retrieves accumulated statistics for the link in a multilink connection.
        /// </summary>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasLinkStatistics"/> object containing connection statistics.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This should not be a property.")]
        public RasLinkStatistics GetLinkStatistics()
        {
            return RasHelper.Instance.GetLinkStatistics(this.Handle, this.SubEntryId);
        }

#if (WIN2K8 || WIN7)

        /// <summary>
        /// Retrieves the network access protection (NAP) status for a remote access connection.
        /// </summary>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasNapStatus"/> object containing the NAP status.</returns>
        /// <remarks><b>Windows Vista and later:</b> This property is available.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This should not be a property.")]
        public RasNapStatus GetNapStatus()
        {
            return RasHelper.Instance.GetNapStatus(this.Handle);
        }

#endif

        /// <summary>
        /// Retrieves information about a remote access projection operation.
        /// </summary>
        /// <param name="projectionType">The protocol of interest.</param>
        /// <returns>The resulting projection information; otherwise, a null reference (<b>Nothing</b> in Visual Basic) if the protocol was not found.</returns>
        /// <remarks>
        /// The type of projection operation performed determines the type of object that is returned.
        /// </remarks>
        public object GetProjectionInfo(RasProjectionType projectionType)
        {
#if (WIN7)

            if (projectionType == RasProjectionType.Ppp || projectionType == RasProjectionType.IkeV2)
            {
                // The projection type requested is for the new projection types, pull it from the new extended method.
                object retval = RasHelper.Instance.GetProjectionInfoEx(this.Handle);
                if (retval != null && ((projectionType == RasProjectionType.Ppp && (!(retval is RasPppInfo))) || (projectionType == RasProjectionType.IkeV2 && (!(retval is RasIkeV2Info)))))
                {
                    retval = null;
                }

                return retval;
            }

#endif

            // Use the standard projection method to retrieve the data.
            NativeMethods.RASPROJECTION projection = NativeMethods.RASPROJECTION.Amb;
            switch (projectionType)
            {
                case RasProjectionType.Amb:
                    projection = NativeMethods.RASPROJECTION.Amb;
                    break;

                case RasProjectionType.Nbf:
                    projection = NativeMethods.RASPROJECTION.Nbf;
                    break;

                case RasProjectionType.Ipx:
                    projection = NativeMethods.RASPROJECTION.Ipx;
                    break;

                case RasProjectionType.IP:
                    projection = NativeMethods.RASPROJECTION.IP;
                    break;

                case RasProjectionType.Ccp:
                    projection = NativeMethods.RASPROJECTION.Ccp;
                    break;

                case RasProjectionType.Lcp:
                    projection = NativeMethods.RASPROJECTION.Lcp;
                    break;

                case RasProjectionType.Slip:
                    projection = NativeMethods.RASPROJECTION.Slip;
                    break;

#if (WIN2K8 || WIN7)

                case RasProjectionType.IPv6:
                    projection = NativeMethods.RASPROJECTION.IPv6;
                    break;

#endif
            }

            return RasHelper.Instance.GetProjectionInfo(this.Handle, projection);
        }

        /// <summary>
        /// Retrieves a connection handle for a subentry of a multilink connection.
        /// </summary>
        /// <param name="subEntryId">The one-based index of the subentry to whose handle to retrieve.</param>
        /// <returns>The handle of the subentry if available, otherwise a null reference (<b>Nothing</b> in Visual Basic).</returns>
        /// <exception cref="System.ArgumentException"><paramref name="subEntryId"/> cannot be less than or equal to zero.</exception>
        public RasHandle GetSubEntryHandle(int subEntryId)
        {
            if (subEntryId <= 0)
            {
                ThrowHelper.ThrowArgumentException("subEntryId", Resources.Argument_ValueCannotBeLessThanOrEqualToZero);
            }

            return RasHelper.Instance.GetSubEntryHandle(this.Handle, subEntryId);
        }

        /// <summary>
        /// Terminates the remote access connection.
        /// </summary>
        public void HangUp()
        {
            this.HangUp(NativeMethods.HangUpPollingInterval);
        }

        /// <summary>
        /// Terminates the remote access connection.
        /// </summary>
        /// <param name="pollingInterval">The length of time, in milliseconds, the thread must be paused while polling whether the connection has terminated.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="pollingInterval"/> must be greater than or equal to zero.</exception>
        public void HangUp(int pollingInterval)
        {
            if (pollingInterval < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("pollingInterval", pollingInterval, Resources.Argument_ValueCannotBeLessThanZero);
            }

            RasHelper.Instance.HangUp(this.Handle, pollingInterval);
        }

#if (WIN7)

        /// <summary>
        /// Updates the tunnel endpoints of an Internet Key Exchange (IKEv2) connection.
        /// </summary>
        /// <param name="interfaceIndex">The new interface index of the endpoint.</param>
        /// <param name="localEndPoint">The new local client endpoint of the connection.</param>
        /// <param name="remoteEndPoint">The new remote server endpoint of the connection.</param>
        /// <remarks><b>Windows 7 and later:</b> This property is available.</remarks>
        public void UpdateConnection(int interfaceIndex, IPAddress localEndPoint, IPAddress remoteEndPoint)
        {
            RasHelper.Instance.UpdateConnection(this.Handle, interfaceIndex, localEndPoint, remoteEndPoint);
        }

#endif

        #endregion
    }
}
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

namespace MessagingToolkit.Core.Ras.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using MessagingToolkit.Core.Properties;

    /// <summary>
    /// Provides methods to interact with the remote access service (RAS) application programming interface.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The design is ok.")]
    internal class RasHelper : IRasHelper
    {
        #region Fields

        /// <summary>
        /// Contains the instance used to handle calls.
        /// </summary>
        private static IRasHelper instance;

        #endregion

        #region Constructors
       
        /// <summary>
        /// Prevents a default instance of the <see cref="RasHelper"/> class from being created.
        /// </summary>
        private RasHelper()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the instance of the <see cref="IRasHelper"/> class to handle calls.
        /// </summary>
        public static IRasHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RasHelper();
                }

                return instance;
            }

            set
            {
                instance = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates a new locally unique identifier.
        /// </summary>
        /// <returns>A new <see cref="MessagingToolkit.Core.Ras.Luid"/> structure.</returns>
        public Luid AllocateLocallyUniqueId()
        {
            Luid retval = Luid.Empty;

            int size = Marshal.SizeOf(typeof(Luid));

            IntPtr pLuid = IntPtr.Zero;
            try
            {
                pLuid = Marshal.AllocHGlobal(size);

                int ret = SafeNativeMethods.Instance.AllocateLocallyUniqueIdImpl(pLuid);
                if (ret == NativeMethods.SUCCESS)
                {
                    retval = (Luid)Marshal.PtrToStructure(pLuid, typeof(Luid));
                }
                else
                {
                    ThrowHelper.ThrowWin32Exception();
                }
            }
            finally
            {
                if (pLuid != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pLuid);
                }
            }

            return retval;
        }

        /// <summary>
        /// Establishes a remote access connection between a client and a server.
        /// </summary>
        /// <param name="phoneBookPath">The full path (including filename) of a phone book. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="parameters">A <see cref="NativeMethods.RASDIALPARAMS"/> structure containing calling parameters for the connection.</param>
        /// <param name="extensions">A <see cref="NativeMethods.RASDIALEXTENSIONS"/> structure containing extended feature information.</param>
        /// <param name="callback">A <see cref="NativeMethods.RasDialFunc2"/> delegate to notify during the connection process.</param>
        /// <param name="eapOptions">Specifies options to use during authentication.</param>
        /// <returns>The handle of the connection.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="parameters"/> contains an empty or null reference (<b>Nothing</b> in Visual Basic) for both the entry name and phone numbers.</exception>
        public RasHandle Dial(string phoneBookPath, NativeMethods.RASDIALPARAMS parameters, NativeMethods.RASDIALEXTENSIONS extensions, NativeMethods.RasDialFunc2 callback, NativeMethods.RASEAPF eapOptions)
        {
            if (string.IsNullOrEmpty(parameters.entryName) && string.IsNullOrEmpty(parameters.phoneNumber))
            {
                ThrowHelper.ThrowArgumentException("parameters", Resources.Argument_EmptyEntryNameAndPhoneNumber);
            }

            if (phoneBookPath != null && phoneBookPath.Length == 0)
            {
                // The phone book path provided was an empty string, set it to null to use the default phone book.
                phoneBookPath = null;
            }

            IntPtr lpRasDialExtensions = IntPtr.Zero;
            RasHandle handle = null;

            try
            {
                int extensionsSize = Marshal.SizeOf(typeof(NativeMethods.RASDIALEXTENSIONS));
                extensions.size = extensionsSize;

#if (WIN7)
                extensions.devSpecificInfo.size = Marshal.SizeOf(typeof(NativeMethods.RASDEVSPECIFICINFO));
#endif

                lpRasDialExtensions = Marshal.AllocHGlobal(extensionsSize);
                Marshal.StructureToPtr(extensions, lpRasDialExtensions, true);

                IntPtr lpRasDialParams = IntPtr.Zero;
                try
                {
                    int parametersSize = Marshal.SizeOf(typeof(NativeMethods.RASDIALPARAMS));
                    parameters.size = parametersSize;

                    lpRasDialParams = Marshal.AllocHGlobal(parametersSize);
                    Marshal.StructureToPtr(parameters, lpRasDialParams, true);

                    int ret = SafeNativeMethods.Instance.Dial(lpRasDialExtensions, phoneBookPath, lpRasDialParams, NativeMethods.RasNotifierType.RasDialFunc2, callback, out handle);
                    if (ret != NativeMethods.SUCCESS)
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (Exception)
                {
                    // An unexpected exception occurred while dialing, check whether the connection handle should be closed before exiting. This check is being performed here
                    // due to other exceptions that may occur within RasDial that bypass the API result check.
                    if (handle != null && !handle.IsInvalid)
                    {
                        // There was an error during the connection attempt, the handle must be released.
                        RasHelper.Instance.HangUp(handle, NativeMethods.HangUpPollingInterval);
                    }

                    throw;
                }
                finally
                {
                    if (lpRasDialParams != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(lpRasDialParams);
                    }
                }
            }
            finally
            {
                if (lpRasDialExtensions != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpRasDialExtensions);
                }
            }

            return handle;
        }

        /// <summary>
        /// Indicates the current AutoDial status for a specific TAPI dialing location.
        /// </summary>
        /// <param name="dialingLocation">The dialing location whose status to check.</param>
        /// <returns><b>true</b> if the AutoDial feature is currently enabled for the dialing location, otherwise <b>false</b>.</returns>
        public bool GetAutoDialEnable(int dialingLocation)
        {
            RasGetAutodialEnableParams info = new RasGetAutodialEnableParams();
            info.DialingLocation = dialingLocation;

            try
            {
                int ret = UnsafeNativeMethods.Instance.GetAutodialEnable(info);
                if (ret != NativeMethods.SUCCESS)
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }

            return info.Enabled;
        }

        /// <summary>
        /// Retrieves the value of an AutoDial parameter.
        /// </summary>
        /// <param name="parameter">The parameter whose value to retrieve.</param>
        /// <returns>The value of the parameter.</returns>
        public int GetAutoDialParameter(NativeMethods.RASADP parameter)
        {
            int retval = 0;

            RasGetAutodialParamParams info = new RasGetAutodialParamParams();
            info.BufferSize = sizeof(int);

            bool retry = false;
            do
            {
                try
                {
                    info.Address = Marshal.AllocHGlobal(info.BufferSize);

                    int ret = UnsafeNativeMethods.Instance.GetAutodialParam(info);
                    if (ret == NativeMethods.SUCCESS)
                    {
                        if (info.BufferSize == sizeof(int))
                        {
                            retval = Marshal.ReadInt32(info.Address);
                        }
                        else
                        {
                            ThrowHelper.ThrowInvalidOperationException(Resources.Exception_UnexpectedSizeReturned);
                        }

                        retry = false;
                    }
                    else if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                    {
                        retry = true;
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
                finally
                {
                    if (info.Address != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(info.Address);
                    }
                }
            }
            while (retry);

            return retval;
        }

        /// <summary>
        /// Clears any accumulated statistics for the specified remote access connection.
        /// </summary>
        /// <param name="handle">The handle to the connection.</param>
        /// <returns><b>true</b> if the function succeeds, otherwise <b>false</b>.</returns>
        public bool ClearConnectionStatistics(RasHandle handle)
        {
            bool retval = false;

            try
            {
                int ret = SafeNativeMethods.Instance.ClearConnectionStatistics(handle);
                if (ret == NativeMethods.SUCCESS)
                {
                    retval = true;
                }
                else if (ret == NativeMethods.ERROR_INVALID_HANDLE)
                {
                    ThrowHelper.ThrowInvalidHandleException(handle, "handle", Resources.Argument_InvalidHandle);
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }

            return retval;
        }

        /// <summary>
        /// Clears any accumulated statistics for the specified link in a remote access multilink connection.
        /// </summary>
        /// <param name="handle">The handle to the connection.</param>
        /// <param name="subEntryId">The subentry index that corresponds to the link for which to clear statistics.</param>
        /// <returns><b>true</b> if the function succeeds, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="subEntryId"/> must be greater than zero.</exception>
        public bool ClearLinkStatistics(RasHandle handle, int subEntryId)
        {
            if (subEntryId <= 0)
            {
                ThrowHelper.ThrowArgumentException("subEntryId", Resources.Argument_ValueCannotBeLessThanOrEqualToZero);
            }

            bool retval = false;

            try
            {
                int ret = SafeNativeMethods.Instance.ClearLinkStatistics(handle, subEntryId);
                if (ret == NativeMethods.SUCCESS)
                {
                    retval = true;
                }
                else if (ret == NativeMethods.ERROR_INVALID_HANDLE)
                {
                    ThrowHelper.ThrowInvalidHandleException(handle, "handle", Resources.Argument_InvalidHandle);
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }

            return retval;
        }

        /// <summary>
        /// Deletes an entry from a phone book.
        /// </summary>
        /// <param name="phoneBookPath">Required. The full path (including file name) of the phone book.</param>
        /// <param name="entryName">Required. The name of the entry to delete.</param>
        /// <returns><b>true</b> if the entry was deleted, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="phoneBookPath"/> or <paramref name="entryName"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public bool DeleteEntry(string phoneBookPath, string entryName)
        {
            if (string.IsNullOrEmpty(phoneBookPath))
            {
                ThrowHelper.ThrowArgumentException("phoneBookPath", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            if (string.IsNullOrEmpty(entryName))
            {
                ThrowHelper.ThrowArgumentException("entryName", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            bool retval = false;

            try
            {
                int ret = UnsafeNativeMethods.Instance.DeleteEntry(phoneBookPath, entryName);
                if (ret == NativeMethods.SUCCESS)
                {
                    retval = true;
                }
                else if (ret == NativeMethods.ERROR_ACCESS_DENIED)
                {
                    ThrowHelper.ThrowUnauthorizedAccessException(Resources.Exception_AccessDeniedBySecurity);
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }

            return retval;
        }

#if (WINXP || WIN2K8 || WIN7)
        /// <summary>
        /// Deletes a subentry from the specified phone book entry.
        /// </summary>
        /// <param name="phoneBook">Required. The <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> containing the entry.</param>
        /// <param name="entry">Required. The <see cref="MessagingToolkit.Core.Ras.RasEntry"/> containing the subentry to be deleted.</param>
        /// <param name="subEntryId">The one-based index of the subentry to delete.</param>
        /// <returns><b>true</b> if the function succeeds, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBook"/> or <paramref name="entry"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public bool DeleteSubEntry(RasPhoneBook phoneBook, RasEntry entry, int subEntryId)
        {
            if (phoneBook == null)
            {
                ThrowHelper.ThrowArgumentNullException("phoneBook");
            }

            if (entry == null)
            {
                ThrowHelper.ThrowArgumentNullException("entry");
            }

            if (subEntryId <= 0)
            {
                ThrowHelper.ThrowArgumentException("subEntryId", Resources.Argument_ValueCannotBeLessThanOrEqualToZero);
            }

            bool retval = false;

            int ret = UnsafeNativeMethods.Instance.DeleteSubEntry(phoneBook.Path, entry.Name, subEntryId);
            if (ret == NativeMethods.SUCCESS)
            {
                retval = true;
            }
            else
            {
                ThrowHelper.ThrowRasException(ret);
            }

            return retval;
        }
#endif

        /// <summary>
        /// Retrieves a read-only list of active connections.
        /// </summary>
        /// <returns>A new read-only collection of <see cref="MessagingToolkit.Core.Ras.RasConnection"/> objects, or an empty collection if no active connections were found.</returns>
        public ReadOnlyCollection<RasConnection> GetActiveConnections()
        {
            ReadOnlyCollection<RasConnection> retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RASCONN));

            StructBufferedPInvokeParams info = new StructBufferedPInvokeParams();
            info.BufferSize = new IntPtr(size);
            info.Count = IntPtr.Zero;

            bool retry = false;

            do
            {
                NativeMethods.RASCONN conn = new NativeMethods.RASCONN();
                conn.size = size;

                try
                {
                    info.Address = Marshal.AllocHGlobal(info.BufferSize);
                    Marshal.StructureToPtr(conn, info.Address, true);
                   
                    int ret = SafeNativeMethods.Instance.EnumConnections(info);
                    if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                    {
                        retry = true;
                    }
                    else if (ret == NativeMethods.SUCCESS)
                    {
                        retry = false;

                        NativeMethods.RASCONN[] connections = Utilities.CreateArrayOfType<NativeMethods.RASCONN>(
                            info.Address, size, info.Count.ToInt32());
                        RasConnection[] tempArray = null;

                        if (connections == null || connections.Length == 0)
                        {
                            tempArray = new RasConnection[0];
                        }
                        else
                        {
                            tempArray = new RasConnection[connections.Length];

                            for (int index = 0; index < connections.Length; index++)
                            {
                                NativeMethods.RASCONN current = connections[index];

                                RasConnection item = new RasConnection();

                                item.Handle = new RasHandle(current.handle, current.subEntryId > 1);
                                item.EntryName = current.entryName;
                                item.Device = RasDevice.Create(current.deviceName, current.deviceType);
                                item.PhoneBookPath = current.phoneBook;
                                item.SubEntryId = current.subEntryId;
                                item.EntryId = current.entryId;

#if (WINXP || WIN2K8 || WIN7)

                                item.SessionId = current.sessionId;
                                Utilities.SetRasConnectionOptions(item, current.connectionOptions);

#endif
#if (WIN2K8 || WIN7)

                                item.CorrelationId = current.correlationId;

#endif

                                tempArray[index] = item;
                            }
                        }

                        retval = new ReadOnlyCollection<RasConnection>(tempArray);
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
                finally
                {
                    if (info.Address != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(info.Address);
                    }
                }
            } 
            while (retry);

            return retval;
        }

        /// <summary>
        /// Retrieves information about the entries associated with a network address in the AutoDial mapping database.
        /// </summary>
        /// <param name="address">The address to retrieve.</param>
        /// <returns>A new <see cref="MessagingToolkit.Core.Ras.RasAutoDialAddress"/> object.</returns>
        public RasAutoDialAddress GetAutoDialAddress(string address)
        {
            RasAutoDialAddress retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RASAUTODIALENTRY));

            RasGetAutodialAddressParams info = new RasGetAutodialAddressParams();
            info.BufferSize = new IntPtr(size);
            info.Count = IntPtr.Zero;

            bool retry = false;

            do
            {
                NativeMethods.RASAUTODIALENTRY entry = new NativeMethods.RASAUTODIALENTRY();
                entry.size = size;

                try
                {
                    info.Address = Marshal.AllocHGlobal(info.BufferSize);
                    Marshal.StructureToPtr(entry, info.Address, true);

                    int ret = UnsafeNativeMethods.Instance.GetAutodialAddress(info);
                    if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                    {
                        retry = true;
                    }
                    else if (ret == NativeMethods.SUCCESS)
                    {
                        retry = false;

                        NativeMethods.RASAUTODIALENTRY[] entries = Utilities.CreateArrayOfType<NativeMethods.RASAUTODIALENTRY>(info.Address, size, info.Count.ToInt32());
                        retval = new RasAutoDialAddress(address);

                        if (entries != null || entries.Length > 0)
                        {
                            for (int index = 0; index < entries.Length; index++)
                            {
                                NativeMethods.RASAUTODIALENTRY current = entries[index];
                                retval.Entries.Add(new RasAutoDialEntry(current.dialingLocation, current.entryName));
                            }
                        }
                    }
                    else if (ret == NativeMethods.ERROR_FILE_NOT_FOUND)
                    {
                        retry = false;
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
                finally
                {
                    if (info.Address != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(info.Address);
                    }
                }
            }
            while (retry);

            return retval;
        }

        /// <summary>
        /// Retrieves a collection of addresses in the AutoDial mapping database.
        /// </summary>
        /// <returns>A new collection of <see cref="MessagingToolkit.Core.Ras.RasAutoDialAddress"/> objects, or an empty collection if no addresses were found.</returns>
        public Collection<string> GetAutoDialAddresses()
        {
            Collection<string> retval = null;

            StructBufferedPInvokeParams info = new StructBufferedPInvokeParams();
            info.BufferSize = IntPtr.Zero;
            info.Count = IntPtr.Zero;

            bool retry = false;

            do
            {
                try
                {
                    info.Address = Marshal.AllocHGlobal(info.BufferSize);

                    int ret = UnsafeNativeMethods.Instance.EnumAutodialAddresses(info);
                    if (ret == NativeMethods.SUCCESS)
                    {
                        if (info.Count.ToInt32() > 0)
                        {
                            retval = Utilities.CreateStringCollectionByCount(info.Address, info.Count.ToInt32() * IntPtr.Size, info.Count.ToInt32());
                        }

                        retry = false;
                    }
                    else if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                    {
                        retry = true;
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
                finally
                {
                    if (info.Address != IntPtr.Zero)
                    {
                        Marshal.AllocHGlobal(info.Address);
                    }
                }
            }
            while (retry);

            return retval;
        }

        /// <summary>
        /// Retrieves the connection status for the handle specified.
        /// </summary>
        /// <param name="handle">The remote access connection handle to retrieve.</param>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasConnectionStatus"/> object containing connection status information.</returns>
        /// <exception cref="MessagingToolkit.Core.Ras.InvalidHandleException"><paramref name="handle"/> is not a valid handle.</exception>
        public RasConnectionStatus GetConnectionStatus(RasHandle handle)
        {
            RasConnectionStatus retval = null;

            IntPtr lpRasConnStatus = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(typeof(NativeMethods.RASCONNSTATUS));

                NativeMethods.RASCONNSTATUS status = new NativeMethods.RASCONNSTATUS();
                status.size = size;

                lpRasConnStatus = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(status, lpRasConnStatus, true);

                int ret = SafeNativeMethods.Instance.GetConnectStatus(handle, lpRasConnStatus);
                if (ret == NativeMethods.ERROR_INVALID_HANDLE)
                {
                    ThrowHelper.ThrowInvalidHandleException(handle, "handle", Resources.Argument_InvalidHandle);
                }
                else if (ret == NativeMethods.SUCCESS)
                {
                    status = (NativeMethods.RASCONNSTATUS)Marshal.PtrToStructure(lpRasConnStatus, typeof(NativeMethods.RASCONNSTATUS));

                    string errorMessage = null;
                    if (status.errorCode != NativeMethods.SUCCESS)
                    {
                        errorMessage = RasHelper.Instance.GetRasErrorString(status.errorCode);
                    }

                    retval = new RasConnectionStatus();
                    retval.ConnectionState = status.connectionState;
                    retval.ErrorCode = status.errorCode;
                    retval.ErrorMessage = errorMessage;
                    retval.PhoneNumber = status.phoneNumber;

                    if (!string.IsNullOrEmpty(status.deviceName) && !string.IsNullOrEmpty(status.deviceType))
                    {
                        retval.Device = RasDevice.Create(status.deviceName, status.deviceType);
                    }

#if (WIN7)
                    retval.LocalEndPoint = Utilities.GetIPAddressFromEndPoint(status.localEndPoint);
                    retval.RemoteEndPoint = Utilities.GetIPAddressFromEndPoint(status.remoteEndPoint);
                    retval.ConnectionSubState = status.connectionSubState;
#endif
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            finally
            {
                if (lpRasConnStatus != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpRasConnStatus);
                }
            }

            return retval;
        }

        /// <summary>
        /// Retrieves user credentials associated with a specified remote access phone book entry.
        /// </summary>
        /// <param name="phoneBookPath">Required. The full path (including filename) of the phone book containing the entry.</param>
        /// <param name="entryName">Required. The name of the entry whose credentials to retrieve.</param>
        /// <param name="options">The options to request.</param>
        /// <returns>The credentials stored in the entry, otherwise a null reference (<b>Nothing</b> in Visual Basic) if the credentials did not exist.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="phoneBookPath"/> or <paramref name="entryName"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public NetworkCredential GetCredentials(string phoneBookPath, string entryName, NativeMethods.RASCM options)
        {
            if (string.IsNullOrEmpty(phoneBookPath))
            {
                ThrowHelper.ThrowArgumentException("phoneBookPath", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            if (string.IsNullOrEmpty(entryName))
            {
                ThrowHelper.ThrowArgumentException("entryName", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            NetworkCredential retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RASCREDENTIALS));

            NativeMethods.RASCREDENTIALS credentials = new NativeMethods.RASCREDENTIALS();
            credentials.size = size;
            credentials.options = options;

            IntPtr pCredentials = IntPtr.Zero;
            try
            {
                pCredentials = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(credentials, pCredentials, true);

                try
                {
                    int ret = UnsafeNativeMethods.Instance.GetCredentials(phoneBookPath, entryName, pCredentials);
                    if (ret == NativeMethods.SUCCESS)
                    {
                        credentials = (NativeMethods.RASCREDENTIALS)Marshal.PtrToStructure(pCredentials, typeof(NativeMethods.RASCREDENTIALS));
                        if (credentials.options != NativeMethods.RASCM.None)
                        {
                            retval = new NetworkCredential(
                                credentials.userName,
                                credentials.password,
                                credentials.domain);
                        }
                    }
                    else if (ret != NativeMethods.ERROR_FILE_NOT_FOUND)
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
            }
            finally
            {
                if (pCredentials != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pCredentials);
                }
            }

            return retval;
        }

        /// <summary>
        /// Lists all available remote access capable devices.
        /// </summary>
        /// <returns>A new collection of <see cref="MessagingToolkit.Core.Ras.RasDevice"/> objects.</returns>
        public ReadOnlyCollection<RasDevice> GetDevices()
        {
            ReadOnlyCollection<RasDevice> retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RASDEVINFO));

            StructBufferedPInvokeParams info = new StructBufferedPInvokeParams();
            info.BufferSize = new IntPtr(size);
            info.Count = IntPtr.Zero;
          
            bool retry = false;

            do
            {
                NativeMethods.RASDEVINFO device = new NativeMethods.RASDEVINFO();
                device.size = size;

                try
                {
                    info.Address = Marshal.AllocHGlobal(info.BufferSize);
                    Marshal.StructureToPtr(device, info.Address, true);

                    int ret = SafeNativeMethods.Instance.EnumDevices(info);
                    if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                    {
                        retry = true;
                    }
                    else if (ret == NativeMethods.SUCCESS)
                    {
                        retry = false;

                        NativeMethods.RASDEVINFO[] devices = Utilities.CreateArrayOfType<NativeMethods.RASDEVINFO>(info.Address, size, info.Count.ToInt32());
                        RasDevice[] tempArray = null;

                        if (devices == null || devices.Length == 0)
                        {
                            tempArray = new RasDevice[0];
                        }
                        else
                        {
                            tempArray = new RasDevice[devices.Length];

                            for (int index = 0; index < devices.Length; index++)
                            {
                                NativeMethods.RASDEVINFO current = devices[index];

                                tempArray[index] = RasDevice.Create(
                                    current.name,
                                    current.type);
                            }
                        }

                        retval = new ReadOnlyCollection<RasDevice>(tempArray);
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
                finally
                {
                    if (info.Address != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(info.Address);
                    }
                }
            } 
            while (retry);

            return retval;
        }

        /// <summary>
        /// Retrieves the entry properties for an entry within a phone book.
        /// </summary>
        /// <param name="phoneBook">Required. The <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> containing the entry.</param>
        /// <param name="entryName">Required. The name of an entry to retrieve.</param>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasEntry"/> object.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="entryName"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBook"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public RasEntry GetEntryProperties(RasPhoneBook phoneBook, string entryName)
        {
            if (phoneBook == null)
            {
                ThrowHelper.ThrowArgumentNullException("phoneBook");
            }

            if (string.IsNullOrEmpty(entryName))
            {
                ThrowHelper.ThrowArgumentException("entryName", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            RasEntry retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RASENTRY));
            bool retry = false;

            IntPtr lpCb = new IntPtr(size);
            do
            {
                NativeMethods.RASENTRY entry = new NativeMethods.RASENTRY();
                entry.size = size;

                IntPtr lpRasEntry = IntPtr.Zero;
                try
                {
                    lpRasEntry = Marshal.AllocHGlobal(lpCb);
                    Marshal.StructureToPtr(entry, lpRasEntry, true);

                    try
                    {
                        int ret = UnsafeNativeMethods.Instance.GetEntryProperties(phoneBook.Path, entryName, lpRasEntry, ref lpCb, IntPtr.Zero, IntPtr.Zero);
                        if (ret == NativeMethods.SUCCESS)
                        {
                            entry = (NativeMethods.RASENTRY)Marshal.PtrToStructure(lpRasEntry, typeof(NativeMethods.RASENTRY));

                            retval = new RasEntry(entryName);

                            if (entry.alternateOffset != 0)
                            {                                
                                retval.AlternatePhoneNumbers = Utilities.CreateStringCollectionByLength(lpRasEntry, entry.alternateOffset, lpCb.ToInt32() - entry.alternateOffset);
                            }

                            if (entry.subentries > 1)
                            {
                                // The first subentry in the collection is always the default entry, need to check if there are two or
                                // more subentries before loading the collection.
                                retval.SubEntries.Load(phoneBook, entry.subentries - 1);
                            }

                            retval.AreaCode = entry.areaCode;

// This warning is being disabled since the object is being loaded by the Win32 API and must have the
// data placed into the object.
#pragma warning disable 0618
                            retval.AutoDialDll = entry.autoDialDll;
                            retval.AutoDialFunc = entry.autoDialFunc;
#pragma warning restore 0618

                            retval.Channels = entry.channels;
                            retval.CountryCode = entry.countryCode;
                            retval.CountryId = entry.countryId;
                            retval.CustomAuthKey = entry.customAuthKey;
                            retval.CustomDialDll = entry.customDialDll;

                            if (entry.deviceName != null && !string.IsNullOrEmpty(entry.deviceType))
                            {
                                retval.Device = RasDevice.Create(entry.deviceName, entry.deviceType);
                            }

                            retval.DialExtraPercent = entry.dialExtraPercent;
                            retval.DialExtraSampleSeconds = entry.dialExtraSampleSeconds;
                            retval.DialMode = entry.dialMode;
                            retval.DnsAddress = new IPAddress(entry.dnsAddress.addr);
                            retval.DnsAddressAlt = new IPAddress(entry.dnsAddressAlt.addr);
                            retval.EncryptionType = entry.encryptionType;
                            retval.EntryType = entry.entryType;
                            retval.FrameSize = entry.frameSize;
                            retval.FramingProtocol = entry.framingProtocol;
                            retval.HangUpExtraPercent = entry.hangUpExtraPercent;
                            retval.HangUpExtraSampleSeconds = entry.hangUpExtraSampleSeconds;
                            retval.Id = entry.id;
                            retval.IdleDisconnectSeconds = entry.idleDisconnectSeconds;
                            retval.IPAddress = new IPAddress(entry.ipAddress.addr);                            
                            retval.PhoneNumber = entry.phoneNumber;
                            retval.Script = entry.script;
                            retval.VpnStrategy = entry.vpnStrategy;
                            retval.WinsAddress = new IPAddress(entry.winsAddress.addr);
                            retval.WinsAddressAlt = new IPAddress(entry.winsAddressAlt.addr);
                            retval.X25Address = entry.x25Address;
                            retval.X25Facilities = entry.x25Facilities;
                            retval.X25PadType = entry.x25PadType;
                            retval.X25UserData = entry.x25UserData;

                            Utilities.SetRasEntryOptions(retval, entry.options);
                            Utilities.SetRasNetworkProtocols(retval, entry.networkProtocols);

#if (WINXP || WIN2K8 || WIN7)

                            Utilities.SetRasEntryExtendedOptions(retval, entry.options2);

                            retval.DnsSuffix = entry.dnsSuffix;
                            retval.TcpWindowSize = entry.tcpWindowSize;
                            retval.PrerequisitePhoneBook = entry.prerequisitePhoneBook;
                            retval.PrerequisiteEntryName = entry.prerequisiteEntryName;
                            retval.RedialCount = entry.redialCount;
                            retval.RedialPause = entry.redialPause;
#endif
#if (WIN2K8 || WIN7)
                            retval.IPv6DnsAddress = new IPAddress(entry.ipv6DnsAddress.addr);
                            retval.IPv6DnsAddressAlt = new IPAddress(entry.ipv6DnsAddressAlt.addr);
                            retval.IPv4InterfaceMetric = entry.ipv4InterfaceMetric;
                            retval.IPv6InterfaceMetric = entry.ipv6InterfaceMetric;
#endif
#if (WIN7)
                            retval.IPv6Address = new IPAddress(entry.ipv6Address.addr);
                            retval.IPv6PrefixLength = entry.ipv6PrefixLength;
                            retval.NetworkOutageTime = entry.networkOutageTime;
#endif

                            retry = false;
                        }
                        else if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                        {
                            retry = true;
                        }
                        else
                        {
                            ThrowHelper.ThrowRasException(ret);
                        }
                    }
                    catch (EntryPointNotFoundException)
                    {
                        ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                    }
                }
                finally
                {
                    if (lpRasEntry != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(lpRasEntry);
                    }
                }
            }
            while (retry);

            return retval;
        }

        /// <summary>
        /// Retrieves a connection handle for a subentry of a multilink connection.
        /// </summary>
        /// <param name="handle">The handle of the connection.</param>
        /// <param name="subEntryId">The one-based index of the subentry to whose handle to retrieve.</param>
        /// <returns>The handle of the subentry if available, otherwise a null reference (<b>Nothing</b> in Visual Basic).</returns>
        /// <exception cref="System.ArgumentException"><paramref name="subEntryId"/> cannot be less than or equal to zero.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="handle"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        public RasHandle GetSubEntryHandle(RasHandle handle, int subEntryId)
        {
            if (Utilities.IsHandleInvalidOrClosed(handle))
            {
                ThrowHelper.ThrowArgumentNullException("handle");
            }

            if (subEntryId <= 0)
            {
                ThrowHelper.ThrowArgumentException("subEntryId", Resources.Argument_ValueCannotBeLessThanOrEqualToZero);
            }

            RasHandle retval = null;

            try
            {
                IntPtr tempHandle = IntPtr.Zero;
                int ret = SafeNativeMethods.Instance.GetSubEntryHandle(handle, subEntryId, out tempHandle);
                if (ret == NativeMethods.SUCCESS)
                {
                    retval = new RasHandle(tempHandle, subEntryId > 1);
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }

            return retval;
        }

        /// <summary>
        /// Retrieves the subentry properties for an entry within a phone book.
        /// </summary>
        /// <param name="phoneBook">Required. The <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> containing the entry.</param>
        /// <param name="entry">Required. The <see cref="MessagingToolkit.Core.Ras.RasEntry"/> containing the subentry.</param>
        /// <param name="subEntryId">The zero-based index of the subentry to retrieve.</param>
        /// <returns>A new <see cref="MessagingToolkit.Core.Ras.RasSubEntry"/> object.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBook"/> or <paramref name="entry"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public RasSubEntry GetSubEntryProperties(RasPhoneBook phoneBook, RasEntry entry, int subEntryId)
        {
            if (phoneBook == null)
            {
                ThrowHelper.ThrowArgumentNullException("phoneBook");
            }

            if (entry == null)
            {
                ThrowHelper.ThrowArgumentNullException("entry");
            }

            RasSubEntry retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RASSUBENTRY));
            bool retry = false;

            IntPtr lpCb = new IntPtr(size);
            do
            {
                NativeMethods.RASSUBENTRY subentry = new NativeMethods.RASSUBENTRY();
                subentry.size = size;

                IntPtr lpRasSubEntry = IntPtr.Zero;
                try
                {
                    lpRasSubEntry = Marshal.AllocHGlobal(lpCb);
                    Marshal.StructureToPtr(subentry, lpRasSubEntry, true);

                    try
                    {
                        int ret = UnsafeNativeMethods.Instance.GetSubEntryProperties(phoneBook.Path, entry.Name, subEntryId + 2, lpRasSubEntry, ref lpCb, IntPtr.Zero, IntPtr.Zero);
                        if (ret == NativeMethods.SUCCESS)
                        {
                            subentry = (NativeMethods.RASSUBENTRY)Marshal.PtrToStructure(lpRasSubEntry, typeof(NativeMethods.RASSUBENTRY));

                            retval = new RasSubEntry();

                            retval.Device = RasDevice.Create(subentry.deviceName, subentry.deviceType);
                            retval.PhoneNumber = subentry.phoneNumber;

                            if (subentry.alternateOffset != 0)
                            {
                                retval.AlternatePhoneNumbers = Utilities.CreateStringCollectionByLength(lpRasSubEntry, subentry.alternateOffset, lpCb.ToInt32() - subentry.alternateOffset);
                            }

                            retry = false;
                        }
                        else if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                        {
                            retry = true;
                        }
                        else
                        {
                            ThrowHelper.ThrowRasException(ret);
                        }
                    }
                    catch (EntryPointNotFoundException)
                    {
                        ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                    }
                }
                finally
                {
                    if (lpRasSubEntry != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(lpRasSubEntry);
                    }
                }
            }
            while (retry);

            return retval;
        }

        /// <summary>
        /// Retrieves user-specific Extensible Authentication Protocol (EAP) information for the specified phone book entry.
        /// </summary>
        /// <param name="userToken">The handle of a Windows account token. This token is usually retrieved through a call to unmanaged code, such as a call to the Win32 API LogonUser function.</param>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The entry name to validate.</param>
        /// <returns>A byte array containing the EAP data, otherwise a null reference (<b>Nothing</b> in Visual Basic).</returns>
        public byte[] GetEapUserData(IntPtr userToken, string phoneBookPath, string entryName)
        {
            byte[] result = null;

            RasGetEapUserDataParams info = new RasGetEapUserDataParams();
            info.UserToken = userToken;
            info.PhoneBookPath = phoneBookPath;
            info.EntryName = entryName;
            info.BufferSize = IntPtr.Zero;

            bool retry = false;

            do
            {
                try
                {
                    info.Address = Marshal.AllocHGlobal(info.BufferSize);

                    int ret = SafeNativeMethods.Instance.GetEapUserData(info);
                    if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                    {
                        retry = true;
                    }
                    else if (ret == NativeMethods.SUCCESS)
                    {
                        int bufferSize = info.BufferSize.ToInt32();

                        if (bufferSize != 0)
                        {
                            result = new byte[bufferSize];
                            Marshal.Copy(info.Address, result, 0, bufferSize);
                        }

                        retry = false;
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
                finally
                {
                    if (info.Address != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(info.Address);
                    }
                }
            }
            while (retry);

            return result;
        }

        /// <summary>
        /// Retrieves a list of entry names within a phone book.
        /// </summary>
        /// <param name="phoneBook">Required. The <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> whose entry names to retrieve.</param>
        /// <returns>An array of <see cref="NativeMethods.RASENTRYNAME"/> structures, or a null reference if the phone-book was not found.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBook"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public NativeMethods.RASENTRYNAME[] GetEntryNames(RasPhoneBook phoneBook)
        {
            if (phoneBook == null)
            {
                ThrowHelper.ThrowArgumentNullException("phoneBook");
            }

            NativeMethods.RASENTRYNAME[] retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RASENTRYNAME));
            IntPtr lpCb = new IntPtr(size);
            IntPtr lpcEntries = IntPtr.Zero;

            bool retry = false;

            do
            {
                NativeMethods.RASENTRYNAME entry = new NativeMethods.RASENTRYNAME();
                entry.size = size;

                IntPtr pEntries = IntPtr.Zero;
                try
                {
                    pEntries = Marshal.AllocHGlobal(lpCb);
                    Marshal.StructureToPtr(entry, pEntries, true);

                    try
                    {
                        int ret = UnsafeNativeMethods.Instance.EnumEntries(IntPtr.Zero, phoneBook.Path, pEntries, ref lpCb, ref lpcEntries);
                        if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                        {
                            retry = true;
                        }
                        else if (ret == NativeMethods.SUCCESS)
                        {
                            retry = false;

                            int entries = lpcEntries.ToInt32();
                            if (entries > 0)
                            {
                                retval = Utilities.CreateArrayOfType<NativeMethods.RASENTRYNAME>(pEntries, size, entries);
                            }
                        }
                        else
                        {
                            ThrowHelper.ThrowRasException(ret);
                        }
                    }
                    catch (EntryPointNotFoundException)
                    {
                        ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                    }
                }
                finally
                {
                    if (pEntries != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pEntries);
                    }
                }
            } 
            while (retry);

            return retval;
        }

        /// <summary>
        /// Retrieves accumulated statistics for the specified connection.
        /// </summary>
        /// <param name="handle">The handle to the connection.</param>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasLinkStatistics"/> structure containing connection statistics.</returns>
        public RasLinkStatistics GetConnectionStatistics(RasHandle handle)
        {
            if (Utilities.IsHandleInvalidOrClosed(handle))
            {
                ThrowHelper.ThrowArgumentException("handle", Resources.Argument_InvalidHandle);
            }

            RasLinkStatistics retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RAS_STATS));

            NativeMethods.RAS_STATS stats = new NativeMethods.RAS_STATS();
            stats.size = size;

            IntPtr lpStatistics = IntPtr.Zero;
            try
            {
                lpStatistics = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(stats, lpStatistics, true);

                int ret = SafeNativeMethods.Instance.GetConnectionStatistics(handle, lpStatistics);
                if (ret == NativeMethods.SUCCESS)
                {
                    stats = (NativeMethods.RAS_STATS)Marshal.PtrToStructure(lpStatistics, typeof(NativeMethods.RAS_STATS));

                    retval = new RasLinkStatistics(
                        stats.bytesTransmitted,
                        stats.bytesReceived,
                        stats.framesTransmitted,
                        stats.framesReceived,
                        stats.crcError,
                        stats.timeoutError,
                        stats.alignmentError,
                        stats.hardwareOverrunError,
                        stats.framingError,
                        stats.bufferOverrunError,
                        stats.compressionRatioIn,
                        stats.compressionRatioOut,
                        stats.linkSpeed,
                        TimeSpan.FromMilliseconds(stats.connectionDuration));
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            finally
            {
                if (lpStatistics != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpStatistics);
                }
            }

            return retval;
        }

        /// <summary>
        /// Retrieves country/region specific dialing information from the Windows Telephony list of countries/regions for a specific country id.
        /// </summary>
        /// <param name="countryId">The country id to retrieve.</param>
        /// <param name="nextCountryId">Upon output, contains the next country id from the list; otherwise zero for the last country/region in the list.</param>
        /// <returns>A new <see cref="MessagingToolkit.Core.Ras.RasCountry"/> object.</returns>
        public RasCountry GetCountry(int countryId, out int nextCountryId)
        {
            RasCountry retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RASCTRYINFO));
            IntPtr lpdwSize = new IntPtr(size);
            nextCountryId = 0;

            bool retry = false;

            do
            {
                NativeMethods.RASCTRYINFO country = new NativeMethods.RASCTRYINFO();
                country.size = size;
                country.countryId = countryId;

                IntPtr lpRasCtryInfo = IntPtr.Zero;
                try
                {
                    lpRasCtryInfo = Marshal.AllocHGlobal(lpdwSize);
                    Marshal.StructureToPtr(country, lpRasCtryInfo, true);

                    int ret = SafeNativeMethods.Instance.GetCountryInfo(lpRasCtryInfo, ref lpdwSize);
                    if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                    {
                        retry = true;
                    }
                    else if (ret == NativeMethods.SUCCESS)
                    {
                        retry = false;
                        country = (NativeMethods.RASCTRYINFO)Marshal.PtrToStructure(lpRasCtryInfo, typeof(NativeMethods.RASCTRYINFO));

                        nextCountryId = country.nextCountryId;

                        string name = string.Empty;
                        if (country.countryNameOffset > 0)
                        {
                            name = Marshal.PtrToStringUni(new IntPtr(lpRasCtryInfo.ToInt64() + country.countryNameOffset));
                        }

                        retval = new RasCountry(
                            country.countryId,
                            country.countryCode,
                            name);
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
                finally
                {
                    if (lpRasCtryInfo != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(lpRasCtryInfo);
                    }
                }
            }
            while (retry);

            return retval;
        }

        /// <summary>
        /// Retrieves accumulated statistics for the specified link in a RAS multilink connection.
        /// </summary>
        /// <param name="handle">The handle to the connection.</param>
        /// <param name="subEntryId">The one-based index that corresponds to the link for which to retrieve statistics.</param>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasLinkStatistics"/> structure containing connection statistics.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="subEntryId"/> must be greater than zero.</exception>
        /// <exception cref="MessagingToolkit.Core.Ras.InvalidHandleException"><paramref name="handle"/> is a null reference (<b>Nothing</b> in Visual Basic) or an invalid handle.</exception>
        public RasLinkStatistics GetLinkStatistics(RasHandle handle, int subEntryId)
        {
            if (Utilities.IsHandleInvalidOrClosed(handle))
            {
                ThrowHelper.ThrowInvalidHandleException(handle, "handle", Resources.Argument_InvalidHandle);
            }

            if (subEntryId <= 0)
            {
                ThrowHelper.ThrowArgumentException("subEntryId", Resources.Argument_ValueCannotBeLessThanOrEqualToZero);
            }

            RasLinkStatistics retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RAS_STATS));

            NativeMethods.RAS_STATS stats = new NativeMethods.RAS_STATS();
            stats.size = size;

            IntPtr lpRasLinkStatistics = IntPtr.Zero;
            try
            {
                lpRasLinkStatistics = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(stats, lpRasLinkStatistics, true);

                int ret = SafeNativeMethods.Instance.GetLinkStatistics(handle, subEntryId, lpRasLinkStatistics);
                if (ret == NativeMethods.SUCCESS)
                {
                    stats = (NativeMethods.RAS_STATS)Marshal.PtrToStructure(lpRasLinkStatistics, typeof(NativeMethods.RAS_STATS));

                    retval = new RasLinkStatistics(
                        stats.bytesTransmitted,
                        stats.bytesReceived,
                        stats.framesTransmitted,
                        stats.framesReceived,
                        stats.crcError,
                        stats.timeoutError,
                        stats.alignmentError,
                        stats.hardwareOverrunError,
                        stats.framingError,
                        stats.bufferOverrunError,
                        stats.compressionRatioIn,
                        stats.compressionRatioOut,
                        stats.linkSpeed,
                        TimeSpan.FromMilliseconds(stats.connectionDuration));
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            finally
            {
                if (lpRasLinkStatistics != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpRasLinkStatistics);
                }
            }

            return retval;
        }

        /// <summary>
        /// Indicates whether a connection is currently active.
        /// </summary>
        /// <param name="handle">The handle to check.</param>
        /// <returns><b>true</b> if the connection is active, otherwise <b>false</b>.</returns>
        public bool IsConnectionActive(RasHandle handle)
        {
            bool retval = false;

            int size = Marshal.SizeOf(typeof(NativeMethods.RASCONNSTATUS));

            IntPtr lpRasConnStatus = IntPtr.Zero;
            try
            {
                NativeMethods.RASCONNSTATUS status = new NativeMethods.RASCONNSTATUS();
                status.size = size;

                lpRasConnStatus = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(status, lpRasConnStatus, true);

                retval = SafeNativeMethods.Instance.GetConnectStatus(handle, lpRasConnStatus) == NativeMethods.SUCCESS;
            }
            finally
            {
                if (lpRasConnStatus != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpRasConnStatus);
                }
            }

            return retval;
        }

        /// <summary>
        /// Terminates a remote access connection.
        /// </summary>
        /// <param name="handle">The remote access connection handle to terminate.</param>
        /// <param name="pollingInterval">The length of time, in milliseconds, the thread must be paused while polling whether the connection has terminated.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="handle"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="pollingInterval"/> must be greater than or equal to zero.</exception>
        /// <exception cref="MessagingToolkit.Core.Ras.InvalidHandleException"><paramref name="handle"/> is not a valid handle.</exception>
        public void HangUp(RasHandle handle, int pollingInterval)
        {
            if (handle == null)
            {
                ThrowHelper.ThrowArgumentNullException("handle");
            }

            if (pollingInterval < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException("pollingInterval", pollingInterval, Resources.Argument_ValueCannotBeLessThanZero);
            }

            try
            {
                int ret = SafeNativeMethods.Instance.HangUp(handle);
                if (ret == NativeMethods.ERROR_INVALID_HANDLE)
                {
                    ThrowHelper.ThrowInvalidHandleException(handle, "handle", Resources.Argument_InvalidHandle);
                }
                else if (ret == NativeMethods.SUCCESS)
                {
                    while (RasHelper.Instance.IsConnectionActive(handle))
                    {
                        if (pollingInterval > 0)
                        {
                            // The caller has specified a polling interval, pause the calling thread while the connection is being terminated.
                            Thread.Sleep(pollingInterval);
                        }
                    }

                    // ATTENTION! This required pause comes from the Windows SDK. Failure to perform this pause may cause the state machine to leave 
                    // the port open which will require the machine to be rebooted to release the port.
                    Thread.Sleep(1000);

                    // Mark the handle as invalid to prevent it from being used elsewhere in the assembly.
                    handle.SetHandleAsInvalid();
                }
                else if (ret != NativeMethods.ERROR_NO_CONNECTION)
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
        }

        /// <summary>
        /// Frees the memory buffer of an EAP user identity.
        /// </summary>
        /// <param name="rasEapUserIdentity">The <see cref="NativeMethods.RASEAPUSERIDENTITY"/> structure to free.</param>
        public void FreeEapUserIdentity(NativeMethods.RASEAPUSERIDENTITY rasEapUserIdentity)
        {
            try
            {
                int size = Marshal.SizeOf(typeof(NativeMethods.RASEAPUSERIDENTITY));

                IntPtr lpRasEapUserIdentity = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(rasEapUserIdentity, lpRasEapUserIdentity, true);

                SafeNativeMethods.Instance.FreeEapUserIdentity(lpRasEapUserIdentity);
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
        }

        /// <summary>
        /// Retrieves any Extensible Authentication Protocol (EAP) user identity information if available.
        /// </summary>
        /// <param name="phoneBookPath">The full path (including filename) of a phone book. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">Required. The name of the entry in the phone book being connected.</param>
        /// <param name="eapOptions">Specifies options to use during authentication.</param>
        /// <param name="owner">The parent window for the UI dialog (if needed).</param>
        /// <param name="identity">Upon return, contains the Extensible Authentication Protocol (EAP) user identity information.</param>
        /// <returns><b>true</b> if the user identity information was returned, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="entryName"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        public bool TryGetEapUserIdentity(string phoneBookPath, string entryName, NativeMethods.RASEAPF eapOptions, IWin32Window owner, out NativeMethods.RASEAPUSERIDENTITY identity)
        {
            if (string.IsNullOrEmpty(entryName))
            {
                ThrowHelper.ThrowArgumentException("entryName", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            bool retval = false;
            identity = new NativeMethods.RASEAPUSERIDENTITY();

            IntPtr lpRasEapUserIdentity = IntPtr.Zero;
            try
            {
                int ret = SafeNativeMethods.Instance.GetEapUserIdentity(phoneBookPath, entryName, eapOptions, owner != null ? owner.Handle : IntPtr.Zero, ref lpRasEapUserIdentity);
                if (ret == NativeMethods.ERROR_INTERACTIVE_MODE)
                {
                    ThrowHelper.ThrowArgumentException("options", Resources.Argument_EapOptionsRequireInteractiveMode);
                }
                else if (ret == NativeMethods.ERROR_INVALID_FUNCTION_FOR_ENTRY)
                {
                    // The protocol being used by the entry does not support EAP, therefore no EAP information is needed.
                    retval = false;
                }
                else if (ret == NativeMethods.SUCCESS)
                {
                    retval = true;

                    // Valid EAP information was returned, marshal the pointer back into the structure.
                    identity = (NativeMethods.RASEAPUSERIDENTITY)Marshal.PtrToStructure(lpRasEapUserIdentity, typeof(NativeMethods.RASEAPUSERIDENTITY));
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            finally
            {
                if (lpRasEapUserIdentity != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpRasEapUserIdentity);
                }
            }

            return retval;
        }

#if (WIN2K8 || WIN7)

        /// <summary>
        /// Retrieves the network access protection (NAP) status for a remote access connection.
        /// </summary>
        /// <param name="handle">The handle of the connection.</param>
        /// <returns>A <see cref="MessagingToolkit.Core.Ras.RasNapStatus"/> object containing the NAP status.</returns>
        public RasNapStatus GetNapStatus(RasHandle handle)
        {
            if (Utilities.IsHandleInvalidOrClosed(handle))
            {
                ThrowHelper.ThrowArgumentException("handle", Resources.Argument_InvalidHandle);
            }

            RasNapStatus retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RASNAPSTATE));

            NativeMethods.RASNAPSTATE napState = new NativeMethods.RASNAPSTATE();
            napState.size = size;

            IntPtr pNapState = IntPtr.Zero;
            try
            {
                pNapState = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(napState, pNapState, true);

                int ret = SafeNativeMethods.Instance.GetNapStatus(handle, pNapState);
                if (ret == NativeMethods.SUCCESS)
                {
                    napState = (NativeMethods.RASNAPSTATE)Marshal.PtrToStructure(pNapState, typeof(NativeMethods.RASNAPSTATE));

                    long fileTime = napState.probationTime.dwHighDateTime << 0x20 | napState.probationTime.dwLowDateTime;
                    
                    retval = new RasNapStatus(
                        napState.isolationState,
                        DateTime.FromFileTime(fileTime));
                }
                else if (ret == NativeMethods.ERROR_INVALID_HANDLE)
                {
                    ThrowHelper.ThrowInvalidHandleException(handle, "handle", Resources.Argument_InvalidHandle);
                }
                else if (ret == NativeMethods.ERROR_NOT_NAP_CAPABLE)
                {
                    ThrowHelper.ThrowInvalidOperationException(Resources.Exception_HandleNotNapCapable);
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            finally
            {
                if (pNapState != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pNapState);
                }
            }

            return retval;
        }

#endif

        /// <summary>
        /// Retrieves information about a remote access projection operation for a connection.
        /// </summary>
        /// <param name="handle">The handle of the connection.</param>
        /// <param name="projection">The protocol of interest.</param>
        /// <returns>The resulting projection information, otherwise null reference (<b>Nothing</b> in Visual Basic) if the protocol was not found.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="handle"/> is not a valid handle.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "The method uses a large switch statement to generate the appropriate return type.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The method uses a large switch statement to generate the appropriate return type.")]
        public object GetProjectionInfo(RasHandle handle, NativeMethods.RASPROJECTION projection)
        {
            if (Utilities.IsHandleInvalidOrClosed(handle))
            {
                ThrowHelper.ThrowArgumentException("handle", Resources.Argument_InvalidHandle);
            }

            int size = 0;

            object retval = null;
            object structure = null;

            switch (projection)
            {
                case NativeMethods.RASPROJECTION.Amb:
                    size = Marshal.SizeOf(typeof(NativeMethods.RASAMB));

                    NativeMethods.RASAMB amb = new NativeMethods.RASAMB();
                    amb.size = size;

                    structure = amb;
                    break;

                case NativeMethods.RASPROJECTION.Ccp:
                    size = Marshal.SizeOf(typeof(NativeMethods.RASPPPCCP));

                    NativeMethods.RASPPPCCP ccp = new NativeMethods.RASPPPCCP();
                    ccp.size = size;

                    structure = ccp;
                    break;

                case NativeMethods.RASPROJECTION.IP:
                    size = Marshal.SizeOf(typeof(NativeMethods.RASPPPIP));

                    NativeMethods.RASPPPIP ip = new NativeMethods.RASPPPIP();
                    ip.size = size;

                    structure = ip;
                    break;

                case NativeMethods.RASPROJECTION.Ipx:
                    size = Marshal.SizeOf(typeof(NativeMethods.RASPPPIPX));

                    NativeMethods.RASPPPIPX ipx = new NativeMethods.RASPPPIPX();
                    ipx.size = size;

                    structure = ipx;
                    break;

                case NativeMethods.RASPROJECTION.Lcp:
                    size = Marshal.SizeOf(typeof(NativeMethods.RASPPPLCP));

                    NativeMethods.RASPPPLCP lcp = new NativeMethods.RASPPPLCP();
                    lcp.size = size;

                    structure = lcp;
                    break;

                case NativeMethods.RASPROJECTION.Nbf:
                    size = Marshal.SizeOf(typeof(NativeMethods.RASPPPNBF));

                    NativeMethods.RASPPPNBF nbf = new NativeMethods.RASPPPNBF();
                    nbf.size = size;

                    structure = nbf;
                    break;

#if (WIN2K8 || WIN7)

                case NativeMethods.RASPROJECTION.IPv6:
                    size = Marshal.SizeOf(typeof(NativeMethods.RASPPPIPV6));

                    NativeMethods.RASPPPIPV6 ipv6 = new NativeMethods.RASPPPIPV6();
                    ipv6.size = size;

                    structure = ipv6;
                    break;

#endif
                case NativeMethods.RASPROJECTION.Slip:
                    size = Marshal.SizeOf(typeof(NativeMethods.RASSLIP));

                    NativeMethods.RASSLIP slip = new NativeMethods.RASSLIP();
                    slip.size = size;

                    structure = slip;
                    break;
            }

            IntPtr lpCb = new IntPtr(size);

            IntPtr lpProjection = IntPtr.Zero;
            try
            {
                lpProjection = Marshal.AllocHGlobal(lpCb);
                Marshal.StructureToPtr(structure, lpProjection, true);

                int ret = SafeNativeMethods.Instance.GetProjectionInfo(handle, projection, lpProjection, ref lpCb);
                if (ret == NativeMethods.SUCCESS)
                {
                    switch (projection)
                    {
                        case NativeMethods.RASPROJECTION.Amb:
                            NativeMethods.RASAMB amb = (NativeMethods.RASAMB)Marshal.PtrToStructure(lpProjection, typeof(NativeMethods.RASAMB));

                            retval = new RasAmbInfo(
                                amb.errorCode,
                                amb.netBiosError,
                                amb.lana);

                            break;

                        case NativeMethods.RASPROJECTION.Ccp:
                            NativeMethods.RASPPPCCP ccp = (NativeMethods.RASPPPCCP)Marshal.PtrToStructure(lpProjection, typeof(NativeMethods.RASPPPCCP));

                            retval = new RasCcpInfo(
                                ccp.errorCode,
                                ccp.compressionAlgorithm,
                                new RasCompressionOptions(ccp.options),
                                ccp.serverCompressionAlgorithm,
                                new RasCompressionOptions(ccp.serverOptions));

                            break;

                        case NativeMethods.RASPROJECTION.IP:
                            NativeMethods.RASPPPIP ip = (NativeMethods.RASPPPIP)Marshal.PtrToStructure(lpProjection, typeof(NativeMethods.RASPPPIP));

                            retval = new RasIPInfo(
                                ip.errorCode,
                                IPAddress.Parse(ip.ipAddress),
                                IPAddress.Parse(ip.serverIPAddress),
                                new RasIPOptions(ip.options),
                                new RasIPOptions(ip.serverOptions));

                            break;

                        case NativeMethods.RASPROJECTION.Ipx:
                            NativeMethods.RASPPPIPX ipx = (NativeMethods.RASPPPIPX)Marshal.PtrToStructure(lpProjection, typeof(NativeMethods.RASPPPIPX));

                            retval = new RasIpxInfo(
                                ipx.errorCode,
                                IPAddress.Parse(ipx.ipxAddress));

                            break;

                        case NativeMethods.RASPROJECTION.Lcp:
                            NativeMethods.RASPPPLCP lcp = (NativeMethods.RASPPPLCP)Marshal.PtrToStructure(lpProjection, typeof(NativeMethods.RASPPPLCP));

                            retval = new RasLcpInfo(
                                lcp.bundled,
                                lcp.errorCode,
                                lcp.authenticationProtocol,
                                lcp.authenticationData,
                                lcp.eapTypeId,
                                lcp.serverAuthenticationProtocol,
                                lcp.serverAuthenticationData,
                                lcp.serverEapTypeId,
                                lcp.multilink,
                                lcp.terminateReason,
                                lcp.serverTerminateReason,
                                lcp.replyMessage,
                                new RasLcpOptions(lcp.options),
                                new RasLcpOptions(lcp.serverOptions));

                            break;

                        case NativeMethods.RASPROJECTION.Nbf:
                            NativeMethods.RASPPPNBF nbf = (NativeMethods.RASPPPNBF)Marshal.PtrToStructure(lpProjection, typeof(NativeMethods.RASPPPNBF));

                            retval = new RasNbfInfo(
                                nbf.errorCode,
                                nbf.netBiosErrorCode,
                                nbf.netBiosErrorMessage,
                                nbf.workstationName,
                                nbf.lana);

                            break;

                        case NativeMethods.RASPROJECTION.Slip:
                            NativeMethods.RASSLIP slip = (NativeMethods.RASSLIP)Marshal.PtrToStructure(lpProjection, typeof(NativeMethods.RASSLIP));

                            retval = new RasSlipInfo(
                                slip.errorCode,
                                IPAddress.Parse(slip.ipAddress));

                            break;

#if (WIN2K8 || WIN7)
                        case NativeMethods.RASPROJECTION.IPv6:
                            NativeMethods.RASPPPIPV6 ipv6 = (NativeMethods.RASPPPIPV6)Marshal.PtrToStructure(lpProjection, typeof(NativeMethods.RASPPPIPV6));

                            retval = new RasIPv6Info(
                                ipv6.errorCode,
                                Utilities.ConvertBytesToInt64(ipv6.localInterfaceIdentifier, 0),
                                Utilities.ConvertBytesToInt64(ipv6.peerInterfaceIdentifier, 0),
                                Utilities.ConvertBytesToInt16(ipv6.localCompressionProtocol, 0),
                                Utilities.ConvertBytesToInt16(ipv6.peerCompressionProtocol, 0));

                            break;
#endif
                    }
                }
                else if (ret != NativeMethods.ERROR_INVALID_PARAMETER && ret != NativeMethods.ERROR_PROTOCOL_NOT_CONFIGURED)
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            finally
            {
                if (lpProjection != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpProjection);
                }
            }

            return retval;
        }

#if (WIN7)
        /// <summary>
        /// Retrieves extended information about a remote access projection operation for a connection.
        /// </summary>
        /// <param name="handle">The handle of the connection.</param>
        /// <returns>The resulting projection information, otherwise null reference (<b>Nothing</b> in Visual Basic) if the protocol was not found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The method uses a large switch statement to generate the appropriate return type.")]
        public object GetProjectionInfoEx(RasHandle handle)
        {
            object retval = null;

            int size = Marshal.SizeOf(typeof(NativeMethods.RAS_PROJECTION_INFO));

            IntPtr lpdwSize = new IntPtr(size);
            bool retry = false;

            do
            {
                NativeMethods.RAS_PROJECTION_INFO projectionInfo = new NativeMethods.RAS_PROJECTION_INFO();
                projectionInfo.version = GetCurrentApiVersion();

                IntPtr pRasProjection = IntPtr.Zero;
                try
                {
                    pRasProjection = Marshal.AllocHGlobal(lpdwSize);
                    Marshal.StructureToPtr(projectionInfo, pRasProjection, true);

                    int ret = SafeNativeMethods.Instance.GetProjectionInfoEx(handle, pRasProjection, ref lpdwSize);
                    if (ret == NativeMethods.ERROR_BUFFER_TOO_SMALL)
                    {
                        retry = true;
                    }
                    else if (ret == NativeMethods.SUCCESS)
                    {
                        projectionInfo = (NativeMethods.RAS_PROJECTION_INFO)Marshal.PtrToStructure(pRasProjection, typeof(NativeMethods.RAS_PROJECTION_INFO));

                        // Use the object located at the end of the structure since the union will cause portability issues on 64-bit platforms.
                        IntPtr pInfo = new IntPtr(pRasProjection.ToInt64() + size);

                        switch (projectionInfo.type)
                        {
                            case NativeMethods.RASPROJECTION_INFO_TYPE.Ppp:
                                NativeMethods.RASPPP_PROJECTION_INFO ppp = (NativeMethods.RASPPP_PROJECTION_INFO)Marshal.PtrToStructure(pInfo, typeof(NativeMethods.RASPPP_PROJECTION_INFO));

                                ReadOnlyCollection<byte> interfaceIdentifier = null;
                                if (ppp.interfaceIdentifier != null && ppp.interfaceIdentifier.Length > 0)
                                {
                                    interfaceIdentifier = new ReadOnlyCollection<byte>(new List<byte>(ppp.interfaceIdentifier));
                                }

                                ReadOnlyCollection<byte> serverInterfaceIdentifier = null;
                                if (ppp.serverInterfaceIdentifier != null && ppp.serverInterfaceIdentifier.Length > 0)
                                {
                                    serverInterfaceIdentifier = new ReadOnlyCollection<byte>(new List<byte>(ppp.serverInterfaceIdentifier));
                                }

                                retval = new RasPppInfo(
                                    ppp.ipv4NegotiationError,
                                    new IPAddress(ppp.ipv4Address.addr),
                                    new IPAddress(ppp.ipv4ServerAddress.addr),
                                    ppp.ipv4Options,
                                    ppp.ipv4ServerOptions,
                                    ppp.ipv6NegotiationError,
                                    interfaceIdentifier,
                                    serverInterfaceIdentifier,
                                    ppp.bundled,
                                    ppp.multilink,
                                    ppp.authenticationProtocol,
                                    ppp.authenticationData,
                                    ppp.serverAuthenticationProtocol,
                                    ppp.serverAuthenticationData,
                                    ppp.eapTypeId,
                                    ppp.serverEapTypeId,
                                    new RasLcpOptions(ppp.lcpOptions),
                                    new RasLcpOptions(ppp.serverLcpOptions),
                                    ppp.ccpCompressionAlgorithm,
                                    ppp.serverCcpCompressionAlgorithm,
                                    new RasCompressionOptions(ppp.ccpOptions),
                                    new RasCompressionOptions(ppp.serverCcpOptions));

                                break;

                            case NativeMethods.RASPROJECTION_INFO_TYPE.IkeV2:
                                NativeMethods.RASIKEV2_PROJECTION_INFO ikev2 = (NativeMethods.RASIKEV2_PROJECTION_INFO)Marshal.PtrToStructure(pInfo, typeof(NativeMethods.RASIKEV2_PROJECTION_INFO));

                                RasIkeV2Options ikev2Options = new RasIkeV2Options();
                                Utilities.SetRasIkeV2Options(ikev2Options, ikev2.options);

                                retval = new RasIkeV2Info(
                                    ikev2.ipv4NegotiationError,
                                    new IPAddress(ikev2.ipv4Address.addr),
                                    new IPAddress(ikev2.ipv4ServerAddress.addr),
                                    ikev2.ipv6NegotiationError,
                                    new IPAddress(ikev2.ipv6Address.addr),
                                    new IPAddress(ikev2.ipv6ServerAddress.addr),
                                    ikev2.prefixLength,
                                    ikev2.authenticationProtocol,
                                    ikev2.eapTypeId,
                                    ikev2Options,
                                    ikev2.encryptionMethod,
                                    Utilities.CreateIPv4AddressCollection(ikev2.ipv4ServerAddresses, ikev2.numIPv4ServerAddresses),
                                    Utilities.CreateIPv6AddressCollection(ikev2.ipv6ServerAddresses, ikev2.numIPv6ServerAddresses));

                                break;
                        }

                        retry = false;
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
                finally
                {
                    if (pRasProjection != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(pRasProjection);
                    }
                }
            }
            while (retry);

            return retval;
        }
#endif 

        /// <summary>
        /// Retrieves an error message for a specified RAS error code.
        /// </summary>
        /// <param name="errorCode">The error code to retrieve.</param>
        /// <returns>An <see cref="System.String"/> with the error message, otherwise a null reference (<b>Nothing</b> in Visual Basic) if the error code was not found.</returns>
        public string GetRasErrorString(int errorCode)
        {
            string retval = null;

            if (errorCode > 0)
            {
                try
                {
                    string buffer = new string('\x00', 512);

                    int ret = SafeNativeMethods.Instance.GetErrorString(errorCode, buffer, buffer.Length);
                    if (ret == NativeMethods.SUCCESS)
                    {
                        retval = buffer.Substring(0, buffer.IndexOf('\x00'));
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
            }

            return retval;
        }

        /// <summary>
        /// Indicates whether the entry name is valid for the phone book specified.
        /// </summary>
        /// <param name="phoneBook">Required. An <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> to validate the name against.</param>
        /// <param name="entryName">Required. The name of an entry to check.</param>
        /// <returns><b>true</b> if the entry name is valid, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="entryName"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBook"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        public bool IsValidEntryName(RasPhoneBook phoneBook, string entryName)
        {
            return RasHelper.Instance.IsValidEntryName(phoneBook, entryName, null);
        }

        /// <summary>
        /// Indicates whether the entry name is valid for the phone book specified.
        /// </summary>
        /// <param name="phoneBook">Required. An <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> to validate the name against.</param>
        /// <param name="entryName">Required. The name of an entry to check.</param>
        /// <param name="acceptableResults">Any additional results that are considered acceptable results from the call.</param>
        /// <returns><b>true</b> if the entry name is valid, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="entryName"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBook"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        public bool IsValidEntryName(RasPhoneBook phoneBook, string entryName, params int[] acceptableResults)
        {
            if (phoneBook == null)
            {
                ThrowHelper.ThrowArgumentNullException("phoneBook");
            }
            
            if (string.IsNullOrEmpty(entryName))
            {
                ThrowHelper.ThrowArgumentException("entryName", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            bool retval = false;

            try
            {
                int ret = SafeNativeMethods.Instance.ValidateEntryName(phoneBook.Path, entryName);

                if (ret == NativeMethods.SUCCESS)
                {
                    retval = true;
                }
                else if (acceptableResults != null && acceptableResults.Length > 0)
                {
                    for (int index = 0; index < acceptableResults.Length; index++)
                    {
                        if (acceptableResults[index] == ret)
                        {
                            retval = true;
                            break;
                        }
                    }
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }

            return retval;
        }

        /// <summary>
        /// Renames an existing entry in a phone book.
        /// </summary>
        /// <param name="phoneBook">Required. The <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> containing the entry to be renamed.</param>
        /// <param name="entryName">Required. The name of an entry to rename.</param>
        /// <param name="newEntryName">Required. The new name of the entry.</param>
        /// <returns><b>true</b> if the operation was successful, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="entryName"/> or <paramref name="newEntryName"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBook"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public bool RenameEntry(RasPhoneBook phoneBook, string entryName, string newEntryName)
        {
            if (phoneBook == null)
            {
                ThrowHelper.ThrowArgumentNullException("phoneBook");
            }

            if (string.IsNullOrEmpty(entryName))
            {
                ThrowHelper.ThrowArgumentException("entryName", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            if (string.IsNullOrEmpty(newEntryName))
            {
                ThrowHelper.ThrowArgumentException("newEntryName", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            bool retval = false;

            try
            {                
                int ret = UnsafeNativeMethods.Instance.RenameEntry(phoneBook.Path, entryName, newEntryName);
                if (ret == NativeMethods.SUCCESS)
                {
                    retval = true;
                }
                else if (ret == NativeMethods.ERROR_CANNOT_FIND_PHONEBOOK_ENTRY)
                {
                    ThrowHelper.ThrowArgumentException("entryName", Resources.Argument_InvalidEntryName, "entryName", entryName);
                }
                else if (ret == NativeMethods.ERROR_ACCESS_DENIED)
                {
                    ThrowHelper.ThrowUnauthorizedAccessException(Resources.Exception_AccessDeniedBySecurity);
                }
                else if (ret == NativeMethods.ERROR_ALREADY_EXISTS)
                {
                    ThrowHelper.ThrowArgumentException("newEntryName", Resources.Argument_EntryAlreadyExists);
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }

            return retval;
        }

        /// <summary>
        /// Updates an address in the AutoDial mapping database.
        /// </summary>
        /// <param name="address">The address to update.</param>
        /// <param name="entries">A collection of <see cref="MessagingToolkit.Core.Ras.RasAutoDialEntry"/> objects containing the entries for the <paramref name="address"/> specified.</param>
        /// <returns><b>true</b> if the update was successful, otherwise <b>false</b>.</returns>
        public bool SetAutoDialAddress(string address, Collection<RasAutoDialEntry> entries)
        {
            bool retval = false;

            IntPtr pEntries = IntPtr.Zero;
            try
            {
                int count = 0;
                int totalSize = 0;

                if (entries != null && entries.Count > 0)
                {
                    // Reset the existing item so the new object being passed in isn't simply appended to any existing entries.
                    RasHelper.Instance.SetAutoDialAddress(address, null);

                    count = entries.Count;
                    int size = Marshal.SizeOf(typeof(NativeMethods.RASAUTODIALENTRY));

                    // Copy the entries into the struct array that will be used.
                    NativeMethods.RASAUTODIALENTRY[] autoDialEntries = new NativeMethods.RASAUTODIALENTRY[entries.Count];
                    for (int index = 0; index < autoDialEntries.Length; index++)
                    {
                        RasAutoDialEntry current = entries[index];
                        if (current != null)
                        {
                            NativeMethods.RASAUTODIALENTRY item = new NativeMethods.RASAUTODIALENTRY();
                            item.size = size;
                            item.dialingLocation = current.DialingLocation;
                            item.entryName = current.EntryName;

                            autoDialEntries[index] = item;
                        }
                    }

                    pEntries = Utilities.CopyObjectsToNewPtr<NativeMethods.RASAUTODIALENTRY>(autoDialEntries, ref size, out totalSize);
                }

                int ret = UnsafeNativeMethods.Instance.SetAutodialAddress(address, 0, pEntries, totalSize, count);
                if (ret == NativeMethods.SUCCESS)
                {
                    retval = true;
                }
                else if (ret == NativeMethods.ERROR_FILE_NOT_FOUND)
                {
                    retval = false;
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            finally
            {
                if (pEntries != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pEntries);
                }
            }
                      
            return retval;
        }

        /// <summary>
        /// Enables or disables the AutoDial feature for a specific TAPI dialing location.
        /// </summary>
        /// <param name="dialingLocation">The TAPI dialing location to update.</param>
        /// <param name="enabled"><b>true</b> to enable the AutoDial feature, otherwise <b>false</b> to disable it.</param>
        /// <returns><b>true</b> if the operation was successful, otherwise <b>false</b>.</returns>
        public bool SetAutoDialEnable(int dialingLocation, bool enabled)
        {
            bool retval = false;

            try
            {
                int ret = UnsafeNativeMethods.Instance.SetAutodialEnable(dialingLocation, enabled);
                if (ret == NativeMethods.SUCCESS)
                {
                    retval = true;
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }

            return retval;
        }

        /// <summary>
        /// Sets the value of an AutoDial parameter.
        /// </summary>
        /// <param name="parameter">The parameter whose value to set.</param>
        /// <param name="value">The new value of the parameter.</param>
        public void SetAutoDialParameter(NativeMethods.RASADP parameter, int value)
        {
            int size = Marshal.SizeOf(typeof(int));

            IntPtr pValue = IntPtr.Zero;
            try
            {
                pValue = Marshal.AllocHGlobal(size);
                Marshal.WriteInt32(pValue, value);

                int ret = UnsafeNativeMethods.Instance.SetAutodialParam(parameter, pValue, size);
                if (ret != NativeMethods.SUCCESS)
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            finally
            {
                if (pValue != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pValue);
                }
            }
        }

        /// <summary>
        /// Store user-specific Extensible Authentication Protocol (EAP) information for the specified phone book entry in the registry.
        /// </summary>
        /// <param name="handle">The handle to a primary or impersonation access token.</param>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The entry name to validate.</param>
        /// <param name="data">A byte array containing the EAP data.</param>
        /// <returns><b>true</b> if the operation was successful, otherwise <b>false</b>.</returns>
        public bool SetEapUserData(IntPtr handle, string phoneBookPath, string entryName, byte[] data)
        {
            bool result = false;

            IntPtr lpEapData = IntPtr.Zero;

            try
            {
                int size = 0;
                if (data != null)
                {
                    size = data.Length;
                    lpEapData = Marshal.AllocHGlobal(size);

                    // Transfer the data into the allocated memory block.
                    Marshal.Copy(data, 0, lpEapData, size);
                }

                int ret = UnsafeNativeMethods.Instance.SetEapUserData(handle, phoneBookPath, entryName, lpEapData, size);
                if (ret == NativeMethods.SUCCESS)
                {
                    result = true;
                }
                else
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            finally
            {
                if (lpEapData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpEapData);
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the entry properties for an existing phone book entry, or creates a new entry.
        /// </summary>
        /// <param name="phoneBook">Required. The <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> that will contain the entry.</param>
        /// <param name="value">An <see cref="MessagingToolkit.Core.Ras.RasEntry"/> object whose properties to set.</param>        
        /// <returns><b>true</b> if the operation was successful, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBook"/> or <paramref name="value"/> are a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public bool SetEntryProperties(RasPhoneBook phoneBook, RasEntry value)
        {
            if (phoneBook == null)
            {
                ThrowHelper.ThrowArgumentNullException("phoneBook");
            }

            if (value == null)
            {
                ThrowHelper.ThrowArgumentNullException("value");
            }

            if (string.IsNullOrEmpty(value.Name))
            {
                ThrowHelper.ThrowArgumentException("Entry name", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            if (!RasHelper.Instance.IsValidEntryName(phoneBook, value.Name, NativeMethods.ERROR_ALREADY_EXISTS, NativeMethods.ERROR_CANNOT_OPEN_PHONEBOOK))
            {
                ThrowHelper.ThrowArgumentException("entry name", Resources.Argument_InvalidEntryName, "entry name", value.Name);
            }

            // Ensure the entry meets the minimum requirements to create or update a phone book entry.
            if ((value.PhoneNumber == null && value.AlternatePhoneNumbers.Count == 0) || (value.Device == null || (value.Device != null && (string.IsNullOrEmpty(value.Device.DeviceType) || string.IsNullOrEmpty(value.Device.Name)))) || value.FramingProtocol == RasFramingProtocol.None || value.EntryType == RasEntryType.None)
            {
                ThrowHelper.ThrowArgumentException("entry", Resources.Argument_MissingRequiredInfo);
            }

            bool retval = false;
            int size = Marshal.SizeOf(typeof(NativeMethods.RASENTRY));
            int lpCb = size;

            IntPtr lpRasEntry = IntPtr.Zero;
            try
            {
                NativeMethods.RASENTRY entry = new NativeMethods.RASENTRY();
                entry.size = size;

#pragma warning disable 0618
                entry.autoDialDll = value.AutoDialDll;
                entry.autoDialFunc = value.AutoDialFunc;
#pragma warning restore 0618

                entry.areaCode = value.AreaCode;
                entry.channels = value.Channels;
                entry.countryCode = value.CountryCode;
                entry.countryId = value.CountryId;
                entry.customAuthKey = value.CustomAuthKey;
                entry.customDialDll = value.CustomDialDll;

                if (value.Device != null)
                {
                    entry.deviceName = value.Device.Name;
                    entry.deviceType = value.Device.DeviceType;
                }

                entry.dialExtraPercent = value.DialExtraPercent;
                entry.dialExtraSampleSeconds = value.DialExtraSampleSeconds;
                entry.dialMode = value.DialMode;
                entry.dnsAddress = Utilities.GetRasIPAddress(value.DnsAddress);
                entry.dnsAddressAlt = Utilities.GetRasIPAddress(value.DnsAddressAlt);
                entry.encryptionType = value.EncryptionType;
                entry.entryType = value.EntryType;
                entry.frameSize = value.FrameSize;
                entry.framingProtocol = value.FramingProtocol;
                entry.hangUpExtraPercent = value.HangUpExtraPercent;
                entry.hangUpExtraSampleSeconds = value.HangUpExtraSampleSeconds;
                entry.id = value.Id;
                entry.idleDisconnectSeconds = value.IdleDisconnectSeconds;
                entry.ipAddress = Utilities.GetRasIPAddress(value.IPAddress);
                entry.networkProtocols = Utilities.GetRasNetworkProtocols(value.NetworkProtocols);
                entry.options = Utilities.GetRasEntryOptions(value);
                entry.phoneNumber = value.PhoneNumber;
                entry.script = value.Script;

                // This member should be set to zero and the subentries should be added after the entry has been created.
                entry.subentries = 0;

                entry.vpnStrategy = value.VpnStrategy;
                entry.winsAddress = Utilities.GetRasIPAddress(value.WinsAddress);
                entry.winsAddressAlt = Utilities.GetRasIPAddress(value.WinsAddressAlt);
                entry.x25Address = value.X25Address;
                entry.x25Facilities = value.X25Facilities;
                entry.x25PadType = value.X25PadType;
                entry.x25UserData = value.X25UserData;

#if (WINXP || WIN2K8 || WIN7)
                entry.options2 = Utilities.GetRasEntryExtendedOptions(value);
                entry.options3 = 0;
                entry.dnsSuffix = value.DnsSuffix;
                entry.tcpWindowSize = value.TcpWindowSize;
                entry.prerequisitePhoneBook = value.PrerequisitePhoneBook;
                entry.prerequisiteEntryName = value.PrerequisiteEntryName;
                entry.redialCount = value.RedialCount;
                entry.redialPause = value.RedialPause;
#endif
#if (WIN2K8 || WIN7)
                entry.ipv4InterfaceMetric = value.IPv4InterfaceMetric;
                entry.ipv6DnsAddress = Utilities.GetRasIPv6Address(value.IPv6DnsAddress);
                entry.ipv6DnsAddressAlt = Utilities.GetRasIPv6Address(value.IPv6DnsAddressAlt);
                entry.ipv6InterfaceMetric = value.IPv6InterfaceMetric;
#endif
#if (WIN7)
                entry.ipv6Address = Utilities.GetRasIPv6Address(value.IPv6Address);
                entry.ipv6PrefixLength = value.IPv6PrefixLength;
                entry.networkOutageTime = value.NetworkOutageTime;
#endif

                int alternatesLength = 0;
                string alternatesList = Utilities.BuildStringList(value.AlternatePhoneNumbers, '\x00', out alternatesLength);
                if (alternatesLength > 0)
                {
                    lpCb = size + alternatesLength;
                    entry.alternateOffset = size;
                }

                lpRasEntry = Marshal.AllocHGlobal(lpCb);
                Marshal.StructureToPtr(entry, lpRasEntry, true);

                if (alternatesLength > 0)
                {
                    // Now that the pointer has been allocated, copy the string to the location.
                    Utilities.CopyString(lpRasEntry, size, alternatesList, alternatesLength);
                }

                try
                {
                    int ret = UnsafeNativeMethods.Instance.SetEntryProperties(phoneBook.Path, value.Name, lpRasEntry, lpCb, IntPtr.Zero, 0);
                    if (ret == NativeMethods.ERROR_ACCESS_DENIED)
                    {
                        ThrowHelper.ThrowUnauthorizedAccessException(Resources.Exception_AccessDeniedBySecurity);
                    }
                    else if (ret == NativeMethods.ERROR_INVALID_PARAMETER)
                    {
                        ThrowHelper.ThrowArgumentException("entry", Resources.Argument_InvalidOrConflictingEntrySettings);
                    }
                    else if (ret == NativeMethods.SUCCESS)
                    {
                        retval = true;

                        if (value.SubEntries.Count > 0)
                        {
                            // The entry has subentries associated with it, add them to the phone book.
                            for (int index = 0; index < value.SubEntries.Count; index++)
                            {
                                RasSubEntry subEntry = value.SubEntries[index];
                                if (subEntry != null)
                                {
                                    RasHelper.Instance.SetSubEntryProperties(value.Owner, value, index, subEntry);
                                }
                            }
                        }

                        if (value.Id == Guid.Empty)
                        {
                            // The entry being set is new, update any properties that need an existing entry.
                            RasEntry newEntry = null;
                            try
                            {
                                // Grab the entry from the phone book.
                                newEntry = RasHelper.Instance.GetEntryProperties(phoneBook, value.Name);
                                value.Id = newEntry.Id;
                            }
                            finally
                            {
                                newEntry = null;
                            }
                        }
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
            }
            finally
            {
                if (lpRasEntry != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpRasEntry);
                }
            }

            return retval;
        }

        /// <summary>
        /// Sets the subentry properties for an existing subentry, or creates a new subentry.
        /// </summary>
        /// <param name="phoneBook">Required. The <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> that will contain the entry.</param>
        /// <param name="entry">Required. The <see cref="MessagingToolkit.Core.Ras.RasEntry"/> whose subentry to set.</param>
        /// <param name="subEntryId">The zero-based index of the subentry to set.</param>
        /// <param name="value">An <see cref="MessagingToolkit.Core.Ras.RasSubEntry"/> object whose properties to set.</param>
        /// <returns><b>true</b> if the operation was successful, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBook"/> or <paramref name="entry"/> or <paramref name="value"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public bool SetSubEntryProperties(RasPhoneBook phoneBook, RasEntry entry, int subEntryId, RasSubEntry value)
        {
            if (phoneBook == null)
            {
                ThrowHelper.ThrowArgumentNullException("phoneBook");
            }

            if (value == null)
            {
                ThrowHelper.ThrowArgumentNullException("value");
            }

            if (entry == null)
            {
                ThrowHelper.ThrowArgumentNullException("entry");
            }

            bool retval = false;
            int size = Marshal.SizeOf(typeof(NativeMethods.RASSUBENTRY));
            int lpCb = size;

            IntPtr lpRasSubEntry = IntPtr.Zero;
            try
            {
                NativeMethods.RASSUBENTRY subentry = new NativeMethods.RASSUBENTRY();
                subentry.size = size;
                subentry.phoneNumber = value.PhoneNumber;

                if (value.Device != null)
                {
                    subentry.deviceName = value.Device.Name;
                    subentry.deviceType = value.Device.DeviceType;
                }

                int alternatesLength = 0;
                string alternatesList = Utilities.BuildStringList(value.AlternatePhoneNumbers, '\x00', out alternatesLength);
                if (alternatesLength > 0)
                {
                    lpCb = size + alternatesLength;
                    subentry.alternateOffset = size;
                }

                lpRasSubEntry = Marshal.AllocHGlobal(lpCb);
                Marshal.StructureToPtr(subentry, lpRasSubEntry, true);

                if (alternatesLength > 0)
                {
                    Utilities.CopyString(lpRasSubEntry, size, alternatesList, alternatesLength);
                }

                try
                {
                    int ret = UnsafeNativeMethods.Instance.SetSubEntryProperties(phoneBook.Path, entry.Name, subEntryId + 2, lpRasSubEntry, lpCb, IntPtr.Zero, 0);
                    if (ret == NativeMethods.SUCCESS)
                    {
                        retval = true;
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
            }
            finally
            {
                if (lpRasSubEntry != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpRasSubEntry);
                }
            }

            return retval;
        }

        /// <summary>
        /// Sets the user credentials for a phone book entry.
        /// </summary>
        /// <param name="phoneBookPath">The full path (including filename) of a phone book. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The name of the entry whose credentials to set.</param>
        /// <param name="credentials">An <see cref="NativeMethods.RASCREDENTIALS"/> object containing user credentials.</param>
        /// <param name="clearCredentials"><b>true</b> clears existing credentials by setting them to an empty string, otherwise <b>false</b>.</param>
        /// <returns><b>true</b> if the operation was successful, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="phoneBookPath"/> or <paramref name="entryName"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public bool SetCredentials(string phoneBookPath, string entryName, NativeMethods.RASCREDENTIALS credentials, bool clearCredentials)
        {
            if (string.IsNullOrEmpty(phoneBookPath))
            {
                ThrowHelper.ThrowArgumentException("phoneBookPath", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            if (string.IsNullOrEmpty(entryName))
            {
                ThrowHelper.ThrowArgumentException("entryName", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            int size = Marshal.SizeOf(typeof(NativeMethods.RASCREDENTIALS));
            bool retval = false;

            IntPtr pCredentials = IntPtr.Zero;
            try
            {
                credentials.size = size;

                pCredentials = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(credentials, pCredentials, true);

                try
                {
                    int ret = UnsafeNativeMethods.Instance.SetCredentials(phoneBookPath, entryName, pCredentials, clearCredentials);
                    if (ret == NativeMethods.SUCCESS)
                    {
                        retval = true;
                    }
                    else if (ret == NativeMethods.ERROR_ACCESS_DENIED)
                    {
                        ThrowHelper.ThrowUnauthorizedAccessException(Resources.Exception_SetCredentialsAccessDenied);
                    }
                    else
                    {
                        ThrowHelper.ThrowRasException(ret);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
                }
            }
            finally
            {
                if (pCredentials != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pCredentials);
                }
            }

            return retval;
        }

#if (WIN7)
        /// <summary>
        /// Updates the tunnel endpoints of an Internet Key Exchange (IKEv2) connection.
        /// </summary>
        /// <param name="handle">The handle of the connection.</param>
        /// <param name="interfaceIndex">The new interface index of the endpoint.</param>
        /// <param name="localEndPoint">The new local client endpoint of the connection.</param>
        /// <param name="remoteEndPoint">The new remote server endpoint of the connection.</param>
        public void UpdateConnection(RasHandle handle, int interfaceIndex, IPAddress localEndPoint, IPAddress remoteEndPoint)
        {
            int size = Marshal.SizeOf(typeof(NativeMethods.RASUPDATECONN));

            IntPtr pRasUpdateConn = IntPtr.Zero;
            try
            {
                NativeMethods.RASUPDATECONN updateConn = new NativeMethods.RASUPDATECONN();
                updateConn.version = GetCurrentApiVersion();
                updateConn.size = size;
                updateConn.interfaceIndex = interfaceIndex;
                updateConn.localEndPoint = Utilities.GetEndPointFromIPAddress(localEndPoint);
                updateConn.remoteEndPoint = Utilities.GetEndPointFromIPAddress(remoteEndPoint);

                pRasUpdateConn = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(updateConn, pRasUpdateConn, true);

                int ret = UnsafeNativeMethods.Instance.UpdateConnection(handle, pRasUpdateConn);
                if (ret == NativeMethods.ERROR_INVALID_PARAMETER)
                {
                    ThrowHelper.ThrowInvalidOperationException(Resources.Exception_InvalidIkeV2Handle);
                }
                else if (ret != NativeMethods.SUCCESS)
                {
                    ThrowHelper.ThrowRasException(ret);
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            finally
            {
                if (pRasUpdateConn != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pRasUpdateConn);
                }
            }
        }
#endif

        /// <summary>
        /// Retrieves the remote access service (RAS) API currently in use.
        /// </summary>
        /// <returns>The <see cref="NativeMethods.RASAPIVERSION"/> in use.</returns>
        private static NativeMethods.RASAPIVERSION GetCurrentApiVersion()
        {
#if (WINXP)
            return NativeMethods.RASAPIVERSION.WinXP;
#elif (WIN2K8)
            return NativeMethods.RASAPIVERSION.WinVista;
#elif (WIN7)
            return NativeMethods.RASAPIVERSION.Win7;
#else
            return NativeMethods.RASAPIVERSION.Win2K;
#endif
        }

        #endregion
    }
}
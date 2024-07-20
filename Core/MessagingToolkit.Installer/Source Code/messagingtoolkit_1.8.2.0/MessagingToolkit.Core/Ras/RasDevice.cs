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
    using System.Globalization;
    using MessagingToolkit.Core.Ras.Internal;
    using MessagingToolkit.Core.Properties;

    /// <summary>
    /// Represents a TAPI device capable of establishing a remote access connection. This class cannot be inherited.
    /// </summary>
    /// <example>
    /// The following example shows how to update the device used by a phone book entry.
    /// <code lang="C#">
    /// using (RasPhoneBook pbk = new RasPhoneBook())
    /// {
    ///    pbk.Open();
    ///    RasEntry entry = pbk.Entries["VPN Connection"];
    ///    if (entry != null)
    ///    {
    ///        entry.Device = RasDevice.GetDeviceByName("(PPTP)", RasDeviceType.Vpn);
    ///        entry.Update();
    ///    }
    /// }
    /// </code>
    /// <code lang="VB.NET">
    /// Dim pbk As RasPhoneBook
    /// Try
    ///    pbk = New RasPhoneBook
    ///    pbk.Open()
    ///    Dim entry As RasEntry = pbk.Entries("VPN Connection")
    ///    If (entry IsNot Nothing) Then
    ///        entry.Device = RasDevice.GetDeviceByName("(PPTP)", RasDeviceType.Vpn)
    ///        entry.Update()
    ///    End If
    /// Finally
    ///    If (pbk IsNot Nothing) Then
    ///        pbk.Dispose()
    ///    End If 
    /// End Try
    /// </code>
    /// </example>
    [Serializable]
    [DebuggerDisplay("Name = {Name}, DeviceType = {DeviceType}")]
    public sealed class RasDevice
    {
        #region Fields

        private string _deviceType;
        private string _name;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasDevice"/> class.
        /// </summary>
        /// <param name="name">The name of the device.</param>
        /// <param name="deviceType">The type of the device.</param>
        private RasDevice(string name, string deviceType)
        {
            this._name = name;
            this._deviceType = deviceType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        /// <remarks>
        /// All or parts of this value may change depending on the language pack used by the operating system. For example, the "WAN Miniport (L2TP)" device can read "Minipuerto WAN (L2TP)" on machines using a Spanish language pack.
        /// </remarks>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>
        /// Gets the type of the device.
        /// </summary>
        /// <remarks>See <see cref="RasDeviceType"/> for possible values.</remarks>
        public string DeviceType
        {
            get { return this._deviceType; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new device. WARNING: This method does not guarantee the hardware will be installed.
        /// </summary>
        /// <param name="name">Required. The name of the device.</param>
        /// <param name="deviceType">Required. The <see cref="MessagingToolkit.Core.Ras.RasDeviceType"/> indicating the type of device.</param>
        /// <remarks>
        /// This method essentially creates hardware on machines that may not exist and is exposed due to problems when hardware is installed and not recognized immediately by the remote access service. Using this method does not guarantee the hardware will be installed and may cause the machine to crash when the entry is dialed.
        /// </remarks>
        /// <returns>A new <see cref="MessagingToolkit.Core.Ras.RasDevice"/> object.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="deviceType"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        public static RasDevice Create(string name, string deviceType)
        {
            if (name == null)
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(deviceType))
            {
                ThrowHelper.ThrowArgumentException("deviceType", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            return new RasDevice(name, deviceType);
        }

        /// <summary>
        /// Lists all available remote access capable devices.
        /// </summary>
        /// <returns>A new read-only collection of <see cref="MessagingToolkit.Core.Ras.RasDevice"/> objects.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This should not be a property.")]
        public static ReadOnlyCollection<RasDevice> GetDevices()
        {
            return RasHelper.Instance.GetDevices();
        }

        /// <summary>
        /// Lists all remote access capable devices for the specified device type.
        /// </summary>
        /// <param name="deviceType">The <see cref="MessagingToolkit.Core.Ras.RasDeviceType"/> of the device(s) to retrieve.</param>
        /// <returns>A new read-only collection of <see cref="MessagingToolkit.Core.Ras.RasDevice"/> objects, or an empty collection if no devices were found.</returns>
        public static ReadOnlyCollection<RasDevice> GetDevicesByType(string deviceType)
        {
            if (string.IsNullOrEmpty(deviceType))
            {
                ThrowHelper.ThrowArgumentException("deviceType", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            Collection<RasDevice> tempCollection = new Collection<RasDevice>();

            foreach (RasDevice device in RasDevice.GetDevices())
            {
                if (string.Compare(device.DeviceType, deviceType, true, CultureInfo.CurrentCulture) == 0)
                {
                    tempCollection.Add(device);
                }
            }

            return new ReadOnlyCollection<RasDevice>(tempCollection);
        }

        /// <summary>
        /// Returns the first device matching the name and device type specified.
        /// </summary>
        /// <param name="name">The name of the device to retrieve.</param>
        /// <param name="deviceType">The <see cref="MessagingToolkit.Core.Ras.RasDeviceType"/> of the device to retrieve.</param>
        /// <returns>The <see cref="MessagingToolkit.Core.Ras.RasDevice"/> if available, otherwise a null reference (<b>Nothing</b> in Visual Basic).</returns>
        public static RasDevice GetDeviceByName(string name, string deviceType)
        {
            return RasDevice.GetDeviceByName(name, deviceType, false);
        }

        /// <summary>
        /// Returns the first device matching the name and device type specified.
        /// </summary>
        /// <param name="name">The name (or partial name) of the device to retrieve.</param>
        /// <param name="deviceType">The <see cref="MessagingToolkit.Core.Ras.RasDeviceType"/> of the device to retrieve.</param>
        /// <param name="exactMatchOnly"><b>true</b> if the <paramref name="name"/> should be matched exactly; <b>false</b> to allow partial matching.</param>
        /// <returns>The <see cref="MessagingToolkit.Core.Ras.RasDevice"/> if available, otherwise a null reference (<b>Nothing</b> in Visual Basic).</returns>
        /// <exception cref="System.ArgumentException"><paramref name="deviceType"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "The argument has been validated.")]
        public static RasDevice GetDeviceByName(string name, string deviceType, bool exactMatchOnly)
        {
            if (name == null)
            {
                ThrowHelper.ThrowArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(deviceType))
            {
                ThrowHelper.ThrowArgumentException("deviceType", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            RasDevice retval = null;

            foreach (RasDevice device in RasDevice.GetDevices())
            {
                if (string.Compare(deviceType, device.DeviceType, true, CultureInfo.CurrentCulture) == 0 && ((!exactMatchOnly && device.Name.ToLower(CultureInfo.CurrentCulture).Contains(name.ToLower(CultureInfo.CurrentCulture))) || (exactMatchOnly && string.Compare(name, device.Name, false, CultureInfo.CurrentCulture) == 0)))
                {
                    retval = device;
                    break;
                }
            }

            return retval;
        }

        #endregion
    }
}
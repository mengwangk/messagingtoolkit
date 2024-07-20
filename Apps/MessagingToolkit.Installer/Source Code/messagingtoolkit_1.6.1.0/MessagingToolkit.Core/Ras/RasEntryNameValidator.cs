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
    using MessagingToolkit.Core.Ras.Internal;
    using MessagingToolkit.Core.Properties;

    /// <summary>
    /// Validates the format of an entry name for a phone book. This class cannot be inherited.
    /// </summary>
    /// <remarks>The name must contain at least one non-whitespace alphanumeric character.</remarks>
    /// <example>
    /// The following example shows how to use the <b>RasEntryNameValidator</b> to validate an entry does not already exist within a phone book. After the <see cref="RasEntryNameValidator.Validate"/> method has been called, the <see cref="RasEntryNameValidator.IsValid"/> property will indicate whether the entry name is valid.
    /// <code lang="C#">
    /// <![CDATA[
    /// RasEntryNameValidator validator = new RasEntryNameValidator();
    /// validator.EntryName = "VPN Connection";
    /// validator.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
    /// validator.AllowExistingEntries = false;
    /// validator.Validate();
    /// ]]>
    /// </code>
    /// <code lang="VB.NET">
    /// <![CDATA[
    /// Dim validator As New RasEntryNameValidator
    /// validator.EntryName = "VPN Connection"
    /// validator.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers)
    /// validator.AllowExistingEntries = False
    /// validator.Validate()
    /// ]]>
    /// </code>
    /// </example>
    public sealed class RasEntryNameValidator
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasEntryNameValidator"/> class.
        /// </summary>
        public RasEntryNameValidator()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the error code, if any, that occurred during validation.
        /// </summary>
        public int ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the entry name is valid.
        /// </summary>
        public bool IsValid
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether existing entries will be allowed.
        /// </summary>
        public bool AllowExistingEntries
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether nonexistent phone books are allowed.
        /// </summary>
        public bool AllowNonExistentPhoneBook
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entry name to validate.
        /// </summary>
        public string EntryName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the phone book path to validate the entry name against.
        /// </summary>
        public string PhoneBookPath
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Evaluates the condition it checks, and updates the <see cref="IsValid"/> property.
        /// </summary>
        public void Validate()
        {
            try
            {
                int errorCode = SafeNativeMethods.Instance.ValidateEntryName(this.PhoneBookPath, this.EntryName);

                if (errorCode == NativeMethods.SUCCESS || (this.AllowExistingEntries && errorCode == NativeMethods.ERROR_ALREADY_EXISTS) || (this.AllowNonExistentPhoneBook && errorCode == NativeMethods.ERROR_CANNOT_OPEN_PHONEBOOK))
                {
                    this.ErrorCode = NativeMethods.SUCCESS;
                    this.ErrorMessage = null;
                }
                else
                {
                    this.ErrorCode = errorCode;
                    this.ErrorMessage = RasHelper.Instance.GetRasErrorString(errorCode);
                }

                this.IsValid = this.ErrorCode == NativeMethods.SUCCESS;
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
        }

        #endregion
    }
}
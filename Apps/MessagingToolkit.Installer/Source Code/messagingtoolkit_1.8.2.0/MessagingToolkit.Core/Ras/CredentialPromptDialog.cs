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
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;
    using MessagingToolkit.Core.Ras.Internal;
    using MessagingToolkit.Core.Properties;
    using Microsoft.Win32;

#if (WINXP || WIN2K8 || WIN7)

    /// <summary>
    /// Prompts the user for credentials. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="TargetName"/> property should contain the name of the server or machine for which credentials are being requested. This dialog also ties into the Windows credential store to pre-populate the user name if the stored credentials are found.
    /// </para>
    /// <para>
    /// This class uses the <b>Network Access: Do not allow storage of credentials or .NET passports for network authentication</b> policy setting. If this policy is enabled, the dialog will not be able to persist credentials.
    /// </para>
    /// <para>
    /// <b>Known Limitations:</b>
    /// <list type="bullet">
    /// <item>This component is only available on Windows XP and later operating systems.</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows how to request credentials from the user.
    /// <code lang="C#">
    /// <![CDATA[
    /// using (CredentialPromptDialog dialog = new CredentialPromptDialog())
    /// {
    ///     dialog.TargetName = "ServerName";
    ///     if (dialog.ShowDialog() == DialogResult.OK)
    ///     {
    ///         // The UserName and Password properties will contain the result.
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// <code lang="VB.NET">
    /// <![CDATA[
    /// Dim dialog As CredentialPromptDialog
    /// Try
    ///     dialog = New CredentialPromptDialog()
    ///     dialog.TargetName = "ServerName"
    ///     If dialog.ShowDialog() = DialogResult.OK Then
    ///         ' The UserName and Password properties will contain the result.
    ///     End If
    /// Finally
    ///     If dialog IsNot Nothing Then
    ///         dialog.Dispose()
    ///     End If
    /// End Try
    /// ]]>
    /// </code>
    /// </example>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(CredentialPromptDialog), "MessagingToolkit.Core.Ras.CredentialPromptDialog.bmp")]
    public sealed class CredentialPromptDialog : CommonDialog
    {
        #region Fields

        /// <summary>
        /// Contains the full path to the disable domain creds DWORD registry setting.
        /// </summary>
        private const string DisableDomainCredsPath = "SYSTEM\\CurrentControlSet\\Control\\Lsa";

        /// <summary>
        /// Holds the value of the <see cref="MaxUserNameLength"/> property.
        /// </summary>
        private int maxUserNameLength;

        /// <summary>
        /// Holds the value of the <see cref="MaxPasswordLength"/> property.
        /// </summary>
        private int maxPasswordLength;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.CredentialPromptDialog"/> class.
        /// </summary>
        public CredentialPromptDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.CredentialPromptDialog"/> class.
        /// </summary>
        /// <param name="container">An <see cref="System.ComponentModel.IContainer"/> that will contain the component.</param>
        public CredentialPromptDialog(IContainer container)
        {
            if (container != null)
            {
                container.Add(this);
            }

            this.InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the caption of the dialog.
        /// </summary>
        [DefaultValue(null)]
        [SRCategory("CatAppearance")]
        [SRDescription("CPDCaptionDesc")]
        public string Caption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <remarks>Upon return from the dialog, this value will contain the user name entered by the user.</remarks>
        [DefaultValue(null)]
        [SRCategory("CatData")]
        [SRDescription("CPDUserNameDesc")]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <remarks>Upon return from the dialog, this value will contain the password entered by the user.</remarks>
        [DefaultValue(null)]
        [SRCategory("CatData")]
        [SRDescription("CPDPasswordDesc")]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target name.
        /// </summary>
        [DefaultValue(null)]
        [SRCategory("CatBehavior")]
        [SRDescription("CPDTargetNameDesc")]
        public string TargetName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message of the dialog.
        /// </summary>
        [DefaultValue(null)]
        [SRCategory("CatAppearance")]
        [SRDescription("CPDMessageDesc")]
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the save checkbox will be checked.
        /// </summary>
        /// <remarks>Upon return from the dialog, this value will contain the last state of the checkbox.</remarks>
        [DefaultValue(false)]
        [SRCategory("CatBehavior")]
        [SRDescription("CPDSaveCheckedDesc")]
        public bool SaveChecked
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum length of the user name.
        /// </summary>
        /// <exception cref="System.ArgumentException"><paramref name="value"/> must be greater than or equal to zero, and less than or equal to 513.</exception>
        [DefaultValue(0)]
        [SRCategory("CatBehavior")]
        [SRDescription("CPDMaxUserNameLengthDesc")]
        public int MaxUserNameLength
        {
            get
            {
                return this.maxUserNameLength;
            }

            set
            {
                if (value < 0 || value > NativeMethods.CREDUI_MAX_USERNAME_LENGTH)
                {
                    ThrowHelper.ThrowArgumentException("value", Resources.Argument_ValueMustBeGreaterThanZeroLessThanOrEqualToOtherValue, NativeMethods.CREDUI_MAX_USERNAME_LENGTH);
                }

                this.maxUserNameLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum length of the password.
        /// </summary>
        /// <exception cref="System.ArgumentException"><paramref name="value"/> must be greater than or equal to zero, and less than or equal to 256.</exception>
        [DefaultValue(0)]
        [SRCategory("CatBehavior")]
        [SRDescription("CPDMaxPasswordLengthDesc")]
        public int MaxPasswordLength
        {
            get
            {
                return this.maxPasswordLength;
            }

            set
            {
                if (value < 0 || value > NativeMethods.CREDUI_MAX_PASSWORD_LENGTH)
                {
                    ThrowHelper.ThrowArgumentException("value", Resources.Argument_ValueMustBeGreaterThanZeroLessThanOrEqualToOtherValue, NativeMethods.CREDUI_MAX_PASSWORD_LENGTH);
                }

                this.maxPasswordLength = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the username should be disabled.
        /// </summary>
        /// <remarks>Setting this to <b>true</b> automatically removes the 'Remember my password' checkbox.</remarks>
        [DefaultValue(false)]
        [SRCategory("CatBehavior")]
        [SRDescription("CPDDisableUserNameDesc")]
        public bool DisableUserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the 'Remember my password' checkbox will be shown.
        /// </summary>
        [DefaultValue(false)]
        [SRCategory("CatAppearance")]
        [SRDescription("CPDShowSaveCheckBoxDesc")]
        public bool ShowSaveCheckBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user should be notified the password was incorrect.
        /// </summary>
        [DefaultValue(false)]
        [SRCategory("CatBehavior")]
        [SRDescription("CPDNotifyIncorrectPasswordDesc")]
        public bool NotifyIncorrectPassword
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets all <see cref="CredentialPromptDialog"/> properties to their default values.
        /// </summary>
        public override void Reset()
        {
            this.Caption = null;
            this.UserName = null;
            this.Password = null;
            this.TargetName = null;
            this.Message = null;
            this.MaxUserNameLength = 0;
            this.MaxPasswordLength = 0;
            this.DisableUserName = false;
            this.ShowSaveCheckBox = false;
            this.NotifyIncorrectPassword = false;
        }

        /// <summary>
        /// Overridden. Displays the modal dialog.
        /// </summary>
        /// <param name="hwndOwner">The handle of the window that owns the dialog box.</param>
        /// <returns><b>true</b> if the user completed the entry successfully, otherwise <b>false</b>.</returns>
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            if (this.TargetName == null)
            {
                ThrowHelper.ThrowInvalidOperationException(Resources.Exception_TargetNameCannotBeNullReference);
            }

            bool retval = false;

            int size = Marshal.SizeOf(typeof(NativeMethods.CREDUI_INFO));

            IntPtr pUiCred = IntPtr.Zero;
            try
            {
                NativeMethods.CREDUI_INFO dlg = new NativeMethods.CREDUI_INFO();
                dlg.size = size;                

                dlg.caption = this.Caption;
                dlg.message = this.Message;
                dlg.hwndOwner = hwndOwner;

                pUiCred = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(dlg, pUiCred, true);

                bool saveChecked = this.SaveChecked;

                // Use the default maximum lengths if the caller did not provide a different value.
                int userNameLength = this.MaxUserNameLength == 0 ? NativeMethods.CREDUI_MAX_USERNAME_LENGTH : this.MaxUserNameLength;
                int passwordLength = this.MaxPasswordLength == 0 ? NativeMethods.CREDUI_MAX_PASSWORD_LENGTH : this.MaxPasswordLength;

                StringBuilder userNameBuffer = new StringBuilder(userNameLength);
                StringBuilder passwordBuffer = new StringBuilder(passwordLength);

                if (!string.IsNullOrEmpty(this.UserName))
                {
                    userNameBuffer.Append(this.UserName);
                }

                if (!string.IsNullOrEmpty(this.Password))
                {
                    passwordBuffer.Append(this.Password);
                }

                NativeMethods.CREDUI_FLAGS flags = this.BuildDialogOptions();

                int ret = SafeNativeMethods.Instance.PromptForCredentials(pUiCred, this.TargetName, IntPtr.Zero, NativeMethods.SUCCESS, userNameBuffer, userNameLength, passwordBuffer, passwordLength, ref saveChecked, flags);
                if (ret == NativeMethods.ERROR_CANCELLED)
                {
                    retval = false;
                }
                else if (ret == NativeMethods.SUCCESS)
                {
                    // Update the output values on the component with the new values returned from the dialog box.
                    this.SaveChecked = saveChecked;

                    this.UserName = userNameBuffer.ToString();
                    this.Password = passwordBuffer.ToString();

                    retval = true;
                }
                else
                {
                    ThrowHelper.ThrowWin32Exception(ret);
                }
            }
            finally
            {
                if (pUiCred != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pUiCred);
                }
            }

            return retval;
        }

        /// <summary>
        /// Indicates whether the policy setting disabling the storing of credentials is enabled.
        /// </summary>
        /// <returns><b>true</b> if the policy setting is enabled, otherwise <b>false</b>.</returns>
        private static bool IsPolicyEnabled()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(DisableDomainCredsPath);
            if (key != null)
            {
                return (int)key.GetValue("disabledomaincreds", 0) != 0;
            }

            return false;
        }

        /// <summary>
        /// Builds the dialog options.
        /// </summary>
        /// <returns>The dialog options.</returns>
        private NativeMethods.CREDUI_FLAGS BuildDialogOptions()
        {
            NativeMethods.CREDUI_FLAGS result = NativeMethods.CREDUI_FLAGS.GenericCredentials | NativeMethods.CREDUI_FLAGS.AlwaysShowUI;

            if (IsPolicyEnabled())
            {
                result = result | NativeMethods.CREDUI_FLAGS.DoNotPersist;
            }
            else
            {
                if (this.ShowSaveCheckBox)
                {
                    result = result | NativeMethods.CREDUI_FLAGS.ShowSaveCheckBox | NativeMethods.CREDUI_FLAGS.DoNotPersist;
                }
                else
                {
                    result = result | NativeMethods.CREDUI_FLAGS.DoNotPersist;
                }
            }

            if (this.DisableUserName)
            {
                result = result | NativeMethods.CREDUI_FLAGS.KeepUserName;
            }

            if (this.NotifyIncorrectPassword)
            {
                result = result | NativeMethods.CREDUI_FLAGS.IncorrectPassword;
            }

            return result;
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            this.MaxPasswordLength = NativeMethods.CREDUI_MAX_PASSWORD_LENGTH;
            this.MaxUserNameLength = NativeMethods.CREDUI_MAX_USERNAME_LENGTH;
        }

        #endregion
    }

#endif
}
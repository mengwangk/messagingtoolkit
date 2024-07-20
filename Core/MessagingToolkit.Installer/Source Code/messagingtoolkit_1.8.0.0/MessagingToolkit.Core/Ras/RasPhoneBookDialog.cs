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
    using System.Security;
    using MessagingToolkit.Core.Ras.Design;
    using MessagingToolkit.Core.Ras.Internal;
    using MessagingToolkit.Core.Properties;

    /// <summary>
    /// Displays the primary Dial-Up Networking dialog box. This class cannot be inherited.
    /// </summary>
    /// <example>
    /// The following example shows how to use the <b>RasPhoneBookDialog</b> component to display the main dial-up networking dialog box.
    /// <code lang="C#">
    /// <![CDATA[
    /// RasPhoneBookDialog dialog = new RasPhoneBookDialog();
    /// public void Begin()
    /// {
    ///     dialog.AddedEntry += new EventHandler<EventArgs>(this.dialog_AddedEntry);
    ///     dialog.ChangedEntry += new EventHandler<EventArgs>(this.dialog_ChangedEntry);
    ///     dialog.DialedEntry += new EventHandler<EventArgs>(this.dialog_DialedEntry);
    ///     dialog.RemovedEntry += new EventHandler<EventArgs>(this.dialog_RemovedEntry);
    ///     if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
    ///     {
    ///         // The dialog has closed successfully.
    ///     }
    /// }
    /// private void dialog_AddedEntry(object sender, EventArgs e)
    /// {
    ///     // The dialog has added a new entry.
    /// }
    /// private void dialog_ChangedEntry(object sender, EventArgs e)
    /// {
    ///     // The dialog has changed an entry.
    /// }
    /// private void dialog_DialedEntry(object sender, EventArgs e)
    /// {
    ///     // The dialog has dialed an entry.
    /// }
    /// private void dialog_RemovedEntry(object sender, EventArgs e)
    /// {
    ///     // The dialog removed an entry.
    /// }
    /// ]]>
    /// </code>
    /// <code lang="VB.NET">
    /// <![CDATA[
    /// Dim dialog As New RasPhoneBookDialog
    /// Public Sub Begin()
    ///     AddHandler dialog.AddedEntry, Me.dialog_AddedEntry
    ///     AddHandler dialog.ChangedEntry, Me.dialog_ChangedEntry
    ///     AddHandler dialog.DialedEntry, Me.dialog_DialedEntry
    ///     AddHandler dialog.RemovedEntry, Me.dialog_RemovedEntry
    ///     If (dialog.ShowDialog() = System.Windows.Forms.DialogResult.OK) Then
    ///         ' The dialog has closed successfully.
    ///     End If
    /// End Sub
    /// Private Sub dialog_AddedEntry(ByVal sender As Object, ByVal e As EventArgs)
    ///     ' The dialog has added a new entry.
    /// End Sub
    /// Private Sub dialog_ChangedEntry(ByVal sender As Object, ByVal e As EventArgs)
    ///     ' The dialog has changed an entry.
    /// End Sub
    /// Private Sub dialog_DialedEntry(ByVal sender As Object, ByVal e As EventArgs)
    ///     ' The dialog has dialed an entry.
    /// End Sub
    /// Private Sub dialog_RemovedEntry(ByVal sender As Object, ByVal e As EventArgs)
    ///     ' The dialog removed an entry.
    /// End Sub
    /// ]]>
    /// </code>
    /// </example>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(RasPhoneBookDialog), "MessagingToolkit.Core.Ras.RasPhoneBookDialog.bmp")]
    public sealed class RasPhoneBookDialog : RasCommonDialog
    {
        #region Fields

        private NativeMethods.RasPBDlgFunc rasPhonebookDlgCallback;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasPhoneBookDialog"/> class.
        /// </summary>
        public RasPhoneBookDialog()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the user creates a new entry or copies an existing entry.
        /// </summary>
        [SRCategory("CatBehavior")]
        [SRDescription("RPBDAddedEntryDesc")]
        public event EventHandler<EventArgs> AddedEntry;
        
        /// <summary>
        /// Occurs when the user changes an existing phone book entry.
        /// </summary>
        [SRCategory("CatBehavior")]
        [SRDescription("RPBDChangedEntryDesc")]
        public event EventHandler<EventArgs> ChangedEntry;

        /// <summary>
        /// Occurs when the user successfully dials an entry.
        /// </summary>
        [SRCategory("CatBehavior")]
        [SRDescription("RPBDDialedEntryDesc")]
        public event EventHandler<EventArgs> DialedEntry;

        /// <summary>
        /// Occurs when the user removes a phone book entry.
        /// </summary>
        [SRCategory("CatBehavior")]
        [SRDescription("RPBDRemovedEntryDesc")]
        public event EventHandler<EventArgs> RemovedEntry;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the full path (including file name) to the phone book.
        /// </summary>
        [DefaultValue(null)]
        [SRCategory("CatData")]
        [SRDescription("REDPhoneBookPathDesc")]
        public string PhoneBookPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the entry to initially highlight.
        /// </summary>
        [DefaultValue(null)]
        [SRCategory("CatData")]
        [SRDescription("RPBDEntryNameDesc")]
        public string EntryName
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets all <see cref="RasPhoneBookDialog"/> properties to their default values.
        /// </summary>
        public override void Reset()
        {
            this.PhoneBookPath = null;
            this.EntryName = null;

            base.Reset();
        }

        /// <summary>
        /// Overridden. Displays the modal dialog.
        /// </summary>
        /// <param name="hwndOwner">The handle of the window that owns the dialog box.</param>
        /// <returns><b>true</b> if the user completed the entry successfully, otherwise <b>false</b>.</returns>
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            NativeMethods.RASPBDLG dlg = new NativeMethods.RASPBDLG();
            dlg.size = Marshal.SizeOf(typeof(NativeMethods.RASPBDLG));
            dlg.hwndOwner = hwndOwner;
            dlg.callback = this.rasPhonebookDlgCallback;
            dlg.callbackId = IntPtr.Zero;
            dlg.reserved = IntPtr.Zero;
            dlg.reserved2 = IntPtr.Zero;

            if (this.Location != Point.Empty)
            {
                dlg.left = this.Location.X;
                dlg.top = this.Location.Y;

                dlg.flags |= NativeMethods.RASPBDFLAG.PositionDlg;
            }

            bool retval = false;
            try
            {
                retval = UnsafeNativeMethods.Instance.PhonebookDlg(this.PhoneBookPath, this.EntryName, ref dlg);
                if (!retval && dlg.error != NativeMethods.SUCCESS)
                {
                    this.OnError(new RasErrorEventArgs(dlg.error, RasHelper.Instance.GetRasErrorString(dlg.error)));
                }
            }
            catch (EntryPointNotFoundException)
            {
                ThrowHelper.ThrowNotSupportedException(Resources.Exception_NotSupportedOnPlatform);
            }
            catch (SecurityException)
            {
                ThrowHelper.ThrowUnauthorizedAccessException(Resources.Exception_AccessDeniedBySecurity);
            }

            return retval;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MessagingToolkit.Core.Ras.RasPhoneBookDialog"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><b>true</b> to release both managed and unmanaged resources; <b>false</b> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.rasPhonebookDlgCallback = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            this.rasPhonebookDlgCallback = new NativeMethods.RasPBDlgFunc(this.RasPhonebookDlgCallback);
        }

        /// <summary>
        /// Raises the <see cref="AddedEntry"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnAddedEntry(EventArgs e)
        {
            if (this.AddedEntry != null)
            {
                this.AddedEntry(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DialedEntry"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnDialedEntry(EventArgs e)
        {
            if (this.DialedEntry != null)
            {
                this.DialedEntry(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ChangedEntry"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnChangedEntry(EventArgs e)
        {
            if (this.ChangedEntry != null)
            {
                this.ChangedEntry(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RemovedEntry"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnRemovedEntry(EventArgs e)
        {
            if (this.RemovedEntry != null)
            {
                this.RemovedEntry(this, e);
            }
        }

        /// <summary>
        /// Signaled by the remote access service of user activity while the dialog box is open.
        /// </summary>
        /// <param name="callbackId">An application defined value that was passed to the RasPhonebookDlg function.</param>
        /// <param name="eventType">The event that occurred.</param>
        /// <param name="message">A string whose value depends on the <paramref name="eventType"/> parameter.</param>
        /// <param name="data">Pointer to an additional buffer argument whose value depends on the <paramref name="eventType"/> parameter.</param>
        private void RasPhonebookDlgCallback(int callbackId, NativeMethods.RASPBDEVENT eventType, string message, IntPtr data)
        {
            switch (eventType)
            {
                case NativeMethods.RASPBDEVENT.AddEntry:
                    this.OnAddedEntry(EventArgs.Empty);
                    break;

                case NativeMethods.RASPBDEVENT.DialEntry:
                    this.OnDialedEntry(EventArgs.Empty);
                    break;

                case NativeMethods.RASPBDEVENT.EditEntry:
                    this.OnChangedEntry(EventArgs.Empty);
                    break;

                case NativeMethods.RASPBDEVENT.RemoveEntry:
                    this.OnRemovedEntry(EventArgs.Empty);
                    break;
            }
        }

        #endregion
    }
}
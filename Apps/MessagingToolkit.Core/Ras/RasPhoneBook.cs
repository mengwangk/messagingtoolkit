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
    using System.IO;
    using MessagingToolkit.Core.Ras.Design;
    using MessagingToolkit.Core.Ras.Internal;
    using MessagingToolkit.Core.Properties;

    /// <summary>
    /// Represents a remote access service (RAS) phone book. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There are multiple phone books in use by Windows at any given point in time and this class can only manage one phone book per instance. If you add an entry to the all user's profile phone book, attempting to manipulate it with the current user's profile phone book opened will result in failure. Entries will not be located, and changes made to the phone book will not be recognized by the instance.
    /// </para>
    /// <para>
    /// When attempting to open a phone book that does not yet exist, the <b>RasPhoneBook</b> class will automatically generate the file to be used. Also, setting the <see cref="EnableFileWatcher"/> property to <b>true</b> will allow the class to monitor for any external changes made to the file.
    /// </para>
    /// <para><b>Known Limitations</b>
    /// <list type="bullet">
    /// <item>For phone books which are not located in the all users profile directory (including those in custom locations) any stored credentials for the entries must be stored per user.</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows how to open a phone book in a custom location using a <b>RasPhoneBook</b> class.
    /// <code lang="C#">
    /// <![CDATA[
    /// using (RasPhoneBook pbk = new RasPhoneBook())
    /// {
    ///     pbk.Open("C:\\Test.pbk");
    /// }
    /// ]]>
    /// </code>
    /// <code lang="VB.NET">
    /// <![CDATA[
    /// Dim pbk As RasPhoneBook
    /// Try
    ///     pbk = New RasPhoneBook
    ///     pbk.Open("C:\Test.pbk")
    /// Finally
    ///     If (pbk IsNot Nothing) Then
    ///         pbk.Dispose()
    ///     End If
    /// End Try
    /// ]]>
    /// </code>
    /// </example>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(RasPhoneBook), "MessagingToolkit.Core.Ras.RasPhoneBook.bmp")]
    public sealed partial class RasPhoneBook : RasComponent
    {
        #region Fields

        /// <summary>
        /// Defines the partial path (including filename) for a default phonebook file.
        /// </summary>
        private const string PhoneBookFilePath = @"Microsoft\Network\Connections\Pbk\rasphone.pbk";

        /// <summary>
        /// Contains the collection of entries in the phone book.
        /// </summary>
        private RasEntryCollection entries;

        /// <summary>
        /// Indicates whether the internal watcher will be enabled.
        /// </summary>
        private bool enableFileWatcher;

        /// <summary>
        /// Contains the internal <see cref="FileSystemWatcher"/> used to monitor the phone book for changes.
        /// </summary>
        private FileSystemWatcher watcher;

        /// <summary>
        /// Indicates whether the phone book has already been opened.
        /// </summary>
        private bool opened;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> class.
        /// </summary>
        public RasPhoneBook()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> class.
        /// </summary>
        /// <param name="container">An <see cref="System.ComponentModel.IContainer"/> that will contain the component.</param>
        public RasPhoneBook(IContainer container)
            : base(container)
        {
            this.InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the phone book has changed.
        /// </summary>
        /// <remarks>This event may be raised multiple times depending on how the file was changed.</remarks>
        [SRCategory("CatBehavior")]
        [SRDescription("RPBChangedDesc")]
        public event EventHandler<EventArgs> Changed;

        /// <summary>
        /// Occurs when the phone book has been deleted.
        /// </summary>
        [SRCategory("CatBehavior")]
        [SRDescription("RPBDeletedDesc")]
        public event EventHandler<EventArgs> Deleted;

        /// <summary>
        /// Occurs when the phone book has been renamed.
        /// </summary>
        [SRCategory("CatBehavior")]
        [SRDescription("RPBRenamedDesc")]
        public event EventHandler<RenamedEventArgs> Renamed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the full path (including filename) of the phone book.
        /// </summary>
        [Browsable(false)]
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of phone book.
        /// </summary>
        [Browsable(false)]
        public RasPhoneBookType PhoneBookType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of entries within the phone book.
        /// </summary>
        [Browsable(false)]
        public RasEntryCollection Entries
        {
            get
            {
                if (this.entries == null)
                {
                    this.entries = new RasEntryCollection(this);
                }

                return this.entries;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the phone book file will be monitored for external changes.
        /// </summary>
        [DefaultValue(false)]
        [SRCategory("CatBehavior")]
        [SRDescription("RPBEnableFileWatcherDesc")]
        public bool EnableFileWatcher
        {
            get
            {
                return this.enableFileWatcher;
            }

            set
            {
                this.enableFileWatcher = value;

                if (this.opened)
                {
                    // The phone book has already been opened, update the setting on the watcher.
                    this.watcher.EnableRaisingEvents = value;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines the full path (including filename) of the phone book.
        /// </summary>
        /// <param name="phoneBookType">The type of phone book to locate.</param>
        /// <returns>The full path (including filename) of the phone book.</returns>
        /// <remarks><see cref="RasPhoneBookType.Custom"/> will always return a null reference (<b>Nothing</b> in Visual Basic).</remarks>
        public static string GetPhoneBookPath(RasPhoneBookType phoneBookType)
        {
            string retval = null;

            if (phoneBookType != RasPhoneBookType.Custom)
            {
                Environment.SpecialFolder folder = Environment.SpecialFolder.CommonApplicationData;
                if (phoneBookType == RasPhoneBookType.User)
                {
                    folder = Environment.SpecialFolder.ApplicationData;
                }

                retval = System.IO.Path.Combine(Environment.GetFolderPath(folder), PhoneBookFilePath);
            }

            return retval;
        }

        /// <summary>
        /// Opens the phone book.
        /// </summary>
        /// <remarks>This method opens the existing default phone book in the All Users profile, or creates a new phone book if the file does not already exist.</remarks>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public void Open()
        {
            this.Open(false);
        }

        /// <summary>
        /// Opens the phone book.
        /// </summary>
        /// <param name="openUserPhoneBook"><b>true</b> to open the phone book in the user's profile; otherwise, <b>false</b> to open the system phone book in the All Users profile.</param>
        /// <remarks>This method opens an existing phone book or creates a new phone book if the file does not already exist.</remarks>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public void Open(bool openUserPhoneBook)
        {
            RasPhoneBookType phoneBookType = RasPhoneBookType.AllUsers;
            if (openUserPhoneBook)
            {
                phoneBookType = RasPhoneBookType.User;
            }

            this.Open(RasPhoneBook.GetPhoneBookPath(phoneBookType));
            this.PhoneBookType = phoneBookType;
        }

        /// <summary>
        /// Opens the phone book.
        /// </summary>
        /// <param name="phoneBookPath">The path (including filename) of a phone book.</param>
        /// <remarks>This method opens an existing phone book or creates a new phone book if the file does not already exist.</remarks>
        /// <exception cref="System.ArgumentException"><paramref name="phoneBookPath"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        public void Open(string phoneBookPath)
        {
            if (string.IsNullOrEmpty(phoneBookPath))
            {
                ThrowHelper.ThrowArgumentException("phoneBookPath", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            FileInfo file = new FileInfo(phoneBookPath);
            if (string.IsNullOrEmpty(file.Name))
            {
                ThrowHelper.ThrowArgumentException("phoneBookPath", Resources.Argument_InvalidFileName);
            }

            this.Path = file.FullName;
            this.PhoneBookType = RasPhoneBookType.Custom;

            // Setup the watcher used to monitor the file for changes, and attempt to load the entries.
            this.SetupFileWatcher(file);
            this.Entries.Load();

            this.opened = true;
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        protected override void InitializeComponent()
        {
            this.watcher = new System.IO.FileSystemWatcher();
            this.watcher.BeginInit();

            this.watcher.Renamed += new System.IO.RenamedEventHandler(this.WatcherRenamed);
            this.watcher.Deleted += new System.IO.FileSystemEventHandler(this.WatcherDeleted);
            this.watcher.Changed += new System.IO.FileSystemEventHandler(this.WatcherChanged);

            this.watcher.EndInit();

            base.InitializeComponent();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><b>true</b> to release both managed and unmanaged resources; <b>false</b> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.watcher != null)
                {
                    this.watcher.Dispose();
                }

                this.Path = null;
                this.entries = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises the <see cref="RasPhoneBook.Changed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnChanged(EventArgs e)
        {
            this.RaiseEvent<EventArgs>(this.Changed, e);
        }

        /// <summary>
        /// Raises the <see cref="RasPhoneBook.Deleted"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnDeleted(EventArgs e)
        {
            this.RaiseEvent<EventArgs>(this.Deleted, e);
        }

        /// <summary>
        /// Raises the <see cref="RasPhoneBook.Renamed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs"/> containing event data.</param>
        private void OnRenamed(RenamedEventArgs e)
        {
            this.RaiseEvent<RenamedEventArgs>(this.Renamed, e);
        }

        /// <summary>
        /// Occurs when the internal watcher notices the file has changed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="FileSystemEventArgs"/> containing event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The design is ok, the exception is being raised in an event.")]
        private void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                this.Entries.Load();

                this.OnChanged(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                this.OnError(new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// Occurs when the internal watcher notices the file has been deleted.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="FileSystemEventArgs"/> containing event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The design is ok, the exception is being raised in an event.")]
        private void WatcherDeleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                this.Entries.Load();

                this.OnDeleted(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                this.OnError(new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// Occurs when the internal watcher notices the file has been renamed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An <see cref="RenamedEventArgs"/> containing event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The design is ok, the exception is being raised in an event.")]
        private void WatcherRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Renamed)
                {
                    this.Path = e.FullPath;

                    // Force the file watcher to disable temporarily while the file being monitored is updated.
                    this.watcher.EnableRaisingEvents = false;
                    this.watcher.Filter = e.Name;
                    this.watcher.EnableRaisingEvents = this.EnableFileWatcher;
                }

                this.OnRenamed(e);
            }
            catch (Exception ex)
            {
                this.OnError(new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// Setup the internal <see cref="FileSystemWatcher"/> used to monitor the phonebook for external changes.
        /// </summary>
        /// <param name="file">The full path (including filename) of the file.</param>
        private void SetupFileWatcher(FileInfo file)
        {
            if (!file.Exists)
            {
                // The file being opened does not exist, create the file so it can be monitored.
                Utilities.CreateFile(file);
            }

            this.watcher.Path = file.DirectoryName;
            this.watcher.Filter = file.Name;
            this.watcher.EnableRaisingEvents = this.EnableFileWatcher;
        }

        #endregion
    }
}
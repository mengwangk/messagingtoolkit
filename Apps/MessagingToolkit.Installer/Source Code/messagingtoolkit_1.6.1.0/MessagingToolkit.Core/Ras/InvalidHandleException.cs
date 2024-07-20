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
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// The exception that is thrown when an invalid connection handle is used.
    /// </summary>
    [Serializable]
    public class InvalidHandleException : Exception
    {
        #region Fields

        private RasHandle _handle;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.InvalidHandleException"/> class.
        /// </summary>
        public InvalidHandleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.InvalidHandleException"/> class.
        /// </summary>
        /// <param name="handle">The <see cref="DotRas.RasHandle"/> that caused the exception.</param>
        public InvalidHandleException(RasHandle handle)
            : this(handle, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.InvalidHandleException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public InvalidHandleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.InvalidHandleException"/> class.
        /// </summary>
        /// <param name="handle">The <see cref="DotRas.RasHandle"/> that caused the exception.</param>
        /// <param name="message">A message that describes the error.</param>
        public InvalidHandleException(RasHandle handle, string message)
            : this(handle, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.InvalidHandleException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or null reference (<b>Nothing</b> in Visual Basic) if no inner exception is specified.</param>
        public InvalidHandleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.InvalidHandleException"/> class.
        /// </summary>
        /// <param name="handle">The <see cref="DotRas.RasHandle"/> that caused the exception.</param>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or null reference (<b>Nothing</b> in Visual Basic) if no inner exception is specified.</param>
        public InvalidHandleException(RasHandle handle, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Handle = handle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.InvalidHandleException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected InvalidHandleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Handle = (RasHandle)info.GetValue("Handle", typeof(RasHandle));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="DotRas.RasHandle"/> that caused the exception.
        /// </summary>
        public RasHandle Handle
        {
            get { return this._handle; }
            private set { this._handle = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridden. Populates a <see cref="System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info != null)
            {
                info.AddValue("Handle", this.Handle, typeof(RasHandle));
            }

            base.GetObjectData(info, context);
        }

        #endregion
    }
}
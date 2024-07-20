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
    /// The exception that is thrown when a remote access service (RAS) error occurs while dialing a connection.
    /// </summary>
    [Serializable]
    public class RasDialException : RasException
    {
        #region Fields

        private int _extendedErrorCode;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasDialException"/> class.
        /// </summary>
        public RasDialException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasDialException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public RasDialException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasDialException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code that caused the exception.</param>
        /// <param name="extendedErrorCode">The extended error code (if any) that occurred.</param>
        public RasDialException(int errorCode, int extendedErrorCode)
            : base(errorCode)
        {
            this._extendedErrorCode = extendedErrorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasDialException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<b>Nothing</b> in Visual Basic) if no inner exception is specified.</param>
        public RasDialException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasDialException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected RasDialException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._extendedErrorCode = (int)info.GetValue("ExtendedErrorCode", typeof(int));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the extended error code (if any) that occurred.
        /// </summary>
        public int ExtendedErrorCode
        {
            get { return this._extendedErrorCode; }
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
                info.AddValue("ExtendedErrorCode", this._extendedErrorCode, typeof(int));
            }

            base.GetObjectData(info, context);
        }

        #endregion
    }
}
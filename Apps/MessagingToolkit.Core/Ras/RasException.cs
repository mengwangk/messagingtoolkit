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
    using MessagingToolkit.Core.Ras.Internal;

    /// <summary>
    /// The exception that is thrown when a remote access service (RAS) error occurs.
    /// </summary>
    [Serializable]
    public class RasException : Exception
    {
        #region Fields

        private int _errorCode;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasException"/> class.
        /// </summary>
        public RasException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public RasException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code that caused the exception.</param>
        public RasException(int errorCode)
            : base(RasHelper.Instance.GetRasErrorString(errorCode))
        {
            this._errorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<b>Nothing</b> in Visual Basic) if no inner exception is specified.</param>
        public RasException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected RasException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._errorCode = (int)info.GetValue("ErrorCode", typeof(int));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the error code that caused the exception.
        /// </summary>
        public int ErrorCode
        {
            get { return this._errorCode; }
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
                info.AddValue("ErrorCode", this._errorCode, typeof(int));
            }

            base.GetObjectData(info, context);
        }

        #endregion
    }
}
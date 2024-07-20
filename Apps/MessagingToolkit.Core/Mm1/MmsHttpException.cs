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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Mm1
{
    /// <summary>
    /// MMS exception.
    /// </summary>
    public class MmsHttpException : BaseException<MmsHttpException>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statusCode"></param>
        public MmsHttpException(int statusCode)
            : base()
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        public MmsHttpException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <param name="cause"></param>
        public MmsHttpException(int statusCode, string message, Exception cause)
            : base(message, cause)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Optional HTTP status code. 0 means ignore. Otherwise this should be a valid HTTP status code.
        /// </summary>
        public virtual int StatusCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new object of MmsException class.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual
        /// information about the source or destination.</param>
        protected MmsHttpException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

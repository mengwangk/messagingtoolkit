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

namespace MessagingToolkit.Wap.Wsp.Headers
{
    /// <summary> 
    /// This exception will be thrown if encoding/decoding of headers fails.
    /// </summary>  
    [Serializable]
    public class HeaderParseException : Exception
    {
        private Exception throwable;

        public HeaderParseException()
            : base()
        {
        }

        public HeaderParseException(string msg)
            : base(msg)
        {
        }

        public HeaderParseException(string msg, Exception t)
            : base(msg)
        {
            throwable = t;
        }

        virtual public Exception Throwable
        {
            get
            {
                return throwable;
            }

        }
        public override string Message
        {
            get
            {
                if ((throwable == null) || (throwable.Message == null))
                {
                    return base.Message;
                }
                return new StringBuilder(base.Message).Append(" ").Append(throwable.Message).ToString();
            }

        }
    }
}

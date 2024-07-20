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

namespace MessagingToolkit.MMS
{
    /// <summary>
    /// Thrown when an error occurs decoding a buffer representing a
    /// Multimedia Message
    /// </summary>
#if !NETFX_CORE
    [Serializable]
#else
    [System.Runtime.Serialization.DataContract]
#endif
	public class MultimediaMessageDecoderException:Exception
	{

        /// <summary>
        /// Initializes a new instance of the <see cref="MultimediaMessageDecoderException"/> class.
        /// </summary>
        /// <param name="errormsg">The error msg.</param>
		public MultimediaMessageDecoderException(string errorMsg):base(errorMsg)
		{
		}
	}
}
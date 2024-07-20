//===============================================================================
// Copyright � TWIT88.COM.  All rights reserved.
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

namespace MessagingToolkit.Barcode.Common.ReedSolomon
{
	
	/// <summary> 
    /// Thrown when an exception occurs during Reed-Solomon decoding, such as when
	/// there are too many errors to correct.
    /// </summary>
#if !SILVERLIGHT && !NETFX_CORE
	[Serializable]
#endif
    internal sealed class ReedSolomonException: Exception
	{
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ReedSolomonException()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReedSolomonException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
		public ReedSolomonException(string message):base(message)
		{
		}
	}
}
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
using System.Net;
using System.Collections;

using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Wsp;

namespace MessagingToolkit.Wap
{
	
	/// <summary>
    /// Use this class for executing a WSP GET request
    /// </summary>
	public class GetRequest:Request
	{
        /// <summary>
        /// Construct a new GET Request
        /// </summary>
        /// <param name="url">The URL.</param>
		public GetRequest(string url):base(url)
		{
		}
	}
}
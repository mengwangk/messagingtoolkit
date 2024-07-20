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

namespace MessagingToolkit.P2P
{
    /// <summary>
    /// </summary>
    internal class P2PGatewayFactory: BaseGatewayFactory<P2PGateway>,IGatewayFactory     
    {

        /// <summary>
        /// </summary>
        /// <param name="config"></param>
        public P2PGatewayFactory(P2PGatewayConfiguration config)
        {

        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IGateway Find()
        {
            return null;           
        }


        /// <summary>
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static P2PGatewayFactory NewInstance(P2PGatewayConfiguration config)
        {
            return new P2PGatewayFactory(config);
        }
       
    }
}

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
using System.IO.Ports;
using System.Threading;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Mobile.Feature;
using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Generic mobile gateway. If the mobile gateway is different, you can extend from this class.
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    internal class MobileGateway: BaseMobileGateway<MobileGateway>, IMobileGateway
    {     

        #region ================== Constructor ===========================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public MobileGateway(MobileGatewayConfiguration config):base(config)
        {                  

        }

        #endregion =======================================================================================

        #region =========== Protected Methods ============================================================




        /// <summary>
        /// Method called upon connection setup
        /// </summary>
        protected override void OnConnected()
        {
           
        }


        /// <summary>
        /// Method called upon connection disconnected 
        /// </summary>
        protected override void OnDisconnected()
        {
            
        }

        #endregion =======================================================================================

    }
}

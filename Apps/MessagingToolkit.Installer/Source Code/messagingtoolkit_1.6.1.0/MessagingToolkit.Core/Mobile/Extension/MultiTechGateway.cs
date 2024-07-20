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

using MessagingToolkit.Core.Mobile.Feature;

namespace MessagingToolkit.Core.Mobile.Extension
{
    /// <summary>
    /// Multi Tech gateway
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    internal class MultiTechGateway : BaseMobileGateway<MultiTechGateway>, IMobileGateway
    {     

        #region ================== Constructor ===========================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public MultiTechGateway(MobileGatewayConfiguration config) : base(config)
        {
            messageStorage = MessageStorage.Sim;
        }

        #endregion =======================================================================================

        #region =========== Protected Methods ============================================================

          /// <summary>
        /// Initialize up the gateway
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            ExecuteFeature feature = GetFeature<ExecuteFeature>(ExecuteFeature.NewInstance());
            IContext context;
         
            feature.Request = "AT+WIND=0";
            context = Execute(feature);
        }

       
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


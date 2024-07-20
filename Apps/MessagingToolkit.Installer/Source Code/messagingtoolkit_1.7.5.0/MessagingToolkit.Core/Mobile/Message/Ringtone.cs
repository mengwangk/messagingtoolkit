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

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// EMS ringtone
    /// </summary>
    public class Ringtone: Sms
    {
        #region =============================== Constants ====================================================================
        
        
        /// <summary>
        /// Source port for ringtone
        /// </summary>
        private const int RingtoneSourcePort = 0;

        /// <summary>
        /// Destination port for ringtone
        /// </summary>
        private const int RingtoneDestinationPort = 5505;

        #endregion ==================================================================================

        
        #region =============================== Private Variables ===================================



        #endregion ==================================================================================


        #region =============================== Public Properties ===================================


       


        #endregion ==================================================================================


        #region =============================== Constructor ========================================= 

        /// <summary>
        /// Constructor
        /// </summary>
		public Ringtone():base()
		{
            this.DataCodingScheme = MessageDataCodingScheme.EightBits;
            this.SourcePort = RingtoneSourcePort;
            this.DestinationPort = RingtoneDestinationPort;
           
		}


        #endregion ==================================================================================  


        
        #region =============================== Internal Methods =====================================




        #endregion ==================================================================================

        
        #region ============== Factory method   =====================================================


        /// <summary>
        /// Static factory to create the ringtone object
        /// </summary>
        /// <returns>A new instance of the SMS object</returns>
        public new static Ringtone NewInstance()
        {
            return new Ringtone();
         
        }

        #endregion ===================================================================================
    }
}

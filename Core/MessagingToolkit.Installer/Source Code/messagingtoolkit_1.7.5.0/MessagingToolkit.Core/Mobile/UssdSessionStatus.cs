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

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Representing the status of a GSM Unstructured Supplemental Service Data
    /// (USSD) session.
    /// </summary>
    public class UssdSessionStatus
    {   
        /// <summary>
        /// No further user action required (network initiated USSD-Notify, or no
        /// further information needed after mobile initiated operation)
        /// </summary>
        private static readonly UssdSessionStatus NoFurtherActionRequired = new UssdSessionStatus(0);

        /// <summary>
        /// Further user action required (network initiated USSD-Request, or further
        /// information needed after mobile initiated operation
        /// </summary>
        private static readonly UssdSessionStatus FurtherActionRequired = new UssdSessionStatus(1);
	  
        /// <summary>
        /// USSD terminated by network
        /// </summary>
        private static readonly UssdSessionStatus TerminatedByNetwork = new UssdSessionStatus(2);

        /// <summary>
        /// Other local client has responded
        /// </summary>
	    private static readonly UssdSessionStatus OtherClientResponsed = new UssdSessionStatus(3);
	    

        /// <summary>
        /// Operation not supported
        /// </summary>
        private static readonly UssdSessionStatus OperationNotSupported = new UssdSessionStatus(4);
	   

        /// <summary>
        /// Network time out
        /// </summary>
	    private static readonly UssdSessionStatus NetworkTimeout = new UssdSessionStatus(5);

        private int numeric;

         /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public static IEnumerable<UssdSessionStatus> Values
        {
            get
            {
                yield return NoFurtherActionRequired;
                yield return FurtherActionRequired;
                yield return TerminatedByNetwork;
                yield return OtherClientResponsed;
                yield return OperationNotSupported;
                yield return NetworkTimeout;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="UssdSessionStatus"/> class.
        /// </summary>
        /// <param name="aNumeric">A numeric.</param>
        UssdSessionStatus(int aNumeric)
        {
            numeric = aNumeric;
        }

        /// <summary>
        /// Gets the numeric.
        /// </summary>
        /// <returns></returns>
        public int Numeric
        {
            get
            {
                return this.numeric;
            }
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return base.ToString() + " (" + numeric + ")";
        }    

        /// <summary>
        /// Get enum by numeric value.
        /// </summary>
        /// <param name="aNumeric"></param>
        /// <returns></returns>
        public static UssdSessionStatus GetByNumeric(int aNumeric)
        {
            foreach (UssdSessionStatus status in UssdSessionStatus.Values)
	        {
		        if (aNumeric == status.Numeric) return status;
	        }
	        return null;
	    }
    }
}

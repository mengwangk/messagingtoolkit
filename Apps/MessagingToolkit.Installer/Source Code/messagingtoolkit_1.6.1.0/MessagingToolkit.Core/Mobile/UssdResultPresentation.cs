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
    public class UssdResultPresentation
    {
        /// <summary>
        /// Disable the result code presentation to the TE
        /// </summary>
        public static readonly UssdResultPresentation PresentationDisabled = new UssdResultPresentation(0);

        /// <summary>
        /// Enable the result code presentation to the TE
        /// </summary>
        public static readonly UssdResultPresentation PresentationEnabled = new UssdResultPresentation(1);

        /// <summary>
        /// Cancel session (not applicable to read command response)
        /// </summary>
        public static readonly UssdResultPresentation CancelSession = new UssdResultPresentation(2);

        private int numeric;

         /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public static IEnumerable<UssdResultPresentation> Values
        {
            get
            {
                yield return PresentationDisabled;
                yield return PresentationEnabled;
                yield return CancelSession;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="USSDResultPresentation"/> class.
        /// </summary>
        /// <param name="aNumeric">A numeric.</param>
        UssdResultPresentation(int aNumeric)
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
    }
}


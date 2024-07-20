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
    /// Provide the signal quality of the gateway
    /// </summary>
    public class SignalQuality
    {
        private int bitErrorRate;
        private int signalStrength;
        private int signalStrengthPercent;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signalStrength">Signal strength</param>
        /// <param name="bitErrorRate">Bit error rate</param>
        public SignalQuality(int signalStrength, int bitErrorRate)
        {
            this.signalStrength = signalStrength;
            this.bitErrorRate = bitErrorRate;
            this.signalStrengthPercent = Convert.ToInt32((this.signalStrength / 31.0) * 100);
        }

        /// <summary>
        /// Gets the bit error rate.
        /// </summary>
        /// <value>The bit error rate.</value>
        public int BitErrorRate
        {
            get
            {
                return this.bitErrorRate;
            }
        }

        /// <summary>
        /// Gets the signal strength.
        /// </summary>
        /// <value>The signal strength.</value>
        public int SignalStrength
        {
            get
            {
                return this.signalStrength;
            }
        }


        /// <summary>
        /// Gets the signal strength percent.
        /// </summary>
        /// <value>The signal strength percent.</value>
        public int SignalStrengthPercent
        {
            get
            {
                return this.signalStrengthPercent;
            }
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "BitErrorRate = ", BitErrorRate, "\r\n");
            str = String.Concat(str, "SignalStrength = ", SignalStrength, "\r\n");
            str = String.Concat(str, "SignalStrengthPercent = ", SignalStrengthPercent, "\r\n");
            return str;
        }
    }
}

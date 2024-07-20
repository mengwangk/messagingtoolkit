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
    /// Phone book size
    /// </summary>
    internal class PhoneBookSize
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="ub"></param>
        /// <param name="nLength"></param>
        /// <param name="tLength"></param>
        public PhoneBookSize(int lb, int ub, int nLength, int tLength)
        {
            this.LowerBound = lb;
            this.UpperBound = ub;
            this.MaximumNumberLength = nLength;
            this.MaximumTextLength = tLength;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int LowerBound
        {
            get;
            internal set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int UpperBound
        {
            get;
            internal set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int MaximumNumberLength
        {
            get;
            internal set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int MaximumTextLength
        {
            get;
            internal set;
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
            str = String.Concat(str, "LowerBound = ", LowerBound, "\r\n");
            str = String.Concat(str, "UpperBound = ", UpperBound, "\r\n");
            str = String.Concat(str, "MaximumNumberLength = ", MaximumNumberLength, "\r\n");
            str = String.Concat(str, "MaximumTextLength = ", MaximumTextLength, "\r\n");
            return str;
        }
    }
}

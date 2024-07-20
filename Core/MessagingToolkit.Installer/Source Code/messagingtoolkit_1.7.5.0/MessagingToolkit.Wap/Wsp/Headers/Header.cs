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

namespace MessagingToolkit.Wap.Wsp.Headers
{

    /// <summary>
    /// This class represents a WSP Header.
    /// </summary>   
    public class Header
    {
        private string name;
        private object value;

        virtual public string Name
        {
            get
            {
                return name;
            }

        }
        virtual public object Value
        {
            get
            {
                return value;
            }
        }

        public Header(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public Header(string name, int value)
        {
            this.name = name;
            this.value = value;
        }

        public Header(string name, long value)
        {
            this.name = name;
            this.value = value;
        }

        public override string ToString()
        {
            return new StringBuilder(name).Append(": ").Append(value).ToString();
        }
    }
}

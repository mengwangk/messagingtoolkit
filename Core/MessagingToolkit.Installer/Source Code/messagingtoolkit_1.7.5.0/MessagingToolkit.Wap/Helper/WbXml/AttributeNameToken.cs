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
using System.Reflection;
using System.IO;

namespace MessagingToolkit.Wap.Helper.WbXml
{
    /// <summary>
    /// Attribute name token
    /// </summary>    
	public class AttributeNameToken:Token
	{
        private string prefix;
              				
		public AttributeNameToken(string name, string prefix, byte value)
		{
			this.name = name;
			this.value = value;
			this.prefix = prefix;
		}
		
		public AttributeNameToken(string name, byte value)
		{
			this.prefix = string.Empty;
			this.name = name;
			this.value = value;
		}

        virtual public string Prefix
        {
            get
            {
                return prefix;
            }

            set
            {
                this.prefix = value;
            }
        }      
	}
}
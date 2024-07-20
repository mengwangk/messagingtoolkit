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
using System.Text;

namespace MessagingToolkit.MMS
{

    /// <summary>
    /// The MultimediaMessageAddress class represents a
    /// generic address for a sender or a receiver
    /// of a Multimedia Message.
    /// </summary>
	[Serializable]
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = false)]
	public class MultimediaMessageAddress : MultimediaMessageConstants
	{
        private string address;
        private byte type;


        /// <summary>
        /// Creates a new and empty multimedia address
        /// </summary>
        public MultimediaMessageAddress()
            : base()
        {
            address = null;
            type = 0;
        }


        /// <summary>
        /// Creates a MM address specifying the address and the type.
        /// param addr the string representing the address
        /// param type the type of the address. 
        /// 
        /// It can be:
        /// AddressTypeUnknown, 
        /// AddressTypePlmn, 
        /// AddressTypeIPv4,
        /// AddressTypeIPv6, 
        /// AddressTypeEmail
        /// </summary>
        /// <param name="addr">The addr.</param>
        /// <param name="type">The type.</param>
        public MultimediaMessageAddress(string addr, byte type)
            : base()
        {
            SetAddress(addr, type);
        }

        /// <summary> Creates a new MM address initialising it to the value passed as parameter. </summary>
        public MultimediaMessageAddress(MultimediaMessageAddress value)
            : base()
        {
            SetAddress(value.address, value.type);
        }

        /// <summary>
        /// Retrieves the MM address value in the full format.
        /// For example: +358990000066/TYPE=PLMN,
        /// joe@user.org, 1.2.3.4/TYPE=IPv4
        /// </summary>
        /// <value>The full address.</value>
		virtual public string FullAddress
		{
			get
			{
				switch (type)
				{
					
					case MultimediaMessageConstants.AddressTypePlmn:  return address + "/TYPE=PLMN";
					
					case MultimediaMessageConstants.AddressTypeIpv4:  return address + "/TYPE=IPv4";
					
					case MultimediaMessageConstants.AddressTypeIpv6:  return address + "/TYPE=IPv6";
					
					default:  return address;
					
				}
			}
			
		}
        /// <summary>
        /// Retrieves the MM address type.
        /// 
        /// It can be:
        /// AddressTypeUnknown, 
        /// AddressTypePlmn, 
        /// AddressTypeIPv4,
        /// AddressTypeIPv6, 
        /// AddressTypeEmail
        /// </summary>
        /// <value>Address type</value>
		virtual public byte Type
		{
			get
			{
				return type;
			}			
		}

        /// <summary>
        /// Sets MM address value specifying the address and the type.
        /// param addr the string representing the address
        /// param type the type of the address.   
        ///
        /// It can be:
        /// AddressTypeUnknown, 
        /// AddressTypePlmn, 
        /// AddressTypeIPv4,
        /// AddressTypeIPv6, 
        /// AddressTypeEmail
        /// </summary>
        /// <param name="addr">The addr.</param>
        /// <param name="type">The type.</param>
		public virtual void  SetAddress(string addr, byte type)
		{
			if (addr != null)
			{
				this.address = new StringBuilder(addr).ToString();
				this.type = type;
			}
		}
		
		/// <summary> Retrieves the MM address value. 
		/// 
		/// </summary>
		public virtual string GetAddress()
		{
			return address;
		}


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy;
            System.Reflection.PropertyInfo[] infos = this.GetType().GetProperties(flags);

            StringBuilder sb = new StringBuilder();

            string typeName = this.GetType().Name;
            sb.AppendLine(typeName);
            sb.AppendLine(string.Empty.PadRight(typeName.Length + 5, '='));

            foreach (var info in infos)
            {
                if (!info.PropertyType.Name.StartsWith("List"))
                {
                    object value = info.GetValue(this, null);
                    sb.AppendFormat("{0}: {1}{2}", info.Name, value != null ? value : string.Empty, Environment.NewLine);
                }                
            }
            return sb.ToString();
        }
	}
}
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
using System.Collections;

namespace MessagingToolkit.Pdu.WapPush
{
    /// <summary>
    /// WAP service indication PDU
    /// </summary>
	public class WapSiPdu:SmsSubmitPdu
	{
        // these are for the WSP header
        // content type
        // charset
        // etc.
        // these are for the <indication> tag
        public const int WapSignalNone = 0x05;
        public const int WapSignalLow = 0x06;
        public const int WapSignalMedium = 0x07;
        public const int WapSignalHigh = 0x08;
        public const int WapSignalDelete = 0x09;

        private int wapSignal = WapSignalMedium;

        private string indicationText;

        private string url;

        private DateTime createDate;

        private DateTime expireDate;

        private string siId;

        private string siClass;
		

		virtual public string SiId
		{
			get
			{
				return siId;
			}
			
			set
			{
				this.siId = value;
			}
			
		}
		virtual public string SiClass
		{
			get
			{
				return siClass;
			}
			
			set
			{
				this.siClass = value;
			}
			
		}
		virtual public string IndicationText
		{
			get
			{
				return indicationText;
			}
			
			set
			{
				this.indicationText = value;
			}
			
		}
		virtual public string Url
		{
			get
			{
				return url;
			}
			
			set
			{
				this.url = value;
			}
			
		}
		virtual public DateTime CreateDate
		{
			get
			{
				return createDate;
			}
			
			set
			{
				this.createDate = value;
			}
			
		}
		virtual public DateTime ExpireDate
		{
			get
			{
				return expireDate;
			}
			
			set
			{
				this.expireDate = value;
			}
			
		}
		virtual public int WapSignal
		{
			get
			{
				return wapSignal;
			}
			
			set
			{
				switch (value)
				{
					
					case WapSignalNone: 
					case WapSignalLow: 
					case WapSignalMedium: 
					case WapSignalHigh: 
					case WapSignalDelete: 
						wapSignal = value;
						break;
					
					default:
                        throw new Exception("Invalid wap signal value: " + value);
					
				}
			}
			
		}
		virtual public string WapSignalFromString
		{
			set
			{
				if (value == null)
				{
					wapSignal = WapSignalMedium;
					return ;
				}
				value = value.Trim();
				if (value.ToUpper().Equals("none".ToUpper()))
				{
					wapSignal = WapSignalNone;
				}
				else if (value.ToUpper().Equals("low".ToUpper()))
				{
					wapSignal = WapSignalLow;
				}
				else if ((value.ToUpper().Equals("medium".ToUpper())) || (value.Equals("")))
				{
					wapSignal = WapSignalMedium;
				}
				else if (value.ToUpper().Equals("high".ToUpper()))
				{
					wapSignal = WapSignalHigh;
				}
				else if (value.ToUpper().Equals("delete".ToUpper()))
				{
					wapSignal = WapSignalDelete;
				}
				else
				{
                    throw new Exception("Cannot determine WAP signal to use");
				}
			}
			
		}
		
		public WapSiPdu()
		{
			DataCodingScheme = PduUtils.DcsEncoding8Bit | PduUtils.DcsCodingGroupData;
		}
		
		public override byte[] GetDataBytes()
		{
			if (base.GetDataBytes() == null)
			{
				WapSiUserDataGenerator udGenerator = new WapSiUserDataGenerator();
				udGenerator.WapSiPdu = this;
				SetDataBytes(udGenerator.GenerateWapSiUDBytes());
			}
			return base.GetDataBytes();
		}
	}
}
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
    /// Wap Push utility
    /// </summary>
	internal class WapPushUtils
	{
        // =======================================
        // WSP CONSTANTS AND UTILITIES
        // =======================================
        
        // =======================================
        // WBXML CONSTANTS
        // =======================================
        public const int WbXmlVersion12 = 0x02;
        public const int WbXmlSi10PublicIdentifier = 0x05;
        public const int WbXMlSl10PublicIdentifier = 0x06;
        public const int WbXmlCharsetUtf8 = 0x6A;

        // =======================================
        // WBXML CONSTANTS
        // =======================================
        public const int WbXmlCloseTag = 0x01;
        public const int WbXMlOpaqueData = 0xC3;
        public const int WbXMlStringStart = 0x03;
        public const int WbXMlStringEnd = 0x00;

        // <sl> tag with content
        public const int WbXmlSlTagContentNoAttributes = 0x85;

        // <si> tag with content
        public const int WbXmlSitagContentNoAttributes = 0x45;

        // <indication> tag with content and attributes
        public const int WbXMlIndicationTagContentAndAttributes = 0xC6;

        // maps for protocol / domain bytes    
        private static List<string> WbXmlProtocols = new List<string>();
        private static Dictionary<string, int> WbXmlProtocolBytes = new Dictionary<string, int>();
        private static List<string> WbXmlDomains = new List<string>();
        private static Dictionary<string, int> WbXMlDomainBytes = new Dictionary<string, int>();

        // href protocol constants
        public const int WbXmlHrefUnknown = 0x0B;
        public const int WbXmlHrefHttp = 0x0C;
        public const int WbXMlHrefHttpWww = 0x0D;
        public const int WbXmlHrefHttps = 0x0E;
        public const int WbXmlHrefHttpsWww = 0x0F;

        // href domain constants
        public const int WbXMlDomainCom = 0x85;
        public const int WbXmlDomainEdu = 0x86;
        public const int WbXmlDomainNet = 0x87;
        public const int WbXmlDomainOrg = 0x88;

        // =======================================
        // <indication> attribute constants
        // =======================================   
        // created attribute
        public const int PushCreated = 0x0A;

        // expires attribute
        public const int PushExpires = 0x10;

        // si-id attribute
        public const int PushSiId = 0x11;

        // class attribute
        public const int PushClass = 0x12;

        // action attributes
        public const int PushSignalNone = 0x05;
        public const int PushSignalLow = 0x06;
        public const int PushSignalMedium = 0x07;
        public const int PushSignalHigh = 0x08;
        public const int PushSignalDelete = 0x09;

        /// <summary>
        /// Constructor
        /// </summary>
        public WapPushUtils()
        {
        }		
		
		public static List<String> GetProtocols() 
        {
            return WbXmlProtocols;
        }
		
		public static List <String> GetDomains() 
        {
            return WbXmlDomains;
        }
		
		public static int GetProtocolByteFor(string protocol)
		{
            int b;
            if (WbXmlProtocolBytes.TryGetValue(protocol, out b))
            {
                return b;
            }
            return 0;			
		}
		
		public static int GetDomainByteFor(string domain)
		{
            int b;
            if (WbXMlDomainBytes.TryGetValue(domain, out b))
            {
                return b;
            }
            return 0;
		}
		
		

        /// <summary>
        /// Static initializer
        /// </summary>
		static WapPushUtils()
		{
			{
                WbXmlProtocols.Add("http://www.");
                WbXmlProtocols.Add("http://");
                WbXmlProtocols.Add("https://www.");
                WbXmlProtocols.Add("https://");
                WbXmlProtocolBytes.Add("http://www.", WbXMlHrefHttpWww);
                WbXmlProtocolBytes.Add("http://", WbXmlHrefHttp);
                WbXmlProtocolBytes.Add("https://www.", WbXmlHrefHttpsWww);
				WbXmlProtocolBytes.Add("https://", WbXmlHrefHttps);
                WbXmlDomains.Add(".com/");
                WbXmlDomains.Add(".edu/");
                WbXmlDomains.Add(".net/");
				WbXmlDomains.Add(".org/");
				WbXMlDomainBytes.Add(".com/", WbXMlDomainCom);
				WbXMlDomainBytes.Add(".edu/", WbXmlDomainEdu);
				WbXMlDomainBytes.Add(".net/", WbXmlDomainNet);
				WbXMlDomainBytes.Add(".org/", WbXmlDomainOrg);
			}
		}
	}
}
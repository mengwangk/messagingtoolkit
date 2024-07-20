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
using System.Collections;
using System.IO;
using System.Xml;

using MessagingToolkit.Wap.Log;

namespace MessagingToolkit.Wap.Helper.WbXml
{

    /// <summary>
    /// This class represents the normative list of public identifiers issued, and maintained, by the WAP Forum.
    /// Public Identifiers values represent well-known document type public identifiers.
    /// The first 128 values are reserved for use in future WAP specifications.
    /// All values are in hexadecimal.
    /// </summary>
	public class PublicIdentifiers
	{
        private Hashtable publicIdentifiers;
        private Hashtable systemIdentifiers;
        private static PublicIdentifiers instance;

		public static PublicIdentifiers Instance
		{
			get
			{
				if (instance == null)
					instance = new PublicIdentifiers();
				return instance;
			}			
		}

		virtual public int DefaultPublicIdentifier
		{
			get
			{
				return 1;
			}
			
		}
	
		private PublicIdentifiers()
		{
			Initialize();
		}

        /// <summary>
        /// The public identifier table is initialized to the  well-known public identifiers issued by the WAP Forum.
        /// These are valid at the time the WBXML V1.3 specification was released.
        /// </summary>
		private void  Initialize()
		{
			publicIdentifiers = Hashtable.Synchronized(new Hashtable());
			publicIdentifiers["0"] = "STRING_TABLE";
			publicIdentifiers["1"] = "UNKNOWN";
			publicIdentifiers["2"] = "-//WAPFORUM//DTD WML 1.0//EN";
			// (WML 1.0)
			publicIdentifiers["3"] = "-//WAPFORUM//DTD WTA 1.0//EN";
			// (WTA Event 1.0)		 deprecated
			publicIdentifiers["4"] = "-//WAPFORUM//DTD WML 1.1//EN";
			//(WML 1.1)";		
			publicIdentifiers["5"] = "-//WAPFORUM//DTD SI 1.0//EN";
			// (Service Indication 1.0)
			publicIdentifiers["6"] = "-//WAPFORUM//DTD SL 1.0//EN";
			// (Service Loading 1.0)		
			publicIdentifiers["7"] = "-//WAPFORUM//DTD CO 1.0//EN";
			// (Cache Operation 1.0)
			publicIdentifiers["8"] = "-//WAPFORUM//DTD CHANNEL 1.1//EN";
			//(Channel 1.1)
			publicIdentifiers["9"] = "-//WAPFORUM//DTD WML 1.2//EN";
			// (WML 1.2)	
			publicIdentifiers["A"] = "-//WAPFORUM//DTD WML 1.3//EN"; //(WML 1.3)
			publicIdentifiers["B"] = "-//WAPFORUM//DTD PROV 1.0//EN";
			// (Provisioning 1.0)
			publicIdentifiers["C"] = "-//WAPFORUM//DTD WTA-WML 1.2//EN";
			// (WTA-WML 1.2)
			publicIdentifiers["D"] = "-//WAPFORUM//DTD CHANNEL 1.2//EN";
			// (Channel 1.2)
			publicIdentifiers["E"] = "-//OMA//DTD DRMREL 1.0//EN";
			// (DRM REL 1.0)		
			publicIdentifiers["1100"] = "-//PHONE.COM//DTD ALERT 1.0//EN";
			// Registered Values			
			publicIdentifiers["FD1"] = "-//SYNCML//DTD SyncML 1.0//EN";
			publicIdentifiers["FD2"] = "-//SYNCML//DTD DevInf 1.0//EN";
			publicIdentifiers["FD3"] = "-//SYNCML//DTD SyncML 1.1//EN";
			publicIdentifiers["FD4"] = "-//SYNCML//DTD DevInf 1.1//EN";
			// no id for metinf
			systemIdentifiers = Hashtable.Synchronized(new Hashtable());
			systemIdentifiers["STRING_TABLE"] = "";
			systemIdentifiers["UNKNOWN"] = "";
			systemIdentifiers["-//WAPFORUM//DTD WML 1.0//EN"] = ""; // (WML 1.0)
			systemIdentifiers["-//WAPFORUM//DTD WTA 1.0//EN"] = "";
			// (WTA Event 1.0)		 deprecated
			systemIdentifiers["-//WAPFORUM//DTD WML 1.1//EN"] = "http://www.wapforum.org/DTD/wml_1_1.dtd";
			//(WML 1.1)";		
			systemIdentifiers["-//WAPFORUM//DTD SI 1.0//EN"] = "http://www.wapforum.org/DTD/si.dtd";
			// (Service Indication 1.0)
			systemIdentifiers["-//WAPFORUM//DTD SL 1.0//EN"] = "http://www.wapforum.org/DTD/sl.dtd";
			// (Service Loading 1.0)		
			systemIdentifiers["-//WAPFORUM//DTD CO 1.0//EN"] = "";
			// (Cache Operation 1.0)
			systemIdentifiers["-//WAPFORUM//DTD CHANNEL 1.1//EN"] = "";
			//(Channel 1.1)
			systemIdentifiers["-//WAPFORUM//DTD WML 1.2//EN"] = "http://www.wapforum.org/DTD/wml12.dtd";
			// (WML 1.2)	
			systemIdentifiers["-//WAPFORUM//DTD WML 1.3//EN"] = "http://www.wapforum.org/DTD/wml13.dtd";
			//(WML 1.3)
			systemIdentifiers["-//WAPFORUM//DTD PROV 1.0//EN"] = "http://www.wapforum.org/DTD/prov.dtd";
			// (Provisioning 1.0)
			systemIdentifiers["-//WAPFORUM//DTD WTA-WML 1.2//EN"] = "http://www.wapforum.org/DTD/wta-wml12.dtd";
			// (WTA-WML 1.2)
			systemIdentifiers["-//WAPFORUM//DTD CHANNEL 1.2//EN"] = "http://www.wapforum.org/DTD/channel12.dtd";
			// (Channel 1.2)
			systemIdentifiers["-//OMA//DTD DRMREL 1.0//EN"] = "";
			// (DRM REL 1.0)		
			systemIdentifiers["-//PHONE.COM//DTD ALERT 1.0//EN"] = "";
			// Registered Values			
		}
		
		public virtual string getPublicIdentifier(int publicIdentifier)
		{
			string key = Convert.ToString(publicIdentifier, 16).ToUpper();
			string value = (string) publicIdentifiers[key];
			value = (value == null)?(string) publicIdentifiers["1"]:value;
			return value;
		}
		
		public virtual string GetSystemIdentifier(string publicIdentifier)
		{
			return (string) systemIdentifiers[publicIdentifier];
		}
		
		public virtual int GetPublicIdentifierValue(string publicId)
		{
            IEnumerator iter = publicIdentifiers.GetEnumerator();
			string hexValue = GetKeyFromValue(iter, publicId);
			byte tokenValue = (byte) Convert.ToInt32(hexValue, 16);
			return tokenValue;
		}
		
		public virtual string GetPublicIdentifierValueHex(string publicId)
		{
            IEnumerator iter = publicIdentifiers.GetEnumerator();
			string hexValue = GetKeyFromValue(iter, publicId);
			return hexValue;
		}
		private string GetKeyFromValue(IEnumerator iterator, string publicId)
		{
            while (iterator.MoveNext())
			{
                DictionaryEntry entry = (DictionaryEntry) iterator.Current;
				if (entry.Value.ToString().ToUpper().Equals(publicId.ToUpper()))
				{
                    return entry.Key.ToString();
				}
			}
			return "1";
		}
	}
}
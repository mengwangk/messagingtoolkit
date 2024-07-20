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
    /// This class represents the collection of character sets defined by IANA (www.iana.org)
    /// The official names for the character sets are stored against the MIBenum value which is a
    /// unique value for use in MIBs to identify coded character sets.
    /// </summary>
	public class IANACharSet
	{
		public static Hashtable charSet = Hashtable.Synchronized(new Hashtable());
		
		public static int GetMIBEnum(string encodingName)
		{
			return ((int) charSet[encodingName]);
		}

		public static string GetEncoding(int mibNumber)
		{
			IEnumerator enumeration = charSet.Keys.GetEnumerator();
			while (enumeration.MoveNext())
			{
				string encoding = (string) enumeration.Current;
				if (((int) charSet[encoding]) == mibNumber)
					return encoding;
			}
			return "UTF-8";
		}

		static IANACharSet()
		{			
			try
			{
                charSet["UTF-8"] = 106;
                charSet["UTF-16"] = 1015;
                charSet["UTF-32"] = 1017;

				XmlDocument tempDocument;
				tempDocument = new XmlDocument();
				tempDocument.Load("./IANACharacterSet.xml");
				XmlDocument tokenDoc = tempDocument;
				XmlNodeList charactersets = tokenDoc.GetElementsByTagName("character-set");
				for (int i = 0; i < charactersets.Count; i++)
				{
					XmlElement characterSet = (XmlElement) charactersets.Item(i);
					charSet[characterSet.GetAttribute("name")] = Int32.Parse(characterSet.GetAttribute("MIBenum"));
				}
			}
			catch (Exception exp)
			{
                Logger.LogThis(exp.Message, LogLevel.Warn);
                Logger.LogThis(exp.StackTrace, LogLevel.Warn);
			}
			
		}
	}
}
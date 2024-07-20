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
using Result = MessagingToolkit.Barcode.Result;
using URIParsedResult = MessagingToolkit.Barcode.Client.Results.URIParsedResult;
namespace MessagingToolkit.Barcode.Client.Results.Optional
{
	
	/// <summary> 
    /// Recognizes an NDEF message that encodes a URI according to the
	/// "URI Record Type Definition" specification.
	/// </summary>
	sealed class NDEFURIResultParser:AbstractNDEFResultParser
	{
        private static readonly string[] UriPrefixes = new string[]{null, "http://www.", "https://www.", "http://", "https://", "tel:", "mailto:", "ftp://anonymous:anonymous@", "ftp://ftp.", "ftps://", "sftp://", "smb://", "nfs://", "ftp://", "dav://", "news:", "telnet://", "imap:", "rtsp://", "urn:", "pop:", "sip:", "sips:", "tftp:", "btspp://", "btl2cap://", "btgoep://", "tcpobex://", "irdaobex://", "file://", "urn:epc:id:", "urn:epc:tag:", "urn:epc:pat:", "urn:epc:raw:", "urn:epc:", "urn:nfc:"};
		
		public static URIParsedResult Parse(Result result)
		{
			byte[] bytes = result.RawBytes;
			if (bytes == null)
			{
				return null;
			}
			NDEFRecord ndefRecord = NDEFRecord.ReadRecord(bytes, 0);
			if (ndefRecord == null || !ndefRecord.MessageBegin || !ndefRecord.MessageEnd)
			{
				return null;
			}
			if (!ndefRecord.Type.Equals(NDEFRecord.UriWellKnownType))
			{
				return null;
			}
			string fullURI = DecodeURIPayload(ndefRecord.Payload);
			return new URIParsedResult(fullURI, null);
		}
		
		internal static string DecodeURIPayload(byte[] payload)
		{
			int identifierCode = payload[0] & 0xFF;
			string prefix = null;
			if (identifierCode < UriPrefixes.Length)
			{
				prefix = UriPrefixes[identifierCode];
			}
			string restOfURI = BytesToString(payload, 1, payload.Length - 1, "UTF-8");
			return prefix == null?restOfURI:prefix + restOfURI;
		}
	}
}
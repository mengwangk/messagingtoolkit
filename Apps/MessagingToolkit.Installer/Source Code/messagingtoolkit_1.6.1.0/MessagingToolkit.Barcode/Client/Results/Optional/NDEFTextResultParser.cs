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
using TextParsedResult = MessagingToolkit.Barcode.Client.Results.TextParsedResult;

namespace MessagingToolkit.Barcode.Client.Results.Optional
{
	
	/// <summary> 
    /// Recognizes an NDEF message that encodes text according to the
	/// "Text Record Type Definition" specification.
	/// </summary>	
	sealed class NDEFTextResultParser:AbstractNDEFResultParser
	{
		
		public static TextParsedResult Parse(Result result)
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
			if (!ndefRecord.Type.Equals(NDEFRecord.TextWellKnownType))
			{
				return null;
			}
			string[] languageText = DecodeTextPayload(ndefRecord.Payload);
			return new TextParsedResult(languageText[0], languageText[1]);
		}
		
		internal static string[] DecodeTextPayload(byte[] payload)
		{
			byte statusByte = payload[0];
			bool isUTF16 = (statusByte & 0x80) != 0;
			int languageLength = statusByte & 0x1F;
			// language is always ASCII-encoded:
			string language = BytesToString(payload, 1, languageLength, "US-ASCII");
			string encoding = isUTF16?"UTF-16":"UTF-8";
			string text = BytesToString(payload, 1 + languageLength, payload.Length - languageLength - 1, encoding);
			return new string[]{language, text};
		}
	}
}
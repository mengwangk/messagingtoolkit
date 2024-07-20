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

namespace MessagingToolkit.Barcode.Client.Results.Optional
{
	
	/// <summary> 
    /// Recognizes an NDEF message that encodes information according to the
	/// "Smart Poster Record Type Definition" specification.
	/// 
	/// This actually only supports some parts of the Smart Poster format: title,
	/// URI, and action records. Icon records are not supported because the size
	/// of these records are infeasibly large for barcodes. Size and type records
	/// are not supported. Multiple titles are not supported.
    /// </summary>
	sealed class NDEFSmartPosterResultParser:AbstractNDEFResultParser
	{
		
		public static NDEFSmartPosterParsedResult Parse(Result result)
		{
			byte[] bytes = result.RawBytes;
			if (bytes == null)
			{
				return null;
			}
			NDEFRecord headerRecord = NDEFRecord.ReadRecord(bytes, 0);
			// Yes, header record starts and ends a message
			if (headerRecord == null || !headerRecord.MessageBegin || !headerRecord.MessageEnd)
			{
				return null;
			}
			if (!headerRecord.Type.Equals(NDEFRecord.SmartPosterWellKnownType))
			{
				return null;
			}
			
			int offset = 0;
			int recordNumber = 0;
			NDEFRecord ndefRecord = null;
			byte[] payload = headerRecord.Payload;
			int action = NDEFSmartPosterParsedResult.ActionUnspecified;
			string title = null;
			string uri = null;
			
			while (offset < payload.Length && (ndefRecord = NDEFRecord.ReadRecord(payload, offset)) != null)
			{
				if (recordNumber == 0 && !ndefRecord.MessageBegin)
				{
					return null;
				}
				
				string type = ndefRecord.Type;
				if (NDEFRecord.TextWellKnownType.Equals(type))
				{
					string[] languageText = NDEFTextResultParser.DecodeTextPayload(ndefRecord.Payload);
					title = languageText[1];
				}
				else if (NDEFRecord.UriWellKnownType.Equals(type))
				{
					uri = NDEFURIResultParser.DecodeURIPayload(ndefRecord.Payload);
				}
				else if (NDEFRecord.ActionWellKnownType.Equals(type))
				{
					action = ndefRecord.Payload[0];
				}
				recordNumber++;
				offset += ndefRecord.TotalRecordLength;
			}
			
			if (recordNumber == 0 || (ndefRecord != null && !ndefRecord.MessageEnd))
			{
				return null;
			}
			
			return new NDEFSmartPosterParsedResult(action, uri, title);
		}
	}
}
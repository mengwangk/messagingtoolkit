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

namespace MessagingToolkit.Barcode.Client.Results.Optional
{	
	/// <summary> 
    /// Represents a record in an NDEF message. This class only supports certain types
	/// of records -- namely, non-chunked records, where ID length is omitted, and only
	/// "short records".
  	/// </summary>
	sealed class NDEFRecord
	{
        private const int SupportedHeaderMask = 0x3F; // 0 0 1 1 1 111 (the bottom 6 bits matter)
        private const int SupportedHeader = 0x11; // 0 0 0 1 0 001

        public const string TextWellKnownType = "T";
        public const string UriWellKnownType = "U";
        public const string SmartPosterWellKnownType = "Sp";
        public const string ActionWellKnownType = "act";

        private int header;
        private string type;
        private byte[] payload;
        private int totalRecordLength;
		

		internal bool MessageBegin
		{
			get
			{
				return (header & 0x80) != 0;
			}
			
		}
		internal bool MessageEnd
		{
			get
			{
				return (header & 0x40) != 0;
			}
			
		}
		internal string Type
		{
			get
			{
				return type;
			}
			
		}
		internal byte[] Payload
		{
			get
			{
				return payload;
			}
			
		}
		internal int TotalRecordLength
		{
			get
			{
				return totalRecordLength;
			}
			
		}


        /// <summary>
        /// Initializes a new instance of the <see cref="NDEFRecord"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="type">The type.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="totalRecordLength">Total length of the record.</param>
		private NDEFRecord(int header, string type, byte[] payload, int totalRecordLength)
		{
			this.header = header;
			this.type = type;
			this.payload = payload;
			this.totalRecordLength = totalRecordLength;
		}

        /// <summary>
        /// Reads the record.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
		internal static NDEFRecord ReadRecord(byte[] bytes, int offset)
		{
			int header = bytes[offset] & 0xFF;
			// Does header match what we support in the bits we care about?
			// XOR figures out where we differ, and if any of those are in the mask, fail
			if (((header ^ SupportedHeader) & SupportedHeaderMask) != 0)
			{
				return null;
			}
			int typeLength = bytes[offset + 1] & 0xFF;
			
			int payloadLength = bytes[offset + 2] & 0xFF;
			
			string type = AbstractNDEFResultParser.BytesToString(bytes, offset + 3, typeLength, "US-ASCII");
			
			byte[] payload = new byte[payloadLength];
			Array.Copy(bytes, offset + 3 + typeLength, payload, 0, payloadLength);
			
			return new NDEFRecord(header, type, payload, 3 + typeLength + payloadLength);
		}
	}
}
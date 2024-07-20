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
using System.Net;
using System.IO;
using System.Collections;

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;
using MessagingToolkit.Wap.Wsp.Pdu;
using MessagingToolkit.Wap.Wsp.Headers;

namespace MessagingToolkit.Wap.Wsp.Multipart
{
    /// <summary>
    /// This class can be used for constructing WSP Multipart
    /// payload according to WAP-230-WSP.
    /// </summary>
	public class MultiPartData
	{
        private ArrayList parts;

		virtual public byte[] Bytes
		{
			get
			{
				MemoryStream outputStream = new MemoryStream();
				byte[] byteArray;
				byteArray = Encoding.UIntVar(parts.Count);
				outputStream.Write(byteArray, 0, byteArray.Length);
				
				for (IEnumerator e = parts.GetEnumerator(); e.MoveNext(); )
				{
					MultiPartEntry entry = (MultiPartEntry) e.Current;
					outputStream.Write(entry.Bytes, 0, entry.Bytes.Length);
				}
				
				outputStream.Flush();
				
				return outputStream.ToArray();
			}			
		}

        /// <summary>
        /// Construct a WSP Multipart object with no content (yet).
        /// </summary>
		public MultiPartData()
		{
			parts = ArrayList.Synchronized(new ArrayList(10));
		}

        /// <summary>
        /// Reconstruct a Multipart object by parsing the specified byte array. The
        /// headers will be decoded according to WAP version 1.1
        /// </summary>
        /// <param name="data">a byte array containing the Multipart payload.</param>
		public MultiPartData(byte[] data):this(data, 1, 1)
		{
		}

        /// <summary>
        /// Reconstruct a Multipart object by parsing the specified byte array.
        /// </summary>
        /// <param name="data">a byte array containing the Multipart payload</param>
        /// <param name="major">WAP major version (used for header decoding)</param>
        /// <param name="minor">WAP minor version (used for header decoding)</param>
		public MultiPartData(byte[] data, int major, int minor):this()
		{
			
			if (data == null)
			{
				return ;
			}
			
			WSPDecoder dc = new WSPDecoder(data);
			CodePage codePage = WAPCodePage.GetInstance(major, minor);
			
			long parts = dc.UIntVar;
			
			Logger.LogThis("# of parts: " + parts, LogLevel.Verbose);
			
			
			while (!dc.EOF)
			{
				long hlen = dc.UIntVar;
				long dlen = dc.UIntVar;
				
				Logger.LogThis("hlen: " + hlen + ", dlen: " + dlen, LogLevel.Verbose);
				
				
				int startCT = dc.Seek(0);
				
				// Decode the content-type
				string ctype = CWSPPDU.GetContentType(dc);
				
				// read in headers
				int ctLen = dc.Seek(0) - startCT;
				int hl = (int) (hlen - ctLen);
				
				Logger.LogThis("Content-Type length: " + ctLen + ", header length: " + hl, LogLevel.Verbose);

				
				int hstart = dc.Seek(0); dc.Seek(hl);
				// Copy the data
				byte[] edata = dc.GetBytes((int) dlen);
				MultiPartEntry entry = new MultiPartEntry(ctype, codePage, edata);
				AddPart(entry);
				
				// Decode the header
				if (hl > 0)
				{
					dc.Pos(hstart);
					CWSPHeaders hdrs = new CWSPHeaders(dc, hl, codePage);
					dc.Seek(edata.Length);
					for (IEnumerator e = hdrs.HeaderNames; e.MoveNext(); )
					{
						string key = (string) e.Current;
						
						for (IEnumerator e2 = hdrs.GetHeaders(key); e2.MoveNext(); )
						{
							string val = (string) e2.Current;
							entry.AddHeader(key, val);
						}
					}
				}
			}
		}

        /// <summary>
        /// Adds the part.
        /// </summary>
        /// <param name="entry">The entry.</param>
		public virtual void  AddPart(MultiPartEntry entry)
		{
			if (parts != null)
			{
				parts.Add(entry);
			}
		}

        /// <summary>
        /// Elementses this instance.
        /// </summary>
        /// <returns></returns>
		public virtual IEnumerator Elements()
		{
			return parts.GetEnumerator();
		}

        /// <summary>
        /// Get the number of Multipart entries
        /// </summary>
        /// <returns></returns>
		public virtual int Size()
		{
			return parts.Count;
		}
	
	}
}
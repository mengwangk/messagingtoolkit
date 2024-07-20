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
using System.Net;
using System.Collections;
using System.IO;

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;
using MessagingToolkit.Wap.Wsp.Headers;

namespace MessagingToolkit.Wap.Wsp.Pdu
{

    /// <summary>
    /// This class represents WSP Headers
    /// </summary>
	public class CWSPHeaders
	{
        // List of Headers
        private ArrayList headers = ArrayList.Synchronized(new ArrayList(10));

        // Default codepage (WAP)
        private CodePage codePage;

		virtual public IEnumerator HeaderNames
		{
			get
			{
				ArrayList v = ArrayList.Synchronized(new ArrayList(10));
				for (IEnumerator e = headers.GetEnumerator(); e.MoveNext(); )
				{
					Header he = (Header) e.Current;					
					if (!v.Contains(he.Name))
					{
						v.Add(he.Name);
					}
				}				
				return v.GetEnumerator();
			}			
		}

		virtual public byte[] Bytes
		{
			/*
			* Get the WSP representation of the headers
			*/
			
			get
			{
				MemoryStream outputStream = new MemoryStream();
				
				try
				{
					for (IEnumerator e = headers.GetEnumerator(); e.MoveNext(); )
					{
						Header he = (Header) e.Current;
						string hn = he.Name;
						object hv = he.Value;
						byte[] enc;
						
						if (hv == null)
						{
							Logger.LogThis(hn + ": Ignoring header since it has no value", LogLevel.Verbose);
						}
						else if (hv is string)
						{
							enc = codePage.Encode(hn, (string) hv);
							
							if (enc != null)
							{								
								outputStream.Write(enc, 0, enc.Length);
							}
							else
							{
								Logger.LogThis("codePage.encode() returned null for header " + he.ToString(), LogLevel.Warn);
							}
						}
						else if (hv is Int64)
						{						
                            DateTime tempAux = new DateTime((long) ((System.Int64) hv));
							enc = codePage.Encode(hn, ref tempAux);
							
							if (enc != null)
							{								
								outputStream.Write(enc, 0, enc.Length);
							}
							else
							{
								Logger.LogThis("codePage.encode() returned null for header " + he.ToString(), LogLevel.Warn);
							}
						}
						else if (hv is System.Int32)
						{
							enc = codePage.Encode(hn, ((System.Int32) hv));
							
							if (enc != null)
							{								
								outputStream.Write(enc, 0, enc.Length);
							}
							else
							{
								Logger.LogThis("codePage.encode() returned null for header " + he.ToString(), LogLevel.Warn);
							}
						}
						else
						{
							
								Logger.LogThis(hv.GetType().FullName + ": Unknown type of header value, using string encoding", LogLevel.Verbose);
							
                                enc = codePage.Encode(hn, hv.ToString());
							
							if (enc != null)
							{								
								outputStream.Write(enc, 0, enc.Length);
							}
							else
							{
								Logger.LogThis("codePage.encode() returned null for header " + he.ToString(), LogLevel.Warn);
							}
						}
					}
					
					outputStream.Flush();
				}
				catch (IOException unknown)
				{
					// We assume that this never occurs when writing to a ByteArrayOutputStream...
				}
				
				return outputStream.ToArray();
			}
			
		}

        /// <summary>
        /// Construct WSP Headers using the default WAP codepage
        /// </summary>
		public CWSPHeaders():this(WAPCodePage.GetInstance())
		{
		}

        /// <summary>
        /// Construct WSP Headers using a specific codepage
        /// </summary>
        /// <param name="codePage">The code page.</param>
		public CWSPHeaders(CodePage codePage)
		{
			this.codePage = codePage;
		}


        /// <summary>
        /// Construct WSP Headers by decoding a byte array
        /// </summary>
        /// <param name="decoder">the WSPDecoder for decoding</param>
        /// <param name="length">The length.</param>
        /// <param name="codePage">the codepage for decoding the headers</param>
        /// <throws>  HeaderParseException if decoding fails </throws>
		public CWSPHeaders(WSPDecoder decoder, int length, CodePage codePage):this(decoder, length, null, codePage)
		{
		}

        /// <summary>
        /// Construct WSP Headers by decoding a byte array.
        /// </summary>
        /// <param name="decoder">the WSPDecoder for decoding</param>
        /// <param name="length">The length.</param>
        /// <param name="stopOn">the header after which decoding stops (may be null)</param>
        /// <param name="codePage">the codepage for decoding the headers</param>
        /// <throws>  HeaderParseException if decoding fails </throws>
		public CWSPHeaders(WSPDecoder decoder, int length, string stopOn, CodePage codePage):this(codePage)
		{
			if (decoder == null)
			{
				return ;
			}
			
			if (decoder.RemainingOctets < length)
			{
				Logger.LogThis(length + ": Length exceeds remaining octets in decoder", LogLevel.Warn);
				length = decoder.RemainingOctets;
			}
			
			if (Logger.LogLevel == LogLevel.Verbose)
			{
				int pos = decoder.Seek(0);
				byte[] bytes = decoder.GetBytes(length);
				decoder.Pos(pos);
				//Logger.LogThis("\n" + DebugUtils.HexDump("data to decode: ", bytes), LogLevel.Verbose);
			}
			
			if (stopOn != null)
			{
				stopOn = stopOn.ToLower();
			}
			
			try
			{
				Header header;
				int offset = decoder.Seek(0);
				
				while (decoder.Seek(0) < (offset + length))
				{
					
					int start = decoder.Seek(0);
					int octet = decoder.UInt8;
					int hlen = 0;
					// logger.debug("1st octet: "+Integer.toHexString(octet));
					
					if (octet == 127)
					{
						// Shift-Delimiter
						// shift-delimiter + page identity
						int pageIdentity = decoder.UInt8;
						Logger.LogThis("Shift-delimiter detected, page identity: " + pageIdentity, LogLevel.Verbose);
						
						
						if (pageIdentity != 1)
						{
							throw new HeaderParseException("Code page switching not supported. (page identity: " + pageIdentity + ")");
						}
						// ignore code page switching to default code page
					}
					else if (octet > 0 && octet <= 31)
					{
						// Short-cut-shift-delimiter
						int shortCut = octet;
						
						Logger.LogThis("Short-cut-shift-delimiter detected, page identity: " + shortCut, LogLevel.Verbose);
						
						
						if (shortCut != 1)
						{
							throw new HeaderParseException("Code page switching not supported. (short-cut-shift-delimiter: " + shortCut + ")");
						}
						// ignore code page switching to default code page
					}
					else
					{
						// message header
						if ((octet & 0x80) == 0x80)
						{
							// short-integer
							//Logger.LogThis("Well-Known-header detected", LogLevel.Verbose);
							// well-known-field-name + wap-value
							int wellKnownHeader = octet & 0x7F;
							
							// header value
							hlen = GetHeaderValueSize(decoder);
							hlen++; // We want the first octet as-well
						}
						// well-known-field-name + wap-value
						else
						{
							Logger.LogThis("Application-header detected", LogLevel.Verbose);
							decoder.Seek(- 1);
							// application header -> header name as string and value as string
							// header name
							hlen = GetHeaderValueSize(decoder);
							decoder.Seek(hlen);
							
							// value
							hlen += GetHeaderValueSize(decoder);
						}
						decoder.Pos(start);
						byte[] encodedHeader = decoder.GetBytes(hlen);
						Logger.LogThis("\n" + DebugUtils.HexDump("Encoded header: ", encodedHeader), LogLevel.Verbose);
						
						
						try
						{
							header = codePage.Decode(encodedHeader);
							
							if (header != null)
							{
								Logger.LogThis("Decoded header: " + header, LogLevel.Verbose);
								
								headers.Add(header);
								if (stopOn != null && header.Name.ToLower().Equals(stopOn))
								{
									
									Logger.LogThis(stopOn + ": stop-on header reached, decode done", LogLevel.Verbose);
									
									break;
								}
							}
							else
							{
								Logger.LogThis("CodePage.decode() returned a null header: \n" + DebugUtils.HexDump(encodedHeader),  LogLevel.Warn);
							}
						}
						catch (Exception unknown)
						{
							// Since header parsing is not yet complete, warn and go-ahead
							Logger.LogThis("Exception while decoding header: " + unknown.Message, LogLevel.Warn);
							
							Logger.LogThis(DebugUtils.HexDump(encodedHeader), LogLevel.Verbose);
							
						}
					}
					// message header
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				// ignore and return
			}
		}



        /// <summary>
        /// Sets a WSP header with the given name and value. If the header had
        /// already been set, the new value overwrites the previous one. The containsHeader
        /// method can be used to test for the presence of a header before setting its value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
		public virtual void  SetHeader(string name, string value)
		{
			if (name == null)
			{
				return ;
			}
			
			RemoveHeader(name);
			
			if (value != null)
			{
				headers.Add(new Header(name, value));
			}
		}

        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
		public virtual void  AddHeader(string name, string value)
		{
			if ((name == null) || (value == null))
			{
				return ;
			}
			
			headers.Add(new Header(name, value));
		}

        /// <summary>
        /// Sets a WSP header with the given name and integer value. If the header
        /// had already been set, the new value overwrites the previous one. The
        /// containsHeader  method can be used to test for the presence of a header
        /// before setting its value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
		public virtual void  SetIntHeader(string name, int value)
		{
			if (name == null)
			{
				return ;
			}
			
			RemoveHeader(name);
			int tempAux = value;
			headers.Add(new Header(name, tempAux));
		}

        /// <summary>
        /// Adds the int header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
		public virtual void  AddIntHeader(string name, int value)
		{
			if (name == null)
			{
				return ;
			}
			
			int tempAux = value;
			headers.Add(new Header(name, tempAux));
		}

        /// <summary>
        /// Sets a WSP header with the given name and date-value. The date is specified
        /// in terms of milliseconds since the epoch. If the header had already been
        /// set, the new value overwrites the previous one. The containsHeader
        /// method can be used to test for the presence of a header before setting
        /// its value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
		public virtual void  SetDateHeader(string name, long value)
		{
			if (name == null)
			{
				return ;
			}
			
			RemoveHeader(name);
			long tempAux = value;
		    headers.Add(new Header(name, tempAux));
		}

        /// <summary>
        /// Adds the date header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
		public virtual void  AddDateHeader(string name, long value)
		{
			if (name == null)
			{
				return ;
			}
			
			long tempAux = value;
			headers.Add(new Header(name, tempAux));
		}

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual string GetHeader(string name)
		{
			if (name != null)
			{
				for (IEnumerator e = headers.GetEnumerator(); e.MoveNext(); )
				{
					Header he = (Header) e.Current;
					
					if (name.ToUpper().Equals(he.Name.ToUpper()))
					{
						object o = he.Value;
						return (o == null)?null:o.ToString();
					}
				}
			}
			return null;
		}

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual IEnumerator GetHeaders(string name)
		{
			ArrayList v = ArrayList.Synchronized(new ArrayList(10));
			
			if (name != null)
			{
				for (IEnumerator e = headers.GetEnumerator(); e.MoveNext(); )
				{
					Header he = (Header) e.Current;
					
					if (name.ToUpper().Equals(he.Name.ToUpper()))
					{
						object value_Renamed = he.Value;
						if (value_Renamed != null)
						{
							v.Add(he.Value.ToString());
						}
					}
				}
			}
			
			return v.GetEnumerator();
		}

        /// <summary>
        /// Determines whether the specified name contains header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if the specified name contains header; otherwise, <c>false</c>.
        /// </returns>
		public virtual bool ContainsHeader(string name)
		{
			if (name != null)
			{
				for (IEnumerator e = headers.GetEnumerator(); e.MoveNext(); )
				{
					Header he = (Header) e.Current;
					
					if (name.ToUpper().Equals(he.Name.ToUpper()))
					{
						return true;
					}
				}
			}
			return false;
		}

        /// <summary>
        /// Removes the header.
        /// </summary>
        /// <param name="name">The name.</param>
		private void  RemoveHeader(string name)
		{
			// Remove all occurences of the header first
			if (name != null)
			{
				for (IEnumerator e = headers.GetEnumerator(); e.MoveNext(); )
				{
					Header he = (Header) e.Current;
					
					if (name.ToUpper().Equals(he.Name.ToUpper()))
					{
						headers.Remove(he);
					}
				}
			}
		}

        /// <summary>
        /// Return the size of a header field value
        /// </summary>
        /// <param name="decoder">The decoder.</param>
        /// <returns></returns>
		public static int GetHeaderValueSize(WSPDecoder decoder)
		{
			int pos = decoder.Seek(0);
			int val = decoder.UInt8;
			int ret = 0;
			
			// logger.debug("First octet of field value: "+val);
			
			if (val < 31)
			{
				// logger.debug("Number of octets to follow: "+val);
				ret = val + 1;
			}
			else if (val == 31)
			{
				// an UIntVar follows...
				//logger.debug("UIntVar follows");
				ret = (int) decoder.UIntVar;
				ret += decoder.Seek(0) - pos;
			}
			else if (val < 128)
			{
				// logger.debug("Text-String, terminated by zero "); 
				val = 0;
				decoder.Seek(- 1);
				while (decoder.Octet != 0)
				{
					val++;
				}
				
				val++; //skip the trailing \0
				ret = val;
			}
			else
			{
				// Else -> Encoded 7-bit value; this header has no more data
				//logger.debug("7-bit value; this header has no more data"); 
				ret = 1;
			}
			
			decoder.Pos(pos); // rollback to initial decoder position
			return ret;
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			for (IEnumerator e = HeaderNames; e.MoveNext(); )
			{
				string key = (string) e.Current;
				
				for (IEnumerator e2 = GetHeaders(key); e2.MoveNext(); )
				{
					string val = (string) e2.Current;
					sb.Append("[").Append(key).Append(": ").Append(val).Append("]");
				}
			}			
			return sb.ToString();
		}
		
	}
}
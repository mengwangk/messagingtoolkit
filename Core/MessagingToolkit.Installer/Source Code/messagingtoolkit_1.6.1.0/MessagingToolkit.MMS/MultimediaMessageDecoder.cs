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
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace MessagingToolkit.MMS
{
	
	/// <summary> 
    /// The MultimediaMessageDecoder class decodes an array of bytes representing a Multimedia Message (MM)
	/// according with the specification.
	/// The class can be used to obtain a MultimediaMessage object from which you can access
	/// to each field and content of the message 
	/// </summary>
	public class MultimediaMessageDecoder : MultimediaMessageConstants
	{		
		internal const bool FLAG_DEBUG = false;
		private MultimediaMessage m_Message = null;
		private int m_i = 0;
		private bool m_bMultipartRelated = false;
		private bool m_bMessageAvailable = false;
		private bool m_bHeaderDecoded = false;
		private byte[] m_In;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultimediaMessageDecoder"/> class.
        /// </summary>
        /// <param name="buf">The buf.</param>
        public MultimediaMessageDecoder(byte[] buf)
        {
            SetMessage(buf);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultimediaMessageDecoder"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public MultimediaMessageDecoder(string fileName)
        {
            byte[] buf = ReadFile(fileName);  
            SetMessage(buf);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultimediaMessageDecoder"/> class.
        /// </summary>
        public MultimediaMessageDecoder()
        {
        }
		

		// ------------------------------------------------------------- BASIC RULES

		private long ReadMultipleByteInt(int length)
		{
			long value = 0L;
			int start = m_i;
			int end = m_i + length - 1;
			
			for (int ii = end, weight = 1; ii >= start; ii--, weight *= 256)
			{
				int bv = UnsignedByte(m_In[ii]);
				value = value + bv * weight;
			}
			
			m_i = end + 1;
			
			return value;
		}

        /// <summary>
        /// Reads the text string.
        /// </summary>
        /// <returns></returns>
		private string ReadTextString()
		{
			string value = string.Empty;
			
			if (m_In[m_i] == 0x22)
			{
				// in this case it's a "Quoted-string"
				m_i++;
			}
			
            List<byte> byteList = new List<byte>(10);
 
			while (m_In[m_i] > 0)			
            {
				//value = value + (char) m_In[m_i++];
                byteList.Add(m_In[m_i++]);
			}
            byte[] byteArray = byteList.ToArray();


            Encoding enc = Encoding.UTF8;   // MultimediaMessageHelper.DetectEncoding(byteArray);
            value = enc.GetString(byteArray);

            /*
            string val1 = Encoding.ASCII.GetString(byteArray);
            string val2 = Encoding.UTF8.GetString(byteArray);           
            value = val1;
            if (!val1.Equals(val2))
            {
                value = val2;
            }	
            */
			m_i++;			
			return value;
		}
		
		private int ReadUintVar()
		{
			int value = 0;
			int bv = UnsignedByte(m_In[m_i]);
			
			if (bv < 0x80)
			{
				value = bv;
				m_i++;
			}
			else
			{
				// In this case the format is "Variable Length Unsigned Integer"
				bool flag = true;
				short count = 0, inc = 0;
				
				// Count the number of byte needed for the number
				while (flag)
				{
					flag = (m_In[m_i + count] & 0x80) == 0x80;
					count++;
				}
				
				inc = count;
				count--;
				
				int weight = 1;
				while (count >= 0)
				{
					bv = DecodeByte(m_In[m_i + count]) * weight;
					weight *= 128;
					value = value + bv;
					count--;
				}
				
				m_i += inc;
			}
			
			return value;
		}
		
		private int ReadValueLength()
		{
			int length = 0;
			int temp = m_In[m_i++];
			
			if (temp < 31)
			{
				length = temp;
			}
			else if (temp == 31)
			{
				length = ReadUintVar();
			}
			
			return length;
		}
		
		
		private string ReadWellKnownMedia()
		{
			string value = "";
			switch (DecodeByte(m_In[m_i]))
			{
				
				
				case (0x00):  value = "*/*"; break;
				
				case (0x01):  value = "text/*"; break;
				
				case (0x02):  value = "text/html"; break;
				
				case (0x03):  value = "text/plain"; break;
				
				case (0x04):  value = "text/x-hdml"; break;
				
				case (0x05):  value = "text/x-ttml"; break;
				
				case (0x06):  value = "text/x-vCalendar"; break;
				
				case (0x07):  value = "text/x-vCard"; break;
				
				case (0x08):  value = "text/vnd.wap.wml"; break;
				
				case (0x09):  value = "text/vnd.wap.wmlscript"; break;
				
				case (0x0A):  value = "text/vnd.wap.channel"; break;
				
				case (0x0B):  value = "multipart/*"; break;
				
				case (0x0C):  value = "multipart/mixed"; break;
				
				case (0x0D):  value = "multipart/form-data"; break;
				
				case (0x0E):  value = "multipart/byteranges"; break;
				
				case (0x0F):  value = "multipart/alternative"; break;
				
				case (0x10):  value = "application/*"; break;
				
				case (0x11):  value = "application/java-vm"; break;
				
				case (0x12):  value = "application/x-www-form-urlencoded"; break;
				
				case (0x13):  value = "application/x-hdmlc"; break;
				
				case (0x14):  value = "application/vnd.wap.wmlc"; break;
				
				case (0x15):  value = "application/vnd.wap.wmlscriptc"; break;
				
				case (0x16):  value = "application/vnd.wap.channelc"; break;
				
				case (0x17):  value = "application/vnd.wap.uaprof"; break;
				
				case (0x18):  value = "application/vnd.wap.wtls-ca-certificate"; break;
				
				case (0x19):  value = "application/vnd.wap.wtls-user-certificate"; break;
				
				case (0x1A):  value = "application/x-x509-ca-cert"; break;
				
				case (0x1B):  value = "application/x-x509-user-cert"; break;
				
				case (0x1C):  value = "image/*"; break;
				
				case (0x1D):  value = "image/gif"; break;
				
				case (0x1E):  value = "image/jpeg"; break;
				
				case (0x1F):  value = "image/tiff"; break;
				
				case (0x20):  value = "image/png"; break;
				
				case (0x21):  value = "image/vnd.wap.wbmp"; break;
				
				case (0x22):  value = "application/vnd.wap.multipart.*"; break;
				
				case (0x23):  value = "application/vnd.wap.multipart.mixed"; break;
				
				case (0x24):  value = "application/vnd.wap.multipart.form-data"; break;
				
				case (0x25):  value = "application/vnd.wap.multipart.byteranges"; break;
				
				case (0x26):  value = "application/vnd.wap.multipart.alternative"; break;
				
				case (0x27):  value = "application/xml"; break;
				
				case (0x28):  value = "text/xml"; break;
				
				case (0x29):  value = "application/vnd.wap.wbxml"; break;
				
				case (0x2A):  value = "application/x-x968-cross-cert"; break;
				
				case (0x2B):  value = "application/x-x968-ca-cert"; break;
				
				case (0x2C):  value = "application/x-x968-user-cert"; break;
				
				case (0x2D):  value = "text/vnd.wap.si"; break;
				
				case (0x2E):  value = "application/vnd.wap.sic"; break;
				
				case (0x2F):  value = "text/vnd.wap.sl"; break;
				
				case (0x30):  value = "application/vnd.wap.slc"; break;
				
				case (0x31):  value = "text/vnd.wap.co"; break;
				
				case (0x32):  value = "application/vnd.wap.coc"; break;
				
				case (0x33):  value = "application/vnd.wap.multipart.related";
					          m_bMultipartRelated = true;
					          break;
				
				case (0x34):  value = "application/vnd.wap.sia"; break;
				
				case (0x35):  value = "text/vnd.wap.connectivity-xml"; break;
				
				case (0x36):  value = "application/vnd.wap.connectivity-wbxml"; break;
				}
			
			m_i++;
			
			return value;
		}
		
		
		// ------------------------------------------------------- MMS Header Encoding
		
		private string ReadContentTypeValue()
		{
			int bv = UnsignedByte(m_In[m_i]);
			string value = "";
			
			if (bv >= 0x80)
			{
				/* Constrained-media - Short Integer*/
				// Short-integer: the assigned number of the well-known encoding is
				// small enough to fit into Short-integer
				value = ReadWellKnownMedia();
			}
			/* Constrained-media - Extension-media*/
			else if (bv >= 0x20 && bv < 0x80)
			{
				value = ReadTextString();
			}
			/* Content-general-form */
			else if (bv < 0x20)
			{
				int valueLength = ReadValueLength();
				bv = UnsignedByte(m_In[m_i]);
				if (bv >= 0x80)
				{
					//Well-known-media
					int i2 = m_i;
					value = ReadWellKnownMedia();
					if (value.Equals("application/vnd.wap.multipart.related"))
					{
						bv = DecodeByte(m_In[m_i]);
						if (bv == MultimediaMessageConstants.WkpaType)
						{
							// Type of the multipart/related
							m_i++;
							m_Message.MultipartRelatedType = ReadTextString();
							bv = DecodeByte(m_In[m_i]);
							if (bv == MultimediaMessageConstants.WkpaStart)
							{
								// Start (it is the pointer to the presentation part)
								m_i++;
								m_Message.PresentationId = ReadTextString();
							}
						}
					}
					
					m_i = i2 + valueLength;
				}
				else
				{
					int i2 = m_i;
					value = ReadTextString();
					m_i = i2 + valueLength;
				}
			}
			return (value);
		}
		
		// ------------------------------------------------------------------ MMS Body
		private void  ReadMMBodyMultiPartRelated()
		{
			int n = 0;
			int c_headerLen = 0, c_dataLen = 0;
			string c_type = "", c_id = "";
			sbyte[] c_buf;
			int nEntries = m_In[m_i++];
			
			while (n < nEntries)
			{
				c_headerLen = ReadUintVar();
				c_dataLen = ReadUintVar();
				int freeze_i = m_i;
                int bodyIndex = m_i + c_headerLen;
				c_type = ReadContentTypeValue();
				int c_typeLen = m_i - freeze_i;
				
				c_id = "A" + n;
				if (c_headerLen - c_typeLen > 0)
				{
					if ((DecodeByte(m_In[m_i]) == MultimediaMessageConstants.HeaderFieldNameContentLocation) || (DecodeByte(m_In[m_i]) == MultimediaMessageConstants.HeaderFieldNameContentId))
					{
						m_i++;
						c_id = ReadTextString();
					}
				}

                m_i = bodyIndex;
				MultimediaMessageContent mmc = new MultimediaMessageContent();
				mmc.Type = c_type;
				mmc.ContentId = c_id;
				mmc.SetContent(m_In, m_i, c_dataLen);
				m_Message.AddContent(mmc);
				m_i += c_dataLen;
				
				n++;
			}
		}
		
		private void  ReadMMBodyMultiPartMixed()
		{
			int n = 0;
			int c_headerLen = 0, c_dataLen = 0;
			string c_type = "", c_id = "";
			int nEntries = m_In[m_i++];
			
			while (n < nEntries)
			{
				c_headerLen = ReadUintVar();
				c_dataLen = ReadUintVar();
                int bodyIndex = m_i + c_headerLen;
				c_type = ReadContentTypeValue();
				c_id = "A" + n;
				if (UnsignedByte(m_In[m_i]) == 0x8E)
				{
					m_i++;
					c_id = ReadTextString();
				}
				
				if (FLAG_DEBUG)
					Console.Out.WriteLine("c_type=(" + c_type + ") c_headerLen=(" + c_headerLen + ") c_dataLen=(" + c_dataLen + ") c_id=(" + c_id + ")");

                m_i = bodyIndex;
				MultimediaMessageContent mmc = new MultimediaMessageContent();
				mmc.Type = c_type;
				mmc.ContentId = c_id;
				mmc.SetContent(m_In, m_i, c_dataLen);
				m_Message.AddContent(mmc);
				m_i += c_dataLen;
				
				
				n++;
			}
		}
		
		// ---------------------------------------------------------------- MMS Header
		private void  ReadMMHeader()
		{
			bool flag_continue = true;
			
			while (flag_continue && m_i < m_In.Length)
			{
				byte currentByte = DecodeByte(m_In[m_i++]);
				
				switch (currentByte)
				{
					
					case MultimediaMessageConstants.FieldNameMessageType: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_MESSAGE_TYPE (0C)");
						m_Message.MessageType = m_In[m_i];
						m_i++;
						break;
					
					case MultimediaMessageConstants.FieldNameTransactionId: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_TRANSACTION_ID (18)");
						m_Message.TransactionId = ReadTextString();
						break;
					
					case MultimediaMessageConstants.FieldNameMessageId: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_MESSAGE_ID (0B)");
						m_Message.MessageId = ReadTextString();
						break;
					
					case MultimediaMessageConstants.FieldNameStatus: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_STATUS (15)");
						m_Message.MessageStatus = m_In[m_i];
						m_i++;
						break;
					
					case MultimediaMessageConstants.FieldNameMmsVersion: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_MMS_VERSION (0D)");
						m_Message.Version = m_In[m_i];
						m_i++;
						break;
					
					case MultimediaMessageConstants.FieldNameTo: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_TO (17)");
						m_Message.AddToAddress(new StringBuilder(ReadTextString()).ToString());
						break;
					
					case MultimediaMessageConstants.FieldNameCc: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_CC (02)");
						m_Message.AddCcAddress(new StringBuilder(ReadTextString()).ToString());
						break;
					
					case MultimediaMessageConstants.FieldNameBcc: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_BCC (01)");
						m_Message.AddBccAddress(new StringBuilder(ReadTextString()).ToString());
						break;
					
					
					case MultimediaMessageConstants.FieldNameDate: 
						{
							if (FLAG_DEBUG)
								Console.Out.WriteLine("FN_DATE (05)");
							int length = m_In[m_i++];
							long msecs = ReadMultipleByteInt(length) * 1000;
						 	m_Message.Date = new DateTime((msecs * TimeSpan.TicksPerMillisecond) + MultimediaMessageConstants.Ticks1970);
						}
						break;
					
					
					case MultimediaMessageConstants.FieldNameDeliveryReport: 
					{
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_DELIVERY_REPORT (06)");
						int value = UnsignedByte(m_In[m_i++]);
						if (value == 0x80)
							m_Message.DeliveryReport = true;
						else
							m_Message.DeliveryReport = false;
						break;
					}
					
					case MultimediaMessageConstants.FieldNameSenderVisibility: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_STATUS (14)");
						m_Message.SenderVisibility = m_In[m_i];
						m_i++;
						break;
					
					case MultimediaMessageConstants.FieldReadReply: 
					{
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_READ_REPLY (10)");
						int value = UnsignedByte(m_In[m_i++]);
						if (value == 0x80)
							m_Message.ReadReply = true;
						else
							m_Message.ReadReply = false;
						break;
					}
					
					
					case MultimediaMessageConstants.FieldNameFrom: 
						{
							if (FLAG_DEBUG)
								Console.Out.WriteLine("FN_FROM (09)");
							int valueLength = m_In[m_i++];
							int addressPresentToken = UnsignedByte(m_In[m_i++]);
							if (addressPresentToken == 0x80)
							{
								// Address-present-token
								m_Message.SetFrom(new StringBuilder(ReadTextString()).ToString());
							}
						}
						break;
					
					case MultimediaMessageConstants.FieldNameSubject: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_SUBJECT (16)");
						m_Message.Subject = ReadTextString();
						break;
					
					case MultimediaMessageConstants.FieldNameMessageClass: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_MESSAGE_CLASS (0A)");
						m_Message.MessageClass = m_In[m_i++];
						break;
					
					case MultimediaMessageConstants.FieldNameExpiry: 
						{
							if (FLAG_DEBUG)
								Console.Out.WriteLine("FN_EXPIRY (08)");
							int valueLength = ReadValueLength();
							int tokenType = UnsignedByte(m_In[m_i++]);
							long expiry = 0;
							
							if (tokenType == 128)
							{
								// Absolute-token
								int length = m_In[m_i++];
								expiry = ReadMultipleByteInt(length) * 1000;
								m_Message.ExpiryAbsolute = true;
							}
							
							if (tokenType == 129)
							{
								// Relative-token
								m_Message.ExpiryAbsolute = false;
								// Read the Delta-seconds-value
								if (valueLength > 3)
								{
									// Long Integer
									int length = m_In[m_i++];
									expiry = ReadMultipleByteInt(length) * 1000;
								}
								else
								{
									// Short Integhet (1 OCTECT)
									int b = m_In[m_i] & 0x7F;
									expiry = b * 1000;
									m_i++;
								}
							}

                            m_Message.Expiry = new DateTime((expiry * TimeSpan.TicksPerMillisecond) + MultimediaMessageConstants.Ticks1970);
						}
						break;
					
					case MultimediaMessageConstants.FieldNameDeliveryTime: 
						{
							if (FLAG_DEBUG)
								Console.Out.WriteLine("FN_DELIVERY_TIME (07)");
							int valueLength = ReadValueLength();
							int tokenType = UnsignedByte(m_In[m_i++]);
							long deliveryTime = 0;
							
							if (tokenType == 128)
							{
								// Absolute-token
								m_Message.DeliveryTimeAbsolute = true;
								int length = m_In[m_i++];
								deliveryTime = ReadMultipleByteInt(length) * 1000;
							}
							
							if (tokenType == 129)
							{
								// Relative-token
								m_Message.DeliveryTimeAbsolute = false;
								// Read the Delta-seconds-value
								if (valueLength > 3)
								{
									// Long Integer
									int length = m_In[m_i++];
									deliveryTime = ReadMultipleByteInt(length) * 1000;
								}
								else
								{
									// Short Integhet (1 OCTECT)
									int b = m_In[m_i] & 0x7F;
									deliveryTime = b * 1000;
									m_i++;
								}
							}
                            m_Message.DeliveryTime = new DateTime((deliveryTime * TimeSpan.TicksPerMillisecond) + MultimediaMessageConstants.Ticks1970);
						}
						break;
					
					case MultimediaMessageConstants.FieldNamePriority: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_PRIORITY (0F)");
						m_Message.Priority = m_In[m_i++];
						break;
					
					case MultimediaMessageConstants.FieldNameContentType: 
						if (FLAG_DEBUG)
							Console.Out.WriteLine("FN_CONTENT_TYPE (04)");
						m_Message.ContentType = ReadContentTypeValue();
						flag_continue = false;
						break;
					}
			}
		}

        /// <summary>
        /// Decodes the byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
		private byte DecodeByte(byte value)
		{
			return ((byte) (value & 0x7F));
		}

        /// <summary>
        /// Unsigneds the byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
		private int UnsignedByte(byte value)
		{
			if (value < 0)
			{
				return (value + 256);
			}
			else
				return value;
		}

        /// <summary>
        /// Resets the Decoder object.
        /// </summary>
		public virtual void  Reset()
		{
			m_Message = null;
			m_bMultipartRelated = false;
			m_bMessageAvailable = false;
			m_bHeaderDecoded = false;
			m_In = null;
		}

        /// <summary>
        /// Sets the buffer representing the Multimedia Message to be decoded.
        /// </summary>
        /// <param name="buf">The buf.</param>
		public virtual void  SetMessage(byte[] buf)
		{
			m_Message = new MultimediaMessage();
			m_In = buf;
			m_bMessageAvailable = true;
		}

        /// <summary>
        /// Sets the buffer representing the Multimedia Message to be decoded.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public virtual void SetMessage(string fileName)
        {
            m_Message = new MultimediaMessage();
            m_In = ReadFile(fileName);
            m_bMessageAvailable = true;
        }

        /// <summary>
        /// Decodes the header of the Multimedia Message. After calling this method
        /// a MultimediaMessage object, containing just the information related to the header and
        /// without the contents, can be obtained by calling getMessage().
        /// This method has to be called after setMessage(byte buf[).
        /// </summary>
		public virtual void DecodeHeader()
		{
			if (m_bMessageAvailable)
			{
				ReadMMHeader();
				m_bHeaderDecoded = true;
			}
			else
			{
				throw new MultimediaMessageDecoderException("Message unavailable. You must call setMessage(byte[] buf) before calling this method.");
			}
		}

        /// <summary>
        /// Decodes the body of the Multimedia Message. This method has to be called
        /// after the method decodeHeader()
        /// </summary>
		public virtual void  DecodeBody()
		{
			if (!m_bHeaderDecoded)
				throw new MultimediaMessageDecoderException("You must call DecodeHeader() before calling DecodeBody()");
			
			if (string.CompareOrdinal((m_Message.ContentType), "application/vnd.wap.multipart.related") == 0)
				ReadMMBodyMultiPartRelated();
			else
				ReadMMBodyMultiPartMixed();
		}

        /// <summary>
        /// Decodes the whole Multimedia Message. After calling this method
        /// a MultimediaMessage object, containing the information related to the header and
        /// the all contents, can be obtained by calling the method GetMessage().
        /// This method has to be called after SetMessage(byte buf[).
        /// </summary>
		public virtual void  DecodeMessage()
		{
			DecodeHeader();
			if (m_Message.MessageType == MultimediaMessageConstants.MessageTypeMSendReq || m_Message.MessageType == MultimediaMessageConstants.MessageTypeMReceived)
				DecodeBody();
		}


        /// <summary>
        /// Retrieves the MultimediaMessage object. This method has to be called after the calling
        /// of methods DecodeMessage() or DecodeHeader().
        /// </summary>
        /// <returns>
        /// An object representing the decoded Multimedia Message
        /// </returns>
		public virtual MultimediaMessage GetMessage()
		{
			if (m_bHeaderDecoded)
				return m_Message;
			else
				return null;
		}


        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Byte array content</returns>
        private byte[] ReadFile(string fileName)
        {
            int fileSize = 0;
            FileStream fileHandler = null;
            // Opens the file for reading.
            try
            {
                fileHandler = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                fileSize = (int)fileHandler.Length;
            }
            catch (IOException ioErr)
            {
                throw new MultimediaMessageEncoderException(ioErr.Message);
            }

            byte[] buf = new byte[fileSize];
            fileHandler.Read(buf, 0, fileSize);
            return buf;
        }
	}
}
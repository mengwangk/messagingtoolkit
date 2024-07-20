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
using System.Globalization;
using System.Collections;
using System.IO;

namespace MessagingToolkit.Core.Mobile.Message
{
	
	/// <summary>
	/// Encapsulates the Service Indication WAP Push instruction.
    /// 
    /// Full documentation can be found at 
    /// http://www.openmobilealliance.org/tech/affiliates/wap/wap-167-serviceind-20010731-a.pdf?doc=wap-167-serviceind-20010731-a.pdf
    /// </summary>
    [global::System.Serializable]
    internal class ServiceIndication
    {
        #region ==================== Public Constants ==============================

        /// <summary>
        /// ServiceIndication 1.0 Public Identifier	
        /// </summary>
		private static byte DocumentDtdServiceIndication = 0x05;		        	

        /// <summary>
        /// SI tag token
        /// </summary>
        private const byte TagTokenSi = 0x5;

        /// <summary>
        /// Indication tag token
        /// </summary>
        private const byte TagTokenIndication = 0x6;

        /// <summary>
        /// Info tag token
        /// </summary>
        private const byte TagTokenInfo = 0x7;

        /// <summary>
        /// Item tag token
        /// </summary>
        private const byte TagTokenItem = 0x8;

        /// <summary>
        /// No signal token
        /// </summary>
        private const byte AttributeStartTokenActionSignalNone = 0x5;

        /// <summary>
        /// Low signal token
        /// </summary>
        private const byte AttributeStartTokenActionSignalLow = 0x6;

        /// <summary>
        /// Medium signal token
        /// </summary>
        private const byte AttributeStartTokenActionSignalMedium = 0x7;

        /// <summary>
        /// High signal token
        /// </summary>
        private const byte AttributeStartTokenActionSignalHigh = 0x8;

        /// <summary>
        /// Delete signal token
        /// </summary>
        private const byte AttributeStartTokenActionSignalDelete = 0x9;

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeStartTokenCreated = 0xA;

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeStartTokenHref = 0xB;

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeStartTokenHrefHttp = 0xC;		// http://

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeStartTokenHrefHttpWww = 0xD;	    // http://www.

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeStartTokenHrefHttps = 0xE;		// https://

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeStartTokenHrefHttpsWww = 0xF;	// https://www.

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeStartTokenSiExpires = 0x10;

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeStartTokenSiId = 0x11;

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeStartTokenClass = 0x12;


        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeValueTokenCom = 0x85;			// .com/

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeValueTokenEdu = 0x86;			// .edu/

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeValueTokenNet = 0x87;			// .net/

        /// <summary>
        /// 
        /// </summary>
        private const byte AttributeValueTokenOrg = 0x88;			// .org/

        /// <summary>
        /// 
        /// </summary>
        private static Hashtable hrefStartTokens;
        
        /// <summary>
        /// 
        /// </summary>
		private static Hashtable attributeValueTokens;

        #endregion ==================================================================


        #region ==================== Public Properties ==============================

        /// <summary>
        /// </summary>
        /// <value></value>
        public string Href
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public DateTime CreatedAt
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public DateTime ExpiresAt
        {
            get;
            set;
        }

        /// <summary>
        /// Service indication action
        /// </summary>
        /// <value>Service indication action</value>
        public ServiceIndicationAction Action
        {
            get;
            set;
        }

        #endregion ===============================================================

        /// <summary>
        /// Private static initializer
        /// </summary>
		static ServiceIndication()
		{
			hrefStartTokens = new Hashtable();
			hrefStartTokens.Add("https://www.", AttributeStartTokenHrefHttpsWww);
			hrefStartTokens.Add("http://www.", AttributeStartTokenHrefHttpWww);
			hrefStartTokens.Add("https://", AttributeStartTokenHrefHttps);
			hrefStartTokens.Add("http://", AttributeStartTokenHrefHttp);

			attributeValueTokens = new Hashtable();
			attributeValueTokens.Add(".com/", AttributeValueTokenCom);
			attributeValueTokens.Add(".edu/", AttributeValueTokenEdu);
			attributeValueTokens.Add(".net/", AttributeValueTokenNet);
			attributeValueTokens.Add(".org/", AttributeValueTokenOrg);
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="href">Location url</param>
        /// <param name="text">Text</param>
        /// <param name="action">Signal action</param>
		public ServiceIndication(string href, string text, ServiceIndicationAction action)
		{
			this.Href = href;
			this.Text = text;
			this.Action = action;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="href"></param>
        /// <param name="text"></param>
        /// <param name="createdAt"></param>
        /// <param name="expiresAt"></param>
		public ServiceIndication(string href, string text, DateTime createdAt, DateTime expiresAt)
			: this(href, text, ServiceIndicationAction.NotSet)
		{
			this.CreatedAt = createdAt;
			this.ExpiresAt = expiresAt;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="href"></param>
        /// <param name="text"></param>
        /// <param name="createdAt"></param>
        /// <param name="expiresAt"></param>
        /// <param name="action"></param>
		public ServiceIndication(string href, string text, DateTime createdAt, DateTime expiresAt, ServiceIndicationAction action)
			: this (href, text, action)
		{
			this.CreatedAt = createdAt;
			this.ExpiresAt = expiresAt;
		}

		/// <summary>
		/// Generates a byte array comprising the encoded Service Indication
		/// </summary>
		/// <returns>The encoded body</returns>
		public byte[] GetWbXmlBytes()
		{
			MemoryStream stream = new MemoryStream();

			// wbxml headers
			stream.WriteByte(WbXml.Version11);
			stream.WriteByte(DocumentDtdServiceIndication);
			stream.WriteByte(WbXml.CharsetUtf8);
			stream.WriteByte(WbXml.Null);

			// start xml doc
			stream.WriteByte(WbXml.SetTagTokenIndications(TagTokenSi, false, true));
			stream.WriteByte(WbXml.SetTagTokenIndications(TagTokenIndication, true , true));

			// href attribute
			// this attribute has some well known start tokens that 
			// are contained within a static hashtable. Iterate through
			// the table and chose the token.
			int i = 0;
			byte hrefTagToken = AttributeStartTokenHref;
			foreach (string startString in hrefStartTokens.Keys)
			{
				if (this.Href.StartsWith(startString))
				{
					hrefTagToken = (byte)hrefStartTokens[startString];
					i = startString.Length;
					break;
				}
			}
			stream.WriteByte(hrefTagToken);

			WriteInlineString(stream, this.Href.Substring(i));

            /*	
             * Date elements removed as does not seem to be supported
             * by all handsets, or I'm doing it incorrectly, or it's a version 1.2
             * thing. 

			// created attribute
			stream.WriteByte(ATTRIBUTESTARTTOKEN_created);
			WriteDate(stream, this.CreatedAt);

			// si-expires attrbute
			stream.WriteByte(ATTRIBUTESTARTTOKEN_si_expires);
			WriteDate(stream, this.ExpiresAt);
            */
			
            // action attibute
			if (this.Action != ServiceIndicationAction.NotSet)
				stream.WriteByte(GetActionToken(this.Action));

			// close indication element attributes
			stream.WriteByte(WbXml.TagTokenEnd);
			
			// text of indication element
			WriteInlineString(stream, this.Text);

			// close indication element
			stream.WriteByte(WbXml.TagTokenEnd);
			// close si element
			stream.WriteByte(WbXml.TagTokenEnd);
			
			return stream.ToArray();
		}

		/// <summary>
		/// Gets the token for the action attribute
		/// </summary>
		/// <param name="action">Interruption level instruction to the handset</param>
		/// <returns>well known byte value for the action attribute</returns>
		protected byte GetActionToken(ServiceIndicationAction action)
		{
			switch (action)
			{
				case ServiceIndicationAction.Delete :
					return AttributeStartTokenActionSignalDelete;
				case ServiceIndicationAction.SignalHigh :
					return AttributeStartTokenActionSignalHigh;
				case ServiceIndicationAction.SignalLow :
					return AttributeStartTokenActionSignalLow;
				case ServiceIndicationAction.SignalMedium : 
					return AttributeStartTokenActionSignalMedium;
				default :
					return AttributeStartTokenActionSignalNone;
			}
		}

		/// <summary>
		/// Encodes an inline string into the stream using UTF8 encoding
		/// </summary>
		/// <param name="stream">The target stream</param>
		/// <param name="text">The text to write</param>
		protected void WriteInlineString(MemoryStream stream, string text)
		{
			// indicate that the follow bytes comprise a string
			stream.WriteByte(WbXml.TokenInlineStringFollows);

			// write character bytes
			byte[] buffer = Encoding.UTF8.GetBytes(text);
			stream.Write(buffer, 0, buffer.Length);

			// end is indicated by a null byte
			stream.WriteByte(WbXml.Null);
		}
		/// <summary>
		/// Encodes the DateTime to the stream.
		/// DateTimes are encoded as Opaque Data with each number in the string represented
		/// by its 4-bit binary value
		/// eg: 1999-04-30 06:40:00
		/// is encoded as 199904300640.
		/// Trailing zero values are not included.
		/// </summary>
		/// <param name="stream">Target stream</param>
		/// <param name="date">DateTime to encode</param>
		protected void WriteDate(MemoryStream stream, DateTime date)
		{
			byte[] buffer = new byte[7];
			
			buffer[0] = (byte)(date.Year / 100);
			buffer[1] = (byte)(date.Year % 100);
			buffer[2] = (byte)date.Month;
			buffer[3] = (byte)date.Day;

			int dateLength = 4;

			if (date.Hour > 0)
			{
				buffer[4] = (byte)date.Hour;
				dateLength = 5;
			}

			if (date.Minute > 0)
			{
				buffer[5] = (byte)date.Minute;
				dateLength = 6;
			}

			if (date.Second > 0)
			{
				buffer[6] = (byte)date.Second;
				dateLength = 7;
			}
			
			// write to stream
			stream.WriteByte(WbXml.TokenOpaqueDataFollows);
			stream.WriteByte((byte)dateLength);
			stream.Write(buffer, 0, dateLength);
		}
	}
}

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
using System.Net;

namespace MessagingToolkit.MMS
{
	
	/// <summary> 
    /// The MultimediaMessageSender class provides methods to send 
    /// Multimedia Messages to a predefined Multimedia Service Center (MMSC).
	/// </summary>
	public class MultimediaMessageSender
	{
        private string m_sUrl; // the URL of the MMSC
        private Hashtable hHeader;


        /// <summary>
        /// Initializes a new instance of the <see cref="MultimediaMessageSender"/> class.
        /// </summary>
        public MultimediaMessageSender()
        {
            hHeader = Hashtable.Synchronized(new Hashtable());
        }

        /// <summary>
        /// Gets the Multimedia Messaging Service Center address.
        /// </summary>
        /// <value>MMSC URL</value>
        /// <returns> the address
        /// </returns>
        /// <param name="value">the URL </param>
		virtual public string MMSCURL
		{
			get
			{
				return m_sUrl;
			}
			
			set
			{
				m_sUrl = value;
			}
			
		}


        /// <summary>
        /// Add a new Header and Value in the HTTP Header
        /// </summary>
        /// <param name="header">header name</param>
        /// <param name="value">The value.</param>
		public virtual void  AddHeader(string header, string value)
		{
			
			string str = (string) hHeader[header];
			if (str != null)
				str += ("," + value);
			else
				str = value;
			hHeader[header] = str;
		}


        /// <summary>
        /// Clear the HTTP Header
        /// </summary>
		public virtual void  ClearHeader()
		{
			hHeader.Clear();
		}

        /// <summary>
        /// Sends a Multimedia Message having a MMMessage object
        /// </summary>
        /// <param name="multimediaMsg">the Multimedia Message object</param>
		public virtual void Send(MultimediaMessage multimediaMsg)
		{
			
			MultimediaMessageEncoder encoder = new MultimediaMessageEncoder();
			encoder.SetMessage(multimediaMsg);
			try
			{
				encoder.EncodeMessage();
			}
			catch (MultimediaMessageEncoderException e)
			{
				throw new MultimediaMessageSenderException("An error occurred encoding the Multimedia Message for sending: " + e.Message);
			}
			
			byte[] buf = encoder.GetMessage();
			
			Send(buf);
		}

        /// <summary>
        /// Sends a Multimedia Message having an array of bytes representing the message.
        /// </summary>
        /// <param name="buf">the array of bytes representing the Multimedia Message.</param>
        /// <returns></returns>
		public virtual MultimediaMessageResponse Send(byte[] buf)
		{		
			
			MultimediaMessageResponse hResponse = Send(buf, 204);
			
			return hResponse;
		}

        /// <summary>
        /// Sends a Multimedia Message having an array of bytes representing the message
        /// and specifying the success code returned by the MMSC.
        /// </summary>
        /// <param name="buf">the array of bytes representing the Multimedia Message.</param>
        /// <param name="successcode">the success code returned by the MMSC. Generally it is 204.</param>
        /// <returns></returns>
		public virtual MultimediaMessageResponse Send(byte[] buf, int successcode)
		{

            Uri url = null;
            HttpWebRequest httpUrlConnection = null;
            Stream outputstream = null;
            //bool flag = false;
            //string s = "";
            MultimediaMessageResponse mmResponse = null;
            try
            {
                url = new Uri(m_sUrl);
            }
            catch (UriFormatException malformedUrlException)
            {
                throw new MultimediaMessageSenderException(malformedUrlException.Message);
            }
            MultimediaMessageDecoder mmDecoder = new MultimediaMessageDecoder();
            mmDecoder.SetMessage(buf);
            try
            {
                mmDecoder.DecodeHeader();
            }
            catch (MultimediaMessageDecoderException mmDecoderException)
            {
                throw new MultimediaMessageSenderException(mmDecoderException.Message);
            }
            MultimediaMessage mmMessage = mmDecoder.GetMessage();
            try
            {
                httpUrlConnection = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (IOException ioException)
            {
                throw new MultimediaMessageSenderException(ioException.Message);
            }
            //httpurlconnection.setDoOutput(true);
            string s2;
            string s3;

            for (IEnumerator enumeration = hHeader.Keys.GetEnumerator(); enumeration.MoveNext();
                httpUrlConnection.Headers.Set(s2, s3))
            {
                s2 = ((string)enumeration.Current);
                s3 = ((string)hHeader[s2]);
            }

            httpUrlConnection.Headers.Set("Content-Type", "application/vnd.wap.mms-message");
            httpUrlConnection.Headers.Set("Content-Length", Convert.ToString(buf.Length));

            try
            {
                try
                {
                    outputstream = httpUrlConnection.GetRequestStream();
                    outputstream.Write(buf, 0, buf.Length);
                    outputstream.Flush();
                }
                catch (Exception connectException)
                {
                    Console.Out.WriteLine(connectException.Message);
                }
                finally
                {
                    if (outputstream != null)
                    {
                        outputstream.Close();
                    }
                }

                HttpWebResponse response = (HttpWebResponse)httpUrlConnection.GetResponse();
                int j = (int)response.StatusCode;
                string s1 = response.StatusDescription;
                mmResponse = new MultimediaMessageResponse();
                mmResponse.SetResponseCode(j);
                mmResponse.SetResponseMessage(s1);
                int contentLength;
                try
                {
                    contentLength = Int32.Parse(httpUrlConnection.GetResponse().Headers.Get("Content-Length"));
                }
                catch (IOException e)
                {
                    contentLength = -1;
                }
                mmResponse.SetContentLength(contentLength);
                string contentType;
                try
                {
                    contentType = httpUrlConnection.GetResponse().Headers.Get("Content-Type");
                }
                catch (ArgumentNullException e)
                {
                    contentType = null;
                }
                mmResponse.SetContentType(contentType);
                int contentLength2;
                try
                {
                    contentLength2 = Int32.Parse(httpUrlConnection.GetResponse().Headers.Get("Content-Length"));
                }
                catch (IOException e)
                {
                    contentLength2 = -1;
                }
                if (contentLength2 >= 0)
                {
                    int contentLength3;
                    try
                    {
                        contentLength3 = Int32.Parse(httpUrlConnection.GetResponse().Headers.Get("Content-Length"));
                    }
                    catch (IOException e)
                    {
                        contentLength3 = -1;
                    }
                    byte[] abyte1 = new byte[contentLength3];
                    BinaryReader datainputstream = new BinaryReader(httpUrlConnection.GetResponse().GetResponseStream());
                    int l;
                    for (int i1 = 0; (l = datainputstream.Read()) != -1; i1++)
                    {
                        abyte1[i1] = (byte)l;
                    }

                    mmResponse.SetContent(abyte1);
                }
                else
                {
                    mmResponse.SetContent(null);
                }
                int k = 1;
                bool flag1 = true;
                while (flag1)
                {
                    string s4 = httpUrlConnection.GetResponse().Headers.Keys.Get(k);
                    if (s4 != null)
                    {
                        string s5 = httpUrlConnection.GetResponse().Headers.Get(k);
                        mmResponse.AddHeader(s4, s5);
                        k++;
                    }
                    else
                    {
                        flag1 = false;
                    }
                }
            }
            catch (IOException ioException)
            {
                Console.Error.Write(ioException.StackTrace);
                Console.Error.Flush();
                throw new MultimediaMessageSenderException(ioException.Message);
            }
            httpUrlConnection.GetResponse().Close();
            return mmResponse;
		}
	}
}
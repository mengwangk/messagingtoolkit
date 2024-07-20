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

using NUnit.Framework;


using MessagingToolkit.Wap.Helper;
using MessagingToolkit.MMS;

namespace MessagingToolkit.Wap.Test
{
    /// <summary>
    /// Test class for WAP client
    /// </summary>
    [TestFixture]
    public class WAPClientTest
    {
        private const string Path = @"c:\temp";
        /// <summary>
        /// Tests the WAP client.
        /// </summary>        
        public void TestSendMMS()
        {
           
            MultimediaMessage mms = new MultimediaMessage();

            // Set the headers
            SetHeaders(mms);

            // Add the contents
            AddContents(mms);

            // Encode the message
            MultimediaMessageEncoder encoder = new MultimediaMessageEncoder();
            encoder.SetMessage(mms);

            try {
                encoder.EncodeMessage();
                byte[] output = encoder.GetMessage();

                // Send the MMS
                SendMMS("10.128.1.242", 9201, "http://mms.celcom.net.my/", output);

                // Create the MMS file
                //CreateMmsFile(out, "Sample.mms");
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Tests the receive MMS.
        /// </summary>
        [Test]
        public void TestReceiveMMS()
        {
            try
            {
                string wapGateway = "10.128.1.242";
                int port = 9201;
                string url = "http://mms.celcom.net.my/?id=826578048B";

                WAPClient wapClient = new WAPClient(wapGateway, port);
                wapClient.LogLevel = MessagingToolkit.Wap.Log.LogLevel.Verbose;
                GetRequest request = new GetRequest(url);
                
                Console.WriteLine("Connecting to \"" + wapGateway + "\":" + port + "...");
                wapClient.Connect();

                Console.WriteLine("Receiving mms message through \"" + url + "\"...");
                Response response = wapClient.Execute(request);
                byte[] binaryMms = response.ResponseBody;
                MultimediaMessageDecoder decoder = new MultimediaMessageDecoder(binaryMms);
                decoder.DecodeMessage();
                MultimediaMessage message = decoder.GetMessage();
                
                wapClient.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private void SetHeaders(MultimediaMessage m)
        {
            // Just going to set the mandatories, and the subject...
            // Type,TransID,Version are all mandatory, and must be the first headers, in this order!

            m.MessageType = MultimediaMessageConstants.MessageTypeMSendReq;
            m.TransactionId = "0123456789";
            m.Version = MultimediaMessageConstants.MmsVersion10;
            m.SetFrom("0192292309/TYPE=PLMN");
            m.AddToAddress("0192292309/TYPE=PLMN");     //at least one To/CC/BCC is mandatory
            m.Subject = "My first test message!";        //subject is optional

            // ContentType is mandatory, and must be last header!  These last 3 lines set the ContentType to
            // application/vnd.wml.multipart.related;type="application/smil";start="<0000>"
            // In case of multipart.mixed, only the first line is needed (and change the constant)

            // Any string will do for the Content-ID, but it must match that used for the presentation part,
            // and it must be Content-ID, it cannot be Content-Location.

            m.ContentType = MultimediaMessageConstants.ContentTypeApplicationMultipartRelated;
            m.MultipartRelatedType = MultimediaMessageConstants.ContentTypeApplicationSmil; //presentation part is written in SMIL
            m.PresentationId = "<0000>"; // presentation part has Content-ID=<0000>

        }

        /// <summary>
        /// Adds the contents.
        /// </summary>
        /// <param name="message">The message.</param>
        private void AddContents(MultimediaMessage message)
        {            
            MultimediaMessageContent multimediaMessageContent = new MultimediaMessageContent();
            multimediaMessageContent.SetContent(@"c:\temp\test.txt");
            multimediaMessageContent.ContentId = "test.txt";  //If "<>" are not used with this method, the result is Content-Location
            multimediaMessageContent.Type = MultimediaMessageConstants.ContentTypeTextPlain;
            message.AddContent(multimediaMessageContent);


            MultimediaMessageContent image = new MultimediaMessageContent();
            image.SetContent(@"c:\temp\haiyi.jpg");
            image.ContentId = "haiyi.jpg";
            image.Type = MultimediaMessageConstants.ContentTypeImageJpeg;
            message.AddContent(image);
        }
        

        private void SendMMS(string wapGatewayHost, int wapGatewayPort, string servlet, byte[] encodedMms) 
        {

            WAPClient wapClient = new WAPClient(wapGatewayHost, wapGatewayPort);
            PostRequest request = new PostRequest(servlet);
            request.ContentType = "application/vnd.wap.mms-message";
            request.RequestBody = encodedMms;

            Console.WriteLine("Connecting to \"" + wapGatewayHost + "\":" + wapGatewayPort + "...");
            wapClient.Connect();

            Console.WriteLine("Sending mms message through \"" + servlet + "\"...");
            Response response = wapClient.Execute(request);
            byte[] binaryMms = response.ResponseBody;
            wapClient.Disconnect();
            try {
                MultimediaMessageDecoder dec = new MultimediaMessageDecoder(binaryMms);
                dec.DecodeMessage();
                MultimediaMessage message = dec.GetMessage();

                Console.Write("Status: " + response.Status);
                Console.WriteLine("Status Text: " + response.StatusText);

                if (response.Status == 200) {
                    Console.WriteLine("Message id: " + message.MessageId);
                    Console.WriteLine("Message sent!");
                } else {
                    Console.WriteLine("Message is not sent");
                }

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

        }
    }
}

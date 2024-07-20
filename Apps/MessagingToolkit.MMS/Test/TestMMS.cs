using System;
using System.IO;
using NUnit.Framework;
//using net.sourceforge.jmmslib;
//using PostRequest = net.sourceforge.jwap.PostRequest;
//using Response = net.sourceforge.jwap.Response;
//using WAPClient = net.sourceforge.jwap.WAPClient;

namespace MessagingToolkit.MMS
{
    [TestFixture]
    public class TestMMS
    {
        private String Path
        {
            get
            {
                return @"c:\temp\";
            }

        }

        [Test]
        public void CreateMMS()
        {
            MultimediaMessage mms = new MultimediaMessage();

            // 1)Set headers
            SetHeaders(mms);

            // 2)Add various content parts
            AddContents(mms);

            MultimediaMessageEncoder encoder = new MultimediaMessageEncoder();
            encoder.SetMessage(mms);
                       
            try
            {
                // 3)Encode the message
                encoder.EncodeMessage();
                
                byte[] output = encoder.GetMessage();

                //sendMMS("10.128.1.242", 9201, "http://mms.celcom.net.my", output);

                encoder.SaveToFile("c:\\temp\\sample4.mms");

                MultimediaMessageDecoder decoder = new MultimediaMessageDecoder(output);
                decoder.DecodeMessage();
                MultimediaMessage message = decoder.GetMessage();
                Console.Out.WriteLine(message.Subject);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
            }
        }

        /*

        /// <summary> Sends the encoded mms</summary>
        /// <throws>  IllegalStateException </throws>
        /// <throws>  IOException </throws>
        /// <throws>  net.sourceforge.jmmslib.MmsMessageException </throws>
        /// <throws>  net.sourceforge.jmmslib.MmsDecoderException </throws>
        private void sendMMS(string wapGatewayHost, int wapGatewayPort, string servlet, sbyte[] encodedMms)
        {
            WAPClient wapClient = new WAPClient(wapGatewayHost, wapGatewayPort);
            PostRequest request = new PostRequest(servlet);
            request.setContentType("application/vnd.wap.mms-message");
            request.setRequestBody(encodedMms);

            System.Console.Out.WriteLine("Connecting to \"" + wapGatewayHost + "\":" + wapGatewayPort + "...");
            wapClient.connect();

            System.Console.Out.WriteLine("Sending mms message through \"" + servlet + "\"...");
            Response response = wapClient.execute(request);
            sbyte[] binaryMms = response.getResponseBody();
            wapClient.disconnect();
            MmsDecoder dec = new MmsDecoder(binaryMms);

            MmsMessage mms = dec.decodeMessage();
            if (!mms.getResponseStatus().equals(MmsMessage.MMS_RESPONSE_STATUS_OK))
            {
                throw new MmsMessageException("Message not sent: error=" + mms.getResponseStatus() + "; description=" + mms.getResponseText());
            }
            System.Console.Out.WriteLine("Message sent!");
        }
        */

        private void SetHeaders(MultimediaMessage m)
        {
            // Just going to set the mandatories, and the subject...
            // Type,TransID,Version are all mandatory, and must be the first headers, in this order!
            m.MessageType = MultimediaMessageConstants.MessageTypeMSendReq;
            m.TransactionId = "0123456789";
            m.Version = MultimediaMessageConstants.MmsVersion10;

            m.SetFrom("0192292309/TYPE=PLMN");
            m.AddToAddress("0192292309/TYPE=PLMN"); //at least one To/CC/BCC is mandatory

            m.Subject = "My first test message!";   //subject is optional

            // ContentType is mandatory, and must be last header!  These last 3 lines set the ContentType to
            // application/vnd.wml.multipart.related;type="application/smil";start="<0000>"
            // In case of multipart.mixed, only the first line is needed (and change the constant)

            // Any string will do for the Content-ID, but it must match that used for the presentation part,
            // and it must be Content-ID, it cannot be Content-Location.

            m.ContentType = MultimediaMessageConstants.ContentTypeApplicationMultipartRelated;
            m.MultipartRelatedType = MultimediaMessageConstants.ContentTypeApplicationSmil; //presentation part is written in SMIL
            m.PresentationId = "<0000>"; // presentation part has Content-ID=<0000>
        }

        private void AddContents(MultimediaMessage m)
        {

            //This is where the majority of the work is done.  Note that here we are adding the parts of the
            //message in the order we want them to appear.  Actually the presentation part specifies that,
            //but in terminals which cannot understand the presentation part, the order may be significant,
            //so there seems to be no reason to use random order.

            //Note also, that the current version (1.1) of the library encodes the message in some sort
            //of random order, so developers must either fix that problem using the source code, or
            //be prepared for random order output.

            // Path where contents are stored assumed to be same as this class...
            string path = Path;

            // Add SMIL content
            /*
            MMContent smil_part = new MMContent();
            byte[] b1 = readFile(path + "HelloWorld.smil");
            smil_part.setContent(b1, 0, b1.length);
            smil_part.setContentId("<0000>"); //If "<>" are used with this method, the result is Content-ID
            smil_part.setType(IMMConstants.CT_APPLICATION_SMIL);
            m.addContent(smil_part);
            */

            // Add slide1 text
            MultimediaMessageContent s1Text = new MultimediaMessageContent();
            s1Text.SetContent("c:\\temp\\test.txt");
            s1Text.ContentId = "test.txt";     //If "<>" are not used with this method, the result is Content-Location
            s1Text.Type = MultimediaMessageConstants.ContentTypeTextPlain;
            m.AddContent(s1Text);

           
            MultimediaMessageContent s1Image = new MultimediaMessageContent();
            s1Image.SetContent("C:\\temp\\haiyi.jpg");
            s1Image.ContentId = "haiyi.jpg";
            s1Image.Type = MultimediaMessageConstants.ContentTypeImageJpeg;
            m.AddContent(s1Image);

            MultimediaMessageContent s2Image = new MultimediaMessageContent();
            s2Image.SetContent("C:\\temp\\autumn.jpg");
            s2Image.ContentId = "haiyi2.jpg";
            s2Image.Type = MultimediaMessageConstants.ContentTypeImageJpeg;
            m.AddContent(s2Image);

            // Add slide1 image
            /*
            MMContent s1_image = new MMContent();
            byte[] b3 = readFile(path + "SmileyFace.gif");
            s1_image.setContent(b3, 0, b3.length);
            s1_image.setContentId("SmileyFace.gif");
            s1_image.setType(IMMConstants.CT_IMAGE_GIF);
            m.addContent(s1_image);
            */
            // Add slide1 audio
            /*
            MMContent s1_audio = new MMContent();
            byte[] b4 = readFile(path + "HelloWorld.amr");
            s1_audio.setContent(b4, 0, b4.length);
            s1_audio.setContentId("HelloWorld.amr");
            s1_audio.setType("audio/amr"); //Note how to use mime-types with no pre-defined constant!
            m.addContent(s1_audio);
            */

            // Add slide2 text
            /*
            MMContent s2_text = new MMContent();
            byte[] b5 = readFile(path + "TheEnd.txt");
            s2_text.setContent(b5, 0, b5.length);
            s2_text.setContentId("<TheEnd.txt>");  //Here, again, we are using Content-ID - just for demonstration
            s2_text.setType(IMMConstants.CT_TEXT_PLAIN);
            m.addContent(s2_text);
            */
            // Add slide2 image
            /*
            MMContent s2_image = new MMContent();
            byte[] b6 = readFile(path + "TheEnd.gif");
            s2_image.setContent(b6, 0, b6.length);
            s2_image.setContentId("<TheEnd.gif>");
            s2_image.setType(IMMConstants.CT_IMAGE_GIF);
            m.addContent(s2_image);
            */
            // Add slide2 audio

            /*
            MMContent s2_audio = new MMContent();
            byte[] b7 = readFile(path + "YallComeBackNowYaHear.amr");
            s2_audio.setContent(b7, 0, b7.length);
            s2_audio.setContentId("<YCBNYH.amr>"); //Note that the filename and Content-ID don't need to be the same
            s2_audio.setType("audio/amr");
            m.addContent(s2_audio);
            */
        }      
    }
}
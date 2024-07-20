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
using System.Text.RegularExpressions;

using NUnit.Framework;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Helper;
using MessagingToolkit.MMS;

namespace MessagingToolkit.Core.Test
{
    /// <summary>
    /// MMS testing
    /// </summary>
    [TestFixture]
    public class MmsTest
    {
        /// <summary>
        /// Tests the MMS setup.
        /// </summary>      
        public void TestMMSSetup()
        {
            List<string> devices = GatewayHelper.GetAllDevices();

            foreach (string device in devices)
            {
                Console.WriteLine(device);
            }
        }
             
        public void TestMatchPdpContext()
        {
            string response = "+CGDCONT: 3,\"IP\",\"mms.celcom.net.my\",\"0.0.0.0\",0,0";
            string pattern = "\\+CGDCONT: (\\d+),\"(.*)\",\"(.*)\",\"(.*)\",(\\d+),(\\d+)";


            Regex regex = new Regex(pattern);
            for (Match match = regex.Match(response); match.Success; match = match.NextMatch())
            {
                int id = int.Parse(match.Groups[1].Value);
                string pdpType = match.Groups[2].Value;
                string apn = match.Groups[3].Value;
                string pdpAddress = match.Groups[4].Value;
                string dcControl = match.Groups[5].Value;
                string hcControl = match.Groups[6].Value;               
            }
        }

       
        public void TestGatewayHelper()
        {
            MobileGatewayConfiguration config = MobileGatewayConfiguration.NewInstance();
            config.DeviceName = "HUAWEI Mobile Connect - 3G Modem";

            if (GatewayHelper.GetDeviceConfiguration(config))
            {
                Console.WriteLine("ok");
            }
            else
            {
                Console.WriteLine("not ok");
            }
        }
               
        public void TestGetGatewayPort()
        {
            string url = "http://122.12";
            string[] temp = url.Split(new char[] { ':' }, StringSplitOptions.None);
            int gatewayPort = 9001;
            if (temp.Length > 1)
            {
                try
                {
                    gatewayPort = Convert.ToInt32(temp[temp.Length - 1]);
                }
                catch (Exception e)
                {
                    gatewayPort = 9001;
                }
            }
        }

        
       
        public void TestMMSTo()
        {
           string DefaultSmilLayout = @"<smil><head>
                                                    <meta name=""title"" content=""{0}"" />
                                                    <meta name=""author"" content=""MessagingToolkit"" />
                                                    <layout>
                                                    <root-layout width=""160"" height=""120""/>
                                                    <region id=""Image"" width=""100%""
                                                    height=""80"" left=""0"" top=""0"" />
                                                    <region id=""Text"" width=""100%""
                                                    height=""40"" left=""0"" top=""80"" />
                                                    </layout>
                                                    </head>
                                                    <body>
                                                    <par dur=""{2}"">
                                                    <img src=""FirstImage.jpg"" region=""Image"" />
                                                    <text src=""FirstText.txt"" region=""Text"" />
                                                    <audio src=""FirstSound.amr""/>
                                                    </par>
                                                    <par dur=""7s"">
                                                    <img src=""SecondImage.jpg"" region=""Image"" />
                                                    <text src=""SecondText.txt"" region=""Text"" />
                                                    <audio src=""SecondSound.amr"" />
                                                    </par>
                                                    </body>
                                                    </smil>";

           Console.WriteLine(DefaultSmilLayout);
        }


        /// <summary>
        /// Determines whether the specified text is Unicode.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the specified text is Unicode; otherwise, <c>false</c>.
        /// </returns>
        private bool IsUnicode(string text)
        {
            int i = 0;
            for (i = 1; i <= text.Length; i++)
            {
                int code = Convert.ToInt32(Convert.ToChar(text.Substring(i - 1, 1)));
                if (code < 0 || code > 255)
                {
                    return true;
                }
            }
            return false;
        }

              
        public void TestMmsSlide()
        {       

           
            Mms mms = Mms.NewInstance("testing", "1234567890");

            MmsSlide slide1 = MmsSlide.NewInstance();
            slide1.AddText("slide 1");
            slide1.AddAttachment("c:\\temp\\test.txt", AttachmentType.Text, ContentType.TextPlain);
            slide1.AddAttachment("c:\\temp\\haiyi.jpg", AttachmentType.Image, ContentType.ImageJpeg);

            mms.AddSlide(slide1);

            if (mms.Slides.Count > 0)
            {
                
                string smilContent = mms.CustomSmilLayout;
                string content = string.Empty;
                foreach (MmsSlide slide in mms.Slides)
                {                  
                    List<MmsAttachment> attachments = slide.Attachments;
                    if (attachments.Count > 0)
                    {
                        content += "<par dur=\"" + slide.Duration + "\">";     
                        foreach (MmsAttachment attachment in attachments) {

                            string contentId = attachment.Name;
                            MultimediaMessageContent multimediaMessageContent = new MultimediaMessageContent();
                            multimediaMessageContent.SetContent(attachment.Data, 0, attachment.Data.Length);
                            multimediaMessageContent.ContentId = contentId;
                            multimediaMessageContent.Type = StringEnum.GetStringValue(attachment.ContentType);
                            mms.AddContent(multimediaMessageContent);  
 
                            if (attachment.AttachmentType == AttachmentType.Image) 
                            {
                                content +=  "<img src=\"" + contentId + "\" region=\"Image\" />";
                            } 
                            else if (attachment.AttachmentType == AttachmentType.Text) 
                            {
                                content +=  "<text src=\"" + contentId + "\" region=\"Text\" />";
                            }
                            else if (attachment.AttachmentType == AttachmentType.Audio) 
                            {
                                content +=  "<audio src=\"" + contentId + "\"/>";
                            }                           
                        }
                        content += "</par>";                       
                    }                    
                }

                string smilPresentationId = Convert.ToString(GatewayHelper.GenerateRandomId());
                smilContent = string.Format(smilContent, mms.Subject, content);

                // Add the SMIL content
                MultimediaMessageContent multimediaSmilContent = new MultimediaMessageContent();
                byte[] smilBytes = Encoding.ASCII.GetBytes(smilContent);
                multimediaSmilContent.SetContent(smilBytes, 0, smilBytes.Length);
                multimediaSmilContent.ContentId = smilPresentationId;
                multimediaSmilContent.Type = Mms.ContentTypeApplicationSmil;
                mms.AddContent(multimediaSmilContent);  
                mms.PresentationId = smilPresentationId;
                mms.TransactionId = smilPresentationId;
            }

            /*
            string path = "test.txt";
            string fileName = System.IO.Path.GetFileName(path);
            Console.WriteLine(fileName);
            */
        }

      
        public void TestGetMMSNotificationUrl()
        {
            string url = string.Empty;
            string content = "\"application/vnd.wap.mms-message\0¯817707733@mmsc4\0+60196178656/TYPE=PLMN\0#³ôhttp://mms.celcom.net.my/?id=817707733D\0";

            string[] values = content.Split(new string[] {"http"}, StringSplitOptions.None);
            if (values.Length > 0)
            {
                string temp = values[values.Length - 1];
                string[] temp1 = temp.Split(new char[] { '\0' });
                if (temp1.Length > 0)
                    url = "http" + temp1[0];
            }
        }

        [Test]
        public void TestGetSpecialFolder()
        {
            string saveFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Console.WriteLine(saveFolder + System.IO.Path.DirectorySeparatorChar + "Received MMS" + System.IO.Path.DirectorySeparatorChar);
        }
    }
}

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
using System.Collections.Specialized;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;

using MessagingToolkit.Core;
using MessagingToolkit.Core.Properties;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile.PduLibrary;

using NUnit.Framework;


namespace MessagingToolkit.Core.Test
{
    /// <summary>
    /// NUnit test for SMS
    /// </summary>
    [TestFixture]
    public class SmsTest
    {
        /// <summary>
        /// Data queue for incoming data
        /// </summary>
        private IncomingDataQueue incomingDataQueue;

        /// <summary>
        /// Set up the test
        /// </summary>
        [SetUp]
        protected void SetUp()
        {
            incomingDataQueue = new IncomingDataQueue();
        }

        /// <summary>
        /// SMS encoding test
        /// </summary>       
        public void TestDefaultEncoding()
        {
            Sms sms = Sms.NewInstance();
            sms.ServiceCenterNumber = "+0160000001";
            sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
            sms.DestinationAddress = "0192292309";
            sms.DataCodingScheme = MessageDataCodingScheme.DefaultAlphabet;
            //sms.MessageReference = 123;
            sms.ValidityPeriod = MessageValidPeriod.OneHour;
            sms.Content = "hello world ";

            // PDU Number:1	Length For AT:23
            // 06911006000010317B0A81102992329000000B0BE8329BFD06DDDF723619
            // 06911006000010317B0A81102992329000000B0CE8329BFD06DDDF723619
            //Console.WriteLine(sms.GetSmsPduCode());

        }

        /// <summary>
        /// SMS encoding test
        /// </summary>       
        public void testUcs2Encoding()
        {
            Sms sms = Sms.NewInstance();
            sms.ServiceCenterNumber = "+0160000001";
            sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
            sms.DestinationAddress = "0192292309";
            sms.DataCodingScheme = MessageDataCodingScheme.Ucs2;
            //sms.MessageReference = 123;
            sms.ValidityPeriod = MessageValidPeriod.OneHour;
            sms.Content = "hello world";

            // PDU Number:1	Length For AT:35
            // 06911006000010317B0A81102992329000080B1600680065006C006C006F00200077006F0072006C0064
            // 06911006000010317B0A81102992329000080B1600680065006C006C006F00200077006F0072006C0064
            //Console.WriteLine(sms.GetSmsPduCode());

        }

        /// <summary>
        /// Swap two bit
        /// </summary>
        /// <param name="twoBitStr"></param>
        /// <returns></returns>
        protected string Swap(ref string twoBitStr)
        {
            char[] c = twoBitStr.ToCharArray();
            char t = c[0];
            c[0] = c[1];
            c[1] = t;
            return (c[0].ToString() + c[1].ToString());
        }

        public void TestConversion()
        {
            try
            {
                byte r = Convert.ToByte("FF", 16);
                Console.WriteLine(r);
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());

            }
        }

        public void TestSwap()
        {
            string s = "hello";
            Console.WriteLine(Swap(ref s));
            Console.WriteLine(s);
        }


        public void TestGetMessageTypeIndicator()
        {
            int i = 10;
            string[] c = new string[Convert.ToInt32(i / 4)];
            int type = 1;
            MessageTypeIndicator t = (MessageTypeIndicator)Enum.Parse(typeof(MessageTypeIndicator), type.ToString());
            Console.WriteLine(t);
        }


        public void TestDecode()
        {
            PduFactory messageFactory = PduFactory.NewInstance();
            MessageInformation msgInfo = messageFactory.Decode("07910691930000F0440B910691222903F900089070031024212385060804A6FB030130003000300030005E735E3859795C315DF27ECF5F886CE8610F57304E86FF0C4F4651734E8E597976844E004E9B65E04E8B751F975E768482B18FB965B095FB716768378BA94EBA70E60020000D000A4E0D80DC70E6300253EF662FFF0C59794E5F4E0D60F38BA98FD94E2A75374EBA660E5929771F768462FF77404E00");
            Console.WriteLine(msgInfo.ToString());
        }

        public void ChangeRefValue(ref string value)
        {
            value = "changed value";
        }


        public void TestChangeValue()
        {
            string value = "original value";
            Console.WriteLine(value);
            ChangeRefValue(ref value);
            Console.WriteLine(value);
        }


        protected void CheckForEncoding(string content)
        {
            int i = 0;
            for (i = 1; i <= content.Length; i++)
            {
                int code = Convert.ToInt32(Convert.ToChar(content.Substring(i - 1, 1)));
                if (code < 0 || code > 255)
                {
                    Console.WriteLine("Unicode");
                    return;
                }
            }
            Console.WriteLine("ascii");
        }



        public void TestCheckEncoding()
        {
            string content = "爸爸，我要釣魚，我要打bowling";
            CheckForEncoding(content);
        }


        public void TestMessageFactory()
        {
            Sms sms = Sms.NewInstance();
            sms.ServiceCenterNumber = "+60193900000";
            sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
            sms.DestinationAddress = "0192292309";
            //sms.DataCodingScheme = MessageDataCodingScheme.DefaultAlphabet;
            //sms.MessageReference = 123;
            sms.ValidityPeriod = MessageValidPeriod.OneHour;
            //sms.Content = "hello 爸爸 world dafdsfdsfdsfdsfdsfsfdsfdsfdsfdsfdsfdsfdsfdsfdsfdsfdsf1222222222222222222222222222222222222222222222234324324324324kkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkkk";
            sms.Content = "hello world " + Convert.ToChar(26);

            // PDU Number:1	Length For AT:23
            // 06911006000010317B0A81102992329000000B0BE8329BFD06DDDF723619
            // 06911006000010317B0A81102992329000000B0CE8329BFD06DDDF723619
            PduFactory messageFactory = PduFactory.NewInstance();
            string[] pduCodes = messageFactory.Generate(sms);
            Console.WriteLine(pduCodes);
        }


        public void TestMessageFactoryDecode()
        {
            string content = "06911006000010717B0A81102992329000000BA006080400000301E8329BFD06DDDF723619440E9BC97333796E26CFCDE4B9993C37CFCDE4B9993C3793E766F2DC4C9E9BC97333796E26CFCDE4B9993C3793E7E6984C2693C96432994C2693C96432994C2693C96432994C2693C96432994C2693C96432994C2693C96433DA4C469BC96833196D26A3ADD7EBF57ABD5EAFD7EBF57ABD5EAFD7EBF57ABD5EAFD7";
            PduFactory messageFactory = PduFactory.NewInstance();

            MessageInformation msgInfo = messageFactory.Decode(content);

            Console.WriteLine(msgInfo);
        }


        public void TestUcs2Decode()
        {
            string content = "0051000B819060137320F300085F8B0C050415B300000804ED0903010033002000380020005400680061006E006B00200079006F007500200066006F00720020007500730069006E00670020007400680069007300200073006500720076006900630065002E002000200059006F007500720020007400720061006E00730061006300740069006F006E002000680061007300200062006500650051000B819060137320F300085F8B0C050415B300000804ED090302006E0020006C006F0067006700650064002000610073002000540058004E002000370020006100620063006400650066002000370020005400680061006E006B00200079006F007500200066006F00720020007500730069006E00670020007400680069007300200073006500720076006900630065002E0020002000590051000B819060137320F300085F6D0C050415B300000804ED090303006F007500720020007400720061006E00730061006300740069006F006E00200068006100730020006200650065006E0020006C006F0067006700650064002000610073002000540058004E002000370020006100620063006400650066007A";
            PduFactory messageFactory = PduFactory.NewInstance();
            MessageInformation msgInfo = messageFactory.Decode(content);
            Console.WriteLine(msgInfo.Content);
        }


        public void TestDecodePdu()
        {
            string[] pduCodes = new string[] {
            //"04912143F571000C911989765722930000FFA0060804007B0301D3BA783E96A7E1F4F4DB0D5A97F3F7B79C3C07BDDDA0594DA60305A5D46673CA1A56A94596B1EA1C06A54496B1EA8C56935A16F2E92C6659CBA0F3C95A069D41D632080D529344D632080D5293C46673CA5A06A5416AB1D99CB2964169905AA462A92CE6D35A6435B3C76A96D5CC1E9352264B5A34328B436AB33965498B46667148C55259",
            //"0051000B819060137320F300085F8B0C050415B300000804ED0903010033002000380020005400680061006E006B00200079006F007500200066006F00720020007500730069006E00670020007400680069007300200073006500720076006900630065002E002000200059006F007500720020007400720061006E00730061006300740069006F006E00200068006100730020006200650065",
            //"069149170000F331FF00810000475ECD703B0C12BFC9E9761954178741EE7A5B5C9683CAEB30A81D96D7D76176981C7681E67531A8BC0EAFC36BD039EC7687ED6177B80D1ACBCB617ADA5E06A1C3F234A89D1ECBDFECB018840ECBD3A073D89D7603",
            //"04912143F571000C9119897657229300F4FF8C060804007B0301537562736372697074696F6E206B6579776F726473206F6E203335323A204152544D4D532C435554452C46554E434152442C46554E5155495A2C484F4E45592C4B414E4F2C4B414E412C4B4150415449442C4B4150415449444D4D532C4B41524154454D4D532C4B41524154455458542C4C4F56452C4D594755592C4D594749524C2C5245"
            //"079145230000001004048126540003904020813413004E4536A8FDB6A7D920F71B444F97DD65103C3C5E83ECE973D94D2F83C865D0B4390591CB737219546EB5C369B60B141CD3D3F632FB0D62B3C3EDB09BFC0685D9205B2CE60201"            
            //"0051000B910621723364F400F5AA570605040B8423F0DC0601AE02056A0045C60C033230322E392E39382E35343A383038302F43475365727665722F73657276652E6A73703F69643D313233343536000103616D757A6F2E636F6D2043472030303031000101"
            "018006530B910691222903F990602021641323906020216423230000000000"
            };

            PduFactory messageFactory = PduFactory.NewInstance();
            foreach (string pduCode in pduCodes)
            {
                MessageInformation msgInfo = messageFactory.Decode(pduCode);
                Console.WriteLine(msgInfo.Content);
            }

        }

        public void TestEncode()
        {
            PduFactory messageFactory = PduFactory.NewInstance();
            Sms sms = Sms.NewInstance();
            sms.ServiceCenterNumber = "+60193900000";
            sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
            sms.DestinationAddress = "0192292309";
            //sms.DataCodingScheme = MessageDataCodingScheme.DefaultAlphabet;          
            sms.ValidityPeriod = MessageValidPeriod.OneHour;
            sms.Content = "ИНФОРМАЦИЯ О ФИНАНСИРОВАНИИ МЕРОПРИЯТИЙ РОССИЙСКОГО ПРЕДСЕДАТЕЛЬСТВА В «ГРУППЕ ВОСЬМИ» В 2006 ГОДУ";

            string[] pduList = messageFactory.Generate(sms);
            Console.WriteLine(pduList.Length);

            MessageInformation msg = messageFactory.Decode(pduList[0]);
            Console.WriteLine(msg.Content);

        }


        public void TestEncodeDecode8Bit()
        {
            Sms sms = Sms.NewInstance();
            sms.ServiceCenterNumber = "+60193900000";
            sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
            sms.DestinationAddress = "0192292309";
            sms.DataCodingScheme = MessageDataCodingScheme.EightBits;
            //sms.MessageReference = 123;
            sms.ValidityPeriod = MessageValidPeriod.OneHour;
            sms.Content = "8 bit encoding";

            PduFactory messageFactory = PduFactory.NewInstance();
            string[] pduCodes = messageFactory.Generate(sms);
            Console.WriteLine(pduCodes);
            MessageInformation messageInformation = messageFactory.Decode(pduCodes[0]);
            Console.WriteLine(messageInformation.Content);
        }



        /// <summary>
        /// SMS encoding test
        /// </summary>       
        public void testNewMessageFactory()
        {
            Sms sms = Sms.NewInstance();
            sms.ServiceCenterNumber = "+0160000001";
            sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
            sms.DestinationAddress = "0192292309";
            sms.DataCodingScheme = MessageDataCodingScheme.EightBits;
            //sms.MessageReference = 123;
            sms.ValidityPeriod = MessageValidPeriod.OneHour;
            //sms.Flash = true;
            sms.Content = "hello world ";
            sms.SourcePort = 1000;
            sms.DestinationPort = 2000;

            // PDU Number:1	Length For AT:23           
            PduFactory messageFactory = PduFactory.NewInstance();
            string[] pduList = messageFactory.Generate(sms);
            Console.WriteLine(pduList);
            MessageInformation msgInfo = messageFactory.Decode(pduList[0]);
            Console.WriteLine(msgInfo);

        }

        public void WappushEncode()
        {
            string href = "http://www.google.com.my";
            string text = "testing";

            // 0605040B8423F025060803AE81EA AF82B48401056A0045C60D03676F6F676C652E636F6D2E6D790008010374657374696E67000101
            // 0605040B8423F001060403AE81EA02056A0045C60D03676F6F676C652E636F6D2E6D79000AC3072009042416453010C307200904241
            // 64530010374657374696E67000101

            //07910691930000F051000A81102992329000F4FF44 
            //0605040B8423F001060403AE81EA02056A0045C60D03676F6F676C652E636F6D2E6D79000AC3072009042416453010C30720090424164530010374657374696E67000101
        }


        public void TestGenerateVCard()
        {
            vCard vCard = vCard.NewInstance();
            vCard.GivenName = "Testing";
            PhoneNumber p = new PhoneNumber();
            p.Number = "0192292309";
            p.HomeWorkType = HomeWorkTypes.Work;
            vCard.Phones.Add(p);
            vCard.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
            vCard.DestinationAddress = "0192292309";

            PduFactory pduFactory = PduFactory.NewInstance();
            string[] pduList = pduFactory.Generate(vCard);
            Console.WriteLine(pduList.Length);

        }

        public void GenerateVCalendar()
        {
            vEvent e = vEvent.NewInstance();
            e.Description = "sample calendar";
            e.DtStart = DateTime.Now;
            e.DtEnd = DateTime.Now.AddDays(1);
            e.Location = "any where";
            vCalendar vCalendar = vCalendar.NewInstance(e);
            vCalendar.DestinationAddress = "0192292309";

            string pdu = vCalendar.ToString();
            Console.WriteLine(pdu);
        }


        /// <summary>
        /// This is generally the most accurate approach to
        /// parsing a body of text into sentences to include
        /// the sentence's termination (e.g., the period,
        /// question mark, etc).  This approach will handle
        /// duplicate sentences with different terminations.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <returns></returns>
        private List<string> SplitSentences(string sourceText)
        {
            // create a local string variable
            // set to contain the string passed it
            string sTemp = sourceText;

            // create the array list that will
            // be used to hold the sentences
            List<string> al = new List<string>();

            // split the sentences with a regular expression
            string[] splitSentences =
                Regex.Split(sTemp, @"(?<=['""A-Za-z0-9][\.\!\?])\s+(?=[A-Z])");

            // loop the sentences
            for (int i = 0; i < splitSentences.Length; i++)
            {
                // clean up the sentence one more time, trim it,
                // and add it to the array list
                string sSingleSentence =
                    splitSentences[i].Replace(Environment.NewLine, string.Empty);
                al.Add(sSingleSentence.Trim());
            }

            // return the arraylist with
            // all sentences added
            return al;
        }


        public void TestMessageSplit()
        {
            string text = "testing";

            string[] results = text.Split(new string[] { ".", "," }, 140, StringSplitOptions.None);
            Console.WriteLine(results.Length);

        }



        public void TestDiagnose()
        {
            string[] diagnosticsCommand = Resources.DiagnosticCommand.Split('\n');
            List<string> commandList = diagnosticsCommand.ToList<string>();
            Console.WriteLine(diagnosticsCommand.Length);
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(currentDirectory);
            string commandFile = currentDirectory + Path.DirectorySeparatorChar + "command.txt";
            Console.WriteLine("Reading " + commandFile);
            if (File.Exists(commandFile))
            {
                string[] fileContent = File.ReadAllLines(commandFile, Encoding.UTF8);
                List<string> fileContentList = fileContent.ToList<string>();
                commandList.AddRange(fileContentList);
            }

            Console.WriteLine(commandList.Count);

        }


        public void TestReplace()
        {
            string value = "Error in command AT+CGMM?\r:\r\nERROR\r\n";
            string f = Regex.Replace(value, "[\r\n]", " ");
            Console.WriteLine(f);
        }

        public void TestGetInstanceType()
        {
            Sms sms = Sms.NewInstance();
            Sms vCal = vCalendar.NewInstance();
        }



        public void TestMatchCall()
        {
            //string input = "\\+CLIP: \"0126868739\",129,,,\"Choon\"";
            string input = "\\+CRING: VOICE";
            CallIndicationHandlers handlers = new CallIndicationHandlers();

            if (handlers.IsUnsolicitedCall(input))
            {
                string description;
                IIndicationObject callIndicationObject =
                    handlers.HandleUnsolicitedCall(ref input, out description);
                Console.WriteLine(description);
            }
        }


        public void TestGetMessageStorageLocation()
        {
            List<string> storages = new List<string>(3);

            string result = "+CPMS: \"SM\",1,30,\"SM\",1,30,\"ME\",60,1000";
            Regex regCpms = new Regex("\\s*\\+CPMS:\\s*", RegexOptions.Compiled);
            string response = regCpms.Replace(result, "");
            response += ",";
            string[] tokens = response.Split(new string[] { "," }, StringSplitOptions.None);
            int i = 0;
            string loc;
            string storageLocations = string.Empty;
            while (i <= tokens.GetUpperBound(0))
            {
                loc = tokens[i].Replace("\"", "");
                if (!storages.Contains(loc)) storages.Add(loc);
                i += 3;
            }
            Console.WriteLine(storageLocations);
        }

        public void TestOtaBitmap()
        {
            Bitmap image = new Bitmap("c:\\otabitmap.png");
            OtaBitmap otaBitmap = OtaBitmap.NewInstance(image);
            Bitmap newImage = otaBitmap.ToBitmap();
            newImage.Save("c:\\newBitmap.jpg", ImageFormat.Jpeg);
        }

        public void TestLicense()
        {
            HashAlgorithm sha = new SHA1CryptoServiceProvider();
            //byte[] dataArray = Encoding.ASCII.GetBytes("850801-02-6191");
            byte[] dataArray = Encoding.ASCII.GetBytes("740731-01-5091");
            byte[] result = sha.ComputeHash(dataArray);
            Console.WriteLine(Encoding.ASCII.GetString(result));
        }


        public void TestMessageIndicationHandlers()
        {
            MessageIndicationHandlers messageIndicationHandlers = new MessageIndicationHandlers();
            string data = "+CMTI: \"SM\",20";
            if (messageIndicationHandlers.IsUnsolicitedMessage(data))
            {
                Console.WriteLine("ok");
            }
            else if (messageIndicationHandlers.IsIncompleteUnsolicitedMessage(data))
            {
                Console.WriteLine("Incomplete");
            }
            else
            {
                Console.WriteLine("not meet at all");
            }
        }


        /// <summary>
        /// Read data from gateway
        /// </summary>     
        [Test]
        public void TestInboundDataReader()
        {
            MessageIndicationHandlers messageIndicationHandlers = new MessageIndicationHandlers();
            CallIndicationHandlers callIndicationHandlers = new CallIndicationHandlers();
            string data = string.Empty;
            string input = "+CLIP: \"+62229374722,145";
            int i = 0;
            while (i++ <= 3)
            {
                try
                {
                    if (i == 1)
                    {
                        //input = "\r\n+CUSD: 1,\"DiGi Menu \r\n1 Account Info\r\n2 Reload\r\n3 Friend";
                        //input = "\r\n+CMGL: 2,1,,108\r\n07910661929";
                        //input = "\r\n\r\n+CMGS 21\r\nOK\r\n+CDS: 29\r\n0180061C0A8110299232900140429135722301404291358223000000000000\r\nOK";
                        //input = "\r\nCM: 29\r\n";
                        //input = "\r\n\r\n+CME ERR\r\n\r\n";
                        input = "+CLIP: \"+622293747223\",145";
                    }
                    else if (i == 2)
                    {
                        //input = "s and Family\r\n4 Talktime Services\r\n5 Roaming\r\n6 Mobile Services\r\n7 Change Features\r\n8 DiGi Rewards\",15\r\n>";
                        //input = "OR 38\r\n";
                        //input = "0180061C0A8110299232900140429135722301404291358223000000000000";
                    }
                    else if (i == 3)
                    {
                        //input = "";
                    }
                    data += input;
                    if (!string.IsNullOrEmpty(data))
                    {

                        // Check if it is an unsolicited message
                        bool isIncompleteMessage = false;
                        bool isIncompleteCall = false;
                        string unhandledData = string.Empty;
                        if (CheckUnsolicitedMessage(messageIndicationHandlers, ref data, ref isIncompleteMessage, ref unhandledData))
                        {
                            if (!string.IsNullOrEmpty(data) && !isIncompleteMessage)
                            {
                                // Need to add into incoming data queue
                                EnqueueData(ref data);
                            }
                            if (!string.IsNullOrEmpty(unhandledData))
                            {
                                // Need to add into incoming data queue
                                EnqueueData(ref unhandledData);
                            }

                        }
                        else if (CheckUnsolicitedCall(callIndicationHandlers, ref data, ref isIncompleteCall, ref unhandledData))
                        {
                            //Logger.LogThis("The incoming call indication is processed.", LogLevel.Verbose, this.Id);
                            if (!string.IsNullOrEmpty(data) && !isIncompleteCall)
                            {
                                // Need to add into incoming data queue
                                EnqueueData(ref data);
                            }
                            if (!string.IsNullOrEmpty(unhandledData))
                            {
                                // Need to add into incoming data queue
                                EnqueueData(ref unhandledData);
                            }
                        }
                        else
                        {
                            EnqueueData(ref data);
                        }
                    }
                    //input = "4\r\nOK\r\n";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }
        }

        /// <summary>
        /// Check for unsolicited message
        /// </summary>
        /// <param name="handlers">Unsolicited message handlers</param>
        /// <param name="input"></param>
        /// <param name="isInCompletedMessage"></param>
        /// <returns></returns>
        private bool CheckUnsolicitedMessage(MessageIndicationHandlers handlers, ref string input, ref bool isInCompletedMessage, ref string unhandledData)
        {
            string[] lines = input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            bool appendCrLf = false;
            bool prefixCrLf = false;

            if (input.EndsWith("\r\n")) appendCrLf = true;
            if (input.StartsWith("\r\n")) prefixCrLf = true;

            string data = string.Empty;
            List<string> rawData = new List<string>();
            bool containsUnsolicitedMessage = false;
            int lineCount = 0;

            foreach (string line in lines)
            {
                lineCount++;
                if (!string.IsNullOrEmpty(line))
                {
                    data += line;
                    if (handlers.IsUnsolicitedMessage(data))
                    {
                        // Is unsolicited message, raise the message received event
                        string description;
                        string message = data;

                        IIndicationObject messageIndicationObject =
                            handlers.HandleUnsolicitedMessage(ref message, out description);

                        //Logger.LogThis("Unsolicited message: " + description, LogLevel.Verbose, this.Id);

                        // Raise event
                        //ProcessMessageReceived(messageIndicationObject);

                        // Reset
                        data = string.Empty;

                        isInCompletedMessage = false;
                        containsUnsolicitedMessage = true;
                    }
                    else if (handlers.IsIncompleteUnsolicitedMessage(data))
                    {
                        // Need to check for next line
                        if (lineCount != lines.Length)
                        {
                            data += "\r\n";
                        }
                        else
                        {
                            if (prefixCrLf)
                                data = "\r\n" + data;

                            if (appendCrLf)
                                data += "\r\n";
                        }
                        containsUnsolicitedMessage = true;
                        isInCompletedMessage = true;
                    }
                    else
                    {
                        // Need to check for next line
                        if (lineCount != lines.Length)
                        {
                            if (lineCount == 1 && prefixCrLf)
                                rawData.Add("\r\n");

                            rawData.Add(data + "\r\n");
                        }
                        else
                        {
                            if (prefixCrLf && lines.Length == 1)
                                rawData.Add("\r\n");

                            if (appendCrLf)
                                rawData.Add(data + "\r\n");
                            else
                                rawData.Add(data);
                        }
                        data = string.Empty;
                    }
                }
            }

            if (!containsUnsolicitedMessage) return false;


            if (isInCompletedMessage)
            {
                foreach (string line in rawData)
                {
                    unhandledData += line;
                }
                return containsUnsolicitedMessage;
            }


            if (!string.IsNullOrEmpty(data))
            {
                string[] unprocessedList = data.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                appendCrLf = false;
                prefixCrLf = false;
                if (data.StartsWith("\r\n")) prefixCrLf = true;
                if (data.EndsWith("\r\n")) appendCrLf = true;
                lineCount = 0;
                foreach (string line in unprocessedList)
                {
                    lineCount++;
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (lineCount != lines.Length)
                        {
                            if (lineCount == 1 && prefixCrLf)
                                rawData.Add("\r\n");

                            rawData.Add(line + "\r\n");
                        }
                        else
                        {

                            if (prefixCrLf && lines.Length == 1)
                                rawData.Add("\r\n");

                            if (appendCrLf)
                                rawData.Add(line + "\r\n");
                            else
                                rawData.Add(line);

                        }
                    }
                }
                input = string.Empty;
                foreach (string line in rawData)
                {
                    input += line;
                }
            }
            else
            {
                input = string.Empty;
                foreach (string line in rawData)
                {
                    unhandledData += line;
                }
            }

            return containsUnsolicitedMessage;
        }

        /// <summary>
        /// Check for unsolicited call
        /// </summary>
        /// <param name="handlers">Unsolicited call handlers</param>
        /// <param name="input">The input.</param>
        /// <param name="isIncompleteCall">if set to <c>true</c> [is incomplete call].</param>
        /// <param name="unhandledData">The unhandled data.</param>
        /// <returns></returns>
        private bool CheckUnsolicitedCall(CallIndicationHandlers handlers, ref string input, ref bool isIncompleteCall, ref string unhandledData)
        {
            string[] lines = input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            bool appendCrLf = false;
            bool prefixCrLf = false;

            if (input.EndsWith("\r\n")) appendCrLf = true;
            if (input.StartsWith("\r\n")) prefixCrLf = true;

            string data = string.Empty;
            List<string> rawData = new List<string>();
            bool containsUnsolicitedCall = false;

            int lineCount = 0;
            foreach (string line in lines)
            {
                lineCount++;
                if (!string.IsNullOrEmpty(line))
                {
                    data += line;
                    if (handlers.IsUnsolicitedCall(data))
                    {
                        // Is unsolicited call, raise the call received event
                        string description;
                        string call = data;

                        IIndicationObject callInformation =
                            handlers.HandleUnsolicitedCall(ref call, out description);

                        //Logger.LogThis("Unsolicited call: " + data, LogLevel.Verbose, this.Id);
                        //Logger.LogThis(description, LogLevel.Verbose, this.Id);

                        // Raise event
                        //ProcessCallReceived(callInformation);

                        // Reset
                        data = string.Empty;

                        isIncompleteCall = false;
                        containsUnsolicitedCall = true;
                    }
                    else if (handlers.IsIncompleteUnsolicitedCall(data))
                    {
                        // Need to check for next line
                        if (lineCount != lines.Length)
                        {
                            data += "\r\n";
                        }
                        else
                        {
                            if (prefixCrLf)
                                data = "\r\n" + data;

                            if (appendCrLf)
                                data += "\r\n";
                        }
                        //Logger.LogThis("Incomplete unsolicited call: " + data, LogLevel.Verbose, this.Id);
                        containsUnsolicitedCall = true;
                        isIncompleteCall = true;
                    }
                    else
                    {
                        // Need to check for next line
                        if (lineCount != lines.Length)
                        {
                            if (lineCount == 1 && prefixCrLf)
                                rawData.Add("\r\n");

                            rawData.Add(data + "\r\n");
                        }
                        else
                        {
                            if (prefixCrLf && lines.Length == 1)
                                rawData.Add("\r\n");

                            if (appendCrLf)
                                rawData.Add(data + "\r\n");
                            else
                                rawData.Add(data);
                        }
                        data = string.Empty;
                    }
                }
            }

            if (!containsUnsolicitedCall) return false;
            if (isIncompleteCall)
            {
                foreach (string line in rawData)
                {
                    unhandledData += line;
                }
                return containsUnsolicitedCall;
            }

            if (!string.IsNullOrEmpty(data))
            {
                string[] unprocessedList = data.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                appendCrLf = false;
                prefixCrLf = false;

                if (data.StartsWith("\r\n")) prefixCrLf = true;
                if (data.EndsWith("\r\n")) appendCrLf = true;

                lineCount = 0;
                foreach (string line in unprocessedList)
                {
                    lineCount++;
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (lineCount != lines.Length)
                        {
                            if (lineCount == 1 && prefixCrLf)
                                rawData.Add("\r\n");

                            rawData.Add(line + "\r\n");
                        }
                        else
                        {
                            if (prefixCrLf && lines.Length == 1)
                                rawData.Add("\r\n");

                            if (appendCrLf)
                                rawData.Add(line + "\r\n");
                            else
                                rawData.Add(line);
                        }
                    }
                }
                input = string.Empty;
                foreach (string line in rawData)
                {
                    input += line;
                }
            }
            else
            {
                input = string.Empty;
                foreach (string line in rawData)
                {
                    unhandledData += line;
                }
            }

            return containsUnsolicitedCall;
        }




        /// <summary>
        /// Encode the data
        /// </summary>
        /// <param name="data">Data to be enqueued</param>
        private void EnqueueData(ref string data)
        {
            string[] lines = data.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            bool appendCrLf = false;
            if (data.EndsWith("\r\n")) appendCrLf = true;
            if (lines.Length > 1)
            {
                // Multiple lines received
                int lineCount = 0;
                foreach (string line in lines)
                {
                    lineCount++;
                    if (!string.IsNullOrEmpty(line))
                    {
                        string input = line;
                        if (lineCount != lines.Length)
                        {
                            incomingDataQueue.Enqueue(input + "\r\n");
                        }
                        else
                        {
                            if (appendCrLf)
                                incomingDataQueue.Enqueue(input + "\r\n");
                            else
                                incomingDataQueue.Enqueue(input);
                        }
                    }
                    else
                    {
                        /*
                        string trimText = line.Trim();
                        if (string.IsNullOrEmpty(trimText)) continue; 
                        */
                        if (lineCount != lines.Length)
                        {
                            incomingDataQueue.Enqueue("\r\n");
                        }
                    }
                }
            }
            else
            {
                incomingDataQueue.Enqueue(data);
            }
            data = string.Empty;
        }



        public void TestGetPduLength()
        {
            Sms sms = Sms.NewInstance();
            sms.ServiceCenterNumber = "002B003900320033003300330030003000300035003100350030";
            Console.WriteLine(GetPduLength(sms, "1B91200B3009300230033003300330003000300030053001300530F031000B813012828842F40010B005E8329BFD06"));
            Console.WriteLine(GetAtLength("1B91200B3009300230033003300330003000300030053001300530F031000B813012828842F40010B005E8329BFD06"));
        }
        /// <summary>
        /// Calculate message length
        /// </summary>
        /// <param name="pduString">PDU string</param>
        /// <returns>Message length</returns>
        protected static int GetAtLength(string pduString)
        {
            // Get AT command length
            return (pduString.Length - Convert.ToInt32(pduString.Substring(0, 2), 16) * 2 - 2) / 2;
        }


        /// <summary>
        /// Calculate message length
        /// </summary>
        /// <param name="sms">SMS object</param>
        /// <param name="pdu">PDU string</param>
        /// <returns>Message length</returns>
        protected static int GetPduLength(Sms sms, string pdu)
        {
            int pduLength = pdu.Length;
            pduLength /= 2;
            if (string.IsNullOrEmpty(sms.ServiceCenterNumber))
                pduLength--;
            else
            {
                int smscNumberLen = sms.ServiceCenterNumber.Length;
                if (sms.ServiceCenterNumber[0] == '+') smscNumberLen--;
                if (smscNumberLen % 2 != 0) smscNumberLen++;
                int smscLen = (2 + smscNumberLen) / 2;
                pduLength = pduLength - smscLen - 1;
            }
            return pduLength;
        }



        public void TestMessageType()
        {
            Sms sms = Sms.NewInstance();
            MessagingToolkit.Core.Base.IMessage vCard = MessagingToolkit.Core.Mobile.Message.vCard.NewInstance();
            vCalendar vCalendar = vCalendar.NewInstance();
            Wappush wapPush = Wappush.NewInstance("ss", "ss", "ss");

            if (sms is Sms)
            {
                Console.WriteLine("Sms is Sms");
            }

            if (vCard is vCard)
            {
                Console.WriteLine("vCard is vCard");
            }

            if (vCard is Sms)
            {
                Console.Write("vCard is Sms");
            }

            if (vCard is vCalendar)
            {
                Console.WriteLine("vCard is vCalendar");
            }
        }

        public void TestPduDecode()
        {
            string pduCode = "0791293303001505240C912933520184250000013011219372020E32994C2693C96432994C269301";
            PduFactory messageFactory = PduFactory.NewInstance();

            MessageInformation messageInformation = messageFactory.Decode(pduCode);

            Console.WriteLine(messageInformation);
        }

        public void TestMessagePolling()
        {

            DateTime now = DateTime.Now;

            MessageInformation message1 = new MessageInformation();
            message1.ReferenceNo = 0;
            message1.Content = "start1";
            message1.CurrentPiece = 1;
            message1.TotalPiece = 2;
            message1.ReceivedDate = now;
            //messages.Add(message1);

            MessageInformation message2 = new MessageInformation();
            message2.ReferenceNo = 0;
            message2.Content = "end";
            message2.CurrentPiece = 2;
            message2.TotalPiece = 2;
            //messages.Add(message2);

            MessageInformation message3 = new MessageInformation();
            message3.ReferenceNo = 0;
            message3.Content = "start3";
            message3.CurrentPiece = 1;
            message3.TotalPiece = 2;
            message3.ReceivedDate = now;
            //messages.Add(message3);


            List<List<MessageInformation>> messageLookup = new List<List<MessageInformation>>(5);

            for (int z = 1; z <= 2; z++)
            {

                List<MessageInformation> messages = new List<MessageInformation>();

                if (z == 1)
                {
                    messages.Add(message1);
                }
                else
                {
                    messages.Add(message2);
                    messages.Add(message3);
                }

                // Concatenate the messages
                List<MessageInformation> tmpMessageList = new List<MessageInformation>(messages.Count);
                foreach (MessageInformation message in messages)
                {
                    //if (message.TotalPiece > 1 && message.ReferenceNo != 0)
                    if (message.TotalPiece > 1)
                    {
                        bool found = false;
                        bool duplicate = false;
                        bool addToList = true;
                        foreach (List<MessageInformation> messageList in messageLookup)
                        {
                            MessageInformation tmpMsg = messageList[0];
                            if (tmpMsg.ReferenceNo == message.ReferenceNo)
                            {
                                duplicate = false;
                                foreach (MessageInformation listMsg in messageList)
                                {
                                    if (listMsg.CurrentPiece == message.CurrentPiece)
                                    {
                                        duplicate = true;
                                        // Additional verification
                                        if (listMsg.ReceivedDate.Equals(message.ReceivedDate))
                                        {
                                            addToList = false;
                                        }
                                        break;
                                    }
                                }
                                if (!duplicate)
                                {
                                    messageList.Add(message);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (!found && addToList)
                        {
                            List<MessageInformation> tmpList = new List<MessageInformation>();
                            tmpList.Add(message);
                            messageLookup.Add(tmpList);
                        }
                    }
                    else
                    {
                        tmpMessageList.Add(message);
                    }
                }

                // Build messages from message lookup
                List<List<MessageInformation>> toRemove = new List<List<MessageInformation>>();
                foreach (List<MessageInformation> msgList in messageLookup)
                {
                    List<MessageInformation> orderList = msgList.OrderByDescending(message => message.CurrentPiece).ToList();
                    MessageInformation lastMsg = orderList[0];
                    if (lastMsg.TotalPiece == msgList.Count())  // Meaning all already received
                    {
                        toRemove.Add(msgList);
                        for (int i = 1; i < orderList.Count(); i++)
                        {
                            MessageInformation msg = orderList[i];
                            lastMsg.Content = msg.Content + lastMsg.Content;
                            lastMsg.Indexes.Add(msg.Index);
                            lastMsg.TotalPieceReceived++;
                            lastMsg.RawMessage = msg.RawMessage + "\r\n" + lastMsg.RawMessage;
                        }
                        // Add to tmpMessageList
                        tmpMessageList.Add(lastMsg);
                    }
                }

                // Remove the complete messages
                foreach (List<MessageInformation> item in toRemove)
                {
                    messageLookup.Remove(item);
                }
            }

        }


        public void TestUssd()
        {
            const string Pattern = "\\+CUSD:\\s*(\\d+),\\s*\"([^\"]*)\"(,\\s*(\\d+))*";
            string input = "+CUSD: 0,\"0E040E380E130E400E1B0E470E190E250E390E010E040E490E32002000220E400E2D0E440E2D0E400E2D0E2A00200E270E310E19002D0E170E39002D0E040E2D0E250021002200200E2B0E210E320E220E400E250E0200200030003800380035003300310031003400380031\",72";

            Match matcher = Regex.Match(input, Pattern);
            if (!matcher.Success)
            {
                Console.Write("not matched");
            }
            else
            {
                Console.WriteLine("matched");
            }

            UssdRequest ussdRequest = new UssdRequest("xx");
            Console.WriteLine(ussdRequest);

            UssdResponse ussdResponse = new UssdResponse(input, "abc");

            string content = "0E040E380E130E400E1B0E470E190E250E390E010E040E490E32002000220E400E2D0E440E2D0E400E2D0E2A00200E270E310E19002D0E170E39002D0E040E2D0E250021002200200E2B0E210E320E220E400E250E0200200030003800380035003300310031003400380031";

            char esc = Convert.ToChar(0x1b);
            string c = Convert.ToString(esc) + "\r";
            byte[] b = new ASCIIEncoding().GetBytes(c);
            Console.WriteLine(b.Length);
            /*
            byte[] responseEncodedSeptets = MessagingToolkit.Pdu.PduUtils.PduToBytes(content);
            byte[] responseUnencodedSeptets = MessagingToolkit.Pdu.PduUtils.EncodedSeptetsToUnencodedSeptets(responseEncodedSeptets);
            string decoded = MessagingToolkit.Pdu.PduUtils.UnencodedSeptetsToString(responseUnencodedSeptets);
            Console.WriteLine(decoded);
            */
        }

        public void TestGetType()
        {
            string text = "[Trial] test";
            string pdu = PduUtils.TextToPdu(text);
            Console.WriteLine(pdu);

            string content = "this is a test";

            int refNo = new Random().Next();
            refNo %= 65536;
            Console.WriteLine(refNo);
        }



        public void TestMatching()
        {
            /*
            
            //string s = "\\+CME ERROR: (\\d+)"
            string s = "\\+CUSD:\\s*(\\d+),\\s*\"([^\"]*)\"(,\\s*(\\d+))*";
            string input = "\r\n+CUSD: 2,\"My mobile number: 60102990874 Last Call Cost : RM 0.10 Balance: RM 0.00 Reload Before: 13/05/2010 Free Talktime: RM 0.00\",15\r\n";
            if (Regex.IsMatch(input, s))
            {
                Console.WriteLine("matched");
            }
            else
            {
                Console.WriteLine("not matched");
            }
            */

            string input = "ERROR\r\n\r\n";
            string pattern = "ERROR\r\n";

            if (Regex.IsMatch(input, pattern))
            {
                Console.WriteLine("yes");
            }
        }
        
        public void TestGuid()
        {
            Console.WriteLine(System.Guid.NewGuid().ToString());
            Console.WriteLine(System.Guid.NewGuid().ToString("N"));
            Console.WriteLine(System.Guid.NewGuid().ToString("D"));
            Console.WriteLine(System.Guid.NewGuid().ToString("B"));
            Console.WriteLine(System.Guid.NewGuid().ToString("P"));
        }
    }
}
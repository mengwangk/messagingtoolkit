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
using System.IO;
using System.Threading;
using System.Drawing;

using MessagingToolkit.Core;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Mobile.Message;

using NUnit.Framework;

namespace MessageToolkit.Core.Test
{
    /// <summary>
    /// NUnit test for MessageServer
    /// </summary>
    [TestFixture]
    public class MessageGatewayTest
    {
        #region ========================= Declarations ========================

        /// <summary>
        /// Port name
        /// </summary>
        private const string PortName = "COM10";


        /// <summary>
        /// Mobile gateway
        /// </summary>
        private IMobileGateway mobileGateway;

        #endregion =============================================================


        #region ======================== Testing ===============================



        /// <summary>
        /// Set up the test
        /// </summary>
        [SetUp]
        protected void SetUp()
        {
            // Create the gateway for mobile
            MessageGateway<IMobileGateway, MobileGatewayConfiguration> messageServer =
                MessageGateway<IMobileGateway, MobileGatewayConfiguration>.NewInstance();

            // Create the mobile gateway configuration
            MobileGatewayConfiguration config = MobileGatewayConfiguration.NewInstance();
            config.PortName = PortName;           
            config.Pin = "740731";
            config.ProviderMMSC = "http://mms.celcom.net.my/";
            config.ProviderWAPGateway = "10.128.1.242";
            config.ProviderAPN = "mms.celcom.net.my";
                        
            try
            {
                mobileGateway= messageServer.Find(config);
                mobileGateway.LogLevel = LogLevel.Verbose;
            }
            catch (GatewayException gex)
            {
                Assert.Fail(gex.Message);
                Assert.Fail(gex.ToString());
            }        
                        
        }

        /// <summary>
        /// Disconnet the gateway upon exit
        /// </summary>
        [TearDown]    
        public void TearDown()
        {
            try
            {
                Thread.Sleep(10000);
                if (mobileGateway != null && mobileGateway.Connected)
                {
                    mobileGateway.Disconnect();
                }
                mobileGateway = null;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        
        public void TestGetSca()
        {
            string sca = mobileGateway.ServiceCentreAddress.Number;
            Console.WriteLine(sca);
        }

      
        public void TestGetNetworkRegistration()
        {
            NetworkRegistration status = mobileGateway.NetworkRegistration;
            Console.WriteLine(status.UnsolicitedResultCode);
        }

        
        public void TestGetSupportedMessageIndications()
        {
            //MessageIndicationSettings settings = mobileGateway.GetSupportedMessageIndications();
            //Console.WriteLine(settings.CellBroadcastMessage);
        }
  
   
        public void TestSendSms()
        {
            try
            {
                mobileGateway.MessageStorage = MessageStorage.Phone;
                mobileGateway.EnableNewMessageNotification(MessageNotification.StatusReport);                                
                mobileGateway.MessageReceived += OnMessageReceived;
                mobileGateway.MessageSendingFailed += OnMessageFailed;
                mobileGateway.MessageSent += OnMessageSent;
                mobileGateway.PollNewMessages = true;
                //mobileGateway.Configuration.DeleteReceivedMessage = true;

                mobileGateway.LogLevel = LogLevel.Verbose;
                
                // Create a new SMS instance
                Sms sms = Sms.NewInstance();
                sms.DestinationAddress = "0192292309";
                sms.Content = "test";
                sms.ServiceCenterNumber = "+60193900000";
                sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
                mobileGateway.Send(sms);

                /*
                sms = Sms.NewInstance();
                sms.DestinationAddress = "0192292309";
                sms.Content = "testing 2";
                sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
                mobileGateway.SendToQueue(sms);

                sms = Sms.NewInstance();
                sms.DestinationAddress = "0192292309";
                sms.Content = "testing 3";
                sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
                mobileGateway.SendToQueue(sms);

                sms = Sms.NewInstance();
                sms.DestinationAddress = "0192292309";
                sms.Content = "testing 4";
                sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
                mobileGateway.SendToQueue(sms);

                sms = Sms.NewInstance();
                sms.DestinationAddress = "0192292309";
                sms.Content = "testing 5";
                sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
                mobileGateway.SendToQueue(sms);
                */ 
                /*
                // Create a new SMS instance
                sms = Sms.NewInstance();
                sms.DestinationAddress = "0192292309";
                sms.Content = "testing 6";
                mobileGateway.SendToQueue(sms);

                sms = Sms.NewInstance();
                sms.DestinationAddress = "0192292309";
                sms.Content = "testing 7";
                mobileGateway.SendToQueue(sms);

                sms = Sms.NewInstance();
                sms.DestinationAddress = "0192292309";
                sms.Content = "testing 8";
                mobileGateway.SendToQueue(sms);

                sms = Sms.NewInstance();
                sms.DestinationAddress = "0192292309";
                sms.Content = "testing 9";
                mobileGateway.SendToQueue(sms);

                sms = Sms.NewInstance();
                sms.DestinationAddress = "0192292309";
                sms.Content = "testing 10";
                mobileGateway.SendToQueue(sms);
                Console.WriteLine(sms);
                */

                // Sleep 30 seconds
                //Thread.Sleep(90000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            
        }

       
        public void TestEnableMessageIndications()
        {
            mobileGateway.EnableMessageNotifications();
        }

        private void OnMessageSending(object sender, MessageEventArgs e)
        {
            Sms sms = (Sms)e.Message;
            Console.WriteLine(sms.Content);
        }
                
        public void TestSendVCalendar()
        {
            vEvent e = vEvent.NewInstance();
            e.Description = "sample calendar";
            e.DtStart = DateTime.Now;
            e.DtEnd = DateTime.Now.AddDays(1);
            e.Summary = "Sample vCalendar";            
            e.Location = "any where";
            vCalendar vCalendar = vCalendar.NewInstance(e);
            vCalendar.DestinationAddress = "0192292309";           

            mobileGateway.SendToQueue(vCalendar);
            Console.WriteLine(vCalendar);
        }

        public void TestSendVCard()
        {
            vCard vCard = vCard.NewInstance();
            vCard.GivenName = "Koh";
            PhoneNumber p = new PhoneNumber();
            p.Number = "0192292309";
            p.HomeWorkType = HomeWorkTypes.Work;
            vCard.Phones.Add(p);
            vCard.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
            vCard.DestinationAddress = "0192292309";
            mobileGateway.SendToQueue(vCard);
            Console.WriteLine(vCard);
        }
             
        public void TestSendWappush()
        {
            // Create a new SMS instance    
            Wappush wappush = Wappush.NewInstance("0192292309", "http://www.google.com.my", "wap testing dvfs fsdfdsfsdlkfjdskfjsdfjwelfjewlfjwelkfjls dfjsldf jdslkfjsdlkfj sdlkfjsdklf jskdlfjklewfjlkwejfweilfjklwefjkdsljf slkfjsdklfjskdlf jdsklfjdslkfjsl kdfjlkwejfilewjflewjflkewj");
            wappush.Flash = true;
            mobileGateway.SendToQueue(wappush);
            Console.WriteLine(wappush);           
        }

    
        public void TestSendOtaBitmap()
        {
            Bitmap image = new Bitmap("c:\\ms.png");
            OtaBitmap otaBitmap = OtaBitmap.NewInstance(image);
            otaBitmap.DestinationAddress = "0126868739";
            if (!mobileGateway.Send(otaBitmap))
            {
                Console.WriteLine(mobileGateway.LastError.Message);
                Console.WriteLine(mobileGateway.LastError.StackTrace);
            }
        }

      
        public void TestSendRingtone()
        {
            Ringtone ringtone = Ringtone.NewInstance();
            ringtone.Content = "7C0C7A00424547494E3A494D454C4F44590D0A56455253494F4E3A312E320D0A464F524D41543A434C415353312E300D0A4D454C4F44593A6433236333643323633364337232643323633364336533236633653323663367336133236733613323673361337232613323673361330D0A454E443A494D454C4F44590D0A";
            ringtone.DestinationAddress = "0126868739";
            if (!mobileGateway.Send(ringtone))
            {
                Console.WriteLine(mobileGateway.LastError.Message);
                Console.WriteLine(mobileGateway.LastError.StackTrace);
            }
        }
                
        public void TestMemoryLocation()
        {
            mobileGateway.MessageStorage = MessageStorage.Sim;
        }


       
        public void TestGetMessages()
        {
            mobileGateway.MessageStorage = MessageStorage.Phone;
            List<MessageInformation> messages = mobileGateway.GetMessages(MessageStatusType.AllMessages);
            foreach (MessageInformation message in messages)
            {
                Console.WriteLine(message.Content);
            }
        }

        
        public void TestGetMessage()
        {
            mobileGateway.MessageStorage = MessageStorage.Phone;
            MessageInformation message = mobileGateway.GetMessage(1);
            Console.WriteLine(message.Content);
        }

        
        public void TestDeleteMessage()
        {
            mobileGateway.DeleteMessage(MessageDeleteOption.AllMessages);
            mobileGateway.DeleteMessage(MessageDeleteOption.ByIndex, 1);
        }

         
        public void TestSendCommand()
        {
            string results = mobileGateway.SendCommand("AT+CSCA?");
           
            Console.WriteLine(results);

            Console.WriteLine(mobileGateway.LastError.Message);
        }

       
        public void TestGetDeviceInformation()
        {
            Console.WriteLine(mobileGateway.DeviceInformation.Model);
            Console.WriteLine(mobileGateway.DeviceInformation.Manufacturer);
            Console.WriteLine(mobileGateway.DeviceInformation.SerialNo);
            Console.WriteLine(mobileGateway.DeviceInformation.FirmwareVersion);
            Console.WriteLine(mobileGateway.DeviceInformation.Imsi);
        }

       
        public void TestDiagnose()
        {
            Console.WriteLine(mobileGateway.Diagnose());
        }

     
        public void TestWatchDog()
        {
            Thread.Sleep(20000);
        }

              
        public void TestReceivedMessage()
        {
            mobileGateway.EnableMessageNotifications();
            mobileGateway.EnableCallNotifications();
            mobileGateway.MessageReceived += OnMessageReceived;
            mobileGateway.CallReceived += OnCallReceived;

            Thread.Sleep(60000);
        }


        private void OnCallReceived(object sender, IncomingCallEventArgs e)
        {
            Console.WriteLine("Call received from " + e.CallInformation.Number);
            Console.WriteLine("Call type " + e.CallInformation.NumberType);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Received type: " + e.Message.MessageType);
            Console.WriteLine("Received Content: " + e.Message.Content);
        }

        private void OnMessageSent(object sender, MessageEventArgs e)
        {
            Sms sms = (Sms)e.Message;
            Console.WriteLine("Sent Content: " + sms.Content);
        }

        private void OnMessageFailed(object sender, MessageErrorEventArgs e)
        {
            Sms sms = (Sms)e.Message;
            Console.WriteLine("Failed Content: " + sms.Content);
            Console.WriteLine("Exception: " + e.Error.Message);
        }

       
        public void TestSignalQuality()
        {
            SignalQuality signalQuality = mobileGateway.SignalQuality;

            Console.WriteLine("Strength: " + signalQuality.SignalStrength);
            Console.WriteLine("Bit error rate: " + signalQuality.BitErrorRate);
        }

      
        public void TestSupportedCharacterSets()
        {
            string[] supportedCharacterSets = mobileGateway.SupportedCharacterSets;
            foreach (string characterSet in supportedCharacterSets)
            {
                Console.WriteLine(characterSet);
            }
        }
              
        public void TestCurrentCharacterSet()
        {
            Console.WriteLine(mobileGateway.CurrentCharacterSet);
        }

        
        public void TestBatteryCharge()
        {
            BatteryCharge batteryCharge = mobileGateway.BatteryCharge;
            Console.WriteLine("Level: " + batteryCharge.BatteryChargeLevel);
            Console.WriteLine("Status: " + batteryCharge.BatteryChargingStatus);
        }

        
        public void TestSetCharacterSet()
        {
            mobileGateway.SetCharacterSet("GSM");
        }

      
        public void TestSetSmsc()
        {
            NumberInformation sca = mobileGateway.ServiceCentreAddress;
            mobileGateway.SetServiceCentreAddress(sca);

        }


      
        public void TestAcknowledge()
        {
            Console.WriteLine(mobileGateway.IsAcknowledgeRequired);
            mobileGateway.RequireAcknowledgeNewMessage(true);
            Console.WriteLine(mobileGateway.IsAcknowledgeRequired);
            mobileGateway.RequireAcknowledgeNewMessage(false);
            Console.WriteLine(mobileGateway.IsAcknowledgeRequired);
            mobileGateway.AcknowledgeNewMessage();
        }

     
        public void TestPhoneFunctions()
        {
            if (mobileGateway.Dial("0126868739"))
            {

            }
        }


     
        public void TestPhoneBook()
        {
            string[] phoneStorages = mobileGateway.PhoneBookStorages;
            foreach (string storage in phoneStorages)
            {
                Console.WriteLine(storage);
            }

            MemoryStatusWithStorage memoryStatus = mobileGateway.GetPhoneBookMemoryStatus(PhoneBookStorage.Phone);
            Console.WriteLine(memoryStatus.Storage);

            PhoneBookEntry[] phoneBookEntries = mobileGateway.GetPhoneBook(PhoneBookStorage.Phone);
            Console.WriteLine(phoneBookEntries.Count());

            vCard[] vCards = mobileGateway.ExportPhoneBookTovCard(phoneBookEntries);
            Console.WriteLine(vCards.Count());

            mobileGateway.MessageStorage = MessageStorage.Phone;
            MessageMemoryStatus status = mobileGateway.MessageMemoryStatus;
            Console.WriteLine(status.ReadStorage.Used);

            PhoneBookEntry entry = new PhoneBookEntry();
            entry.Number = "0192292309";
            entry.Text = "messagingtoolkit";
            mobileGateway.AddPhoneBookEntry(entry, PhoneBookStorage.Sim);

            mobileGateway.DeletePhoneBook(PhoneBookStorage.Sim);

            PhoneBookEntry[] entries = mobileGateway.FindPhoneBookEntries("Testing", PhoneBookStorage.Phone);
            Console.WriteLine(entries.Count());
        }

      
        public void TestNetwork()
        {
            NetworkOperator networkOperator = mobileGateway.NetworkOperator;
            Console.WriteLine(networkOperator.OperatorInfo);

            SupportedNetworkOperator[] operatorList = mobileGateway.GetSupportedNetworkOperators();
            Console.WriteLine(operatorList.Count());

            Subscriber[] subscribers = mobileGateway.Subscribers;
            Console.WriteLine(subscribers.Count());            
        }


        
        public void TestSaveMessage()
        {
            mobileGateway.MessageStorage = MessageStorage.Phone;

            // Create a new SMS instance
            Sms sms = Sms.NewInstance();
            sms.DestinationAddress = "0192292309";
            sms.Content = "testing stored sent";

            mobileGateway.SaveMessage(sms, MessageStatusType.StoredSentMessages);
        }

        
        public void TestMessagePolling()
        {
            mobileGateway.MessageStorage = MessageStorage.Phone;
            List<MessageInformation> messages = mobileGateway.GetMessages(MessageStatusType.ReceivedReadMessages);
            foreach (MessageInformation message in messages)
            {
                mobileGateway.DeleteMessage(MessageDeleteOption.ByIndex, message.Index);
            }
        }

        [Test]
        public void TestInitDataConnection()
        {
            mobileGateway.InitializeDataConnection();
        }

        #endregion =============================================================

    }
 
}

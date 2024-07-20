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
    public class TestSms
    {
        [Test]
        public void TestEncodeDecode8Bit()
        {
            Sms sms = Sms.NewInstance();
            sms.ServiceCenterNumber = "+60193900000";
            sms.StatusReportRequest = MessageStatusReportRequest.SmsReportRequest;
            sms.DestinationAddress = "0192292309";
            sms.DataCodingScheme = MessageDataCodingScheme.EightBits;
            sms.DcsMessageClass = MessageClasses.Me;
            sms.ValidityPeriod = MessageValidPeriod.OneHour;
            sms.Content = "test";

           
            PduFactory messageFactory = PduFactory.NewInstance();
            string[] pduCodes = messageFactory.Generate(sms);
            Console.WriteLine(pduCodes[0]);
            MessageInformation messageInformation = messageFactory.Decode(pduCodes[0]);
            Console.WriteLine(messageInformation.Content);
            
        }


    }
}

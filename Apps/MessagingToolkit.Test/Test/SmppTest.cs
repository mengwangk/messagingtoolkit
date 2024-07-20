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

using MessagingToolkit.Core.Service;
using MessagingToolkit.Core.Smpp;
using MessagingToolkit.Core.Smpp.Packet;
using MessagingToolkit.Core.Smpp.Utility;
using MessagingToolkit.Core.Smpp.Packet.Request;
using MessagingToolkit.Core.Smpp.Packet.Response;
using MessagingToolkit.Core.Smpp.EventObjects;

using NUnit.Framework;

namespace MessagingToolkit.Core.Test
{
    [TestFixture]
    public class SmppTest
    {
        private ISmppGateway smppGateway = SmppGatewayFactory.Default;

        [SetUp]
        public void Setup()
        {
            MessageGateway<ISmppGateway, SmppGatewayConfiguration> messageGateway =
               MessageGateway<ISmppGateway, SmppGatewayConfiguration>.NewInstance();

            SmppGatewayConfiguration smppGatewayConfiguration = SmppGatewayConfiguration.NewInstance();
            smppGatewayConfiguration.Port = 2775;
            smppGatewayConfiguration.Host = "localhost";
            smppGatewayConfiguration.Password = "password";
            smppGatewayConfiguration.SystemId = "smppclient1";
            smppGatewayConfiguration.SystemType = "systemType";
            smppGatewayConfiguration.LogLevel = MessagingToolkit.Core.Log.LogLevel.Verbose;

            
            try
            {
                smppGateway = messageGateway.Find(smppGatewayConfiguration);
                smppGateway.Bind();
            }
            catch (GatewayException gex)
            {
                Assert.Fail(gex.Message);
                Assert.Fail(gex.ToString());
            }        
        }

        [TearDown]
        public void TearDown()
        {
            smppGateway.Unbind();
        }


        [Test]
        public void TestSendSms()
        {
            SmppSubmitSm submitSm = new SmppSubmitSm();
            
        }


    }
}

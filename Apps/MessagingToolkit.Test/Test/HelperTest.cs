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
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Feature;
using MessagingToolkit.Core.Helper;

namespace MessagingToolkit.Test.Test
{
    [TestFixture]
    public class HelperTest
    {

        [Test]
        public void TestConvertBitmap()
        {
            //GatewayHelper.ConvertBlackAndWhite(@"C:\MySpace\Personal\Koh Meng Wang.jpg", @"C:\MySpace\Personal\bw.jpg");
            string content = "C135BD1E66BBF3A0393DEC06ADDF6E7A580702E15CB01A0804653A5DA0E5DB4D7F83EE61BDBB0C22BF4164773A0C02C168AF98EC2583C562";
            byte[] responseEncodedSeptets = MessagingToolkit.Pdu.PduUtils.PduToBytes(content);
            byte[] responseUnencodedSeptets = MessagingToolkit.Pdu.PduUtils.EncodedSeptetsToUnencodedSeptets(responseEncodedSeptets);
            string decodedContent = MessagingToolkit.Pdu.PduUtils.UnencodedSeptetsToString(responseUnencodedSeptets);
            Console.WriteLine(decodedContent);
        }
    }
}

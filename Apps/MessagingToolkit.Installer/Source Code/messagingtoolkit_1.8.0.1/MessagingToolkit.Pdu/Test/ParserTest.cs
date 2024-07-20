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

using NUnit.Framework;

using MessagingToolkit.Pdu;
using MessagingToolkit.Pdu.Ie;
using MessagingToolkit.Pdu.WapPush;

namespace MessagingToolkit.Pdu.Test
{
    /// <summary>
    /// Test class for the PDU parser
    /// </summary>
    [TestFixture]
    public class ParserTest
    {
        private PduParser pduParser;
        private PduFactory pduFactory;
        private PduGenerator pduGenerator;

        [SetUp]
        public void Setup()
        {
            pduParser = new PduParser();
            pduFactory = new PduFactory();
            pduGenerator = new PduGenerator();
        }

        [TearDown]
        public void TearDown()
        {
        }


        [Test]
        public void TestDecode()
        {
            try
            {
                Pdu pdu;

                //pdu = pduParser.parsePdu("0791361907002004040C9136093631073200008040325191702321CCA235091256B32018AD068BB560B65AEE357B3589CA5750F90A2A5F39");
                //Console.WriteLine(pdu.DecodedText);

                //pdu = pduParser.parsePdu("0791361908001081040ED0D0F03CCC7C87C90000804032612074239BB1970C249BB58270392856D3CD72BA1A4D0795C140E8F01C242E97DD20BA3CEC9E9BCB7279990C32CBDF6D102C17C3CD6837D82C5603D1DF20582E869BC162381C0EE7020DDFEE79BD5D06A5DD20190D8496CF5DA0ECBB2E07B9CB7710F31D2683846176D83D2E83D27310540673C160A0B09B0C82818CF2721944C5D341CDF979EE0251D161F71A");
                //Console.WriteLine(pdu.DecodedText);
                
                //pdu = pduParser.parsePdu("079136190800104244038153F20011804042417280239B050003E60301A675F17C2C4FC3E9E9B71BB42EE7EF6F39790E7ABB41B39A4C070A4AA9CDE69435AC528B2C63D5390C4A892C63D519AD26B52CE4D359CCB29641E793B50C3A83AC65101AA42689AC65101AA42689CDE694B50C4A83D462B339652D83D220B548C55259CCA7B5C86A668FD52CAB993D26A54C96B468641687D46673CA92168DCCE210");
                //Console.WriteLine(pdu.DecodedText);

                //pdu = pduParser.parsePdu("079136190800104244038153F20011804042417290239B050003E60302A8582A6B1A3406A549D6949A651659D36BB148153EB3ACE9B558A41E9352266B9A7D5283C2679635CD3EA9C16352CA64519F55298B2A4D5A9341D635989CBA404F3ABA2C07ADCBF9FB5B4E9EEB404162B39A741A9F2CD0302825B24046AC0B44C5D341BC75397F7FCBC93E10B2C88482CC6F39085D66C3416F37280C5A97F3F7B71C");
                //Console.WriteLine(pdu.DecodedText);

                //pdu = pduParser.parsePdu("0051000B819060137320F300085F8B0C050415B300000804ED0903010033002000380020005400680061006E006B00200079006F007500200066006F00720020007500730069006E00670020007400680069007300200073006500720076006900630065002E002000200059006F007500720020007400720061006E00730061006300740069006F006E00200068006100730020006200650065");
                //Console.WriteLine(pdu.DecodedText);
                //pdu = pduParser.parsePdu("0791361907002004440C9136197773977800F48050827112512311060504D9030000000000023C746578743E");
                                
                //pdu = pduParser.ParsePdu("06050C9126182921223290115212858382901152128534820000000012");
                //pdu = pduParser.ParsePdu("060E0D91267868080099F7901172029301829011720293718200000012");
                pdu = pduParser.ParsePdu("07910691930000F04012D0C32273F86C829ACD2939F5011003804185238C0B05040B8423F000034902011406226170706C69636174696F6E2F766E642E7761702E6D6D732D6D65737361676500AF848C8298383236353738303438406D6D736332008D928918802B36303139323239323330392F545950453D504C4D4E008A808E0201FC8805810303F48083687474703A2F2F6D6D732E63656C636F6D2E6E65742E6D792F3F69643D38");
                
                if (pdu.Binary)
			    {
			        pdu.SetDataBytes(pdu.UserDataAsBytes);
			    }
                
                /*
                string generatedPduString = pduGenerator.GeneratePduString(pdu);			              
			    pdu = pduParser.ParsePdu(generatedPduString);
                */
                Console.WriteLine("Decoded text: " + pdu.DecodedText);
                Console.WriteLine(pdu);
			
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        
        public void TestEncode()
        {
            SmsSubmitPdu pdu = PduFactory.NewSmsSubmitPdu();
            pdu.SmscInfoLength = 0;          
            pdu.Address = "01921212121";
            pdu.AddressType = PduUtils.AddressTypeUnknown | PduUtils.AddressNumberPlanIdTelephone;

            string decodedText = "testing content";
            pdu.SetDataBytes(Encoding.ASCII.GetBytes(decodedText));
            pdu.ValidityPeriod = 8; // Validity period in hours
            pdu.ProtocolIdentifier = 0;
                    
                          
            //pdu.AddInformationElement(InformationElementFactory.GeneratePortInfo(5555, 0));

            pdu.AddReplyPath("0123456789", PduUtils.AddressTypeDomesticFormat);

            string pduString = pduGenerator.GeneratePduString(pdu);

            Console.WriteLine(pduString);
                    

        }

     
        public void TestAddReplyPath()
        {
            SmsSubmitPdu pdu = PduFactory.NewSmsSubmitPdu();
            pdu.AddReplyPath("01921212121", PduUtils.AddressTypeDomesticFormat);
        }


    }
}

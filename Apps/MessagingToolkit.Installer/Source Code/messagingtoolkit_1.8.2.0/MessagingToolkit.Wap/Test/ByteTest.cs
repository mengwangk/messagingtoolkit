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
using System.Collections;
using System.Collections.Specialized;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp.Headers;

using NUnit.Framework;

namespace MessagingToolkit.Wap.Test
{
    [TestFixture]
    public class ByteTest
    {
        [Test]
        public void TestWriteOutput()
        {
            /*
            BitArrayOutputStream m = new BitArrayOutputStream();
            m.Write(0x00 + 0x80, 8);
            m.WriteUIntVar(65535);
            byte[] b = m.ToByteArray();

            Console.WriteLine(m.ToByteArray());
            */

            /*
            byte value = 128;
            string binaryRepresentation = Convert.ToString(value, 2);
            Console.WriteLine(binaryRepresentation);

            string a = GetBitString(value);
            Console.WriteLine(a);
            */
            /*
            sbyte s = -128;
            byte b = (byte)s;
            Console.WriteLine(Convert.ToString(s, 2));
            Console.WriteLine(Convert.ToString(b, 2));    
            */


            /*
            byte[] b = new byte[] { 129 };
            BitArrayOutputStream m = new BitArrayOutputStream();
            m.Write(b);
            Console.WriteLine(b.ToString());
            */
            /*
            string value = "application/vnd.wap.mms-message";

            IEnumerator e = HeaderToken.Tokenize(value);
            if (e.MoveNext())
            {
                HeaderToken token = (HeaderToken)e.Current;
                string contentType = token.Token;
                Console.WriteLine(contentType);
            }
            */

            /*
            string line = "testing\r\n\testing\rtesting\n\testing\r\ntest";
            string[] lines = line.Split(new char[] { '\r', '\n' });

            Console.WriteLine(lines.Length);
            */

            string assemblyName = typeof(WAPClient).Assembly.GetName().Name;
            Console.WriteLine(assemblyName);
             
            
        }

    }
}

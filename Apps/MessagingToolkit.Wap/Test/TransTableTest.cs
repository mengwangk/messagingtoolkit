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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Globalization;

using NUnit.Framework;
using MessagingToolkit.Wap.Helper;

namespace MessagingToolkit.Wap.Test
{
    /// <summary>
    /// Test for TransTable
    /// </summary>
    [TestFixture]
    public class TransTableTest
    {
      
        public void TestGetTable()
        {
            TransTable table = TransTable.GetTable("http-status-codes");
            Console.WriteLine(table.ToString());
        }

        [Test]
        public void TestDecodeHex()
        {
            string key = "1F";
            int code = Int32.Parse(key, NumberStyles.AllowHexSpecifier);
            Console.WriteLine(code + "");
        }
    }
}

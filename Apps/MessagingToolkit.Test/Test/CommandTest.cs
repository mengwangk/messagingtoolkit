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
using System.Collections;
using System.Globalization;

using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile;
using NUnit.Framework;

namespace MessagingToolkit.Core.Test
{
    /// <summary>
    /// NUnit test for Command
    /// </summary>
    [TestFixture]
    public class CommandTest
    {

        /// <summary>
        /// Set up the test
        /// </summary>
        [SetUp]
        protected void SetUp()
        {
        }

        /// <summary>
        /// Test getting ASCII character
        /// </summary>
        [Test]
        public void TestFormat()
        {
            char c = Convert.ToChar(26);
            string s = Convert.ToString(26);
            Console.WriteLine(c);                

        }

        /// <summary>
        /// Test regular expression
        /// </summary>
        [Test]
        public void TestRegularExp()
        {
            Regex r = new Regex("OK", RegexOptions.Compiled |
                            RegexOptions.IgnoreCase | RegexOptions.Multiline |
                            RegexOptions.IgnorePatternWhitespace);
            string result = "sony ericsson\n\nok1\n";

            Console.WriteLine(r.IsMatch(result));
        }

        /// <summary>
        /// Test formatting
        /// </summary>
        [Test]
        public void TestFormatting()
        {
            string message = "Error {0}:{1}";
            Console.WriteLine(String.Format(message, "1", "unknown"));
        }

        [Test]
        public void TestParseResult()
        {
            string result = "\r\nSony Ericsson K610\r\n\r\nOK\r\n";
            string[] lines = result.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            List<string> filteredResult = new List<string>(2);
            foreach (string line in lines)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    filteredResult.Add(line);
                }
            }
            string[] filteredLines = filteredResult.ToArray();
            Console.WriteLine(result);
        }

        [Test]
        public void TestAscw()
        {
            Console.WriteLine("Test ascw");
            string val = "ABC";
            Console.WriteLine((int)(val.ToCharArray()[0]));
        }

        [Test]
        public void TestRegex()
        {
            Regex expectedResponse = new Regex("SIM PIN");

            string input = "+CPIN: SIM PxIN2";

            if (expectedResponse.IsMatch(input))
            {
                Console.WriteLine("matched");
            }
            else
            {
                Console.WriteLine("not matched");
            }
        }


        [Test]
        public void TestAlphaNumeric()
        {
            string address = "002B003900320033003300330030003000300035003100350030";

            if (string.IsNullOrEmpty(address))
            {
                return;
                // return false;
            }

            if (address.StartsWith("+"))
            {
                return;
                // return false;
            }

            // now we need to loop through the string, examining each character
            for (int i = 0; i < address.Length; i++)
            {
                // if this character isn't a letter and it isn't a number then return false
                // because it means this isn't a valid alpha numeric string
                if (!(char.IsNumber(address[i])))
                    //return false;
                    return;
            }
            // we made it this far so return true
            //return true;
            Console.WriteLine("is numeric");
        }

        [Test]
        public void TestParseDate()
        {            
            string[] DateFormats = new string[]{
                                   "M/d/yyyy H:m:s",
                                   "M/d/yyyy h:m:s",
                                   "M/d/yy H:m:s",
                                   "M/d/yy h:m:s"};
            string dateString = "1/22/2010 12:20:32";

            DateTime dt = DateTime.ParseExact(dateString, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            dt = dt.ToLocalTime();

            Console.WriteLine(dt);
        }

        [Test]
        public void TestNullEmpty()
        {
            string data = "test\r\ntest\rhaha\rtt";
            string[] lines = data.Split(new string[] { "\r\n" , "\r", "\n"}, StringSplitOptions.None);
            Console.WriteLine(lines.Length);
            if (!string.IsNullOrEmpty(data))
            {
                Console.WriteLine("Not Empty");
            }
            else
            {
                Console.WriteLine("Empty");
            }
        }

        

    }
}


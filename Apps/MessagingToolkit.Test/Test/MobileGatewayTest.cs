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

using MessagingToolkit.Core;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile;

namespace MessagingToolkit.Core.Test
{
    /// <summary>
    /// NUnit test for MobileGateway
    /// </summary>
    [TestFixture]
    internal class MobileGatewayTest
    {
        /// <summary>
        /// Set up the test
        /// </summary>
        [SetUp]
        protected void SetUp()
        {
        }

        /// <summary>
        /// </summary>
        [Test]
        public void testGetCmsErrorDescription()
        {
            Console.WriteLine(ResultParser.GetCmsErrorDescription("1"));
            Console.WriteLine(ResultParser.GetCmsErrorDescription("2"));

        }

        /// <summary>
        /// Check for unsolicited message
        /// </summary>
        /// <param name="handlers">Unsolicited message handlers</param>
        /// <param name="input"></param>
        private bool CheckUnsolicitedMessage(MessageIndicationHandlers handlers, ref string input)
        {
            string[] lines = input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string data = string.Empty;
            List<string> rawData = new List<string>();
            bool containsUnsolicitedMessage = false;

            foreach (string line in lines)
            {
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

                        //Logger.LogThis("Unsolicited message: " + description, LogLevel.Verbose);

                        // Raise event
                        //OnMessageReceived(messageIndicationObject);

                        // Reset
                        data = string.Empty;

                        containsUnsolicitedMessage = true;
                    }
                    else if (handlers.IsIncompleteUnsolicitedMessage(data))
                    {
                        // Need to check for next line
                        data += "\r\n";
                        containsUnsolicitedMessage = true;
                    }
                    else
                    {
                        rawData.Add(data);
                        data = string.Empty;
                    }
                }
            }

            if (!containsUnsolicitedMessage) return false;

            if (!string.IsNullOrEmpty(data))
            {
                string[] unprocessedList = data.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string line in unprocessedList)
                {
                    if (!string.IsNullOrEmpty(line))
                        rawData.Add(line);
                }
            }
            input = string.Empty;
            foreach (string line in rawData)
            {
                input += line + "\r\n";
            }
            return containsUnsolicitedMessage;
        }


        [Test]
        public void testCheckUnsolicitedMessage()
        {
            MessageIndicationHandlers handlers = new MessageIndicationHandlers();
            string input = "+CSCA: 0192222\r\n+CMTI: \"ME\",11\r\n+CSCA: 012121\r\n";
            CheckUnsolicitedMessage(handlers, ref input);
        }
    }
}

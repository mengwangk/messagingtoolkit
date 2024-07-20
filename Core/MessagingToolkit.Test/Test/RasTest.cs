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
using System.Collections.ObjectModel;
using System.Management;
using System.Net;

using NUnit.Framework;

using MessagingToolkit.Core.Ras;

namespace MessagingToolkit.Core.Test
{
    /// <summary>
    /// RAS testing
    /// </summary>
    [TestFixture]
    public class RasTest
    {

        private RasPhoneBook rasPhoneBook;
        private RasDialer rasDialer;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            rasPhoneBook = new RasPhoneBook();
            rasDialer = new RasDialer();
        }

        /// <summary>
        /// Tests the create dialup entry.
        /// </summary>
        public void TestGetRasDevices()
        {
            ReadOnlyCollection<RasDevice> devices = RasDevice.GetDevices();

            Console.WriteLine("Device count: " + devices.Count);
            foreach (RasDevice device in devices)
            {
                Console.WriteLine("Name: " + device.Name);
                Console.WriteLine("Type: " + device.DeviceType);
            }

            ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher("Select * from Win32_POTSModem");
            ManagementObjectCollection objectCollection = objectSearcher.Get();

            foreach (ManagementBaseObject obj in objectCollection)
            {
                Console.WriteLine("Name: " + obj["Name"]);
                Console.WriteLine("Model: " + obj["Model"]);
                /*
                Console.WriteLine(obj["AttachedTo"]);
                Console.WriteLine(obj["MaxBaudRateToSerialPort"]);
                Console.WriteLine(obj["ErrorControlForced"]);
                Console.WriteLine(obj["ErrorControlOff"]);
                Console.WriteLine(obj["ErrorControlOn"]);
                Console.WriteLine(obj["FlowControlHard"]);
                Console.WriteLine(obj["FlowControlOff"]);
                Console.WriteLine(obj["FlowControlSoft"]);
                Console.WriteLine(obj["InactivityScale"]);
                Console.WriteLine(obj["InactivityTimeout"]);   
                */
            }
        }

        /// <summary>
        /// Tests the get active IP address.
        /// </summary>
        [Test]
        public void TestGetActiveIPAddress()
        {
            Console.WriteLine(DateTime.Now);
            rasPhoneBook.Open();
            Console.WriteLine(DateTime.Now);
            string name = rasPhoneBook.Entries[1].Name;
            foreach (RasConnection connection in this.rasDialer.GetActiveConnections())
            {
                Console.WriteLine("Name: " + connection.EntryName);
                Console.WriteLine("Id: " + connection.EntryId);

                RasIPInfo ipAddresses = (RasIPInfo)connection.GetProjectionInfo(RasProjectionType.IP);
                if (ipAddresses != null)
                {
                    Console.WriteLine("Client: " + ipAddresses.IPAddress.ToString());
                    Console.WriteLine("Server: " + ipAddresses.ServerIPAddress.ToString());
                }
            }
        }


        /// <summary>
        /// Tests the create dialup entry.
        /// </summary>
        public void TestCreateDialupEntry()
        {
            rasPhoneBook.Open();

            RasEntry entry = RasEntry.CreateDialUpEntry("testing", "1234", RasDevice.GetDeviceByName("HUAWEI Mobile Connect - 3G Modem", RasDeviceType.Modem));
            // Add the new entry to the phone book.
            this.rasPhoneBook.Entries.Add(entry);

        }

        /// <summary>
        /// Tests the dial ras entry.
        /// </summary>
        public void TestDialRasEntry()
        {
            // This button will be used to dial the connection.
            this.rasDialer.EntryName = "testing";
            this.rasDialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);

            try
            {
                // NOTE: The entry MUST be in the phone book before the connection can be dialed.
                // Begin dialing the connection; this will raise events from the dialer instance.
                this.rasDialer.Dial(new NetworkCredential("username", "password"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

             
        public void TestGetUsbPort()
        {
            Console.WriteLine("\n\nThe Port names\n\n");

            try
            {
                int count = 0;
                ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("root\\WMI",
                "SELECT * FROM MSSerial_PortName");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("MSSerial_PortName & instance " + count++);
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("PortName: {0}", queryObj["PortName"]);
                    Console.WriteLine("InstanceName: {0}\n", queryObj["InstanceName"]);
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An error occurred while querying for WMI data: " + e.Message);
            }

            Console.WriteLine("\n\nNow the Base IO Address\n\n");

            try
            {
                int count = 0;
                ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("root\\WMI",
                "SELECT * FROM MSSerial_HardwareConfiguration");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("MSSerial_HardwareConfiguration & instance " + count++);
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("BaseIOAddress: {0}", queryObj["BaseIOAddress"]);
                    Console.WriteLine("InstanceName: {0}\n", queryObj["InstanceName"]);
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An error occurred while querying for WMI data: " + e.Message);
            }

        }

    }
}

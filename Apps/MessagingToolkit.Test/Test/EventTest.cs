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
using System.IO.Ports;
using System.Timers;

using System.Threading;
using NUnit.Framework;

using MessagingToolkit.Core.Mobile;

namespace MessagingToolkit.Core.Test
{
    [TestFixture]
    internal class EventTest
    {
        private SerialPort serialPort;

        private const string Port = "COM4";

        private SerialDataReceivedEventHandler onDataReceiveHandler;

        //private System.Timers.Timer timer;
        
        //public string data;

        private Thread watchDog;
        private Thread dataReader;


        [SetUp]
        public void Setup()
        {
            /*
            timer = new System.Timers.Timer(1000); ;
            timer.Elapsed += new  ElapsedEventHandler(tmrTimersTimer_Elapsed);
            timer.Start();
            */

            serialPort = new SerialPort();
            serialPort.PortName = Port;
            serialPort.Open();

            byte[] callCommand = new ASCIIEncoding().GetBytes("ATE0\r");
            serialPort.Write(callCommand, 0, callCommand.Length);       

        }

        /*
        private void tmrTimersTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            data = "ok";
        }
        */

        private void onDataReceived(Object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        [TearDown]
        public void TearDown()
        {
            serialPort.Close();

            /*
            watchDog.Interrupt();
            watchDog.Join();

            dataReader.Interrupt();
            dataReader.Join();
            */
        }

        [Test]
        public void testDateReceived()
        {
            /*
            watchDog = new Thread(new ThreadStart(new WatchDog(serialPort).Run));
            dataReader = new Thread(new ThreadStart(new GatewayDataReader(serialPort).Run));
            watchDog.Start();
            dataReader.Start();

            string output = serialPort.ReadExisting();

            Console.WriteLine(output);
            */

            byte[] callCommand = new ASCIIEncoding().GetBytes("AT+CMGS=23\r");
            serialPort.Write(callCommand, 0, callCommand.Length);

            Thread.Sleep(5000);

            string output = serialPort.ReadExisting();
            
            Console.WriteLine(output);

            callCommand = new ASCIIEncoding().GetBytes("AT\r");
            serialPort.Write(callCommand, 0, callCommand.Length);
            Thread.Sleep(5000);

            output = serialPort.ReadExisting();

            //Thread.Sleep(100000);

            
            /*
            serialPort = new SerialPort();
            serialPort.PortName = Port;
            serialPort.Open();

            onDataReceiveHandler = new SerialDataReceivedEventHandler(onDataReceived);
            serialPort.DataReceived += onDataReceiveHandler;

            serialPort.WriteLine("AT\r");
          
            serialPort.Close();
            */            
        }
    }

    public class WatchDog
    {
        private SerialPort serialPort;

        public WatchDog(SerialPort port)
        {
            serialPort = port;
        }

        public void Run()
        {
            while (serialPort.IsOpen)
            {
                byte[] callCommand = new ASCIIEncoding().GetBytes("AT+CSCA?\r");
                serialPort.Write(callCommand, 0, callCommand.Length);             
            }
        }
    }


    public class GatewayDataReader
    {
        private SerialPort serialPort;
        private IncomingDataQueue dataQueue;
        private int i = 0;

        public GatewayDataReader(SerialPort port)
        {
            serialPort = port;
            dataQueue = new IncomingDataQueue();   
        }

        public void Run()
        {
            while (serialPort.IsOpen)
            {
                if (serialPort.BytesToRead > 0)
                {
                    dataQueue.Enqueue(serialPort.ReadLine());
                    i++;
                }
            }
        }
    }
}

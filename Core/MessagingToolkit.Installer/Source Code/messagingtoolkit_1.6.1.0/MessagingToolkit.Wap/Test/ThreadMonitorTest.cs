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
using System.Collections;
using System.Threading;
using System.Reflection;
using System.Runtime.CompilerServices;

using NUnit.Framework;

namespace MessagingToolkit.Wap.Test
{
    [TestFixture]
    public class ThreadMonitorTest
    {
        private object sessionLock = new object();
        private Hashtable pendingRequests = Hashtable.Synchronized(new Hashtable());
        //private Hashtable pendingRequests = new Hashtable();
        //private object myLock = new object();

        
        [Test]
        public void TestWaitForCompletion()
        {
            //lock (this)
            //{
                WaitForCompletion(sessionLock, 30000);
            //}
        }

       
        private object WaitForCompletion(object key, long timeout)
        {

            object obj = null;
            long startAt = 0;

            if (timeout > 0)
            {
                startAt = (DateTime.Now.Ticks - 621355968000000000) / 10000;
            }
            while (obj == null)
            {
                if (timeout > 0 && (startAt + timeout) < (DateTime.Now.Ticks - 621355968000000000) / 10000)
                {
                    Console.WriteLine("Timeout occurred");
                    break;
                }
                /*
                lock (pendingRequests.SyncRoot)
                {
                    object tempObject;
                    tempObject = pendingRequests[key];
                    pendingRequests.Remove(key);
                    obj = tempObject;
                    if (obj == null)
                    {
                */
                        try
                        {
                            lock (pendingRequests)
                            {
                                Monitor.Wait(pendingRequests, TimeSpan.FromMilliseconds(timeout));
                            }
                        }
                        catch (ThreadInterruptedException e)
                        {
                            Console.WriteLine("Interrupted");
                        }
            /*        
            }
                }
            */
                
            }
            return obj;

        }



    }
}

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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MessagingToolkit.Core.Service
{
    /// <summary>
    /// Gateway status
    /// </summary>
    public enum GatewayStatus
    {
        /// <summary>
        /// Stopped
        /// </summary>
        [StringValue("Stopped")]
        Stopped,
        /// <summary>
        /// Stopping
        /// </summary>
        [StringValue("Stopping")]
        Stopping,
        /// <summary>
        /// Starting
        /// </summary>
        [StringValue("Starting")]
        Starting,
        /// <summary>
        /// Started
        /// </summary>
        [StringValue("Started")]
        Started,
        /// <summary>
        /// Failure
        /// </summary>
        [StringValue("Failed")]
        Failed,
        /// <summary>
        /// Restart 
        /// </summary>
        [StringValue("Restart")]
        Restart
    }

    /// <summary>
    /// Gateway attribute
    /// </summary>
    [Flags]
    public enum GatewayAttribute
    {
        /// <summary>
        /// Send message
        /// </summary>
        Send = 0x01,
        /// <summary>
        /// Receive message through trigger based mechanism
        /// </summary>
        ReceiveByTrigger = 0x02,
        /// <summary>
        /// Receive message by polling from the gateway
        /// </summary>
        ReceiveByPolling = 0x04,
        /// <summary>
        /// Support long message
        /// </summary>
        LongMessage = 0x08,
        /// <summary>
        /// Support WAP push
        /// </summary>
        WapPush = 0x16,
        /// <summary>
        /// Support Smart SMS
        /// </summary>
        SmartSms = 0x32,
        /// <summary>
        /// Support flash/alert message
        /// </summary>
        FlashSms = 0x48,
        /// <summary>
        /// Request for delivery report
        /// </summary>
        DeliveryReport = 0x64,
    }

    /// <summary>
    /// Object cloning
    /// </summary>
    public class ObjectClone
    {
        /// <summary>
        /// Deep copy of object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T item)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, item);
            stream.Seek(0, SeekOrigin.Begin);
            T result = (T)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }
    }
}

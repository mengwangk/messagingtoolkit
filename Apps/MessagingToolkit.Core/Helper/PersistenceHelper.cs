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
using System.Runtime.Serialization.Formatters.Binary;

using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Helper
{
    /// <summary>
    /// Helper class for message persistence
    /// </summary>
    internal static class PersistenceHelper
    {
        /// <summary>
        /// Serializes the message.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool SerializeMessage(string path, IMessage message)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = Path.Combine(path, message.Identifier);
                if (File.Exists(path))
                {
                    return true;
                }

                Stream stream = File.Open(filePath, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, message);
                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogThis(string.Format("Serialization error: {0}", ex.Message), LogLevel.Error);                
            }
            return false;
        }

        /// <summary>
        /// Deserializes the message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static T DeserializeMessage<T>(string fileName)
        {
            try
            {
                if (!File.Exists(fileName)) return default(T);

                Stream stream = File.Open(fileName, FileMode.Open);
                BinaryFormatter bformatter = new BinaryFormatter();
                T obj = (T)bformatter.Deserialize(stream);
                stream.Close();
                return obj;
            }
            catch (Exception ex)
            {
                Logger.LogThis(string.Format("Deserialization error: {0}", ex.Message), LogLevel.Error);                
            }
            return default(T);
        }
    }
}

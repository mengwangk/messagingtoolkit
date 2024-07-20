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
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Helper;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Delay message queue implementation
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    internal class DelayedMessageQueue: List<IMessage>
    {

        /// <summary>
        /// Lock object
        /// </summary>
        private readonly object queueLock = new object();

        /// <summary>
        /// Message folder
        /// </summary>
        private string messageFolder;
        
        /// <summary>
        /// SMS folder
        /// </summary>
        private string smsFolder;

        /// <summary>
        /// MMS folder
        /// </summary>
        private string mmsFolder;

        /// <summary>
        /// Indicate if the queue is initialized
        /// </summary>
        private bool isLoaded;

        /// <summary>
        /// Indicate if the queue will be persisted
        /// </summary>
        private bool isPersistence;

        /// <summary>
        /// Persistence base folder
        /// </summary>
        private string persistenceFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedMessageQueue"/> class.
        /// </summary>
        public DelayedMessageQueue()
            : base()
        {
            // Default to not persist
            this.Persist = false;
            this.isLoaded = false;
            this.persistenceFolder = string.Empty;

            // Set the base message store folder
            Init(string.Empty);
            //this.messageFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PersistenceQueue.DelayedQueueFolder);

            //this.smsFolder = Path.Combine(this.messageFolder, PersistenceQueue.SmsFolder); 
            //this.mmsFolder = Path.Combine(this.messageFolder, PersistenceQueue.MmsFolder);
        }

        /// <summary>
        /// Count of dued messages
        /// </summary>
        /// <returns>Dued messages</returns>
        public int CountDue()
        {
            DateTime now = DateTime.Now;
            lock (queueLock)
            {
                return this.Count(message => (message.ScheduledDeliveryDate != null && message.ScheduledDeliveryDate <= now));
            }
        }

        /// <summary>
        /// Due message
        /// </summary>
        /// <returns>Message</returns>
        public IMessage DueMessage
        {
            get 
            {
                DateTime now = DateTime.Now;
                int index = -1;
                IMessage message = null;
                lock (queueLock)
                {
                    for (int i = 0; i < this.Count(); i++)
                    {
                        if (this[i].ScheduledDeliveryDate != null && this[i].ScheduledDeliveryDate <= now)
                        {
                            index = i;
                            message = this[i];
                            break;
                        }
                    }

                    if (index > -1)
                    {
                        this.RemoveAt(index);
                        RemovePersistedStorage(message);
                        return message;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Queues the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void QueueMessage(IMessage message)
        {
            if (this.Persist)
            {
                message.Persisted = true;
                if (message is Mms)
                {
                    PersistenceHelper.SerializeMessage(mmsFolder, message);
                    //ObjectXmlSerializer<Mms>.Save((Mms)message, Path.Combine(mmsFolder, message.Identifier));
                }
                else
                {                   
                    PersistenceHelper.SerializeMessage(smsFolder, message);
                    //ObjectXmlSerializer<Sms>.Save((Sms)message, Path.Combine(smsFolder, message.Identifier));
                }                
            }

            lock (queueLock)
            {
                this.Add(message);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DelayedMessageQueue"/> is persist.
        /// Default to false.
        /// </summary>
        /// <value><c>true</c> if persist; otherwise, <c>false</c>.</value>
        public bool Persist
        {
            get 
            {
                return this.isPersistence;
            }
            set
            {
                this.isPersistence = value;
                if (this.isPersistence && !this.isLoaded)
                {
                    Load();
                }
            }
        }

        /// <summary>
        /// Gets or sets the persistence folder.
        /// </summary>
        /// <value>The persistence folder.</value>
        public string PersistFolder
        {
            get
            {
                return this.persistenceFolder;
            }
            set
            {
                this.persistenceFolder = value;
                Init(this.persistenceFolder);
            }
        }


        /// <summary>
        /// Loads messages from the persistence store
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {           
            if (Persist)
            {
                try
                {
                    if (Directory.Exists(smsFolder))
                    {
                        string[] fileNames = Directory.GetFiles(smsFolder);
                        foreach (string fn in fileNames)
                        {
                            Sms message = PersistenceHelper.DeserializeMessage<Sms>(fn);
                            //Sms message = ObjectXmlSerializer<Sms>.Load(fn);
                            if (message == null) continue;
                            lock (queueLock)
                            {
                                this.Add(message);
                            }
                        }
                    }

                    if (Directory.Exists(mmsFolder))
                    {
                        string[] fileNames = Directory.GetFiles(mmsFolder);
                        foreach (string fn in fileNames)
                        {
                            Mms message = PersistenceHelper.DeserializeMessage<Mms>(fn);
                            //Mms message = ObjectXmlSerializer<Mms>.Load(fn);
                            if (message == null) continue;
                            lock (queueLock)
                            {
                                this.Add(message);
                            }
                        }
                    }
                    this.isLoaded = true;
                }
                catch (Exception ex)
                {
                    Logger.LogThis(string.Format("Delayed queue load error: {0}", ex.Message), LogLevel.Error); 
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Inits the specified base directory.
        /// </summary>
        /// <param name="baseDirectory">The base directory.</param>
        private void Init(string baseDirectory)
        {
            if (string.IsNullOrEmpty(baseDirectory))
            {
                // Set the base message store folder
                this.messageFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PersistenceQueue.DelayedQueueFolder);
            }
            else
            {
                // Set the base message store folder
                this.messageFolder = Path.Combine(baseDirectory, PersistenceQueue.DelayedQueueFolder);
                if (!Directory.Exists(this.messageFolder) && this.Persist)
                {
                    Directory.CreateDirectory(this.messageFolder);
                }               
            }

            this.smsFolder = Path.Combine(this.messageFolder, PersistenceQueue.SmsFolder);
            this.mmsFolder = Path.Combine(this.messageFolder, PersistenceQueue.MmsFolder);
        }

        /// <summary>
        /// Removes the persisted file.
        /// </summary>
        /// <returns>true if removal is successful, otherwise returns false</returns>
        private bool RemovePersistedStorage(IMessage message)
        {
            if (message.Persisted)
            {
                try
                {
                    string fileName = string.Empty;
                    if (message is Mms)
                    {
                        fileName = Path.Combine(mmsFolder, message.Identifier);

                    }
                    else
                    {
                        fileName = Path.Combine(smsFolder, message.Identifier);
                    }

                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    message.Persisted = false;
                }
                catch (Exception ex)
                {
                    Logger.LogThis(string.Format("Remove persistence storage error: {0}", ex.Message), LogLevel.Error);
                    return false;
                }
            }
            return true;
        }
    }
}
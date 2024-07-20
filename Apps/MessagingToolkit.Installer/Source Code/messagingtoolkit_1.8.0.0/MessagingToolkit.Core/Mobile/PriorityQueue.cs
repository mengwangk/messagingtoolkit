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
using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile.Http;


namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Priority queue item for message sending
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TPriority">The type of the priority.</typeparam>
    [Serializable]
    [ComVisible(false)]
    internal struct PriorityQueueItem<TValue, TPriority> where TValue : IMessage
    {
        private TValue value;
        private TPriority priority;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public TValue Value
        {
            get { return value; }
        }


        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public TPriority Priority
        {
            get { return priority; }
        }

        internal PriorityQueueItem(TValue val, TPriority pri)
        {
            this.value = val;
            this.priority = pri;
        }
    }

    /// <summary>
    /// Priority queue for message sending
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TPriority">The type of the priority.</typeparam>
    [Serializable]
    [ComVisible(false)]
    internal class PriorityQueue<TValue, TPriority> : ICollection,
        IEnumerable<PriorityQueueItem<TValue, TPriority>>
        where TValue : IMessage
    {
        private PriorityQueueItem<TValue, TPriority>[] items;

        private const Int32 DefaultCapacity = 16;
        private Int32 capacity;
        private Int32 numItems;

        private Comparison<TPriority> compareFunc;

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
        /// The HTTP message folder
        /// </summary>
        private string httpMessageFolder;

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
        /// The persisted message type.
        /// </summary>
        private PersistedMessageType persistedMessageType;


        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the default initial capacity, and uses the default IComparer.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        public PriorityQueue(PersistedMessageType messageType)
            : this(DefaultCapacity, Comparer<TPriority>.Default, messageType)
        {
            Init(string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue&lt;TValue, TPriority&gt;" /> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="messageType">Type of the message.</param>
        public PriorityQueue(Int32 initialCapacity, PersistedMessageType messageType)
            : this(initialCapacity, Comparer<TPriority>.Default, messageType)
        {
            Init(string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue&lt;TValue, TPriority&gt;" /> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <param name="messageType">Type of the message.</param>
        public PriorityQueue(IComparer<TPriority> comparer, PersistedMessageType messageType)
            : this(DefaultCapacity, comparer, messageType)
        {
            Init(string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue&lt;TValue, TPriority&gt;"/> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="comparer">The comparer.</param>
        public PriorityQueue(int initialCapacity, IComparer<TPriority> comparer, PersistedMessageType messageType)
        {
            Init(initialCapacity, new Comparison<TPriority>(comparer.Compare));
            this.persistedMessageType = messageType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue&lt;TValue, TPriority&gt;" /> class.
        /// </summary>
        /// <param name="comparison">The comparison.</param>
        /// <param name="messageType">Type of the message.</param>
        public PriorityQueue(Comparison<TPriority> comparison, PersistedMessageType messageType)
            : this(DefaultCapacity, comparison, messageType)
        {
            Init(string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityQueue&lt;TValue, TPriority&gt;"/> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="comparison">The comparison.</param>
        public PriorityQueue(int initialCapacity, Comparison<TPriority> comparison, PersistedMessageType messageType)
        {
            Init(initialCapacity, comparison);
            this.persistedMessageType = messageType;
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
                SetupDirectory(this.persistenceFolder);
            }
        }

        /// <summary>
        /// Loads messages from the persistence store
        /// </summary>
        /// <returns></returns>
        public List<IMessage> Load()
        {
            List<IMessage> messages = new List<IMessage>();
            if (this.isLoaded) return messages;
            if (Persist)
            {
                try
                {
                    /***
                    if (Directory.Exists(smsFolder))
                    {
                        string[] fileNames = Directory.GetFiles(smsFolder);
                        foreach (string fn in fileNames)
                        {
                            Sms message = PersistenceHelper.DeserializeMessage<Sms>(fn);
                            if (message == null) continue;
                            messages.Add(message);

                        }
                    }

                    if (Directory.Exists(mmsFolder))
                    {
                        string[] fileNames = Directory.GetFiles(mmsFolder);
                        foreach (string fn in fileNames)
                        {
                            Mms message = PersistenceHelper.DeserializeMessage<Mms>(fn);
                            if (message == null) continue;
                            messages.Add(message);
                        }
                    }

                    if (Directory.Exists(httpMessageFolder))
                    {
                        string[] fileNames = Directory.GetFiles(httpMessageFolder);
                        foreach (string fn in fileNames)
                        {
                            PostMessage message = PersistenceHelper.DeserializeMessage<PostMessage>(fn);
                            if (message == null) continue;
                            messages.Add(message);
                        }
                    }
                    **/
                    LoadPersistedMessage<Sms>(messages, smsFolder);
                    LoadPersistedMessage<Mms>(messages, mmsFolder);
                    LoadPersistedMessage<PostMessage>(messages, httpMessageFolder);
                    this.isLoaded = true;
                }
                catch (Exception ex)
                {
                    this.isLoaded = false;
                    Logger.LogThis(string.Format("Persistence queue load error: {0}", ex.Message), LogLevel.Error);
                }
            }
            return messages;
        }


        /// <summary>
        /// Loads messages from the persistence store
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns></returns>
        public List<IMessage> Load(PersistedMessageType messageType)
        {
            List<IMessage> messages = new List<IMessage>();
            if (this.isLoaded) return messages;
            if (Persist)
            {
                try
                {
                    if (HasMessageType(messageType, PersistedMessageType.Sms))
                    {
                        LoadPersistedMessage<Sms>(messages, smsFolder);
                    }

                    if (HasMessageType(messageType, PersistedMessageType.Mms))
                    {
                        LoadPersistedMessage<Mms>(messages, mmsFolder);
                    }

                    if (HasMessageType(messageType, PersistedMessageType.Http))
                    {
                        LoadPersistedMessage<PostMessage>(messages, httpMessageFolder);
                    }

                    this.isLoaded = true;
                }
                catch (Exception ex)
                {
                    this.isLoaded = false;
                    Logger.LogThis(string.Format("Persistence queue load error: {0}", ex.Message), LogLevel.Error);
                }
            }
            return messages;
        }

        /// <summary>
        /// Determines if the message type enum is available.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="toFind">To find.</param>
        /// <returns></returns>
        private bool HasMessageType(PersistedMessageType messageType, PersistedMessageType toFind)
        {
            return ((messageType & toFind) == toFind);
        }


        /// <summary>
        /// Loads the persisted message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messages">The messages.</param>
        /// <param name="folder">The folder.</param>
        private void LoadPersistedMessage<T>(List<IMessage> messages, string folder) where T : IMessage
        {
            if (Directory.Exists(folder))
            {
                string[] fileNames = Directory.GetFiles(folder);
                foreach (string fn in fileNames)
                {
                    T message = PersistenceHelper.DeserializeMessage<T>(fn);
                    if (message == null) continue;
                    messages.Add(message);
                }
            }
        }

        /// <summary>
        /// Removes the persisted file.
        /// </summary>
        /// <returns>true if removal is successful, otherwise returns false</returns>
        private bool RemovePersistedStorage(TValue message)
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
                    else if (message is PostMessage)
                    {
                        fileName = Path.Combine(httpMessageFolder, message.Identifier);
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


        /// <summary>
        /// Inits the specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <param name="comparison">The comparison.</param>
        private void Init(int initialCapacity, Comparison<TPriority> comparison)
        {
            numItems = 0;
            compareFunc = comparison;
            SetCapacity(initialCapacity);
            Init(string.Empty);
        }


        /// <summary>
        /// Inits this instance.
        /// </summary>
        private void Init(string baseDirectory)
        {
            // Default to not persist
            this.Persist = false;
            this.isLoaded = false;
            SetupDirectory(baseDirectory);
        }


        /// <summary>
        /// Setups the directory.
        /// </summary>
        /// <param name="baseDirectory">The base directory.</param>
        private void SetupDirectory(string baseDirectory)
        {
            if (string.IsNullOrEmpty(baseDirectory))
            {
                // Set the base message store folder
                this.messageFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PersistenceQueue.QueueFolder);

            }
            else
            {
                // Set the base message store folder
                this.messageFolder = Path.Combine(baseDirectory, PersistenceQueue.QueueFolder);
                if (!Directory.Exists(this.messageFolder) && this.Persist)
                {
                    Directory.CreateDirectory(this.messageFolder);
                }
            }

            this.smsFolder = Path.Combine(this.messageFolder, PersistenceQueue.SmsFolder);
            this.mmsFolder = Path.Combine(this.messageFolder, PersistenceQueue.MmsFolder);
            this.httpMessageFolder = Path.Combine(this.messageFolder, PersistenceQueue.HttpMessageFolder);
        }



        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
        /// </returns>
        public int Count
        {
            get { return numItems; }
        }

        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        public int Capacity
        {
            get { return items.Length; }
            set { SetCapacity(value); }
        }

        /// <summary>
        /// Sets the capacity.
        /// </summary>
        /// <param name="newCapacity">The new capacity.</param>
        private void SetCapacity(int newCapacity)
        {
            int newCap = newCapacity;
            if (newCap < DefaultCapacity)
                newCap = DefaultCapacity;

            // throw exception if newCapacity < NumItems
            if (newCap < numItems)
                throw new ArgumentOutOfRangeException("newCapacity", "New capacity is less than Count");

            this.capacity = newCap;
            if (items == null)
            {
                items = new PriorityQueueItem<TValue, TPriority>[newCap];
                return;
            }

            // Resize the array.
            Array.Resize<PriorityQueueItem<TValue, TPriority>>(ref items, newCap);
        }

        /// <summary>
        /// Enqueues the specified new item.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        public void Enqueue(PriorityQueueItem<TValue, TPriority> newItem)
        {
            if (numItems == capacity)
            {
                // need to increase capacity
                // grow by 50 percent
                SetCapacity((3 * Capacity) / 2);
            }

            int i = numItems;
            ++numItems;
            while ((i > 0) && (compareFunc(items[(i - 1) / 2].Priority, newItem.Priority) < 0))
            {
                items[i] = items[(i - 1) / 2];
                i = (i - 1) / 2;
            }
            items[i] = newItem;


            // Persist the message
            if (this.Persist)
            {
                TValue message = newItem.Value;
                message.Persisted = true;
                if (message is Mms)
                {
                    PersistenceHelper.SerializeMessage(mmsFolder, message);
                }
                else if (message is PostMessage)
                {
                    PersistenceHelper.SerializeMessage(httpMessageFolder, message);
                }
                else
                {
                    PersistenceHelper.SerializeMessage(smsFolder, message);
                }
            }


            //if (!VerifyQueue())
            //{
            //    Console.WriteLine("ERROR: Queue out of order!");
            //}
        }

        /// <summary>
        /// Enqueues the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="priority">The priority.</param>
        public void Enqueue(TValue value, TPriority priority)
        {
            Enqueue(new PriorityQueueItem<TValue, TPriority>(value, priority));
        }

        private PriorityQueueItem<TValue, TPriority> RemoveAt(Int32 index)
        {
            PriorityQueueItem<TValue, TPriority> o = items[index];
            --numItems;
            // move the last item to fill the hole
            PriorityQueueItem<TValue, TPriority> tmp = items[numItems];
            // If you forget to clear this, you have a potential memory leak.
            items[numItems] = default(PriorityQueueItem<TValue, TPriority>);
            if (numItems > 0 && index != numItems)
            {
                // If the new item is greater than its parent, bubble up.
                int i = index;
                int parent = (i - 1) / 2;
                while (compareFunc(tmp.Priority, items[parent].Priority) > 0)
                {
                    items[i] = items[parent];
                    i = parent;
                    parent = (i - 1) / 2;
                }

                // if i == index, then we didn't move the item up
                if (i == index)
                {
                    // bubble down ...
                    while (i < (numItems) / 2)
                    {
                        int j = (2 * i) + 1;
                        if ((j < numItems - 1) && (compareFunc(items[j].Priority, items[j + 1].Priority) < 0))
                        {
                            ++j;
                        }
                        if (compareFunc(items[j].Priority, tmp.Priority) <= 0)
                        {
                            break;
                        }
                        items[i] = items[j];
                        i = j;
                    }
                }
                // Be sure to store the item in its place.
                items[i] = tmp;
            }
            //if (!VerifyQueue())
            //{
            //    Console.WriteLine("ERROR: Queue out of order!");
            //}
            return o;
        }

        // Function to check that the queue is coherent.
        public bool VerifyQueue()
        {
            int i = 0;
            while (i < numItems / 2)
            {
                int leftChild = (2 * i) + 1;
                int rightChild = leftChild + 1;
                if (compareFunc(items[i].Priority, items[leftChild].Priority) < 0)
                {
                    return false;
                }
                if (rightChild < numItems && compareFunc(items[i].Priority, items[rightChild].Priority) < 0)
                {
                    return false;
                }
                ++i;
            }
            return true;
        }

        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <returns></returns>
        public PriorityQueueItem<TValue, TPriority> Dequeue()
        {
            if (Count == 0)
                throw new InvalidOperationException("The queue is empty");

            PriorityQueueItem<TValue, TPriority> item = RemoveAt(0);
            RemovePersistedStorage(item.Value);
            return item;
        }


        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">The queue is empty</exception>
        public PriorityQueueItem<TValue, TPriority> Dequeue(PriorityQueueItem<TValue, TPriority> item)
        {
            if (Count == 0)
                throw new InvalidOperationException("The queue is empty");

            Remove(item.Value);
            RemovePersistedStorage(item.Value);
            return item;
        }

        /// <summary>
        /// Removes the item with the specified value from the queue.
        /// The passed equality comparison is used.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <param name="comparer">An object that implements the IEqualityComparer interface
        /// for the type of item in the collection.</param>
        public void Remove(TValue item, IEqualityComparer comparer)
        {
            // need to find the PriorityQueueItem that has the Data value of o
            for (int index = 0; index < numItems; ++index)
            {
                if (comparer.Equals(item, items[index].Value))
                {
                    RemoveAt(index);
                    return;
                }
            }
            throw new Exception("The specified item is not in the queue.");
        }

        /// <summary>
        /// Removes the item with the specified value from the queue.
        /// The default type comparison function is used.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        public void Remove(TValue item)
        {
            Remove(item, EqualityComparer<TValue>.Default);
        }

        public PriorityQueueItem<TValue, TPriority> Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException("The queue is empty");
            return items[0];
        }

        // Clear
        public void Clear()
        {
            for (int i = 0; i < numItems; ++i)
            {
                items[i] = default(PriorityQueueItem<TValue, TPriority>);
            }
            numItems = 0;
            TrimExcess();

            // Clear the persistence storage as well

        }

        /// <summary>
        /// Set the capacity to the actual number of items, if the current
        /// number of items is less than 90 percent of the current capacity.
        /// </summary>
        public void TrimExcess()
        {
            if (numItems < (float)0.9 * capacity)
            {
                SetCapacity(numItems);
            }
        }

        // Contains
        public bool Contains(TValue o)
        {
            foreach (PriorityQueueItem<TValue, TPriority> x in items)
            {
                if (x.Value.Equals(o))
                    return true;
            }
            return false;
        }

        public void CopyTo(PriorityQueueItem<TValue, TPriority>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
            if (array.Rank > 1)
                throw new ArgumentException("array is multidimensional.");
            if (numItems == 0)
                return;
            if (arrayIndex >= array.Length)
                throw new ArgumentException("arrayIndex is equal to or greater than the length of the array.");
            if (numItems > (array.Length - arrayIndex))
                throw new ArgumentException("The number of elements in the source ICollection is greater than the available space from arrayIndex to the end of the destination array.");

            for (int i = 0; i < numItems; i++)
            {
                array[arrayIndex + i] = items[i];
            }
        }

        #region =================================== ICollection Members ===========================

        public void CopyTo(Array array, int index)
        {
            this.CopyTo((PriorityQueueItem<TValue, TPriority>[])array, index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return items.SyncRoot; }
        }

        #endregion =================================================================================

        #region ================ IEnumerable<PriorityQueueItem<TValue,TPriority>> Members ==========

        public IEnumerator<PriorityQueueItem<TValue, TPriority>> GetEnumerator()
        {
            for (int i = 0; i < numItems; i++)
            {
                yield return items[i];
            }
        }

        #endregion =================================================================================

        #region ==================================== IEnumerable Members ===========================

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion =================================================================================
    }
}

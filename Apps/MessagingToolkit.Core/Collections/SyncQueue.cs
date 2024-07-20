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

namespace MessagingToolkit.Core.Collections
{

    /// <summary>
    /// Synchronized wrapper around the unsynchronized ArrayList class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SyncQueue<T>
    {
        private Queue<T> queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncQueue{T}"/> class.
        /// </summary>
        public SyncQueue()
        {
            queue = new Queue<T>();

        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            lock (queue)
            {
                queue.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get
            {
                lock (queue)
                {
                    return queue.Count;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            lock (queue)
            {
                return queue.Dequeue();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Enqueue(T obj)
        {
            lock (queue)
            {
                queue.Enqueue(obj);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            lock (queue)
            {
                return queue.Peek();
            }
        }

    }
}

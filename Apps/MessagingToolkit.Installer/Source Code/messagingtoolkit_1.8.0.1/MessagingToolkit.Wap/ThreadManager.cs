﻿//===============================================================================
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
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MessagingToolkit.Wap
{
    /// <summary>
    /// Thread manager 
    /// </summary>
    public class ThreadManager : IThreadRunnable
    {
        /// <summary>
        /// The instance of Thread
        /// </summary>
        private Thread threadField;

        /// <summary>
        /// Initializes a new instance of the ThreadClass class
        /// </summary>
        public ThreadManager()
        {
            threadField = new Thread(new ThreadStart(Run));
        }

        /// <summary>
        /// Initializes a new instance of the Thread class.
        /// </summary>
        /// <param name="Name">The name of the thread</param>
        public ThreadManager(string Name)
        {
            threadField = new Thread(new ThreadStart(Run));
            this.Name = Name;
        }

        /// <summary>
        /// Initializes a new instance of the Thread class.
        /// </summary>
        /// <param name="Start">A ThreadStart delegate that references the methods to be invoked when this thread begins executing</param>
        public ThreadManager(ThreadStart Start)
        {
            threadField = new Thread(Start);
        }

        /// <summary>
        /// Initializes a new instance of the Thread class.
        /// </summary>
        /// <param name="Start">A ThreadStart delegate that references the methods to be invoked when this thread begins executing</param>
        /// <param name="Name">The name of the thread</param>
        public ThreadManager(ThreadStart Start, string Name)
        {
            threadField = new Thread(Start);
            this.Name = Name;
        }

        /// <summary>
        /// This method has no functionality unless the method is overridden
        /// </summary>
        public virtual void Run()
        {
        }

        /// <summary>
        /// Causes the operating system to change the state of the current thread instance to ThreadState.Running
        /// </summary>
        public virtual void Start()
        {
            threadField.Start();
        }

        /// <summary>
        /// Interrupts a thread that is in the WaitSleepJoin thread state
        /// </summary>
        public virtual void Interrupt()
        {
            threadField.Interrupt();
        }

        /// <summary>
        /// Gets the current thread instance
        /// </summary>
        public Thread Instance
        {
            get
            {
                return threadField;
            }
            set
            {
                threadField = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the thread
        /// </summary>
        public string Name
        {
            get
            {
                return threadField.Name;
            }
            set
            {
                if (threadField.Name == null)
                    threadField.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the scheduling priority of a thread
        /// </summary>
        public ThreadPriority Priority
        {
            get
            {
                return threadField.Priority;
            }
            set
            {
                threadField.Priority = value;
            }
        }

        /// <summary>
        /// Gets a value indicating the execution status of the current thread
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return threadField.IsAlive;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a thread is a background thread.
        /// </summary>
        public bool IsBackground
        {
            get
            {
                return threadField.IsBackground;
            }
            set
            {
                threadField.IsBackground = value;
            }
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates
        /// </summary>
        public void Join()
        {
            threadField.Join();
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates or the specified time elapses
        /// </summary>
        /// <param name="MiliSeconds">Time of wait in milliseconds</param>
        public void Join(long MiliSeconds)
        {
            lock (this)
            {
                threadField.Join(new System.TimeSpan(MiliSeconds * 10000));
            }
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates or the specified time elapses
        /// </summary>
        /// <param name="MiliSeconds">Time of wait in milliseconds</param>
        /// <param name="NanoSeconds">Time of wait in nanoseconds</param>
        public void Join(long MiliSeconds, int NanoSeconds)
        {
            lock (this)
            {
                threadField.Join(new System.TimeSpan(MiliSeconds * 10000 + NanoSeconds * 100));
            }
        }


        /// <summary>
        /// Raises a ThreadAbortException in the thread on which it is invoked, 
        /// to begin the process of terminating the thread. Calling this method 
        /// usually terminates the thread
        /// </summary>
        public void Abort()
        {
            threadField.Abort();
        }

        /// <summary>
        /// Raises a ThreadAbortException in the thread on which it is invoked, 
        /// to begin the process of terminating the thread while also providing
        /// exception information about the thread termination. 
        /// Calling this method usually terminates the thread.
        /// </summary>
        /// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted</param>
        public void Abort(object stateInfo)
        {
            lock (this)
            {
                threadField.Abort(stateInfo);
            }
        }

        /// <summary>
        /// Obtain a String that represents the current Object
        /// </summary>
        /// <returns>A String that represents the current Object</returns>
        public override string ToString()
        {
            return "Thread[" + Name + "," + Priority.ToString() + "," + "" + "]";
        }

        /// <summary>
        /// Gets the currently running thread
        /// </summary>
        /// <returns>The currently running thread</returns>
        public static ThreadManager Current()
        {
            ThreadManager CurrentThread = new ThreadManager();
            CurrentThread.Instance = Thread.CurrentThread;
            return CurrentThread;
        }
    }

}

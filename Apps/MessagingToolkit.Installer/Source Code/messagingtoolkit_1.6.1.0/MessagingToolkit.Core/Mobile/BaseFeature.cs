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
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

using MessagingToolkit.Core.Properties;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Base class for all gateway feature
    /// </summary>
    /// <typeparam name="T">Derived feature</typeparam>
    internal abstract class BaseFeature<T>
    {
        #region ============== Private Constant =======================

        /// <summary>
        /// Interval in milliseconds to wait for the response
        /// </summary>
        private int WaitForResponseInterval = 300;

        /// <summary>
        /// Number of retries to wait for response
        /// </summary>
        private int WaitForResponseRetries = 100;

        #endregion ====================================================

        #region ============== Private Variable =======================

        /// <summary>
        /// Command for this feature
        /// </summary>
        private List<ICommand> commands;


        /// <summary>
        /// Echo indicator flag
        /// </summary>
        private bool hasEcho;

        /// <summary>
        /// Required reset flag
        /// </summary>
        private bool requiredReset;

        #endregion ====================================================

        #region ====================== Constructor ====================

        /// <summary>
        /// Default constructor
        /// </summary>
        protected BaseFeature()
        {
            /// Set to true by default
            Supported = true;

            // Default to synchronous behavior
            ExecutionBehavior = ExecutionBehavior.Synchronous;
                       
            // Create the command list
            commands = new List<ICommand>(1);

            // Default to false
            StopExecution = false;

            // Default is AT command type
            CommandType = FeatureCommandType.AT;

            // Default to false
            hasEcho = false;

            // Indicate if there is a need to reset the gateway
            requiredReset = false;
         
        }

        #endregion =====================================================


        #region =========== Public Properties ==========================

        /// <summary>
        /// Physical connection to the gateway
        /// </summary>
        /// <value>Serial port. See <see cref="SerialPort"/></value>
        public SerialPort Port
        {
            get;
            set;
        }

        /// <summary>
        /// Data queue for gateway incoming data
        /// </summary>
        /// <value>data queue</value>
        public IncomingDataQueue IncomingDataQueue
        {
            get;
            set;
        }

        /// <summary>
        /// Indicator to show that if this feature is supported
        /// </summary>
        /// <value>true or false. true if it is supported</value>
        public bool Supported
        {
            get;
            set;
        }

        /// <summary>
        /// Indicate the feature execution behavior.
        /// See <see cref="ExecutionBehavior"/>
        /// </summary>
        /// <value>Return the execution behavior</value>
        public ExecutionBehavior ExecutionBehavior
        {
            get;
            set;
        }

        /// <summary>
        /// Stop the execution of the feature command flow
        /// </summary>
        /// <value>true to stop execution</value>
        public bool StopExecution
        {
            get;
            set;
        }

        /// <summary>
        /// Commands to to be executed in sequence for this feature.
        /// See <see cref="Command"/>
        /// </summary>
        /// <value>List of commands</value>
        public List<ICommand> Commands
        {
            get
            {
                return commands;
            }
        }


        /// <summary>
        /// Gets or sets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public FeatureCommandType CommandType
        {
            get;
            set;
        }


        /// <summary>
        /// Indicate if the command is being echo
        /// </summary>
        /// <value><c>true</c> if this instance has echo; otherwise, <c>false</c>.</value>
        public bool HasEcho
        {
            get
            {
                return hasEcho;
            }            
        }


        /// <summary>
        /// Gets a value indicating whether [required reset].
        /// </summary>
        /// <value><c>true</c> if [required reset]; otherwise, <c>false</c>.</value>
        public bool RequiredReset
        {
            get
            {
                return this.requiredReset;
            }
        }

        #endregion =====================================================


        #region =========== Public Methods =============================


        /// <summary>
        /// Execute the model feature
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <returns>true if execution is successful</returns>
        /// <exception cref="GatewayException">GatewayException is thrown when there is error in execution</exception>
        public virtual bool Execute(IContext context)
        {
            // Execute the commands
            if (Commands.Count > 0)
            {
                StopExecution = false;

                foreach (ICommand command in Commands)
                {
                    if (!command.Preprocessing(context, command) && !command.IgnoreError) return false;

                    if (StopExecution) return true;

                    try
                    {
                        if (!SendCommand(context, command) && !command.IgnoreError) return false;
                    }
                    catch (Exception ex)
                    {
                        if (command.ExceptionHandler != null)
                        {
                            command.ExceptionHandler(context, command, ex);
                        }

                        if (command.IgnoreError)
                            //throw ex;
                            continue;
                        else
                            throw ex;
                    }
                    if (!command.Postprocessing(context, command) && !command.IgnoreError) return false;

                    if (StopExecution) return true;

                }
                return true;
            }           
            throw new GatewayException(string.Format(Resources.NoCommandDefined, ToString()));
        }

       

        #endregion ======================================================


        #region =========== Protected Methods ==========================

     

        /// <summary>
        /// This method acts like a method placeholder.
        /// It does nothing but just return the passed in context
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>Just return true</returns>
        protected bool DoNothing(IContext context, ICommand command)
        {
            return true;
        }

        /// <summary>
        /// Add the command
        /// </summary>
        /// <param name="command">Command instance</param>
        protected void AddCommand(ICommand command)
        {
            commands.Add(command);            
        }

        /// <summary>
        /// Send command to the gateway
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if execution is successful</returns>      
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected bool SendCommand(IContext context, ICommand command)
        {            
            string result = string.Empty;
            //this.requiredReset = false;
            //this.hasEcho = false;
            try
            {
                //throw new Exception("testing exception");
                string callText;
                byte[] callCommand;
                int count = 0;
                callText = command.Request;

                // Log the command
                Logger.LogThis("Sending: " + callText.Trim(), LogLevel.Verbose);
                
                // Send as ASCII
                callCommand = new ASCIIEncoding().GetBytes(callText);
                Port.Write(callCommand, 0, callCommand.Length);
                //while (Port.BytesToRead <= 0)
                while (IncomingDataQueue.Count == 0) 
                {
                    Thread.Sleep(WaitForResponseInterval);
                    if (count++ == WaitForResponseRetries) break;
                }
                //if (Port.BytesToRead > 0)
                if (IncomingDataQueue.Count > 0) 
                {
                    count = 0;
                    while (count++ <= WaitForResponseRetries)
                    {
                        // Read the response from gateway
                        //string data = Port.ReadExisting();

                        while (IncomingDataQueue.Count > 0)
                        {
                            string data = IncomingDataQueue.Peek();

                            if (string.IsNullOrEmpty(data))
                            {
                                // Log the received data
                                //Logger.LogThis(data, LogLevel.Verbose);

                                // Remove it from queue
                                IncomingDataQueue.Dequeue();

                                // Wait and continue reading
                                Thread.Sleep(WaitForResponseInterval);
                                continue;
                            }
                            // Remove from queue
                            IncomingDataQueue.Dequeue();
                            result += data;

                            // Log the result                            
                            //Logger.LogThis("Data from queue: " + result, LogLevel.Verbose);

                            // Check if the result match expectation
                            if (command.MatchExpectedResponse(result))
                            {
                                Logger.LogThis("Response: " + result.Replace("\r\n", "<cr><lf>"), LogLevel.Verbose);
                                context.PutData(result);

                                // Check for echo
                                if (result.Trim().StartsWith(command.Request))
                                {
                                    Logger.LogThis("Found echo in response", LogLevel.Verbose);
                                    hasEcho = true;
                                }
                                return true;
                            }

                            // Check for error
                            string errorDescription = ResultParser.ParseError(command, data);
                            if (!string.IsNullOrEmpty(errorDescription))
                            {
                                Logger.LogThis(errorDescription.Replace("\r\n", "<cr><lf>"), LogLevel.Verbose);
                                throw new GatewayException(errorDescription);
                            }

                            errorDescription = ResultParser.ParseError(command, result);
                            if (!string.IsNullOrEmpty(errorDescription))
                            {
                                Logger.LogThis(errorDescription.Replace("\r\n", "<cr><lf>"), LogLevel.Verbose);
                                throw new GatewayException(errorDescription);
                            }
                        }
                        // Continue waiting and reading
                        Thread.Sleep(WaitForResponseInterval);
                    }
                }
                this.requiredReset = true;  // Maybe required a reset
                throw new GatewayException(string.Format(Resources.CommandException, ResultParser.Trim(command.Request), ResultParser.Trim(result)));
            }
            catch (GatewayException gex)
            {
                throw gex;
            }
            catch (Exception ex)
            {
                throw new GatewayException(string.Format(Resources.CommandException, ResultParser.Trim(command.Request), ResultParser.Trim(result + ex.Message)), ex);
            }
        }

        #endregion =====================================================





    }
}

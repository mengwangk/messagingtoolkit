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
using System.Text.RegularExpressions;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Command class which contains the request, response and the
    /// method to parse the response
    /// </summary>
    internal sealed class Command: ICommand
    {
        #region ============ Private Variables================================


        /// <summary>
        /// Command request string
        /// </summary>
        private string request;

        /// <summary>
        /// Command response string. Can be a regular expression
        /// </summary>
        private string expectedResponse;

        /// <summary>
        /// Alternative command response string
        /// </summary>
        private string alternativeExpectedResponse;

        /// <summary>
        /// Preprocessing method. E.g. to format the request
        /// </summary>
        private Func<IContext, ICommand, bool> preprocessMethod;

        /// <summary>
        /// Postprocessing method. E.g. to parse the response
        /// </summary>
        private Func<IContext, ICommand, bool> postprocessMethod;


        /// <summary>
        /// Regular expression for the expected response
        /// </summary>
        private Regex expectedResponsePattern;

        /// <summary>
        /// Regular expression for alternative expected response
        /// </summary>
        private Regex alternativeResponsePattern;


        #endregion ===========================================================


        #region ============ Constructor ====================================

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="request">Request string</param>
        /// <param name="expectedResponse">Response string</param>
        /// <param name="preprocessMethod">Pre processing method</param>
        /// <param name="postprocessMethod">Post processing</param>
        /// <param name="ignoreError">Ignore error during command execution</param>
        private Command(string request, string expectedResponse, Func<IContext, ICommand, bool> preprocessMethod, Func<IContext, ICommand, bool> postprocessMethod, bool ignoreError)
        {  
            this.request = FormatRequest(request);
            this.expectedResponse = expectedResponse;
            this.alternativeExpectedResponse = string.Empty;
            this.preprocessMethod = preprocessMethod;
            this.postprocessMethod = postprocessMethod;
            this.IgnoreError = ignoreError;

            // Generate the regular expression
            expectedResponsePattern = new Regex(expectedResponse, RegexOptions.Compiled | 
                            RegexOptions.IgnoreCase | RegexOptions.Multiline|
                            RegexOptions.IgnorePatternWhitespace);

            // Generate the regular expression
            alternativeResponsePattern = new Regex(alternativeExpectedResponse);
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="request">Request string</param>
        /// <param name="responseEnum">Response enumeration. See <see cref="Response"/></param>
        /// <param name="alternativeResponseEnum">The alternative response enumeration</param>
        /// <param name="preprocessMethod">Pre processing method</param>
        /// <param name="postprocessMethod">Post processing method</param>
        /// <param name="ignoreError">Ignore error during command execution</param>
        private Command(string request, Response responseEnum, Response alternativeResponseEnum, Func<IContext, ICommand, bool> preprocessMethod, Func<IContext, ICommand, bool> postprocessMethod, bool ignoreError)
        {
            this.request = FormatRequest(request);
            this.expectedResponse = StringEnum.GetStringValue(responseEnum);
            this.alternativeExpectedResponse = StringEnum.GetStringValue(alternativeResponseEnum);
            this.preprocessMethod = preprocessMethod;
            this.postprocessMethod = postprocessMethod;
            this.IgnoreError = ignoreError;

            // Generate the regular expression
            expectedResponsePattern = new Regex(expectedResponse, RegexOptions.Compiled |
                            RegexOptions.IgnoreCase | RegexOptions.Multiline |
                            RegexOptions.IgnorePatternWhitespace);

            // Generate the regular expression
            alternativeResponsePattern = new Regex(alternativeExpectedResponse);

        }

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="request">Request string</param>
        /// <param name="responseEnum">Response enumeration. See <see cref="Response"/></param>
        /// <param name="preprocessMethod">Pre processing method</param>
        /// <param name="postprocessMethod">Post processing method</param>
        /// <param name="ignoreError">Ignore error during command execution</param>
        private Command(string request, Response responseEnum, Func<IContext, ICommand, bool> preprocessMethod, Func<IContext, ICommand, bool> postprocessMethod, bool ignoreError)
        {
            this.request = FormatRequest(request);
            this.expectedResponse = StringEnum.GetStringValue(responseEnum);
            this.alternativeExpectedResponse = string.Empty;
            this.preprocessMethod = preprocessMethod;
            this.postprocessMethod = postprocessMethod;
            this.IgnoreError = ignoreError;

            // Generate the regular expression
            expectedResponsePattern = new Regex(expectedResponse, RegexOptions.Compiled |
                            RegexOptions.IgnoreCase | RegexOptions.Multiline |
                            RegexOptions.IgnorePatternWhitespace);

            // Generate the regular expression
            alternativeResponsePattern = new Regex(alternativeExpectedResponse); 

        }

       
        #endregion ===========================================================

        #region ============ Public Functions ================================

        /// <summary>
        /// Match the response with the expected result
        /// </summary>
        /// <param name="result">Response from gateway</param>
        /// <returns>true if met, false otherwise</returns>
        public bool MatchExpectedResponse(string result)
        {
            // If nothing is expected then just return true
            if (string.IsNullOrEmpty(expectedResponse)) return true;

            if (expectedResponsePattern.IsMatch(result))
            {
                return true;
            }
            if (!string.IsNullOrEmpty(alternativeExpectedResponse) && alternativeResponsePattern.IsMatch(result))
            {
                return true;
            }
            return false;
        }

        #endregion ===========================================================


        #region ============ Private Functions ================================

        /// <summary>
        /// Format the command.
        /// Make sure the request ends with the required control character
        /// </summary>
        /// <param name="command">Command to be formatted</param>
        /// <returns>Formatted command</returns>
        private string FormatRequest(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return "\r";
            }   

            // The command must end with ASCII char 26 or CR
            if (
                !(
                  command.EndsWith(Convert.ToChar(26).ToString()) ||                 
                  command.EndsWith("\r"))
                  )
            {
                command += "\r";
                return command;
            }

            return command;            
        }

        #endregion ===========================================================


        #region ============ Public Properties ==============================

        /// <summary>
        /// Command to be sent
        /// </summary>
        /// <value>Command string</value>
        public string Request
        {
            get
            {
                return request;
            }
            set
            {
                this.request = FormatRequest(value);
            }
        }

        /// <summary>
        /// Function/method to parse the actual response, returning
        /// any expected result in the context
        /// </summary>
        /// <value>
        /// Function to parse the response and send back the result in context
        /// </value>
        public Func<IContext, ICommand, bool> Postprocessing
        {
            get
            {
                return postprocessMethod;
            }
        }


        /// <summary>
        /// Function/method to preprocess the request. E.g. formatting the request
        /// command to be sent
        /// </summary>
        /// <value>
        /// Function to preprocess the request
        /// </value>
        public Func<IContext, ICommand, bool> Preprocessing
        {
            get
            {
                return preprocessMethod;
            }
        }

        /// <summary>
        /// If set to true, the command is executed and any exceptions are ignored
        /// </summary>
        /// <value>Set to true to ignore exception. Default is false</value>
        public bool IgnoreError
        {
            get;
            set;
        }

        /// <summary>
        /// Exception handler. Handler get executed when there is 
        /// error in executing the command
        /// </summary>
        /// <value>Exception handler function</value>
        public Func<IContext, ICommand, Exception, bool> ExceptionHandler
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the expected response.
        /// </summary>
        /// <value>The expected response.</value>
        public string ExpectedResponse
        {
            get
            {
                return this.expectedResponse;
            }
            set
            {
                this.expectedResponse = value;

                // Generate the regular expression
                expectedResponsePattern = new Regex(expectedResponse, RegexOptions.Compiled |
                                RegexOptions.IgnoreCase | RegexOptions.Multiline |
                                RegexOptions.IgnorePatternWhitespace);
            }
        }
        #endregion ===========================================================


        #region ============ Public Static Method=============================

        /// <summary>
        /// Static factory method used to create a command
        /// </summary>
        /// <param name="request">Request string</param>
        /// <param name="response">Expected response</param>
        /// <param name="preprocessMethod">Pre processing method</param>
        /// <param name="postprocessMethod">Post processing method</param>
        /// <returns>A Command instance</returns>
        public static Command NewInstance(string request, string response, Func<IContext, ICommand, bool> preprocessMethod, Func<IContext, ICommand, bool> postprocessMethod)
        {
            return new Command(request, response, preprocessMethod, postprocessMethod, false);
        }
        
        /// <summary>
        /// Static factory method used to create a command
        /// </summary>
        /// <param name="request">Request string</param>
        /// <param name="responseEnum">Expected response enumeration. See <see cref="Response"/></param>
        /// <param name="alternativeResponseEnum">The alternative response enumeration</param>
        /// <param name="preprocessMethod">Pre processing method</param>
        /// <param name="postprocessMethod">Post processing method</param>
        /// <returns>A Command instance</returns>
        public static Command NewInstance(string request, Response responseEnum, Response alternativeResponseEnum, Func<IContext, ICommand, bool> preprocessMethod, Func<IContext, ICommand, bool> postprocessMethod)
        {
            return new Command(request, responseEnum, alternativeResponseEnum, preprocessMethod, postprocessMethod, false);
        }

        /// <summary>
        /// Static factory method used to create a command
        /// </summary>
        /// <param name="request">Request string</param>
        /// <param name="responseEnum">Expected response enumeration. See <see cref="Response"/></param>
        /// <param name="preprocessMethod">Pre processing method</param>
        /// <param name="postprocessMethod">Post processing method</param>
        /// <returns>A Command instance</returns>
        public static Command NewInstance(string request, Response responseEnum, Func<IContext, ICommand, bool> preprocessMethod, Func<IContext, ICommand, bool> postprocessMethod)
        {
            return new Command(request, responseEnum, preprocessMethod, postprocessMethod, false);
        }

        /// <summary>
        /// Static factory method used to create a command
        /// </summary>
        /// <param name="request">Request string</param>
        /// <param name="response">Expected response</param>
        /// <param name="preprocessMethod">Pre processing method</param>
        /// <param name="postprocessMethod">Post processing method</param>
        /// <param name="ignoreError">Ignore error during command execution</param>
        /// <returns>A Command instance</returns>
        public static Command NewInstance(string request, string response, Func<IContext, ICommand, bool> preprocessMethod, Func<IContext, ICommand, bool> postprocessMethod, bool ignoreError)
        {
            return new Command(request, response, preprocessMethod, postprocessMethod, ignoreError);
        }

        /// <summary>
        /// Static factory method used to create a command
        /// </summary>
        /// <param name="request">Request string</param>
        /// <param name="responseEnum">Expected response enumeration. See <see cref="Response"/></param>
        /// <param name="preprocessMethod">Pre processing method</param>
        /// <param name="postprocessMethod">Post processing method</param>
        /// <param name="ignoreError">Ignore error during command execution</param>
        /// <returns>A Command instance</returns>
        public static Command NewInstance(string request, Response responseEnum, Func<IContext, ICommand, bool> preprocessMethod, Func<IContext, ICommand, bool> postprocessMethod, bool ignoreError)
        {
            return new Command(request, responseEnum, preprocessMethod, postprocessMethod, ignoreError);
        }

        #endregion ===========================================================
    }
}

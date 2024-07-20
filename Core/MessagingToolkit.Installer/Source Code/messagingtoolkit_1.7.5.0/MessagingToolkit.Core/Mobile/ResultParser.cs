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

using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Utilities class to parse response from gateway
    /// </summary>
    internal static class ResultParser
    {
        #region =========== Private Static Variable ========================================================

        /// <summary>
        /// Error pattern
        /// </summary>
        private static Regex ErrorPattern = new Regex(
                            StringEnum.GetStringValue(Response.Error), 
                            RegexOptions.Compiled |
                            RegexOptions.IgnoreCase | RegexOptions.Multiline |
                            RegexOptions.IgnorePatternWhitespace);


        #endregion ========================================================================================

        #region =========== Public Static Methods =========================================================

        /// <summary>
        /// Return CMS error description
        /// </summary>
        /// <param name="errorCode">CMS error code</param>
        /// <returns>Error description</returns>
        public static string GetCmsErrorDescription(string errorCode)
        {
            try
            {
                CmsErrorCode error = (CmsErrorCode)Enum.Parse(typeof(CmsErrorCode), "_" + errorCode);
                string errorDescription =  StringEnum.GetStringValue(error);
                if (string.IsNullOrEmpty(errorDescription))
                {
                    return (string.Format(Resources.ErrorDescriptionNotFound, errorCode));
                }
                return errorDescription;
            }
            catch (Exception ex)
            {
                // Cannot find any error description
                return (string.Format(Resources.ErrorDescriptionNotFound, errorCode, ex.Message));
            }
        }

        /// <summary>
        /// Return CME error description
        /// </summary>
        /// <param name="errorCode">CME error code</param>
        /// <returns>Error description</returns>
        public static string GetCmeErrorDescription(string errorCode)
        {
            try
            {
                CmeErrorCode error = (CmeErrorCode)Enum.Parse(typeof(CmeErrorCode), "_" + errorCode);
                string errorDescription = StringEnum.GetStringValue(error);
                if (string.IsNullOrEmpty(errorDescription))
                {
                    return (string.Format(Resources.ErrorDescriptionNotFound, errorCode));
                }
                return errorDescription;
            }
            catch (Exception ex)
            {
                // Cannot find any error description
                return (string.Format(Resources.ErrorDescriptionNotFound, errorCode, ex.Message));
            }
        }

        /// <summary>
        /// Parse for any gateway error
        /// </summary>
        /// <param name="command">Command instance</param>
        /// <param name="response">Response from gateway</param>
        /// <returns>
        /// Return the error description or empty string if no error is found
        /// </returns>
        public static string ParseError(ICommand command, string response)
        {
            // Check if there is any CMS error
            if (Regex.IsMatch(response,StringEnum.GetStringValue(Response.CmsError)))
            {
                string[] cols = response.Split(':');

                if (cols.Length >= 1) {
                    string errorCode = cols[1].Trim();
                    return GetCmsErrorDescription(errorCode);
                } else {
                    // Unknown CMS error, so just return it
                    return response;
                }                
            }

            // Check if there is any CME error
            if (Regex.IsMatch(response, StringEnum.GetStringValue(Response.CmeError)))
            {
                string[] cols = response.Split(':');

                if (cols.Length >= 1)
                {
                    string errorCode = cols[1].Trim();
                    return GetCmeErrorDescription(errorCode);
                }
                else
                {
                    // Unknown CME error, so just return it
                    return response;
                }
            }

            // Check if there is any unknown error
            //if (ErrorPattern.IsMatch(response))
            //if (StringEnum.GetStringValue(Response.Error).Equals(response.Trim()))
            if (Regex.IsMatch(response, StringEnum.GetStringValue(Response.Error)))
            {
                return string.Format(Resources.UnsupportedCommand, Trim(command.Request), Trim(response));            
            }

            return string.Empty;
        }

        /// <summary>
        /// Parse the response from gateway.
        /// <example>
        /// E.g.
        /// for response
        /// "\r\nSony Ericsson K610\r\n\r\nOK\r\n"
        /// it will splitted into multiple lines,
        /// and blank lines will be removed
        /// Sony Ericsson K610
        /// OK
        /// </example>
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string[] ParseResponse(string response)
        {
            List<string> filteredResult = new List<string>(2);
            
            if (string.IsNullOrEmpty(response)) return filteredResult.ToArray();

            string[] lines = response.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    filteredResult.Add(line);
                }
            }
            return filteredResult.ToArray();
        }


        /// <summary>
        /// Trim the response from the gateway, removing any blank lines
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Trim(string value)
        {
            return Regex.Replace(value, "[\r\n]", " ");
        }

        /// <summary>
        /// Parse a string to integer list.
        /// E.g. 
        /// (1,2) to list of 1 and 2
        /// (0-3) to list of 0,1,2,3
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static List<int> ParseToList(string s)
        {
            List<int> list = new List<int>();
            Match match = new Regex(@"(?:(\d+),?)+").Match(s);
            if (match.Success)
            {
                foreach (Capture capture in match.Groups[1].Captures)
                {
                    list.Add(int.Parse(capture.Value));
                }
            }
            Match match2 = new Regex(@"(\d+)-(\d+)").Match(s);
            if (match2.Success)
            {
                int num = int.Parse(match2.Groups[1].Value);
                int num2 = int.Parse(match2.Groups[2].Value);
                for (int i = num; i <= num2; i++)
                {
                    list.Add(i);
                }
            }
            return list;
        }



        #endregion ============================================================================================
    }
}

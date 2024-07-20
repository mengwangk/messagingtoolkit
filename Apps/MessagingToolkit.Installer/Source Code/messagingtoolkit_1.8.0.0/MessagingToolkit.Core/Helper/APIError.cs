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
using System.Runtime.InteropServices;

namespace MessagingToolkit.Core.Helper
{
    /// <summary>
    /// Summary description for API errors.
    /// </summary>
    internal class APIError
    {
        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        /// <summary>
        /// Formats the message.
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <param name="source">The source.</param>
        /// <param name="messageId">The message id.</param>
        /// <param name="languageId">The language id.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="size">The size.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        private static extern UInt32 FormatMessage(UInt32 flags, IntPtr source, UInt32 messageId,
            UInt32 languageId, System.Text.StringBuilder buffer, UInt32 size, IntPtr arguments);


        /// <summary>
        /// Gets the API error message description.
        /// </summary>
        /// <param name="ApiErrNumber">The API err number.</param>
        /// <returns></returns>
        public static string GetAPIErrorMessageDescription(UInt32 ApiErrNumber)
        {
            System.Text.StringBuilder sError = new System.Text.StringBuilder(512);
            UInt32 lErrorMessageLength;
            lErrorMessageLength = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, (IntPtr)0, ApiErrNumber, 0, sError, (UInt32)sError.Capacity, (IntPtr)0);

            if (lErrorMessageLength > 0)
            {
                string strgError = sError.ToString();
                strgError = strgError.Substring(0, strgError.Length - 2);
                return strgError + " (" + ApiErrNumber.ToString() + ")";
            }
            return "none";

        }
        /// <summary>
        /// Gets the last error.
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32", EntryPoint = "GetLastError", SetLastError = true)]
        public static extern uint GetLastError();
    }
}

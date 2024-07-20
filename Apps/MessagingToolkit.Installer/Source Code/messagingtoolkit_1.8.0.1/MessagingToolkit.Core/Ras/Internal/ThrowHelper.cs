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

namespace MessagingToolkit.Core.Ras.Internal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Provides methods used to throw exceptions within the assembly.
    /// </summary>
    internal static class ThrowHelper
    {
        /// <summary>
        /// Throws a new <see cref="System.ArgumentOutOfRangeException"/> exception.
        /// </summary>
        /// <param name="argumentName">The argument name that caused the exception.</param>
        /// <param name="actualValue">The value of the argument.</param>
        /// <param name="resource">An <see cref="System.String"/> to include with the exception message.</param>
        public static void ThrowArgumentOutOfRangeException(string argumentName, object actualValue, string resource)
        {
            throw new ArgumentOutOfRangeException(argumentName, actualValue, ThrowHelper.FormatResourceString(resource, argumentName));
        }

        /// <summary>
        /// Throws a new <see cref="System.ArgumentNullException"/> exception.
        /// </summary>
        /// <param name="argumentName">The argument name that caused the exception.</param>
        public static void ThrowArgumentNullException(string argumentName)
        {
            throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Throws a new <see cref="System.ArgumentException"/> exception.
        /// </summary>
        /// <param name="argumentName">The argument name that caused the exception.</param>
        /// <param name="resource">An <see cref="System.String"/> to include with the exception message.</param>
        public static void ThrowArgumentException(string argumentName, string resource)
        {
            object[] args = { argumentName };

            ThrowHelper.ThrowArgumentException(argumentName, resource, args);
        }

        /// <summary>
        /// Throws a new <see cref="System.ArgumentException"/> exception.
        /// </summary>
        /// <param name="argumentName">The argument name that caused the exception.</param>
        /// <param name="resource">An <see cref="System.String"/> to include with the exception message.</param>
        /// <param name="args">A <see cref="System.Object"/> array containing zero or more items to format.</param>
        public static void ThrowArgumentException(string argumentName, string resource, params object[] args)
        {
            throw new ArgumentException(ThrowHelper.FormatResourceString(resource, args), argumentName);
        }

        /// <summary>
        /// Throws a new <see cref="MessagingToolkit.Core.Ras.InvalidHandleException"/> exception.
        /// </summary>
        /// <param name="handle">The <see cref="System.IntPtr"/> that caused the exeption.</param>
        /// <param name="resource">An <see cref="System.String"/> to include with the exception message.</param>
        public static void ThrowInvalidHandleException(RasHandle handle, string resource)
        {
            throw new InvalidHandleException(handle, resource);
        }

        /// <summary>
        /// Throws a new <see cref="MessagingToolkit.Core.Ras.InvalidHandleException"/> exception.
        /// </summary>
        /// <param name="handle">The <see cref="System.IntPtr"/> that caused the exeption.</param>
        /// <param name="argumentName">The argument name that caused the exception.</param>
        /// <param name="resource">An <see cref="System.String"/> to include with the exception message.</param>
        public static void ThrowInvalidHandleException(RasHandle handle, string argumentName, string resource)
        {
            throw new InvalidHandleException(handle, ThrowHelper.FormatResourceString(resource, new object[] { argumentName }));
        }

        /// <summary>
        /// Throws a new <see cref="System.InvalidOperationException"/> exception.
        /// </summary>
        /// <param name="resource">An <see cref="System.String"/> to include with the exception message.</param>
        public static void ThrowInvalidOperationException(string resource)
        {
            throw new InvalidOperationException(resource);
        }

        /// <summary>
        /// Throws a new <see cref="System.NotSupportedException"/> exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public static void ThrowNotSupportedException(string message)
        {
            throw new NotSupportedException(message);
        }

        /// <summary>
        /// Throws a new <see cref="System.UnauthorizedAccessException"/> exception.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public static void ThrowUnauthorizedAccessException(string message)
        {
            throw new UnauthorizedAccessException(message);
        }

        /// <summary>
        /// Throws a new <see cref="MessagingToolkit.Core.Ras.RasException"/> exception.
        /// </summary>
        /// <param name="errorCode">The error code that caused the exception.</param>
        public static void ThrowRasException(int errorCode)
        {
            throw new RasException(errorCode);
        }

        /// <summary>
        /// Throws a new <see cref="System.ComponentModel.Win32Exception"/> exception containing the last Win32 error that occurred.
        /// </summary>
        public static void ThrowWin32Exception()
        {
            throw new Win32Exception();
        }

        /// <summary>
        /// Throws a new <see cref="System.ComponentModel.Win32Exception"/> exception containing the last Win32 error that occurred.
        /// </summary>
        /// <param name="errorCode">The error code that caused the exception.</param>
        public static void ThrowWin32Exception(int errorCode)
        {
            throw new Win32Exception(errorCode);
        }

        /// <summary>
        /// Replaces the format item of the <see cref="System.String"/> resource specified with the equivalent in the <paramref name="args"/> array specified.
        /// </summary>
        /// <param name="resource">An <see cref="System.String"/> to format.</param>
        /// <param name="args">An <see cref="System.Object"/> array containing zero or more items to format.</param>
        /// <returns>The formatted resource string.</returns>
        private static string FormatResourceString(string resource, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, resource, args);
        }
    }
}
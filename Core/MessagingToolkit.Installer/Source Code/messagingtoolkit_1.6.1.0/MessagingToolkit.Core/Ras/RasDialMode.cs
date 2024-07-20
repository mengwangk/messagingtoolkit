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
namespace MessagingToolkit.Core.Ras
{
    using System;

    /// <summary>
    /// Defines the dial modes.
    /// </summary>
    public enum RasDialMode
    {
        /// <summary>
        /// No dial mode specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Dial all subentries.
        /// </summary>
        DialAll = 1,

        /// <summary>
        /// Dial the number of subentries as additional bandwidth is needed.
        /// </summary>
        DialAsNeeded = 2
    }
}
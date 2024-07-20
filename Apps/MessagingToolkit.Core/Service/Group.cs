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

namespace MessagingToolkit.Core.Service
{
    /// <summary>
    /// Group of destination numbers
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Group name</param>
        public Group(string name)
        {
            Name = name;
            Numbers = new List<string>();
        }


        /// <summary>
        /// Group name
        /// </summary>
        /// <value>Group name</value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Numbers associated with the group
        /// </summary>
        /// <value>List of numbers</value>
        public List<string> Numbers
        {
            get;
            private set;
        }


        /// <summary>
        /// Add a number
        /// </summary>
        /// <param name="number">Number</param>
        public void AddNumber(string number)
        {
            Numbers.Add(number);
        }

        /// <summary>
        /// Removes a number from the group.
        /// </summary>
        /// <param name="number">The number to be removed from the group.</param>
        /// <returns>
        /// return True if the removal was a success. False if the number was not found
        /// </returns>
        public bool RemoveNumber(string number)
        {
            return Numbers.Remove(number);
        }

        /// <summary>
        /// Removes all numbers from the group (clears the group).
        /// </summary>      
        public void Clear()
        {
            Numbers.Clear();
        }
    }
}

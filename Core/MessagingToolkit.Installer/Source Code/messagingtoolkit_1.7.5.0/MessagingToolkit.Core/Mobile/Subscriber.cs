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

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Subscriber information
    /// </summary>
    public class Subscriber
    {
        private string alpha;
        private int itc;
        private string number;
        private int service;
        private int speed;
        private int type;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number"></param>
        /// <param name="type"></param>
        public Subscriber(string number, int type)
        {
            this.alpha = string.Empty;
            this.number = number;
            this.type = type;
            this.speed = -1;
            this.service = -1;
            this.itc = -1;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="number"></param>
        /// <param name="type"></param>
        /// <param name="speed"></param>
        /// <param name="service"></param>
        /// <param name="itc"></param>
        public Subscriber(string alpha, string number, int type, int speed, int service, int itc)
        {
            this.alpha = alpha;
            this.number = number;
            this.type = type;
            this.speed = speed;
            this.service = service;
            this.itc = itc;
        }


        /// <summary>
        /// </summary>
        /// <value></value>
        public string Alpha
        {
            get
            {
                return this.alpha;
            }
        }

        /// <summary>
        /// Information transfer capability
        /// </summary>
        /// <value></value>
        public int Itc
        {
            get
            {
                return this.itc;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string Number
        {
            get
            {
                return this.number;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int Service
        {
            get
            {
                return this.service;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int Speed
        {
            get
            {
                return this.speed;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int Type
        {
            get
            {
                return this.type;
            }
        }

    }
}

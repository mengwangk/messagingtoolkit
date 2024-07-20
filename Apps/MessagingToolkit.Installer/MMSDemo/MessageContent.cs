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

namespace MessagingToolkit.MMS.Demo
{
    /// <summary>
    /// Message content
    /// </summary>
    public class MessageContent
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageContent"/> class.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="contentId">The content id.</param>
        /// <param name="fileName">Name of the file.</param>
        public MessageContent(string contentType, string contentId, string fileName)
        {
            this.ContentType = contentType;
            this.ContentId = contentId;
            this.FileName = fileName;
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the content id.
        /// </summary>
        /// <value>The content id.</value>
        public string ContentId
        {
            get;
            private set;
        }

        /// <summary>
        /// File name
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get;
            private set;
        }       
    }
}

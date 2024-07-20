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
using System.IO;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// MMS attachment class
    /// </summary>
    [global::System.Serializable]
    public class MmsAttachment
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MmsAttachment"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="data">The data.</param>
        /// <param name="attachmentType">Type of the attachment.</param>
        /// <param name="contentType">Type of the content.</param>
        public MmsAttachment(string name, byte[] data, AttachmentType attachmentType, ContentType contentType)
        {
            this.Name = name;
            this.Data = data;
            this.AttachmentType = attachmentType;
            this.ContentType = contentType;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the type of the attachment.
        /// </summary>
        /// <value>The type of the attachment.</value>
        public AttachmentType AttachmentType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public ContentType ContentType
        {
            get;
            private set;
        }
    }

}

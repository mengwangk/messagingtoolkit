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

namespace MessagingToolkit.Pdu
{
    /// <summary> 
    /// The parent of all message-related classes. Most of common fields and
    /// attributes of both inbound and outbound messages are placed in this class.
    /// </summary>
#if !NETFX_CORE && !PORTABLE
    [Serializable]
#else
    [System.Runtime.Serialization.DataContract]
#endif
    internal abstract class Message
    {
        private static long messageIdSeed = 0;

        private long messageId;

        private string gtwId;

        private MessageTypes type;

        private DateTime date;

        private string id;

        private string text;

        private MessageEncodings encoding;

        protected internal int dstPort;

        protected internal int srcPort;

        protected internal int messageCharCount;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="myType">My type.</param>
        /// <param name="myDate">My date.</param>
        /// <param name="myText">My text.</param>
        public Message(MessageTypes myType, ref DateTime myDate, string myText)
        {
            this.messageId = messageIdSeed++;
            GatewayId = "" ;
            Type = myType;
            Id = "";
            Date = myDate;
            if (!string.IsNullOrEmpty(myText))
                text = myText;
            Encoding = MessageEncodings.Enc7Bit ;
            SourcePort = -1;
            DestinationPort = -1;
            this.messageCharCount = 0;
        }


        /// <summary>
        /// Returns the creation date. For outbound messages, this is the object's
        /// creation date. For inbound messages, this is the date when the originator
        /// has sent the message.
        /// </summary>
        /// <value>The date.</value>
        virtual public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                this.date = value;
            }

        }

        /// <summary>
        /// Sets the message ID to a specific value.
        /// </summary>
        /// <value>The id.</value>
        virtual public string Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }

        }
        /// <summary>
        /// Returns the auto-generated, internal message ID.
        /// </summary>
        /// <value>The message id.</value>
        virtual public long MessageId
        {
            get
            {
                return this.messageId;
            }

        }

        /// <summary>
        /// Sets the destination port of the message. Source and Destination ports
        /// are used when messages are targeting a midlet application. For standard
        /// SMS messages, the Source and Destination ports should <b>both</b> be set
        /// to -1 (which is their default value anyway).
        /// <p>
        /// The default is (-1).
        /// </p>
        /// </summary>
        /// <value></value>
        virtual public int DestinationPort
        {
            get
            {
                return this.dstPort;
            }

            set
            {
                this.dstPort = value;
            }

        }

        /// <summary>
        /// Sets the source port of the message. Source and Destination ports are
        /// used when messages are targeting a midlet application. For standard SMS
        /// messages, the Source and Destination ports should <b>both</b> be set to
        /// -1 (which is their default value anyway).
        /// <p>
        /// The default is (-1).
        /// </p>
        /// </summary>
        /// <value></value>
        virtual public int SourcePort
        {
            get
            {
                return this.srcPort;
            }

            set
            {
                this.srcPort = value;
            }

        }
        
        public abstract string PduUserData 
        { 
            get; 
        }

        public abstract string PduUserDataHeader 
        { 
            get; 
        }

        /// <summary>
        /// Set or get the message encoding
        /// </summary>
        /// <value></value>
        public virtual MessageEncodings Encoding
        {
            get
            {
                return this.encoding;
            }
            set
            {
                this.encoding = value;
            }
        }

    

        /// <summary> 
        /// Return or set the ID of the gateway which the message was received from (for
        /// inbound messages) or the message was dispatched from (outbound messages).        /// 
        /// </summary>
        /// <returns> The Gateway ID.</returns>      
        public virtual string GatewayId 
        {
            get 
            {
                return this.gtwId;
            }
            set
            {
                this.gtwId = value;
            }
        }

        public virtual string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }
        
        public virtual void AddText(string addText)
        {
            this.text += addText;
        }

        public virtual MessageTypes Type 
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        protected internal virtual void CopyTo(Message msg)
        {
            msg.Date = Date;
            msg.Encoding = this.Encoding;
            msg.Id = Id;
            msg.GatewayId = this.GatewayId;
            msg.SourcePort = SourcePort;
            msg.DestinationPort = DestinationPort;
            msg.Type = this.Type;
            msg.Text = this.Text;           
            msg.messageCharCount = this.messageCharCount;
        }
    }
}

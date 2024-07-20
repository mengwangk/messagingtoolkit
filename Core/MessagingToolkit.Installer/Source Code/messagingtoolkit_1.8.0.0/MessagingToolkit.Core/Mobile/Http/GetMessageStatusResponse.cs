using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// GET message request returning a list of messages.
    /// </summary>
    [DataContract]
    internal sealed class GetMessageStatusResponse : BaseResponse
    {

        /// <summary>
        /// Message information
        /// </summary>
        /// <value>
        /// The message status.
        /// </value>
        [DataMember(Name = "message")]
        public MessageStatusInformation MessageStatusInformation { get; set; }


        /// <summary>
        /// Messaging sending status, which can be "Sent", "Delivered", "Queued" or "Failed".
        /// </summary>
        /// <value>
        /// The message status.
        /// </value>
        [DataMember(Name = "status")]
        public string Status { get; set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "RequestMethod = ", RequestMethod, "\r\n");
            str = String.Concat(str, "Description = ", Description, "\r\n");
            str = String.Concat(str, "IsSuccessful = ", IsSuccessful, "\r\n");
            str = String.Concat(str, "MessageStatusInformation = ", MessageStatusInformation, "\r\n");
            str = String.Concat(str, "Status = ", Status, "\r\n");
            return str;
        }
    }
}


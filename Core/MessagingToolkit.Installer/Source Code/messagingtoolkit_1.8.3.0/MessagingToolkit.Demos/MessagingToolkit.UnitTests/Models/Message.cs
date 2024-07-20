using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MessagingToolkit.UnitTests.Models
{
    [DataContract]
    public class Message
    {
        [DataMember(Name = "message")]
        public string Content { get; set; }
    }
}

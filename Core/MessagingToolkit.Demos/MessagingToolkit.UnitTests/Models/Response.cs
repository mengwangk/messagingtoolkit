using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MessagingToolkit.UnitTests.Models
{
   [DataContract]
    public class Response
    {
       [DataMember(Name = "requestMethod")]
       public string RequestMethod { get; set; }

       [DataMember(Name = "messages")]
       public List<Message> Messages { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MessagingToolkit.Barcode.Provider
{
    public interface IOutput
    {
        string Content { get; set; }
    }
}

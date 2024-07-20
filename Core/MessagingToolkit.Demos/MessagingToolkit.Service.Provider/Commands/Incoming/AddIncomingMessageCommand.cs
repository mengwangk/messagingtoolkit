﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingToolkit.Service.Common.Models;

namespace MessagingToolkit.Service.Provider.Commands
{
    public sealed class AddIncomingMessageCommand : ICommand<Incoming>
    {
        public Incoming Message { get; set; }
    }
}

﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeSketchName.Services
{
    internal class MessagesService
    {
        internal class ConfirmationMessagesService : PubSubEvent<string> { }
        internal class InformationMessagesService : PubSubEvent<string> { }
        internal class ErrorMessagesService : PubSubEvent<string> { }

    }
}
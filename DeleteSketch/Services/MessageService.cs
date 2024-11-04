using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteSketch.Services
{
    internal class ConfirmationMessagesService : PubSubEvent<string> { }
    internal class InformationMessagesService : PubSubEvent<string>
    {
        internal void Publish(object informationMessages)
        {
            throw new NotImplementedException();
        }
    }
    internal class ErrorMessagesService : PubSubEvent<string> { }
}

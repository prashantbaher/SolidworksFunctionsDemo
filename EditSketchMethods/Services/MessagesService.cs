using Prism.Events;

namespace InsertSketch.Services
{
    internal class ConfirmationMessagesService : PubSubEvent<string> { }
    internal class InformationMessagesService : PubSubEvent<string> { }
    internal class ErrorMessagesService : PubSubEvent<string> { }
}

using CompanyDirectoryService.Database;

using Messages.NServiceBus.Events;
using Messages.DataTypes;

using NServiceBus;
using NServiceBus.Logging;

using System.Threading.Tasks;

namespace CompanyDirectoryService.Handlers
{
    /// <summary>
    /// This is the handler class for the reverse echo. 
    /// This class is created and its methods called by the NServiceBus framework
    /// </summary>
    public class CreatedAccountHandler : IHandleMessages<AccountCreated>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        static ILog log = LogManager.GetLogger<AccountCreated>();

        /// <summary>
        /// Saves the echo to the database
        /// This method will be called by the NServiceBus framework when an event of type "AsIsEchoEvent" is published.
        /// </summary>
        /// <param name="message">Information about the echo</param>
        /// <param name="context"></param>
        /// <returns>Nothing</returns>
        public Task Handle(AccountCreated message, IMessageHandlerContext context)
        {
            if (message.type == AccountType.business)
            {
                CompanyDirectoryServiceDatabase.getInstance().saveBusiness(message);
            }
            return Task.CompletedTask;
        }
    }
}

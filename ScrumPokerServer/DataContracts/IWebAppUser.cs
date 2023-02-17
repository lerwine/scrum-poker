using System;

namespace ScrumPokerServer.DataContracts
{
    // TODO: Make this obsolete
    [Obsolete("Use DataContracts.DeveloperEntity, instead")]
    public interface IWebAppUser
    {
        string DisplayName { get; }
        string UserName { get; }
        string Password { get; }
    }
}

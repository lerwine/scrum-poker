namespace ScrumPokerServer.DataContracts
{
    public interface IWebAppUser
    {
        string DisplayName { get; }
        string UserName { get; }
        string Password { get; }
    }
}

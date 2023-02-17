namespace ScrumPoker.WebApp.Models;

public interface IParticipant
{
    Guid ID { get; set; }

    string Name { get; set; }

    string EmailAddress { get; set; }

    DateTime LastActivity { get; set; }
}

public class Participant : IParticipant
{
    public Guid ID { get; set; }

    public string Name { get; set; } = "";

    public string EmailAddress { get; set; } = "";

    public string Token { get; set; } = "";

    public DateTime LastActivity { get; set; }
}

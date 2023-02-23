using System;

namespace ScrumPoker.DataContracts
{
    public interface IUserProfile
    {
        Guid Id { get; set; }
        string DisplayName { get; set; }
        string UserName { get; set; }
        bool IsAdmin { get; set; }
    }
}
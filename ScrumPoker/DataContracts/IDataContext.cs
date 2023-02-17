using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    /// <summary>
    /// Abstraction for data storage.
    /// </summary>
    public class IDataContext
    {
        IDbSet<IScrumSession> Sessions { get; set; }

        IDbSet<IScrumPokerUser> Organizers { get; set; }
    }
}
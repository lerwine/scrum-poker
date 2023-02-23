using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    public interface ITitleAndIdentifier : IValidatableObject
    {
        string Title { get; set; }
        
        Guid Id { get; set; }
    }
}
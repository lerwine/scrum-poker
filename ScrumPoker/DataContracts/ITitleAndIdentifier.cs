using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    public interface ITitleAndIdentifier : IValidatableObject
    {
        string Title { get; set; }
        
        string Identifier { get; set; }
    }
}
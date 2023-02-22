using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    [System.Obsolete("Replace with DTO object(s)")]
    public interface ITitleAndIdentifier : IValidatableObject
    {
        string Title { get; set; }
        
        string Identifier { get; set; }
    }
}
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    public interface ITitleAndIdentifier
    {
        string Title { get; set; }
        
        string Identifier { get; set; }
    }
    // TODO: Move to ScrumPoker.StandaloneServer
    [DataContract]
    public abstract class TitleAndIdentifier
    {
        private string _title = "";
        [DataMember(Name = "title", IsRequired = true)]
        public string Title
        {
            get { return _title; }
            set { _title = value.EmptyIfNullOrTrimmed(); }
        }

        private string _identifier = null;
        [DataMember(Name = "identifier", EmitDefaultValue = false)]
        public string Identifier
        {
            get { return _identifier; }
            set { _identifier = value.TrimmedOrNullIfEmpty(); }
        }
    }
}
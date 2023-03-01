using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    [DataContract]
    public class DeckListItem
    {
        [DataMember(Name = "deckId", IsRequired = true)]
        #pragma warning disable IDE0051, IDE1006
        private string __DeckId
        #pragma warning restore IDE0051, IDE1006
        {
            get { return _deckId.ToJsonString(); }
            set { _deckId = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private Guid _deckId;
        public Guid DeckId
        {
            get { return _deckId; }
            set { _deckId = value; }
        }

        private string _name = "";
        [DataMember(Name = "title", IsRequired = true)]
        public string Name
        {
            get { return _name; }
            set { _name = value.WsNormalized(); }
        }

        private string _description;
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        {
            get { return _description; }
            set { _description = value.TrimmedOrNullIfEmpty(); }
        }
    }
}
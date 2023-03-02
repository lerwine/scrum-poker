using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.Deck
{
    [DataContract]
    public class BaseEntry
    {
        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
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
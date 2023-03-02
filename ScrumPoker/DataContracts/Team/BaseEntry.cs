using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.Team
{
    [DataContract]
    public class BaseEntry
    {
        [DataMember(Name = "id", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __Id
#pragma warning restore IDE0051, IDE1006
        {
            get { return _id.ToJsonString(); }
            set { _id = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _title = "";
        [DataMember(Name = "title", IsRequired = true)]
        public string Title
        {
            get { return _title; }
            set { _title = value.WsNormalized(); }
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

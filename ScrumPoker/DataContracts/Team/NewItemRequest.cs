using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.Team
{
    [DataContract]
    public class NewItemRequest
    {
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

        private Guid? _facilitatorId;
        public Guid? FacilitatorId
        {
            get { return _facilitatorId; }
            set { _facilitatorId = value; }
        }

        [DataMember(Name = "facilitatorId", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __FacilitatorId
#pragma warning restore IDE0051, IDE1006
        {
            get { return _facilitatorId.ToJsonString(); }
            set { _facilitatorId = value.JsonStringToGuid(); }
        }
    }
}
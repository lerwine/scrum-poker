using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.Initiative
{
    [DataContract]
    public class RecordEntry : BaseEntry
    {
        private Guid _teamId;
        public Guid TeamId
        {
            get { return _teamId; }
            set { _teamId = value; }
        }

        [DataMember(Name = "teamId", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __TeamId
#pragma warning restore IDE0051, IDE1006
        {
            get { return _teamId.ToJsonString(); }
            set { _teamId = value.JsonStringToGuid() ?? Guid.Empty; }
        }
    }
}
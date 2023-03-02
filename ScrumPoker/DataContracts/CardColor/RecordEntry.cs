using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.CardColor
{
    [DataContract]
    public class RecordEntry : BaseEntry
    {
        private Guid _schemeId;
        public Guid SchemeId
        {
            get { return _schemeId; }
            set { _schemeId = value; }
        }

        [DataMember(Name = "schemeId", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __SchemeId
#pragma warning restore IDE0051, IDE1006
        {
            get { return _schemeId.ToJsonString(); }
            set { _schemeId = value.JsonStringToGuid() ?? Guid.Empty; }
        }
    }
}
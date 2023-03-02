using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.Team
{
    [DataContract]
    public class RecordEntry : BaseEntry
    {
        private Guid _facilitatorId;
        /// <summary>
        /// The facilitator's unique identifier.
        /// </summary>
        public Guid FacilitatorId
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
            set { _facilitatorId = value.JsonStringToGuid() ?? Guid.Empty; }
        }
    }
}

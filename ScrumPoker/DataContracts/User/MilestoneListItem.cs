using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    [DataContract]
    public class MilestoneListItem : SprintGroupingResponse
    {
        private Guid _milestoneId;
        /// <summary>
        /// Gets the team's unique identifier.
        /// </summary>
        public Guid MilestoneId
        {
            get { return _milestoneId; }
            set { _milestoneId = value; }
        }

        [DataMember(Name = "milestoneId", IsRequired = true)]
        #pragma warning disable IDE0051, IDE1006
        private string __MilestoneId
        #pragma warning restore IDE0051, IDE1006
        {
            get { return _milestoneId.ToJsonString(); }
            set { _milestoneId = value.JsonStringToGuid() ?? Guid.Empty; }
        }
    }
}
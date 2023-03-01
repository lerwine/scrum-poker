using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class MilestoneListItem : SprintGroupingItem
    {
        private Guid _milestoneId;
        /// <summary>
        /// The milestone's unique identifier.
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
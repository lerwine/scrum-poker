using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.Milestone
{
    public class NewMilestoneRequest : SprintGroupingItem
    {
        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember(Name = "id", IsRequired = true)]
        #pragma warning disable IDE0051, IDE1006
        private string __Id
        #pragma warning restore IDE0051, IDE1006
        {
            get { return _id.ToJsonString(); }
            set { _id = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private bool _isTeamMilestone;

        [DataMember(Name = "isTeamMilestone", EmitDefaultValue = false)]
        public bool IsTeamMilestone
        {
            get { return _isTeamMilestone; }
            set { _isTeamMilestone = value; }
        }
    }
}
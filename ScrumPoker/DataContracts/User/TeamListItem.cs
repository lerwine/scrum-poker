using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// Response contract representing a team.
    /// </summary>
    [DataContract]
    public class TeamListItem
    {
        private Guid _teamId;
        /// <summary>
        /// The team's unique identifier.
        /// </summary>
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

        private string _title = "";
        [DataMember(Name = "title", IsRequired = true)]
        /// <summary>
        /// The title of the current team.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value.WsNormalized(); }
        }

        private string _description = null;
        [DataMember(Name = "description", EmitDefaultValue = false)]
        /// <summary>
        /// The description of the current tem.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value.TrimmedOrNullIfEmpty(); }
        }
    }
}

using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class InitiativeListItem : SprintGroupingItem
    {
        private Guid _initiativeId;
        /// <summary>
        /// The initiative's unique identifier.
        /// </summary>
        public Guid InitiativeId
        {
            get { return _initiativeId; }
            set { _initiativeId = value; }
        }

        [DataMember(Name = "initiativeId", IsRequired = true)]
        #pragma warning disable IDE0051, IDE1006
        private string __InitiativeId
        #pragma warning restore IDE0051, IDE1006
        {
            get { return _initiativeId.ToJsonString(); }
            set { _initiativeId = value.JsonStringToGuid() ?? Guid.Empty; }
        }
    }
}
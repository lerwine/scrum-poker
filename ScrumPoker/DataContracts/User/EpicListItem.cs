using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    [DataContract]
    public class EpicListItem : SprintGroupingResponse
    {
        private Guid _epicId;
        /// <summary>
        /// Gets the team's unique identifier.
        /// </summary>
        public Guid EpicId
        {
            get { return _epicId; }
            set { _epicId = value; }
        }

        [DataMember(Name = "epicId", IsRequired = true)]
        #pragma warning disable IDE0051, IDE1006
        private string __EpicId
        #pragma warning restore IDE0051, IDE1006
        {
            get { return _epicId.ToJsonString(); }
            set { _epicId = value.JsonStringToGuid() ?? Guid.Empty; }
        }
    }
}
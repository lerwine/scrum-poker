using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    [DataContract]
    public class UserStoryEntity : TitleAndIdentifier
    {
        private string _description;
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        {
            get { return _description; }
            set { _description = value.TrimmedOrNullIfEmpty(); }
        }

        private string _acceptanceCriteria;
        [DataMember(Name = "acceptanceCriteria", EmitDefaultValue = false)]
        public string AcceptanceCriteria
        {
            get { return _acceptanceCriteria; }
            set { _acceptanceCriteria = value.TrimmedOrNullIfEmpty(); }
        }

        private int? _projectId;
        [DataMember(Name = "projectId", EmitDefaultValue = false)]
        public int? ProjectId
        {
            get { return _projectId; }
            set { _projectId = (value.HasValue && value.Value >= 0) ? value : null; }
        }

        private int? _themeId;
        [DataMember(Name = "themeId", EmitDefaultValue = false)]
        public int? ThemeId
        {
            get { return _themeId; }
            set { _themeId = (value.HasValue && value.Value >= 0) ? value : null; }
        }
        
        private DateTime _created = DateTime.Now;
        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        [DataMember(Name = "created", IsRequired = true)]
        private string __Created
        {
            get { return _created.ToJsonString(); }
            set { _created = value.JsonStringToDate() ?? DateTime.Now; }
        }

        private int? _points;
        [DataMember(Name = "points", EmitDefaultValue = false)]
        public int? Points
        {
            get { return _points; }
            set { _points = (value.HasValue && value.Value >= 0) ? value : null; }
        }

        private StoryState _state = StoryState.Draft;
        [DataMember(Name = "state", IsRequired = true)]
        public StoryState State
        {
            get { return _state; }
            set { _state = value; }
        }

        private int? _assignedToId;
        [DataMember(Name = "assignedToId", EmitDefaultValue = false)]
        public int? AssignedToId
        {
            get { return _assignedToId; }
            set { _assignedToId = (value.HasValue && value.Value >= 0) ? value : null; }
        }

        private int _order = 0;
        [DataMember(Name = "order", IsRequired = true)]
        public int Order
        {
            get { return _order; }
            set { _order = (value < 0) ? 0 : value; }
        }

        private Collection<int> _preRequisiteIds = new Collection<int>();
        [DataMember(Name = "preRequisiteIds", IsRequired = true)]
        public Collection<int> PreRequisiteIds
        {
            get { return _preRequisiteIds; }
            set { _preRequisiteIds = value ?? new Collection<int>(); }
        }

        public override string ToString() { return this.ToJSON<UserStoryEntity>(); }

        public static UserStoryEntity FromJSON(string jsonText) { return jsonText.FromJSON<UserStoryEntity>(); }
    }
}

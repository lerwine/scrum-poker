using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    public interface IUserStory : ITitleAndIdentifier
    {
        string Description { get; set; }

        string AcceptanceCriteria { get; set; }
        
        Guid? ProjectId { get; set; }
        
        Guid? ThemeId { get; set; }
        
        DateTime Created { get; set; }
        
        int? Points { get; set; }
        
        StoryState State { get; set; }
        
        Guid? AssignedToId { get; set; }
        
        int Order { get; set; }
        
        ICollection<int> PreRequisiteIds { get; set; }
    }
    // TODO: Move to ScrumPoker.StandaloneServer
    [DataContract]
    public class UserStory : TitleAndIdentifier, IUserStory
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

        private Guid? _projectId;
        public Guid? ProjectId
        {
            get { return _projectId; }
            set { _projectId = (value.HasValue && !value.Value.Equals(Guid.Empty)) ? value : null; }
        }

        [DataMember(Name = "projectId", EmitDefaultValue = false)]
        private string __ProjectId
        {
            get { return _projectId.ToJsonString(); }
            set { _projectId = JsonStringToGuidNotEmpty(value); }
        }

        private iGuidnt? _themeId;
        public Guid? ThemeId
        {
            get { return _themeId; }
            set { _themeId = (value.HasValue && !value.Value.Equals(Guid.Empty)) ? value : null; }
        }

        [DataMember(Name = "themeId", EmitDefaultValue = false)]
        private string __ThemeId
        {
            get { return _themeId.ToJsonString(); }
            set { _themeId = JsonStringToGuidNotEmpty(value); }
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

        private Guid? _assignedToId;
        public Guid? AssignedToId
        {
            get { return _assignedToId; }
            set { _assignedToId = (value.HasValue && !value.Value.Equals(Guid.Empty)) ? value : null; }
        }

        [DataMember(Name = "assignedToId", EmitDefaultValue = false)]
        private string __AssignedToId
        {
            get { return _assignedToId.ToJsonString(); }
            set { _assignedToId = JsonStringToGuidNotEmpty(value); }
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
        ICollection<int> IUserStory.PreRequisiteIds { get { return _preRequisiteIds; } }

        public override string ToString() { return this.ToJSON<UserStory>(); }

        public static UserStory FromJSON(string jsonText) { return jsonText.FromJSON<UserStory>(); }
    }
}

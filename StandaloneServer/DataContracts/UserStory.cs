using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    [DataContract]
    public class UserStory : TitleAndIdentifier
    {
        private static readonly PropertyDescriptor _pdDescription;
        private string _description;
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        {
            get { return _description; }
            set
            {
                if (value.ToTrimmedOrNullIfEmpty(SyncRoot, ref _description))
                    RaisePropertyChanged(_pdDescription);
            }
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
#pragma warning disable IDE1006
        private string __ProjectId
#pragma warning restore IDE1006
        {
            get { return _projectId.ToJsonString(); }
            set { _projectId = value.JsonStringToGuidNotEmpty(); }
        }

        private Guid? _themeId;
        public Guid? ThemeId
        {
            get { return _themeId; }
            set { _themeId = (value.HasValue && !value.Value.Equals(Guid.Empty)) ? value : null; }
        }

        [DataMember(Name = "themeId", EmitDefaultValue = false)]
#pragma warning disable IDE1006
        private string __ThemeId
#pragma warning restore IDE1006
        {
            get { return _themeId.ToJsonString(); }
            set { _themeId = value.JsonStringToGuidNotEmpty(); }
        }
        
        private DateTime _created = DateTime.Now;
        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        [DataMember(Name = "created", IsRequired = true)]
#pragma warning disable IDE1006
        private string __Created
#pragma warning restore IDE1006
        {
            get { return _created.ToJsonDateString(); }
            set { _created = value.JsonStringToDate() ?? DateTime.Now; }
        }

        private int? _points;
        [DataMember(Name = "points", EmitDefaultValue = false)]
        public int? Points
        {
            get { return _points; }
            set { _points = (value.HasValue && value.Value >= 0) ? value : null; }
        }

        private ScrumPoker.DataContracts.StoryState _state = ScrumPoker.DataContracts.StoryState.Draft;
        [DataMember(Name = "state", IsRequired = true)]
        public ScrumPoker.DataContracts.StoryState State
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
#pragma warning disable IDE1006
        private string __AssignedToId
#pragma warning restore IDE1006
        {
            get { return _assignedToId.ToJsonString(); }
            set { _assignedToId = value.JsonStringToGuidNotEmpty(); }
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

        public override string ToString() { return this.ToJSON<UserStory>(); }

        public static UserStory FromJSON(string jsonText) { return jsonText.FromJSON<UserStory>(); }
    }
}

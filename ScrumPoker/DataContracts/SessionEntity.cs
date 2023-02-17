using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    [DataContract]
    public class SessionEntity : TitleAndIdentifier
    {
        private string _description;
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        {
            get { return _description; }
            set { _description = value.TrimmedOrNullIfEmpty(); }
        }

        private SprintGrouping _initiative;
        [DataMember(Name = "initiative", EmitDefaultValue = false)]
        public SprintGrouping Initiative
        {
            get { return _initiative; }
            set { _initiative = value; }
        }

        private SprintGrouping _epic;
        [DataMember(Name = "epic", EmitDefaultValue = false)]
        public SprintGrouping Epic
        {
            get { return _epic; }
            set { _epic = value; }
        }

        private SprintGrouping _milestone;
        [DataMember(Name = "milestone", EmitDefaultValue = false)]
        public SprintGrouping Milestone
        {
            get { return _milestone; }
            set { _milestone = value; }
        }

        private DateTime? _plannedStartDate;
        public DateTime? PlannedStartDate
        {
            get { return _plannedStartDate; }
            set { _plannedStartDate = value; }
        }

        [DataMember(Name = "plannedStartDate", EmitDefaultValue = false)]
        private string __PlannedStartDate
        {
            get { return _plannedStartDate.ToJsonString(); }
            set { _plannedStartDate = value.JsonStringToDate(); }
        }

        private DateTime? _plannedEndDate;
        public DateTime? PlannedEndDate
        {
            get { return _plannedEndDate; }
            set { _plannedEndDate = value; }
        }

        [DataMember(Name = "plannedEndDate", EmitDefaultValue = false)]
        private string __PlannedEndDate
        {
            get { return _plannedEndDate.ToJsonString(); }
            set { _plannedEndDate = value.JsonStringToDate(); }
        }

        private int _currentScopePoints = 0;
        [DataMember(Name = "currentScopePoints", IsRequired = true)]
        public int CurrentScopePoints
        {
            get { return _currentScopePoints; }
            set { _currentScopePoints = (value < 0) ? 0 : value; }
        }

        private int? _sprintCapacity;
        [DataMember(Name = "sprintCapacity", EmitDefaultValue = false)]
        public int? SprintCapacity
        {
            get { return _sprintCapacity; }
            set { _sprintCapacity = (value.HasValue && value.Value >= 0) ? value : null; }
        }

        private int _deckId = -1;
        [DataMember(Name = "deckId", IsRequired = true)]
        public int DeckId
        {
            get { return _deckId; }
            set { _deckId = (value < -1) ? -1 : value; }
        }

        private Collection<SprintGrouping> _projects = new Collection<SprintGrouping>();
        [DataMember(Name = "projects", IsRequired = true)]
        public Collection<SprintGrouping> Projects
        {
            get { return _projects; }
            set { _projects = value ?? new Collection<SprintGrouping>(); }
        }

        private Collection<SprintGrouping> _themes = new Collection<SprintGrouping>();
        [DataMember(Name = "themes", IsRequired = true)]
        public Collection<SprintGrouping> Themes
        {
            get { return _themes; }
            set { _themes = value ?? new Collection<SprintGrouping>(); }
        }
        private Collection<UserStoryEntity> _stories = new Collection<UserStoryEntity>();
        [DataMember(Name = "stories", IsRequired = true)]
        public Collection<UserStoryEntity> Stories
        {
            get { return _stories; }
            set { _stories = value ?? new Collection<UserStoryEntity>(); }
        }

        private Collection<DeveloperEntity> _developers = new Collection<DeveloperEntity>();
        [DataMember(Name = "developers", IsRequired = true)]
        public Collection<DeveloperEntity> Developers
        {
            get { return _developers; }
            set { _developers = value ?? new Collection<DeveloperEntity>(); }
        }

        private DeveloperEntity _adminUser;
        [DataMember(Name = "adminUser", IsRequired = true)]
        public DeveloperEntity AdminUser
        {
            get { return _adminUser; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("AdminUser");
                _adminUser = value;
            }
        }

        public override string ToString() { return this.ToJSON<SessionEntity>(); }

        public static SessionEntity FromJSON(string jsonText) { return jsonText.FromJSON<SessionEntity>(); }
    }
}

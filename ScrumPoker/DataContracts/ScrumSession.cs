using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    public interface IScrumSession : ITitleAndIdentifier
    {
        string Description { get; set; }

        ISprintGrouping Initiative { get; set; }
        
        ISprintGrouping Epic { get; set; }
        
        ISprintGrouping Milestone { get; set; }
        
        DateTime? PlannedStartDate { get; set; }
        
        DateTime? PlannedEndDate { get; set; }
        
        int CurrentScopePoints { get; set; }
        
        int? SprintCapacity { get; set; }
        
        Guid DeckId { get; set; }
        
        TeamMember Organizer { get; set; }
        
        ICollection<ITeamMember> Members { get; }
        
        ICollection<ISprintGrouping> Projects { get; }
        
        ICollection<ISprintGrouping> Themes { get; }
        
        ICollection<IUserStory> Stories { get; }
    }
    // TODO: Move to ScrumPoker.StandaloneServer
    [DataContract]
    public class ScrumSession : TitleAndIdentifier, IScrumSession
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

        private Guid _deckId = Guid.Empty;
        public Guid DeckId
        {
            get { return _deckId; }
            set { _deckId = value; }
        }

        [DataMember(Name = "deckId", IsRequired = true)]
        private string __DeckId
        {
            get { return _deckId.ToJsonString() : null; }
            set { _deckId = JsonStringToGuid(value) ?? Guid.Empty; }
        }

        private TeamMember _organizer;
        [DataMember(Name = "organizer", IsRequired = true)]
        public TeamMember Organizer
        {
            get { return _organizer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Organizer");
                _organizer = value;
            }
        }

        private readonly GenericCollectionWrapper<ITeamMember, TeamMember, Collection<ITeamMember>> _members = new GenericCollectionWrapper<ITeamMember, TeamMember, Collection<ITeamMember>>();
        [DataMember(Name = "members", IsRequired = true)]
        public Collection<TeamMember> Members
        {
            get { return _members.Source; }
            set { _members.Source = value }
        }
        ICollection<ITeamMember> IScrumSession.Members { get { return _members; } }

        private readonly GenericCollectionWrapper<ISprintGrouping, SprintGrouping, Collection<SprintGrouping>> _projects = new GenericCollectionWrapper<ISprintGrouping, SprintGrouping, Collection<SprintGrouping>>();
        [DataMember(Name = "projects", IsRequired = true)]
        public Collection<SprintGrouping> Projects
        {
            get { return _projects.Source; }
            set { _projects.Source = value }
        }
        ICollection<ISprintGrouping> IScrumSession.Projects { get { return _projects; } }

        private readonly GenericCollectionWrapper<ISprintGrouping, SprintGrouping, Collection<SprintGrouping>> _themes = new GenericCollectionWrapper<ISprintGrouping, SprintGrouping, Collection<SprintGrouping>>();
        [DataMember(Name = "themes", IsRequired = true)]
        public Collection<SprintGrouping> Themes
        {
            get { return _themes.Source; }
            set { _themes.Source = value }
        }
        ICollection<ISprintGrouping> IScrumSession.Themes { get { return _themes; } }

        private readonly GenericCollectionWrapper<IUserStory, UserStory, Collection<UserStory>> _stories = new GenericCollectionWrapper<IUserStory, UserStory, Collection<UserStory>>();
        [DataMember(Name = "stories", IsRequired = true)]
        public Collection<UserStory> Stories
        {
            get { return _stories.Source; }
            set { _stories.Source = value }
        }
        ICollection<IUserStory> IScrumSession.Stories { get { return _stories; } }
        
        public override string ToString() { return this.ToJSON<ScrumSession>(); }

        public static ScrumSession FromJSON(string jsonText) { return jsonText.FromJSON<ScrumSession>(); }
    }
}

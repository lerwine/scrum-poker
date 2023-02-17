using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ScrumPokerServer.DataContracts
{
    public static class StringExtensions
    {
        public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, false);

        public static string EmptyIfNullOrTrimmed(this string value) { return (value == null) ? "" : value.Trim(); }

        public static string TrimmedOrNullIfEmpty(this string value) { return (value != null && (value = value.Trim()).Length > 0) ? value : null; }

        public static string ToJsonString(this DateTime value) { return value.ToString("yyyy-MM-dd"); }
        
        public static string ToJsonString(this DateTime? value) { return value.HasValue ? ToJsonString(value.Value) : null; }
        
        public static DateTime? JsonStringToDate(this string value)
        {
            DateTime result;
            if ((value = value.TrimmedOrNullIfEmpty()) != null && DateTime.TryParse(value, out result))
                return result;
            return null;
        }
        
        public static T FromJSON<T>(this string jsonText) where T : class, new()
        {
            using (MemoryStream stream = new MemoryStream(DefaultEncoding.GetBytes(jsonText)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return serializer.ReadObject(stream) as T;
            }
        }

        public static string ToJSON<T>(this T value) where T : class, new()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, value);
                byte[] bytes = stream.ToArray();
                return DefaultEncoding.GetString(bytes, 0, bytes.Length);
            }
        }
    }

    [DataContract]
    public abstract class TitleAndIdentifier
    {
        private string _title = "";
        [DataMember(Name = "title", IsRequired = true)]
        public string Title
        {
            get { return _title; }
            set { _title = value.EmptyIfNullOrTrimmed(); }
        }

        private string _identifier = null;
        [DataMember(Name = "identifier", EmitDefaultValue = false)]
        public string Identifier
        {
            get { return _identifier; }
            set { _identifier = value.TrimmedOrNullIfEmpty(); }
        }
    }

    public enum StoryState
    {
        Draft = 0,
        Ready = 1,
        Cancelled = 2
    }

    [DataContract]
    public class SprintGrouping : TitleAndIdentifier
    {
        private string _description;
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        {
            get { return _description; }
            set { _description = value.TrimmedOrNullIfEmpty(); }
        }

        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        [DataMember(Name = "startDate", EmitDefaultValue = false)]
        private string __StartDate
        {
            get { return _startDate.ToJsonString(); }
            set { _startDate = value.JsonStringToDate(); }
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

        public override string ToString() { return this.ToJSON<SprintGrouping>(); }

        public static SprintGrouping FromJSON(string jsonText) { return jsonText.FromJSON<SprintGrouping>(); }
    }

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

    [DataContract]
    public class Developer
    {
        private readonly object _syncRoot = new object();
        
        private string _displayName = "";
        [DataMember(Name = "displayName", IsRequired = true)]
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value.EmptyIfNullOrTrimmed(); }
        }

        private string _userName;
        [DataMember(Name = "userName", EmitDefaultValue = false)]
        public string UserName
        {
            get { return _isParticipant ? _userName ?? "" : null; }
            set { _userName = value.TrimmedOrNullIfEmpty(); }
        }

        private int? _colorId;
        [DataMember(Name = "colorId", EmitDefaultValue = false)]
        public int? ColorId
        {
            get { return _isParticipant ? (int?)(_colorId ?? -1) : null; }
            set { _colorId = (value.HasValue && value.Value >= 0) ? value : null; }
        }

        private int? _sprintCapacity;
        [DataMember(Name = "sprintCapacity", EmitDefaultValue = false)]
        public int? SprintCapacity
        {
            get { return _sprintCapacity; }
            set { _sprintCapacity = (value.HasValue && value.Value >= 0) ? value : null; }
        }

        private bool _isParticipant = false;
        [DataMember(Name = "isParticipant", EmitDefaultValue = false)]
        public bool IsParticipant
        {
            get { return _isParticipant; }
            set { _isParticipant = true; }
        }

        public override string ToString() { return this.ToJSON<Developer>(); }

        public static Developer FromJSON(string jsonText) { return jsonText.FromJSON<Developer>(); }
    }

    [DataContract]
    public class DeveloperEntity : Developer
    {
        private int? _selectedCardId;
        [DataMember(Name = "selectedCardId", EmitDefaultValue = false)]
        public int? SelectedCardId
        {
            get { return IsParticipant ? _selectedCardId : null; }
            set { _selectedCardId = value; }
        }

        private int _assignedPoints = 0;
        [DataMember(Name = "assignedPoints", IsRequired = true)]
        public int AssignedPoints
        {
            get { return _assignedPoints; }
            set { _assignedPoints = (value < 0) ? 0 : value; }
        }

        public override string ToString() { return this.ToJSON<DeveloperEntity>(); }

        public static new DeveloperEntity FromJSON(string jsonText) { return jsonText.FromJSON<DeveloperEntity>(); }
    }
}
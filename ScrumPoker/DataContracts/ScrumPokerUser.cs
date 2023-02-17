using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    public interface IScrumPokerUser
    {
        string DisplayName { get; set; }

        string UserName { get; set; }
        
        Guid? ColorId { get; set; }
        
        int? SprintCapacity { get; set; }
        
        bool IsParticipant { get; set; }
    }
    // TODO: Move to ScrumPoker.StandaloneServer
    [DataContract]
    public class ScrumPokerUser : IScrumPokerUser
    {
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

        private Guid? _colorId;
        public Guid? ColorId
        {
            get { return _isParticipant ? (Guid?)(_colorId ?? Guid.Empty) : null; }
            set { _colorId = (value.HasValue && !value.Value.Equals(Guid.Empty)) ? value : null; }
        }

        [DataMember(Name = "colorId", EmitDefaultValue = false)]
        private string __ColorId
        {
            get { return _colorId ? _selectedCardId.ToJsonString() : null; }
            set { _colorId = JsonStringToGuidNotEmpty(value); }
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

        public ScrumPokerUser() { }

        public ScrumPokerUser(string login)
        {
            if (string.IsNullOrEmpty(login))
                return;
            _userName = login;
            int index = login.IndexOf('/');
            _displayName = (index < 0 || (login = login.Substring(index + 1).Trim()).Length == 0) ? _userName : login;
        }
        
        public ScrumPokerUser(string login, string displayName)
        {
            _displayName = displayName.EmptyIfNullOrTrimmed();
            _userName = login.TrimmedOrNullIfEmpty();
        }
        
        public ScrumPokerUser(string login, string password, string displayName)
        {
            _displayName = displayName.EmptyIfNullOrTrimmed();
            _userName = login.TrimmedOrNullIfEmpty();
        }
        
        public ScrumPokerUser(ScrumPokerUser cloneFrom)
        {
            if (cloneFrom == null)
                throw new ArgumentNullException("cloneFrom");
            _colorId = cloneFrom._colorId;
            _displayName = cloneFrom._displayName;
            _isParticipant = cloneFrom._isParticipant;
            _sprintCapacity = cloneFrom._sprintCapacity;
            _userName = cloneFrom._userName;
        }
        
        public override string ToString() { return this.ToJSON<ScrumPokerUser>(); }

        public static ScrumPokerUser FromJSON(string jsonText) { return jsonText.FromJSON<ScrumPokerUser>(); }
    }
}

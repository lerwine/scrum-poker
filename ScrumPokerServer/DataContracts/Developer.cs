using System;
using System.Runtime.Serialization;

namespace ScrumPokerServer.DataContracts
{
    [DataContract]
    public class Developer
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

        public Developer() { }

        public Developer(string login)
        {
            if (string.IsNullOrEmpty(login))
                return;
            _userName = login;
            int index = login.IndexOf('/');
            _displayName = (index < 0 || (login = login.Substring(index + 1).Trim()).Length == 0) ? _userName : login;
        }
        
        public Developer(string login, string displayName)
        {
            _displayName = displayName.EmptyIfNullOrTrimmed();
            _userName = login.TrimmedOrNullIfEmpty();
        }
        
        public Developer(string login, string password, string displayName)
        {
            _displayName = displayName.EmptyIfNullOrTrimmed();
            _userName = login.TrimmedOrNullIfEmpty();
        }
        
        public Developer(Developer cloneFrom)
        {
            if (cloneFrom == null)
                throw new ArgumentNullException("cloneFrom");
            _colorId = cloneFrom._colorId;
            _displayName = cloneFrom._displayName;
            _isParticipant = cloneFrom._isParticipant;
            _sprintCapacity = cloneFrom._sprintCapacity;
            _userName = cloneFrom._userName;
        }
        
        public override string ToString() { return this.ToJSON<Developer>(); }

        public static Developer FromJSON(string jsonText) { return jsonText.FromJSON<Developer>(); }
    }
}

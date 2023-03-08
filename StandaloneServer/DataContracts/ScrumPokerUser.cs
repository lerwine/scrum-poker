using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    [DataContract]
    public class ScrumPokerUser : ValidatableObject
    {
        private static readonly PropertyDescriptor _pdDisplayName;
        private string _displayName = "";
        [DataMember(Name = "displayName", IsRequired = true)]
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value.ToWsNormalized(SyncRoot, ref _displayName))
                    RaisePropertyChanged(_pdDisplayName);
            }
        }

        private static readonly PropertyDescriptor _pdUserName;
        private string _userName;
        [DataMember(Name = "userName", EmitDefaultValue = false)]
        public string UserName
        {
            get { return _isParticipant ? _userName ?? "" : null; }
            set
            {
                if (value.ToTrimmedOrNullIfEmpty(SyncRoot, ref _userName))
                    RaisePropertyChanged(_pdUserName);
            }
        }

        private static readonly PropertyDescriptor _pdColorId;
        private Guid? _colorId;
        public Guid? ColorId
        {
            get { return _isParticipant ? (Guid?)(_colorId ?? Guid.Empty) : null; }
            set
            {
                if (value.ToNullIfEmpty(SyncRoot, ref _colorId))
                    RaisePropertyChanged(_pdColorId);
            }
        }

        [DataMember(Name = "colorId", EmitDefaultValue = false)]
#pragma warning disable IDE1006, IDE0051
        private string __ColorId
#pragma warning restore IDE1006, IDE0051
        {
            get { return _isParticipant ? _colorId.ToJsonString() : null; }
            set
            {
                if (value.JsonStringToGuidNotEmpty().ToNullIfEmpty(SyncRoot, ref _colorId))
                    RaisePropertyChanged(_pdColorId);
            }
        }

        private static readonly PropertyDescriptor _pdSprintCapacity;
        private int? _sprintCapacity;
        [DataMember(Name = "sprintCapacity", EmitDefaultValue = false)]
        [Range(1, int.MaxValue)]
        public int? SprintCapacity
        {
            get { return _sprintCapacity; }
            set
            {
                if (value.SetIfDifferent(SyncRoot, ref _sprintCapacity))
                    RaisePropertyChanged(_pdSprintCapacity);
            }
        }

        private bool _isParticipant = false;

        private static readonly PropertyDescriptor _pdIsParticipant;

        [DataMember(Name = "isParticipant", EmitDefaultValue = false)]
        public bool IsParticipant
        {
            get { return _isParticipant; }
            set
            {
                if (value.SetIfDifferent(SyncRoot, ref _isParticipant))
                    RaisePropertyChanged(_pdIsParticipant);
            }
        }

        static ScrumPokerUser()
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(ScrumPokerUser));
            _pdDisplayName = pdc["DisplayName"];
            _pdUserName = pdc["UserName"];
            _pdColorId = pdc["ColorId"];
            _pdSprintCapacity = pdc["SprintCapacity"];
            _pdIsParticipant = pdc["IsParticipant"];
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
            _displayName = displayName.WsNormalizedOrEmptyIfNull();
            _userName = login.TrimmedOrNullIfEmpty();
        }

        public ScrumPokerUser(string login, string password, string displayName)
        {
            _displayName = displayName.WsNormalizedOrEmptyIfNull();
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

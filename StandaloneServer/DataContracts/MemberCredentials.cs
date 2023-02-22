using System;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace ScrumPoker.StandaloneServer.DataContracts
{
    public class MemberCredentials : ScrumPokerUser, ICloneable
    {
        private readonly PropertyDescriptor _pdPassword = TypeDescriptor.GetProperties(typeof(MemberCredentials))["Password"];
        private string _password;
        [DataMember(Name = "password", EmitDefaultValue = false)]
        public string Password
        {
            get { return _password; }
            set
            {
                if (value.ToNullIfEmpty(SyncRoot, ref _password))
                    RaisePropertyChanged(_pdPassword);
            }
        }

        public MemberCredentials() { }

        public MemberCredentials(MemberCredentials cloneFrom)
        {
            if (cloneFrom == null)
                throw new ArgumentNullException("cloneFrom");
            _password = cloneFrom._password;
        }

        public MemberCredentials Clone() { return new MemberCredentials(this); }

        object ICloneable.Clone() { return Clone(); }
    }
}

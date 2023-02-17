using System;
using System.Runtime.Serialization;

namespace ScrumPoker.StandaloneServer
{
    public class MemberCredentials : ScrumPoker.DataContracts.ScrumPokerUser, ICloneable
    {
        private string _password;
        [DataMember(Name = "password", EmitDefaultValue = false)]
        public string Password
        {
            get { return _password; }
            set { _password = (value == null || value.Length > 0) ? value : null; }
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

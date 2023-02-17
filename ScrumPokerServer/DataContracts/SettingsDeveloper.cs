using System;
using System.Runtime.Serialization;

namespace ScrumPokerServer.DataContracts
{
    public class SettingsDeveloper : Developer, ICloneable
    {
        private string _password;
        [DataMember(Name = "password", EmitDefaultValue = false)]
        public string Password
        {
            get { return _password; }
            set { _password = (value == null || value.Length > 0) ? value : null; }
        }

        public SettingsDeveloper() { }

        public SettingsDeveloper(SettingsDeveloper cloneFrom)
        {
            if (cloneFrom == null)
                throw new ArgumentNullException("cloneFrom");
            _password = cloneFrom._password;
        }

        public SettingsDeveloper Clone() { return new SettingsDeveloper(this); }

        object ICloneable.Clone() { return Clone(); }
    }
}

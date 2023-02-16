using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ScrumPokerServer.DataContracts
{
    [DataContract]
    public class HostSettings
    {
        private string _webRootPath;
        [DataMember(Name = "webRootPath", EmitDefaultValue = false)]
        public string WebRootPath
        {
            get { return _webRootPath; }
            set { _webRootPath = value.TrimmedOrNullIfEmpty(); }
        }
        
        [Obsolete("use WebRootPath")]
        internal string webRootPath;

        private int? _portNumber;
        [DataMember(Name = "portNumber", EmitDefaultValue = false)]
        private int? __PortNumber
        {
            get { return _portNumber; }
            set { _portNumber = (value.HasValue && value.Value == 8080) ? null : value; }
        }
        public int PortNumber
        {
            get { return _portNumber ?? 8080 ; }
            set { __PortNumber = value; }
        }
        
        [Obsolete("use PortNumber")]
        internal int portNumber = 8080;

        private Developer _adminUser;
        [DataMember(Name = "adminUser", required = true)]
        public Developer AdminUser
        {
            get { return _adminUser; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _adminUser = value;
            }
        }
        
        [Obsolete("use AdminUser")]
        internal AdminUser adminUser;

        private Collection<Developer> _developers = new Collection<Developer>();
        [DataMember(Name = "developers", IsRequired = true)]
        public Collection<Developer> Developers
        {
            get { return _developers; }
            set { _developers = value ?? new Collection<Developer>(); }
        }

        [Obsolete("use Developers")]
        internal IWebAppUser[] participants;

        private AuthenticationSchemes _authentication = AuthenticationSchemes.Negotiate;
        public AuthenticationSchemes Authentication
        {
            get { return _authentication; }
            set
            {
                switch (value)
                {
                    case AuthenticationSchemes.Negotiate:
                    case AuthenticationSchemes.Digest:
                    case AuthenticationSchemes.IntegratedWindowsAuthentication:
                    case AuthenticationSchemes.Ntlm:
                        _authentication = authentication;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        [DataMember(Name = "authentication", Required = false, EmitDefaultValue = false)]
        private string __Authentication
        {
            get { return (_authentication == AuthenticationSchemes.Negotiate) ? null : _authentication.ToString("F"); }
            set
            {
                string s = value.TrimmedOrNullIfEmpty();
                if (s == null)
                    _authentication = AuthenticationSchemes.Negotiate;
                else
                {
                    AuthenticationSchemes authentication;
                    if (!Enum.TryParse(value, out authentication))
                        throw new ArgumentOutOfRangeException("value");
                    Authentication = value;
                }
            }
        }

        [Obsolete("use Authentication")]
        internal string authentication;

        private readonly static Encoding _serializationEncoding = new UTF8Encoding(false, false);

        public override string ToString()
        {
            byte[] data;
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(HostSettings));
                serializer.WriteObject(stream, this);
                data = ms.ToArray();
            }
            return _serializationEncoding.GetString(json, 0, json.Length);
        }

        public static HostSettings FromJSON(string jsonText)
        {
            using (MemoryStream stream = new MemoryStream(_serializationEncoding.GetBytes(jsonText)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(HostSettings));
                return serializer.ReadObject(stream) as HostSettings;
            }
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ScrumPoker.StandaloneServer
{
    [DataContract]
    public class HostSettings : ICloneable
    {
        private string _webRootPath;
        [DataMember(Name = "webRootPath", EmitDefaultValue = false)]
        public string WebRootPath
        {
            get { return _webRootPath; }
            set { _webRootPath = value.TrimmedOrNullIfEmpty(); }
        }
        
        public const int DEFAULT_PORT_NUMBER = 8080;

        private int? _portNumber;
        [DataMember(Name = "portNumber", EmitDefaultValue = false)]
        private int? __PortNumber
        {
            get { return _portNumber; }
            set
            {
                if (value.HasValue && value.Value != DEFAULT_PORT_NUMBER)
                {
                    if (value.Value < 0 || value.Value > 65535)
                        throw new ArgumentOutOfRangeException("value");
                    _portNumber = value.Value; 
                }
                else
                 _portNumber = null;
            }
        }

        public int PortNumber
        {
            get { return _portNumber ?? DEFAULT_PORT_NUMBER ; }
            set { __PortNumber = value; }
        }
        
        private MemberCredentials _adminUser;
        [DataMember(Name = "adminUser", IsRequired = true)]
        public MemberCredentials AdminUser
        {
            get { return _adminUser; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _adminUser = value;
            }
        }
        
        private Collection<MemberCredentials> _members = new Collection<MemberCredentials>();
        [DataMember(Name = "members", IsRequired = true)]
        public Collection<MemberCredentials> ScrumPokerUsers
        {
            get { return _members; }
            set { _members = value ?? new Collection<MemberCredentials>(); }
        }

        private bool _useIntegratedWindowsAuthentication = false;
        [DataMember(Name = "useIntegratedWindowsAuthentication", IsRequired = false, EmitDefaultValue = false)]
        public bool UseIntegratedWindowsAuthentication
        {
            get { return _useIntegratedWindowsAuthentication; }
            set { _useIntegratedWindowsAuthentication = value; }
        }

        private readonly static Encoding _serializationEncoding = new UTF8Encoding(false, false);

        public override string ToString()
        {
            byte[] data;
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(HostSettings));
                serializer.WriteObject(stream, this);
                data = stream.ToArray();
            }
            return _serializationEncoding.GetString(data, 0, data.Length);
        }

        public static HostSettings FromJSON(string jsonText)
        {
            using (MemoryStream stream = new MemoryStream(_serializationEncoding.GetBytes(jsonText)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(HostSettings));
                return serializer.ReadObject(stream) as HostSettings;
            }
        }

        public HostSettings Clone()
        {
            HostSettings result = new HostSettings
            {
                _adminUser = (_adminUser == null) ? null : _adminUser.Clone(),
                _members = _members,
                _portNumber = _portNumber,
                _useIntegratedWindowsAuthentication = _useIntegratedWindowsAuthentication,
                _webRootPath = _webRootPath,
            };
            foreach (MemberCredentials d in _members)
                if (d != null)
                    result._members.Add(d.Clone());
            return result;
        }

        object ICloneable.Clone() { return Clone(); }
    }
}
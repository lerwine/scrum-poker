using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts
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
        
        private SettingsDeveloper _adminUser;
        [DataMember(Name = "adminUser", IsRequired = true)]
        public SettingsDeveloper AdminUser
        {
            get { return _adminUser; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _adminUser = value;
            }
        }
        
        private Collection<SettingsDeveloper> _developers = new Collection<SettingsDeveloper>();
        [DataMember(Name = "developers", IsRequired = true)]
        public Collection<SettingsDeveloper> Developers
        {
            get { return _developers; }
            set { _developers = value ?? new Collection<SettingsDeveloper>(); }
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
                _developers = _developers,
                _portNumber = _portNumber,
                _useIntegratedWindowsAuthentication = _useIntegratedWindowsAuthentication,
                _webRootPath = _webRootPath,
            };
            foreach (SettingsDeveloper d in _developers)
                if (d != null)
                    result._developers.Add(d.Clone());
            return result;
        }

        object ICloneable.Clone() { return Clone(); }
    }
}
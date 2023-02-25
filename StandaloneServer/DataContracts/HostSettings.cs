using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    [DataContract]
    public class HostSettings : ValidatableObject, ICloneable
    {
        private static readonly PropertyDescriptor _pdWebRootPath;
        private string _webRootPath = "";
        [DataMember(Name = "webRootPath", IsRequired = true)]
        [Required]
        public string WebRootPath
        {
            get { return _webRootPath; }
            set
            {
                if (value.ToEmptyIfNullOrTrimmed(SyncRoot, ref _webRootPath))
                    RaisePropertyChanged(_pdWebRootPath);
            }
        }
        
        public const int DEFAULT_PORT_NUMBER = 8080;
        private static readonly PropertyDescriptor _pdPortNumber;
        private int? _portNumber;
        [DataMember(Name = "portNumber", EmitDefaultValue = false)]
        [Range(1, 65535)]
#pragma warning disable IDE1006
        private int? __PortNumber
#pragma warning restore IDE1006
        {
            get { return _portNumber; }
            set
            {
                if (((value.HasValue && value.Value != DEFAULT_PORT_NUMBER) ? value : null).SetIfDifferent(SyncRoot, ref _portNumber))
                    RaisePropertyChanged(_pdPortNumber);
            }
        }

        public int PortNumber
        {
            get { return _portNumber ?? DEFAULT_PORT_NUMBER ; }
            set { __PortNumber = value; }
        }
        
        private static readonly PropertyDescriptor _pdAdminUser;
        private MemberCredentials _adminUser;
        [DataMember(Name = "adminUser", IsRequired = true)]
        [Required]
        public MemberCredentials AdminUser
        {
            get { return _adminUser; }
            set
            {
                if (value.SetIfDifferentObject(SyncRoot, ref _adminUser))
                    RaisePropertyChanged(_pdAdminUser);
            }
        }
        
        private static readonly PropertyDescriptor _pdScrumPokerUsers;
        private Collection<MemberCredentials> _members = new Collection<MemberCredentials>();
        [DataMember(Name = "members", IsRequired = true)]
        [Required]
        public Collection<MemberCredentials> ScrumPokerUsers
        {
            get { return _members; }
            set
            {
                if (value.SetIfDifferentObject(SyncRoot, ref _members))
                    RaisePropertyChanged(_pdScrumPokerUsers);
            }
        }

        private bool _useIntegratedWindowsAuthentication = false;
        private static readonly PropertyDescriptor _pdUseIntegratedWindowsAuthentication;

        [DataMember(Name = "useIntegratedWindowsAuthentication", IsRequired = false, EmitDefaultValue = false)]
        public bool UseIntegratedWindowsAuthentication
        {
            get { return _useIntegratedWindowsAuthentication; }
            set
            {
                if (value.SetIfDifferent(SyncRoot, ref _useIntegratedWindowsAuthentication))
                    RaisePropertyChanged(_pdUseIntegratedWindowsAuthentication);
            }
        }

        private readonly static Encoding _serializationEncoding = new UTF8Encoding(false, false);

        static HostSettings()
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(HostSettings));
            _pdWebRootPath = pdc["WebRootPath"];
            _pdPortNumber = pdc["PortNumber"];
            _pdAdminUser = pdc["AdminUser"];
            _pdScrumPokerUsers = pdc["ScrumPokerUsers"];
            _pdUseIntegratedWindowsAuthentication = pdc["UseIntegratedWindowsAuthentication"];
        }

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
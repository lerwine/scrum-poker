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
        [DataMember(IsRequired = true)]
        internal string webRootPath;

        [DataMember(IsRequired = false)]
        internal int portNumber = 8080;

        [DataMember(IsRequired = false)]
        internal AdminUser adminUser;

        [DataMember(IsRequired = true)]
        internal WebAppUser[] participants;

        [DataMember(IsRequired = false)]
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.User
{
    [DataContract]
    public class ColorSchemeListItem
    {
        [DataMember(Name = "schemeId", IsRequired = true)]
        #pragma warning disable IDE0051, IDE1006
        private string __SchemeId
        #pragma warning restore IDE0051, IDE1006
        {
            get { return _schemeId.ToJsonString(); }
            set { _schemeId = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private Guid _schemeId;
        public Guid SchemeId
        {
            get { return _schemeId; }
            set { _schemeId = value; }
        }

        private string _name = "";
        [DataMember(Name = "name", IsRequired = true)]
        public string Name
        {
            get { return _name; }
            set { _name = value.EmptyIfNullOrTrimmed(); }
        }

        private string _fill = "";
        [DataMember(Name = "fill", IsRequired = true)]
        public string Fill
        {
            get { return _fill; }
            set { _fill = value.EmptyIfNullOrTrimmed(); }
        }

        private string _stroke = "";
        [DataMember(Name = "stroke", IsRequired = true)]
        public string Stroke
        {
            get { return _stroke; }
            set { _stroke = value.EmptyIfNullOrTrimmed(); }
        }

        private string _text = "";
        [DataMember(Name = "text", IsRequired = true)]
        public string Text
        {
            get { return _text; }
            set { _text = value.EmptyIfNullOrTrimmed(); }
        }
    }
}
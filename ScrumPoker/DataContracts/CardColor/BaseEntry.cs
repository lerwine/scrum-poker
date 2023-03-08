using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.CardColor
{
    [DataContract]
    public class BaseEntry
    {
        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember(Name = "id", IsRequired = true)]
#pragma warning disable IDE0051, IDE1006
        private string __Id
#pragma warning restore IDE0051, IDE1006
        {
            get { return _id.ToJsonString(); }
            set { _id = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private string _name = "";
        /// <summary>
        /// The name of the color.
        /// </summary>
        [DataMember(Name = "name", IsRequired = true)]
        public string Name
        {
            get { return _name; }
            set { _name = value.WsNormalizedOrEmptyIfNull(); }
        }

        private string _fill = "";
        /// <summary>
        /// The fill color.
        /// </summary>
        [DataMember(Name = "fill", IsRequired = true)]
        public string Fill
        {
            get { return _fill; }
            set { _fill = value.EmptyIfNullOrTrimmed(); }
        }

        private string _stroke = "";
        /// <summary>
        /// The stroke color.
        /// </summary>
        [DataMember(Name = "stroke", IsRequired = true)]
        public string Stroke
        {
            get { return _stroke; }
            set { _stroke = value.EmptyIfNullOrTrimmed(); }
        }

        private string _text = "";
        /// <summary>
        /// The color for the text.
        /// </summary>
        [DataMember(Name = "text", IsRequired = true)]
        public string Text
        {
            get { return _text; }
            set { _text = value.EmptyIfNullOrTrimmed(); }
        }
    }
}

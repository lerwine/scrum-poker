using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// Represents a card color.
    /// </summary>
    [DataContract]
    public class CardColorListItem
    {
        [DataMember(Name = "colorId", IsRequired = true)]
        #pragma warning disable IDE0051, IDE1006
        private string __ColorId
        #pragma warning restore IDE0051, IDE1006
        {
            get { return _colorId.ToJsonString(); }
            set { _colorId = value.JsonStringToGuid() ?? Guid.Empty; }
        }

        private Guid _colorId;
        /// <summary>
        /// The unique identifier of the card color.
        /// </summary>
        public Guid ColorId
        {
            get { return _colorId; }
            set { _colorId = value; }
        }

        private string _name = "";
        /// <summary>
        /// The name of the color.
        /// </summary>
        [DataMember(Name = "name", IsRequired = true)]
        public string Name
        {
            get { return _name; }
            set { _name = value.WsNormalized(); }
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
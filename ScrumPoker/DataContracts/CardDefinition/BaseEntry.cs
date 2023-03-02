using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.CardDefinition
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

        private string _title = "";
        [DataMember(Name = "title", IsRequired = true)]
        public string Title
        {
            get { return _title; }
            set { _title = value.WsNormalized(); }
        }

        private string _symbolText = "";
        [DataMember(Name = "symbolText", IsRequired = true)]
        public string SymbolText
        {
            get { return _symbolText; }
            set { _symbolText = value.EmptyIfNullOrTrimmed(); }
        }

        private string _symbolFont;
        [DataMember(Name = "symbolFont", EmitDefaultValue = false)]
        public string SymbolFont
        {
            get { return _symbolFont; }
            set { _symbolFont = value.TrimmedOrNullIfEmpty(); }
        }

        private string _upperSymbolPath;
        [DataMember(Name = "upperSymbolPath", EmitDefaultValue = false)]
        public string UpperSymbolPath
        {
            get { return _upperSymbolPath; }
            set { _upperSymbolPath = value.TrimmedOrNullIfEmpty(); }
        }

        private string _middleSymbolPath;
        [DataMember(Name = "middleSymbolPath", EmitDefaultValue = false)]
        public string MiddleSymbolPath
        {
            get { return _middleSymbolPath; }
            set { _middleSymbolPath = value.TrimmedOrNullIfEmpty(); }
        }

        private string _lowerSymbolPath;
        [DataMember(Name = "lowerSymbolPath", EmitDefaultValue = false)]
        public string LowerSymbolPath
        {
            get { return _lowerSymbolPath; }
            set { _lowerSymbolPath = value.TrimmedOrNullIfEmpty(); }
        }

        private string _description;
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        {
            get { return _description; }
            set { _description = value.TrimmedOrNullIfEmpty(); }
        }

        private string _truncatedDescription;
        [DataMember(Name = "truncatedDescription", EmitDefaultValue = false)]
        public string TruncatedDescription
        {
            get { return _truncatedDescription; }
            set { _truncatedDescription = value.TrimmedOrNullIfEmpty(); }
        }

        private int? _value;
        [DataMember(Name = "value", EmitDefaultValue = false)]
        public int? Value
        {
            get { return _value; }
            set { _value = (value.HasValue && value.Value > -1) ? value : null; }
        }

        private int? _middleSymbolTop;
        [DataMember(Name = "middleSymbolTop", EmitDefaultValue = false)]
        public int? MiddleSymbolTop
        {
            get { return _middleSymbolTop; }
            set { _middleSymbolTop = (value.HasValue && value.Value > -1) ? value : null; }
        }

        private int? _smallSymbolFontSize;
        [DataMember(Name = "smallSymbolFontSize", EmitDefaultValue = false)]
        public int? SmallSymbolFontSize
        {
            get { return _smallSymbolFontSize; }
            set { _smallSymbolFontSize = (value.HasValue && value.Value > 0) ? value : null; }
        }

        private int? _largeSymbolFontSize;
        [DataMember(Name = "largeSymbolFontSize", EmitDefaultValue = false)]
        public int? LargeSymbolFontSize
        {
            get { return _largeSymbolFontSize; }
            set { _largeSymbolFontSize = (value.HasValue && value.Value > 0) ? value : null; }
        }

        private CardType _type;
        [DataMember(Name = "type", IsRequired = true)]
        public CardType Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}
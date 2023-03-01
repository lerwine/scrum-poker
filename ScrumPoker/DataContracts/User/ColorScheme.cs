using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.User
{
    /// <summary>
    /// Defines a color scheme.
    /// </summary>
    [DataContract]
    public class ColorScheme
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
        /// <summary>
        /// The unique identifier of the color scheme.
        /// </summary>
        public Guid SchemeId
        {
            get { return _schemeId; }
            set { _schemeId = value; }
        }

        private string _name = "";
        /// <summary>
        /// The name of the color scheme.
        /// </summary>
        [DataMember(Name = "name", IsRequired = true)]
        public string Name
        {
            get { return _name; }
            set { _name = value.WsNormalized(); }
        }

        private string _votingFill = "";
        /// <summary>
        /// The fill color for the "voting" card.
        /// </summary>
        [DataMember(Name = "fill", IsRequired = true)]
        public string VotingFill
        {
            get { return _votingFill; }
            set { _votingFill = value.EmptyIfNullOrTrimmed(); }
        }

        private string _votingStroke = "";
        /// <summary>
        /// The stroke color for the "voting" card.
        /// </summary>
        [DataMember(Name = "stroke", IsRequired = true)]
        public string VotingStroke
        {
            get { return _votingStroke; }
            set { _votingStroke = value.EmptyIfNullOrTrimmed(); }
        }

        private string _votingText = "";
        /// <summary>
        /// The color for the text for the "voting" card.
        /// </summary>
        [DataMember(Name = "text", IsRequired = true)]
        public string VotingText
        {
            get { return _votingText; }
            set { _votingText = value.EmptyIfNullOrTrimmed(); }
        }

        private Collection<CardColorListItem> _cardColors = new Collection<CardColorListItem>();
        /// <summary>
        /// Card deck colors.
        /// </summary>
        [DataMember(Name = "cardColors", IsRequired = true)]
        public Collection<CardColorListItem> CardColors
        {
            get { return _cardColors
            ; }
            set { _cardColors
             = value ?? new Collection<CardColorListItem>(); }
        }
    }
}
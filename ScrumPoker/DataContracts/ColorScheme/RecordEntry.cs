using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.ColorScheme
{
    [DataContract]
    public class RecordEntry
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
        /// The name of the color scheme.
        /// </summary>
        [DataMember(Name = "name", IsRequired = true)]
        public string Name
        {
            get { return _name; }
            set { _name = value.WsNormalizedOrEmptyIfNull(); }
        }

        private string _votingFill = "";
        /// <summary>
        /// The fill color for the "voting" card.
        /// </summary>
        [DataMember(Name = "votingFill", IsRequired = true)]
        public string VotingFill
        {
            get { return _votingFill; }
            set { _votingFill = value.EmptyIfNullOrTrimmed(); }
        }

        private string _votingStroke = "";
        /// <summary>
        /// The stroke color for the "voting" card.
        /// </summary>
        [DataMember(Name = "votingStroke", IsRequired = true)]
        public string VotingStroke
        {
            get { return _votingStroke; }
            set { _votingStroke = value.EmptyIfNullOrTrimmed(); }
        }

        private string _votingText = "";
        /// <summary>
        /// The color for the text for the "voting" card.
        /// </summary>
        [DataMember(Name = "votingText", IsRequired = true)]
        public string VotingText
        {
            get { return _votingText; }
            set { _votingText = value.EmptyIfNullOrTrimmed(); }
        }
    }
}

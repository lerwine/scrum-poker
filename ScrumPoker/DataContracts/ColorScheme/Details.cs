using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.ColorScheme
{
    [DataContract]
    public class Details : RecordEntry
    {
        private Collection<CardColor.BaseEntry> _cardColors = new Collection<CardColor.BaseEntry>();
        /// <summary>
        /// Card deck colors.
        /// </summary>
        [DataMember(Name = "cardColors", IsRequired = true)]
        public Collection<CardColor.BaseEntry> CardColors
        {
            get { return _cardColors
            ; }
            set { _cardColors
             = value ?? new Collection<CardColor.BaseEntry>(); }
        }
    }
}
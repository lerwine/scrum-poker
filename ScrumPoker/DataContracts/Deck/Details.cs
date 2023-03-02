using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.Deck
{
    [DataContract]
    public class Details : RecordEntry
    {
        private Collection<CardEntry> _cards = new Collection<CardEntry>();
        /// <summary>
        /// Card deck colors.
        /// </summary>
        [DataMember(Name = "cards", IsRequired = true)]
        public Collection<CardEntry> Cards
        {
            get { return _cards
            ; }
            set { _cards
             = value ?? new Collection<CardEntry>(); }
        }
    }
}
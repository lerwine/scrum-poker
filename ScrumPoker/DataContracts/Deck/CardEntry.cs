using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.Deck
{
    [DataContract]
    public class CardEntry : CardDefinition.BaseEntry
    {
        private int _order;
        [DataMember(Name = "order", IsRequired = true)]
        public int Order
        {
            get { return _order; }
            set { _order = (value < 0) ? 0 : value; }
        }
    }
}
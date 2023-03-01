using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ScrumPoker.StandaloneServer.DataContracts
{
    [DataContract]
    public abstract class TitleAndIdentifier : ValidatableObject
    {
        private static readonly PropertyDescriptor _pdTitle;
        private string _title = "";
        [DataMember(Name = "title", IsRequired = true)]
        [Required()]
        [MinLength(1)]
        public string Title
        {
            get { return _title; }
            set
            {
                if (value.ToWsNormalized(SyncRoot, ref _title))
                    RaisePropertyChanged(_pdTitle);
            }
        }

        private static readonly PropertyDescriptor _pdIdentifier;
        private string _identifier = null;
        [DataMember(Name = "identifier", EmitDefaultValue = false)]
        public string Identifier
        {
            get { return _identifier; }
            set
            {
                if (value.ToTrimmedOrNullIfEmpty(SyncRoot, ref _identifier))
                    RaisePropertyChanged(_pdIdentifier);
            }
        }

        static TitleAndIdentifier()
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(TitleAndIdentifier));
            _pdTitle = pdc["Title"];
            _pdIdentifier = pdc["Identifier"];
        }
    }
}
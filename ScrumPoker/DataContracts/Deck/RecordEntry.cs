using System;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts.Deck
{
    public class RecordEntry : BaseEntry
    {
        private string _previewUrl = "";
        [DataMember(Name = "previewUrl", IsRequired = true)]
        public string PreviewUrl
        {
            get { return _previewUrl; }
            set { _previewUrl = value.EmptyIfNullOrTrimmed(); }
        }

        private int _width;
        public int Width
        {
            get { return _width; }
            set { _width = (value < 1) ? 1 : value; }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set { _height = (value < 1) ? 1 : value; }
        }
    }
}
using System.ComponentModel;

namespace ScrumPoker
{
    public class PropertyChangedWithDescriptorEventArgs : PropertyChangedEventArgs
    {
        public PropertyChangedWithDescriptorEventArgs(PropertyDescriptor descriptor) : base(descriptor.Name)
        {
            _descriptor = descriptor;
        }

        private readonly PropertyDescriptor _descriptor;
        public PropertyDescriptor Descriptor { get { return _descriptor; } }
    }
}

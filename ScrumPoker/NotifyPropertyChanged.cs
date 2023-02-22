using System;
using System.ComponentModel;

namespace ScrumPoker
{
    public class NotifyPropertyChanged : INotifyPropertyChangedWithDescriptor
    {
        public event EventHandler<PropertyChangedWithDescriptorEventArgs> PropertyChanged;

        #pragma warning disable IDE1006
        private event PropertyChangedEventHandler _propertyChanged;
        #pragma warning restore IDE1006
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        protected virtual void OnPropertyChanged(PropertyChangedWithDescriptorEventArgs args)
        {
            PropertyChangedEventHandler pc2;
            EventHandler<PropertyChangedWithDescriptorEventArgs> pc1 = PropertyChanged;
            if (pc1 != null)
                try { pc1(this, args); }
                finally
                {
                    if ((pc2 = _propertyChanged) != null)
                        pc2(this, args);
                }
            else if ((pc2 = _propertyChanged) != null)
                pc2(this, args);
        }

        protected void RaisePropertyChanged(PropertyDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");
            OnPropertyChanged(new PropertyChangedWithDescriptorEventArgs(descriptor));
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("'propertyName' cannot be null or empty.", "propertyName");
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this)[propertyName];
            if (descriptor == null)
                throw new ArgumentException("Property '" + propertyName + "' not found on " + TypeDescriptor.GetClassName(this) + ".", "propertyName");
        }
    }
}

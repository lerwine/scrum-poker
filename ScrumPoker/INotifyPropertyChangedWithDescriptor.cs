using System;
using System.ComponentModel;

namespace ScrumPoker
{
    public interface INotifyPropertyChangedWithDescriptor : INotifyPropertyChanged
    {
        new event EventHandler<PropertyChangedWithDescriptorEventArgs> PropertyChanged;
    }
}

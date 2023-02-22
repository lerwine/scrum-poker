using System.ComponentModel;

namespace ScrumPoker
{
    public interface IValidatableObject : INotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo
    {
        bool Validate(bool force);
        bool Validate();
        bool IsValid(string propertyName);
        new string[] GetErrors(string propertyName);
        bool TryGetError(string propertyName, out string errorMessage);
    }
}

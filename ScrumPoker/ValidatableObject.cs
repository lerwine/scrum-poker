using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;

namespace ScrumPoker
{
    public class ValidatableObject : NotifyPropertyChanged, IValidatableObject
    {
        protected readonly object SyncRoot = new object();
        private bool _hasErrors = false;

        private Dictionary<string, Tuple<string, string[]>> _validationMessageDictionary;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        string IDataErrorInfo.this[string columnName] { get { return TryGetError(columnName, out columnName) ? columnName : null; } }

        string IDataErrorInfo.Error
        {
            get
            {
                string result;
                IEnumerable<string> raiseErrorsChanged;
                Monitor.Enter(SyncRoot);
                try
                {
                    if (_validationMessageDictionary == null)
                    {
                        raiseErrorsChanged = ValidateAll();
                        result = (_validationMessageDictionary.Count == 0) ? null : _validationMessageDictionary.Values.SelectMany(t =>
                        {
                            string n = t.Item1;
                            return t.Item2.Select(m => n + ": " + m);
                        }).JoinAsLines();
                    }
                    else
                        return (_validationMessageDictionary.Count == 0) ? null : _validationMessageDictionary.Values.SelectMany(t =>
                        {
                            string n = t.Item1;
                            return t.Item2.Select(m => n + ": " + m);
                        }).JoinAsLines();
                }
                finally { Monitor.Exit(SyncRoot); }
                EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
                if (handler != null)
                    using (IEnumerator<string> enumerator = raiseErrorsChanged.GetEnumerator())
                        if (enumerator.MoveNext())
                            RaiseErrorsChanged(enumerator, handler);
                return result;
            }
        }

        bool INotifyDataErrorInfo.HasErrors { get { return !Validate(); } }

        protected override void OnPropertyChanged(PropertyChangedWithDescriptorEventArgs args)
        {
            IEnumerable<string> raiseErrorsChanged;
            try { base.OnPropertyChanged(args); }
            finally
            {
                Monitor.Enter(SyncRoot);
                try
                {
                    if (_validationMessageDictionary == null)
                        raiseErrorsChanged = ValidateAll();
                    else
                    {
                        bool r;
                        ValidateProperty(args.Descriptor, out r);
                        raiseErrorsChanged = r ? Enumerable.Repeat(args.PropertyName, 1) : null;
                    }
                }
                finally { Monitor.Exit(SyncRoot); }
                if (raiseErrorsChanged != null)
                {
                    EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
                    if (handler != null)
                        using (IEnumerator<string> enumerator = raiseErrorsChanged.GetEnumerator())
                            if (enumerator.MoveNext())
                                RaiseErrorsChanged(enumerator, handler);
                }
            }
        }

        private bool ValidateProperty(PropertyDescriptor descriptor, out bool raiseErrorsChanged)
        {
            bool hadErrors = _validationMessageDictionary.ContainsKey(descriptor.Name);
            ValidationContext validationContext = new ValidationContext(this);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            if (Validator.TryValidateProperty(descriptor.GetValue(this), validationContext, validationResults))
            {
                raiseErrorsChanged = _validationMessageDictionary.ContainsKey(descriptor.Name);
                if (raiseErrorsChanged)
                {
                    _validationMessageDictionary.Remove(descriptor.Name);
                    if (_validationMessageDictionary.Count == 0)
                        _hasErrors = false;
                }
                return true;
            }
            Tuple<string, string[]> t;
            raiseErrorsChanged = _validationMessageDictionary.TryGetValue(descriptor.Name, out t);
            if (raiseErrorsChanged)
            {
                string[] newMessages = validationResults.Select(vr => vr.ErrorMessage).ToArray();
                raiseErrorsChanged = t.Item2.Any(m => !newMessages.Contains(m)) || newMessages.Any(m => !t.Item2.Contains(m));
                if (raiseErrorsChanged)
                    _validationMessageDictionary[descriptor.Name] = new Tuple<string, string[]>(t.Item1, newMessages);
                return false;
            }
            _validationMessageDictionary.Add(descriptor.Name, new Tuple<string, string[]>(descriptor.DisplayName, validationResults.Select(vr => vr.ErrorMessage).ToArray()));
            return false;
        }

        public bool IsValid(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName' cannot be null or empty.", "propertyName");
            IEnumerable<string> raiseErrorsChanged;
            bool returnValue;
            Monitor.Enter(SyncRoot);
            try
            {
                if (_validationMessageDictionary == null)
                    returnValue = (raiseErrorsChanged = ValidateAll()).Contains(propertyName);
                else
                    return ! _validationMessageDictionary.ContainsKey(propertyName);
            }
            finally { Monitor.Exit(SyncRoot); }
            using (IEnumerator<string> enumerator = raiseErrorsChanged.GetEnumerator())
                if (enumerator.MoveNext())
                {
                    EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
                    if (handler != null)
                        RaiseErrorsChanged(enumerator, handler);
                }
            return returnValue;
        }

        public string[] GetErrors(string propertyName)
        {
            IEnumerable<string> raiseErrorsChanged;
            string[] returnValue;
            Monitor.Enter(SyncRoot);
            try
            {
                    Tuple<string, string[]> messages;
                if (_validationMessageDictionary == null)
                {
                    raiseErrorsChanged = ValidateAll();
                    returnValue = _validationMessageDictionary.TryGetValue(propertyName, out messages) ? messages.Item2 : Array.Empty<string>();
                }
                else
                    return _validationMessageDictionary.TryGetValue(propertyName, out messages) ? messages.Item2 : Array.Empty<string>();
            }
            finally { Monitor.Exit(SyncRoot); }
            using (IEnumerator<string> enumerator = raiseErrorsChanged.GetEnumerator())
                if (enumerator.MoveNext())
                {
                    EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
                    if (handler != null)
                        RaiseErrorsChanged(enumerator, handler);
                }
            return returnValue;
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName) { return GetErrors(propertyName); }

        public bool TryGetError(string propertyName, out string errorMessage)
        {
            IEnumerable<string> raiseErrorsChanged;
            bool returnValue;
            Monitor.Enter(SyncRoot);
            try
            {
                    Tuple<string, string[]> messages;
                if (_validationMessageDictionary == null)
                {
                    raiseErrorsChanged = ValidateAll();
                    returnValue = _validationMessageDictionary.TryGetValue(propertyName, out messages);
                    errorMessage = returnValue ? messages.Item2.JoinAsLines() : null;
                }
                else
                {
                    if (_validationMessageDictionary.TryGetValue(propertyName, out messages))
                    {
                        errorMessage = messages.Item2.JoinAsLines();
                        return true;
                    }
                    errorMessage = null;
                    return false;
                }
            }
            finally { Monitor.Exit(SyncRoot); }
            using (IEnumerator<string> enumerator = raiseErrorsChanged.GetEnumerator())
                if (enumerator.MoveNext())
                {
                    EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
                    if (handler != null)
                        RaiseErrorsChanged(enumerator, handler);
                }
            return returnValue;
        }

        private string[] ValidateAll()
        {
            ValidationContext validationContext = new ValidationContext(this);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            _hasErrors = Validator.TryValidateObject(this, validationContext, validationResults, true);
            if (!_hasErrors)
            {
                if (_validationMessageDictionary == null)
                    _validationMessageDictionary = new Dictionary<string, Tuple<string, string[]>>();
                return Array.Empty<string>();
            }
            Dictionary<string, Tuple<string, string[]>> dict = validationResults.ToValidationMessageDictionary(TypeDescriptor.GetProperties(this));
            if (dict.Count == 0)
            {
                if (_validationMessageDictionary == null)
                {
                    _validationMessageDictionary = new Dictionary<string, Tuple<string, string[]>>();
                    return Array.Empty<string>();
                }
                if (_validationMessageDictionary.Count == 0)
                    return Array.Empty<string>();
                string[] result = _validationMessageDictionary.Keys.ToArray();
                _validationMessageDictionary.Clear();
                return result;
            }
            if (_validationMessageDictionary == null)
                return (_validationMessageDictionary = dict).Keys.ToArray();
            if (_validationMessageDictionary.Count == 0)
            {
                foreach (KeyValuePair<string, Tuple<string, string[]>> kvp in dict)
                    _validationMessageDictionary.Add(kvp.Key, kvp.Value);
                return dict.Keys.ToArray();
            }
            string[] cleared = _validationMessageDictionary.Keys.Where(k => !dict.ContainsKey(k)).ToArray();
            IEnumerable<string> changed = dict.Where(kvp =>
            {
                Tuple<string, string[]> t;
                if (_validationMessageDictionary.TryGetValue(kvp.Key, out t))
                {
                    if (t.Item2.Any(n => !kvp.Value.Item2.Contains(n)) || kvp.Value.Item2.Any(n => !t.Item2.Contains(n)))
                    {
                        _validationMessageDictionary[kvp.Key] = t;
                        return true;  
                    }
                    return false;
                }
                _validationMessageDictionary.Add(kvp.Key, t);
                return true;
            }).Select(kvp => kvp.Key);
            if (cleared.Length == 0)
                return changed.ToArray();
            return changed.Concat(cleared).ToArray();
        }

        public bool Validate() { return Validate(false); }

        public bool Validate(bool force)
        {
            IEnumerable<string> raiseErrorsChanged;
            Monitor.Enter(SyncRoot);
            try
            {
                if (force || _validationMessageDictionary == null)
                    raiseErrorsChanged = ValidateAll();
                else
                    return !_hasErrors;
            }
            finally {Monitor.Exit(SyncRoot); }
            using (IEnumerator<string> enumerator = raiseErrorsChanged.GetEnumerator())
                if (enumerator.MoveNext())
                {
                    EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
                    if (handler != null)
                        RaiseErrorsChanged(enumerator, handler);
                }
            return !_hasErrors;
        }

        private void RaiseErrorsChanged(IEnumerator<string> enumerator, EventHandler<DataErrorsChangedEventArgs> handler)
        {
            try { handler(this, new DataErrorsChangedEventArgs(enumerator.Current)); }
            finally
            {
                if (enumerator.MoveNext())
                    RaiseErrorsChanged(enumerator, handler);
            }
        }
    }
}
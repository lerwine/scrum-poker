using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumPoker.WebApp.Models
{
    class FKOptionalNavProperty<TNav>
        where TNav : class, new()
    {   
        private readonly object _syncRoot = new();
        private readonly Func<TNav, Guid> _getKey;
        private Guid? _foreignKey;
        public Guid? ForeignKey
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_model is not null)
                    {
                        Guid fk = _getKey(_model);
                        if (_foreignKey.HasValue && fk.Equals(_foreignKey))
                            return fk;
                        _foreignKey = fk;
                    }
                    else if (_foreignKey.HasValue)
                        _foreignKey = null;
                }
                return _foreignKey;
            }
            set
            {
                lock (_syncRoot)
                {
                    if (value.HasValue)
                    {
                        if (_foreignKey.HasValue && _foreignKey.Value.Equals(value.Value))
                            return;
                        if (_model is not null && !value.Value.Equals(_getKey(_model)))
                            _model = null;
                    }
                    else
                    {
                        if (!_foreignKey.HasValue)
                            return;
                        if (_model is not null)
                            _model = null;
                    }
                    _foreignKey = value;
                }
            }
        }

        private TNav? _model;
        public TNav? Model
        {
            get => _model;
            set
            {
                lock (_syncRoot)
                {
                    if (value == null)
                    {
                        if (_model is null)
                            return;
                        if (_foreignKey.HasValue)
                            _foreignKey = null;
                    }
                    else
                    {
                        if (_model is not null && ReferenceEquals(_model, value))
                            return;
                        _foreignKey = _getKey(_model = value);
                    }
                }
            }
        }
        
        internal FKOptionalNavProperty(Func<TNav, Guid> getKey)
        {
            _getKey = getKey ?? throw new ArgumentNullException(nameof(getKey));
        }
        
        internal FKOptionalNavProperty(Guid? foreignKey, Func<TNav, Guid> getKey)
        {
            _getKey = getKey ?? throw new ArgumentNullException(nameof(getKey));
            _foreignKey = foreignKey;
        }
        
        internal FKOptionalNavProperty(TNav? model, Func<TNav, Guid> getKey)
        {
            _getKey = getKey ?? throw new ArgumentNullException(nameof(getKey));
            if ((_model = model) is not null)
                _foreignKey = getKey(model!);
        }
    }
}
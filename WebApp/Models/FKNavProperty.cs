using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumPoker.WebApp.Models;
    
class FKNavProperty<TNav>
    where TNav : class, new()
{   
    private readonly object _syncRoot = new();
    private readonly Func<TNav, Guid> _getKey;
    private Guid _foreignKey;
    public Guid ForeignKey
    {
        get
        {
            lock (_syncRoot)
            {
                if (_model is not null)
                {
                    Guid fk = _getKey(_model);
                    if (fk.Equals(_foreignKey))
                        return fk;
                    _foreignKey = fk;
                }
            }
            return _foreignKey;
        }
        set
        {
            lock (_syncRoot)
            {
                if (_foreignKey.Equals(value))
                    return;
                if (_model is not null && !value.Equals(_getKey(_model)))
                    _model = null;
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
                    if (!_foreignKey.Equals(Guid.Empty))
                        _foreignKey = Guid.Empty;
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
    
#pragma warning disable CS8618
    internal FKNavProperty(Func<TNav, Guid> getKey)
    {
        _getKey = getKey ?? throw new ArgumentNullException(nameof(getKey));
    }
    
    internal FKNavProperty(Guid foreignKey, Func<TNav, Guid> getKey)
    {
        _getKey = getKey ?? throw new ArgumentNullException(nameof(getKey));
        _foreignKey = foreignKey;
    }
    
    internal FKNavProperty(TNav? model, Func<TNav, Guid> getKey)
    {
        _getKey = getKey ?? throw new ArgumentNullException(nameof(getKey));
        if ((_model = model) is not null)
            _foreignKey = getKey(model);
    }
#pragma warning restore CS8618
}
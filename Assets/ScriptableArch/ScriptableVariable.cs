// ----------------------------------------------------------------------------
// Expands upon Ryan Hipple's ScriptableVariables
// from Unite 2017 - Game Architecture with Scriptable Objects
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace ScriptableVariables {
    public abstract class ScriptableVariable<T> : ScriptableObject {
        public T Value;

        public event Action<T> ValueChangeEvent;

        public void SetValue(T value) {
            Value = value;
            OnValueChange(Value);
        }

        public void SetValue(ScriptableVariable<T> value) {
            Value = value.Value;
            OnValueChange(Value);
        }

        public virtual void OnValueChange(T newValue) {
            ValueChangeEvent?.Invoke(newValue);
        }
    }
}

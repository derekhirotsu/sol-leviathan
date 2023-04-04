// ----------------------------------------------------------------------------
// Expands upon Ryan Hipple's ScriptableVariables
// from Unite 2017 - Game Architecture with Scriptable Objects
// ----------------------------------------------------------------------------

using System;
using UnityEngine;

namespace ScriptableVariables {
    [Serializable]
    public class ScriptableVariableReference<T> {
        public bool UseConstant = true;
        public T ConstantValue;
        [SerializeField]
        private ScriptableVariable<T> Variable;

        public ScriptableVariableReference() {}

        public ScriptableVariableReference(T value) {
            UseConstant = true;
            ConstantValue = value;
        }

        public T Value {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public static implicit operator T(ScriptableVariableReference<T> reference) {
            return reference.Value;
        }

        public void Subscribe(Action<T> callback) {
            if (UseConstant || Variable == null) {
                return;
            }

            Variable.ValueChangeEvent += callback;
        }

        public void Unsubscribe(Action<T> callback) {
            if (UseConstant || Variable == null) {
                return;
            }

            Variable.ValueChangeEvent -= callback;
        }
    }

}

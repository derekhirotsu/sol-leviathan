// ----------------------------------------------------------------------------
// Expands upon Ryan Hipple's ScriptableVariables
// from Unite 2017 - Game Architecture with Scriptable Objects
// ----------------------------------------------------------------------------

using UnityEngine;

namespace ScriptableVariables {
    [CreateAssetMenu (menuName = "ScriptableVariables/Float")]
    public class FloatVariable : ScriptableVariable<float> {
        public void ApplyChange(float amount) {
            Value += amount;
            base.OnValueChange(Value);
        }

        public void ApplyChange(FloatVariable amount) {
            Value += amount.Value;
            base.OnValueChange(Value);
        }
    }
}

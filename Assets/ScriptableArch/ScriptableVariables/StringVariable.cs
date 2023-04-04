// ----------------------------------------------------------------------------
// Expands upon Ryan Hipple's ScriptableVariables
// from Unite 2017 - Game Architecture with Scriptable Objects
// ----------------------------------------------------------------------------

using UnityEngine;

namespace ScriptableVariables {
    [CreateAssetMenu (menuName = "ScriptableVariables/String")]
    public class StringVariable : ScriptableVariable<string> {
        public void ApplyChange(string amount) {
            Value += amount;
            base.OnValueChange(Value);
        }

        public void ApplyChange(StringVariable amount) {
            Value += amount.Value;
            base.OnValueChange(Value);
        }
    }
}

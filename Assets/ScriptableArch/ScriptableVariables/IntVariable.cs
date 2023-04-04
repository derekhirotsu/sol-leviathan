// ----------------------------------------------------------------------------
// Expands upon Ryan Hipple's ScriptableVariables
// from Unite 2017 - Game Architecture with Scriptable Objects
// ----------------------------------------------------------------------------

using UnityEngine;

namespace ScriptableVariables {
    [CreateAssetMenu (menuName = "ScriptableVariables/Int")]
    public class IntVariable : ScriptableVariable<int> {
        public void ApplyChange(int amount) {
            Value += amount;
            base.OnValueChange(Value);
        }

        public void ApplyChange(IntVariable amount) {
            Value += amount.Value;
            base.OnValueChange(Value);
        }
    }
}

// ----------------------------------------------------------------------------
// Expands upon Ryan Hipple's ScriptableVariables
// from Unite 2017 - Game Architecture with Scriptable Objects
// ----------------------------------------------------------------------------

using UnityEngine;

namespace ScriptableVariables {
    [CreateAssetMenu (menuName = "ScriptableVariables/Bool")]
    public class BoolVariable : ScriptableVariable<bool> {
        public void ToggleChange() {
            Value = !Value;
            base.OnValueChange(Value);
        }

        public void ApplyChange(BoolVariable amount) {
            Value = amount.Value;
            base.OnValueChange(Value);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "States/Conditions/StateCondition")]
public class StateCondition : ScriptableObject
{
    [SerializeField] protected State finiteState;
    [SerializeField] protected List<State> substates;

    public bool CurrentStateMatchesCondition(StateMachine frame) {
        
        // If a finite state is provided, we can consider if it's accurate.
        if (finiteState != null) {

            // If the finite state does not match, immediately return false.
            if (!frame.MatchesFiniteState(finiteState)) {
                return false;
            }
        }

        // Provide the list of substates to the frame
        return frame.MatchesSubstateList(substates);
    }
}

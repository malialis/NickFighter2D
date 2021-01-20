using UnityEngine;
using System.Collections;

public class HandleMovementCollider_ASB : StateMachineBehaviour {

    StateManager states;

    public int index;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(states == null)        
            states = animator.transform.GetComponentInParent<StateManager>();

            states.CloseMovementCollider(index);
     }

    //onStateUp is called on each update frame between onStateEnter and onStateExit callbacks
    //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{        
    //}

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == null)
            states = animator.transform.GetComponentInParent<StateManager>();

        states.OpenMovementCollider(index);
    }
    
}

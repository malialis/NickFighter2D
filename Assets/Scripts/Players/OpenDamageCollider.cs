﻿using UnityEngine;
using System.Collections;

public class OpenDamageCollider : StateMachineBehaviour {

    StateManager states;
    public HandleDamageColliders.DamageType damageType;
    public HandleDamageColliders.DCtype dcType;
    public float delay;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == null)
            states = animator.transform.GetComponentInParent<StateManager>();

        states.handleDC.OpenCollider(dcType, delay, damageType);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == null)
            states = animator.transform.GetComponentInParent<StateManager>();

        states.handleDC.CloseColliders();
    }



}

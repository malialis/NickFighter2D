﻿using UnityEngine;
using System.Collections;

public class DoDamage : MonoBehaviour {

    StateManager states;

    public HandleDamageColliders.DamageType damageType;


	// Use this for initialization
	void Start ()
    {
        states = GetComponentInParent<StateManager>();
	}
	
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<StateManager>())
        {
            StateManager oState = other.GetComponentInParent<StateManager>();
            if(oState != states)
            {
                if (!oState.currentlyAttacking)
                {
                    oState.TakeDamage(30, damageType);
                }                
            }
        }

    }


}

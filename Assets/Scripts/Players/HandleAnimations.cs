using UnityEngine;
using System.Collections;

public class HandleAnimations : MonoBehaviour {

    public Animator anim;
    StateManager states;
    public Transform firePoint;
    public GameObject fireBall;

    public float attackRate = 0.3f;
    public AttackBase[] attacks = new AttackBase[2];

	// Use this for initialization
	void Start ()
    {
        states = GetComponent<StateManager>();
        anim = GetComponentInChildren<Animator>();
        
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        states.dontMove = anim.GetBool("DontMove");

        anim.SetBool("TakesHit", states.gettingHit);
        anim.SetBool("OnAir", !states.onGround);
        anim.SetBool("Crouch", states.crouch);

        float movement = Mathf.Abs(states.horizontal);
        anim.SetFloat("Movement", movement);

        Block();
        HandleAttacks();
        PunchAndKick();
        SpecialMoves();

    }


    void HandleAttacks()
    {
        if (states.canAttack)
        {
            if (states.attack1)
            {
                attacks[0].attack = true;
                attacks[0].attackTimer = 0;
                attacks[0].timesPressed++;
            }
            if (attacks[0].attack)
            {
                attacks[0].attackTimer += Time.deltaTime;

                if (attacks[0].attackTimer > attackRate || attacks[0].timesPressed >= 5)
                {
                    attacks[0].attackTimer = 0;
                    attacks[0].attack = false;
                    attacks[0].timesPressed = 0;
                }
            }
            if (states.attack2)
            {
                attacks[1].attack = true;
                attacks[1].attackTimer = 0;
                attacks[1].timesPressed++;
            }

            if (attacks[1].attack)
            {
                attacks[1].attackTimer += Time.deltaTime;

                if (attacks[1].attackTimer > attackRate || attacks[0].timesPressed >= 5)
                {
                    attacks[1].attackTimer = 0;
                    attacks[1].attack = false;
                    attacks[1].timesPressed = 0;
                }
            }
            if (states.attack3)
            {
                attacks[2].attack = true;
                attacks[2].attackTimer = 0;
                attacks[2].timesPressed++;
            }
                        
        }
        anim.SetBool("Attack1", attacks[0].attack);
        anim.SetBool("Attack2", attacks[1].attack);
        anim.SetBool("Attack3", attacks[2].attack);
        anim.SetBool("Punch", states.punch);
        anim.SetBool("MPunch", states.mediumPunch);
        anim.SetBool("Kick", states.kick);
        anim.SetBool("MKick", states.mediumKick);

    }


    public void JumpAnim()
    {
        anim.SetBool("Attack1", false);
        anim.SetBool("Attack2", false);
        anim.SetBool("Jump", true);
        StartCoroutine(CloseBoolInAnim("Jump"));
    }

    void PunchAndKick()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            anim.Play("Punch");
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            anim.Play("Kick");
        }
        if(Input.GetKeyUp(KeyCode.M))
        {
            anim.Play("MKick");
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            anim.Play("MPunch");
        }
    }

    void Block()
    {
        if (states.vertical < 0)
        {
            states.crouch = true;
        }
        else
        {
            states.crouch = false;
        }
        if (states.lookRight & states.horizontal < 0 & states.gettingHit || !states.lookRight & states.horizontal > 0 & states.gettingHit)
        {
            anim.SetBool("Block", states.block);
        }
    }

    void SpecialMoves()
    {
        if (states.lookRight)
        {
            if (states.vertical < 0)
            {
                if (states.horizontal > 0 & Input.GetKeyDown(KeyCode.B))
                {
                    Debug.Log("FireBall... Yo");
                    FireBall();
                }
            }
        }
    }

    void FireBall()
    {
        Instantiate(fireBall, firePoint.transform.position, Quaternion.identity);
    }

    IEnumerator CloseBoolInAnim(string name)
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool(name, false);
    }


    [System.Serializable]
    public class AttackBase
    {
        public bool attack;
        public float attackTimer;
        public int timesPressed;
    }


}

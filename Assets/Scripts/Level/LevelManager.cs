using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    WaitForSeconds oneSec; //we will use it a lot so we dont want to create a new one
    public Transform[] spawnPositions; // positions players spawn at

    CharacterManager charManager;
    LevelUI levelUI; // we store the ui elements here

    public int maxTurns = 2;
    int currentTurn = 1; // the current turn we are, start at 1

    //variable for the countdown
    public bool countdown;
    public int maxTurnTimer = 99;
    int currentTimer;
    float internalTimer;


	// Use this for initialization
	void Start ()
    {
        //get singleton refferences
        charManager = CharacterManager.GetInstance();
        levelUI = LevelUI.GetInstance();

        //init the waitForSeconds
        oneSec = new WaitForSeconds(1);

        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        levelUI.AnnouncerTextLine2.gameObject.SetActive(false);

        StartCoroutine("StartGame");

	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    //a fast way to handle player orientation in the scene
        //just compare the x of the first player, if its lower then the enemy is on the right
        if(charManager.players[0].playerStates.transform.position.x <
            charManager.players[1].playerStates.transform.position.x)
        {
            charManager.players[0].playerStates.lookRight = true;
            charManager.players[1].playerStates.lookRight = false;
        }
        else
        {
            charManager.players[0].playerStates.lookRight = false;
            charManager.players[1].playerStates.lookRight = true;
        }
	}

    void Update()
    {
        if(countdown) //if we enable countdown
        {
            HandleTurnTimer(); // control the timer here
        }
    }

    IEnumerator StartGame()
    {
        //when we start the game
        //we create the players first

        yield return CreatePlayers();
        
        //then initialize the turn
        yield return InitTurn();
    }

    IEnumerator CreatePlayers()
    {
        //go to all the players we have in our list
        for (int i = 0; i < charManager.players.Count; i++)
        {
            //and instantiate the prefabs
            GameObject go = Instantiate(charManager.players[i].playerPrefab,
                spawnPositions[i].position, Quaternion.identity) as GameObject;
            // and assign the needed references
            charManager.players[i].playerStates = go.GetComponent<StateManager>();
            charManager.players[i].playerStates.healthSlider = levelUI.healthSliders[i];
        }
        yield return null;
    }

    IEnumerator InitTurn()
    {
        //to init the turn
        //disable announcer texts first
        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        levelUI.AnnouncerTextLine2.gameObject.SetActive(false);

        //reset the timer
        currentTimer = maxTurnTimer;
        countdown = false;

        //start initializing the players
        yield return InitPlayers();

        // and then start the coroutine to enable the controls of each player
        yield return EnableControl();
        
    }

    IEnumerator InitPlayers()
    {
        //right now, the only thing we have to do is reset their health
        for(int i = 0; i < charManager.players.Count; i++)
        {
            charManager.players[i].playerStates.health = 100;
            charManager.players[i].playerStates.handleAnim.anim.Play("Locomotion");
            charManager.players[i].playerStates.transform.position = spawnPositions[i].position;           
        }
        yield return null;
    }

    IEnumerator EnableControl()
    {
        //start with the announcer text
        levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
        levelUI.AnnouncerTextLine1.text = "ROUND " + currentTurn;
        levelUI.AnnouncerTextLine1.color = Color.white;
        yield return oneSec;
        yield return oneSec;

        //change the ui text and color every second
        levelUI.AnnouncerTextLine1.text = "3";
        levelUI.AnnouncerTextLine1.color = Color.green;
        yield return oneSec;
        levelUI.AnnouncerTextLine1.text = "2";
        levelUI.AnnouncerTextLine1.color = Color.yellow;
        yield return oneSec;
        levelUI.AnnouncerTextLine1.text = "1";
        levelUI.AnnouncerTextLine1.color = Color.red;
        yield return oneSec;
        levelUI.AnnouncerTextLine1.text = "FIGHT!";
        levelUI.AnnouncerTextLine1.color = Color.red;
        
        // and for every player enable what they need to have open to be controlled
        for(int i = 0; i < charManager.players.Count; i++)
        {
            // for users players, enable the input handler for example
            if(charManager.players[i].playerType == CharacterManager.PlayerBase.PlayerType.user)
            {
                InputHandler ih = charManager.players[i].playerStates.gameObject.GetComponent<InputHandler>();
                ih.playerInput = charManager.players[i].inputID;
                ih.enabled = true;
            }
        }
        //after a second disable the announcer text
        yield return oneSec;
        levelUI.AnnouncerTextLine1.gameObject.SetActive(false);
        countdown = true;
    }

    IEnumerator EndTurn()
    {
        //wait 3 seconds for the previous text to clearly show
        yield return oneSec;
        yield return oneSec;
        
        //find who was the winner
        CharacterManager.PlayerBase vPlayer = FindWinningPlayer();

        if(vPlayer == null) // if our function returned a null
        {
            // that means its a draw
            levelUI.AnnouncerTextLine1.text = "Draw";
            levelUI.AnnouncerTextLine1.color = Color.blue;
        }
        else
        {
            //else  that player is the winner
            levelUI.AnnouncerTextLine1.text = vPlayer.playerID + " Wins!";
            levelUI.AnnouncerTextLine1.color = Color.red;
            
        }
        // wait 3 seconds
        yield return oneSec;
        yield return oneSec;
        
        // check to see if victorious player has taken any damage
        if(vPlayer != null)
        {
            //if not, then it's a flawless victory
            if(vPlayer.playerStates.health == 100)
            {
                levelUI.AnnouncerTextLine2.gameObject.SetActive(true);
                levelUI.AnnouncerTextLine2.text = "PERFECT!";
            }
        }

        //wait 3 seconds
        yield return oneSec;
        yield return oneSec;
        

        currentTurn++; // add to the turn counter

        bool matchOver = isMatchOver();

        if (!matchOver)
        {
            StartCoroutine("InitTurn"); // and start the loop for the next turn
        }
        else
        {
            for(int i = 0; i < charManager.players.Count; i++)
            {
                charManager.players[i].score = 0; // can make highScores here
                charManager.players[i].hasCharacter = false;
            }
            SceneManager.LoadSceneAsync("select");
        }
    }

    void DisableControl()
    {
        //to disable the controls, you need to disable the component that makes a character move
        for(int i = 0; i < charManager.players.Count; i++)
        {
            //but first reset the variables in their state manager
            charManager.players[i].playerStates.ResetStateInputs();

            //for user players thats the input handler
            if(charManager.players[i].playerType == CharacterManager.PlayerBase.PlayerType.user)
            {
                charManager.players[i].playerStates.GetComponent<InputHandler>().enabled = false;
            }
        }
    }

    void HandleTurnTimer()
    {
        levelUI.LevelTimer.text = currentTimer.ToString();
        internalTimer += Time.deltaTime; // every second (frame dependant)
        if(internalTimer > 1)
        {
            currentTimer--; // subtract from the current timer one
            internalTimer = 0;
        }
        if(currentTimer <= 0) // if the countdown is over
        {
            EndTurnFunction(true); //end the turn
            countdown = false;
        }
    }

    public void EndTurnFunction(bool timeOut = false)
    {
        //we call this function everytime we want to end the turn 
        //but we need to know if we do so by a timeout or not
        countdown = false;

        // reset the timer text
        levelUI.LevelTimer.text = maxTurnTimer.ToString();

        //if its a timeout
        if (timeOut)
        {
            //add this text first
            levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
            levelUI.AnnouncerTextLine1.text = "Time Out!";
            levelUI.AnnouncerTextLine1.color = Color.cyan;
        }
        else
        {

            levelUI.AnnouncerTextLine1.gameObject.SetActive(true);
            levelUI.AnnouncerTextLine1.text = "K.O.";
            levelUI.AnnouncerTextLine1.color = Color.red;
        }

        //disable the controls
        DisableControl();

        //and start the coroutine to end turn
        StartCoroutine("EndTurn");
    }

    bool isMatchOver()
    {
        bool retVal = false;
        for(int i = 0; i < charManager.players.Count; i++)
        {
            if(charManager.players[i].score >= maxTurns)
            {
                retVal = true;
                break;
            }
        }
        return retVal;
    }

    CharacterManager.PlayerBase FindWinningPlayer()
    {
        //to find out  who won
        CharacterManager.PlayerBase retVal = null;

        StateManager targetPlayer = null;

        //check first to see if both player have equal health
        if(charManager.players[0].playerStates.health != charManager.players[1].playerStates.health)
        {
            //if not, then check who has the lower health
            if(charManager.players[0].playerStates.health < charManager.players[1].playerStates.health)
            {
                charManager.players[1].score++;
                charManager.players[1].playerStates.GetComponent<Animator>().Play("Victory");
                targetPlayer = charManager.players[1].playerStates;
                levelUI.AddWinIndicator(1);
            }
            else
            {
                charManager.players[0].score++;
                targetPlayer = charManager.players[0].playerStates;
                levelUI.AddWinIndicator(0);
            }
            retVal = charManager.returnPlayerFromStates(targetPlayer);
        }
        return retVal;
    }

    public static LevelManager instance;
    public static LevelManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

}

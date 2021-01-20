using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SelectScreenManager : MonoBehaviour {

    public int numberOfPlayers = 1;
    public List<PlayerInterfaces> plInterfaces = new List<PlayerInterfaces>();
    public PortraitInfo[] portraitPrefabs; // all our entries as portraits
    public int maxX; // how many portraits we have on x and y NOTE this is hardcoded
    public int maxY;
    PortraitInfo[,] charGrid; // the grid we are making to select entries

    public GameObject portraitCanvas; // canvas that holds all the portraits

    bool loadLevel; // if we load the level
    public bool bothPlayersSelected;

    CharacterManager charManager; 

    #region Singleton
    public static SelectScreenManager instance;
    public static SelectScreenManager GetInstance()
    {
        return instance;
    }
    
    // Use this for initialization
	void Awake ()
    {
        instance = this;
	}
    #endregion

    void Start()
    {
        //we start by getting the reference to the character manager
        charManager = CharacterManager.GetInstance();
        numberOfPlayers = charManager.numberOfUsers;

        //we create the grid
        charGrid = new PortraitInfo[maxX, maxY];

        int x = 0;
        int y = 0;

        portraitPrefabs = portraitCanvas.GetComponentsInChildren<PortraitInfo>();
        // we need to go into all our portraits
        for(int i = 0; i < portraitPrefabs.Length; i++)
        {
            //assign a grid position
            portraitPrefabs[i].posX += x;
            portraitPrefabs[i].posY += y;

            charGrid[x, y] = portraitPrefabs[i];

            if(x < maxX - 1)
            {
                x++;
            }
            else
            {
                x = 0;
                y++;
            }
        }
    }
    

    // Update is called once per frame
    void Update ()
    {
        if (!loadLevel)
        {
            for (int i = 0; i < plInterfaces.Count; i++)
            {
                if(i < numberOfPlayers)
                {
                   /* //to deselect
                    if (Input.GetButtonUp("Fire2" + charManager.players[i].inputID))
                    {
                        plInterfaces[i].playerBase.hasCharacter = false;
                    } */

                    if (!charManager.players[i].hasCharacter)
                    {
                        plInterfaces[i].playerBase = charManager.players[i];

                        HandleSelectorPosition(plInterfaces[i]);
                        HandleSelectScreenInput(plInterfaces[i], charManager.players[i].inputID);
                        HandleCharacterPreview(plInterfaces[i]);
                    }
                }
                else
                {
                    charManager.players[i].hasCharacter = true;
                }
            }
        }
        if (bothPlayersSelected)
        {
            Debug.Log("loading");
            StartCoroutine("LoadLevel"); //start to load the level
            loadLevel = true;
        }
        else
        {
            if(charManager.players[0].hasCharacter
                && charManager.players[1].hasCharacter)
            {
                bothPlayersSelected = true;
            }
        }
	}

    void HandleSelectScreenInput(PlayerInterfaces pl, string playerId)
    {
        #region Grid Navigation
        /* to navigate the grid 
         * we change the active x and y 
         * we limit the input to not move more than 1 time each 1/2 second
         * */

        float vertical = Input.GetAxis("Vertical" + playerId);
        if(vertical != 0)
        {
            if (!pl.hitInputOnce)
            {
                if(vertical > 0)
                {
                    pl.activeY = (pl.activeY > 0) ? pl.activeY - 1 : maxY - 1;
                }
                else
                {
                    pl.activeY = (pl.activeY < maxY - 1) ? pl.activeY + 1 : 0;
                }
                pl.hitInputOnce = true;
            }
        }
        float horizontal = Input.GetAxis("Horizontal" + playerId);
        if(horizontal != 0)
        {
            if (!pl.hitInputOnce)
            {
                if(horizontal > 0)
                {
                    pl.activeX = (pl.activeX > 0) ? pl.activeX - 1 : maxX - 1;
                }
                else
                {
                    pl.activeX = (pl.activeX < maxX - 1) ? pl.activeX + 1 : 0;
                }
                pl.timerToReset = 0;
                pl.hitInputOnce = true;
            }
        }

        if(vertical == 0 && horizontal == 0)
        {
            pl.hitInputOnce = false;
        }
        if (pl.hitInputOnce)
        {
            pl.timerToReset += Time.deltaTime;
            if(pl.timerToReset > 0.8f)
            {
                pl.hitInputOnce = false;
                pl.timerToReset = 0;
            }
        }

        #endregion

        //if space is pressed select character
        if(Input.GetButtonUp("Fire1" + playerId))
        {
            //make a reaction on the character
            pl.createdCharacter.GetComponentInChildren<Animator>().Play("Victory");
            //pass the character into the characterManager
            pl.playerBase.playerPrefab =
                charManager.returnCharacterWithID(pl.activePortrait.characterId).prefab;
            pl.playerBase.hasCharacter = true;
        }
        
    }

    void HandleSelectorPosition(PlayerInterfaces pl)
    {
        pl.selector.SetActive(true); // enable selector
        pl.activePortrait = charGrid[pl.activeX, pl.activeY]; // find active portrait
        //place selector over portrait position
        Vector2 selectorPosition = pl.activePortrait.transform.localPosition;
        selectorPosition = selectorPosition + new Vector2(portraitCanvas.transform.localPosition.x,
            portraitCanvas.transform.localPosition.y);

        pl.selector.transform.localPosition = selectorPosition;
    }

    void HandleCharacterPreview(PlayerInterfaces pl)
    {
        //if previews portrait we had is not the same as the active one that means we changed characters
        if(pl.previewPortrait != pl.activePortrait)
        {
            if(pl.createdCharacter != null) // delete the one we have now if we do not have one
            {
                Destroy(pl.createdCharacter);
            }
            //create an other one
            GameObject go = Instantiate(
                CharacterManager.GetInstance().returnCharacterWithID(pl.activePortrait.characterId).prefab,
                pl.charVisPos.position, Quaternion.identity) as GameObject;

            pl.createdCharacter = go;
            pl.previewPortrait = pl.activePortrait;

            if(!string.Equals(pl.playerBase.playerID, charManager.players[0].playerID))
            {
                pl.createdCharacter.GetComponent<StateManager>().lookRight = false;
            }
        }
    }

    IEnumerator LoadLevel()
    {
        //if any of the players is an ai, then assign random
        for(int i = 0; i < charManager.players.Count; i++)
        {
            if(charManager.players[i].playerType == CharacterManager.PlayerBase.PlayerType.ai)
            {
                if(charManager.players[i].playerPrefab == null)
                {
                    int ranValue = Random.Range(0, portraitPrefabs.Length);

                    charManager.players[i].playerPrefab =
                        charManager.returnCharacterWithID(portraitPrefabs[ranValue].characterId).prefab;

                    Debug.Log(portraitPrefabs[ranValue].characterId);
                }
            }
        }

        yield return new WaitForSeconds(2); // after 2 secs load level
        SceneManager.LoadSceneAsync("level", LoadSceneMode.Single);
    }


    [System.Serializable]
    public class PlayerInterfaces
    {
        public PortraitInfo activePortrait; // current active portrait for player 1
        public PortraitInfo previewPortrait;
        public GameObject selector; //the select indicator for player1
        public Transform charVisPos; // the visualization position for player 1
        public GameObject createdCharacter; // the created character for player 1

        public int activeX; // the active x and y entries for player 1
        public int activeY;

        //variables for smoothing out input
        public bool hitInputOnce;
        public float timerToReset;

        public CharacterManager.PlayerBase playerBase;
    }


}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour {

    public int numberOfUsers;
    public List<PlayerBase> players = new List<PlayerBase>(); // the list with all our players and player types

    //the list where we hold anything we need to know for each separate character,
    // for now, it's their id and their corresponding prefab
    public List<CharacterBase> characterList = new List<CharacterBase>();

    //we use this function to find characters from their id
    public CharacterBase returnCharacterWithID(string id)
    {
        CharacterBase retVal = null;
        for (int i = 0; i < characterList.Count; i++)
        {
            if(string.Equals(characterList[i].charID, id))
            {
                retVal = characterList[i];
                break;
            }
        }
        return retVal;
    }

    //we use this one to return the player from his created character, states
    public PlayerBase returnPlayerFromStates(StateManager states)
    {
        PlayerBase retVal = null;
        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].playerStates == states)
            {
                retVal = players[i];
                break;
            }
        }
        return retVal;
    }

    public static CharacterManager instance;
    public static CharacterManager GetInstance()
    {
        return instance;
    }

	// Use this for initialization
	void Awake ()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}


    [System.Serializable]
    public class CharacterBase
    {
        public string charID;
        public GameObject prefab;
    }

    [System.Serializable]
    public class PlayerBase
    {
        public string playerID;
        public string inputID;
        public PlayerType playerType;
        public bool hasCharacter;
        public GameObject playerPrefab;
        public StateManager playerStates;
        public int score;

        public enum PlayerType
        {
            user, //real person
            ai, //skynet computer
            simulation //multiplayer over internet .... maybe
        }
    }


}

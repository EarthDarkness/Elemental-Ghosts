using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerReadyCheck : MonoBehaviour {

	
	public PlayerInfo[] players;
	
	public int waitingID=0;
	
	public static PlayerReadyCheck instance;
	// Use this for initialization
	void Start () {
		instance=this;
		for(int i=0;i<players.Length;i++)
		{
			players[i].SetId(i);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.K))
		{
			ClickedJoined();
		}
		if(Input.GetKeyDown(KeyCode.Space))
		{
			for(int i=0;i<players.Length;i++)
			{
				players[i].Ready();
			}
		}
		
	}
	public void ClickedJoined()
	{
		if(waitingID<players.Length)
		{
			players[waitingID].Joined();
			waitingID++;
		}
	}
	public void ReadyCheck()
	{
		int joinedAndReady=0;
		for(int i=0;i<players.Length;i++)
		{
			if(players[i].GetJoined())
			{
				if(	players[i].GetReady())
					joinedAndReady++;
				else return;
			}
			
		}
		if(joinedAndReady>=2)
		{
			// DO THE THING THAT LOADS THE GAME
		}
	}
	
}
[System.Serializable]
public class PlayerInfo
{
	bool isReady=false;
	bool hasClicked=false;
	public PlayerInput input;
	public Animator CrystalController;
	public TMPro.TextMeshProUGUI text;
	public Color col;
	public Image CrystalBase;
	int ID=-1;
	public void Joined()
	{
		hasClicked=true;
		text.text="Player "+(ID+1)+" has joined the game";
		CrystalBase.color=col;
		CrystalController.SetTrigger("Joined");
	}
	public void Ready()
	{
		isReady=true;
		text.text="Player "+(ID+1)+" is ready";
		CrystalController.SetTrigger("Ready");
		PlayerReadyCheck.instance.ReadyCheck();
	}
	public void SetId(int id)
	{
		this.ID=id;
	}
	public bool GetJoined()
	{
		return hasClicked;
	}
	public bool GetReady()
	{
		return isReady;
	}
	
	
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public AudioInfo[] info;
	public static AudioManager instance;
	public Dictionary<string, AudioInfo> infoDictionary = new Dictionary<string, AudioInfo>();
	// Use this for initialization
	void Awake () {
		instance = this;
		for (int i = 0; i < info.Length; i++) {
			infoDictionary.Add (info [i].Name, info [i]);
		}
	}
	void Update()
	{
		if(Input.GetKey(KeyCode.K))
		{
			PlaySound("Scream");
		}
	}
	

	public static void PlaySound(string id, bool Loop=false, float pitch=1f, bool forceReplay=true){
		instance.infoDictionary [id].Play (Loop, pitch,forceReplay);
	}
	/*public void PlayInactiveSound(string id){
		if(!infoDictionary[id].isPlaying)
		infoDictionary [id].Play ();
	}*/
	public void StopSound(string id){
		infoDictionary [id].Stop ();
	}
	

}[System.Serializable]
public class AudioInfo
{
	public AudioSource[] Source;
	public string Name;
	public void Play(bool loop, float pitch,bool forceReplay)
	{	
		
		for(int i=0;i<Source.Length;i++)
		{
			if(Source[i].isPlaying){
				if(forceReplay)
				{
					Source[i].Stop();
				}
				else{
					return;
				}
			}
		}
		
		int id=Random.Range(0,Source.Length);
		Source[id].loop=loop;
		Source[id].pitch=pitch;
		Source[id].Play();
	}
	public void Stop()
	{
		for(int i=0;i<Source.Length;i++)
		{
			Source[i].Stop();
		}
	}
}
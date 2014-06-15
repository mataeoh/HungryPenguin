using UnityEngine;
using System.Collections;

public class CS_SoundMgr : MonoBehaviour {

	public AudioSource Snd_Jump;
	public AudioSource Snd_BG_01;
	public AudioSource Snd_BG_02;
	public AudioSource Snd_GameOver_01;
	public AudioSource Snd_GameOver_02;
	public AudioSource Snd_Click;
	public AudioSource Snd_Explosion;
	public AudioSource Snd_Coin;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {	
	}

	public void PlaySnd_BG() {
		if(Random.Range(0,2) == 1) {
			Snd_BG_01.audio.Play();
		}
		else {
			Snd_BG_02.audio.Play();
		}
	}

	public void StopSnd_BG() {
		Snd_BG_01.audio.Stop();
		Snd_BG_02.audio.Stop();
	}

	public void PlaySnd_Jump() {
		Snd_Jump.audio.Play ();
	}

	public void PlaySnd_Coin() {
		Snd_Coin.audio.Play ();
	}

	public void PlaySnd_Click() {
		Snd_Click.audio.Play ();
	}

	public void PlaySnd_Explosion() {
		Snd_Explosion.audio.Play ();
	}

	public void PlaySnd_GameOver() {
		Snd_GameOver_02.audio.Play();
	}

	public void StopSnd_GameOver() {
		Snd_GameOver_02.audio.Stop();
	}
}

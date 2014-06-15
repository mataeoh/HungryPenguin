using UnityEngine;
using System.Collections;

public class CS_DebugPrint : MonoBehaviour {

	public GameObject 	MainThread;
	CS_MainThread 		m_MainThread;

	string DebugString;

	// Use this for initialization
	void Start () {
		this.guiText.text = "";
		m_MainThread = MainThread.GetComponent<CS_MainThread>();
	}
	
	// Update is called once per frame
	void Update () {

		if(m_MainThread.IsDebug()) {
			this.guiText.text = "FPS : " + (1.0f / Time.deltaTime).ToString() + "\n";
			this.guiText.text += Screen.width.ToString() + "  " + Screen.height.ToString();
			this.guiText.text += DebugString;
			DebugString = "";
		}
		else {
			this.guiText.text = "";
		}
	}
	
	public void print(string str) {
		DebugString += "\n" + str;
	}
}

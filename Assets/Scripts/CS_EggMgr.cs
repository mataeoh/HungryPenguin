using UnityEngine;
using System.Collections;

public class CS_EggMgr : MonoBehaviour {
	
	public GameObject 	MainThread;
	CS_MainThread 		m_MainThread;
	
	public GameObject 	Mesh_Egg;
	public Texture[] 	Eggextures;
	float m_EggRadius = 0.75f;
	static int nScore = 4;
	
	ArrayList m_Eggs = new ArrayList();
	
	// Use this for initialization
	void Start () {
		m_MainThread = MainThread.GetComponent<CS_MainThread>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_MainThread.IsPause() && m_MainThread.GetState() == CS_MainThread.eState.Play) {
			float fMoveDist = m_MainThread.m_Player.GetSpeed() * Time.deltaTime;
			Vector3 PlayerPos = m_MainThread.m_Player.GetPosition();
			ArrayList RemoveEggs = new ArrayList();
			foreach(GameObject egg in m_Eggs) {
				egg.transform.position += new Vector3(-fMoveDist, 0.0f, 0.0f);
				if(egg.transform.position.x < m_MainThread.m_BlockMgr.GetDestroyPosX()) {
					RemoveEggs.Add(egg);
				}
				// Get Score
				else if(Mathf.Abs(egg.transform.position.x - PlayerPos.x) < m_EggRadius
				        && Mathf.Abs(egg.transform.position.y - PlayerPos.y) < m_EggRadius) {
					m_MainThread.AddScore(nScore);
					m_MainThread.m_SoundMgr.PlaySnd_Coin();
					RemoveEggs.Add(egg);
				}
			}
			
			for(int i=0; i<RemoveEggs.Count; ++i) {
				Remove_Egg((GameObject)RemoveEggs[i]);
			}
		}
	}
	
	// Reset
	public void Reset(){
		for(int i=0; i<m_Eggs.Count; ++i) {
			Destroy ((GameObject)m_Eggs[i]);
		}
		m_Eggs.Clear();
	}
	
	// Create Egg
	public void Create_Egg(Vector3 pos) {
		GameObject egg = (GameObject)GameObject.Instantiate(Mesh_Egg, pos, new Quaternion());
		egg.renderer.material.SetTexture("_MainTex", Eggextures[Random.Range(0, Eggextures.GetLength(0))]);
		m_Eggs.Add(egg);
	}
	
	// Remove Egg
	public void Remove_Egg(GameObject egg) {
		m_Eggs.Remove(egg);
		Destroy(egg);
	}
}
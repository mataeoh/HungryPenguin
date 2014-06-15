using UnityEngine;
using System.Collections;

public class CS_FishMgr : MonoBehaviour {
	
	public GameObject 	MainThread;
	CS_MainThread 		m_MainThread;
	
	public GameObject 	Mesh_Fish;
	public Texture[] 	fishTextures;

	float 				m_fSpawnInterval = 0.3f;
	float 				m_fCurSpawnTime = 0.0f;
	int 				m_nMaxFish = 10;
	static int 			nScore = 1;
	Vector3 			vSpawnPos = new Vector3(11.0f, -4.1f, -1.0f);
	
	ArrayList m_Fishs = new ArrayList();
	
	// Use this for initialization
	void Start () {
		m_MainThread = MainThread.GetComponent<CS_MainThread>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_MainThread.IsPause() && m_MainThread.GetState() == CS_MainThread.eState.Play) {
			// Create Fish
			if(m_Fishs.Count < m_nMaxFish) {
				m_fCurSpawnTime -= Time.deltaTime;
				if(m_fCurSpawnTime < 0.0f) {
					m_fCurSpawnTime = m_fSpawnInterval;
					Create_Fish();
				}
			}

			Vector3 PlayerPos = m_MainThread.m_Player.GetPosition();
			ArrayList RemoveFishs = new ArrayList();
			float fRadius = 0.0f;
			foreach(CS_Fish fish in m_Fishs) {
				fRadius = fish.GetRadius();
				// Get Score
				if(Mathf.Abs(fish.transform.position.x - PlayerPos.x) < fRadius
				        && Mathf.Abs(fish.transform.position.y - PlayerPos.y) < fRadius) {
					m_MainThread.AddScore(nScore);
					m_MainThread.m_SoundMgr.PlaySnd_Coin();
					RemoveFishs.Add(fish);
				}
			}
			
			for(int i=0; i<RemoveFishs.Count; ++i) {
				Remove_Fish((CS_Fish)RemoveFishs[i]);
			}
		}
	}
	
	// Reset
	public void Reset(){
		m_fCurSpawnTime = 0.0f;
		for(int i=0; i<m_Fishs.Count; ++i) {
			CS_Fish fish = (CS_Fish)m_Fishs[i];
			Destroy(fish.gameObject);
		}
		m_Fishs.Clear();
	}
	
	// Create Fish
	public void Create_Fish() {
		GameObject fish = (GameObject)GameObject.Instantiate(Mesh_Fish, vSpawnPos, new Quaternion());
		fish.renderer.material.SetTexture("_MainTex", fishTextures[Random.Range(0, fishTextures.GetLength(0))]);
		CS_Fish cs_fish = fish.GetComponent<CS_Fish>();
		cs_fish.SetMainThread(m_MainThread);
		m_Fishs.Add(cs_fish);
	}
	
	// Remove Fish
	public void Remove_Fish(CS_Fish fish) {
		m_Fishs.Remove(fish);
		Destroy(fish.gameObject);
	}
}
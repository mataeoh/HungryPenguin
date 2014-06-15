using UnityEngine;
using System.Collections;

public class CS_FruitMgr : MonoBehaviour {

	public GameObject 	MainThread;
	CS_MainThread 		m_MainThread;

	public GameObject 	Mesh_Fruit;
	public Texture[] 	fruitTextures;
	float m_FruitRadius = 0.55f;
	static int nScore = 2;

	ArrayList m_Fruits = new ArrayList();

	// Use this for initialization
	void Start () {
		m_MainThread = MainThread.GetComponent<CS_MainThread>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_MainThread.IsPause() && m_MainThread.GetState() == CS_MainThread.eState.Play) {
			float fMoveDist = m_MainThread.m_Player.GetSpeed() * Time.deltaTime;
			Vector3 PlayerPos = m_MainThread.m_Player.GetPosition();
			ArrayList RemoveFruits = new ArrayList();
			foreach(GameObject fruit in m_Fruits) {
				fruit.transform.position += new Vector3(-fMoveDist, 0.0f, 0.0f);
				if(fruit.transform.position.x < m_MainThread.m_BlockMgr.GetDestroyPosX()) {
					RemoveFruits.Add(fruit);
				}
				// Get Score
				else if(Mathf.Abs(fruit.transform.position.x - PlayerPos.x) < m_FruitRadius
				          && Mathf.Abs(fruit.transform.position.y - PlayerPos.y) < m_FruitRadius) {
					m_MainThread.AddScore(nScore);
					m_MainThread.m_SoundMgr.PlaySnd_Coin();
					RemoveFruits.Add(fruit);
				}
			}

			for(int i=0; i<RemoveFruits.Count; ++i) {
				Remove_Fruit((GameObject)RemoveFruits[i]);
			}
		}
	}

	// Reset
	public void Reset(){
		for(int i=0; i<m_Fruits.Count; ++i) {
			Destroy ((GameObject)m_Fruits[i]);
		}
		m_Fruits.Clear();
	}

	// Create Fruit
	public void Create_Fruit(Vector3 pos) {
		GameObject fruit = (GameObject)GameObject.Instantiate(Mesh_Fruit, pos, new Quaternion());
		fruit.renderer.material.SetTexture("_MainTex", fruitTextures[Random.Range(0, fruitTextures.GetLength(0))]);
		m_Fruits.Add(fruit);
	}

	// Remove Fruit
	public void Remove_Fruit(GameObject fruit) {
		m_Fruits.Remove(fruit);
		Destroy(fruit);
	}
}
using UnityEngine;
using System.Collections;

public class CS_BlockMgr : MonoBehaviour {

	public GameObject 	MainThread;
	CS_MainThread 		m_MainThread;

	public GameObject 	Mesh_BLock;
	GameObject 			m_LastBlock = null;
	public Texture[] 	BlockTextures;

	float Padding_X_Rnd = 0.0f;
	float Padding_Y_Rnd = 0.0f;
	float Padding_X = 6.5f; //7.5f;
	float Padding_Y = 2.55f; //4.0f;
	float SpawnPosX = 11.5f;
	float DestroyPosX = -11.5f;
	Vector2 Inner_HeightRange = new Vector2(-3.5f, 4.5f);
	Vector2 Outer_HeightRange = new Vector2(-5.5f, 5.5f);
	float ItemSpawnRange = 3.5f;

	ArrayList m_Blocks = new ArrayList();

	// Use this for initialization
	void Start () {
		m_MainThread = MainThread.GetComponent<CS_MainThread>();
	}

	// Reset
	public void Reset() {
		for(int i=0; i<m_Blocks.Count; ++i) {
			Destroy((GameObject)m_Blocks[i]);
		}

		m_Blocks.Clear();
		m_LastBlock = null;
	}

	// Create Block
	public void Create_Block() {
		float fPaddingX = Padding_X;
		float fPaddingY = Padding_Y;
		Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);
		int nBottom_TextureIndex = Random.Range(1,3);
		int nTop_TextureIndex = nBottom_TextureIndex == 1 ? 1 : 0;
		Vector3 vBottom_Scale = new Vector3(2.0f, (float)(nBottom_TextureIndex + 2) * 2.0f, 1.0f);
		Vector3 vTop_Scale = new Vector3(2.0f, (float)(nTop_TextureIndex + 2) * 2.0f, 1.0f);
		Vector3 vBottom_HalfScale = vBottom_Scale * 0.5f;
		Vector3 vTop_HalfScale = vTop_Scale * 0.5f;

		Position.x = m_LastBlock == null ? SpawnPosX : m_LastBlock.transform.position.x + m_LastBlock.transform.localScale.x * 0.5f + fPaddingX + vBottom_HalfScale.x;
		Position.y = Random.Range(Inner_HeightRange[0] - vBottom_HalfScale.y, Mathf.Min(Inner_HeightRange[1] - fPaddingY - vBottom_HalfScale.y, Outer_HeightRange[0] + vBottom_HalfScale.y));

		GameObject BottomObj = (GameObject)GameObject.Instantiate(Mesh_BLock, Position, new Quaternion());
		GameObject TopObj = (GameObject)GameObject.Instantiate(Mesh_BLock, Position + new Vector3(0.0f, fPaddingY + vBottom_HalfScale.y + vTop_HalfScale.y, 0.0f), new Quaternion());
		BottomObj.transform.localScale = vBottom_Scale;
		TopObj.transform.localScale = vTop_Scale;

		// SetTexture
		TopObj.renderer.material.SetTexture("_MainTex", BlockTextures[nTop_TextureIndex]);
		BottomObj.renderer.material.SetTexture("_MainTex", BlockTextures[nBottom_TextureIndex]);

		m_Blocks.Add(BottomObj);
		m_Blocks.Add(TopObj);

		m_LastBlock = TopObj;
		m_MainThread.m_FruitMgr.Create_Fruit(new Vector3(Position.x, Position.y + fPaddingY * 0.5f + vBottom_HalfScale.y, 0.0f));
		m_MainThread.m_FruitMgr.Create_Fruit(new Vector3(Position.x + fPaddingX * 0.5f + vBottom_HalfScale.x, Random.Range(-ItemSpawnRange, ItemSpawnRange), 0.0f));
		m_MainThread.m_EggMgr.Create_Egg(new Vector3(Position.x, TopObj.transform.position.y + vTop_HalfScale.y + 0.75f, 0.0f));
	}

	// Remove Block
	public void Remove_Block(GameObject block) {
		m_Blocks.Remove(block);
		Destroy (block);
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_MainThread.IsPause() && m_MainThread.GetState() == CS_MainThread.eState.Play) {
			float fMoveDist = m_MainThread.m_Player.GetSpeed() * Time.deltaTime;
			ArrayList RemoveBlocks = new ArrayList();
			foreach(GameObject curBlock in m_Blocks) {
				// Update Position
				curBlock.transform.position += new Vector3(-fMoveDist, 0.0f, 0.0f);

				// Reset Position
				if(curBlock.transform.position.x < DestroyPosX) {
					RemoveBlocks.Add(curBlock);
				}
			}

			// Remove Blocks
			for(int i=0; i<RemoveBlocks.Count; ++i) {
				Remove_Block((GameObject)RemoveBlocks[i]);
			}

			// Create Block
			if(m_LastBlock == null || m_LastBlock.transform.position.x + Padding_X < SpawnPosX) {
				Create_Block();
			}			
		}
	}

	public float GetDestroyPosX() { return DestroyPosX; }
	public void PaddingXUp() { Padding_X += 1.0f; }
	public void PaddingXDown() { Padding_X -= 1.0f; }
	public void PaddingYUp() { Padding_Y += 0.5f; }
	public void PaddingYDown() { Padding_Y -= 0.5f; }
}
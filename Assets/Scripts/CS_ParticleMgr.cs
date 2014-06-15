using UnityEngine;
using System.Collections;

public class CS_ParticleMgr : MonoBehaviour {

	public GameObject 	Par_StarBust;
	public GameObject m_par_StarBust = null;

	// Use this for initialization
	void Start () {
	}


	public void Reset() {
		if(m_par_StarBust!=null) Destroy(m_par_StarBust);
		m_par_StarBust = null;
	}
	
	// Update is called once per frame
	void Update () {
		if(m_par_StarBust != null && !m_par_StarBust.particleSystem.isPlaying) {
			Destroy(m_par_StarBust);
			m_par_StarBust = null;
		}
	
	}

	public void Create_StarBust(Vector3 pos) {
		m_par_StarBust = (GameObject)GameObject.Instantiate(Par_StarBust, pos, new Quaternion());
	}
}

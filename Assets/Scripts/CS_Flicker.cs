using UnityEngine;
using System.Collections;

public class CS_Flicker : MonoBehaviour {

	float m_LifeTime = 0.5f;
	float m_CurTime = 0.0f;

	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
		m_CurTime += Time.deltaTime;
		float Opacity = (m_CurTime > m_LifeTime) ? 0.0f : (1.0f - m_CurTime / m_LifeTime);
		renderer.material.SetFloat("_Opacity", Opacity);
		if(m_CurTime >= m_LifeTime) {
			Destroy(gameObject);
		}
	}
}

using UnityEngine;
using System.Collections;

public class CS_Fish : MonoBehaviour {
	CS_MainThread 		m_MainThread;

	bool bLeft = true;
	float fSpeedX = Random.Range(1.0f, 5.0f);
	float fSpeedY = Random.Range(0.5f, 1.0f);
	float fCurSpeedX = 0.0f;
	float fPosY_Max = -4.1f;
	float fPosY_Min = -13.1f;
	float fHeightRatio = Random.Range(0.5f, 1.0f);
	Vector3 vStarPos = new Vector3(11.0f, -4.1f, -1.0f);
	Vector3 vEndPos = new Vector3(-11.0f, -4.1f, -1.0f);
	Vector3 vScale = new Vector3(1.0f, 1.0f, 1.0f);
	float fFishRadius = 0.55f;
	float fPauseTime = 0.0f;
	public void SetMainThread(CS_MainThread MainThread) {
		m_MainThread = MainThread;
	}
	
	// Use this for initialization
	void Start () {
		float fScale = Random.Range(1.0f, 2.0f);
		vScale *= fScale;
		fFishRadius *= fScale;

		Reset();
	}

	// Reset
	void Reset() {
		fPauseTime = 0.0f;
		bLeft = Random.Range(0,2) == 1 ? true : false;
		if(bLeft) {
			transform.position = new Vector3(vStarPos.x, Random.Range(fPosY_Min, fPosY_Max), vStarPos.z);
			transform.localScale = vScale;
			fCurSpeedX = -fSpeedX * 1.5f;
		}
		else {
			transform.position = new Vector3(vEndPos.x, Random.Range(fPosY_Min, fPosY_Max), vEndPos.z);
			transform.localScale = new Vector3(-vScale.x, vScale.y, vScale.z);
			fCurSpeedX = fSpeedX;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(m_MainThread.IsPause()) {
			fPauseTime += Time.deltaTime;
			return;
		}

		Vector3 OldPos = transform.position;
		Vector3 Velocity = new Vector3(fCurSpeedX, 0.0f, 0.0f);
		Vector3 NewPos = new Vector3();
		
		NewPos = OldPos + Velocity * Time.deltaTime;
		NewPos.y = Mathf.Lerp(fPosY_Min, fPosY_Max, (Mathf.Sin((Time.time-fPauseTime) * fSpeedY) * 0.5f + 0.5f) * fHeightRatio);
		if(NewPos.y > fPosY_Max) NewPos.y = fPosY_Max;
		transform.position = NewPos;
		
		// move to left
		if(bLeft && transform.position.x < vEndPos.x || !bLeft && transform.position.x > vStarPos.x) {
			Reset();
		}
	}

	// Get Radius
	public float GetRadius() {
		return fFishRadius;
	}
}
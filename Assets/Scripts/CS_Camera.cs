using UnityEngine;
using System.Collections;

public class CS_Camera : MonoBehaviour {

	public GameObject 	MainThread;
	CS_MainThread 		m_MainThread;

	bool bShake = false;
	float fShakeTime = 0.7f;
	float fCurShakeTime = 0.0f;
	float fShakeSpeed = 80.0f;
	float fShakeRadius = 0.2f;
	float fPosY_Normal = 0.0f;
	float fPosY_Moved = 8.5f;
	static float fUnderWaterMove_StartPos = 4.5f;
	static float fUnderWaterMove_EndPos = 7.5f;
	static float fUnderWaterMove_Length = fUnderWaterMove_EndPos - fUnderWaterMove_StartPos;
	static float fSkyMove_StartPos = 3.5f;
	static float fSkyMove_EndPos = 12.0f;
	static float fSkyMove_Length = fSkyMove_EndPos - fSkyMove_StartPos;
	Vector3 vInitPos = new Vector3(0.0f, 0.0f, -10.0f);
	public GameObject 	Mesh_Flicker;

	// Use this for initialization
	void Start () {
		m_MainThread = MainThread.GetComponent<CS_MainThread>();
		Reset ();
	}

	public void Reset() {
		transform.position = vInitPos;
	}
	
	// Update is called once per frame
	void Update () {
		if(bShake && fCurShakeTime < fShakeTime) {
			fCurShakeTime += Time.deltaTime;
			float fDist = Mathf.Sin(fCurShakeTime * fShakeSpeed) * fShakeRadius;
			float Ratio = (fCurShakeTime > fShakeTime) ? 0.0f : (1.0f - fCurShakeTime / fShakeTime);
			fDist *= Ratio*Ratio;
			transform.position = vInitPos + new Vector3(fDist, 0.0f, 0.0f);
		}

		// Calc Camera Position
		float fPlayerPosY = m_MainThread.m_Player.GetPosition().y;
		// under water
		if(fPlayerPosY < 0.0f) {
			float fRatio = Mathf.Clamp(-(fPlayerPosY + fUnderWaterMove_StartPos) / fUnderWaterMove_Length, 0.0f, 1.0f);
			float PosY = Mathf.Lerp(fPosY_Normal, -fPosY_Moved, fRatio);
			transform.position = new Vector3(transform.position.x, PosY, transform.position.z);
		}
		// up in the sky
		else {
			float fRatio = Mathf.Clamp((fPlayerPosY - fSkyMove_StartPos) / fSkyMove_Length, 0.0f, 1.0f);
			float PosY = Mathf.Lerp(fPosY_Normal, fPosY_Moved, fRatio);
			transform.position = new Vector3(transform.position.x, PosY, transform.position.z);
		}
	}

	public void SetCameraShake() {
		bShake = true;
		fCurShakeTime = 0.0f;
		GameObject.Instantiate(Mesh_Flicker);
	}
}

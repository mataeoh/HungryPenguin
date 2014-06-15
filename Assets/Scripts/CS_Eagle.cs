using UnityEngine;
using System.Collections;

public class CS_Eagle : MonoBehaviour {
	
	public GameObject 	MainThread;
	CS_MainThread 		m_MainThread;
	
	enum eEagleState {
		Ready = 0,
		Patrol = 1
	};
	
	eEagleState m_State = eEagleState.Ready;
	bool bLeft = true;
	float fSpawnTerm_Min = 0.5f;
	float fSpawnTerm_Max = 1.0f;
	float fSpawnTime = 0.0f;
	float fSpeedX = 3.5f;
	float fCurSpeedX = 0.0f;
	float fTurningPos = 4.0f;
	float fPosY_Max = 13.1f;
	float fPosY_Min = 7.0f;
	float fPauseTime = 0.0f;
	Vector3 vStarPos = new Vector3(11.0f, 4.1f, -1.0f);
	Vector3 vEndPos = new Vector3(-11.0f, 4.1f, -1.0f);
	Vector3 vScale = new Vector3(4.0f, 2.8f, 1.0f);

	// Use this for initialization
	void Start () {
		m_MainThread = MainThread.GetComponent<CS_MainThread>();
		Reset();
	}
	
	// Reset
	public void Reset() {
		fPauseTime = 0.0f;
		fSpawnTime = Random.Range(fSpawnTerm_Min, fSpawnTerm_Max);
		transform.position = vStarPos;
		SetState(eEagleState.Ready);
	}
	
	// Update is called once per frame
	void Update () {
		if(m_MainThread.IsPause()) {
			fPauseTime += Time.deltaTime;
			return;
		}

		switch(m_State) {
		case eEagleState.Ready:
			Update_Ready ();
			break;
		case eEagleState.Patrol:
			Update_Patrol();
			break;
		}
	}
	
	// Set Turn
	void Set_Turning() {
		bLeft = !bLeft;
		if(bLeft) {
			transform.localScale = vScale;
			fCurSpeedX = -fSpeedX * 2.0f;
		}
		else {
			transform.localScale = new Vector3(-vScale.x, vScale.y, vScale.z);
			fCurSpeedX = fSpeedX;
		}
	}
	
	// Update Ready
	void Update_Ready() {
		if(m_MainThread.IsPause() || m_MainThread.GetState() != CS_MainThread.eState.Play) return;

		fSpawnTime -= Time.deltaTime;

		// Eagle Spawn
		if(fSpawnTime < 0.0f) {
			fSpawnTime = fSpawnTime = Random.Range(fSpawnTerm_Min, fSpawnTerm_Max);
			SetState(eEagleState.Patrol);
			bLeft = !bLeft;
			Vector3 StartPos = vStarPos;
			StartPos.y = Random.Range(fPosY_Min, fPosY_Max);
			if(bLeft) {
				transform.position = new Vector3(vStarPos.x, Random.Range(fPosY_Min, fPosY_Max), vStarPos.z);
				transform.localScale = vScale;
				fCurSpeedX = -fSpeedX * 3.0f;
			}
			else {
				transform.position = new Vector3(vEndPos.x, Random.Range(fPosY_Min, fPosY_Max), vEndPos.z);
				transform.localScale = new Vector3(-vScale.x, vScale.y, vScale.z);
				fCurSpeedX = fSpeedX;
			}
			
		}
	}
	
	// Update patrol
	void Update_Patrol() {
		Vector3 OldPos = transform.position;
		Vector3 Velocity = new Vector3(fCurSpeedX, 0.0f, 0.0f);
		Vector3 NewPos = new Vector3();
		Vector3 PlayerPos = m_MainThread.m_Player.GetPosition();

		NewPos = OldPos + Velocity * Time.deltaTime;
		NewPos.y = Mathf.Lerp(fPosY_Min, fPosY_Max, Mathf.Sin((Time.time-fPauseTime) * 1.3f) * 0.5f + 0.5f);
		
		if(NewPos.y < fPosY_Min) NewPos.y = fPosY_Min;
		transform.position = NewPos;
		
		// move to left
		if(bLeft) {
			// arrived end point
			if(transform.position.x < vEndPos.x) {
				SetState(eEagleState.Ready);
			}
		}
		// move to right
		else {
			// arrived end point
			if(transform.position.x > vStarPos.x) {
				SetState(eEagleState.Ready);
			}
			// set turn
			else if(transform.position.x > fTurningPos && OldPos.x < fTurningPos) {
				if(Random.Range(0, 4) == 0) Set_Turning();
			}
		}
	}
	
	// SetState
	void SetState(eEagleState state) {
		if(m_State == state) return;
		m_State = state;		
		if(m_State == eEagleState.Ready) {
		}
		else if(m_State == eEagleState.Patrol) {
		}
	}
	
	// GetState
	eEagleState GetState() {
		return m_State;
	}

	// return HangOn Offset
	public Vector3 GetHangOnOffset() {
		return bLeft ? new Vector3(-0.7f, -1.5f, 0.0f) : new Vector3(0.7f, -1.5f, 0.0f);
	}
}

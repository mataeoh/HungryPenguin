using UnityEngine;
using System.Collections;

public class CS_Shark : MonoBehaviour {

	public GameObject 	MainThread;
	CS_MainThread 		m_MainThread;

	enum eSharkState {
		Ready = 0,
		Patrol = 1
	};

	eSharkState m_State = eSharkState.Ready;
	bool bLeft = true;
	float fSpawnTerm_Min = 0.0f;
	float fSpawnTerm_Max = 0.0f;
	float fSpawnTime = 0.0f;
	float fSpeedX = 3.0f;
	float fCurSpeedX = 0.0f;
	float fSpeedY = 4.0f;
	float fTurningPos = 4.0f;
	float fPosY_Max = -4.1f;
	float fPosY_Min = -13.1f;
	Vector3 vStarPos = new Vector3(11.0f, -4.1f, -1.0f);
	Vector3 vEndPos = new Vector3(-11.0f, -4.1f, -1.0f);
	Vector3 vScale = new Vector3(5.0f, 1.5f, 1.0f);
	float fPauseTime = 0.0f;
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
		SetState(eSharkState.Ready);
	}

	// Update is called once per frame
	void Update () {
		if(m_MainThread.IsPause()) {
			fPauseTime += Time.deltaTime;
			return;
		}

		switch(m_State) {
			case eSharkState.Ready:
				Update_Ready ();
				break;
			case eSharkState.Patrol:
				Update_Patrol();
				break;
		}
	}

	// Set Turn
	void Set_Turning() {
		if(m_MainThread.m_Player.IsDead()) return;
		bLeft = !bLeft;
		if(bLeft) {
			transform.localScale = vScale;
			fCurSpeedX = -fSpeedX * 1.25f;
		}
		else {
			transform.localScale = new Vector3(-vScale.x, vScale.y, vScale.z);
			fCurSpeedX = fSpeedX;
		}
	}

	// Update Move
	void Update_Move() {
		Vector3 OldPos = transform.position;
		Vector3 Velocity = new Vector3(fCurSpeedX, 0.0f, 0.0f);
		Vector3 NewPos = new Vector3();
		Vector3 PlayerPos = m_MainThread.m_Player.GetPosition();

		// Attack
		/*if(bLeft && PlayerPos.x < OldPos.x || !bLeft && PlayerPos.x > OldPos.x) {
			float fDistY = m_MainThread.m_Player.GetPosition().y - OldPos.y;
			Velocity.y = fDistY < 0.0f ? -fSpeedY : fSpeedY;
			Velocity.y *= Mathf.Clamp(Mathf.Abs(fDistY), 0.0f, 1.0f);
		}*/

		NewPos = OldPos + Velocity * Time.deltaTime;
		NewPos.y = Mathf.Lerp(fPosY_Min, fPosY_Max, Mathf.Sin((Time.time-fPauseTime) * 0.65f) * 0.5f + 0.5f);
		if(NewPos.y > fPosY_Max) NewPos.y = fPosY_Max;
		transform.position = NewPos;
		
		// move to left
		if(bLeft) {
			// arrived end point
			if(transform.position.x < vEndPos.x) {
				SetState(eSharkState.Ready);
			}
		}
		// move to right
		else {
			// arrived end point
			if(transform.position.x > vStarPos.x) {
				SetState(eSharkState.Ready);
			}
			// set turn
			else if(transform.position.x > fTurningPos && OldPos.x < fTurningPos) {
				if(Random.Range(0, 4) == 0) Set_Turning();
			}
		}
	}

	// Update Ready
	void Update_Ready() {
		if(m_MainThread.IsPause() || m_MainThread.GetState() != CS_MainThread.eState.Play) return;
		fSpawnTime -= Time.deltaTime;
		// Shark Spawn
		if(fSpawnTime < 0.0f) {
			fSpawnTime = fSpawnTime = Random.Range(fSpawnTerm_Min, fSpawnTerm_Max);
			SetState(eSharkState.Patrol);
			bLeft = !bLeft;
			Vector3 StartPos = vStarPos;
			StartPos.y = Random.Range(fPosY_Min, fPosY_Max);
			if(bLeft) {
				transform.position = new Vector3(vStarPos.x, Random.Range(fPosY_Min, fPosY_Max), vStarPos.z);
				transform.localScale = vScale;
				fCurSpeedX = -fSpeedX * 2.0f;
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
		Update_Move();
	}

	// SetState
	void SetState(eSharkState state) {
		if(m_State == state) return;
		m_State = state;		
		if(m_State == eSharkState.Ready) {
		}
		else if(m_State == eSharkState.Patrol) {
		}
	}

	// GetState
	eSharkState GetState() {
		return m_State;
	}

	// return HangOn Offset
	public Vector3 GetHangOnOffset() {
		return bLeft ? new Vector3(-2.0f, -0.7f, 0.0f) : new Vector3(2.0f, -0.7f, 0.0f);
	}
}

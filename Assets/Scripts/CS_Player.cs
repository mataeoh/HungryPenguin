using UnityEngine;
using System.Collections;

public class CS_Player : MonoBehaviour {
	public GameObject 	MainThread;
	CS_MainThread 		m_MainThread;

	public GameObject 	Particle_Bubble;
	public GameObject 	Particle_Cloud;
	public GameObject 	Particle_Snow;

	public Texture[] 	PenguinTexture;

	// HangOn Obj
	GameObject 			Obj_HangOn;
	Vector3 			HangOnOffset = new Vector3();

	float m_fGravity = 25.0f;
	float m_fCurGravity = 0.0f;
	bool bUnderWater = false;
	bool bUpSky = false;
	bool bFly = true;
	bool bDead = false;
	float[] fStartPositionY = new float [3] { 3.0f, 10.0f, -10.0f }; // ground, sky, sea
	Vector3 vStartPosition = new Vector3(-4.0f, 3.0f, -0.5f);
	Vector3 vEuler = new Vector3(270.0f, 90.0f, 270.0f);
	float m_fVeclocity = 0.0f;
	// start, end, under water end degree
	float[] fRotationRange = new float[3] {260.0f, 330.0f, 300.0f};
	float fRotVelocity = 0.0f;
	float m_fRotForce = -200.0f;
	float m_fCurRotForce = 0.0f;
	float m_fRotGravity = 720.0f;
	float m_fCurRotGravity = 0.0f;
	float m_fJumpForce = 9.0f;
	float m_fCurJumpForce = 0.0f;
	float fJumpDelayTime = 0.1f;
	float fJumpDelay = 0.0f;
	float fRadius = 0.25f;
	float m_fSpeed = 5.0f;
	float m_fCurSpeed = 0.0f;
	float fCeilPos = 13.5f;
	static float fFloorPos = -13.5f;
	static float fWaterPos = -4.0f;
	static float fSkyPos = 5.0f;
	// Jump Accelate - item..
	float fMaxAccelate = 0.0f;
	float fAccelate = 0.0f;
	float fCurAccel = 0.0f;
	float fAccelDecayTime = 1.5f;

	// Get Speed();
	public float GetSpeed() { return m_fCurSpeed + fCurAccel; }
	public float GetCurAccel() { return fCurAccel; }
	public float GetAccelate() { return fAccelate; }

	// Use this for initialization
	void Start () {
		m_MainThread = MainThread.GetComponent<CS_MainThread>();
	}

	// Reset
	public void Reset() {
		// spawn
		int nStage = Random.Range(0,3);
		vStartPosition.y = fStartPositionY[nStage];

		transform.position = vStartPosition;
		transform.eulerAngles = vEuler;
		m_fVeclocity = 0.0f;
		fRotVelocity = 0.0f;
		fCurAccel = 0.0f;
		bFly = true;
		bUnderWater = nStage == 2 ? true : false;
		bUpSky = nStage == 1 ? true : false;
		Obj_HangOn = null;
		CalcuatePhysic();
		SetDead(false);
		Calc_ParticleSpeed();
	}

	// calcuate particle speed
	void Calc_ParticleSpeed() {
		ParticleEmitter emitter_snow = Particle_Snow.GetComponent<ParticleEmitter>();
		ParticleEmitter emitter_bubble = Particle_Bubble.GetComponent<ParticleEmitter>();
		ParticleEmitter emitter_cloud = Particle_Cloud.GetComponent<ParticleEmitter>();
		emitter_snow.worldVelocity = new Vector3(-m_fCurSpeed - m_fCurSpeed * 0.5f, emitter_snow.worldVelocity.y, emitter_snow.worldVelocity.z);
		emitter_bubble.worldVelocity = new Vector3(-m_fCurSpeed * 0.3f - m_fCurSpeed * 0.15f, emitter_bubble.worldVelocity.y, emitter_bubble.worldVelocity.z);
	}

	// Update Material
	void Update_Material() {
		renderer.material.SetFloat("_WingSpeed", bDead ? 0.0f : 1.0f);
		renderer.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1.0f));
	}
	
	// Update is called once per frame
	void Update () {
		// Update_Material
		Update_Material();

		if(bFly) {
			transform.position = vStartPosition + new Vector3(0.0f, Mathf.Sin(Time.time * 3.0f) * 0.3f, 0.0f);
		}
		// Update HangOn
		else if(IsDead() && Obj_HangOn != null) {
			Update_HangOn();
		}
		else if(!m_MainThread.IsPause()) {

			// check under water
			if(!bUnderWater && transform.position.y <= fWaterPos) {
				bUnderWater = true;
				CalcuatePhysic();
			}
			else if(bUnderWater && transform.position.y > fWaterPos) {
				bUnderWater = false;
				CalcuatePhysic();
			}
			// check up in sky
			else if(!bUpSky && transform.position.y > fSkyPos) {
				bUpSky = true;
				CalcuatePhysic();
			}
			else if(bUpSky && transform.position.y <= fSkyPos) {
				bUpSky = false;
				CalcuatePhysic();
			}

			// Update Accel
			fCurAccel -= fAccelate / fAccelDecayTime * Time.deltaTime;
			if(fCurAccel < 0.0f) {
				fCurAccel = 0.0f;
			}

			fJumpDelay -= Time.deltaTime;

			//float fGravityRatio = 1.0f - Mathf.Clamp((transform.position.y - 9.0f) * 0.1f, 0.0f, 1.0f);

			m_fVeclocity -=  m_fCurGravity * Time.deltaTime;

			transform.position += new Vector3(0.0f, m_fVeclocity * Time.deltaTime, 0.0f);

			if(transform.position.y > fCeilPos) {
				m_fVeclocity = 0.0f;
				transform.position = new Vector3(transform.position.x, fCeilPos, transform.position.z);
			}
			else if(transform.position.y < fFloorPos) {
				m_fVeclocity = 0.0f;
				transform.position = new Vector3(transform.position.x, fFloorPos, transform.position.z);
			}

			fRotVelocity += m_fCurRotGravity * Time.deltaTime;
			transform.Rotate(new Vector3(0.0f, fRotVelocity * Time.deltaTime, 0.0f));

			if(transform.eulerAngles.x < fRotationRange[0]) {
				fRotVelocity = 0.0f;
				transform.eulerAngles = new Vector3(fRotationRange[0], vEuler.y, vEuler.z);
			}
			else if(bUnderWater && transform.eulerAngles.x > fRotationRange[2] && !IsDead ()) {
				fRotVelocity = 0.0f;
				transform.eulerAngles = new Vector3(fRotationRange[2], vEuler.y, vEuler.z);
			}
			else if(transform.eulerAngles.x > fRotationRange[1]) {
				fRotVelocity = 0.0f;
				transform.eulerAngles = new Vector3(fRotationRange[1], vEuler.y, vEuler.z);
			}
		}

		m_MainThread.m_DebugPrint.print("Gravity : " + m_fGravity);
		m_MainThread.m_DebugPrint.print ("Jump : " + m_fJumpForce);
	}

	// Update HnagOn
	void Update_HangOn() {
		transform.position = Obj_HangOn.transform.position + HangOnOffset;
	}

	// Collide
	void OnTriggerEnter(Collider other) {
		SetDead(true);
		if(other.tag == "Eagle") {
			Obj_HangOn = m_MainThread.m_Eagle.gameObject;
			HangOnOffset = m_MainThread.m_Eagle.GetHangOnOffset();
			Update_HangOn();
		}
		else if(other.tag == "Shark") {
			Obj_HangOn = m_MainThread.m_Shark.gameObject;
			HangOnOffset = m_MainThread.m_Shark.GetHangOnOffset();
			Update_HangOn();
		}
	}

	// Set Dead
	public void SetDead(bool bDead) {
		if(this.bDead == bDead) return;
		this.bDead = bDead;
		if(bDead) {
			m_fVeclocity = 0.0f;
			m_MainThread.SetState(CS_MainThread.eState.Dead);
			m_MainThread.m_Camera.SetCameraShake();
			m_MainThread.m_ParticleMgr.Create_StarBust(transform.position);
			m_MainThread.m_SoundMgr.PlaySnd_Explosion();
		}
	}

	public bool IsDead() {
		return bDead;
	}

	// Calcuate physic components
	void CalcuatePhysic() {
		//MeshRenderer renderer = GetComponent<MeshRenderer>();

		if(bUnderWater) {
			renderer.material.SetTexture("_MainTex", PenguinTexture[4]);
			renderer.material.SetTexture("_MainTex_02", PenguinTexture[5]);

			// water collide friction - decay velocity
			if(m_fVeclocity < 0.0f) {
				m_fVeclocity *= 0.2f;
			}
			m_fCurJumpForce = m_fJumpForce * 0.7f;
			m_fCurGravity = m_fGravity * 0.5f;
			m_fCurSpeed = m_fSpeed * 0.7f;
			m_fCurRotForce = m_fRotForce * 0.3f;
			m_fCurRotGravity = m_fRotGravity * 0.3f;
		}
		else {

			if(bUpSky) {
				renderer.material.SetTexture("_MainTex", PenguinTexture[2]);
				renderer.material.SetTexture("_MainTex_02", PenguinTexture[3]);
			}
			else {
				renderer.material.SetTexture("_MainTex", PenguinTexture[0]);
				renderer.material.SetTexture("_MainTex_02", PenguinTexture[1]);
			}

			// water jump
			if(transform.position.y < 0.0f && m_fVeclocity > 0.0f) {
				m_fVeclocity = 12.0f;
			}

			m_fCurJumpForce = m_fJumpForce;
			m_fCurGravity = m_fGravity;
			m_fCurSpeed = m_fSpeed;
			m_fCurRotForce = m_fRotForce;
			m_fCurRotGravity = m_fRotGravity;
		}
	}

	// Get Position
	public Vector3 GetPosition() {
		return transform.position;
	}

	// Get Radius
	public float GetRadius() {
		return fRadius;
	}

	// Set Jump
	public void SetJump() {
		if(!bFly && fJumpDelay < 0.0f) {
			m_MainThread.m_SoundMgr.PlaySnd_Jump();
			// Accumulate
			fCurAccel += fAccelate;
			if(fCurAccel > fMaxAccelate) fCurAccel = fMaxAccelate;
			fJumpDelay = fJumpDelayTime;
			m_fVeclocity = m_fCurJumpForce;
			fRotVelocity = m_fCurRotForce;
			transform.eulerAngles = new Vector3(fRotationRange[0], vEuler.y, vEuler.z);
		}
	}

	// Set Fly
	public void SetFly(bool bFly) {
		this.bFly = bFly;
	}

	// Is Fly?
	public bool IsFly() {
		return bFly;
	}

	public void JumpForceUp() {	
		m_fJumpForce += 1.0f; 
		CalcuatePhysic();
	}
	public void JumpForceDown() { 
		m_fJumpForce -= 1.0f;
		CalcuatePhysic();
	}
	public void GravityUp() {
		m_fGravity += 1.0f; 
		CalcuatePhysic();
	}
	public void GravityDown() { 
		m_fGravity -= 1.0f;
		CalcuatePhysic();
	}

	public void SpeedUp() { 
		m_fSpeed += 1.0f; 
		CalcuatePhysic();
	}
	public void SpeedDown() { 
		m_fSpeed -= 1.0f; 
		CalcuatePhysic();
	}
}
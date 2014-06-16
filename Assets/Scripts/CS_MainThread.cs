using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class CS_MainThread : MonoBehaviour {
	float ratio;
	int Width;
	int Height;
	bool bSaveScore = false;
	int nScore = 0;
	int nHighScore = -1;
	float fWidth;
	float fHeight;
	bool bQuit = false;
	bool bPasue = false;
	bool bDebug = false;
	bool bRelease = true;
	public GameObject Text_HighScore;
	public GameObject Text_HighScoreShadow;
	public GameObject Text_Score;
	public GameObject Text_ScoreShadow;

	public GameObject AndroidManager;
	public GameObject Obj_DebugPrint;
	public GameObject Obj_BlockMgr;
	public GameObject Obj_Player;
	public GameObject Obj_EggMgr;
	public GameObject Obj_FishMgr;
	public GameObject Obj_FruitMgr;
	public GameObject Obj_ParticleMgr;
	public GameObject Obj_Eagle;
	public GameObject Obj_Shark;
	public GameObject Obj_SoundMgr;

	//public AdvertisementManager	m_AdsManager;
	public CS_Camera 		m_Camera;
	public CS_DebugPrint 	m_DebugPrint;
	public CS_BlockMgr		m_BlockMgr;
	public CS_Player 		m_Player;
	public CS_EggMgr		m_EggMgr;
	public CS_FishMgr		m_FishMgr;
	public CS_FruitMgr		m_FruitMgr;
	public CS_Eagle			m_Eagle;
	public CS_Shark			m_Shark;
	public CS_ParticleMgr	m_ParticleMgr;
	public CS_SoundMgr 		m_SoundMgr;

	public Texture Tex_Btn_TapToPlay;
	public Texture Tex_Btn_Restart;
	public Texture Tex_Btn_Quit;
	public Texture Tex_Btn_Resume;

	public String text="";
	String SaveDir = "";
	String SaveFile = "";

	public enum eState {
		Start = 0,
		Play = 1,
		Dead = 2
	};

	eState m_State = eState.Start;
	
	// Use this for initialization
	void Start () {
		// Init Screen
		ratio = (float)Screen.height / (float)Screen.width;
		Width = 800;
		Height = 480;//(int)(Width*ratio);
		fWidth = (float)Width;
		fHeight = (float)Height;
		Screen.SetResolution(Width,Height,false);

		// Init Save directory and file
		if (Application.platform == RuntimePlatform.Android) 
		{
			SaveDir = Application.persistentDataPath + "\\save\\";
			SaveFile = SaveDir + "save.txt";
		}
		else //PC
		{
			SaveDir = "\\save\\";
			SaveFile = SaveDir + "save.txt";
		}

		// check save directory
		if (!System.IO.Directory.Exists(SaveDir)){
			System.IO.Directory.CreateDirectory(SaveDir);
		}

		// Load high score
		LoadScore();

		// Get Managers
		//m_AdsManager = AndroidManager.GetComponent<AdvertisementManager>();
		//m_AdsManager.Init();
		m_Camera = Camera.main.GetComponent<CS_Camera>();
		m_DebugPrint = Obj_DebugPrint.GetComponent<CS_DebugPrint>();
		m_Player = Obj_Player.GetComponent<CS_Player>();
		m_BlockMgr = Obj_BlockMgr.GetComponent<CS_BlockMgr>();
		m_EggMgr = Obj_EggMgr.GetComponent<CS_EggMgr>();
		m_FishMgr = Obj_FishMgr.GetComponent<CS_FishMgr>();
		m_FruitMgr = Obj_FruitMgr.GetComponent<CS_FruitMgr>();
		m_ParticleMgr = Obj_ParticleMgr.GetComponent<CS_ParticleMgr>();
		m_Eagle = Obj_Eagle.GetComponent<CS_Eagle>();
		m_Shark = Obj_Shark.GetComponent<CS_Shark>();
		m_SoundMgr = Obj_SoundMgr.GetComponent<CS_SoundMgr>();

		// Reset
		Reset();
	}

	// Quit
	void Quit() {
		SaveScore();
		Application.Quit();
	}

	// Reset
	public void Reset() {
		SetPause(false);
		m_Camera.Reset();
		m_Eagle.Reset();
		m_Shark.Reset();
		m_Player.Reset();
		m_BlockMgr.Reset ();
		m_EggMgr.Reset ();
		m_FishMgr.Reset ();
		m_FruitMgr.Reset ();
		m_ParticleMgr.Reset ();

		SetScore(0);
		ShowScore(false);
		SetState(eState.Start);
	}

	// Update is called once per frame
	void Update () {
		bool bJump = false;

		if(bQuit) return;

		//Android Input
		if (Application.platform == RuntimePlatform.Android) 
		{
			if (Input.GetKey(KeyCode.Escape)) {
				bQuit = true;
				SetPause(true);
			}

			foreach(Touch i in Input.touches) {
				if(i.fingerId == 0) {
					if(i.phase == TouchPhase.Began) {
						bJump = true;
						break;
					}
				}
			}
		}
		else // PC Input
		{
			if(Input.GetKey(KeyCode.Escape)) {
				Quit();
			}
			
			if(Input.GetMouseButtonDown(0))
			{
				bJump = true;
			}
		}

		// 
		// Tap to Play
		if(m_State == eState.Start) {
			if(bJump)
			{
				SetState(eState.Play);
				ShowScore(true);
				m_Player.SetFly(false);
			}
		}
		else if(m_State == eState.Play) {
			if(bJump) {
				//m_AdsManager.HideAds();
				m_Player.SetJump();
			}
		}
	}

	public void SetState(eState state) {
		m_State = state;

		if(m_State == eState.Start) {
			//m_AdsManager.ShowAds();
			m_SoundMgr.StopSnd_GameOver();
			m_SoundMgr.PlaySnd_BG();
		}
		else if(m_State == eState.Play) {
			int PosY = 90;
			int Width = 80;
			int Height = 25;
			int Index = 0;
			GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "ResetScore");
		}
		else if(m_State == eState.Dead) {
			//m_AdsManager.ShowAds();
			m_SoundMgr.StopSnd_BG();
			m_SoundMgr.PlaySnd_GameOver();
			SaveScore();
		}
	}

	public eState GetState() {
		return m_State;
	}

	// isDebug?
	public bool IsDebug() {
		return bDebug;
	}
	
	// OnGUI
	void OnGUI() {

		if(bQuit) {
			// quit
			if(GUI.Button(new Rect(220, 180, 180, 80), Tex_Btn_Quit, new GUIStyle()))
			{
				Quit ();
			}
			// resume
			else if(GUI.Button(new Rect(420, 180, 180, 80), Tex_Btn_Resume, new GUIStyle()))
			{
				bQuit = false;
				SetPause(false);
			}
			return;
		}


		// tap to play
		if(m_State == eState.Start) {
			GUI.DrawTexture(new Rect(190, 180, 380, 125), Tex_Btn_TapToPlay);
		}
		// restart
		else if(m_State == eState.Dead) {
			if(GUI.Button(new Rect(290, 200, 220, 110), Tex_Btn_Restart, new GUIStyle()))
			{
				m_SoundMgr.PlaySnd_Click();
				Reset();
			}
		}

		// debug menus
		int PosY = 90;
		int Width = 80;
		int Height = 25;
		int Index = 0;

		if(!bRelease && GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "Debug"))
		{
			bDebug = !bDebug;
		}

		if(bDebug) {
			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "Quit"))
			{
				bQuit = true;
				SetPause(true);
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "ResetScore"))
			{
				SetHighScore(0);
				SaveScore();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "Reset"))
			{
				Reset();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "Pause"))
			{
				SetPause(!IsPause ());
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "Gravity+"))
			{
				m_Player.GravityUp();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "Gravity-"))
			{
				m_Player.GravityDown();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "Jump+"))
			{
				m_Player.JumpForceUp();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "Jump-"))
			{
				m_Player.JumpForceDown();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "PaddingX+"))
			{
				m_BlockMgr.PaddingXUp();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "PaddingX-"))
			{
				m_BlockMgr.PaddingXDown();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "PaddingY+"))
			{
				m_BlockMgr.PaddingYUp();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "PaddingY-"))
			{
				m_BlockMgr.PaddingYDown();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "Speed +"))
			{
				m_Player.SpeedUp();
			}

			if(GUI.Button(new Rect(0, PosY + Height * Index++, Width, Height), "Speed -"))
			{
				m_Player.SpeedDown();
			}
		}
	}

	// Set Score
	public void SetScore(int score) {
		nScore = score;
		Text_Score.guiText.text = score.ToString ();
		Text_ScoreShadow.guiText.text = score.ToString ();
		// check high score
		if(nScore > nHighScore) {
			SetHighScore(nScore);
		}
	}

	// Set HighScore
	public void SetHighScore(int highscore) {
		bSaveScore = true;
		nHighScore = highscore;
		Text_HighScore.guiText.text = "High Score : " + nHighScore.ToString ();
		Text_HighScoreShadow.guiText.text = "High Score : " + nHighScore.ToString ();
	}

	// Add Score
	public void AddScore(int nScore) {
		this.nScore += nScore;
		SetScore (this.nScore);
	}

	// Show Score Text
	void ShowScore(bool bShow) {
		Text_HighScore.SetActive (bShow);
		Text_HighScoreShadow.SetActive (bShow);
		Text_Score.SetActive (bShow);
		Text_ScoreShadow.SetActive (bShow);
	}

	// save score
	void SaveScore() {
		if(bSaveScore) {
			bSaveScore = false;
			// Write
			StreamWriter writer = File.CreateText (SaveFile);
			writer.WriteLine(nHighScore.ToString());
			writer.Close();				
		}
	}

	// load score
	void LoadScore() {
		int highscore = 0;
		if(File.Exists(SaveFile) != false)
		{
			FileInfo file = new FileInfo(SaveFile);
			StreamReader reader = file.OpenText();
			ArrayList Scores = new ArrayList();
			while(true) {
				String temp = reader.ReadLine();
				if(temp == null) break;
				Scores.Add(Convert.ToInt32(temp));
			}
			
			if(Scores.Count > 0) highscore = (int)Scores[0];
		}

		SetHighScore(highscore);
	}

	public bool IsPause() {
		return bPasue;
	}

	public void SetPause(bool bPause) {
		this.bPasue = bPause;
	}
}

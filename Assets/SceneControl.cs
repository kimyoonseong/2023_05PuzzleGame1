using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SceneControl : MonoBehaviour
{

	private ScoreCounter score_counter = null;
	public enum STEP
	{
		NONE = -1, // 상태 정보 없음.
		PLAY = 0, // 플레이 중.
		CLEAR, // 클리어.
		FAIL,//실패
		NUM, // 상태가 몇 종류인지 나타낸다(=2).
	};
	public STEP step = STEP.NONE; // 현재 상태.
	public STEP next_step = STEP.NONE; // 다음 상태.
	public float step_timer = 0.0f; // 경과 시간.
	public float Stage1_RemainTime = 90.0f; // 남은시간
	private float clear_time = 0.0f; // 클리어 시간.
	public GUIStyle guistyle; // 폰트 스타일.
	public static int sceneNumber = 1;      //씬이동 관리하는 스태틱형 변수 20230506
	public GameObject UIpanal;  //UI관리하는 패널		20230506
	public GameObject ClearPanal;       //클리어 관리하는패널 다른데는 넣기만하고  마지막스테이지에서만 사용	0506
	public GameObject LosePanal;       //실패 관리하는패널 다른데는 넣기만하고  마지막스테이지에서만 사용	0506
	public Text scoretext;		//끝나면 패널에 남은시간 띄워줌 0506
	public Text cleartext;		//모든 스테이지 끝나면 클리어텍스트로 남은시간띄워줌 0506
	private int laststage = 3;       //라스트스테이지 이걸 변환해서 스테이지양 조절가능 20230506

	public int stagenum = sceneNumber;		//static형인 sceneNumber를 옮기기위해 stagenum 변수만듬 20230506

	private BlockRoot block_root = null;
	void Start()
	{
		// BlockRoot 스크립트를 취득.
		this.block_root = this.gameObject.GetComponent<BlockRoot>();

		this.block_root.create();
		//UIpanal = SetActive(false);
		// BlockRoot 스크립트의 initialSetUp()을 호출한다.
		this.block_root.initialSetUp();

		// ScoreCounter를 가져온다.
		this.score_counter = this.gameObject.GetComponent<ScoreCounter>();
		this.next_step = STEP.PLAY; // 다음 상태를 '플레이 중'으로.
		this.guistyle.fontSize = 36; // 폰트 크기를 36로.

		UIpanal.SetActive(false);
		ClearPanal.SetActive(false);
		LosePanal.SetActive(false);
		//timeText();

		if(sceneNumber == 1)
		{
			Stage1_RemainTime = 90.0f;
		}

		else if (sceneNumber == 2)
		{
			Stage1_RemainTime = 80.0f;
		}
		else if (sceneNumber == 3)
		{
			Stage1_RemainTime = 70.0f;
		}

		stagenum = sceneNumber;
	}

	void Update()
	{
		this.step_timer += Time.deltaTime;
		this.Stage1_RemainTime -= Time.deltaTime;
		switch (this.step)
		{
			case STEP.CLEAR:
				stagenum = sceneNumber;     //해당 스테이지클리어시 stagenum을 현재 씬넘버로 변경 20230506

				// block_root를 정지.
				if (stagenum < laststage)
				{
					SeeUI();
				}
				else
				{
					SeeClearUI();
				}
				break;
			case STEP.FAIL:
				SeelosePanal();
				break;
		}


		// 상태변화대기-----.
		if (this.next_step == STEP.NONE)
		{
			switch (this.step)
			{
				case STEP.PLAY:
					// 클리어 조건을 만족하면.
					if (this.score_counter.isGameClear())
					{
						this.next_step = STEP.CLEAR; // 클리어 상태로 이행.
					}
					else if (this.Stage1_RemainTime <= 0.0f)
					{
						this.next_step = STEP.FAIL;//실패상태로 이행 2023 0504 추가
					}
					break;

			}
		}
		// 상태가 변화하면------.
		while (this.next_step != STEP.NONE)
		{
			this.step = this.next_step;
			this.next_step = STEP.NONE;
			switch (this.step)
			{
				case STEP.CLEAR:

					this.block_root.enabled = false;
					// 경과 시간을 클리어 시간으로 설정.
					this.clear_time = this.step_timer;
					break;
				case STEP.FAIL:
					this.block_root.enabled = false;
					break;
			}
			this.step_timer = 0.0f;
		}

		//timeText();		//시간을 관리하는 타임텍스트 
	}

	void OnGUI()
	{
		switch (this.step)
		{
			case STEP.PLAY:
				GUI.color = Color.black;
				 //경과 시간을 표시.
				GUI.Label(new Rect(40.0f, 10.0f, 200.0f, 20.0f),
						  "남은 시간" + Mathf.CeilToInt(this.Stage1_RemainTime).ToString() + "초",
						  guistyle);
				GUI.color = Color.white;
				break;
			case STEP.CLEAR:
				//GUI.color = Color.black;
				// 「☆클리어-！☆」라는 문자열을 표시.
				//GUI.Label(new Rect(
				//	Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f),
				//		  "☆클리어-！☆", guistyle);
				// 클리어 시간을 표시.
				//GUI.Label(new Rect(
				//	Screen.width / 2.0f - 20.0f, 60.0f, 200.0f, 20.0f),
				//		  "클리어 시간" + Mathf.CeilToInt(this.clear_time).ToString() +
				//		  "초", guistyle);
				//GUI.color = Color.white;
				
				timeText();
				break;
			case STEP.FAIL:
				GUI.color = Color.black;
				// 「실패!」라는 문자열을 표시.
				//GUI.Label(new Rect(
				//	Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f),
				//		  "실패-！", guistyle);
				break;
		}
	}


	//UI보이게하는 함수 20230506
	public void SeeUI()
	{
		UIpanal.SetActive(true);
	}

	//모든 스테이지 다끝냈을때 클리어패널 보내게끔하는 함수 20230506
	public void SeeClearUI()
	{
		ClearPanal.SetActive(true);
	}

	public void SeelosePanal()
	{
		LosePanal.SetActive(true);
	}

	//다음스테이지로 넘어가는 버튼함수 20230506
	public void NextStage()
	{
		sceneNumber++;
		SceneManager.LoadScene(sceneNumber);
	}

	//게임종료 버튼함수 20230506
	public void EndGame()
	{
		Debug.Log("게임종료");
		Application.Quit();
	}

	public void GoTitle()
	{
		SceneManager.LoadScene("TitleScene");
		sceneNumber = 1;
	}

	public void Retry()
	{
		SceneManager.LoadScene(sceneNumber);
	}


	//현재 시간을 표현하는 함수 20230506
	public void timeText()
	{
		scoretext.text = "클리어시간 : " + Mathf.CeilToInt(clear_time).ToString() + "초";
		cleartext.text = "클리어시간 : " + Mathf.CeilToInt(clear_time).ToString() + "초";
	}

	//현재 스테이지 번호 호출하는 함수 20230506
	public int NowStage()
	{
		return sceneNumber;
	}

}

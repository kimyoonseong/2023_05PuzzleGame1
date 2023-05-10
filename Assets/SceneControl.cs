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
		NONE = -1, // ���� ���� ����.
		PLAY = 0, // �÷��� ��.
		CLEAR, // Ŭ����.
		FAIL,//����
		NUM, // ���°� �� �������� ��Ÿ����(=2).
	};
	public STEP step = STEP.NONE; // ���� ����.
	public STEP next_step = STEP.NONE; // ���� ����.
	public float step_timer = 0.0f; // ��� �ð�.
	public float Stage1_RemainTime = 90.0f; // �����ð�
	private float clear_time = 0.0f; // Ŭ���� �ð�.
	public GUIStyle guistyle; // ��Ʈ ��Ÿ��.
	public static int sceneNumber = 1;      //���̵� �����ϴ� ����ƽ�� ���� 20230506
	public GameObject UIpanal;  //UI�����ϴ� �г�		20230506
	public GameObject ClearPanal;		//Ŭ���� �����ϴ��г� �ٸ����� �ֱ⸸�ϰ�  �������������������� ���	0506
	public Text scoretext;		//������ �гο� �����ð� ����� 0506
	public Text cleartext;		//��� �������� ������ Ŭ�����ؽ�Ʈ�� �����ð������ 0506
	public int laststage = 2;       //��Ʈ�������� �̰� ��ȯ�ؼ� ���������� �������� 20230506

	public int stagenum = sceneNumber;		//static���� sceneNumber�� �ű������ stagenum �������� 20230506

	private BlockRoot block_root = null;
	void Start()
	{
		// BlockRoot ��ũ��Ʈ�� ���.
		this.block_root = this.gameObject.GetComponent<BlockRoot>();

		this.block_root.create();
		//UIpanal = SetActive(false);
		// BlockRoot ��ũ��Ʈ�� initialSetUp()�� ȣ���Ѵ�.
		this.block_root.initialSetUp();

		// ScoreCounter�� �����´�.
		this.score_counter = this.gameObject.GetComponent<ScoreCounter>();
		this.next_step = STEP.PLAY; // ���� ���¸� '�÷��� ��'����.
		this.guistyle.fontSize = 36; // ��Ʈ ũ�⸦ 36��.

		UIpanal.SetActive(false);
		ClearPanal.SetActive(false);
		//timeText();
		stagenum = sceneNumber;
	}

	void Update()
	{
		this.step_timer += Time.deltaTime;
		this.Stage1_RemainTime -= Time.deltaTime;
		switch (this.step)
		{
			case STEP.CLEAR:
				stagenum = sceneNumber;		//�ش� ��������Ŭ����� stagenum�� ���� ���ѹ��� ���� 20230506
				break;
			case STEP.FAIL:
				if (Input.GetMouseButton(0))
				{
					SceneManager.LoadScene("TitleScene");
				}
				break;
		}


		// ���º�ȭ���-----.
		if (this.next_step == STEP.NONE)
		{
			switch (this.step)
			{
				case STEP.PLAY:
					// Ŭ���� ������ �����ϸ�.
					if (this.score_counter.isGameClear())
					{
						if (sceneNumber < laststage)
						{
							SeeUI();
						}
						else
						{
							SeeClearUI();
						}
						this.next_step = STEP.CLEAR; // Ŭ���� ���·� ����.
					}
					else if (this.Stage1_RemainTime <= 0.0f)
					{
						this.next_step = STEP.FAIL;//���л��·� ���� 2023 0504 �߰�
					}
					break;

			}
		}
		// ���°� ��ȭ�ϸ�------.
		while (this.next_step != STEP.NONE)
		{
			this.step = this.next_step;
			this.next_step = STEP.NONE;
			switch (this.step)
			{
				case STEP.CLEAR:
					// block_root�� ����.
					this.block_root.enabled = false;
					// ��� �ð��� Ŭ���� �ð����� ����.
					this.clear_time = this.step_timer;
					break;
				case STEP.FAIL:
					this.block_root.enabled = false;
					break;
			}
			this.step_timer = 0.0f;
		}

		//timeText();		//�ð��� �����ϴ� Ÿ���ؽ�Ʈ 
	}

	void OnGUI()
	{
		switch (this.step)
		{
			case STEP.PLAY:
				GUI.color = Color.black;
				 //��� �ð��� ǥ��.
				GUI.Label(new Rect(40.0f, 10.0f, 200.0f, 20.0f),
						  "���� �ð�" + Mathf.CeilToInt(this.Stage1_RemainTime).ToString() + "��",
						  guistyle);
				GUI.color = Color.white;
				break;
			case STEP.CLEAR:
				//GUI.color = Color.black;
				// ����Ŭ����-���١���� ���ڿ��� ǥ��.
				//GUI.Label(new Rect(
				//	Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f),
				//		  "��Ŭ����-����", guistyle);
				// Ŭ���� �ð��� ǥ��.
				//GUI.Label(new Rect(
				//	Screen.width / 2.0f - 20.0f, 60.0f, 200.0f, 20.0f),
				//		  "Ŭ���� �ð�" + Mathf.CeilToInt(this.clear_time).ToString() +
				//		  "��", guistyle);
				//GUI.color = Color.white;
				
				//timeText();
				break;
			case STEP.FAIL:
				GUI.color = Color.black;
				// ������!����� ���ڿ��� ǥ��.
				//GUI.Label(new Rect(
				//	Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f),
				//		  "����-��", guistyle);
				break;
		}
	}


	//UI���̰��ϴ� �Լ� 20230506
	public void SeeUI()
	{
		UIpanal.SetActive(true);
	}

	//��� �������� �ٳ������� Ŭ�����г� �����Բ��ϴ� �Լ� 20230506
	public void SeeClearUI()
	{
		ClearPanal.SetActive(true);
	}

	//�������������� �Ѿ�� ��ư�Լ� 20230506
	public void NextStage()
	{
		sceneNumber++;
		SceneManager.LoadScene(sceneNumber);
	}

	//�������� ��ư�Լ� 20230506
	public void EndGame()
	{
		Debug.Log("��������");
		Application.Quit();
	}

	//���� �ð��� ǥ���ϴ� �Լ� 20230506
	public void timeText()
	{
		scoretext.text = "Ŭ����ð� : " + Mathf.CeilToInt(clear_time).ToString() + "��";
		cleartext.text = "Ŭ����ð� : " + Mathf.CeilToInt(clear_time).ToString() + "��";
	}

	//���� �������� ��ȣ ȣ���ϴ� �Լ� 20230506
	public int NowStage()
	{
		return sceneNumber;
	}

}

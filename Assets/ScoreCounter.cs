using UnityEngine;
using System.Collections;

using UnityEngine.UI;
public class ScoreCounter : MonoBehaviour
{
	public Block blockenum;
	public struct Count
	{ // ���� ������ ����ü.
		public int ignite; // ���� ��.
		public int score; // ����.
		public int total_socre; // �հ� ����.
		public int cur_score; //���� ���� ���� ���� //20230504 �߰�
		public int combo; //���� ���� �޺� �� //20230504 �߰�
		public int pink;
		public int blue;
		public int yellow;
		public int green;	//������ ������߰� //20230523
		public int magenta;
		public int orange;
		
	};
	public Count last; // ������(�̹�) ����.
	public Count best; // �ְ� ����.
	public static int CLEAR_SCORE = 0;// ���� �����ϰ� 0�̵Ǹ� Ŭ���� //20230504 �߰�
	public static int QUOTA_SCORE = 1000; // Ŭ��� �ʿ��� ����.
	public GUIStyle guistyle; // ��Ʈ ��Ÿ��.

	public SceneControl stagenum;       //scenecontrol�� ���� stage�� Ȯ���Ҽ��ִ� scenenumber�� ������������ ���� 20230506
	private AudioSource WinSoundSource;
	public AudioClip WinSound;

	public Text PinkRemain;
    public Text YellowRemain;
    public Text GreenRemain;
    public Text MagentaRemain;
    public Text BlueRemain;
    public Text OrangeRemain;
	public Text Combo;
	void Start()
	{
		stagenum.GetComponent<SceneControl>();		//stagenum�ʱ�ȭ
		this.last.ignite = 0;
		this.last.score = 0;
		this.last.total_socre = 0;
		this.guistyle.fontSize = 36;
		this.last.pink = 0;
		this.last.blue = 0;
		this.last.green = 0;
		this.last.magenta = 0;
		this.last.orange = 0;

		this.WinSoundSource = this.gameObject.AddComponent<AudioSource>();

		//
		if (stagenum.NowStage() == 1)			//���� ���������� Ȯ���ϰ� ���罺������������ stagenum�� �ٸ����Ͽ� ���������α���
		{
			Debug.Log(stagenum.NowStage());
			//this.last.cur_score = 40;
			this.last.pink = Random.Range(10, 25);
			this.last.blue = Random.Range(10, 20);
			this.last.yellow = Random.Range(10, 25);
			this.last.green = Random.Range(10, 25);
			this.last.magenta = Random.Range(10, 25);
			this.last.orange = Random.Range(10, 25); 
		}

		if (stagenum.NowStage() == 2)           //���� ���������� Ȯ���ϰ� ���罺������������ stagenum�� �ٸ����Ͽ� ���������α���
		{
			Debug.Log(stagenum.NowStage());
			this.last.pink = Random.Range(15, 30);
			this.last.blue = Random.Range(15, 30);
			this.last.yellow = Random.Range(15, 30);
			this.last.green = Random.Range(15, 30);
			this.last.magenta = Random.Range(15, 30);
			this.last.orange = Random.Range(15, 30);
			//this.last.cur_score = 45;
		}

		if (stagenum.NowStage() == 3)           //���� ���������� Ȯ���ϰ� ���罺������������ stagenum�� �ٸ����Ͽ� ���������α���
		{
			Debug.Log(stagenum.NowStage());
			this.last.pink = Random.Range(15, 30);
			this.last.blue = Random.Range(15, 30);
			this.last.yellow = Random.Range(15, 30);
			this.last.green = Random.Range(15, 30);
			this.last.magenta = Random.Range(15, 30);
			this.last.orange = Random.Range(15, 30);
			//this.last.cur_score = 50;
		}

		this.last.combo = 0;
	}
    private void Update()
    {
		PinkRemain.text = " X" + this.last.pink;
		YellowRemain.text = " X" + this.last.yellow;
		BlueRemain.text = " X" + this.last.blue;
		GreenRemain.text = " X" + this.last.green;
		MagentaRemain.text = " X" + this.last.magenta;
		OrangeRemain.text = " X" + this.last.orange;
		Combo.text = "�����޺� : " + this.last.combo;
	}
    void OnGUI()
	{
		int x = 20;
		int y = 50;
		GUI.color = Color.black;
		//this.print_value(x + 20, y, "��ȭ ī��Ʈ", this.last.ignite);
		//y += 50;
		//this.print_value(x + 20, y, "���� ���ھ�", this.last.score);
		//y += 50;
		//this.print_value(x + 20, y, "�հ� ���ھ�", this.last.total_socre);          //20230504 �ʿ���� ���ھ� ����
		//y += 50;
		//this.print_value(x + 20, y, "���� ���� ����", this.last.cur_score);
		//y += 80;
		
		//this.print_value(x + 1200, y, "�����޺� ��", this.last.combo);
		//y += 160;
	
		
	}
	public void print_value(int x, int y, string label, int value)
	{
		// label�� ǥ��.
		GUI.Label(new Rect(x, y, 100, 20), label, guistyle);
		y += 40;
		// ���� �࿡ value�� ǥ��.
		GUI.Label(new Rect(x + 20, y, 100, 20), value.ToString(), guistyle);
		y += 80;
	}
	public void addIgniteCount(int count)
	{
		this.last.ignite += count; // ��ȭ ���� count�� ����.
		this.update_score(); // ������ ���.
	}
	public void clearIgniteCount()
	{
		this.last.ignite = 0; // ��ȭ Ƚ���� ����.
	}
	private void update_score()
	{
		this.last.score = this.last.ignite * 10; // ���ھ ����.
	}
	public void updateTotalScore()
	{
		this.last.total_socre += this.last.score; // �հ� ���ھ ����.
	}
	public void DiffBlockCount(int count)// 20230504 ��ü ������������ ���� ���� ���� �Լ�
    {
		this.last.cur_score -= count;
	}
	public void ColorDiffCheck(Block.COLOR C) // 20230523 ���� ���� ���� ���� �Լ�
    {
		if (C == Block.COLOR.PINK)//5��ġ�� ��ũ���̸� ��ũ�� ����
		{
			this.last.pink -= 1;
            if (this.last.pink <= 0)
            {
				this.last.pink = 0;
			}
		}
		else if (C== Block.COLOR.BLUE)
		{
			this.last.blue -= 1;
			if (this.last.blue <= 0)
			{
				this.last.blue = 0;
			}
		}
		else if (C == Block.COLOR.YELLOW)
		{
			this.last.yellow -= 1;
			if (this.last.yellow <= 0)
			{
				this.last.yellow = 0;
			}
		}
		else if (C == Block.COLOR.GREEN)
		{
			this.last.green -= 1;
			if (this.last.green <= 0)
			{
				this.last.green= 0;
			}
			//Debug.Log("�����");
		}
		else if (C == Block.COLOR.MAGENTA)
		{
			this.last.magenta -= 1;
			if (this.last.magenta <= 0)
			{
				this.last.magenta = 0;
			}
		}
		else if (C == Block.COLOR.ORANGE)
		{
			this.last.orange -= 1;
			if (this.last.orange <= 0)
			{
				this.last.orange = 0;
			}
		}
 
		
	}
	public void ComboCount(int count, bool state)// 20230504 �������� ���� �޺� �Լ�       
	{
		if (state == true)
		{
			this.last.combo += 1;
			//Debug.Log("������");
		}

		else if (!state)
		{
			this.last.combo = 0;
		}
	}
	

	public bool isGameClear()
	{
		bool is_clear = false;

		//     if (this.last.cur_score <= CLEAR_SCORE)//20230523 �������� ����
		//     {
		//is_clear = true;
		//     }


		if (this.last.pink==0 && this.last.blue == 0 && this.last.yellow == 0 && this.last.green == 0 && this.last.magenta == 0 &&
			this.last.orange == 0)
        {
			WinSoundSource.PlayOneShot(WinSound);
			is_clear = true;
		}
		return (is_clear);
	}
	
}

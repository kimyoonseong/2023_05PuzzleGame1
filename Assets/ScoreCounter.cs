using UnityEngine;
using System.Collections;

public class ScoreCounter : MonoBehaviour
{

	public struct Count
	{ // ���� ������ ����ü.
		public int ignite; // ���� ��.
		public int score; // ����.
		public int total_socre; // �հ� ����.
		public int cur_score; //���� ���� ���� ���� //20230504 �߰�
		public int combo; //���� ���� �޺� �� //20230504 �߰�          ���� ���Ⱑ ���� �޺����� ����
	};
	public Count last; // ������(�̹�) ����.
	public Count best; // �ְ� ����.
	public static int CLEAR_SCORE = 0;// ���� �����ϰ� 0�̵Ǹ� Ŭ���� //20230504 �߰�
	public static int QUOTA_SCORE = 1000; // Ŭ��� �ʿ��� ����.
	public GUIStyle guistyle; // ��Ʈ ��Ÿ��.

	public SceneControl stagenum;		//scenecontrol�� ���� stage�� Ȯ���Ҽ��ִ� scenenumber�� ������������ ���� 20230506

	void Start()
	{
		stagenum.GetComponent<SceneControl>();		//stagenum�ʱ�ȭ
		this.last.ignite = 0;
		this.last.score = 0;
		this.last.total_socre = 0;
		this.guistyle.fontSize = 36;
		//
		if (stagenum.NowStage() >= 1)			//���� ���������� Ȯ���ϰ� ���罺������������ stagenum�� �ٸ����Ͽ� ���������α���
		{
			Debug.Log(stagenum.NowStage());
			this.last.cur_score = 20 * stagenum.stagenum;
		}

		this.last.combo = 0;
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
		this.print_value(x + 20, y, "���� ���� ����", this.last.cur_score);
		y += 80;
		this.print_value(x + 20, y, "�����޺� ��", this.last.combo);
		y += 80;
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
		// ���� �հ� ���ھ Ŭ���� ���غ��� ũ�ٸ�. <---���� �������� ���� 20230503
		//if (this.last.total_socre > QUOTA_SCORE)
		//{
		//	is_clear = true;
		//}
        if (this.last.cur_score <= CLEAR_SCORE)
        {
			is_clear = true;
        }
		return (is_clear);
	}
	
}

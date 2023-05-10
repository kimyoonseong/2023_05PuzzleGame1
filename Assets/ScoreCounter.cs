using UnityEngine;
using System.Collections;

public class ScoreCounter : MonoBehaviour
{

	public struct Count
	{ // 점수 관리용 구조체.
		public int ignite; // 연쇄 수.
		public int score; // 점수.
		public int total_socre; // 합계 점수.
		public int cur_score; //현재 남은 보석 갯수 //20230504 추가
		public int combo; //현재 연속 콤보 수 //20230504 추가          은직 여기가 이제 콤보띄우는 변수
	};
	public Count last; // 마지막(이번) 점수.
	public Count best; // 최고 점수.
	public static int CLEAR_SCORE = 0;// 블럭을 제거하고 0이되면 클리어 //20230504 추가
	public static int QUOTA_SCORE = 1000; // 클리어에 필요한 점수.
	public GUIStyle guistyle; // 폰트 스타일.

	public SceneControl stagenum;		//scenecontrol에 현재 stage를 확인할수있는 scenenumber를 가져오기위한 변수 20230506

	void Start()
	{
		stagenum.GetComponent<SceneControl>();		//stagenum초기화
		this.last.ignite = 0;
		this.last.score = 0;
		this.last.total_socre = 0;
		this.guistyle.fontSize = 36;
		//
		if (stagenum.NowStage() >= 1)			//현재 스테이지를 확인하고 현재스테이지에따라 stagenum을 다르게하여 레벨디자인구현
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
		//this.print_value(x + 20, y, "발화 카운트", this.last.ignite);
		//y += 50;
		//this.print_value(x + 20, y, "가산 스코어", this.last.score);
		//y += 50;
		//this.print_value(x + 20, y, "합계 스코어", this.last.total_socre);          //20230504 필요없는 스코어 삭제
		//y += 50;
		this.print_value(x + 20, y, "남은 보석 갯수", this.last.cur_score);
		y += 80;
		this.print_value(x + 20, y, "연속콤보 수", this.last.combo);
		y += 80;
	}
	public void print_value(int x, int y, string label, int value)
	{
		// label을 표시.
		GUI.Label(new Rect(x, y, 100, 20), label, guistyle);
		y += 40;
		// 다음 행에 value를 표시.
		GUI.Label(new Rect(x + 20, y, 100, 20), value.ToString(), guistyle);
		y += 80;
	}
	public void addIgniteCount(int count)
	{
		this.last.ignite += count; // 발화 수에 count를 가산.
		this.update_score(); // 점수를 계산.
	}
	public void clearIgniteCount()
	{
		this.last.ignite = 0; // 발화 횟수를 리셋.
	}
	private void update_score()
	{
		this.last.score = this.last.ignite * 10; // 스코어를 갱신.
	}
	public void updateTotalScore()
	{
		this.last.total_socre += this.last.score; // 합계 스코어를 갱신.
	}
	public void DiffBlockCount(int count)// 20230504 전체 보석갯수에서 맞춘 보석 차감 함수
    {
		this.last.cur_score -= count;
	}
	public void ComboCount(int count, bool state)// 20230504 연속으로 맞춘 콤보 함수       
	{
		if (state == true)
		{
			this.last.combo += 1;
			//Debug.Log("ㅁㄴㅇ");
		}

		else if (!state)
		{
			this.last.combo = 0;
		}
	}

	public bool isGameClear()
	{
		bool is_clear = false;
		// 현재 합계 스코어가 클리어 기준보다 크다면. <---기존 성공조건 삭제 20230503
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

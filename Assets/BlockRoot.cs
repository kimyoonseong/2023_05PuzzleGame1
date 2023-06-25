using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class BlockRoot : MonoBehaviour
{
	public GameObject feverEffect;
	//public GameObject Match3Effect;
	//public GameObject BombPrefab = null;//4매치 폭탄 블록
	public GameObject BlockPrefab = null; // 만들어 낼 블록의 프리팹.
	public BlockControl[,] blocks; // 그리드.

	private GameObject main_camera = null; // 메인 카메라.
	private BlockControl grabbed_block = null; // 잡은 블록.

	private ScoreCounter score_counter = null; // ScoreCounter.
	protected bool is_vanishing_prev = false; // 이전에 발화했었는가?.

	public TextAsset levelData = null; // 레벨 데이터의 텍스트를 저장.
	public LevelControl level_control; // LevelControl를 저장.

	private bool check1 = true;

	private bool cantchange = true;

	public float FeverTimer = 5.0f;
	private int IntteruptCount = 0;

	public Slider sltimer;

	//class변수추가
	private SceneControl stagenum;



	private AudioSource BlockChangeaudio;
	public AudioClip BlockChangeSound;
	public AudioClip Bomb4sound;
	public AudioClip Bomb5sound;
	public AudioClip CollectSound;
	public AudioClip FeverSound;
	private bool isInputEnabled = true;
	void Start()
	{
		this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
		this.score_counter = this.gameObject.GetComponent<ScoreCounter>();
		//start부분 참조 추가
		stagenum = GameObject.Find("GameRoot").GetComponent<SceneControl>();

		this.BlockChangeaudio = this.gameObject.AddComponent<AudioSource>();
		sltimer.value = 5.0f;
		sltimer.gameObject.SetActive(false);

	}


	void Update()
	{
		Vector3 mouse_position; // 마우스 위치.
		this.unprojectMousePosition( // 마우스 위치를 획득.
									out mouse_position, Input.mousePosition);
		// 획득한 마우스 위치를 X와 Y만으로 한다.
		Vector2 mouse_position_xy =
			new Vector2(mouse_position.x, mouse_position.y);

		if (check1)
		{
			foreach (BlockControl block in this.blocks)
			{
				if (this.CheckStartBlock(block))
				{
					block.color = this.selectBlockColor();
				}
			}
		}

		if (score_counter.last.combo != 0)
		{
			ComboTimer();
		}

		check1 = false;

		if (this.grabbed_block == null)
		{ // 블록을 잡지 않았을 때.
			if (!this.is_has_falling_block())
			{
				if (Input.GetMouseButtonDown(0) && isInputEnabled)
				{ // 마우스 버튼이 눌렸다면.
				  // blocks 배열의 모든 요소를 차례로 처리한다.

					foreach (BlockControl block in this.blocks)
					{
						
						if (!block.isGrabbable())
						{ // 블록을 잡을 수 없으면.
							continue; // 다음 블록으로.
						}
						// 마우스 위치가 블록 영역 안에 없으면.
						if (!block.isContainedPosition(mouse_position_xy))
						{
							continue; // 다음 블록으로.
						}
						
						//Debug.Log(block.pop5Color);
						if (block.color == Block.COLOR.Bomb)//20230510 4매치폭탄 눌렸을때
						{
							
							this.BlockChangeaudio.clip = this.Bomb4sound;
							this.BlockChangeaudio.loop = false;
							BlockChangeaudio.PlayOneShot(Bomb4sound);
							for (int x = 0; x < Block.BLOCK_NUM_X; x++)
							{
								if (blocks[x, block.i_pos.y].color != Block.COLOR.Wall)
								{
									this.blocks[x, block.i_pos.y].Match4Effect.SetActive(true);
									this.blocks[x, block.i_pos.y].toVanishing();
									//if (block.step == Block.STEP.IDLE)//발화중에 누르면 계속 점수 카운트 되는거 수정
									//{
									//	continue;//
									//}
									Debug.Log(blocks[x, block.i_pos.y].step);
									score_counter.ColorDiffCheck(blocks[x, block.i_pos.y].color);

								}
							}
							for (int y = block.i_pos.y + 1; y < Block.BLOCK_NUM_Y; y++)
							{
								if (blocks[block.i_pos.x, y].color != Block.COLOR.Wall)
								{
									this.blocks[block.i_pos.x, y].Match4Effect.SetActive(true);
									this.blocks[block.i_pos.x, y].toVanishing();


									score_counter.ColorDiffCheck(blocks[block.i_pos.x, y].color);


									//isCounted2 = true;

								}


							}
							for (int y = 0; y < block.i_pos.y; y++)
							{
								if (blocks[block.i_pos.x, y].color != Block.COLOR.Wall)
								{


									this.blocks[block.i_pos.x, y].Match4Effect.SetActive(true);
									this.blocks[block.i_pos.x, y].toVanishing();

									score_counter.ColorDiffCheck(blocks[block.i_pos.x, y].color);

									//if (block.next_step == Block.STEP.VACANT)//발화중에 누르면 계속 점수 카운트 되는거 수정
									//{
									//	continue;//
									//}
									//block.step = Block.STEP.VACANT;
									//isCounted3 = true;



									//Debug.Log(blocks[block.i_pos.x, y].color);
								}
							}
							isInputEnabled = false;
							Invoke("EnableInput", 0.8f);
							if (block.step == Block.STEP.IDLE)//발화중에 누르면 계속 점수 카운트 되는거 수정
							{
								continue;//
							}
							

						}
						else if (block.color == Block.COLOR.POP5)//20230510 5매치폭탄 눌렸을때
						{
							Debug.Log(block.pop5Color);
							this.BlockChangeaudio.clip = this.Bomb5sound;
							this.BlockChangeaudio.loop = false;
							BlockChangeaudio.PlayOneShot(Bomb5sound);
							foreach (BlockControl block2 in this.blocks)
							{
								if (block.pop5Color == block2.color)
								{
									block2.Match4Effect.SetActive(true);
									block2.toVanishing();

									score_counter.ColorDiffCheck(block.pop5Color);


								}
							}
							this.blocks[block.i_pos.x, block.i_pos.y].toVanishing();

							isInputEnabled = false;
							Invoke("EnableInput", 0.8f);

						}
						else if (block.color == Block.COLOR.FeverItem)//20230511 피버모드 아이템
						{

							for (int i = -1; i < 2; i++)
							{
								for (int j = -1; j < 2; j++)
								{

									if (block.i_pos.x + i < Block.BLOCK_NUM_X && block.i_pos.x + i >= 0)
									{
										if (block.i_pos.y + j < Block.BLOCK_NUM_Y && block.i_pos.y + j >= 0)
										{
											if (blocks[block.i_pos.x + i, block.i_pos.y + j].color != Block.COLOR.Wall)
											{
												this.blocks[block.i_pos.x + i, block.i_pos.y + j].Match5Effect.SetActive(true);
												this.blocks[block.i_pos.x + i, block.i_pos.y + j].toVanishing();

												score_counter.ColorDiffCheck(blocks[block.i_pos.x + i, block.i_pos.y + j].color);
												//}
												


												//Debug.Log(this.blocks[block.i_pos.x + i, block.i_pos.y + j]);
											}

										}
									}

								}
							}

							isInputEnabled = false;
							Invoke("EnableInput", 0.5f);
							if (block.step == Block.STEP.IDLE)//발화중에 누르면 계속 점수 카운트 되는거 수정
							{
								continue;//
							}
						}
						foreach (BlockControl block3 in this.blocks)
						{
							block3.Match4Effect.SetActive(false);
							block3.Match5Effect.SetActive(false);
						}
						// 처리 중인 블록을 grabbed_block에 등록.
						this.grabbed_block = block;
						// 잡았을 때의 처리를 실행.
						if (this.grabbed_block.color == Block.COLOR.Obstacle)
						{
							grabbed_block = null;
						}
						else if (this.grabbed_block.color == Block.COLOR.Wall)
						{
							grabbed_block = null;
						}
						else
						{
							this.grabbed_block.beginGrab();
						}
						break;
						
					}
				}
			}
		}
		else
		{ // 블록을 잡고 있을 때.


			do
			{
				// 슬라이드할 곳의 블록을 가져온다.
				BlockControl swap_target =
					this.getNextBlock(grabbed_block, grabbed_block.slide_dir);


				// 슬라이드할 곳 블록이 비어 있다면.
				if (swap_target == null)
				{
					break; // 루프 탈출. 
				}
				// 슬라이드할 곳 블록을 잡을 수 있는 상태가 아니라면.
				if (!swap_target.isGrabbable())
				{
					break; // 루프 탈출. 
				}

				if (is_has_sliding_block())
				{
					break;
				}

				//  현재 위치에서 슬라이드할 곳까지의 거리를 구한다.
				float offset = this.grabbed_block.calcDirOffset(
					mouse_position_xy, this.grabbed_block.slide_dir);
				// 이동 거리가 블록 크기의 절반보다 작다면 .
				if (offset < Block.COLLISION_SIZE / 2.0f)
				{
					break; // 루프 탈출. 
				}

				//if (!this.checkConnection(this.grabbed_block) || !this.checkConnection(swap_target))
				//{
				//	Debug.Log(grabbed_block);
				//	this.score_counter.ComboCount(0, false);
				//}

				//슬라이드할 곳이 장애물이거나 벽이면 루프탈출   2023 0526
				if (swap_target.color == Block.COLOR.Obstacle)
				{
					break;
				}

				//슬라이드할 곳이 장애물이거나 벽이면 루프탈출  2023 0526
				if (swap_target.color == Block.COLOR.Wall)
				{
					break;
				}


				if (cantchange)
				{
					// 블록을 교체한다.
					this.swapBlock(
						grabbed_block, grabbed_block.slide_dir, swap_target);
					//Debug.Log(grabbed_block);
				}


				//bool check 변수 만들기 20230506
				bool checkcheck = true;
				int match_count1 = 1; //바꿨을때 매치된 보석 수 20230506

				//블록을 잡고 이동을 하였을경우 모든 블록을 검사함. 20230506
				foreach (BlockControl block in this.blocks)
				{
					block.Match3Effect.SetActive(false);
					if (this.checkConnection(block))            //3매치블록이 겹치는경우
					{
						this.BlockChangeaudio.clip = this.CollectSound;
						this.BlockChangeaudio.PlayDelayed(0.2f);
						//BlockChangeaudio.PlayOneShot(CollectSound);
						this.score_counter.DiffBlockCount(match_count1); //매치된 보석수 scorecounter스크립트 함수에 삽입 20230504
						this.score_counter.ComboCount(0, true);     //콤보카운트 변경
						checkcheck = false;     //check check를 false로 변경
						for (int i = 1; i < 9; i++)
						{
							if (this.score_counter.last.combo == 7 * i)
							{
								BlockChangeaudio.PlayOneShot(FeverSound);
								feverEffect.SetActive(true);
								Invoke("turnoffFE", 1f);
								CreateFeverBlock();
							}
						}
						FeverTimer = 5.0f;
						sltimer.value = 5.0f;
						sltimer.gameObject.SetActive(true);

						break;

					}

				}

				if (checkcheck)     //checkcheck가 그대로 true일경우 3매치가 되지않았다는뜻이므로
				{
					CreateInterruptBlock();

					StartCoroutine(returnBlock(grabbed_block, grabbed_block.slide_dir, swap_target));

					//combo 초기화
					this.score_counter.ComboCount(0, false);
					sltimer.gameObject.SetActive(false);


				}



				this.grabbed_block = null; // 지금은 블록을 잡고 있지 않다.



			} while (false);



			if (!Input.GetMouseButton(0))
			{ // 마우스 버튼이 눌려져 있지 않으면.
				this.grabbed_block.endGrab(); // 블록을 놓았을 때의 처리를 실행.
				this.grabbed_block = null; //  grabbed_block을 비게 설정.

			}
		}

		// 낙하 중 또는 슬라이드 중이면.
		if (this.is_has_falling_block() || this.is_has_sliding_block())
		{
			// 아무것도 하지 않는다.
			// 낙하 중도 슬라이드 중도 아니면.
		}
		else
		{
			//int combo_count = 0;// 콤보가운트 20230504
			int match_count = 1; // 매치된 보석 수 20230504
			int ignite_count = 0; // 발화 수.
								  // 그리드 안의 모든 블록에 대해서 처리.

			cantchange = true;
			foreach (BlockControl block in this.blocks)
			{
				if (!block.isIdle())
				{ // 대기 중이면 루프의 처음으로 점프하고,.\
				  //Debug.Log(block);
					continue; // 다음 블록을 처리한다.
				}
				// 세로 또는 가로에 같은 색 블록이 세 개 이상 나열했다면.
				if (this.checkConnection(block))
				{
					this.BlockChangeaudio.clip = this.CollectSound;
					this.BlockChangeaudio.loop = false;
					BlockChangeaudio.PlayOneShot(CollectSound);
					ignite_count++; // 발화 수를 증가.
									//combo_count++;  //20230504 콤보 증가            
									//Debug.Log(block);
									//this.score_counter.ComboCount(combo_count, true);
					this.score_counter.DiffBlockCount(match_count);//매치된 보석수 scorecounter스크립트 함수에 삽입 20230504
				}


			}


			if (ignite_count > 0)
			{ // 발화 수가 0보다 크면.

				if (!this.is_vanishing_prev)
				{
					// 직전에 연쇄가 아니라면 발화 횟수 리셋.
					this.score_counter.clearIgniteCount();

				}
				// 발화 횟수를 늘린다.
				this.score_counter.addIgniteCount(ignite_count);
				// 합계 스코어 갱신.
				this.score_counter.updateTotalScore();



				// ＝한 군데라도 맞춰진 곳이 있으면.
				int block_count = 0; // 발화 중인 블록 수(다음 장에서 사용한다).
									 // 그리드 내의 모든 블록에 대해서 처리.
				foreach (BlockControl block in this.blocks)
				{
					if (block.isVanishing())
					{ // 발화중（점점 사라진다）이면.
						block.rewindVanishTimer(); // 재발화！.
						block_count++; // 발화 중인 블록의 개수를 증가.
					}
				}
			}

		}

		// 하나라도 연소 중인 블록이 있는가?.
		bool is_vanishing = this.is_has_vanishing_block();
		// 조건을 만족하면 블록을 떨어뜨리고 싶다.
		do
		{
			if (is_vanishing)
			{ // 연소 중인 블록이 있다면.
				break; // 낙하 처리를 실행하지 않는다.
			}
			if (this.is_has_sliding_block())
			{ // 교체 중인 블록이 있다면.
				break; // 낙하 처리를 실행하지 않는다.
			}
			for (int x = 0; x < Block.BLOCK_NUM_X; x++)
			{
				// 열에 교체 중인 블록이 있다면, 그 열은 처리하지 않고 다음 열로 진행한다.
				if (this.is_has_sliding_block_in_column(x))
				{
					continue;
				}
				// 그 열에 있는 블록을 위에서부터 검사.
				for (int y = 0; y < Block.BLOCK_NUM_Y - 1; y++)
				{
					// 지정 중인 블록이 비표시라면, 다음 블록으로.
					if (!this.blocks[x, y].isVacant())
					{
						continue;
					}
					// 지정 중인 블록 아래에 있는 블록을 검사.
					for (int y1 = y + 1; y1 < Block.BLOCK_NUM_Y; y1++)
					{
						// 아래에 있는 블록이 비표시라면, 다음 블록으로.
						if (this.blocks[x, y1].isVacant())
						{
							continue;
						}
						//  블록을 교체한다.
						this.fallBlock(this.blocks[x, y], Block.DIR4.UP,
									   this.blocks[x, y1]);
						break;
					}
				}
			}
			// 보충처리.
			for (int x = 0; x < Block.BLOCK_NUM_X; x++)
			{
				int fall_start_y = Block.BLOCK_NUM_Y;
				for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
				{
					// 비표시 블록이 아니라면 다음 블록으로.
					if (!this.blocks[x, y].isVacant())
					{
						continue;
					}
					this.blocks[x, y].beginRespawn(fall_start_y); // 블록 부활.
					fall_start_y++;
				}
			}
		} while (false);
		CheckWall();
		SetWall();
		this.is_vanishing_prev = is_vanishing;
	}






	// 블록을 만들어 내고, 가로 아홉 칸 세로 아홉 칸으로 배치.
	public void initialSetUp()
	{
		// 크기는 9×9로 한다.
		this.blocks =
			new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];
		// 블록의 색 번호.
		int color_index = 0;

		Block.COLOR color = Block.COLOR.PINK;


		for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
		{ // 처음행부터 시작행부터 마지막행까지.
			for (int x = 0; x < Block.BLOCK_NUM_X; x++)
			{// 왼쪽 끝에서부터 오른쪽 끝까지.
			 // BlockPrefab의 인스턴스를 씬 위에 만든다.
				GameObject game_object =
					Instantiate(this.BlockPrefab) as GameObject;
				// 위에서 만든 블록의 BlockControl 클래스를 가져온다.
				BlockControl block = game_object.GetComponent<BlockControl>();
				// 블록을 칸에 넣는다.
				this.blocks[x, y] = block;
				// 블록의 위치 정보(그리드 좌표)를 설정.
				block.i_pos.x = x;
				block.i_pos.y = y;
				// 각 BlockControl이 연계하는 GameRoot는 자신이라고 설정.
				block.block_root = this;
				// 그리드 좌표를 실제 위치(씬 좌표)로 변환.
				Vector3 position = BlockRoot.calcBlockPosition(block.i_pos);
				// 씬 상의 블록 위치를 이동.
				block.transform.position = position;

				// 블록의 색을 변경. 
				// block.setColor((Block.COLOR)color_index);
				// 지금의 출현 확률을 바탕으로 색을 결정한다.
				color = this.selectBlockColor();
				block.setColor(color);

				// 블록의 이름을 설정(후술).
				block.name = "block(" + block.i_pos.x.ToString() +
					"," + block.i_pos.y.ToString() + ")";
				// 모든 종류의 색 중에서 임의로 한 색을 선택.
				color_index =
					Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
			}
		}
	}


	// 지정된 그리드 좌표에서 씬 상의 좌표를 구한다. 
	public static Vector3 calcBlockPosition(Block.iPosition i_pos)
	{
		// 배치할 좌측 상단 모퉁이 위치를 초깃값으로 설정.
		Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f),
									   -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);
		// 초깃값＋그리드 좌표 × 블록 크기.
		position.x += (float)i_pos.x * Block.COLLISION_SIZE;
		position.y += (float)i_pos.y * Block.COLLISION_SIZE;
		return (position); // 씬의 좌표를 반환한다.
	}


	public bool unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position)
	{
		bool ret;
		// 판을 생성. 이 판은 카메라에서 보이는 면이 앞.
		// 블록의 절반 크기만큼 앞으로 놓인다.
		Plane plane = new Plane(Vector3.back, new Vector3(
			0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));
		// 카메라와 마우스를 통과하는 광선을 생성.
		Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(
			mouse_position);
		float depth;
		// 광선 ray가 판 plane에 닿았다면.
		if (plane.Raycast(ray, out depth))
		{
			// 인수 world_position을 마우스 위치로 덮어쓴다.
			world_position = ray.origin + ray.direction * depth;
			ret = true;
			// 닿지 않았다면.
		}
		else
		{
			// 인수 world_position을 제로인 벡터로 덮어쓴다.
			world_position = Vector3.zero;
			ret = false;
		}
		return (ret);
	}




	public BlockControl getNextBlock(
		BlockControl block, Block.DIR4 dir)
	{
		// 슬라이드할 곳의 블록을 여기에 저장.
		BlockControl next_block = null;
		switch (dir)
		{
			case Block.DIR4.RIGHT:
				if (block.i_pos.x < Block.BLOCK_NUM_X - 1)
				{
					// 그리드 안이라면.
					next_block = this.blocks[block.i_pos.x + 1, block.i_pos.y];
					next_block.color = this.blocks[block.i_pos.x + 1, block.i_pos.y].color;     //다음블럭 색변환
				}
				break;

			case Block.DIR4.LEFT:
				if (block.i_pos.x > 0)
				{ // 그리드 안이라면.
					next_block = this.blocks[block.i_pos.x - 1, block.i_pos.y];
					next_block.color = this.blocks[block.i_pos.x - 1, block.i_pos.y].color;   //다음블럭 색변환
				}
				break;
			case Block.DIR4.UP:
				if (block.i_pos.y < Block.BLOCK_NUM_Y - 1)
				{ // 그리드 안이라면.
					next_block = this.blocks[block.i_pos.x, block.i_pos.y + 1];
					next_block.color = this.blocks[block.i_pos.x, block.i_pos.y + 1].color;   //다음블럭 색변환
				}
				break;
			case Block.DIR4.DOWN:
				if (block.i_pos.y > 0)
				{ // 그리드 안이라면.
					next_block = this.blocks[block.i_pos.x, block.i_pos.y - 1];
					next_block.color = this.blocks[block.i_pos.x, block.i_pos.y - 1].color;   //다음블럭 색변환
				}
				break;
		}
		return (next_block);
	}

	public static Vector3 getDirVector(Block.DIR4 dir)
	{
		Vector3 v = Vector3.zero;
		switch (dir)
		{
			case Block.DIR4.RIGHT: v = Vector3.right; break; // 오른쪽으로 1단위 이동한다.
			case Block.DIR4.LEFT: v = Vector3.left; break; // 왼쪽으로 1단위 이동한다.
			case Block.DIR4.UP: v = Vector3.up; break; // 위로 1단위 이동한다.
			case Block.DIR4.DOWN: v = Vector3.down; break; // 아래로 1단위 이동한다.
		}
		v *= Block.COLLISION_SIZE; // 블록 크기를 곱한다.
		return (v);
	}

	public static Block.DIR4 getOppositDir(Block.DIR4 dir)
	{
		Block.DIR4 opposit = dir;
		switch (dir)
		{
			case Block.DIR4.RIGHT: opposit = Block.DIR4.LEFT; break;
			case Block.DIR4.LEFT: opposit = Block.DIR4.RIGHT; break;
			case Block.DIR4.UP: opposit = Block.DIR4.DOWN; break;
			case Block.DIR4.DOWN: opposit = Block.DIR4.UP; break;
		}
		return (opposit);
	}



	public void swapBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
	{
		// 각 블록의 색을 기억해 둔다.
		Block.COLOR color0 = block0.color;
		Block.COLOR color1 = block1.color;
		// 각 블록의.
		// 확대율을 기억해 둔다.
		Vector3 scale0 =
			block0.transform.localScale;
		Vector3 scale1 =
			block1.transform.localScale;
		//  각 블록의 '사라지는 시간'을 기억해 둔다.
		float vanish_timer0 = block0.vanish_timer;
		float vanish_timer1 = block1.vanish_timer;
		// 각 블록이 이동할 곳을 구한다.
		Vector3 offset0 = BlockRoot.getDirVector(dir);
		Vector3 offset1 = BlockRoot.getDirVector(BlockRoot.getOppositDir(dir));
		block0.setColor(color1); //  색을 교체한다.
		block1.setColor(color0);
		block0.transform.localScale = scale1; // 확대율을 교체한다.
		block1.transform.localScale = scale0;
		block0.vanish_timer = vanish_timer1; // 사라지는 시간을 교체한다.
		block1.vanish_timer = vanish_timer0;
		block0.beginSlide(offset0); // 원래 블록의 이동을 시작.
		block1.beginSlide(offset1); // 이동할 곳의 블록 이동을 시작.

		this.BlockChangeaudio.clip = this.BlockChangeSound;
		this.BlockChangeaudio.loop = false;
		BlockChangeaudio.PlayOneShot(BlockChangeSound);
	}


	public bool checkConnection(BlockControl start)
	{
		bool ret = false;
		int normal_block_num = 0;

		// 인수인 블록이 발화 후가 아니면.
		if (!start.isVanishing())
		{
			normal_block_num = 1;
		}
		// 그리드 좌표를 기억해 둔다.
		int rx = start.i_pos.x;
		int lx = start.i_pos.x;
		// 블록의 왼쪽을 검사.
		for (int x = lx - 1; x >= 0; x--)
		{
			BlockControl next_block = this.blocks[x, start.i_pos.y];
			if (start.color == Block.COLOR.Obstacle)
			{
				break;
			}
			if (start.color == Block.COLOR.Wall)
			{
				break;
			}

			if (next_block.color != start.color)
			{ // 색이 다르면.
				break; // 루프 탈출.
			}
			if (next_block.step == Block.STEP.FALL || // 낙하 중이면.
			   next_block.next_step == Block.STEP.FALL)
			{
				break; // 루프 탈출.
			}
			if (next_block.step == Block.STEP.SLIDE || // 슬라이드 중이면.
			   next_block.next_step == Block.STEP.SLIDE)
			{
				break; // 루프 탈출.
			}
			if (!next_block.isVanishing())
			{ // 발화 중이 아니면.
				normal_block_num++; // 검사용 카운터를 증가.
			}
			lx = x;
		}
		// 블록의 오른쪽을 검사.
		for (int x = rx + 1; x < Block.BLOCK_NUM_X; x++)
		{
			BlockControl next_block = this.blocks[x, start.i_pos.y];
			if (start.color == Block.COLOR.Obstacle)
			{
				break;
			}
			if (start.color == Block.COLOR.Wall)
			{
				break;
			}
			if (next_block.color != start.color)
			{
				break;
			}
			if (next_block.step == Block.STEP.FALL ||
			   next_block.next_step == Block.STEP.FALL)
			{
				break;
			}
			if (next_block.step == Block.STEP.SLIDE ||
			   next_block.next_step == Block.STEP.SLIDE)
			{
				break;
			}
			if (!next_block.isVanishing())
			{

				normal_block_num++;
			}
			rx = x;
		}
		do
		{
			// 오른쪽 블록의 그리드 번호 - 왼쪽 블록의 그리드 번호 +.
			// 중앙 블록(1)을 더한 수가 3미만 이면.
			if (rx - lx + 1 < 3)
			{
				break; // 루프 탈출.
			}

			if (normal_block_num == 0)
			{ // 발화 중이 아닌 블록이 하나도 없으면.
				break; // 루프 탈출.
			}
			if (rx - lx + 1 == 4)//20230510 4매치 일때  움직인 블록 제외 다 터뜨리는 for문
			{
				for (int x = lx; x < start.i_pos.x; x++)
				{

					// 나열된 같은 색 블록을 발화 상태로.
					this.blocks[x, start.i_pos.y].Match3Effect.SetActive(true);
					this.blocks[x, start.i_pos.y].toVanishing();
					
					score_counter.ColorDiffCheck(this.blocks[x, start.i_pos.y].color);//20230523 색상확인해서 차감
																					  //score_counter.
					ret = true;
				}
				for (int x = start.i_pos.x + 1; x < rx + 1; x++)
				{
					// 나열된 같은 색 블록을 발화 상태로.
					this.blocks[x, start.i_pos.y].Match3Effect.SetActive(true);
					this.blocks[x, start.i_pos.y].toVanishing();
					
					score_counter.ColorDiffCheck(this.blocks[x, start.i_pos.y].color);//20230523 색상확인해서 차감
					ret = true;
				}

				score_counter.ColorDiffCheck(this.blocks[start.i_pos.x, start.i_pos.y].color);//20230523 이렇게되면 3개만 지워지니깐,


				this.blocks[start.i_pos.x, start.i_pos.y].color = Block.COLOR.Bomb;


			}
			else if (rx - lx + 1 == 5)//20230510 5매치 일때  움직인 블록 제외 다 터뜨리는 for문
			{
				//Debug.Log("5매치 실행");
				for (int x = lx; x < start.i_pos.x; x++)
				{
					// 나열된 같은 색 블록을 발화 상태로.
					this.blocks[x, start.i_pos.y].Match3Effect.SetActive(true);
					score_counter.ColorDiffCheck(this.blocks[x, start.i_pos.y].color);//20230523 색상확인해서 차감
					
					this.blocks[x, start.i_pos.y].toVanishing();
					
					ret = true;
				}
				for (int x = start.i_pos.x + 1; x < rx + 1; x++)
				{
					// 나열된 같은 색 블록을 발화 상태로.
					this.blocks[x, start.i_pos.y].Match3Effect.SetActive(true);
					score_counter.ColorDiffCheck(this.blocks[x, start.i_pos.y].color);//20230523 색상확인해서 차감
					
					this.blocks[x, start.i_pos.y].toVanishing();
					
					ret = true;
				}

				score_counter.ColorDiffCheck(this.blocks[start.i_pos.x, start.i_pos.y].color);//20230523이렇게하면 4개만 지워지니깐.
				this.blocks[start.i_pos.x, start.i_pos.y].pop5Color = this.blocks[start.i_pos.x, start.i_pos.y].color;//pop5color는 색을 저장
				this.blocks[start.i_pos.x, start.i_pos.y].color = Block.COLOR.POP5;
				//Debug.Log(this.blocks[start.i_pos.x, start.i_pos.y].pop5Color);
				//this.blocks[start.i_pos.y, start.i_pos.y].step = Block.STEP.FALL;
			}
			else
			{
				for (int x = lx; x < rx + 1; x++)
				{
					// 나열된 같은 색 블록을 발화 상태로.
					this.blocks[x, start.i_pos.y].Match3Effect.SetActive(true);
					score_counter.ColorDiffCheck(this.blocks[x, start.i_pos.y].color);
					
					this.blocks[x, start.i_pos.y].toVanishing();
					
					//근처에 방해물이 있는지 검사
					DestroyInterruptBlock(blocks[x, start.i_pos.y]);
					ret = true;
				}
			}
		} while (false);
		normal_block_num = 0;
		if (!start.isVanishing())
		{
			normal_block_num = 1;
		}
		int uy = start.i_pos.y;
		int dy = start.i_pos.y;
		// 블록의 위쪽을 검사.
		for (int y = dy - 1; y >= 0; y--)
		{
			BlockControl next_block = this.blocks[start.i_pos.x, y];
			if (start.color == Block.COLOR.Obstacle)
			{
				break;
			}
			if (start.color == Block.COLOR.Wall)
			{
				break;
			}
			if (next_block.color != start.color)
			{
				break;
			}
			if (next_block.step == Block.STEP.FALL ||
			   next_block.next_step == Block.STEP.FALL)
			{
				break;
			}
			if (next_block.step == Block.STEP.SLIDE ||
			   next_block.next_step == Block.STEP.SLIDE)
			{
				break;
			}
			if (!next_block.isVanishing())
			{
				normal_block_num++;
			}
			dy = y;
		}
		// 블록의 아래쪽을 검사.
		for (int y = uy + 1; y < Block.BLOCK_NUM_Y; y++)
		{
			BlockControl next_block = this.blocks[start.i_pos.x, y];
			if (start.color == Block.COLOR.Obstacle)
			{
				break;
			}
			if (start.color == Block.COLOR.Wall)
			{
				break;
			}
			if (next_block.color != start.color)
			{
				break;
			}
			if (next_block.step == Block.STEP.FALL ||
			   next_block.next_step == Block.STEP.FALL)
			{
				break;
			}
			if (next_block.step == Block.STEP.SLIDE ||
			   next_block.next_step == Block.STEP.SLIDE)
			{
				break;
			}
			if (!next_block.isVanishing())
			{
				normal_block_num++;
			}
			uy = y;
		}
		do
		{
			if (uy - dy + 1 < 3)
			{
				break;
			}
			if (normal_block_num == 0)
			{
				break;
			}
			if (uy - dy + 1 == 4)
			{

				for (int y = dy; y < start.i_pos.y; y++) //20230510 4매치 일때  움직인 블록 제외 다 터뜨리는 for문
				{
					//밑부분 발화
					this.blocks[start.i_pos.x, y].Match3Effect.SetActive(true);
					score_counter.ColorDiffCheck(this.blocks[start.i_pos.x, y].color);//20230523 색상확인해서 차감					
					this.blocks[start.i_pos.x, y].toVanishing();
					ret = true;
				}
				for (int y = start.i_pos.y + 1; y < uy + 1; y++)
				{
					this.blocks[start.i_pos.x, y].Match3Effect.SetActive(true);
					// 윗부분 발화
					score_counter.ColorDiffCheck(this.blocks[start.i_pos.x, y].color);//20230523 색상확인해서 차감
					
					this.blocks[start.i_pos.x, y].toVanishing();
					ret = true;
				}
				score_counter.ColorDiffCheck(this.blocks[start.i_pos.x, start.i_pos.y].color);//20230523이렇게하면 3개만 지워지니깐.
				this.blocks[start.i_pos.x, start.i_pos.y].color = Block.COLOR.Bomb;
				this.blocks[start.i_pos.y, start.i_pos.y].step = Block.STEP.FALL;
			}
			else if (uy - dy + 1 == 5)
			{
				//Block.COLOR co = this.blocks[start.i_pos.x, start.i_pos.y].color;
				score_counter.ColorDiffCheck(start.color);
				this.blocks[start.i_pos.x, start.i_pos.y - 2].pop5Color = start.color;
				start.color = Block.COLOR.POP5;
				//this.blocks[start.i_pos.y, start.i_pos.y].step = Block.STEP.FALL;
				//Debug.Log(start.pop5Color);
				for (int y = dy; y < start.i_pos.y; y++) //20230510 5매치 일때  움직인 블록 제외 다 터뜨리는 for문
				{
					//밑부분 발화
					score_counter.ColorDiffCheck(this.blocks[start.i_pos.x, y].color);//20230523 색상확인해서 차감
					this.blocks[start.i_pos.x, y].Match3Effect.SetActive(true);
					this.blocks[start.i_pos.x, y].toVanishing();
					ret = true;
				}
				for (int y = start.i_pos.y + 1; y < uy + 1; y++)
				{
					// 윗부분 발화
					this.blocks[start.i_pos.x, y].Match3Effect.SetActive(true);
					score_counter.ColorDiffCheck(this.blocks[start.i_pos.x, y].color);//20230523 색상확인해서 차감
					this.blocks[start.i_pos.x, y].toVanishing();
					ret = true;
				}

			}
			else
			{
				for (int y = dy; y < uy + 1; y++)
				{
					this.blocks[start.i_pos.x, y].Match3Effect.SetActive(true);
					//근처에 방해물이 있는지 검사
					score_counter.ColorDiffCheck(this.blocks[start.i_pos.x, y].color);//20230523 색상확인해서 차감
					DestroyInterruptBlock(blocks[start.i_pos.x, y]);
					
					this.blocks[start.i_pos.x, y].toVanishing();
					ret = true;
				}
			}
		} while (false);


		return (ret);
	}


	//불붙는중인 블록이 하나라도 있으면 true 반환
	private bool is_has_vanishing_block()
	{
		bool ret = false;
		foreach (BlockControl block in this.blocks)
		{
			if (block.vanish_timer > 0.0f)
			{
				ret = true;
				break;
			}
		}
		return (ret);
	}

	//슬라이드 중인 블록이 하나라도 있으면 true반환
	private bool is_has_sliding_block()
	{
		bool ret = false;
		foreach (BlockControl block in this.blocks)
		{
			if (block.step == Block.STEP.SLIDE)
			{
				ret = true;
				break;
			}
		}
		return (ret);
	}
	//낙하중인 블록이 하나라도 있으면 true 반환
	private bool is_has_falling_block()
	{
		bool ret = false;
		foreach (BlockControl block in this.blocks)
		{
			if (block.step == Block.STEP.FALL)
			{
				ret = true;
				break;
			}
		}
		return (ret);
	}

	public void fallBlock(
		BlockControl block0, Block.DIR4 dir, BlockControl block1)
	{
		// block0과 block1의 색, 크기, 사라질 때까지 걸리는 시간, 표시, 비표시, 상태를 기록.
		Block.COLOR color0 = block0.color;
		Block.COLOR color1 = block1.color;
		Vector3 scale0 = block0.transform.localScale;
		Vector3 scale1 = block1.transform.localScale;
		float vanish_timer0 = block0.vanish_timer;
		float vanish_timer1 = block1.vanish_timer;
		bool visible0 = block0.isVisible();
		bool visible1 = block1.isVisible();
		Block.STEP step0 = block0.step;
		Block.STEP step1 = block1.step;
		// block0과 block1의 각종 속성을 교체한다.
		block0.setColor(color1);
		block1.setColor(color0);
		block0.transform.localScale = scale1;
		block1.transform.localScale = scale0;
		block0.vanish_timer = vanish_timer1;
		block1.vanish_timer = vanish_timer0;
		block0.setVisible(visible1);
		block1.setVisible(visible0);
		block0.step = step1;
		block1.step = step0;
		block0.beginFall(block1);
	}


	private bool is_has_sliding_block_in_column(int x)
	{
		bool ret = false;
		for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
		{
			if (this.blocks[x, y].isSliding())
			{ // 슬라이드 중인 블록이 있으면.
				ret = true; // true를 반환한다. 
				break;
			}
		}
		return (ret);
	}



	public void create()
	{
		this.level_control = new LevelControl();
		this.level_control.initialize(); // 레벨 데이터 초기화.
		this.level_control.loadLevelData(this.levelData); // 데이터 읽기.
		this.level_control.selectLevel(); // 레벨 선택.
	}
	public Block.COLOR selectBlockColor()
	{
		Block.COLOR color = Block.COLOR.PINK;
		// 이번 레벨의 레벨 데이터를 가져온다.
		LevelData level_data =
			this.level_control.getCurrentLevelData();
		float rand = Random.Range(0.0f, 1.0f); // 0.0~1.0 사이의 난수.
		float sum = 0.0f; // 출현 확률의 합계.
		int i = 0;
		// 블록의 종류 전체를 처리하는 루프.
		for (i = 0; i < level_data.probability.Length - 1; i++)
		{
			if (level_data.probability[i] == 0.0f)
			{
				continue; // 출현 확률이 0이면 루프의 처음으로 점프.
			}
			sum += level_data.probability[i]; // 출현 확률을 더한다.
			if (rand < sum)
			{ // 합계가 난숫값을 웃돌면.
				break; // 루프를 빠져나온다.
			}
		}
		color = (Block.COLOR)i; // i번째 색을 반환한다.
		return (color);
	}


	//Range범위안에서 랜덤 블럭을 장애물로 바꾸는함수
	public void CreateInterruptBlock()
	{
		int x = Random.Range(0, Block.BLOCK_NUM_X);
		int y = Random.Range(0, Block.BLOCK_NUM_Y);

		if (IntteruptCount < 5)
		{
			if (blocks[x, y].color != Block.COLOR.Wall || blocks[x, y].color != Block.COLOR.FeverItem || blocks[x, y].color != Block.COLOR.Bomb || blocks[x, y].color != Block.COLOR.POP5)
			{
				blocks[x, y].setColor(Block.COLOR.Obstacle);
				IntteruptCount++;
			}
			else
			{
				Debug.Log("check");
				CreateInterruptBlock();
			}
		}
	}
	//Range범위안에서 랜덤 블럭을 피버아이템 바꾸는함수//20230511
	public void CreateFeverBlock()
	{
		for (int i = 0; i < 3; i++)//여기에 if문 넣어서 그 2,3스테이지 투명블럭은 제외하면 될듯?
		{
			int x = Random.Range(0, Block.BLOCK_NUM_X);
			int y = Random.Range(0, Block.BLOCK_NUM_Y);
			if (blocks[x, y].color == Block.COLOR.Wall)
			{
				i--;
			}
			else
			{
				blocks[x, y].setColor(Block.COLOR.FeverItem);
			}
		}

	}
	//BlockRoot에서 장애물을 지우는 함수
	public void DestroyInterruptBlock(BlockControl block)
	{
		if (block.i_pos.x + 1 < Block.BLOCK_NUM_X && blocks[block.i_pos.x + 1, block.i_pos.y].color == Block.COLOR.Obstacle)
		{
			blocks[block.i_pos.x + 1, block.i_pos.y].Match3Effect.SetActive(true);
			blocks[block.i_pos.x + 1, block.i_pos.y].toVanishing();
			IntteruptCount--;
		}
		if (block.i_pos.x - 1 >= 0 && blocks[block.i_pos.x - 1, block.i_pos.y].color == Block.COLOR.Obstacle)
		{
			blocks[block.i_pos.x - 1, block.i_pos.y].Match3Effect.SetActive(true);
			blocks[block.i_pos.x - 1, block.i_pos.y].toVanishing();
			IntteruptCount--;
		}
		if (block.i_pos.y + 1 < Block.BLOCK_NUM_X && blocks[block.i_pos.x, block.i_pos.y + 1].color == Block.COLOR.Obstacle)
		{
			blocks[block.i_pos.x, block.i_pos.y + 1].Match3Effect.SetActive(true);
			blocks[block.i_pos.x, block.i_pos.y + 1].toVanishing();
			IntteruptCount--;
		}
		if (block.i_pos.y - 1 >= 0 && blocks[block.i_pos.x, block.i_pos.y - 1].color == Block.COLOR.Obstacle)
		{
			blocks[block.i_pos.x, block.i_pos.y - 1].Match3Effect.SetActive(true);
			blocks[block.i_pos.x, block.i_pos.y - 1].toVanishing();
			IntteruptCount--;
		}
	}
	public void SetWall()
	{
		for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
		{ // 처음행부터 시작행부터 마지막행까지.
			for (int x = 0; x < Block.BLOCK_NUM_X; x++)
			{
				if (stagenum.NowStage() == 2)
				{
					//제일 오른쪽 아래
					if (x > 5 && y == 0)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}

					if (x > 6 && y == 1)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}

					if (x > 7 && y == 2)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}

					//제일 왼쪽 아래
					if (x < 3 && y == 0)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}

					if (x < 2 && y == 1)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}

					if (x < 1 && y == 2)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}


					////제일 오른쪽 위
					if (x > 5 && y == 8)
					{
						blocks[x, y].color = Block.COLOR.Wall;
						//blocks[x, y].setColor(Block.COLOR.Wall);
					}

					if (x > 6 && y == 7)
					{
						blocks[x, y].color = Block.COLOR.Wall;
						//blocks[x, y].setColor(Block.COLOR.Wall);
					}


					if (x > 7 && y == 6)
					{
						//blocks[x, y].setColor(Block.COLOR.Wall);
						blocks[x, y].color = Block.COLOR.Wall;
					}

					////제일 왼쪽 아래
					if (x < 3 && y == 8)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}

					if (x < 2 && y == 7)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}

					if (x < 1 && y == 6)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}
				}


				if (stagenum.NowStage() == 3)
				{
					if (y == 3 && x > 2 && x < 6)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}

					if (y == 4 && x > 2 && x < 6)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}

					if (y == 5 && x > 2 && x < 6)
					{
						blocks[x, y].setColor(Block.COLOR.Wall);
					}

					blocks[0, 0].setColor(Block.COLOR.Wall);
					blocks[8, 0].setColor(Block.COLOR.Wall);
					blocks[8, 8].setColor(Block.COLOR.Wall);
					blocks[0, 8].setColor(Block.COLOR.Wall);
				}
			}
		}
	}

	public void CheckWall()
	{
		for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
		{ // 처음행부터 시작행부터 마지막행까지.
			for (int x = 0; x < Block.BLOCK_NUM_X; x++)
			{
				if (this.blocks[x, y].color == Block.COLOR.Wall)
				{
					int color_index = Random.Range(
					(int)Block.COLOR.PINK, (int)Block.COLOR.LAST + 1);
					this.blocks[x, y].setColor((Block.COLOR)color_index);
				}
			}
		}
	}

	//블럭을 다시 되돌리는 코루틴 딜레이 0.3초를 주고 sawpblock함수를 다시 호출했다. 2023 05 26
	IEnumerator returnBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
	{
		cantchange = false;

		yield return new WaitForSeconds(0.2f);


		swapBlock(block0, dir, block1);
	}


	public bool CheckStartBlock(BlockControl start)
	{
		bool ret = false;
		int normal_block_num = 0;

		// 인수인 블록이 발화 후가 아니면.
		if (!start.isVanishing())
		{
			normal_block_num = 1;
		}
		// 그리드 좌표를 기억해 둔다.
		int rx = start.i_pos.x;
		int lx = start.i_pos.x;
		// 블록의 왼쪽을 검사.
		for (int x = lx - 1; x >= 0; x--)
		{
			BlockControl next_block = this.blocks[x, start.i_pos.y];
			if (start.color == Block.COLOR.Obstacle)
			{
				break;
			}
			if (start.color == Block.COLOR.Wall)
			{
				break;
			}

			if (next_block.color != start.color)
			{ // 색이 다르면.
				break; // 루프 탈출.
			}

			if (!next_block.isVanishing())
			{ // 발화 중이 아니면.
				normal_block_num++; // 검사용 카운터를 증가.
			}
			lx = x;
		}
		// 블록의 오른쪽을 검사.
		for (int x = rx + 1; x < Block.BLOCK_NUM_X; x++)
		{
			BlockControl next_block = this.blocks[x, start.i_pos.y];
			if (start.color == Block.COLOR.Obstacle)
			{
				break;
			}
			if (start.color == Block.COLOR.Wall)
			{
				break;
			}
			if (next_block.color != start.color)
			{
				break;
			}

			if (!next_block.isVanishing())
			{

				normal_block_num++;
			}
			rx = x;
		}
		do
		{
			// 오른쪽 블록의 그리드 번호 - 왼쪽 블록의 그리드 번호 +.
			// 중앙 블록(1)을 더한 수가 3미만 이면.
			if (rx - lx + 1 < 3)
			{
				break; // 루프 탈출.
			}

			if (normal_block_num == 0)
			{ // 발화 중이 아닌 블록이 하나도 없으면.
				break; // 루프 탈출.
			}

			if (rx - lx + 1 >= 3)
			{
				for (int x = lx; x < rx + 1; x++)
				{
					this.blocks[x, start.i_pos.y].color = this.selectBlockColor();
					//근처에 방해물이 있는지 검사
					ret = true;
				}
			}
		} while (false);
		normal_block_num = 0;
		if (!start.isVanishing())
		{
			normal_block_num = 1;
		}
		int uy = start.i_pos.y;
		int dy = start.i_pos.y;
		// 블록의 위쪽을 검사.
		for (int y = dy - 1; y >= 0; y--)
		{
			BlockControl next_block = this.blocks[start.i_pos.x, y];
			if (start.color == Block.COLOR.Obstacle)
			{
				break;
			}
			if (start.color == Block.COLOR.Wall)
			{
				break;
			}
			if (next_block.color != start.color)
			{
				break;
			}
			if (!next_block.isVanishing())
			{
				normal_block_num++;
			}
			dy = y;
		}
		// 블록의 아래쪽을 검사.
		for (int y = uy + 1; y < Block.BLOCK_NUM_Y; y++)
		{
			BlockControl next_block = this.blocks[start.i_pos.x, y];
			if (start.color == Block.COLOR.Obstacle)
			{
				break;
			}
			if (start.color == Block.COLOR.Wall)
			{
				break;
			}
			if (next_block.color != start.color)
			{
				break;
			}

			if (!next_block.isVanishing())
			{
				normal_block_num++;
			}
			uy = y;
		}
		do
		{
			if (uy - dy + 1 < 3)
			{
				break;
			}
			if (normal_block_num == 0)
			{
				break;
			}

			if (uy - dy + 1 >= 3)
			{
				for (int y = dy; y < uy + 1; y++)
				{

					this.blocks[start.i_pos.x, y].color = this.selectBlockColor();
					ret = true;
				}
			}
		} while (false);


		return (ret);
	}

	public void ComboTimer()
	{
		if (FeverTimer > 0)
		{
			FeverTimer -= Time.deltaTime;
			sltimer.value -= Time.deltaTime;
		}
		else
		{
			sltimer.gameObject.SetActive(false);
			this.score_counter.ComboCount(0, false);
		}
	}
	private void EnableInput()
	{
		isInputEnabled = true;
	}
	private void turnoffFE()
	{
		feverEffect.SetActive(false);
	}
}


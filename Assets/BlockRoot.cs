using UnityEngine;
using System.Collections;

public class BlockRoot : MonoBehaviour
{
	
	//public GameObject BombPrefab = null;//4��ġ ��ź ���
	public GameObject BlockPrefab = null; // ����� �� ����� ������.
	public BlockControl[,] blocks; // �׸���.

	private GameObject main_camera = null; // ���� ī�޶�.
	private BlockControl grabbed_block = null; // ���� ���.

	private ScoreCounter score_counter = null; // ScoreCounter.
	protected bool is_vanishing_prev = false; // ������ ��ȭ�߾��°�?.

	public TextAsset levelData = null; // ���� �������� �ؽ�Ʈ�� ����.
	public LevelControl level_control; // LevelControl�� ����.


	//class�����߰�
	private SceneControl stagenum;

	void Start()
	{
		this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
		this.score_counter = this.gameObject.GetComponent<ScoreCounter>();
		//start�κ� ���� �߰�
		stagenum = GameObject.Find("GameRoot").GetComponent<SceneControl>();
	}


	void Update()
	{
		Vector3 mouse_position; // ���콺 ��ġ.
		this.unprojectMousePosition( // ���콺 ��ġ�� ȹ��.
									out mouse_position, Input.mousePosition);
		// ȹ���� ���콺 ��ġ�� X�� Y������ �Ѵ�.
		Vector2 mouse_position_xy =
			new Vector2(mouse_position.x, mouse_position.y);
		if (this.grabbed_block == null)
		{ // ����� ���� �ʾ��� ��.
			if (!this.is_has_falling_block())
			{
				if (Input.GetMouseButtonDown(0))
				{ // ���콺 ��ư�� ���ȴٸ�.
				  // blocks �迭�� ��� ��Ҹ� ���ʷ� ó���Ѵ�.

					foreach (BlockControl block in this.blocks)
					{
						if (!block.isGrabbable())
						{ // ����� ���� �� ������.
							continue; // ���� �������.
						}
						// ���콺 ��ġ�� ��� ���� �ȿ� ������.
						if (!block.isContainedPosition(mouse_position_xy))
						{
							continue; // ���� �������.
						}
						Debug.Log(block.pop5Color);
						if (block.color == Block.COLOR.Bomb)//20230510 4��ġ��ź ��������
						{

							for (int x = 0; x < Block.BLOCK_NUM_X; x++)
							{
								if (blocks[x, block.i_pos.y].color != Block.COLOR.Wall)
								{
									this.blocks[x, block.i_pos.y].toVanishing();
								}
							}
							for (int y = block.i_pos.y + 1; y < Block.BLOCK_NUM_Y; y++)
							{
								if (blocks[block.i_pos.x, y].color != Block.COLOR.Wall)
									this.blocks[block.i_pos.x, y].toVanishing();

							}
							for (int y = 0; y < block.i_pos.y; y++)
							{
								if (blocks[block.i_pos.x, y].color != Block.COLOR.Wall)
									this.blocks[block.i_pos.x, y].toVanishing();
							}						
							this.score_counter.DiffBlockCount(1);//20230511����ī��Ʈ
						}
						else if(block.color == Block.COLOR.POP5)//20230510 5��ġ��ź ��������
                        {
							foreach (BlockControl block2 in this.blocks)
                            {
								if (block.pop5Color == block2.color)
								{
									block2.toVanishing();
								}
							}
							this.blocks[block.i_pos.x, block.i_pos.y].toVanishing();
							this.score_counter.DiffBlockCount(1);
						}
						else if (block.color == Block.COLOR.FeverItem)//20230511 �ǹ���� ������
                        {
							
							for(int i = -1; i < 2; i++)
                            {
								for (int j = -1; j < 2; j++)
                                {
									if (blocks[block.i_pos.x + i, block.i_pos.y + j].color != Block.COLOR.Wall)
										this.blocks[block.i_pos.x+i, block.i_pos.y+j].toVanishing();
								}
                            }
						}
						// ó�� ���� ����� grabbed_block�� ���.
						this.grabbed_block = block;
						// ����� ���� ó���� ����.
						if (this.grabbed_block.color == Block.COLOR.Obstacle)
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
		{ // ����� ��� ���� ��.


			do
			{
				// �����̵��� ���� ����� �����´�.
				BlockControl swap_target =
					this.getNextBlock(grabbed_block, grabbed_block.slide_dir);
				// �����̵��� �� ����� ��� �ִٸ�.
				if (swap_target == null)
				{
					break; // ���� Ż��. 
				}
				// �����̵��� �� ����� ���� �� �ִ� ���°� �ƴ϶��.
				if (!swap_target.isGrabbable())
				{
					break; // ���� Ż��. 
				}
				//  ���� ��ġ���� �����̵��� �������� �Ÿ��� ���Ѵ�.
				float offset = this.grabbed_block.calcDirOffset(
					mouse_position_xy, this.grabbed_block.slide_dir);
				// �̵� �Ÿ��� ��� ũ���� ���ݺ��� �۴ٸ� .
				if (offset < Block.COLLISION_SIZE / 2.0f)
				{
					break; // ���� Ż��. 
				}

				//if (!this.checkConnection(this.grabbed_block) || !this.checkConnection(swap_target))
				//{
				//	Debug.Log(grabbed_block);
				//	this.score_counter.ComboCount(0, false);
				//}

				// ����� ��ü�Ѵ�.
				this.swapBlock(
					grabbed_block, grabbed_block.slide_dir, swap_target);
				//Debug.Log(grabbed_block);


				//bool check ���� ����� 20230506
				bool checkcheck = true;
				int match_count1 = 1; //�ٲ����� ��ġ�� ���� �� 20230506

				//����� ��� �̵��� �Ͽ������ ��� ����� �˻���. 20230506
				foreach (BlockControl block in this.blocks)
				{

					if (this.checkConnection(block))			//3��ġ����� ��ġ�°��
					{
						this.score_counter.DiffBlockCount(match_count1); //��ġ�� ������ scorecounter��ũ��Ʈ �Լ��� ���� 20230504
						this.score_counter.ComboCount(0, true);		//�޺�ī��Ʈ ����
						checkcheck = false;     //check check�� false�� ����
						for(int i = 1; i < 9; i++)
                        {
							if (this.score_counter.last.combo == 7*i)
							{
								CreateFeverBlock();
							}
						}
                        
						break;
						
					}
					
				}

				if(checkcheck)		//checkcheck�� �״�� true�ϰ�� 3��ġ�� �����ʾҴٴ¶��̹Ƿ�
				{
					CreateInterruptBlock();
					//combo �ʱ�ȭ
					this.score_counter.ComboCount(0, false);
				}



				this.grabbed_block = null; // ������ ����� ��� ���� �ʴ�.



			} while (false);



			if (!Input.GetMouseButton(0))
			{ // ���콺 ��ư�� ������ ���� ������.
				this.grabbed_block.endGrab(); // ����� ������ ���� ó���� ����.
				this.grabbed_block = null; //  grabbed_block�� ��� ����.
				
			}
		}

		// ���� �� �Ǵ� �����̵� ���̸�.
		if (this.is_has_falling_block() || this.is_has_sliding_block())
		{
			// �ƹ��͵� ���� �ʴ´�.
			// ���� �ߵ� �����̵� �ߵ� �ƴϸ�.
		}
		else
		{
			//int combo_count = 0;// �޺�����Ʈ 20230504
			int match_count = 1; // ��ġ�� ���� �� 20230504
			int ignite_count = 0; // ��ȭ ��.
								  // �׸��� ���� ��� ��Ͽ� ���ؼ� ó��.
			foreach (BlockControl block in this.blocks)
			{
				if (!block.isIdle())
				{ // ��� ���̸� ������ ó������ �����ϰ�,.\
				  //Debug.Log(block);
					continue; // ���� ����� ó���Ѵ�.
				}
				// ���� �Ǵ� ���ο� ���� �� ����� �� �� �̻� �����ߴٸ�.
				if (this.checkConnection(block))
				{
					ignite_count++; // ��ȭ ���� ����.
									//combo_count++;  //20230504 �޺� ����            
									//Debug.Log(block);
					//this.score_counter.ComboCount(combo_count, true);
					this.score_counter.DiffBlockCount(match_count);//��ġ�� ������ scorecounter��ũ��Ʈ �Լ��� ���� 20230504
				}


			}
			if (ignite_count > 0)
			{ // ��ȭ ���� 0���� ũ��.

				if (!this.is_vanishing_prev)
				{
					// ������ ���Ⱑ �ƴ϶�� ��ȭ Ƚ�� ����.
					this.score_counter.clearIgniteCount();

				}
				// ��ȭ Ƚ���� �ø���.
				this.score_counter.addIgniteCount(ignite_count);
				// �հ� ���ھ� ����.
				this.score_counter.updateTotalScore();



				// ���� ������ ������ ���� ������.
				int block_count = 0; // ��ȭ ���� ��� ��(���� �忡�� ����Ѵ�).
									 // �׸��� ���� ��� ��Ͽ� ���ؼ� ó��.
				foreach (BlockControl block in this.blocks)
				{
					if (block.isVanishing())
					{ // ��ȭ�ߣ����� ������٣��̸�.
						block.rewindVanishTimer(); // ���ȭ��.
						block_count++; // ��ȭ ���� ����� ������ ����.
					}
				}
			}
			
		}

		// �ϳ��� ���� ���� ����� �ִ°�?.
		bool is_vanishing = this.is_has_vanishing_block();
		// ������ �����ϸ� ����� ����߸��� �ʹ�.
		do
		{
			if (is_vanishing)
			{ // ���� ���� ����� �ִٸ�.
				break; // ���� ó���� �������� �ʴ´�.
			}
			if (this.is_has_sliding_block())
			{ // ��ü ���� ����� �ִٸ�.
				break; // ���� ó���� �������� �ʴ´�.
			}
			for (int x = 0; x < Block.BLOCK_NUM_X; x++)
			{
				// ���� ��ü ���� ����� �ִٸ�, �� ���� ó������ �ʰ� ���� ���� �����Ѵ�.
				if (this.is_has_sliding_block_in_column(x))
				{
					continue;
				}
				// �� ���� �ִ� ����� ���������� �˻�.
				for (int y = 0; y < Block.BLOCK_NUM_Y - 1; y++)
				{
					// ���� ���� ����� ��ǥ�ö��, ���� �������.
					if (!this.blocks[x, y].isVacant())
					{
						continue;
					}
					// ���� ���� ��� �Ʒ��� �ִ� ����� �˻�.
					for (int y1 = y + 1; y1 < Block.BLOCK_NUM_Y; y1++)
					{
						// �Ʒ��� �ִ� ����� ��ǥ�ö��, ���� �������.
						if (this.blocks[x, y1].isVacant())
						{
							continue;
						}
						//  ����� ��ü�Ѵ�.
						this.fallBlock(this.blocks[x, y], Block.DIR4.UP,
									   this.blocks[x, y1]);
						break;
					}
				}
			}
			// ����ó��.
			for (int x = 0; x < Block.BLOCK_NUM_X; x++)
			{
				int fall_start_y = Block.BLOCK_NUM_Y;
				for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
				{
					// ��ǥ�� ����� �ƴ϶�� ���� �������.
					if (!this.blocks[x, y].isVacant())
					{
						continue;
					}
					this.blocks[x, y].beginRespawn(fall_start_y); // ��� ��Ȱ.
					fall_start_y++;
				}
			}
		} while (false);
		CheckWall();
		SetWall();
		this.is_vanishing_prev = is_vanishing;
	}






	// ����� ����� ����, ���� ��ȩ ĭ ���� ��ȩ ĭ���� ��ġ.
	public void initialSetUp()
	{
		// ũ��� 9��9�� �Ѵ�.
		this.blocks =
			new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y];
		// ����� �� ��ȣ.
		int color_index = 0;

		Block.COLOR color = Block.COLOR.FIRST;


		for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
		{ // ó������� ��������� �����������.
			for (int x = 0; x < Block.BLOCK_NUM_X; x++)
			{// ���� ���������� ������ ������.
			 // BlockPrefab�� �ν��Ͻ��� �� ���� �����.
				GameObject game_object =
					Instantiate(this.BlockPrefab) as GameObject;
				// ������ ���� ����� BlockControl Ŭ������ �����´�.
				BlockControl block = game_object.GetComponent<BlockControl>();
				// ����� ĭ�� �ִ´�.
				this.blocks[x, y] = block;
				// ����� ��ġ ����(�׸��� ��ǥ)�� ����.
				block.i_pos.x = x;
				block.i_pos.y = y;
				// �� BlockControl�� �����ϴ� GameRoot�� �ڽ��̶�� ����.
				block.block_root = this;
				// �׸��� ��ǥ�� ���� ��ġ(�� ��ǥ)�� ��ȯ.
				Vector3 position = BlockRoot.calcBlockPosition(block.i_pos);
				// �� ���� ��� ��ġ�� �̵�.
				block.transform.position = position;

				// ����� ���� ����. 
				// block.setColor((Block.COLOR)color_index);
				// ������ ���� Ȯ���� �������� ���� �����Ѵ�.
				color = this.selectBlockColor();
				block.setColor(color);

				// ����� �̸��� ����(�ļ�).
				block.name = "block(" + block.i_pos.x.ToString() +
					"," + block.i_pos.y.ToString() + ")";
				// ��� ������ �� �߿��� ���Ƿ� �� ���� ����.
				color_index =
					Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
			}
		}
	}


	// ������ �׸��� ��ǥ���� �� ���� ��ǥ�� ���Ѵ�. 
	public static Vector3 calcBlockPosition(Block.iPosition i_pos)
	{
		// ��ġ�� ���� ��� ������ ��ġ�� �ʱ갪���� ����.
		Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f),
									   -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);
		// �ʱ갪���׸��� ��ǥ �� ��� ũ��.
		position.x += (float)i_pos.x * Block.COLLISION_SIZE;
		position.y += (float)i_pos.y * Block.COLLISION_SIZE;
		return (position); // ���� ��ǥ�� ��ȯ�Ѵ�.
	}


	public bool unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position)
	{
		bool ret;
		// ���� ����. �� ���� ī�޶󿡼� ���̴� ���� ��.
		// ����� ���� ũ�⸸ŭ ������ ���δ�.
		Plane plane = new Plane(Vector3.back, new Vector3(
			0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));
		// ī�޶�� ���콺�� ����ϴ� ������ ����.
		Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(
			mouse_position);
		float depth;
		// ���� ray�� �� plane�� ��Ҵٸ�.
		if (plane.Raycast(ray, out depth))
		{
			// �μ� world_position�� ���콺 ��ġ�� �����.
			world_position = ray.origin + ray.direction * depth;
			ret = true;
			// ���� �ʾҴٸ�.
		}
		else
		{
			// �μ� world_position�� ������ ���ͷ� �����.
			world_position = Vector3.zero;
			ret = false;
		}
		return (ret);
	}




	public BlockControl getNextBlock(
		BlockControl block, Block.DIR4 dir)
	{
		// �����̵��� ���� ����� ���⿡ ����.
		BlockControl next_block = null;
		switch (dir)
		{
			case Block.DIR4.RIGHT:
				if (block.i_pos.x < Block.BLOCK_NUM_X - 1)
				{
					// �׸��� ���̶��.
					next_block = this.blocks[block.i_pos.x + 1, block.i_pos.y];
				}
				break;

			case Block.DIR4.LEFT:
				if (block.i_pos.x > 0)
				{ // �׸��� ���̶��.
					next_block = this.blocks[block.i_pos.x - 1, block.i_pos.y];
				}
				break;
			case Block.DIR4.UP:
				if (block.i_pos.y < Block.BLOCK_NUM_Y - 1)
				{ // �׸��� ���̶��.
					next_block = this.blocks[block.i_pos.x, block.i_pos.y + 1];
				}
				break;
			case Block.DIR4.DOWN:
				if (block.i_pos.y > 0)
				{ // �׸��� ���̶��.
					next_block = this.blocks[block.i_pos.x, block.i_pos.y - 1];
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
			case Block.DIR4.RIGHT: v = Vector3.right; break; // ���������� 1���� �̵��Ѵ�.
			case Block.DIR4.LEFT: v = Vector3.left; break; // �������� 1���� �̵��Ѵ�.
			case Block.DIR4.UP: v = Vector3.up; break; // ���� 1���� �̵��Ѵ�.
			case Block.DIR4.DOWN: v = Vector3.down; break; // �Ʒ��� 1���� �̵��Ѵ�.
		}
		v *= Block.COLLISION_SIZE; // ��� ũ�⸦ ���Ѵ�.
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
		// �� ����� ���� ����� �д�.
		Block.COLOR color0 = block0.color;
		Block.COLOR color1 = block1.color;
		// �� �����.
		// Ȯ������ ����� �д�.
		Vector3 scale0 =
			block0.transform.localScale;
		Vector3 scale1 =
			block1.transform.localScale;
		//  �� ����� '������� �ð�'�� ����� �д�.
		float vanish_timer0 = block0.vanish_timer;
		float vanish_timer1 = block1.vanish_timer;
		// �� ����� �̵��� ���� ���Ѵ�.
		Vector3 offset0 = BlockRoot.getDirVector(dir);
		Vector3 offset1 = BlockRoot.getDirVector(BlockRoot.getOppositDir(dir));
		block0.setColor(color1); //  ���� ��ü�Ѵ�.
		block1.setColor(color0);
		block0.transform.localScale = scale1; // Ȯ������ ��ü�Ѵ�.
		block1.transform.localScale = scale0;
		block0.vanish_timer = vanish_timer1; // ������� �ð��� ��ü�Ѵ�.
		block1.vanish_timer = vanish_timer0;
		block0.beginSlide(offset0); // ���� ����� �̵��� ����.
		block1.beginSlide(offset1); // �̵��� ���� ��� �̵��� ����.
	}

	
	public bool checkConnection(BlockControl start)  
	{
		bool ret = false;
		int normal_block_num = 0;
		
		// �μ��� ����� ��ȭ �İ� �ƴϸ�.
		if (!start.isVanishing())
		{
			normal_block_num = 1;
		}
		// �׸��� ��ǥ�� ����� �д�.
		int rx = start.i_pos.x;
		int lx = start.i_pos.x;
		// ����� ������ �˻�.
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
			{ // ���� �ٸ���.
				break; // ���� Ż��.
			}
			if (next_block.step == Block.STEP.FALL || // ���� ���̸�.
			   next_block.next_step == Block.STEP.FALL)
			{
				break; // ���� Ż��.
			}
			if (next_block.step == Block.STEP.SLIDE || // �����̵� ���̸�.
			   next_block.next_step == Block.STEP.SLIDE)
			{
				break; // ���� Ż��.
			}
			if (!next_block.isVanishing())
			{ // ��ȭ ���� �ƴϸ�.
				normal_block_num++; // �˻�� ī���͸� ����.
			}
			lx = x;
		}
		// ����� �������� �˻�.
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
			// ������ ����� �׸��� ��ȣ - ���� ����� �׸��� ��ȣ +.
			// �߾� ���(1)�� ���� ���� 3�̸� �̸�.
			if (rx - lx + 1 < 3)
			{
				break; // ���� Ż��.
			}
			
			if (normal_block_num == 0)
			{ // ��ȭ ���� �ƴ� ����� �ϳ��� ������.
				break; // ���� Ż��.
			}
			if (rx - lx + 1 == 4)//20230510 4��ġ �϶�  ������ ��� ���� �� �Ͷ߸��� for��
			{
				for (int x = lx; x < start.i_pos.x; x++)
				{
					// ������ ���� �� ����� ��ȭ ���·�.
					this.blocks[x, start.i_pos.y].toVanishing();
					ret = true;
				}
				for (int x = start.i_pos.x+1; x < rx + 1; x++)
				{
					// ������ ���� �� ����� ��ȭ ���·�.
					this.blocks[x, start.i_pos.y].toVanishing();
					ret = true;
				}
				this.blocks[start.i_pos.x, start.i_pos.y].color = Block.COLOR.Bomb;
				//this.blocks[start.i_pos.y, start.i_pos.y].step = Block.STEP.FALL;
			}
			else if (rx - lx + 1 == 5)//20230510 5��ġ �϶�  ������ ��� ���� �� �Ͷ߸��� for��
            {
				Debug.Log("5��ġ ����");
				for (int x = lx; x < start.i_pos.x; x++)
				{
					// ������ ���� �� ����� ��ȭ ���·�.
					this.blocks[x, start.i_pos.y].toVanishing();
					ret = true;
				}
				for (int x = start.i_pos.x + 1; x < rx + 1; x++)
				{
					// ������ ���� �� ����� ��ȭ ���·�.
					this.blocks[x, start.i_pos.y].toVanishing();
					ret = true;
				}
				this.blocks[start.i_pos.x, start.i_pos.y].pop5Color = this.blocks[start.i_pos.x, start.i_pos.y].color;//pop5color�� ���� ����
				this.blocks[start.i_pos.x, start.i_pos.y].color = Block.COLOR.POP5;
				//Debug.Log(this.blocks[start.i_pos.x, start.i_pos.y].pop5Color);
				//this.blocks[start.i_pos.y, start.i_pos.y].step = Block.STEP.FALL;
			}
			else
			{
				for (int x = lx; x < rx + 1; x++)
				{
					// ������ ���� �� ����� ��ȭ ���·�.
					this.blocks[x, start.i_pos.y].toVanishing();
					//��ó�� ���ع��� �ִ��� �˻�
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
		// ����� ������ �˻�.
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
		// ����� �Ʒ����� �˻�.
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
				
				for (int y = dy; y < start.i_pos.y; y++) //20230510 4��ġ �϶�  ������ ��� ���� �� �Ͷ߸��� for��
				{
					//�غκ� ��ȭ
					this.blocks[start.i_pos.x, y].toVanishing();
					ret = true;
				}
				for (int y = start.i_pos.y + 1; y < uy + 1; y++)
				{
					// ���κ� ��ȭ
					this.blocks[start.i_pos.x, y].toVanishing();
					ret = true;
				}
				
				this.blocks[start.i_pos.x, start.i_pos.y].color = Block.COLOR.Bomb;
				this.blocks[start.i_pos.y, start.i_pos.y].step = Block.STEP.FALL;
			}
			else if(uy - dy + 1 == 5)
			{
				//Block.COLOR co = this.blocks[start.i_pos.x, start.i_pos.y].color;
				this.blocks[start.i_pos.x, start.i_pos.y-2].pop5Color = start.color;
				start.color = Block.COLOR.POP5;
				//this.blocks[start.i_pos.y, start.i_pos.y].step = Block.STEP.FALL;
				Debug.Log(start.pop5Color);
				for (int y = dy; y < start.i_pos.y; y++) //20230510 5��ġ �϶�  ������ ��� ���� �� �Ͷ߸��� for��
				{
					//�غκ� ��ȭ
					this.blocks[start.i_pos.x, y].toVanishing();
					ret = true;
				}
				for (int y = start.i_pos.y + 1; y < uy + 1; y++)
				{
					// ���κ� ��ȭ
					this.blocks[start.i_pos.x, y].toVanishing();
					ret = true;
				}
				
			}
			else
			{
				for (int y = dy; y < uy + 1; y++)
				{
					//��ó�� ���ع��� �ִ��� �˻�
					DestroyInterruptBlock(blocks[start.i_pos.x, y]);
					this.blocks[start.i_pos.x, y].toVanishing();
					ret = true;
				}
			}
		} while (false);

		
		return (ret);
	}


	//�Һٴ����� ����� �ϳ��� ������ true ��ȯ
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

	//�����̵� ���� ����� �ϳ��� ������ true��ȯ
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
	//�������� ����� �ϳ��� ������ true ��ȯ
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
		// block0�� block1�� ��, ũ��, ����� ������ �ɸ��� �ð�, ǥ��, ��ǥ��, ���¸� ���.
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
		// block0�� block1�� ���� �Ӽ��� ��ü�Ѵ�.
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
			{ // �����̵� ���� ����� ������.
				ret = true; // true�� ��ȯ�Ѵ�. 
				break;
			}
		}
		return (ret);
	}



	public void create()
	{
		this.level_control = new LevelControl();
		this.level_control.initialize(); // ���� ������ �ʱ�ȭ.
		this.level_control.loadLevelData(this.levelData); // ������ �б�.
		this.level_control.selectLevel(); // ���� ����.
	}
	public Block.COLOR selectBlockColor()
	{
		Block.COLOR color = Block.COLOR.FIRST;
		// �̹� ������ ���� �����͸� �����´�.
		LevelData level_data =
			this.level_control.getCurrentLevelData();
		float rand = Random.Range(0.0f, 1.0f); // 0.0~1.0 ������ ����.
		float sum = 0.0f; // ���� Ȯ���� �հ�.
		int i = 0;
		// ����� ���� ��ü�� ó���ϴ� ����.
		for (i = 0; i < level_data.probability.Length - 1; i++)
		{
			if (level_data.probability[i] == 0.0f)
			{
				continue; // ���� Ȯ���� 0�̸� ������ ó������ ����.
			}
			sum += level_data.probability[i]; // ���� Ȯ���� ���Ѵ�.
			if (rand < sum)
			{ // �հ谡 �������� ������.
				break; // ������ �������´�.
			}
		}
		color = (Block.COLOR)i; // i��° ���� ��ȯ�Ѵ�.
		return (color);
	}


	//Range�����ȿ��� ���� ���� ��ֹ��� �ٲٴ��Լ�
	public void CreateInterruptBlock()
	{
		int x = Random.Range(0, Block.BLOCK_NUM_X);
		int y = Random.Range(0, Block.BLOCK_NUM_Y);
		if (blocks[x, y].color != Block.COLOR.Wall)
		{
			blocks[x, y].setColor(Block.COLOR.Obstacle);
		}
		else
		{
			Debug.Log("check");
			CreateInterruptBlock();
		}
	}
	//Range�����ȿ��� ���� ���� �ǹ������� �ٲٴ��Լ�//20230511
	public void CreateFeverBlock()
	{
		for (int i = 0; i < 3; i++)//���⿡ if�� �־ �� 2,3�������� ������� �����ϸ� �ɵ�?
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
	//BlockRoot���� ��ֹ��� ����� �Լ�
	public void DestroyInterruptBlock(BlockControl block)
	{
		if (block.i_pos.x + 1 < Block.BLOCK_NUM_X && blocks[block.i_pos.x + 1, block.i_pos.y].color == Block.COLOR.Obstacle)
		{
			blocks[block.i_pos.x + 1, block.i_pos.y].toVanishing();
		}
		if (block.i_pos.x - 1 >= 0 && blocks[block.i_pos.x - 1, block.i_pos.y].color == Block.COLOR.Obstacle)
		{
			blocks[block.i_pos.x - 1, block.i_pos.y].toVanishing();
		}
		if (block.i_pos.y + 1 < Block.BLOCK_NUM_X && blocks[block.i_pos.x, block.i_pos.y + 1].color == Block.COLOR.Obstacle)
		{
			blocks[block.i_pos.x, block.i_pos.y + 1].toVanishing();
		}
		if (block.i_pos.y - 1 >= 0 && blocks[block.i_pos.x, block.i_pos.y - 1].color == Block.COLOR.Obstacle)
		{
			blocks[block.i_pos.x, block.i_pos.y - 1].toVanishing();
		}
	}
	public void SetWall()
	{
		for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
		{ // ó������� ��������� �����������.
			for (int x = 0; x < Block.BLOCK_NUM_X; x++)
			{
				if (stagenum.NowStage() == 2)
				{
					//���� ������ �Ʒ�
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

					//���� ���� �Ʒ�
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


					////���� ������ ��
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

					////���� ���� �Ʒ�
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
			}
		}
	}

	public void CheckWall()
	{
		for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
		{ // ó������� ��������� �����������.
			for (int x = 0; x < Block.BLOCK_NUM_X; x++)
			{
				if (this.blocks[x, y].color == Block.COLOR.Wall)
				{
					int color_index = Random.Range(
					(int)Block.COLOR.FIRST, (int)Block.COLOR.LAST + 1);
					this.blocks[x, y].setColor((Block.COLOR)color_index);
				}
			}
		}
	}


}


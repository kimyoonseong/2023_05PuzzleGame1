using UnityEngine;
using System.Collections;


// ��Ͽ� ���õ� ������ �ٷ��.
public class Block
{
	public static float COLLISION_SIZE = 1.0f; // ����� �浹 ũ��.
	public static float VANISH_TIME = 1.0f; // ��ȭ�ϰ� ������� �ð�.		//���� 3������ 1�� ����
	public struct iPosition
	{ // �׸��忡���� ��ǥ�� ��Ÿ���� ����ü.
		public int x; // X��ǥ.
		public int y; // Y��ǥ.
	}

	public enum COLOR
	{ // ��� ����.
		NONE = -1, // �� ���� ����.
		PINK = 0, // ��ȫ��.
		BLUE, // �Ķ���.
		YELLOW, // �����.
		GREEN, // ���.
		MAGENTA, // ����Ÿ.
		ORANGE, // ������.
		GRAY, // ȸ��.

		NUM, // ������ �� �������� ��Ÿ����(=7).
		//FIRST = PINK, // �ʱ� ����(��ȫ��).
		LAST = ORANGE, // ������ ����(������).
		NORMAL_COLOR_NUM = GRAY, // �Ϲ� ����(�׷��� �̿� ��)�� ��.
		Bomb,//4��ġ ��ź, ������
		POP5,//5��ġ ��ź, ���
		Obstacle,//��ֹ�
		FeverItem,// �ǹ�Ÿ�� ��ź
		Wall	//����
	};

	public enum DIR4
	{ // �����¿� �� ����.
		NONE = -1, // ���� ���� ����.
		RIGHT, // ������.
		LEFT, // ����.
		UP, // ��.
		DOWN, // �Ʒ�.
		NUM, // ������ �� �������� ��Ÿ����(=4).
	};

	public enum STEP
	{ // ����� ���� ǥ��.
		NONE = -1, // ���� ���� ����.
		IDLE = 0, // ��� ��.
		GRABBED, // �����ִ�.
		RELEASED, // ���� ����.
		SLIDE, // �����̵� ��.
		VACANT, // �Ҹ� ��.
		RESPAWN, // ����� ��.
		FALL, // ���� ��.
		LONG_SLIDE, // ��� �����̵� �ϰ� �ִ�.
		NUM, // ���°� �� �������� ��Ÿ����(=8).
	};


	public static int BLOCK_NUM_X = 9; // ����� ��ġ�� �� �ִ� X ���� �ִ�.
	public static int BLOCK_NUM_Y = 9; // ����� ��ġ�� �� �ִ� Y ���� �ִ�.
}



public class BlockControl : MonoBehaviour
{
	public GameObject Bomb; //20230521 4��ġ ��ź 
	public GameObject Match5; //20230521 4��ġ ��ź
	public GameObject FeverBomb; //20230521 4��ġ ��ź
	public GameObject Obstacle; //20230521 4��ġ ��ź
								

	public Block.COLOR color = (Block.COLOR)0; // ��� ��.
	public Block.COLOR pop5Color = (Block.COLOR)(-1); //20230510 5��ġ ��ϻ����� Ŭ����.
	
	public BlockRoot block_root = null; // ����� ��.
	public Block.iPosition i_pos; // ��� ��ǥ.

	public Block.STEP step = Block.STEP.NONE; // ���� ����.
	public Block.STEP next_step = Block.STEP.NONE; // ���� ����.
	private Vector3 position_offset_initial = Vector3.zero; // ��ü �� ��ġ.
	public Vector3 position_offset = Vector3.zero; // ��ü �� ��ġ.


	public float vanish_timer = -1.0f; // ����� ����� �������� �ð�.
	public Block.DIR4 slide_dir = Block.DIR4.NONE; // �����̵�� ����.
	public float step_timer = 1.0f; // ����� ��ü�� ���� �̵� �ð� ��.

	// 10-------.
	public Material opaque_material; // ������� ����.
	public Material transparent_material; // ������� ����.


	private struct StepFall
	{
		public float velocity; // ���ϼӵ�.
	}
	private StepFall fall;



	void Start()
	{
		this.transform.localScale = new Vector3(0.5f, 0.5f, 0.4f);
		this.setColor(this.color); // ���� ĥ�Ѵ�.

		this.next_step = Block.STEP.IDLE; // ���� ����� ��� ������.
	}

	void Update()
	{
		Vector3 mouse_position; // ���콺 ��ġ.
		this.block_root.unprojectMousePosition( // ���콺 ��ġ ��������.
											   out mouse_position, Input.mousePosition);
		// ������ ���콺 ��ġ�� X�� Y������ �Ѵ�.
		Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y);


		if (this.vanish_timer >= 0.0f)
		{ // Ÿ�̸Ӱ� 0�̻��̸�.
			this.vanish_timer -= Time.deltaTime; // Ÿ�̸��� ���� ���δ�.
			if (this.vanish_timer < 0.0f)
			{ // Ÿ�̸Ӱ� 0�̸��̸�.
				if (this.step != Block.STEP.SLIDE)
				{ // �����̵� ���� �ƴϹǷ�.
					this.vanish_timer = -1.0f;
					this.next_step = Block.STEP.VACANT; // ���¸� '�Ҹ� ��'����.
				}
				else//�����̵����̸�
				{
					this.vanish_timer = 0.0f;
				}
			}
		}


		this.step_timer += Time.deltaTime;
		float slide_time = 0.2f;

		if (this.next_step == Block.STEP.NONE)
		{ // ���� ������ ���� ����.
			switch (this.step)
			{
				case Block.STEP.SLIDE:
					if (this.step_timer >= slide_time)
					{
						// �����̵� �߿� ����� �Ҹ��� �Ҹ��ϸ�.
						// VACANT(�������)���·� ��ȯ.
						if (this.vanish_timer == 0.0f)
						{
							this.next_step = Block.STEP.VACANT;
							// vanish_timer�� 0�� �ƴϸ�.
							// IDLE(���)���·� ��ȯ.
						}
						else
						{
							this.next_step = Block.STEP.IDLE;
						}
					}
					break;

				case Block.STEP.IDLE:
					this.GetComponent<Renderer>().enabled = true;
					break;
				case Block.STEP.FALL:
					if (this.position_offset.y <= 0.0f)
					{
						this.next_step = Block.STEP.IDLE;
						this.position_offset.y = 0.0f;
					}
					break;

			}
		}



		// '���� ���'�� ���°� '���� ����' �̿��� ����.
		// = '���� ���'�� ���°� ����� ���.
		while (this.next_step != Block.STEP.NONE)
		{
			this.step = this.next_step;
			this.next_step = Block.STEP.NONE;
			switch (this.step)
			{
				case Block.STEP.IDLE: // '���' ����.
					this.position_offset = Vector3.zero;
					// ����� ǥ�� ũ�⸦ �Ϲ� ũ��� �Ѵ�.
					this.transform.localScale = Vector3.one * 1.0f;
					break;
				case Block.STEP.GRABBED: // '���� ����'.
										 // ��� ǥ�� ũ�⸦ ũ�� �Ѵ�.
					this.transform.localScale = Vector3.one * 1.2f;
					break;
				case Block.STEP.RELEASED: // '���� ����'.
					this.position_offset = Vector3.zero;
					// ����� ǥ�� ũ�⸦ �Ϲ� ũ��� �Ѵ�.
					this.transform.localScale = Vector3.one * 1.0f;
					break;

				case Block.STEP.VACANT:
					this.position_offset = Vector3.zero;
					this.setVisible(false); // ����� ��ǥ�÷�.
					break;

				case Block.STEP.RESPAWN:
					// ���� �����ϰ� �����Ͽ� ����� �� ������ ����.
					int color_index = Random.Range(
						0, (int)Block.COLOR.NORMAL_COLOR_NUM);
					this.setColor((Block.COLOR)color_index);
					this.next_step = Block.STEP.IDLE;
					break;
				case Block.STEP.FALL:
					this.setVisible(true); // ����� ǥ��.
					this.fall.velocity = 0.0f; // ���� �ӵ��� ����.
					break;
			}
			this.step_timer = 0.0f;
		}


		switch (this.step)
		{
			case Block.STEP.GRABBED: //  '���� ����'.
									 // '���� ����'�� ���� �׻� �����̵� ������ üũ.
				this.slide_dir = this.calcSlideDir(mouse_position_xy);
				break;
			case Block.STEP.SLIDE: // �����̵�(��ü) ��.
								   // ����� ������ �̵��ϴ� ó��.
								   // (�����Ƿ� ������ ���� ������).
				float rate = this.step_timer / slide_time;
				rate = Mathf.Min(rate, 1.0f);
				rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
				this.position_offset = Vector3.Lerp(
					this.position_offset_initial, Vector3.zero, rate);
				break;
			case Block.STEP.FALL:
				// �ӵ��� �߷��� ������ �ش�.
				this.fall.velocity += Physics.gravity.y * Time.deltaTime * 1.5f;
				// ���� ���� ��ġ�� ���.
				this.position_offset.y += this.fall.velocity * Time.deltaTime;
				if (this.position_offset.y < 0.0f)
				{ // �� �����Դٸ�.
					this.position_offset.y = 0.0f; // �� �ڸ��� �ӹ���.
				}
				break;
		}



		// �׸��� ��ǥ�� ���� ��ǥ(���� ��ǥ)�� ��ȯ�ϰ�.
		// position_offset�� ���Ѵ�.
		Vector3 position =
			BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset;
		// ���� ��ġ�� ���ο� ��ġ�� ����.
		this.transform.position = position;


		this.setColor(this.color);
		if (this.vanish_timer >= 0.0f)
		{
			// ���� ������ ���ҽð����� ����.
			float vanish_time =
				this.block_root.level_control.getVanishTime();


			Color color0 = // ���� ���� ����� �߰���.
				Color.Lerp(this.GetComponent<Renderer>().material.color, Color.white, 0.5f);
			Color color1 = // ���� ���� �������� �߰���.
				Color.Lerp(this.GetComponent<Renderer>().material.color, Color.black, 0.5f);
			// ��ȭ ���� �ð��� ������ �����ٸ�.
			if (this.vanish_timer < Block.VANISH_TIME / 2.0f)
			{
				// ����(a)�� ����.
				color0.a = this.vanish_timer / (Block.VANISH_TIME / 2.0f);
				color1.a = color0.a;
				//  ������ ��Ƽ������ ����. 
				this.GetComponent<Renderer>().material = this.transparent_material;
			}
			// vanish_timer�� �پ�� ���� 1�� ���������.
			float rate = 1.0f - this.vanish_timer / Block.VANISH_TIME;
			// ������ ���� �ٲ۴�.
			this.GetComponent<Renderer>().material.color = Color.Lerp(color0, color1, rate);
		}

	}


	// �μ� color�� ������ ����� ĥ�Ѵ�.
	public void setColor(Block.COLOR color)
	{
		this.color = color; // ���� ������ ���� ��� ������ ����.
		Color color_value; // Color Ŭ������ ���� ��Ÿ����.
		switch (this.color)
		{ // ĥ�� ���� ���� �б��Ѵ�.
			default:
			case Block.COLOR.PINK:
				//block[0].GetComponent<Renderer>().material.SetFloat("Color", 1);
				color_value = new Color(1.0f, 0.5f, 0.5f);

				Obstacle.SetActive(false);
				Bomb.SetActive(false);
				Match5.SetActive(false);
				FeverBomb.SetActive(false);
				break;
			case Block.COLOR.BLUE:
				
				color_value = Color.blue;
				Obstacle.SetActive(false);
				Bomb.SetActive(false);
				Match5.SetActive(false);
				FeverBomb.SetActive(false);
				break;
			case Block.COLOR.YELLOW:
				color_value = Color.yellow;
				Obstacle.SetActive(false);
				Bomb.SetActive(false);
				Match5.SetActive(false);
				FeverBomb.SetActive(false);
				break;
			case Block.COLOR.GREEN:
				color_value = Color.green;
				Obstacle.SetActive(false);
				Bomb.SetActive(false);
				Match5.SetActive(false);
				FeverBomb.SetActive(false);
				break;
			case Block.COLOR.MAGENTA:
				color_value = Color.magenta;

				Obstacle.SetActive(false);
				Bomb.SetActive(false);
				Match5.SetActive(false);
				FeverBomb.SetActive(false);
				break;
			case Block.COLOR.ORANGE:
				color_value = new Color(1.0f, 0.46f, 0.0f);


				Obstacle.SetActive(false);
				Bomb.SetActive(false);
				Match5.SetActive(false);
				FeverBomb.SetActive(false);
				break;
			case Block.COLOR.Bomb://2023 0510 bomb �÷� �߰�

				GetComponent<MeshRenderer>().enabled = false;
				
				Bomb.SetActive(true);
				Match5.SetActive(false);
				FeverBomb.SetActive(false);
				Obstacle.SetActive(false);
				color_value = Color.red;
				break;
			case Block.COLOR.POP5://2023 0510 bomb �÷� �߰�
				GetComponent<MeshRenderer>().enabled = false;
				color_value = Color.blue;
				Bomb.SetActive(false);
				Match5.SetActive(true);
				FeverBomb.SetActive(false);
				Obstacle.SetActive(false);
				break;
			case Block.COLOR.Obstacle://2023 0510 bomb �÷� �߰�
				GetComponent<MeshRenderer>().enabled = false;
				color_value = Color.black;
				Obstacle.SetActive(true);
				Bomb.SetActive(false);
				FeverBomb.SetActive(false);
				Match5.SetActive(false);
				break;
			case Block.COLOR.FeverItem://20230511 FEVER BOMB�߰�
				GetComponent<MeshRenderer>().enabled = false;
				Bomb.SetActive(false);
				Match5.SetActive(false);
				FeverBomb.SetActive(true);
				Obstacle.SetActive(false);
				color_value = new Color(0.7f, 0.3f, 1.0f);
				break;
			case Block.COLOR.Wall://20230511 ���� �߰�
				color_value = new Color(1f, 1f, 1.0f, 0);
				break;
		}
		// �� GameObject�� ��Ƽ���� ������ ����.
		this.GetComponent<Renderer>().material.color = color_value;
	}


	public void beginGrab()
	{
		this.next_step = Block.STEP.GRABBED;
	}

	public void endGrab()
	{
		this.next_step = Block.STEP.IDLE;
	}

	public bool isGrabbable()
	{
		bool is_grabbable = false;
		switch (this.step)
		{
			case Block.STEP.IDLE: // ����⡹������ ����.
				is_grabbable = true; // true������ �� �ִ٣��� ��ȯ�Ѵ�.
				break;
		}
		return (is_grabbable);
	}

	public bool isContainedPosition(Vector2 position)
	{
		bool ret = false;
		Vector3 center = this.transform.position;
		float h = Block.COLLISION_SIZE / 2.0f;
		do
		{
			// X��ǥ�� �ڽſ��� �������� �ʴٸ� break�� ������ �������´�.
			if (position.x < center.x - h || center.x + h < position.x)
			{
				break;
			}
			// Y��ǥ�� �ڽſ��� �������� �ʴٸ� break�� ������ �������´�.
			if (position.y < center.y - h || center.y + h < position.y)
			{
				break;
			}
			// X��ǥ, Y��ǥ ������ �����ִٸ� true�� ��ȯ�Ѵ�.
			ret = true;
		} while (false);
		return (ret);
	}


	public Block.DIR4 calcSlideDir(Vector2 mouse_position)
	{
		Block.DIR4 dir = Block.DIR4.NONE;
		// ������ mouse_position�� ���� ��ġ�� ���� ��Ÿ���� ����.
		Vector2 v = mouse_position -
			new Vector2(this.transform.position.x, this.transform.position.y);
		// ������ ũ�Ⱑ 0.1���� ũ��.
		// (�׺��� ������ �����̵����� ���� �ɷ� �����Ѵ�).
		if (v.magnitude > 0.1f)
		{
			if (v.y > v.x)
			{
				if (v.y > -v.x)
				{
					dir = Block.DIR4.UP;
				}
				else
				{
					dir = Block.DIR4.LEFT;
				}
			}
			else
			{
				if (v.y > -v.x)
				{
					dir = Block.DIR4.RIGHT;
				}
				else
				{
					dir = Block.DIR4.DOWN;
				}
			}
		}
		return (dir);
	}

	public float calcDirOffset(Vector2 position, Block.DIR4 dir)
	{
		float offset = 0.0f;
		// ������ ��ġ�� ����� ���� ��ġ�� ���̸� ��Ÿ���� ����.
		Vector2 v = position - new Vector2(
			this.transform.position.x, this.transform.position.y);
		switch (dir)
		{ // ������ ���⿡ ���� �б�.
			case Block.DIR4.RIGHT:
				offset = v.x;
				break;
			case Block.DIR4.LEFT:
				offset = -v.x;
				break;
			case Block.DIR4.UP:
				offset = v.y;
				break;
			case Block.DIR4.DOWN:
				offset = -v.y;
				break;
		}
		return (offset);
	}

	public void beginSlide(Vector3 offset)
	{
		this.position_offset_initial = offset;
		this.position_offset =
			this.position_offset_initial;
		// ���¸� SLIDE�� ����.
		this.next_step = Block.STEP.SLIDE;
	}


	public void toVanishing()
	{
		// ����� ������ �ɸ��� �ð��� ����ġ�� ����.
		// this.vanish_timer = Block.VANISH_TIME;
		// ���� ������ ���ҽð����� ����.
		float vanish_time = this.block_root.level_control.getVanishTime();
		this.vanish_timer = vanish_time;
	}

	public bool isVanishing()
	{
		// vanish_timer�� 0���� ũ�� true.
		bool is_vanishing = (this.vanish_timer > 0.0f);
		return (is_vanishing);
	}

	public void rewindVanishTimer()
	{
		// ����� ������ �ɸ��� �ð��� ����ġ�� ����.
		// this.vanish_timer = Block.VANISH_TIME;
		// ���� ������ ���ҽð����� ����.
		float vanish_time = this.block_root.level_control.getVanishTime();
		this.vanish_timer = vanish_time;
	}

	public bool isVisible()
	{
		// �׸��� ����(renderer.enabled�� true)�̶��.
		// ǥ�õǰ� �ִ�. 
		bool is_visible = this.GetComponent<Renderer>().enabled;
		return (is_visible);
	}

	public void setVisible(bool is_visible)
	{
		// �׸��� ���� ������ �μ��� �����Ѵ�.
		this.GetComponent<Renderer>().enabled = is_visible;
	}

	public bool isIdle()
	{
		bool is_idle = false;
		// ���� ��� ���°� '��� ��'�̰�.
		// ���� ��� ���°� '����'�̸�.
		if (this.step == Block.STEP.IDLE &&
		   this.next_step == Block.STEP.NONE)
		{
			is_idle = true;
		}
		return (is_idle);
	}


	public void beginFall(BlockControl start)
	{
		this.next_step = Block.STEP.FALL;
		// ������ ��Ͽ��� ��ǥ�� ����� ����.
		this.position_offset.y =
			(float)(start.i_pos.y - this.i_pos.y) * Block.COLLISION_SIZE;
	}

	public void beginRespawn(int start_ipos_y)
	{
		// ���� ��ġ���� y��ǥ�� �̵�.
		this.position_offset.y =
			(float)(start_ipos_y - this.i_pos.y) *
				Block.COLLISION_SIZE;
		this.next_step = Block.STEP.FALL;


		// int color_index = Random.Range(
		// (int)Block.COLOR.FIRST, (int)Block.COLOR.LAST + 1);
		// this.setColor((Block.COLOR)color_index);
		// ���� ������ ���� Ȯ���� �������� ����� ���� �����Ѵ�.
		Block.COLOR color = this.block_root.selectBlockColor();
		this.setColor(color);

	}

	public bool isVacant()
	{
		bool is_vacant = false;
		if (this.step == Block.STEP.VACANT && this.next_step == Block.STEP.NONE)
		{
			is_vacant = true;
		}
		return (is_vacant);
	}

	public bool isSliding()
	{
		bool is_sliding = (this.position_offset.x != 0.0f);
		return (is_sliding);
	}

}

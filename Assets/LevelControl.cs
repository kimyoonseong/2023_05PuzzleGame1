using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData
{
	public float[] probability; // ����� �����󵵸� �����ϴ� �迭.
	public float heat_time; // ���ҽð�.
	public LevelData() // ������.
	{
		// ����� ���� ���� ���� ũ��� ���̾� ������ Ȯ��.
		this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM];
		// ��� ������ ����Ȯ���� �켱 �յ��ϰ� �صд�.
		for (int i = 0; i < (int)Block.COLOR.NORMAL_COLOR_NUM; i++)
		{
			this.probability[i] =
				1.0f / (float)Block.COLOR.NORMAL_COLOR_NUM;
		}
	}
	// ��� ������ ����Ȯ���� 0���� �����ϴ� �޼ҵ�.
	public void clear()
	{
		for (int i = 0; i < this.probability.Length; i++)
		{
			this.probability[i] = 0.0f;
		}
	}
	// ��� ������ ����Ȯ���� �հ踦 100%(=1.0)�� �ϴ� �޼ҵ�.
	public void normalize()
	{
		float sum = 0.0f;
		// ����Ȯ���� '�ӽ� �հ谪'�� ����Ѵ�.
		for (int i = 0; i < this.probability.Length; i++)
		{
			sum += this.probability[i];
		}
		for (int i = 0; i < this.probability.Length; i++)
		{
			// ������ ����Ȯ���� '�ӽ� �հ谪'���� ������, �հ谡 100%(=1.0) �� ��������.
			this.probability[i] /= sum;
			// ���� �� ���� ���Ѵ��� .
			if (float.IsInfinity(this.probability[i]))
			{
				this.clear(); // ��� Ȯ���� 0���� �����ϰ�.
				this.probability[0] = 1.0f; // ������ ��Ҹ� 1.0���� �صд�.
				break; // �׸��� ������ ����������.
			}
		}
	}
}


public class LevelControl
{
	private List<LevelData> level_datas = null; // �� ������ ���� ������.
	private int select_level = 0; // ���õ� ����.

	public void initialize()
	{
		// List�� �ʱ�ȭ.
		this.level_datas = new List<LevelData>();
	}

	public void loadLevelData(
		TextAsset level_data_text)
	{
		// �ؽ�Ʈ �����͸� ���ڿ��μ� �޾Ƶ��δ�.
		string level_texts = level_data_text.text;
		// ���� �ڵ�'\'���� ������, ���ڿ� �迭�� ����ִ´�.
		string[] lines = level_texts.Split('\n');
		// lines ���� �� �࿡ ���Ͽ� ���ʷ� ó���ذ��� ����.
		foreach (var line in lines)
		{
			if (line == "")
			{ // ���� �������.
				continue; // �Ʒ� ó���� ���� �ʰ� ������ ó������ ����.
			}
			string[] words = line.Split(); // �� ���� ���带 �迭�� ����.
			int n = 0;
			// LevelData�� ������ �ۼ�.
			// ���⿡ ���� ó���ϴ� ���� �����͸� �ִ´�.
			LevelData level_data = new LevelData();
			// words���� �� ���忡 ���ؼ�, ������� ó���� ���� ����.
			foreach (var word in words)
			{
				if (word.StartsWith("#"))
				{ // ������ ���� ���ڰ� #�̸�.
					break; // ���� Ż��.
				}
				if (word == "")
				{ // ���尡 �������.
					continue; // ���� �������� ����.
				}
				// 'n'�� ���� 0,1,2,...6���� ��ȭ���Ѱ����ν� �ϰ� �� �׸��� ó��.
				// �� ���带 float������ ��ȯ�ϰ� level_data�� ����. 
				switch (n)
				{
					case 0:
						level_data.probability[(int)Block.COLOR.PINK] =
							float.Parse(word); break;
					case 1:
						level_data.probability[(int)Block.COLOR.BLUE] =
							float.Parse(word); break;
					case 2:
						level_data.probability[(int)Block.COLOR.GREEN] =
							float.Parse(word); break;
					case 3:
						level_data.probability[(int)Block.COLOR.ORANGE] =
							float.Parse(word); break;
					case 4:
						level_data.probability[(int)Block.COLOR.YELLOW] =
							float.Parse(word); break;
					case 5:
						level_data.probability[(int)Block.COLOR.MAGENTA] =
							float.Parse(word); break;
					case 6:
						level_data.heat_time =
							float.Parse(word); break;
				}
				n++;
			}
			if (n >= 7)
			{ // 8�׸�(�̻�)�� ����� ó���Ǿ��ٸ�.
			  // ���� Ȯ���� �հ谡 ��Ȯ�� 100%�� �ǵ��� �ϰ� ����.
				level_data.normalize();
				// List ������ level_datas�� level_data�� �߰��Ѵ�.
				this.level_datas.Add(level_data);


			}
			else
			{ // �׷��� ������(���� ���ɼ��� �ִ�).
				if (n == 0)
				{ // 1���嵵 ó������ ���� ���� �ּ��̹Ƿ�.
				  // ���� ����. �ƹ��͵� ���� �ʴ´�.
				}
				else
				{ // �� �̿ܶ�� ����.
				  // �������� ������ ���� �ʴ´ٴ� ���� �޽����� ǥ��.
					Debug.LogError("[LevelData] Out of parameter.\n");
				}
			}
		}


		// level_datas�� �����Ͱ� �ϳ��� ������.
		if (this.level_datas.Count == 0)
		{
			// ���� �޽����� ǥ��.
			Debug.LogError("[LevelData] Has no data.\n");
			// level_datas�� LevelData�� �ϳ� �߰��� �д�.
			this.level_datas.Add(new LevelData());
		}
	}
	public void selectLevel()
	{
		// 0~���� ������ ���� ���Ƿ� ����.
		this.select_level = Random.Range(0, this.level_datas.Count);
		Debug.Log("select level = " + this.select_level.ToString());
	}
	public LevelData getCurrentLevelData()
	{
		// ���õ� ������ ���� �����͸� ��ȯ�Ѵ�.
		return (this.level_datas[this.select_level]);
	}
	public float getVanishTime()
	{
		// ���õ� ������ ���ҽð��� ��ȯ�Ѵ�.
		return (this.level_datas[this.select_level].heat_time);
	}
}












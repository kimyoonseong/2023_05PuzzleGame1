using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			SceneManager.LoadScene(1);
			//SceneManager.LoadScene("GameScene");
		}
	}

	private GUIStyle guiStyle = new GUIStyle();
	
	
	void OnGUI()
	{

		guiStyle.fontSize = 80; //change the font size
		guiStyle.normal.textColor = Color.black;

		
		//GUI.Label(new Rect(Screen.width / 2 - 140, Screen.height / 2 - 40, 64, 32), "POP POP POP IT", guiStyle);
		GUI.Label(new Rect(Screen.width / 2 - 140, Screen.height / 2 - 40, 64, 32), "POP POP POP IT", guiStyle);
	}
}

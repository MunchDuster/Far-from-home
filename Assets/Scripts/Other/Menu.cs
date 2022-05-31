using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	public void Quit()
	{
		Application.Quit();
	}

	public void LoadScene(string name)
	{
		Time.timeScale = 1;
		SceneManager.LoadScene(name);
	}
}

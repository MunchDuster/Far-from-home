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
		SceneManager.LoadScene(name);
	}
}

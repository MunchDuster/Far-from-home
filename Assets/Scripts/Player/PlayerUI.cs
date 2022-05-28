using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour
{
	private struct Subtitle
	{
		public string talker;
		public string text;
		public float lifeTime;
		public float makeTime;

		public Subtitle(string text, string talker, float subtitleCharLifetime)
		{
			this.text = text;
			this.talker = talker;

			lifeTime = text.Length * subtitleCharLifetime;
			makeTime = Time.timeSinceLevelLoad;
		}
	}

	public static PlayerUI ui;
	

	public TextMeshProUGUI areaText;
	public TextMeshProUGUI errorText;
	public TextMeshProUGUI tasksText;
	public TextMeshProUGUI subtitleText;
	public TextMeshProUGUI minigameText;
	public TextMeshProUGUI itemText;

	public float errorWaitTimePerChar = 0.15f;
	public float errorWriteTimePerChar = 0.05f;
	public float subtitleCharLifetime = 0.12f;

	private Coroutine errorCoroutine;
	private List<Task> tasks = new List<Task>();
	private List<Subtitle> subtitles = new List<Subtitle>();

	public void SetArea(string area)
	{
		areaText.text = area;
	}
	public void ShowError(string errorMsg)
	{
		if (errorCoroutine != null)
		{
			StopCoroutine(errorCoroutine);
			errorText.text = "";
		}

		errorCoroutine = StartCoroutine(WriteError(errorMsg));
	}
	public void AddSubtitle(string text, string talker)
	{
		Subtitle newSubtitle = new Subtitle(text, talker, subtitleCharLifetime);
		subtitles.Add(newSubtitle);

		UpdateSubtitlesText();

		StartCoroutine(HideSubtitle(newSubtitle));
	}

	public void AddTask(string name)
	{
		Task task = new Task();
		task.name = name;

		AddTask(task);
	}
	public void AddTask(Task task)
	{
		tasks.Insert(0, task);
		UpdateTasksText();
	}
	public void RemoveTask(string name)
	{
		Task task = tasks.Find((task) => task.name == name);

		if (task == null)
		{
			Debug.LogError("RemoveTask error: Task not found (name\"" + name + "\")");
			Debug.LogError(new System.Exception().StackTrace);
		}
		RemoveTask(task);

	}
	public void CompleteTask(string name)
	{
		Task task = tasks.Find((task) => task.name == name);

		if (task == null)
		{
			Debug.LogError("CompleteTask error: Task not found (name\"" + name + "\")");
			Debug.LogError(new System.Exception().StackTrace);
		}
		CompleteTask(task);
	}
	public void RemoveTask(Task task)
	{
		tasks.Remove(task);
		UpdateTasksText();
	}
	public void CompleteTask(Task task)
	{
		task.SetCompleted(true);
		UpdateTasksText();
	}

	// Start is called before the first frame update
	private void Start()
	{
		ui = this;
	}
	private void UpdateTasksText()
	{
		StringBuilder text = new StringBuilder();

		text.Append("<B><u>Tasks</u></B>\n");

		for (int i = 0; i < tasks.Count; i++)
		{
			if (tasks[i].completed)
			{
				text.Append("<s>");
				text.Append(tasks[i].name);
				text.Append("</s>");
			}
			else
			{
				text.Append(tasks[i].name);
			}
			text.Append('\n');
		}

		tasksText.text = text.ToString();
	}
	private IEnumerator WriteError(string errorMsg)
	{

		for (int i = 0; i < errorMsg.Length; i++)
		{
			errorText.text = errorMsg.Substring(0, i + 1);
			yield return new WaitForSeconds(errorWriteTimePerChar);
		}

		errorCoroutine = StartCoroutine(ClearError(errorMsg));
	}
	private IEnumerator ClearError(string errorMsg)
	{
		yield return new WaitForSeconds(errorWaitTimePerChar * errorMsg.Length);
		errorText.text = "";
		errorCoroutine = null;
	}
	private IEnumerator HideSubtitle(Subtitle subtitle)
	{
		yield return new WaitForSeconds(subtitle.lifeTime);
		subtitles.Remove(subtitle);
		UpdateSubtitlesText();
	}
	private void UpdateSubtitlesText()
	{
		subtitleText.text = "";

		StringBuilder stringBuilder = new StringBuilder();

		stringBuilder.Append("<mark=#202020A0>");
		foreach (Subtitle subtitle in subtitles)
		{
			stringBuilder.Append(subtitle.talker);
			stringBuilder.Append(": ");
			stringBuilder.Append(subtitle.text);
			stringBuilder.Append("\n");
		}
		stringBuilder.Append("</mark>");


		subtitleText.text = stringBuilder.ToString();
	}
}
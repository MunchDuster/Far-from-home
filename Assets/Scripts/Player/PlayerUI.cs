using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour
{
	public TextMeshProUGUI areaText;
	public TextMeshProUGUI errorText;
	public TextMeshProUGUI tasksText;

	public float errorWaitTimePerChar = 0.15f;
	public float errorWriteTimePerChar = 0.05f;

	public void SetArea(string area)
	{
		areaText.text = area;
	}

	private Coroutine errorCoroutine;
	public void ShowError(string errorMsg)
	{
		if (errorCoroutine != null)
		{
			StopCoroutine(errorCoroutine);
			errorText.text = "";
		}

		errorCoroutine = StartCoroutine(WriteError(errorMsg));
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

	//Task management
	private static List<Task> tasks = new List<Task>();

	public void AddTask(Task task)
	{
		tasks.Insert(0, task);
		UpdateTasksText();
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

}
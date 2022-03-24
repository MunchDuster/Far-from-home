using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class RocketComputer : MonoBehaviour
{
	public TextMeshProUGUI loadingText;
	public TextMeshProUGUI consoleText;
	public float dotsDelta = 0.25f;
	public float turnOnTime = 3;

	public UnityEvent OnTurnOn;
	public UnityEvent OnTurnOff;

	private delegate void OnGUIUpdate();
	private OnGUIUpdate onGui;

	private List<string> consoleLines = new List<string>();
	private string inputLine = "";

	public void TurnOn()
	{

		StartCoroutine(ShowStartText());
	}
	public void TurnOff()
	{
		OnTurnOff.Invoke();
	}

	private IEnumerator ShowStartText()
	{
		int noOfDots = -1;

		for (float t = 0; t < turnOnTime; t += dotsDelta)
		{
			noOfDots = (noOfDots >= 3) ? 0 : ++noOfDots;

			string dots = "";
			for (int i = 0; i < noOfDots; i++) dots += ".";

			loadingText.text = "Booting" + dots;

			yield return new WaitForSeconds(dotsDelta);
		}

		loadingText.text = "";

		OnTurnOn.Invoke();

		UpdateConsole();
		onGui = CommandInput;
	}

	void OnGUI()
	{
		if (onGui != null) onGui();
	}

	private void CommandInput()
	{
		Event e = Event.current;
		if (e.isKey)
		{
			switch (e.keyCode)
			{
				case KeyCode.None:
					//Add character
					inputLine += (e.shift) ? e.character.ToString().ToUpper() : e.character;

					return;
				case KeyCode.Return:
					//Enter command
					CheckCommand();
					break;
				case KeyCode.Backspace:
					//Implement backspace
					if (inputLine.Length > 0) inputLine = inputLine.Substring(0, inputLine.Length - 1);
					break;
				case KeyCode.Escape:
					//Stop inputting
					onGui -= CommandInput;
					break;
				default:

					break;
			}
			UpdateConsole();
		}
	}
	private void UpdateConsole()
	{
		StringBuilder log = new StringBuilder();
		for (int i = 0; i < consoleLines.Count; i++)
		{
			log.Append(consoleLines[i]);
		}
		log.Append("Input: " + inputLine);

		consoleText.text = log.ToString();
	}
	private void CheckCommand()
	{
		Debug.Log("Command: " + inputLine);

		consoleLines.Add(inputLine);

		consoleText.text = "You entered: " + inputLine + "\nOoga booga";


		//Code here...

		inputLine = "";
	}
}
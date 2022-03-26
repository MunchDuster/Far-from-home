using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class RocketComputer : MonoBehaviour
{
	private class Line
	{
		public static List<Line> lines;

		public string text;

		public Line()
		{
			lines.Add(this);
		}
		public Line(string text)
		{
			this.text = text;
			lines.Add(this);
		}
	}
	public TextMeshProUGUI loadingText;
	public TextMeshProUGUI consoleText;

	public float dotsDelta = 0.25f;
	public float bootTime = 3;
	public float blinkSpeed = 0.7f;

	public string systemColour = "green";

	public UnityEvent OnTurnOn;
	public UnityEvent OnTurnOff;

	private delegate void OnEvent();
	private OnEvent onGui;

	private delegate void OnSetText(string text);

	private Dictionary<string, OnEvent> commands = new Dictionary<string, OnEvent>();

	//Console logs
	private Line inputLine;
	private string inputText;
	private bool takingInput;


	// Start is called before the first frame update
	private void Start()
	{
		commands.Add("clear", ClearConsole);
		commands.Add("diagnostics", () => { StartCoroutine(RunDiagnostics()); });
		commands.Add("help", ListCommands);

		Line.lines = new List<Line>();
	}


	//Events
	private void OnFinishedTurningOn()
	{
		loadingText.text = "";

		//Get things going
		OnTurnOn.Invoke();

		new Line(SystemText("Enter \"help\" for a list of commands."));
		inputLine = new Line();
		UpdateConsole();
		onGui += CommandInput;
		onGui += UpdateConsole;
	}
	private void OnFinishedCommand()
	{
		inputLine = new Line();
		inputText = "";
		takingInput = true;
	}

	//Text control
	private string SystemText(string text, int type = 0)
	{
		if (type == 0) return "<color=\"" + systemColour + "\">" + text + "</color>";
		else if (type == 1) return "<color=\"yellow\">" + text + "</color>";
		else if (type == 2) return "<color=\"red\">" + text + "</color>";
		else throw new System.ArgumentException("Invalid type for SystemText: " + type);
	}
	private string InputChar()
	{
		return (Time.time % blinkSpeed < blinkSpeed / 2) ? "\u2588" : "";
	}


	//Main public functions
	public void TurnOn()
	{
		consoleText.text = "";
		Line.lines.Clear();

		takingInput = true;

		StartCoroutine(LoadText("Booting", bootTime, (string text) => { loadingText.text = text; }, OnFinishedTurningOn));
	}
	public void TurnOff()
	{
		OnTurnOff.Invoke();
	}

	//Commands
	private void ClearConsole()
	{
		Line.lines.Clear();
		takingInput = true;

		OnFinishedCommand();
	}
	private IEnumerator RunDiagnostics()
	{
		Line loadingLine = new Line("");

		yield return StartCoroutine(LoadText("Checking", 2, (string text) => { loadingLine.text = text; }, () => { }));

		string tick = "<sprite name=\"tick\" color=\"#00FF00\">";
		string cross = "<sprite name=\"cross\" color=\"#FF0000\">";


		bool enginesAreFuelled = false;
		if (enginesAreFuelled) new Line(SystemText("Engines Fuelled: " + tick));
		else
		{
			loadingLine.text += SystemText(" Error.", 2);
			new Line(SystemText("Engines Fuelled: " + cross, 2));
		}

		OnFinishedCommand();
	}
	private void ListCommands()
	{
		new Line(SystemText("help: See list of commands."));
		new Line(SystemText("clear: Clear console."));
		new Line(SystemText("diagnostics: Run flight checks."));

		OnFinishedCommand();
	}

	//Private functions
	private IEnumerator LoadText(string text, float time, OnSetText setter, OnEvent callback)
	{
		int noOfDots = -1;

		for (float t = 0; t < time; t += dotsDelta)
		{
			//Loop from 0 to 3 dots
			noOfDots = ++noOfDots % 4;

			//Put that many dots onto string
			string dots = "";
			for (int i = 0; i < noOfDots; i++) dots += ".";

			setter(SystemText(text + dots));

			yield return new WaitForSeconds(dotsDelta);
		}

		callback();
	}

	void OnGUI()
	{
		if (onGui != null) onGui();
	}

	private void CommandInput()
	{
		Event e = Event.current;
		if (e.isKey && takingInput)
		{
			switch (e.keyCode)
			{
				case KeyCode.None:

					if (e.character == '\n') break;
					//Add character
					inputText += (e.shift) ? e.character.ToString().ToUpper() : e.character;
					break;

				case KeyCode.Return:

					if (e.type == EventType.KeyDown)
					{
						//Enter command
						CheckCommand();
					}
					break;
				case KeyCode.Backspace:
					if (e.type == EventType.KeyDown)
					{
						//Implement backspace
						if (inputText.Length > 0) inputText = inputText.Substring(0, inputText.Length - 1);
					}
					break;
				case KeyCode.Escape:
					if (e.type == EventType.KeyDown)
					{
						//Stop inputting
						onGui -= CommandInput;
					}
					break;
				default:

					break;
			}
		}
	}

	private void UpdateConsole()
	{
		if (takingInput) inputLine.text = SystemText("Input: ") + inputText + InputChar();


		StringBuilder log = new StringBuilder();
		foreach (Line line in Line.lines)
		{
			log.Append(line.text);
			log.Append('\n');
		}

		consoleText.text = log.ToString();
	}

	private void CheckCommand()
	{
		Debug.Log("Command: " + inputText);

		inputLine.text = SystemText("Input: ") + inputText;

		string[] words = inputText.Split(" ");

		if (commands.ContainsKey(words[0]))
		{
			//Run command
			takingInput = false;
			commands[words[0]]();
		}
		else
		{
			new Line(SystemText("Error: unknown command", 2));
			OnFinishedCommand();
		}
	}
}
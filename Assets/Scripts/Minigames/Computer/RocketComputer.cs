using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class RocketComputer : Computer
{
	public int maxLines = 10;

	private class Line
	{
		public static List<Line> lines;
		public static int maxLines;

		public string text;

		public Line()
		{
			lines.Add(this);

			if(lines.Count >= maxLines)
			{
				lines.RemoveAt(0);
			}
		}
		public Line(string text)
		{
			this.text = text;
			lines.Add(this);
		}
	}

	public TextMeshProUGUI loadingText;
	public TextMeshProUGUI consoleText;
	public Animator animator;

	[Header("Settings")]
	public float maxPreferredHeight = 100;
	public float bootTime = 3;
	public float blinkSpeed = 0.7f;
	public string systemColour = "green";
	public float launchTime = 10;
	public string[] credits;

	[Header("Control")]
	public bool enginesAreFuelled = false;
	public bool flightPathCreated = false;

	[Header("Events")]
	public UnityEvent OnLaunch;
	public UnityEvent OnAfterLaunched;

	private Dictionary<string, OnEvent> commands = new Dictionary<string, OnEvent>();

	//Console logs
	private Line inputLine;
	private bool takingInput;

	// Start is called before the first frame update
	private void Start()
	{
		commands.Add("clear", ClearConsole);
		commands.Add("diagnostics", () => { StartCoroutine(RunDiagnostics()); });
		commands.Add("help", ListCommands);
		commands.Add("launch", () => { StartCoroutine(Launch()); });

		Line.lines = new List<Line>();
		Line.maxLines = maxLines;
	}

	//Events
	protected override void PoweredOn()
	{
		loadingText.text = "";

		//Get things going
		OnPowerOn.Invoke(true);

		new Line(SystemText("Enter \"help\" for a list of commands."));
		inputLine = new Line();

		OnCharEntered();
		caretBlinker = StartCoroutine(BlinkCaret());
	}
	private void OnFinishedCommand()
	{
		inputLine = new Line();
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
	private string GetCaretBlinkChar()
	{
		return (Time.time % blinkSpeed < blinkSpeed / 2) ? "\u2588" : "";
	}
	private Coroutine caretBlinker;

	//Main public functions
	public override void PowerOn(bool on)
	{
		takingInput = on;

		if(on)
		{
			consoleText.text = "";
			Line.lines.Clear();			
		}
		else
		{
			takingInput = false;
			if(caretBlinker != null) StopCoroutine(caretBlinker);
		}

		base.PowerOn(on);
	}

	protected override IEnumerator PowerUp()
	{
		yield return StartCoroutine(LoadText(
				"Booting", 
				bootTime, 
				(string text) => { loadingText.text = SystemText(text); }, 
				PoweredOn
			)
		);
	}

	public float engines = 3;
	private int enginesFuelled;

	public void EngineFuelled() 
	{
		enginesFuelled++;
		if(enginesFuelled == engines) 
		{
			PlayerUI.ui.CompleteTask("Fuel Engines x3");
			enginesAreFuelled = true;
		}
	}

	public void EnginesAreFuelled() 
	{ 
		enginesAreFuelled = true; 
	}
	public void PathIsCalculated()
	{
		PlayerUI.ui.CompleteTask("Calculate flight path");
		flightPathCreated = true;
	}
	public void StartRollingCredits()
	{
		Line.lines.Clear();
		StartCoroutine(RollCredits());
	}
	private IEnumerator RollCredits()
	{
		Debug.Log("Rolling Credits");
		new Line("<u>Credits</u>");
		UpdateConsole();

		int i = 0;
		while(true)
		{
			yield return new WaitForSeconds(2);
			if(i < credits.Length) new Line(SystemText(credits[i++]));
			else new Line("");
			UpdateConsole();
		}
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

		yield return StartCoroutine(LoadText("Checking", 2, (string text) => { loadingLine.text = text; UpdateConsole();}));

		string tick = "<sprite name=\"tick\" color=\"#00FF00\">";
		string cross = "<sprite name=\"cross\" color=\"#FF0000\">";

		if (enginesAreFuelled)
		{
			//Yes fuel
			new Line(SystemText("Engines Fuelled: " + tick));
			

			yield return StartCoroutine(LoadText("Checking", 2, (string text) => { loadingLine.text = text; UpdateConsole();}));

			if (flightPathCreated)
			{
				//Diagnostics success
				loadingLine.text += SystemText(" Done");
				new Line(SystemText("Flight path: " + tick));
			}
			else
			{
				//No flight path
				loadingLine.text += SystemText(" Error.", 2);
				new Line(SystemText("Flight path: " + cross, 2));
				PlayerUI.ui.AddTask("Calculate flight path");
			}
		}
		else
		{
			//No fuel
			loadingLine.text += SystemText(" Error.", 2);
			new Line(SystemText("Engines Fuelled: " + cross, 2));
			PlayerUI.ui.AddTask("Fuel Engines x3");
		}

		OnFinishedCommand();
	}
	
	private IEnumerator Launch()
	{
		Line initLine = new Line();

		yield return StartCoroutine(LoadText(SystemText("Initializing launch sequence", 1), 2, (string text) => { initLine.text = text; UpdateConsole();}));

		if (enginesAreFuelled && flightPathCreated)
		{
			initLine.text += SystemText(" Done", 1);

			Line countdownLine = new Line();

			//Countdown
			for (int i = 10; i > 0; i--)
			{
				countdownLine.text = SystemText("Launching in... " + i, 1);
				UpdateConsole();
				yield return new WaitForSeconds(1);
			}

			countdownLine.text = SystemText("Launching!", 1);
			UpdateConsole();

			OnLaunch.Invoke();

			yield return new WaitForSeconds(launchTime);

			OnAfterLaunched.Invoke();
		}
		else
		{
			initLine.text += SystemText(" Error!", 2);

			new Line(SystemText("Error: systems not ready to launch, check diagnostics for more info.", 2));
			UpdateConsole();

			OnFinishedCommand();
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
	private void ListCommands()
	{
		new Line(SystemText("help: See list of commands."));
		new Line(SystemText("clear: Clear console."));
		new Line(SystemText("diagnostics: Run flight checks."));
		new Line(SystemText("launch: Launch rocket."));

		OnFinishedCommand();
	}


	private IEnumerator BlinkCaret()
	{
		while (true)
		{
			if (takingInput)
			{
				Debug.Log("Running");
				inputLine.text = SystemText("Input: ") + line + GetCaretBlinkChar();
				UpdateConsole();
			}
			yield return new WaitForEndOfFrame();

		}
	}
	protected override void OnCharEntered()
	{
		if (takingInput)
		{
			inputLine.text = SystemText("Input: ") + line + GetCaretBlinkChar();

			UpdateConsole();
		}
	}

	private void UpdateConsole()
	{
		if(consoleText.preferredHeight > maxPreferredHeight)
		{
			//Remove oldest log
			Line.lines.RemoveAt(0);
		}
		StringBuilder log = new StringBuilder();
		
		foreach (Line logline in Line.lines)
		{
			log.Append(logline.text);
			log.Append('\n');
		}

		consoleText.text = log.ToString();		
	}
	
	protected override void OnCommandEntered()
	{
		inputLine.text = SystemText("Input: ") + line;

		string[] words = line.Split(" ");

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
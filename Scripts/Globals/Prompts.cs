using Godot;
using System;
using System.Collections.Generic;

public class Prompts : Node
{
	public static Prompts Main;

	Prompts()
	{
		Main = this;
	}

	// ================================================================

	private bool anyFocused = false;
	private int focusedIndex = 0;
	private List<string> promptsList = new List<string>();

	 private Dictionary<string, string> keyCodes = new Dictionary<string, string>()
	{
		{"key_a", "A"},
		{"key_b", "B"},
		{"key_c", "C"},
		{"key_d", "D"},
		{"key_e", "E"},
		{"key_f", "F"},
		{"key_g", "G"},
		{"key_h", "H"},
		{"key_i", "I"},
		{"key_j", "J"},
		{"key_k", "K"},
		{"key_l", "L"},
		{"key_m", "M"},
		{"key_n", "N"},
		{"key_o", "O"},
		{"key_p", "P"},
		{"key_q", "Q"},
		{"key_r", "R"},
		{"key_s", "S"},
		{"key_t", "T"},
		{"key_u", "U"},
		{"key_v", "V"},
		{"key_w", "W"},
		{"key_x", "X"},
		{"key_y", "Y"},
		{"key_z", "Z"},
		{"key_1", "1"},
		{"key_2", "2"},
		{"key_3", "3"},
		{"key_4", "4"},
		{"key_5", "5"},
		{"key_6", "6"},
		{"key_7", "7"},
		{"key_8", "8"},
		{"key_9", "9"},
		{"key_0", "0"},
		{"key_comma", ","},
		{"key_period", "."},
		{"key_question", "?"},
		{"key_exclamation", "!"},
		{"key_hash", "#"},
		{"key_dollarsign", "$"},
		{"key_percent", "%"},
		{"key_ampersand", "&"},
		{"key_openparen", "("},
		{"key_closeparen", ")"},
		{"key_hyphen", "-"},
		{"key_plus", "+"},
		{"key_equal", "="},
		{"key_slash", "/"},
		{"key_apostrophe", "'"},
		{"key_quote", "\""},
		{"key_colon", ":"},
		{"key_semicolon", ";"}
	};

	// Refs
	private PackedScene promptRef = GD.Load<PackedScene>("res://Instances/Prompt.tscn");
	private DynamicFont textFont = GD.Load<DynamicFont>("res://Fonts/TextFont.tres");

	// ================================================================

	public bool AnyFocused { get => Prompts.Main.anyFocused; set => Prompts.Main.anyFocused = value; }
	public int FocusedIndex { get => Prompts.Main.focusedIndex; set => Prompts.Main.focusedIndex = value; }
	public List<string> PromptsList { get => Prompts.Main.promptsList; }

	// ================================================================

	public override void _Ready()
	{
		File file = new File();
		try
		{
			file.Open("res://Prompts/PromptsMASTER.txt", (int)File.ModeFlags.Read);
			while (!file.EofReached())
			{
				string line = file.GetLine();
				promptsList.Add(line);
			}
		}
		finally
		{
			if (file.IsOpen())
				file.Close();
		}
	}


	public override void _Process(float delta)
	{
		// Debug
		if (Input.IsActionJustPressed("debug_1"))
		{
			AddPrompt(PromptsList[Mathf.RoundToInt((float)GD.RandRange(0, PromptsList.Count - 1))], new Vector2((int)GD.RandRange(0, 300), (int)GD.RandRange(0, 180)));
		}

		if (Input.IsActionJustPressed("cancel_prompt") && anyFocused)
		{
			GetChild<Prompt>(focusedIndex).Reset();
			anyFocused = false;
			return;
		}

		if (Input.IsActionJustPressed("select_up") && anyFocused)
		{
			GetChild<Prompt>(focusedIndex).Reset();
			focusedIndex = Mathf.Wrap(focusedIndex - 1, 0, GetChildren().Count - 1);
			GetChild<Prompt>(focusedIndex).Focused = true;
			return;
		}

		if (Input.IsActionJustPressed("select_down") && anyFocused)
		{
			GetChild<Prompt>(focusedIndex).Reset();
			focusedIndex = Mathf.Wrap(focusedIndex + 1, 0, GetChildren().Count - 1);
			GetChild<Prompt>(focusedIndex).Focused = true;
			return;
		}

		if (anyFocused)
		{
			var p = GetChild<Prompt>(focusedIndex);
			foreach (var action in keyCodes)
			{
				if (Input.IsActionJustPressed(action.Key) && p.PromptText[p.TextPosition].ToString().ToUpper() == action.Value)
					IncrementPosition(ref p);
			}
		}
		else
		{
			for (int i = 0; i < GetChildren().Count; i++)
			{
				var p = GetChild<Prompt>(i);
				foreach (var action in keyCodes)
				{
					if (Input.IsActionJustPressed(action.Key) && p.PromptText[p.TextPosition].ToString().ToUpper() == action.Value)
					{
						p.Focused = true;
						IncrementPosition(ref p);
						focusedIndex = i;
						anyFocused = true;
						return;
					}	
				}
			}
		}
	}


	/* public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey)
		{
			var ev = (InputEventKey)@event;

			//if (OS.GetScancodeString(ev.Scancode) == "Shift+comma")
				//GD.Print("hello");
			GD.Print(OS.GetScancodeString(ev.Scancode));

			if (ev.Scancode == (int)KeyList.Tab && anyFocused)
			{
				GetChild<Prompt>(focusedIndex).Reset();
				anyFocused = false;
				return;
			}

			if (anyFocused)
			{
				var p = GetChild<Prompt>(focusedIndex);
				//if (OS.GetScancodeString(ev.Scancode) == "Shift+comma")
				//GD.Print("hello");
				//GD.Print(OS.GetScancodeString(ev.Scancode));
				//if (p.PromptText[p.TextPosition].ToString().ToUpper() == keyCodes[OS.GetScancodeString(ev.Scancode).ToUpper()])
					//IncrementPosition(ref p);
			}
			else
			{
				for (int i = 0; i < GetChildren().Count; i++)
				{
					var p = GetChild<Prompt>(i);
					if (p.PromptText[0].ToString().ToUpper() == OS.GetScancodeString(ev.Scancode))
					{
						p.Focused = true;
						IncrementPosition(ref p);
						focusedIndex = i;
						anyFocused = true;
						break;
					}
				}
			}
		}
	}*/

	// ================================================================

	public static void AddPrompt(string text, Vector2 position)
	{
		var promptInst = (Prompt)Prompts.Main.promptRef.Instance();
		promptInst.PromptText = text;
		promptInst.Position = position;
		promptInst.TextFont = Prompts.Main.textFont;
		Prompts.Main.AddChild(promptInst);
	}


	public static void IncrementPosition(ref Prompt prompt)
	{
		prompt.TextPosition++;
		if (prompt.TextPosition >= prompt.PromptText.Length)
			prompt.Finished = true;
		
		if (!prompt.Finished)
			while (prompt.PromptText[prompt.TextPosition] == ' ')
				prompt.TextPosition++;
	}
}

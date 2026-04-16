using Ast.Tables;
using Runtime.Objects;
using Ast.Nodes.Melodies.Chords.Notes.Modifiers;
using Ast.Nodes.Samples;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes;

public class NoteNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string SampleName = "";
	public string Pitch = "";
	public Note Note0 = new();

	protected override void Parse()
	{
		string firstIdentifier = "";
		Parser.ConsumeToken(TokenType.Identifier, (value) => firstIdentifier = value);

		if (IsPitch(firstIdentifier))
		{
			Pitch = firstIdentifier;
		}
		else
		{
			SampleName = firstIdentifier;

			if (Parser.CurrentToken.Type == TokenType.Identifier)
			{
				Parser.ConsumeToken(TokenType.Identifier, (value) => Pitch = value);
			}
		}

		if (Parser.CurrentToken.Type == TokenType.LeftParentheses)
		{
			new ModifiersNode(this);
		}
	}

	protected override void Validate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		MelodyNode melodyNode = ancestors.Get<MelodyNode>();

		if (!string.IsNullOrEmpty(SampleName) && !symbols.Contains(typeof(SampleNode), SampleName))
		{
			Validator.AddError(this, $"Melody: '{melodyNode.Id}'. The sample reference '{SampleName}' is not declared");
		}
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		Melody melody = ancestors.Get<MelodyNode>().Melody0;
		ChordNode chordNode = ancestors.Get<ChordNode>();

		Note0.StartBeat = chordNode.StartBeat;
		Note0.EndBeat = chordNode.EndBeat;
		
		if (!string.IsNullOrEmpty(Pitch))
		{
			Note0.Pitch0 = new(Pitch);
		}

		if (!string.IsNullOrEmpty(SampleName))
		{
			Sample sample = variables.Get<Sample>(SampleName);
			Note0.SampleOverride = sample;
		}

		melody.Notes.Add(Note0);
	}

	private static bool IsPitch(string value)
	{
		if (string.IsNullOrEmpty(value) || value.Length < 2)
		{
			return false;
		}

		char firstChar = char.ToLower(value[0]);
		if (firstChar < 'a' || firstChar > 'g')
		{
			return false;
		}

		return value.Substring(1).Any(char.IsDigit);
	}
}



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
	public bool IsRoman = false;
	public string RomanValue = "";
	public Note Note0 = new();

	protected override void Parse()
	{
		string firstIdentifier = "";
		Parser.ConsumeToken(TokenType.Identifier, (value) => firstIdentifier = value);

		if (IsPitch(firstIdentifier))
		{
			Pitch = firstIdentifier;
		}
		else if (IsRomanNumeral(firstIdentifier))
		{
			IsRoman = true;
			RomanValue = firstIdentifier;
		}
		else
		{
			SampleName = firstIdentifier;

			if (Parser.CurrentToken.Type == TokenType.Identifier)
			{
				Parser.ConsumeToken(
					TokenType.Identifier,
					value => Pitch = value);
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
		if (IsRoman && melodyNode.ScaleReferenceId == "")
		{
			Validator.AddError(
				this,
				$"Melody '{melodyNode.Id}' uses roman numerals but has no scale");
		}
	}

	protected override void Evaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		Melody melody = ancestors.Get<MelodyNode>().Melody0;
		ChordNode chordNode = ancestors.Get<ChordNode>();

		Note0.StartBeat = chordNode.StartBeat;
		Note0.EndBeat = chordNode.EndBeat;
		
		MelodyNode melodyNode = ancestors.Get<MelodyNode>();

		if (IsRoman)
		{
			if (melodyNode.Melody0.Scale == null)
			{
				throw new Exception(
					$"Melody '{melodyNode.Id}' uses roman numerals but has no scale");
			}

			Note0.Pitch0 =
				ResolveRomanPitch(
					melodyNode.Melody0.Scale,
					RomanValue);
		}
		else if (!string.IsNullOrEmpty(Pitch))
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

	private static bool IsRomanNumeral(string value)
	{
		if (string.IsNullOrEmpty(value))
			return false;

		string lower = value.ToLower();

		string romanPart = new string(lower
			.TakeWhile(c => char.IsLetter(c))
			.ToArray());

		return romanPart switch
		{
			"i" or "ii" or "iii" or "iv"
			or "v" or "vi" or "vii" => true,
			_ => false
		};
	}

	private static Pitch ResolveRomanPitch(
	Scale scale,
	string romanValue)
	{
		string lower = romanValue.ToLower();

		string romanPart = new string(
			lower.TakeWhile(char.IsLetter).ToArray());

		string octavePart = new string(
			lower.SkipWhile(char.IsLetter).ToArray());

		int octave = 5;

		if (!string.IsNullOrEmpty(octavePart))
		{
			octave = int.Parse(octavePart);
		}

		int degree = RomanToDegree(romanPart);

		Pitch basePitch =
			scale.Degrees[degree];

		return new Pitch
		{
			PitchClass = basePitch.PitchClass,
			Octave = octave
		};
	}
	private static int RomanToDegree(string roman)
	{
		return roman switch
		{
			"i" => 0,
			"ii" => 1,
			"iii" => 2,
			"iv" => 3,
			"v" => 4,
			"vi" => 5,
			"vii" => 6,
			_ => throw new Exception(
				$"Invalid roman numeral '{roman}'")
		};
}
}



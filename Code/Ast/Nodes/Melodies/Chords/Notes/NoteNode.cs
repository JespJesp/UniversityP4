using Ast.Nodes.Samples;
using Phases.Annotation;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes;

public class NoteNode : Node
{
	public ChordNode ChordNode;
	public Note Note = new();
	public string? PitchString;
	private string? _sampleOverrideId;
	public string SampleName = "";
	public string Pitch = "";
	public bool IsRoman = false;
	public string RomanValue = "";
	public Note Note0 = new();

	public NoteNode(ChordNode chordsNode)
	{
		this.ChordNode = chordsNode;
	}

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.Identifier, out string firstIdentifier);

		if (Pitch.IsPitch(firstIdentifier))
		{
			PitchString = firstIdentifier;
		}
		else if (IsRomanNumeral(firstIdentifier))
		{
			IsRoman = true;
			RomanValue = firstIdentifier;
		}
		else
		{
			_sampleOverrideId = firstIdentifier;
			if (parser.TryConsumeToken(TokenType.Identifier, out string pitchStringValue))
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
			throw new Exception($"Melody: '{ChordNode.ChordsNode.MelodyNode.Id}'. " + string.Join(" ", errors));
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		this.Note.StartBeat = ChordNode.StartBeat.Value;
		this.Note.EndBeat = ChordNode.EndBeat.Value;
		
		if (PitchString is not null)
		{
			this.Note.Pitch = Pitch.FromString(this.PitchString);
		}
		if (_sampleOverrideId is not null)
		{
			this.Note.SampleOverride = SymbolTable.Get<SampleNode>(_sampleOverrideId).Sample;
		}

		Melody melody = ChordNode.ChordsNode.MelodyNode.Melody;
		melody.Notes.Add(Note);
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


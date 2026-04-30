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
		else
		{
			_sampleOverrideId = firstIdentifier;
			if (parser.TryConsumeToken(TokenType.Identifier, out string pitchStringValue))
			{
				// Note: We cannot simply do "out PitchString", since the out would set its value to "", not null.
				PitchString = pitchStringValue; 
			};
		}

		if (parser.TryConsumeToken(TokenType.LeftParentheses))
		{
			parser.ParseChild(this, new ModifiersNode(this));
		}
	}

	public override void Annotate(Annotator annotator)
	{
		List<string> errors = new();
		if (_sampleOverrideId is not null && !SymbolTable.Contains<SampleNode>(_sampleOverrideId))
		{
			errors.Add($"Sample reference '{_sampleOverrideId}' is not declared");
		}

		if (errors.Count != 0)
		{
			throw new Exception($"Melody: '{ChordNode.ChordsNode.MelodyNode.Id}'. " + string.Join(" ", errors));
		}
	}

	public override void Validate(Validator validator)
	{
		List<string> errors = new();
		if (PitchString is not null)
		{
			try
			{
				Pitch.FromString(PitchString); // Throws exception if pitch string cannot be converted
			}
			catch (Exception exception)
			{
				errors.Add(exception.Message);
			}
		}
		
		if (errors.Count != 0)
		{
			throw new Exception($"Melody: '{ChordNode.ChordsNode.MelodyNode.Id}'. " + string.Join(" ", errors));
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Note.StartBeat = ChordNode.StartBeat.Value;
		Note.EndBeat = ChordNode.EndBeat.Value;
		
		if (PitchString is not null)
		{
			Note.Pitch = Pitch.FromString(PitchString);
		}
		if (_sampleOverrideId is not null)
		{
			Note.SampleOverride = SymbolTable.Get<SampleNode>(_sampleOverrideId).Sample;
		}

		Melody melody = ChordNode.ChordsNode.MelodyNode.Melody;
		melody.Notes.Add(Note);
	}
}


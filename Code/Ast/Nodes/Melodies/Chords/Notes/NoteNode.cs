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
	public string? PitchString;
	public string? SampleOverrideId;
	public Note Note = new();

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
			SampleOverrideId = firstIdentifier;
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
		if (SampleOverrideId is not null && !SymbolTable.Contains<SampleNode>(SampleOverrideId))
		{
			errors.Add($"Sample reference '{SampleOverrideId}' is not declared");
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
				Pitch.FromString(this.PitchString); // Throws exception if pitch string cannot be converted
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
		this.Note.StartBeat = ChordNode.StartBeat.Value;
		this.Note.EndBeat = ChordNode.EndBeat.Value;
		
		if (PitchString is not null)
		{
			this.Note.Pitch = Pitch.FromString(this.PitchString);
		}
		if (SampleOverrideId is not null)
		{
			this.Note.SampleOverride = SymbolTable.Get<SampleNode>(SampleOverrideId).Sample;
		}

		Melody melody = ChordNode.ChordsNode.MelodyNode.Melody;
		melody.Notes.Add(Note);
	}
}


using Phases.Annotation;
using Phases.Evaluation;
using Phases.Parsing;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes;

public class NoteNode : Node
{
	public ChordNode ChordNode;
	public string PitchString = "";
	public Note Note = new();

	public NoteNode(ChordNode chordsNode)
	{
		this.ChordNode = chordsNode;
	}

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.Identifier, (value) => PitchString = value);

		if (parser.TryConsumeToken(TokenType.LeftParentheses))
		{
			parser.ParseChild(this, new ModifiersNode(this));
		}
	}

	public override void Annotate(Annotator annotator)
	{
		Pitch.FromString(this.PitchString); // Throws error if pitch string cannot be converted
	}

	public override void Evaluate(Evaluator evaluator)
	{
		this.Note.StartBeat = ChordNode.StartBeat.Value;
		this.Note.EndBeat = ChordNode.EndBeat.Value;
		this.Note.Pitch = Pitch.FromString(this.PitchString);

		Melody melody = ChordNode.ChordsNode.MelodyNode.Melody;
		melody.Notes.Add(Note);
	}
}


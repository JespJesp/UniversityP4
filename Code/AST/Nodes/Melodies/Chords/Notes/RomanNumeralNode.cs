using Ast.Tables;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects;
using Tokens;

namespace Ast.Nodes.Melodies.Chords.Notes;

public class RomanNumeralNode : Node
{
    public ChordNode ChordNode;
    public string Numeral;

    public RomanNumeralNode(ChordNode chordNode)
    {
        ChordNode = chordNode;
    }

    public override void CascadeParse(Parser parser)
    {
        parser.ConsumeToken(TokenType.Identifier, out Numeral);
    }

    public override void Validate(Validator validator)
    {
        if (!RomanNumeralHelper.IsRomanNumeral(Numeral))
        {
            throw new Exception($"Invalid roman numeral '{Numeral}'");
        }
    }

    public override void Evaluate(Evaluator evaluator)
    {
        MelodyNode melodyNode = ChordNode.ChordsNode.MelodyNode;
        Melody melody = melodyNode.Melody;

        var scale = melodyNode.Melody0.Scale;

        if (scale == null)
        {
            throw new Exception($"Melody '{melodyNode.Id}' has no scale defined");
        }

        int degree = RomanNumeralHelper.ToDegree(Numeral);

        Pitch pitch = scale.Degrees[(degree - 1) % scale.Degrees.Count];

        // 🔥 Create Note exactly like NoteNode does
        Note note = new Note
        {
            StartBeat = ChordNode.StartBeat.Value,
            EndBeat = ChordNode.EndBeat.Value,
            Pitch = pitch
        };

        melody.Notes.Add(note);
    }
}
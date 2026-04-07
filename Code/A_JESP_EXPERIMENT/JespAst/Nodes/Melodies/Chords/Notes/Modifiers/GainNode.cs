using System.Globalization;
using JespRuntime;
using JespRuntime.Nodes;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Chords.Notes.Modifiers;

public class GainNode(Node parent) : Node(parent)
{
	public float Volume = 1;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Float, (value) => Volume = float.Parse(value, CultureInfo.InvariantCulture));
	}

	protected override void Annotate(HashSet<(Type, string)> localSymbolTable)
	{
		if (Volume < 0.0f)
		{
			AddSemanticError($"Volume cannot be negative, but was: {Volume}");
		}
	}

	protected override void Evaluate(LocalVariables localSymbolTable)
	{
		Note note = localSymbolTable.Get<Note>();
		note.Volume = Volume;
	}
}


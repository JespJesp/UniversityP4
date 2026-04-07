using System.Globalization;
using JespAst.Tables;
using JespRuntime.Nodes;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Chords.Notes.Modifiers;

public class PanNode(Node parent) : Node(parent)
{
	public float Pan = 0;

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.Float, (value) => Pan = float.Parse(value, CultureInfo.InvariantCulture));
	}

	protected override void Annotate(NodeTable localNodes, SymbolTable localSymbols)
	{
		if (Pan < -1.0f || Pan > 1.0f)
		{
			AddSemanticError($"Pan must be between -1 and 1, but was: {Pan}");
		}
	}

	protected override void Evaluate(NodeTable localNodes, VariableTable localVariables)
	{
		Note note = localSymbolTable.Get<Note>();
		note.Pan = Pan;
	}
}


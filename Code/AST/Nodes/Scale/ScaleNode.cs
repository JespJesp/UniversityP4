using Phases.Parsing;
using Phases.Validation;
using Phases.Annotation;
using Runtime.Objects;

namespace Ast.Nodes.Scales;

public class ScaleNode : SymbolNode
{
    public Scale Scale = new();
    private string _root = "";
    private List<string> _notes = new();

    public override void CascadeParse(Parser parser)
    {
        parser.ConsumeToken(TokenType.Identifier, out this.Id);
        parser.ConsumeToken(TokenType.Identifier, out _root);

        while (parser.CursorToken.Type == TokenType.Identifier)
        {
            parser.ConsumeToken(TokenType.Identifier, out string note);
            _notes.Add(note);
        }
    }

    public override void Annotate(Annotator annotator)
    {
        // register scale in symbol table (automatic via SymbolNode)
    }

    public override void Evaluate(Evaluator evaluator)
    {
        Scale.Root = _root;
		  Scale.Degrees.Clear();
        foreach (var note in _notes)
        {
            Scale.Degrees.Add(Pitch.FromString(note));
        }
		  evaluator.Variables.Set(Id, Scale);
    }
}
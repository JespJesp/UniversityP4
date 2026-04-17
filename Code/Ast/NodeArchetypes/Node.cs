using Phases.Annotation;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;

namespace Ast.NodeArchetypes;

public abstract class Node
{
	public int ScopeDepth;
	public int Column;
	public int Line;
	public List<Node> Children = new();
	public SymbolTable SymbolTable = new();
	public bool CreatesNestedScope;		

	public abstract void CascadeParse(Parser parser);
	public virtual void Annotate(Annotator annotator) { }
	public virtual void Validate(Validator validator) { }
	public virtual void Evaluate(Evaluator evaluator) { }
}


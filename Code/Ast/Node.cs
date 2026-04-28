using Phases.Annotation;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;

namespace Ast;

public abstract class Node
{
	public int ScopeDepth;
	public Location Location = new();
	public List<Node> Children = new();
	public SymbolTable SymbolTable = new();
	public bool CreatesNestedScope;

	public abstract void CascadeParse(Parser parser);
	public virtual void Annotate(Annotator annotator) { }
	public virtual void Validate(Validator validator) { }
	public virtual void Evaluate(Evaluator evaluator) { }
}


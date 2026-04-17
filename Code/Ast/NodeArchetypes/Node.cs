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

	public abstract void CascadeParse();
	public virtual void Annotate() { }
	public virtual void Validate() { }
	public virtual void Evaluate() { }
}


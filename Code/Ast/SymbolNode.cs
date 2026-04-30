using Phases.Annotation;

namespace Ast;

public abstract class SymbolNode : Node
{
	public string Id = "";

	public sealed override void Annotate(Annotator annotator)
	{
		SymbolTable.Upsert(this, Id, ScopeDepth);
		AdditionalAnnotation(annotator);
	}

	public virtual void AdditionalAnnotation(Annotator annotator) {}
}


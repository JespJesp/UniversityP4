using Phases.Annotation;

namespace Ast;

public abstract class SymbolNode : Node
{
	public string Id = "";

	public sealed override void Annotate(Annotator annotator)
	{
		UpsertSymbol(annotator);
		AfterSymbolUpsert(annotator);
	}

	public virtual void UpsertSymbol(Annotator annotator)
	{
		SymbolTable.Upsert(this, Id, ScopeDepth);
	}

	public virtual void AfterSymbolUpsert(Annotator annotator)
	{
		// No behavior
	}
}


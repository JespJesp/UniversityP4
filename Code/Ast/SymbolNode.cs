using Phases.Annotation;

namespace Ast;

public abstract class SymbolNode : Node
{
	public string Id = "";

	public sealed override void Annotate(Annotator annotator)
	{
		UpsertToSymbolTable();
	}

	private void UpsertToSymbolTable()
	{
		(Type type, string id) key = (this.GetType(), this.Id);

		if (SymbolTable.Symbols.TryGetValue(key, out SymbolNode? oldEntry))
		{
			if (this.ScopeDepth <= oldEntry.ScopeDepth)
			{
				throw new Exception($"ID: '{this.Id}'. Scope depth: '{this.ScopeDepth}'. Double declaration within the same scope depth level.");
			}
		}

		SymbolTable.Symbols[key] = this;
	}
}


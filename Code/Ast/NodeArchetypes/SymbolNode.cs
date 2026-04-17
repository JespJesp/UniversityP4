namespace Ast.NodeArchetypes;

public abstract class SymbolNode : BranchNode
{
	public string Id = "";

	protected sealed override void Annotate()
	{
		UpsertToSymbolTable();
	}

	private void UpsertToSymbolTable()
	{
		(Type type, string id) key = (this.GetType(), this.Id);

		if (_symbolTable.Symbols.TryGetValue(key, out SymbolNode? oldEntry))
		{
			if (this.ScopeDepth <= oldEntry.ScopeDepth)
			{
				throw new Exception($"ID: '{this.Id}'. Scope depth: '{this.ScopeDepth}'. Double declaration within the same scope depth level.");
			}
		}

		_symbolTable.Symbols[key] = this;
	}
}


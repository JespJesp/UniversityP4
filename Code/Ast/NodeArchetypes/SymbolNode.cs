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
		SymbolTable.SymbolKey key = new(this.GetType(), this.Id);

		if (_symbolTable.Symbols.TryGetValue(key, out SymbolTable.SymbolValue? oldEntry))
		{
			if (this.ScopeDepth <= oldEntry.ScopeDepth)
			{
				Annotator.AddError(this, $"ID: '{this.Id}'. Scope depth: '{this.ScopeDepth}'. Double declaration within the same scope level.");
			}
		}

		SymbolTable.SymbolValue newValue = new(this, this.ScopeDepth);
		_symbolTable.Symbols[key] = newValue;
	}
}


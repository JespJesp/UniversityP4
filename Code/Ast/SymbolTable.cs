namespace Ast;

public class SymbolTable
{
	private Dictionary<string, SymbolNode> _symbols = new();

	public SymbolTable Clone()
	{
		return new()
		{
			_symbols = new(this._symbols)
		};
	}

	public bool Contains<T>(string id) where T : SymbolNode
	{
		if (_symbols.TryGetValue(id, out var symbol))
		{
			return symbol is T;
		}
		return false;
	}

	public T Get<T>(string id) where T : SymbolNode
	{
		if (_symbols.TryGetValue(id, out var symbol))
		{
			if (symbol is T)
			{
				return (T)symbol;
			}

			throw new Exception($"Internal error: The symbol at the symbol table ID '{id}' is not of the expected type '{typeof(T)}', but instead of the type '{symbol.GetType()}' - this should have been checked in the annotation phase");
		}

		throw new Exception($"Internal error: Cannot get symbol of type '{typeof(T)}' and ID '{id}' from symbol table because the ID does not exist - this should have been checked in the annotation phase");
	}

	public bool TryGet<T>(string id, out T value) where T : SymbolNode
	{
		if (_symbols.TryGetValue(id, out var symbol))
		{
			if (symbol is T)
			{
				value = (T)symbol;
				return true;
			}
		}

		// The "!" (null-forgiving operator) hides the warning "cannot convert null literal to non-nullable reference type". We can safely do this because you should never use the out parameter value if this method returns false.
		value = default!;
		return false;
	}

	public void Upsert(SymbolNode node, string id, int scopeDepth = 0)
	{
		if (_symbols.TryGetValue(id, out var oldEntry))
		{
			if (scopeDepth <= oldEntry.ScopeDepth)
			{
				throw new Exception($"ID: '{id}'. Scope depth: '{scopeDepth}'. Double declaration within the same scope depth level");
			}
		}

		_symbols[id] = node;
	}
}
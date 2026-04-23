namespace Ast;

public class SymbolTable
{
	public Dictionary<(Type type, string id), SymbolNode> Symbols = new();

	public SymbolTable Clone()
	{
		return new()
		{
			Symbols = new(this.Symbols)
		};
	}

	public bool Contains<T>(string id) where T : Node
	{
		return Symbols.ContainsKey((typeof(T), id));
	}

	public T Get<T>(string id) where T : SymbolNode
	{
		T result = (T)Symbols[(typeof(T), id)];

		if (result == null)
		{
			throw new Exception($"Internal error: Cannot get symbol of type '{typeof(T)}' and id '{id}' from symbol table because the symbol does not exist - this should have been checked in the annotation phase");
		}

		return result;
	}

	public bool TryGet<T>(string id, out T value) where T : SymbolNode
	{
		if (Symbols.TryGetValue((typeof(T), id), out SymbolNode? output))
		{
			value = (T)output;
			return true;
		}
		else
		{
			// The "!" (null-forgiving operator) hides the warning "cannot convert null literal to non-nullable reference type". We can safely do this because you should never use the out parameter value if this method returns false.
			value = default!;
			return false;
		}
	}
}
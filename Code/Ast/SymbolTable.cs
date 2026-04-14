using Ast.NodeArchetypes;

namespace Ast;

public class SymbolTable
{
	public record SymbolKey
	(
		Type Type,
		string Id
	);
	public record SymbolValue
	(
		SymbolNode node,
		int ScopeDepth
	);
	public Dictionary<SymbolKey, SymbolValue> Symbols = new();

	public SymbolTable Clone()
	{
		return new()
		{
			Symbols = new(this.Symbols)
		};
	}

	public bool Contains<T>(string id) where T : Node
	{
		SymbolKey key = new(typeof(T), id);
		return Symbols.ContainsKey(key);
	}

	public T Get<T>(string id) where T : SymbolNode
	{
		SymbolKey key = new(typeof(T), id);
		T result = (T)Symbols[key].node;

		if (result == null)
		{
			throw new Exception($"Internal error: Cannot get variable of type '{typeof(T)}' and id '{id}' from variable table because the variable does not exist.");
		}

		return result;
	}

	public bool TryGet<T>(string id, out T value) where T : SymbolNode
	{
		SymbolKey key = new(typeof(T), id);
		if (Symbols.TryGetValue(key, out SymbolValue? output))
		{
			value = (T)output.node;
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}
}
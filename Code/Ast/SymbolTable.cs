using Ast.NodeArchetypes;

namespace Ast;

public class SymbolTable
{
	record SymbolKey
	(
		Type Type,
		string Id
	);
	record SymbolValue
	(
		SymbolNode node,
		int ScopeDepth
	);
	private Dictionary<SymbolKey, SymbolValue> _symbols = new();

	public SymbolTable Clone()
	{
		return new()
		{
			_symbols = new(this._symbols)
		};
	}

	public bool Contains<T>(string id) where T : Node
	{
		SymbolKey key = new(typeof(T), id);
		return _symbols.ContainsKey(key);
	}

	public T Get<T>(string id) where T : SymbolNode
	{
		SymbolKey key = new(typeof(T), id);
		T result = (T)_symbols[key].node;

		if (result == null)
		{
			throw new Exception($"Internal error: Cannot get variable of type '{typeof(T)}' and id '{id}' from variable table because the variable does not exist.");
		}

		return result;
	}

	public bool TryGet<T>(string id, out T value) where T : SymbolNode
	{
		SymbolKey key = new(typeof(T), id);
		if (_symbols.TryGetValue(key, out SymbolValue output))
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

	public void Upsert(SymbolNode symbolNode)
	{
		SymbolKey key = new(symbolNode.GetType(), symbolNode.Id);
		if (_symbols.TryGetValue(key, out SymbolValue oldEntry))
		{
			if (symbolNode.ScopeDepth <= oldEntry.ScopeDepth)
			{
				Validator.AddError(symbolNode, $"ID: '{symbolNode.Id}'. Scope depth: '{symbolNode.ScopeDepth}'. Double declaration within the same scope level.");
			}
		}

		SymbolValue newValue = new(symbolNode, symbolNode.ScopeDepth);
		_symbols[key] = newValue;
	}
}
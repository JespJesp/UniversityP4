namespace JespAst.Tables;

public class SymbolTable
{
	// TOOD: Add exception messages and handling

	private HashSet<(Type, string)> _symbols = new();

	public SymbolTable Clone()
	{
		return new()
		{
			_symbols = new(this._symbols)
		};
	}

	public bool Contains(Type type, string id)
	{
		return _symbols.Contains((type, id));
	}

	public void Add(Type type, string id)
	{
		_symbols.Add((type, id));
	}

}
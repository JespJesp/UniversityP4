namespace Ast.Tables;

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
		// TODO: Throw semantic error if you try to overwrite a symbol inside of the same scope level
		// (because then it isn't a shadow variable, but a double declaration - which is erroneous)

		_symbols.Add((type, id));
	}

}
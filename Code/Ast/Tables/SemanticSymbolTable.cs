using Ast.Nodes;

namespace Ast.Tables;

public class SemanticSymbolTable
{
	/// <summary>
	/// Key: node type and id.
	/// Value: scope depth.
	/// </summary>
	private Dictionary<(Type, string), int> _symbols = new();

	public SemanticSymbolTable Clone()
	{
		return new()
		{
			_symbols = new(this._symbols)
		};
	}

	public bool Contains(Type type, string id)
	{
		return _symbols.ContainsKey((type, id));
	}

	public void Add(Node node, string id)
	{
		// TODO: Throw semantic error if you try to overwrite a symbol inside of the same scope level
		// (because then it isn't a shadow variable, but a double declaration - which is erroneous)

		Console.WriteLine("---------"); // REMOVE THIS	
		Console.WriteLine($"ID: {id}, Type: {node.GetType()}"); // REMOVE THIS	

		if (_symbols.TryGetValue((node.GetType(), id), out int oldScope))
		{
			Console.WriteLine("Found"); // REMOVE THIS	
			if (node.Scope <= oldScope)
			{
				Console.WriteLine("=============== SCOPE IS THE SAME!"); // REMOVE THIS	
				Annotator.AddSemanticError(node, $"ID: '{id}'. Scope depth: '{node.Scope}'. Double declaration within the same scope level.");
			}
		}

		_symbols[(node.GetType(), id)] = node.Scope;
	}

}